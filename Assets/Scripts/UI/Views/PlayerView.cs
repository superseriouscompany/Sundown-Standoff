using System.Collections;
using System.Collections.Generic;
using ReactiveUI;
using TMPro;
using System.Text;

public class PlayerView : UIView {
	public TextMeshProUGUI text;
	public int id;

	public override void Render(UIState state) {
		var player = state.players[id];
		if (player == null || player.cards == null) { return; }

		var sb = new StringBuilder();

		switch(state.phase) {
			case Phase.CARDS:
				sb.Append("Cards: ");
				for (int i = 0; i < player.cards.Count; i++) {
					sb.Append($"{player.cards[i].actions} ");
				}
				break;
			case Phase.ACTIONS:
				sb.Append("Actions: ");
				sb.Append(player.actionCount + " / " + player.card.actions);
				break;
		}

		text.text = $"Player {id + 1}\nHP: {state.players[id].hp}\n{sb}";
	}
}
