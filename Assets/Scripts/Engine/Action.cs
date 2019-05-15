using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {
	public ActionType actionType;
	public Player player;
	public int turn;
	public Vector2Int direction;
	public Vector2Int dualDirection;

	public override string ToString() {
		return $"Action {actionType} {direction} for player {player.id} on turn {turn}";
	}

	public static List<Action> FromInput(Player[] players, KeyMapping[] mappings) {
		var actions = new List<Action>();

		for (int i = 0; i < mappings.Length; i++) {
			var action = FromMapping(i, mappings[i], players[i]);
			if (action != null) {
				actions.Add(action);
			}
		}

		return actions;
	}

	static Action FromMapping(int playerId, KeyMapping keys, Player player) {
		Action action;

		if (Input.GetKeyDown(keys.shoot)) {
			action = new Action() { actionType = ActionType.SHOOT };
		} else if (Input.GetKeyDown(keys.move)) {
			action = new Action() { actionType = ActionType.MOVE };
		} else {
			return null;
		}

		action.direction = new Vector2Int();
		if (Input.GetKey(keys.directions[0])) {
			action.direction.y++;
		}
		if (Input.GetKey(keys.directions[1])) {
			action.direction.x++;
		}
		if (Input.GetKey(keys.directions[2])) {
			action.direction.y--;
		}
		if (Input.GetKey(keys.directions[3])) {
			action.direction.x--;
		}

		action.player = player;
		return action;
	}
}

public enum ActionType {
	MOVE,
	RELOAD,
	SHOOT
}
