using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbox : MonoBehaviour {
	public float speed;
	public Animator animator;

	// Start is called before the first frame update
	void Start() {
		StartCoroutine(DoMovement());
	}

	void Update() {
		if (Input.GetKeyUp(KeyCode.Space)) {
			animator.SetTrigger("DoubleShot");
		}

		if (Input.GetButtonDown("Shoot1")) {
			Debug.Log("SHOOT MFER");
		}

		if (Input.GetKeyDown(KeyCode.Tab)) {
			Debug.Log("TAB MFER");
		}

		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			Debug.Log("LEFT SHIFT DOWN");
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Debug.Log("This is bullshit.");
		}

		if (Input.GetKeyDown(KeyCode.LeftAlt)) {
			Debug.Log("ALT");
		}

		if (Input.GetKeyDown(KeyCode.LeftControl)) {
			Debug.Log("CTRL");
		}

		if (Input.GetKeyDown(KeyCode.LeftCommand)) {
			Debug.Log("CMD");
		}

		if (Input.GetKeyDown(KeyCode.Backslash)) {
			Debug.Log("BACKSLASH");
		}

		if (Input.GetKeyDown(KeyCode.BackQuote)) {
			Debug.Log("BACKQUOTE");
		}

		if (Input.GetKeyDown(KeyCode.Alpha7)) {
			Debug.Log("7");
		}
	}

	IEnumerator DoMovement() {
		while (true) {
			var startTime = Time.time;
			var origin = transform.position;
			var destination = transform.position + Vector3.right;
			var distance = (destination - origin).magnitude;
			float deltaTime;
			do {
				deltaTime = Time.time - startTime;
				transform.position = Vector3.Lerp(origin, destination, (Time.time - startTime) * speed);
				yield return null;
			} while (deltaTime < distance / speed);

			transform.position = destination;
			yield return new WaitForSeconds(1);
		}
	}
}
