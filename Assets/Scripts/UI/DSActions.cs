using ReactiveUI;

namespace DSUI {
	public struct SetPlayer : IUIAction {
		public Player player;

		public void Execute(UIState state) {
			state.players[player.id] = player;
		}
	}

	public struct RenderAction : IUIAction {
		public void Execute(UIState state) { }
	}
}