using System.Collections.Generic;

namespace ReactiveUI {

	public class UIDispatcher {

		public static UIDispatcher singleton;

		UIState state;
		Queue<IUIAction> actions = new Queue<IUIAction>();

		public event System.Action<IUIAction> OnDispatch;

		public UIDispatcher(UIState state, bool isSingleton = false) {
			this.state = state;

			actions = new Queue<IUIAction>();
			if (isSingleton) {
				singleton = this;
			}
		}

		public static void Send(IUIAction action) {
			if (singleton == null) { return; }

			singleton.Send(action, true);
		}

		public void Send(IUIAction action, bool _) {
			if (OnDispatch != null) { OnDispatch.Invoke(action); }

			actions.Enqueue(action);
		}

		public void Update() {
			if (ResolveActions()) {
				state.TriggerChanged();
			}
		}

		bool ResolveActions() {
			if (actions.Count == 0) { return false; }
			actions.Dequeue().Execute(state);
			return true;
		}
	}
}
