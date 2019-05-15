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
		var effect = action.player.card.effect;

		foreach (var direction in directions) {
			for (var nextSquare = origin + direction; IsValidSquare(nextSquare); nextSquare += direction) {
				squares.Add(nextSquare);
			}
		}

		if (effect == Effect.Explosive) {
			var explosionSquares = new List<Vector2Int>();
			foreach (var square in squares) {
				if (square.x == 0) {
					explosionSquares.Add(new Vector2Int(square.x, square.y + 1));
					explosionSquares.Add(new Vector2Int(square.x, square.y - 1));
					explosionSquares.Add(new Vector2Int(square.x + 1, square.y + 1));
					explosionSquares.Add(new Vector2Int(square.x + 1, square.y - 1));
					explosionSquares.Add(new Vector2Int(square.x + 1, square.y));
				}
				if (square.x == gridSize - 1) {
					explosionSquares.Add(new Vector2Int(square.x, square.y + 1));
					explosionSquares.Add(new Vector2Int(square.x, square.y - 1));
					explosionSquares.Add(new Vector2Int(square.x - 1, square.y + 1));
					explosionSquares.Add(new Vector2Int(square.x - 1, square.y - 1));
					explosionSquares.Add(new Vector2Int(square.x - 1, square.y));
				}
				if (square.y == 0) {
					explosionSquares.Add(new Vector2Int(square.x - 1, square.y));
					explosionSquares.Add(new Vector2Int(square.x + 1, square.y));
					explosionSquares.Add(new Vector2Int(square.x - 1, square.y + 1));
					explosionSquares.Add(new Vector2Int(square.x + 1, square.y + 1));
					explosionSquares.Add(new Vector2Int(square.x, square.y + 1));
				}
				if (square.y == gridSize - 1) {
					explosionSquares.Add(new Vector2Int(square.x - 1, square.y));
					explosionSquares.Add(new Vector2Int(square.x + 1, square.y));
					explosionSquares.Add(new Vector2Int(square.x - 1, square.y - 1));
					explosionSquares.Add(new Vector2Int(square.x + 1, square.y - 1));
					explosionSquares.Add(new Vector2Int(square.x, square.y - 1));
				}
			}

			foreach (var square in explosionSquares) {
				if (!IsValidSquare(square)) { continue; }
				bool squareExists = false;
				foreach (var s in squares) {
					if (s.x == square.x && s.y == square.y) {
						squareExists = true;
					}
				}
				if (squareExists) { continue; }

				squares.Add(square);
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
