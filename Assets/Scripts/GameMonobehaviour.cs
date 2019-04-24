using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMonobehaviour : MonoBehaviour {
	public int gridSize = 5;
	public float gridSquareWidth = 0.6f;
	public GameObject gridSquare;

	GameObject playerOne;
	GameObject playerTwo;

	void Start() {
		playerOne = GameObject.Find("Player One");
		playerTwo = GameObject.Find("Player Two");

		var start = gridSize * -gridSquareWidth / 2;

		playerOne.transform.position = new Vector2(start, start + gridSquareWidth * 2);
		playerTwo.transform.position = new Vector2(-start - 1, start + gridSquareWidth * 2);

		for (int i = 0; i < gridSize; i++) {
			for (int j = 0; j < gridSize; j++) {
				var square = Instantiate(gridSquare);
				square.transform.position = new Vector2(start + i * gridSquareWidth, start + j * gridSquareWidth);
			}
		}
	}

	void Update() {
		var translation = Vector2.zero;

		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			translation.x = gridSquareWidth;
		} else if (Input.GetKeyUp(KeyCode.LeftArrow)) {
			translation.x = -gridSquareWidth;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow)) {
			translation.y = gridSquareWidth;
		} else if (Input.GetKeyUp(KeyCode.DownArrow)) {
			translation.y = -gridSquareWidth;
		}

		playerOne.transform.Translate(translation);
	}
}
