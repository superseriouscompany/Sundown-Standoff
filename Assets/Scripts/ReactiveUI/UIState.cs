namespace ReactiveUI {
	[System.Serializable]
	public partial class UIState {
		public event System.Action OnStateChanged;

		public static UIState singleton { get; protected set; }

		public void SetSingleton() {
			singleton = this;
		}

		public void TriggerChanged() {
			if (OnStateChanged != null) {
				OnStateChanged.Invoke();
			}
		}
	}
}