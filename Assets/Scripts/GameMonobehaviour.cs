using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMonobehaviour : MonoBehaviour {
	public int gridSize = 5;
	public int startingHp = 3;
	public float gridSquareWidth = 0.6f;
	public GameObject gridSquare;

	public float speed = 1f;

	public bool diagonalShots;
	public bool diagonalMove;
	public bool moveBeatsShot;
	public bool minimum2;
	public bool incrementalResolution;
	public bool doubleShot;
	public bool obstacles;
	public bool mines;

	Player[] players = new Player[2];
	Grid grid;
	GameObject playerTwo;

	float start;
	float end;
	float center;

	Action[] moves = new Action[2];
	int moveIndex;

	void Start() {
		players[0] = new Player(0, GameObject.Find("Player One"), new Vector2Int(0, gridSize / 2));
		players[1] = new Player(1, GameObject.Find("Player Two"), new Vector2Int(gridSize - 1, gridSize / 2));

		for (int i = 0; i < players.Length; i++) {
			players[i].gameObject.transform.position = GridToWorld(players[i].targetPosition);
		}

		start = gridSize * -gridSquareWidth / 2;
		end = -start - 1;
		center = start + gridSquareWidth * 2;

		for (int i = 0; i < gridSize; i++) {
			for (int j = 0; j < gridSize; j++) {
				var square = Instantiate(gridSquare);
				square.transform.position = GridToWorld(i, j);
			}
		}

		grid = new Grid() { gridSize = gridSize };
	}

	Vector2 GridToWorld(int x, int y) {
		return GridToWorld(new Vector2Int(x, y));
	}

	Vector2 GridToWorld(Vector2Int vector) {
		return new Vector2(
			vector.x * gridSquareWidth - 2.5f + gridSquareWidth / 2,
			vector.y * gridSquareWidth - 2.5f + gridSquareWidth / 2
		);
	}

	Action action;
	void Update() {
		if (Input.GetKeyUp(KeyCode.Space)) {
			action = new Action() { actionType = ActionType.SHOOT };
		} else if (Input.GetKeyUp(KeyCode.Return)) {
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

		action.player = players[moveIndex];

		if (!grid.Validate(action)) {
			Debug.Log("Action rejected");
			return;
		}

		Debug.Log($"Action received for player {moveIndex}");
		moves[moveIndex++] = action;

		if (moveIndex >= moves.Length) {
			for (int i = 0; i < moves.Length; i++) {
				switch(moves[i].actionType) {
					case ActionType.MOVE:
						players[i].Move(moves[i].direction);
						StartCoroutine(
							SmoothMove(players[i])
						);
						break;
					case ActionType.SHOOT:
						var squares = grid.Raycast(moves[i]);
						for (int x = 0; x < squares.Count; x++) {
							for (int j = 0; j < players.Length; j++) {
								if (squares[x] == players[j].targetPosition) {
									Debug.Log($"Hit {players[j].id}!");
								}
							}
						}
						break;
				}
			}
			moveIndex = 0;
		}
	}

	IEnumerator SmoothMove(Player p) {
		var origin = GridToWorld(p.position);
		var destination = GridToWorld(p.targetPosition);
		var distance = (destination - origin).magnitude;
		var startTime = Time.time;

		float deltaTime;
		do {
			deltaTime = Time.time - startTime;
			p.gameObject.transform.position = Vector3.Lerp(origin, destination, (Time.time - startTime) * speed);
			yield return null;
		} while (deltaTime < distance / speed);
		p.gameObject.transform.position = destination;
		p.position = p.targetPosition;
		yield return null;
	}
}
