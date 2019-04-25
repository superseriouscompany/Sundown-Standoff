using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ReactiveUI;

public class Resolver {
	List<Action> actions;
	int round;
	Grid grid;
	Player[] players;

	public bool isComplete {
		get {
			return actions.FirstOrDefault((a) => a.turn == round) == null;
		}
	}

	public Resolver(Grid grid, Player[] players, List<Action> actions) {
		this.actions = actions;
		this.actions.Sort((a, b) => a.turn == b.turn ? a.actionType.CompareTo(b.actionType) : a.turn.CompareTo(b.turn));
		this.grid = grid;
		this.players = players;
	}

	public void Step() {
		var roundActions = actions.Where((a) => a.turn == round).ToList();

		foreach (var action in roundActions) {
			var player = action.player;

			switch (action.actionType) {
				case ActionType.MOVE:
					player.Move(action.direction);
					Debug.Log($"Changing action on player: {player.id} {action.direction}");
					break;
				case ActionType.SHOOT:
					var squares = grid.Raycast(action);
					var animator = player.gameObject.GetComponent<Animator>();
					animator.SetTrigger("Shoot");

					for (int i = 0; i < squares.Count; i++) {
						for (int j = 0; j < players.Length; j++) {
							if (squares[i] == players[j].targetPosition) {
								var victimAnimator = players[j].gameObject.GetComponent<Animator>();
								victimAnimator.SetTrigger("Hit");
								Debug.Log($"Player {player.id} hits player {players[j].id} with a shot!");
								players[j].hp--;
								UIDispatcher.Send(new DSUI.RenderAction());
							}
						}
					}
					break;
			}
		}

		round++;
	}
}
