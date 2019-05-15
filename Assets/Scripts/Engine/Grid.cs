using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
	public int gridSize;

	public bool Validate(Action action) {
		var hasMovement = action.direction.x != 0 || action.direction.y != 0;
		var isDiagonal = action.direction.x != 0 && action.direction.y != 0;

		switch(action.actionType) {
			case ActionType.MOVE:
				if (!hasMovement) { return false; }
				var targetPosition = ExpectedPosition(action) + action.direction;
				return IsValidSquare(targetPosition);
			case ActionType.SHOOT:
				return true;
			default:
				return true;
		}
	}

	Vector2Int[] directions = new Vector2Int[] {
		new Vector2Int(0,1),
		new Vector2Int(1,1),
		new Vector2Int(1,0),
		new Vector2Int(1,-1),
		new Vector2Int(0,-1),
		new Vector2Int(-1, -1),
		new Vector2Int(-1,0),
		new Vector2Int(-1,1)
	};

	public List<Vector2Int> Raycast(Action action) {
		var origin = action.player.position;
		var squares = new List<Vector2Int>();

		foreach (var direction in directions) {
			for (var nextSquare = origin + direction; IsValidSquare(nextSquare); nextSquare += direction) {
				squares.Add(nextSquare);
			}
		}

		return squares;
	}

	Vector2Int ExpectedPosition(Action action) {
		var position = action.player.position;
		foreach(var a in action.player.actions) {
			if (a.actionType == ActionType.MOVE) {
				position += a.direction;
			}
		}
		return position;
	}

	bool IsValidSquare(Vector2Int vector) {
		return vector.x >= 0 && vector.x < gridSize
			&& vector.y >= 0 && vector.y < gridSize;
	}
}
