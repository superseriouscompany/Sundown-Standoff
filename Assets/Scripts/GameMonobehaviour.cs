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

	GameObject playerOne;
	GameObject playerTwo;

	float start;
	float end;
	float center;

	void Start() {
		playerOne = GameObject.Find("Player One");
		playerTwo = GameObject.Find("Player Two");

		start = gridSize * -gridSquareWidth / 2;
		end = -start - 1;
		center = start + gridSquareWidth * 2;

		playerOne.transform.position = new Vector2(start, center);
		playerTwo.transform.position = new Vector2(end, center);

		for (int i = 0; i < gridSize; i++) {
			for (int j = 0; j < gridSize; j++) {
				var square = Instantiate(gridSquare);
				square.transform.position = new Vector2(start + i * gridSquareWidth, start + j * gridSquareWidth);
			}
		}
	}

	void Update() {
		var position = playerOne.transform.position;

		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			position.x += gridSquareWidth;
		} else if (Input.GetKeyUp(KeyCode.LeftArrow)) {
			position.x -= gridSquareWidth;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow)) {
			position.y += gridSquareWidth;
		} else if (Input.GetKeyUp(KeyCode.DownArrow)) {
			position.y -= gridSquareWidth;
		}

		position.x = Mathf.Clamp(position.x, start, end);
		position.y = Mathf.Clamp(position.y, start, end);

		playerOne.transform.position = position;
	}
}
