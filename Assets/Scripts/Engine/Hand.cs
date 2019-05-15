using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Hand {
	public List<Card> cards;
	List<Card> graveyard;
	Queue<Card> library;

	public Hand(List<Card> deck) {
		var activeCards = deck.GetRange(0, Rules.instance.handSize);
		var inactiveCards = deck.GetRange(Rules.instance.handSize, deck.Count - Rules.instance.handSize);

		cards = activeCards;
		library = new Queue<Card>(inactiveCards);
		graveyard = new List<Card>();
	}

	public void Draw() {
		if (library.Count == 0) {
			Reshuffle();
		}

		cards.Add(library.Dequeue());
	}

	public void Discard(int index) {
		graveyard.Add(cards[index]);
		cards.RemoveAt(index);
	}

	void Reshuffle() {
		// https://stackoverflow.com/questions/273313/randomize-a-listt
		var rng = new Random();
		int n = graveyard.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			var card = graveyard[k];
			graveyard[k] = graveyard[n];
			graveyard[n] = card;
		}

		library = new Queue<Card>(graveyard);
		graveyard = new List<Card>();
	}

	public override string ToString() {
		var sb = new StringBuilder();
		sb.Append("Hand:\n");
		foreach (var card in cards) {
			sb.Append(card.id + " ");
		}
		sb.AppendLine();
		sb.Append("Library:\n");
		foreach (var card in library) {
			sb.Append(card.id + " ");
		}
		sb.AppendLine();
		sb.Append("Graveyard:\n");
		foreach (var card in graveyard) {
			sb.Append(card.id + " ");
		}
		return sb.ToString();
	}
}