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
		if (player == null || player.hand == null) { return; }

		var sb = new StringBuilder();

		switch(state.phase) {
			case Phase.Cards:
				int index = 0;
				foreach (var card in player.hand.cards) {
					sb.AppendLine();
					sb.Append($"{++index}. {card.name} ({card.actions}) - {card.description}");
				}
				break;
			case Phase.Actions:
				sb.Append("Actions: ");
				sb.Append(player.actionCount + " / " + player.card.actions);
				break;
		}

		text.text = $"HP: {state.players[id].hp}\nAmmo: {state.players[id].ammo}\n{sb}";
	}
}
