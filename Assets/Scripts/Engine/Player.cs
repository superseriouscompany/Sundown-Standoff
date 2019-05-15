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
	public Card card;
	public Hand hand;
	public List<Action> actions = new List<Action>();
	public int ammo;

	public KeyMapping keyMapping;
	public int actionsTaken;
	public int actionModifier;

	int turn;
	int cardIndex = -1;

	public Player(int id, int hp, Vector2Int position) {
		this.id = id;
		this.hp = hp;
		this.position = position;
		targetPosition = position;
		ammo = Rules.instance.startingAmmo;

		hand = new Hand(Deck.Deal());
		Debug.Log(hand);
	}

	public void PickCard(int index) {
		cardIndex = index;
		if (index > hand.cards.Count) {
			throw new CardMissingException($"Couldn't find card at index {index}");
		}

		card = hand.cards[index];
		Debug.Log($"Player {id} picked card {card}");
		actions.Clear();
		actionsTaken = 0;
		actionModifier = 0;
		turn = 0;
	}

	public void Discard() {
		hand.Discard(cardIndex);
		while (hand.cards.Count < Rules.instance.handSize) {
			hand.Draw();
		}
		Debug.Log(hand);
		cardIndex = -1;
	}

	public void AddAction(Action action) {
		if (actionsEntered >= actionsAvailable) {
			throw new TooManyActionsException($"Tried to add an action but we already have {actions.Count} which is more than {card.actions}");
		}

		action.turn = turn++;
		actions.Add(action);
	}

	public bool isReady {
		get {
			if (Rules.instance.incrementalResolution) {
				return actionsEntered > actionsTaken || actionsEntered == actionsAvailable;
			}
			return actionsEntered == actionsAvailable;
		}
	}

	public int actionsEntered {
		get {
			return actions.Count;
		}
	}

	public int actionsAvailable {
		get {
			return card == null ? 0 : card.actions + actionModifier;
		}
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