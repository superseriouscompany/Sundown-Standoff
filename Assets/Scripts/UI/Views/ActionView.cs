using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReactiveUI;
using TMPro;

public class ActionView : UIView {
	public TextMeshProUGUI text;

	public override void Render(UIState state) {
		text.text = state.message;
	}
}
