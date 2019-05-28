using System.Collections;
using System.Collections.Generic;
using ReactiveUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandView : UIView {
	public int id;

	public GameObject handContainer;
	public GameObject cardPrefab;

	public GameObject drawPileContainer;
	public GameObject minicardPrefab;

	public override void Render(UIState state) {
		if (state.players[id] == null) { return; }
		var hand = state.players[id].hand;

		foreach (Transform child in handContainer.transform) {
			Destroy(child.gameObject);
		}

		foreach (var card in hand.cards) {
			var cardBox = Instantiate(cardPrefab);
			cardBox.transform.SetParent(handContainer.transform, false);
			cardBox.transform.Find("Card").GetComponent<Image>().sprite = card.sprite;
			cardBox.transform.Find("Words").transform.Find("Name").GetComponent<TextMeshProUGUI>().text = card.name;
			cardBox.transform.Find("Words").transform.Find("Description").GetComponent<TextMeshProUGUI>().text = card.description;
		}

		foreach (Transform child in drawPileContainer.transform) {
			Destroy(child.gameObject);
		}

		foreach (var card in hand.library) {
			var cardBox = Instantiate(minicardPrefab);
			cardBox.transform.SetParent(drawPileContainer.transform, false);
			cardBox.GetComponent<Image>().sprite = card.sprite;
		}
	}
}
