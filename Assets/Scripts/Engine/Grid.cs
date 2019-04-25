using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
	public int gridSize;

	public bool Validate(Action action) {
		if (action.direction.x == 0 && action.direction.y == 0) { return false; }

		switch(action.actionType) {
			case ActionType.MOVE:
				var targetPosition = action.player.position + action.direction;
				return IsValidSquare(targetPosition);
			default:
				return true;
		}
	}

	public List<Vector2Int> Raycast(Action action) {
		var origin = action.player.position;
		var squares = new List<Vector2Int>();

		for (var nextSquare = origin + action.direction; IsValidSquare(nextSquare); nextSquare += action.direction) {
			squares.Add(nextSquare);
		}

		return squares;
	}

	bool IsValidSquare(Vector2Int vector) {
		return vector.x >= 0 && vector.x < gridSize
			&& vector.y >= 0 && vector.y < gridSize;
	}
}
