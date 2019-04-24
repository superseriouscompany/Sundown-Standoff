using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMonobehaviour : MonoBehaviour {
	public int gridSize = 5;
	public int startingHp = 3;
	public float gridSquareWidth = 0.6f;
	public GameObject gridSquare;

	public bool diagonalShots;
	public bool diagonalMove;
	public bool moveBeatsShot;
	public bool minimum2;
	public bool incrementalResolution;
	public bool doubleShot;
	public bool obstacles;
	public bool mines;

	public Player[] players = new Player[2];
	GameObject playerTwo;

	float start;
	float end;
	float center;

	Vector2Int[] moves = new Vector2Int[2];
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

	void Update() {
		for (int i = 0; i < players.Length; i++) {
			if (players[i].targetPosition != players[i].position) {
				players[i].position = players[i].targetPosition;
				players[i].gameObject.transform.position = GridToWorld(players[i].targetPosition);
			}
		}

		var position = new Vector2Int();
		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			position.x++;
		} else if (Input.GetKeyUp(KeyCode.LeftArrow)) {
			position.x--;
		} else if (Input.GetKeyUp(KeyCode.UpArrow)) {
			position.y++;
		} else if (Input.GetKeyUp(KeyCode.DownArrow)) {
			position.y--;
		} else {
			return;
		}

		moves[moveIndex++] = position;
		if (moveIndex >= moves.Length) {
			for (int i = 0; i < moves.Length; i++) {
				players[i].Move(moves[i]);
				players[i].targetPosition.x = Mathf.Clamp(players[i].targetPosition.x, 0, gridSize - 1);
				players[i].targetPosition.y = Mathf.Clamp(players[i].targetPosition.y, 0, gridSize - 1);
			}
			moveIndex = 0;
		}
	}
}
