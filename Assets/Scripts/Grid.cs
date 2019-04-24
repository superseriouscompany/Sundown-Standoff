using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
	public int gridSize;

	public bool Validate(Action action) {
		if (action.target.x == 0 && action.target.y == 0) { return false; }

		switch(action.actionType) {
			case ActionType.MOVE:
				var targetPosition = action.player.position + action.target;
				return targetPosition.x >= 0 && targetPosition.x < gridSize 
					&& targetPosition.y >= 0 && targetPosition.y < gridSize;
			default:
				return true;
		}
	}
}
