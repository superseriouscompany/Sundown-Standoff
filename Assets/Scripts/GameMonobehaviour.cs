using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactiveUI;

public class GameMonobehaviour : MonoBehaviour {
	public GameObject playerPrefab;
	public GameObject gridSquare;
	public float gridSquareWidth = 0.6f;

	public GameObject desert;

	SpriteRenderer[,] gridSquares;
	Player[] players = new Player[2];
	Grid grid;
	Resolver resolver;
	int coroutineCount;

	bool isGameOver;

	void Awake() {
		gameObject.AddComponent<Coroutines>();
	}

	void Start() {
		if (Rules.instance.debugPositions) {
			players[0] = new Player(0, Rules.instance.hp, new Vector2Int(2, Rules.instance.gridSize / 2));
			players[1] = new Player(1, Rules.instance.hp, new Vector2Int(3, Rules.instance.gridSize / 2));
		} else {
			players[0] = new Player(0, Rules.instance.hp, new Vector2Int(0, Rules.instance.gridSize / 2));
			players[1] = new Player(1, Rules.instance.hp, new Vector2Int(Rules.instance.gridSize - 1, Rules.instance.gridSize / 2));
		}


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
		gridSquares = new SpriteRenderer[Rules.instance.gridSize, Rules.instance.gridSize];
		for (int i = 0; i < Rules.instance.gridSize; i++) {
			for (int j = 0; j < Rules.instance.gridSize; j++) {
				var square = Instantiate(gridSquare);
				square.transform.position = GridToWorld(i, j);
				square.transform.localScale = new Vector3(7 * gridSquareWidth, 7 * gridSquareWidth, 7 * gridSquareWidth);
				square.transform.parent = gridCnr.transform;
				gridSquares[i, j] = square.GetComponent<SpriteRenderer>();
			}
		}

		grid = new Grid() { gridSize = Rules.instance.gridSize };
		UIDispatcher.Send(new DSUI.SetPhaseAction() { phase = Phase.CARDS });
		UIDispatcher.Send(new DSUI.SetTurnAction() { turn = 0 });

		resolver = new Resolver(grid, players);

		cardSelectionIndex = 0;
		coroutineCount = 0;
		isGameOver = false;

		desert.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
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
		if (isGameOver) { return; }

		foreach (var player in players) {
			if (player.hp <= 0) {
				Maestro.instance.PlayDeath();
				isGameOver = true;
				desert.GetComponent<SpriteRenderer>().color = new Color(0.8584906f, 0.133633f, 0.133633f);
				StartCoroutine(Restart());
				return;
			}
		}

		if (Input.GetKeyUp(KeyCode.R)) {
			StartCoroutine(Restart(0));
			return;
		}

		if (cardSelectionIndex < players.Length) {
			PickCard();
			return;
		}

		var action = ActionFromInput();
		if (action == null) { return; }

		action.player = players[UIState.singleton.turn];
		if (!grid.Validate(action)) {
			Log("Action rejected");
			return;
		}
		try {
			action.player.AddAction(action);
		} catch(NeedsDoubleShotException) { return; }

		bool allReady = true;
		List<Action> actions = new List<Action>();
		for (int i = 0; i < players.Length; i++) {
			if (!players[i].isReady) { 
				if (allReady) {
					UIDispatcher.Send(new DSUI.SetTurnAction() { turn = i });
				}
				allReady = false; 
			}
			for (int j = 0; j < players[i].actions.Count; j++) {
				if (j < players[i].actionsTaken) { continue; }
				Debug.Log($"Adding action {players[i].actions[j]}");
				actions.Add(players[i].actions[j]);
			}
		}
		if (!allReady) { return; }

		resolver.AddActions(actions);
		Debug.Log($"Running round");
		StartCoroutine(ResolveActions());
	}

	IEnumerator SmoothMove(Player p) {
		var originSquare = p.position;
		var origin = GridToWorld(p.position);
		var destination = GridToWorld(p.targetPosition);
		var distance = (destination - origin).magnitude;
		var startTime = Time.time;
		var animator = p.gameObject.GetComponent<Animator>();
		animator.SetBool("Move", true);
		Maestro.instance.PlayMovement();
		float deltaTime;
		do {
			deltaTime = Time.time - startTime;
			p.gameObject.transform.position = Vector3.Lerp(origin, destination, (Time.time - startTime) * Rules.instance.moveSpeed);
			yield return null;
		} while (deltaTime < distance / Rules.instance.moveSpeed);
		animator.SetBool("Move", false);
		p.gameObject.transform.position = destination;

		if (p.bounceBack) {
			p.targetPosition = originSquare;
			p.bounceBack = false;
			yield return StartCoroutine(SmoothMove(p));
			p.position = p.targetPosition;
		} else {
			coroutineCount--;
		}
		yield return null;
	}

	IEnumerator Restart(int seconds = 5) {
		yield return new WaitForSeconds(seconds);
		for (int i = 0; i < players.Length; i++) {
			Destroy(players[i].gameObject);
		}
		Start();
	}

	IEnumerator ResolveActions() {
		while (!resolver.isComplete) {
			resolver.StepMovement();
			foreach(var player in players) {
				if (player.position != player.targetPosition) {
					StartCoroutine(SmoothMove(player));
					coroutineCount++;
					player.position = player.targetPosition;
				}
			}

			Debug.Log($"COROUTINE COUNT IS INITIALLY {coroutineCount}");
			while (coroutineCount > 0) {
				yield return null;
			}

			Debug.Log("RESOLVING SHOTS");

			resolver.StepShots();
			foreach (var square in resolver.hitSquares) {
				gridSquares[square.x, square.y].color = new Color(1, 0.5f, 1);
			}
			yield return new WaitForSeconds(Rules.instance.turnDelay);
			foreach (var square in resolver.hitSquares) {
				gridSquares[square.x, square.y].color = new Color(1, 1, 1);
			}
		}

		bool isComplete = true;
		foreach (var player in players) {
			if (player.actionsTaken < player.card.actions) {
				if (isComplete == true) {
					UIDispatcher.Send(new DSUI.SetTurnAction() { turn = player.id });
				}
				isComplete = false;
			}
		}

		if (isComplete) {
			resolver.Reset();
			cardSelectionIndex = 0;
			UIDispatcher.Send(new DSUI.SetPhaseAction() { phase = Phase.CARDS });
			UIDispatcher.Send(new DSUI.SetTurnAction() { turn = 0 });
			foreach (var player in players) {
				player.actionsTaken = 0;
			}
		}
	}

	Action ActionFromInput() {
		Action action;
		if (Input.GetKeyDown(KeyCode.Space)) {
			action = new Action() { actionType = ActionType.SHOOT };
		} else if (Input.GetKeyDown(KeyCode.Return)) {
			action = new Action() { actionType = ActionType.MOVE };
		} else {
			return null;
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
		return action;
	}

	void PickCard() {
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
				coroutineCount = 0;
				for (int i = 0; i < players.Length; i++) {
					players[i].Discard();
				}
			}
		} catch (CardMissingException) { }
	}

	void Log(string msg) {
		Debug.Log(msg);
		UIDispatcher.Send(new DSUI.SetMessageAction() { message = msg });
	}
}