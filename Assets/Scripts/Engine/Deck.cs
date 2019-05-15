using System;
using UnityEngine;
using System.Collections.Generic;

public class Deck {
	static Card[] allCards = new Card[] {
		new Card() {
			actions = 1,
			effect = Effect.Heal
		},
		new Card() {
			actions = 2,
			effect = Effect.GoldenGun
		},
		new Card() {
			actions = 2
		},
		new Card() {
			actions = 2,
			effect = Effect.TeleportCenter
		},
		new Card() {
			actions = 3,
		},
		new Card() {
			actions = 4
		}
	};

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
		var index = UnityEngine.Random.Range(0, allCards.Length);
		var card = allCards[index];
		card.id = index;
		return card;
	}
}