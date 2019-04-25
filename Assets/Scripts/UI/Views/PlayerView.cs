using System.Collections;
using System.Collections.Generic;
using ReactiveUI;
using TMPro;
using System.Text;

public class PlayerView : UIView {
	public TextMeshProUGUI text;
	public int id;

	public override void Render(UIState state) {
		if (state.players[id] == null || state.players[id].cards == null) { return; }

		var sb = new StringBuilder();
		for (int i = 0; i < state.players[id].cards.Count; i++) {
			sb.Append($"{state.players[id].cards[i].actions} ");
		}
		text.text = $"Player {id + 1}\nHP: {state.players[id].hp}\nCards: {sb}";
	}
}
