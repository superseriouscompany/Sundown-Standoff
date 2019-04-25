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
		if (actions.Count >= card.actions) {
			throw new TooManyActionsException($"Tried to add an action but we already have {actions.Count} which is more than {card.actions}");
		}

		action.turn = turn++;
		Debug.Log($"Setting turn to {turn}");
		actions.Add(action);
	}

	public bool isReady {
		get {
			return actions.Count == card.actions;
		}
	}

	void Deal() {
		cards = new List<Card>() {
			new Card() { actions = 3 },
			new Card() { actions = 2 },
			new Card() { actions = 1 },
			new Card() { actions = 1 },
			new Card() { actions = 1 }
		};
	}

	public void Move(int horizontal, int vertical) {
		Move(new Vector2Int(horizontal, vertical));
	}

	public void Move(Vector2Int vector) {
		targetPosition = position + vector;
	}
}

class CardMissingException : System.Exception {
	public CardMissingException(string message) : base(message) { }
}

class TooManyActionsException : System.Exception {
	public TooManyActionsException(string message) : base(message) { }
}