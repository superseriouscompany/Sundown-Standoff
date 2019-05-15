using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ReactiveUI;

public class Resolver {
	int round;
	List<Action> actions = new List<Action>();
	Grid grid;
	Player[] players;

	public List<Vector2Int> hitSquares = new List<Vector2Int>();

	public bool isComplete {
		get {
			return actions.FirstOrDefault((a) => a.turn == round) == null;
		}
	}

	public Resolver(Grid grid, Player[] players) {
		this.grid = grid;
		this.players = players;
		SortActions();
	}

	public void AddActions(List<Action> newActions) {
		actions.AddRange(newActions);
		SortActions();
	}

	public void AddAction(Action action) {
		actions.Add(action);
		SortActions();
	}

	public void Reset() {
		round = 0;
		actions.Clear();
	}

	void SortActions() {
		actions.Sort((a, b) => a.turn == b.turn ? a.actionType.CompareTo(b.actionType) : a.turn.CompareTo(b.turn));
	}

	public void StepMovement() {
		var roundMovement = actions.Where((a) => a.turn == round && a.actionType == ActionType.MOVE).ToList();
		Debug.Log($"Running {roundMovement.Count} moves");
		foreach (var action in roundMovement) {
			var player = action.player;
			player.Move(action.direction);
			player.actionsTaken++;
		}

		foreach (var player in players) {
			if (player.targetPosition == player.position) { continue; }
			foreach (var opponent in players) {
				if (opponent.id == player.id) { continue; }
				if (player.targetPosition == opponent.targetPosition) {
					if (player.position == player.targetPosition) {
						Hit(player);
						opponent.bounceBack = true;
					} else if (opponent.position == opponent.targetPosition) {
						Hit(opponent);
						player.bounceBack = true;
					} else {
						player.bounceBack = true;
						opponent.bounceBack = true;
					}
				} else if (player.targetPosition == opponent.position && opponent.targetPosition == player.position) {
					player.bounceBack = true;
					opponent.bounceBack = true;
				}
			}
		}
	}

	public void StepReload() {
		var roundReloads = actions.Where((a) => a.turn == round && a.actionType == ActionType.RELOAD).ToList();

		Debug.Log($"Running {roundReloads.Count} reloads");

		foreach (var action in roundReloads) {
			Reload(action.player);
		}
	}

	public void StepShots() {
		var roundShooting = actions.Where((a) => a.turn == round && a.actionType == ActionType.SHOOT).ToList();

		Debug.Log($"Running {roundShooting.Count} shots");

		hitSquares.Clear();
		foreach (var action in roundShooting) {
			var player = action.player;
			if (player.ammo <= 0) {
				Reload(player);
				continue;
			}
			player.ammo--;
			player.ammo = Mathf.Clamp(player.ammo, 0, Rules.instance.maxAmmo);
			var squares = grid.Raycast(action);
			hitSquares.AddRange(squares);
			var animator = player.gameObject.GetComponent<Animator>();
			animator.SetTrigger("Shoot");
			Maestro.instance.PlayGunshot();

			for (int i = 0; i < squares.Count; i++) {
				for (int j = 0; j < players.Length; j++) {
					var position = players[j].bounceBack ? players[j].position : players[j].targetPosition;
					if (squares[i] == position) {
						Coroutines.Start(Hit(players[j]));
					}
				}
			}

			player.actionsTaken++;
		}

		round++;
	}

	void Reload(Player player) {
		player.ammo++;
		player.ammo = Mathf.Clamp(player.ammo, 0, Rules.instance.maxAmmo);
		player.actionsTaken++;
	}

	IEnumerator Hit(Player player) {
		yield return new WaitForSecondsRealtime(0.8f);
		var otherPlayer = player.id == 0 ? players[1] : players[0];
		if (otherPlayer.card.effect == Effect.GoldenGun) {
			player.hp = 0;
		} else {
			player.hp--;
		}
		var animator = player.gameObject.GetComponent<Animator>();
		animator.SetTrigger("Hit");
		yield return new WaitForSecondsRealtime(0.1f);
		Maestro.instance.PlayHit();
		UIDispatcher.Send(new DSUI.RenderAction());
	}
}
