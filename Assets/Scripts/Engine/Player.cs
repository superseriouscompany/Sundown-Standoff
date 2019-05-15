using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player {
	public int id;
	public Vector2Int position;
	public Vector2Int targetPosition;
	public bool bounceBack;
	public GameObject gameObject;
	public int hp;
	public List<Card> cards;
	public Card card;
	public List<Action> actions = new List<Action>();


	public KeyMapping keyMapping;
	public int actionsTaken;
	int turn;

	public Player(int id, int hp, Vector2Int position) {
		this.id = id;
		this.hp = hp;
		this.position = position;
		targetPosition = position;

		Deal();
	}

	public void PickCard(int numActions) {
		for (int i = 0; i < cards.Count; i++) {
			if (cards[i].actions == numActions) {
				card = cards[i];
				actions.Clear();
				actionsTaken = 0;
				Debug.Log($"Player {id} picked card {card.actions}");
				return;
			}
		}

		throw new CardMissingException($"Couldn't find card with {numActions} actions");
	}

	public void Discard() {
		cards.Remove(card);
		turn = 0;
		if (cards.Count == 0) { Deal(); }
	}

	public void AddAction(Action action) {
		if (actionCount >= card.actions) {
			throw new TooManyActionsException($"Tried to add an action but we already have {actions.Count} which is more than {card.actions}");
		}

		if (Rules.instance.doubleShot) {
			var lastAction = actions.Count > 0 ? actions[actions.Count - 1] : null;
			var expectsShot = lastAction != null && lastAction.actionType == ActionType.SHOOT && lastAction.dualDirection.x == 0 && lastAction.dualDirection.y == 0;

			if (expectsShot && action.actionType == ActionType.SHOOT) {
				lastAction.dualDirection = action.direction;
				return;
			} else if (expectsShot && action.actionType == ActionType.MOVE) {
				throw new NeedsDoubleShotException($"Expected second shoot action but got move");
			}
		}

		action.turn = turn++;
		actions.Add(action);
	}

	public bool isReady {
		get {
			if (Rules.instance.incrementalResolution) {
				return actionCount > actionsTaken || actionCount == card.actions;
			}
			return actionCount == card.actions;
		}
	}

	public int actionCount {
		get {
			int total = 0;
			foreach(var action in actions) {
				total++;

				if (Rules.instance.doubleShot) {
					if (action.actionType == ActionType.SHOOT && action.dualDirection.x == 0 && action.dualDirection.y == 0) {
						total--;
					}
				}
			}
			return total;
		}
	}

	void Deal() {
		var minimum = Rules.instance.minimum2 ? 2 : 1;
		cards = new List<Card>() {
			new Card() { actions = 3 },
			new Card() { actions = 2 },
			new Card() { actions = minimum },
			new Card() { actions = minimum },
			new Card() { actions = minimum }
		};
	}

	public void Move(int horizontal, int vertical) {
		Move(new Vector2Int(horizontal, vertical));
	}

	public void Move(Vector2Int vector) {
		targetPosition = position + vector;
		targetPosition.x = Mathf.Clamp(targetPosition.x, 0, Rules.instance.gridSize - 1);
		targetPosition.y = Mathf.Clamp(targetPosition.y, 0, Rules.instance.gridSize - 1);
	}
}

class CardMissingException : System.Exception {
	public CardMissingException(string message) : base(message) { }
}

class TooManyActionsException : System.Exception {
	public TooManyActionsException(string message) : base(message) { }
}

class NeedsDoubleShotException : System.Exception {
	public NeedsDoubleShotException(string message) : base(message) { }
}