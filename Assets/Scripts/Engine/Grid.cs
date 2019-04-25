using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
	public int gridSize;

	public bool Validate(Action action) {
		if (action.direction.x == 0 && action.direction.y == 0) { return false; }

		var isDiagonal = action.direction.x != 0 && action.direction.y != 0;

		switch(action.actionType) {
			case ActionType.MOVE:
				if (isDiagonal && !Rules.instance.diagonalMove) { return false; }
				var targetPosition = ExpectedPosition(action) + action.direction;
				return IsValidSquare(targetPosition);
			case ActionType.SHOOT:
				if (isDiagonal && !Rules.instance.diagonalShots) { return false; }
				return true;
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

		if (Rules.instance.doubleShot) {
			for (var nextSquare = origin + action.dualDirection; IsValidSquare(nextSquare); nextSquare += action.dualDirection) {
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
