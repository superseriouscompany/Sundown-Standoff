using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player {
	public int id;
	public Vector2Int position;
	public Vector2Int targetPosition;
	public GameObject gameObject;

	public Player(int id, GameObject gameObject, Vector2Int position) {
		this.id = id;
		this.gameObject = gameObject;
		this.position = position;
		targetPosition = position;
	}

	public void Move(int horizontal, int vertical) {
		Move(new Vector2Int(horizontal, vertical));
	}

	public void Move(Vector2Int vector) {
		targetPosition = position + vector;
	}
}
