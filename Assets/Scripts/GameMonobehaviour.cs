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
	Phase phase = Phase.Cards;

	KeyMapping[] keyMappings = new KeyMapping[] {
		new KeyMapping() {
			directions = new KeyCode[] {
				KeyCode.W,
				KeyCode.D,
				KeyCode.S,
				KeyCode.A
			},
			cards = new KeyCode[] {
				KeyCode.Alpha1,
				KeyCode.Alpha2,
				KeyCode.Alpha3,
				KeyCode.Alpha4
			},
			move = KeyCode.LeftShift,
			shoot = KeyCode.LeftControl,
			reload = KeyCode.LeftAlt
		},
		new KeyMapping() {
			directions = new KeyCode[] {
				KeyCode.UpArrow,
				KeyCode.RightArrow,
				KeyCode.DownArrow,
				KeyCode.LeftArrow
			},
			cards = new KeyCode[] {
				KeyCode.Alpha7,
				KeyCode.Alpha8,
				KeyCode.Alpha9,
				KeyCode.Alpha0
			},
			move = KeyCode.Alpha7,
			shoot = KeyCode.Alpha8,
			reload = KeyCode.Alpha9
		}
	};

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
		UIDispatcher.Send(new DSUI.SetPhaseAction(phase));
		UIDispatcher.Send(new DSUI.SetTurnAction() { turn = 0 });

		resolver = new Resolver(grid, players);

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

		if (phase == Phase.Cards) {
			PickCards();
			return;
		}

		if (phase == Phase.Effects) {
			foreach (var player in players) {
				ProcessEffect(player);
			}
			GoToActionPhase();
		}

		var actions = Action.FromInput(players, keyMappings);
		if (actions.Count == 0) { return; }

		foreach (var action in actions) {
			ProcessAction(action);
		}

		bool allReady = true;
		actions.Clear();
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

	void ProcessAction(Action action) {
		if (!grid.Validate(action)) {
			Log("Action rejected");
			return;
		}
		if (!action.Validate()) {
			Log("Action invalid");
			return;
		}

		action.player.AddAction(action);
	}

	void ProcessEffect(Player player) {
		var card = player.card;
		Player opponent = GetOtherPlayer(player);
		switch (card.effect) {
			case Effect.Heal:
				player.hp++;
				break;
			case Effect.TeleportCenter:
				var center = new Vector2Int(
					Rules.instance.gridSize / 2,
					Rules.instance.gridSize / 2
				);
				player.position = center;
				player.targetPosition = center;
				player.gameObject.transform.position = GridToWorld(center);
				break;
			case Effect.Steal:
				if (opponent.card.actions == 0) { return; }
				if (opponent.card.effect == Effect.Curse) { return; }
				player.actionModifier++;
				opponent.actionModifier--;
				break;
			case Effect.Curse:
				if (opponent.card.effect == Effect.Steal) { return; }
				player.actionModifier += opponent.card.actions - player.card.actions;
				opponent.actionModifier += player.card.actions - opponent.card.actions;
				break;
		}
	}

	Player GetOtherPlayer(Player player) {
		return player.id == 0 ? players[1] : players[0];
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

			while (coroutineCount > 0) {
				yield return null;
			}

			resolver.StepReload();

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
			if (player.actionsTaken < player.actionsAvailable) {
				if (isComplete == true) {
					UIDispatcher.Send(new DSUI.SetTurnAction() { turn = player.id });
				}
				isComplete = false;
			}
		}

		if (isComplete) {
			foreach (var player in players) {
				player.Discard();
			}
			GoToCardsPhase();
		}
	}

	void PickCards() {
		for (int i = 0; i < players.Length; i++) {
			var keys = keyMappings[i];

			int index = -1;
			if (Input.GetKeyUp(keys.cards[0])) {
				index = 0;
			} else if (Input.GetKeyUp(keys.cards[1])) {
				index = 1;
			} else if (Input.GetKeyUp(keys.cards[2])) {
				index = 2;
			} else if (Input.GetKeyUp(keys.cards[3])) {
				index = 3;
			}

			if (index == -1) { continue; }

			try {
				players[i].PickCard(index);
				UIDispatcher.Send(new DSUI.RenderAction());
			} catch (CardMissingException) { }
		}

		bool isReady = true;
		foreach (var player in players) {
			if (player.card == null) {
				isReady = false;
				break;
			}
		}

		if (isReady) {
			GoToEffectsPhase();
		}
		coroutineCount = 0;
	}

	void GoToCardsPhase() {
		resolver.Reset();
		phase = Phase.Cards;
		UIDispatcher.Send(new DSUI.SetPhaseAction(phase));
		UIDispatcher.Send(new DSUI.SetTurnAction() { turn = 0 });
		foreach (var player in players) {
			player.actionsTaken = 0;
			player.card = null;
		}
	}

	void GoToEffectsPhase() {
		phase = Phase.Effects;
		UIDispatcher.Send(new DSUI.SetPhaseAction(phase));
	}

	void GoToActionPhase() {
		phase = Phase.Actions;
		UIDispatcher.Send(new DSUI.SetPhaseAction(phase));
	}

	void Log(string msg) {
		Debug.Log(msg);
		UIDispatcher.Send(new DSUI.SetMessageAction() { message = msg });
	}
}