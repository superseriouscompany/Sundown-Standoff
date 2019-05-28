using ReactiveUI;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class TableView : UIView {
	public Image p0Card;
	public Image p1Card;

	public TextMeshProUGUI p0Ready;
	public TextMeshProUGUI p0Actions;

	public TextMeshProUGUI p1Ready;
	public TextMeshProUGUI p1Actions;

	public override void Render(UIState state) {
		if (state.phase == Phase.Cards) {
			p0Ready.text = state.players[0].card != null ? "Ready!" : "";
			p1Ready.text = state.players[1].card != null ? "Ready!" : "";
			p0Actions.text = "";
			p1Actions.text = "";
			if (state.players[0].card != null) {
				p0Card.sprite = state.players[0].card.sprite;
			}
			if (state.players[1].card != null) {
				p1Card.sprite = state.players[1].card.sprite;
			}
			p0Card.color = Color.black;
			p1Card.color = Color.black;
		} else if (state.phase == Phase.Actions) {
			p0Actions.text = state.players[0].actionsEntered + " / " + state.players[0].actionsAvailable;
			p1Actions.text = state.players[1].actionsEntered + " / " + state.players[1].actionsAvailable;
			p0Ready.text = state.players[0].actionsEntered >= state.players[0].actionsAvailable ? "Ready!" : "";
			p1Ready.text = state.players[1].actionsEntered >= state.players[1].actionsAvailable ? "Ready!" : "";
			p0Card.color = Color.white;
			p1Card.color = Color.white;
		}
	}
}