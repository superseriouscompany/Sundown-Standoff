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
					sb.Append($"{++index}. {card.name} ({card.actions})");
					sb.AppendLine();
					sb.Append($"<size=80%>{card.description}</size>");
					sb.AppendLine();
					sb.AppendLine();
				}

				sb.AppendLine();
				sb.Append("-------------------");
				sb.AppendLine();
				sb.Append($"<size=80%>{player.hand}</size>");

				if (player.card != null) {
					sb.Append("Card Selected.");
				}
				break;
			case Phase.Actions:
				sb.Append("Actions: ");
				sb.Append(player.actionsEntered + " / " + player.actionsAvailable);
				break;
		}

		text.text = $"HP: {state.players[id].hp}\nAmmo: {state.players[id].ammo}\n\n{sb}";
	}
}
