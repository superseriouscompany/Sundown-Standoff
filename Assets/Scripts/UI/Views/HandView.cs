using System.Collections;
using System.Collections.Generic;
using ReactiveUI;
using TMPro;
using UnityEngine;

public class HandView : UIView {
	public TextMeshProUGUI text;
	public int id;

	public GameObject handContainer;
	public GameObject cardPrefab;

	public override void Render(UIState state) {
		if (state.players[id] == null) { return; }
		var hand = state.players[id].hand;

		foreach (Transform child in handContainer.transform) {
			Destroy(child.gameObject);
		}

		foreach (var card in hand.cards) {
			var cardBox = Instantiate(cardPrefab);
			cardBox.transform.SetParent(handContainer.transform, false);
			cardBox.transform.Find("Words").transform.Find("Name").GetComponent<TextMeshProUGUI>().text = card.name;
		}
	}
}
