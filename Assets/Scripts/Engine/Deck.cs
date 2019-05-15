using System;
using UnityEngine;
using System.Collections.Generic;

public class Deck {
	public static List<Card> Deal() {
		var cards = new List<Card>();
		while (cards.Count < Rules.instance.deckSize) {
			var candidate = NextCard();
			int dupeCount = 0;
			foreach (var card in cards) {
				if (card.id == candidate.id) {
					dupeCount++;
				}
			}
			if (dupeCount >= 2) {
				Debug.Log("Ignoring dupe card " + candidate.id);
				continue;
			}

			cards.Add(candidate);
		}
		return cards;
	}

	static Card NextCard() {
		var index = UnityEngine.Random.Range(0, Rules.instance.cards.Count);
		var card = Rules.instance.cards[index];
		card.id = index;
		return card;
	}
}