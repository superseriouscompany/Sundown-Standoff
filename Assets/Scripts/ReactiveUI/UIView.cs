using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReactiveUI {
	[ExecuteInEditMode]
	public abstract class UIView : MonoBehaviour {
		protected virtual void OnEnable() {
			if (GetState() == null) {
				Debug.LogWarning("UIState is null. Have you included a UIStateMonobehaviour in the scene?");
				return;
			}
			Subscribe();
		}

		protected virtual void OnDisable() {
			Unsubscribe();
		}

		protected virtual UIState GetState() {
			return UIState.singleton;
		}

		protected UIState state {
			get { return GetState(); }
		}

		void Subscribe() {
			Unsubscribe();
			GetState().OnStateChanged += RenderWrapper;
		}

		void Unsubscribe() {
			GetState().OnStateChanged -= RenderWrapper;
		}

		void RenderWrapper() {
			try {
				Render(state);
			} catch (System.Exception e) {
				Debug.LogError("Render encountered an exception: " + e);
			}
		}

		public abstract void Render(UIState state);

	}
}
