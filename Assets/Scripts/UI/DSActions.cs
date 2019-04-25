using ReactiveUI;

namespace DSUI {
	public struct SetPlayer : IUIAction {
		public Player player;

		public void Execute(UIState state) {
			state.players[player.id] = player;
		}
	}

	public struct SetMessageAction : IUIAction {
		public string message;

		public void Execute(UIState state) {
			state.message = message;
		}
	}

	public struct SetPhaseAction : IUIAction {
		public Phase phase;

		public void Execute(UIState state) {
			state.phase = phase;
		}
	}

	public struct SetTurnAction : IUIAction {
		public int turn;

		public void Execute(UIState state) {
			state.turn = turn;
		}
	}

	public struct RenderAction : IUIAction {
		public void Execute(UIState state) { }
	}
}