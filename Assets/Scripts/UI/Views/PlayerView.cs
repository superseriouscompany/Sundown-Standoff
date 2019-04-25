using System.Collections;
using System.Collections.Generic;
using ReactiveUI;
using TMPro;

public class PlayerView : UIView {
	public TextMeshProUGUI text;
	public int id;

	public override void Render(UIState state) {
		text.text = $"Player {id + 1}\nHP: {state.players[id].hp}\nCards: 1 1 1 1 1";
	}
}
