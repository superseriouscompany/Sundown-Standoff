using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReactiveUI {
	public abstract class UIEvents {
		protected UIState state;

		public void SetState(UIState state) {
			this.state = state;
			Subscribe();
		}

		void Subscribe() {
			Unsubscribe();
			state.OnStateChanged += OnStateChanged;
		}

		void Unsubscribe() {
			state.OnStateChanged -= OnStateChanged;
		}

		protected abstract void OnStateChanged();
	}
}
