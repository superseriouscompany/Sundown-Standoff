using System.Collections.Generic;

namespace ReactiveUI {
	public partial class UIState {
		public Player[] players = new Player[2];

		public string message;
		public Phase phase;
		public int turn;
	}

	public enum Phase {
		CARDS,
		ACTIONS
	}
}
