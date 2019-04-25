using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutines : MonoBehaviour {
	public static Coroutine Start(IEnumerator enumerator) {
		if (instance == null && Application.isPlaying) { Debug.LogError("Coroutines script is not attached"); }
		if (instance == null || !instance.gameObject.activeSelf) { return null; }

		return instance.StartCoroutine(enumerator);
	}

	public static void Stop(Coroutine coroutine) {
		if (instance == null || coroutine == null) { return; }

		instance.StopCoroutine(coroutine);
	}

	public static Coroutines instance;

	void Awake() {
		if (instance != null) { Debug.LogError("Tried to instantiate Coroutines twice!"); }

		instance = this;
	}
}