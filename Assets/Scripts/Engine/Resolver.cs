﻿using System.Collections;
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
		var roundMovement = actions.Where((a) => a.turn == round && a.actionType == ActionType.MOVE).ToList();
		var roundShooting = actions.Where((a) => a.turn == round && a.actionType == ActionType.SHOOT).ToList();

		foreach (var action in roundMovement) {
			var player = action.player;
			player.Move(action.direction);
			foreach (var opponent in players) {
				if (opponent.id == player.id) { continue; }
				if (player.targetPosition == opponent.targetPosition) {
					if (player.position == player.targetPosition) {
						player.hp--;
						opponent.targetPosition = opponent.position;
					} else if (opponent.position == opponent.targetPosition) {
						opponent.hp--;
						player.targetPosition = player.position;
					} else {
						player.targetPosition = player.position;
						opponent.targetPosition = opponent.position;
					}
				}
			}
		}

		foreach (var action in roundShooting) {
			var player = action.player;
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
		}

		round++;
	}
}
