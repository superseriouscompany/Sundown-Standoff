using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player {
	public int id;
	public Vector2Int position;
	public Vector2Int targetPosition;
	public GameObject gameObject;
	public int hp;
	public List<Card> cards;
	public Card card;

	public Player(int id, int hp, Vector2Int position) {
		this.id = id;
		this.hp = hp;
		this.position = position;
		targetPosition = position;

		Deal();
	}

	public void PickCard(int actions) {
		for (int i = 0; i < cards.Count; i++) {
			if (cards[i].actions == actions) {
				card = cards[i];
				return;
			}
		}

		throw new CardMissingException($"Couldn't find card with {actions} actions");
	}

	public void Discard() {
		cards.Remove(card);
		if (cards.Count == 0) { Deal(); }
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
