using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactiveUI;

public class GameMonobehaviour : MonoBehaviour {
	public GameObject playerPrefab;
	public GameObject gridSquare;
	public float turnDelay = 1f;
	public float moveSpeed = 1f;
	public float gridSquareWidth = 0.6f;

	Player[] players = new Player[2];
	Grid grid;

	void Start() {
		players[0] = new Player(0, Rules.instance.hp, new Vector2Int(0, Rules.instance.gridSize / 2));
		players[1] = new Player(1, Rules.instance.hp, new Vector2Int(Rules.instance.gridSize - 1, Rules.instance.gridSize / 2));

		for (int i = 0; i < players.Length; i++) {
			var playerObject = Instantiate(playerPrefab);
			playerObject.transform.position = GridToWorld(players[i].position);
			playerObject.transform.localScale = new Vector3(2 * gridSquareWidth, 2 * gridSquareWidth, 2 * gridSquareWidth);
			if (i == 1) {
				playerObject.GetComponent<SpriteRenderer>().flipX = true;
			}
			players[i].gameObject = playerObject;
			UIDispatcher.Send(new DSUI.SetPlayer() { player = players[i] });
		}

		var gridCnr = new GameObject("Grid");
		for (int i = 0; i < Rules.instance.gridSize; i++) {
			for (int j = 0; j < Rules.instance.gridSize; j++) {
				var square = Instantiate(gridSquare);
				square.transform.position = GridToWorld(i, j);
				square.transform.localScale = new Vector3(7 * gridSquareWidth, 7 * gridSquareWidth, 7 * gridSquareWidth);
				square.transform.parent = gridCnr.transform;
			}
		}

		grid = new Grid() { gridSize = Rules.instance.gridSize };
		UIDispatcher.Send(new DSUI.SetPhaseAction() { phase = Phase.CARDS });
		UIDispatcher.Send(new DSUI.SetTurnAction() { turn = 0 });
	}

	Vector2 GridToWorld(int x, int y) {
		return GridToWorld(new Vector2Int(x, y));
	}

	Vector2 GridToWorld(Vector2Int vector) {
		return new Vector2(
			vector.x * gridSquareWidth - (Rules.instance.gridSize * gridSquareWidth / 2) + gridSquareWidth / 2,
			vector.y * gridSquareWidth - (Rules.instance.gridSize * gridSquareWidth / 2) + gridSquareWidth / 2 - 1
		);
	}

	int cardSelectionIndex;
	void Update() {
		if (Input.GetKeyUp(KeyCode.R)) {
			StartCoroutine(Restart());
			return;
		}

		if (cardSelectionIndex < players.Length) {
			int numActions = 0;
			if (Input.GetKeyUp(KeyCode.Alpha1)) {
				numActions = 1;
			} else if (Input.GetKeyUp(KeyCode.Alpha2)) {
				numActions = 2;
			} else if (Input.GetKeyUp(KeyCode.Alpha3)) {
				numActions = 3;
			}

			if (numActions == 0) { return; }
			try {
				players[cardSelectionIndex].PickCard(numActions);
				cardSelectionIndex++;
				UIDispatcher.Send(new DSUI.RenderAction());

				UIDispatcher.Send(new DSUI.SetTurnAction() { turn = cardSelectionIndex });
				if (cardSelectionIndex >= players.Length) {
					UIDispatcher.Send(new DSUI.SetTurnAction() { turn = 0 });
					UIDispatcher.Send(new DSUI.SetPhaseAction() { phase = Phase.ACTIONS });

					for (int i = 0; i < players.Length; i++) {
						players[i].Discard();
					}
				}
			} catch (CardMissingException) { }

			return;
		}

		Action action;
		if (Input.GetKeyDown(KeyCode.Space)) {
			action = new Action() { actionType = ActionType.SHOOT };
		} else if (Input.GetKeyDown(KeyCode.Return)) {
			action = new Action() { actionType = ActionType.MOVE };
		} else {
			return;
		}

		action.direction = new Vector2Int();
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			action.direction.x++;
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
			action.direction.x--;
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			action.direction.y++;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
			action.direction.y--;
		}

		action.player = players[0].isReady ? players[1] : players[0];
		if (!grid.Validate(action)) {
			Log("Action rejected");
			return;
		}

		action.player.AddAction(action);

		UIDispatcher.Send(new DSUI.SetTurnAction() { turn = players[0].isReady ? 1 : 0 });

		bool allReady = true;
		List<Action> actions = new List<Action>();
		for (int i = 0; i < players.Length; i++) {
			if (!players[i].isReady) { allReady = false; }
			for (int j = 0; j < players[i].actions.Count; j++) {
				actions.Add(players[i].actions[j]);
			}
		}
		if (!allReady) { return; }

		StartCoroutine(ResolveActions(actions));
	}

	IEnumerator SmoothMove(Player p) {
		var originSquare = p.position;
		var origin = GridToWorld(p.position);
		var destination = GridToWorld(p.targetPosition);
		var distance = (destination - origin).magnitude;
		var startTime = Time.time;
		var animator = p.gameObject.GetComponent<Animator>();
		animator.SetBool("Move", true);
		float deltaTime;
		do {
			deltaTime = Time.time - startTime;
			p.gameObject.transform.position = Vector3.Lerp(origin, destination, (Time.time - startTime) * moveSpeed);
			yield return null;
		} while (deltaTime < distance / moveSpeed);
		animator.SetBool("Move", false);
		p.gameObject.transform.position = destination;

		if (p.bounceBack) {
			p.targetPosition = originSquare;
			p.bounceBack = false;
			yield return StartCoroutine(SmoothMove(p));
			p.position = p.targetPosition;
		}
		coroutineCount--;
		yield return null;
	}

	IEnumerator Restart() {
		yield return new WaitForSeconds(1);
		for (int i = 0; i < players.Length; i++) {
			Destroy(players[i].gameObject);
		}
		Start();
	}

	int coroutineCount = 0;
	IEnumerator ResolveActions(List<Action> actions) {
		var resolver = new Resolver(grid, players, actions);

		while (!resolver.isComplete) {
			resolver.Step();
			foreach(var player in players) {
				Debug.Log($"Checking player: {player.id} {player.position} {player.targetPosition}");

				if (player.position != player.targetPosition) {
					StartCoroutine(SmoothMove(player));
					coroutineCount++;
					player.position = player.targetPosition;
				}
			}

			while (coroutineCount > 0) {
				yield return null;
			}
			yield return new WaitForSeconds(turnDelay);
		}

		cardSelectionIndex = 0;
		UIDispatcher.Send(new DSUI.SetPhaseAction() { phase = Phase.CARDS });
		UIDispatcher.Send(new DSUI.SetTurnAction() { turn = 0 });

		foreach (var player in players) {
			if (player.hp <= 0) {
				yield return StartCoroutine(Restart());
			}
		}
	}

	void Log(string msg) {
		Debug.Log(msg);
		UIDispatcher.Send(new DSUI.SetMessageAction() { message = msg });
	}
}
