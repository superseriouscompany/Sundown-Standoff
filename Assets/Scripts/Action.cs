using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {
	public ActionType actionType;
	public Player player;
	public int turn;
	public Vector2Int direction;

	public override string ToString() {
		return $"Action {actionType} {direction} for player {player.id} on turn {turn}";
	}
}

public enum ActionType {
	MOVE,
	SHOOT
}
