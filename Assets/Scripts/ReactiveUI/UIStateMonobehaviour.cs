using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReactiveUI {
	[ExecuteInEditMode]
	public class UIStateMonobehaviour : MonoBehaviour {
		public bool isSingleton = true;
		public UIState state;
		public UIEvents events;

		UIDispatcher dispatcher;

		void OnEnable() {
			dispatcher = new UIDispatcher(state, isSingleton);
			if (isSingleton) {
				state.SetSingleton();
			}
		}

		void FixedUpdate() {
			dispatcher.Update();
		}

		void OnValidate() {
			state.TriggerChanged();
		}
	}
}
