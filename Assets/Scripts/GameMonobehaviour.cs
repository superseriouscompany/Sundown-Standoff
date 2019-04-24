using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMonobehaviour : MonoBehaviour {
	public int gridSize = 5;
	public float gridSquareWidth = 0.6f;

	GameObject playerOne;
	GameObject playerTwo;

	void Start() {
		playerOne = GameObject.Find("Player One");
		playerTwo = GameObject.Find("Player Two");

		playerOne.transform.position = new Vector2(gridSize * -gridSquareWidth / 2,0);
		playerTwo.transform.position = new Vector2(gridSize * gridSquareWidth / 2,0);
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
