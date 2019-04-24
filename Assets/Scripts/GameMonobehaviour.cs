using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMonobehaviour : MonoBehaviour {
	public int numPlayers;
	public float gridSize = 0.6f;


	GameObject playerOne;

	void Start() {
		playerOne = GameObject.Find("Player One");
	}

	void Update() {
		var translation = Vector2.zero;

		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			translation.x = gridSize;
		} else if(Input.GetKeyUp(KeyCode.LeftArrow)) {
			translation.x = -gridSize;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow)) {
			translation.y = gridSize;
		} else if(Input.GetKeyUp(KeyCode.DownArrow)) {
			translation.y = -gridSize;
		}

		playerOne.transform.Translate(translation);
	}
}
