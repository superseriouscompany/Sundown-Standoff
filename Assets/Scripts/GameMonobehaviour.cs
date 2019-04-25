﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactiveUI;

public class GameMonobehaviour : MonoBehaviour {
	public int gridSize = 5;
	public int hp = 3;

	public bool diagonalShots;
	public bool diagonalMove;
	public bool moveBeatsShot;
	public bool minimum2;
	public bool incrementalResolution;
	public bool doubleShot;
	public bool obstacles;
	public bool mines;

	public GameObject player;
	public GameObject gridSquare;
	public float speed = 1f;
	public float gridSquareWidth = 0.6f;

	Player[] players = new Player[2];
	Grid grid;

	Action[] moves = new Action[2];
	int moveIndex;

	void Start() {
		players[0] = new Player(0, hp, new Vector2Int(0, gridSize / 2));
		players[1] = new Player(1, hp, new Vector2Int(gridSize - 1, gridSize / 2));

		for (int i = 0; i < players.Length; i++) {
			var playerObject = Instantiate(player);
			playerObject.transform.position = GridToWorld(players[i].position);
			if (i == 1) {
				playerObject.GetComponent<SpriteRenderer>().flipX = true;
			}
			players[i].gameObject = playerObject;
			UIDispatcher.Send(new DSUI.SetPlayer() { player = players[i] });
		}

		var gridCnr = new GameObject("Grid");
		for (int i = 0; i < gridSize; i++) {
			for (int j = 0; j < gridSize; j++) {
				var square = Instantiate(gridSquare);
				square.transform.position = GridToWorld(i, j);
				square.transform.parent = gridCnr.transform;
			}
		}

		grid = new Grid() { gridSize = gridSize };
	}

	Vector2 GridToWorld(int x, int y) {
		return GridToWorld(new Vector2Int(x, y));
	}

	Vector2 GridToWorld(Vector2Int vector) {
		return new Vector2(
			vector.x * gridSquareWidth - 2.5f + gridSquareWidth / 2,
			vector.y * gridSquareWidth - 2.5f + gridSquareWidth / 2
		);
	}

	Action action;
	int cardSelectionIndex;
	void Update() {
		int actions = 0;
		if (Input.GetKeyUp(KeyCode.Alpha1)) {
			actions = 1;
		} else if (Input.GetKeyUp(KeyCode.Alpha2)) {
			actions = 2;
		} else if(Input.GetKeyUp(KeyCode.Alpha3)) {
			actions = 3;
		}

		if (actions  == 0) { return; }
		try {
			players[cardSelectionIndex].UseCard(actions);
			UIDispatcher.Send(new DSUI.RenderAction());
			if (++cardSelectionIndex >= players.Length) { cardSelectionIndex = 0; }
		} catch(CardMissingException e) { }

		return;

		if (Input.GetKeyUp(KeyCode.Space)) {
			action = new Action() { actionType = ActionType.SHOOT };
		} else if (Input.GetKeyUp(KeyCode.Return)) {
			action = new Action() { actionType = ActionType.MOVE };
		} else {
			return;
		}

		action.direction = new Vector2Int();
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			action.direction.x++;
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
			action.direction.x--;
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			action.direction.y++;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
			action.direction.y--;
		}

		action.player = players[moveIndex];

		if (!grid.Validate(action)) {
			Log("Action rejected");
			return;
		}

		Debug.Log($"Action {action.actionType} {action.direction} received for player {action.player.id}");
		Log($"Action received for player {action.player.id + 1}");
		moves[moveIndex++] = action;

		if (moveIndex >= moves.Length) {
			System.Array.Sort(moves, (a, b) => a.actionType.CompareTo(b.actionType));
			for (int i = 0; i < moves.Length; i++) {
				var player = players[moves[i].player.id];
				switch(moves[i].actionType) {
					case ActionType.MOVE:
						player.Move(moves[i].direction);
						StartCoroutine(
							SmoothMove(player)
						);
						break;
					case ActionType.SHOOT:
						var squares = grid.Raycast(moves[i]);
						var animator = player.gameObject.GetComponent<Animator>();
						animator.SetTrigger("Shoot");

						for (int x = 0; x < squares.Count; x++) {
							for (int j = 0; j < players.Length; j++) {
								if (squares[x] == players[j].targetPosition) {
									var victimAnimator = players[j].gameObject.GetComponent<Animator>();
									victimAnimator.SetTrigger("Hit");
									Log($"Player {player.id} hits player {players[j].id} with a shot!");
									players[j].hp--;
									UIDispatcher.Send(new DSUI.RenderAction());
									if (players[j].hp <= 0) {
										StartCoroutine(Restart());
									}
								}
							}
						}
						break;
				}
			}
			moveIndex = 0;
		}
	}

	IEnumerator SmoothMove(Player p) {
		var origin = GridToWorld(p.position);
		var destination = GridToWorld(p.targetPosition);
		var distance = (destination - origin).magnitude;
		var startTime = Time.time;
		var animator = p.gameObject.GetComponent<Animator>();
		animator.SetBool("Move", true);
		float deltaTime;
		do {
			deltaTime = Time.time - startTime;
			p.gameObject.transform.position = Vector3.Lerp(origin, destination, (Time.time - startTime) * speed);
			yield return null;
		} while (deltaTime < distance / speed);
		animator.SetBool("Move", false);
		p.gameObject.transform.position = destination;
		p.position = p.targetPosition;
		yield return null;
	}

	IEnumerator Restart() {
		yield return new WaitForSeconds(1);
		for (int i = 0; i < players.Length; i++) {
			Destroy(players[i].gameObject);
		}
		Start();
	}

	void Log(string msg) {
		Debug.Log(msg);
		UIDispatcher.Send(new DSUI.SetMessageAction() { message = msg });
	}
}
