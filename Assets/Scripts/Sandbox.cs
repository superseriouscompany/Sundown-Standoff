using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbox : MonoBehaviour {
	public float speed;
	public Animator animator;

	Hand hand;

	// Start is called before the first frame update
	void Start() {
		StartCoroutine(DoMovement());
		var cards = Deck.Deal();

		foreach (var card in cards) {
			Debug.Log($"CARD {card.id}");
		}

		hand = new Hand(cards);
		Debug.Log(hand);
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

		if (Input.GetKeyDown(KeyCode.D)) {
			hand.Draw();
			Debug.Log(hand);
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			hand.Discard(0);
			Debug.Log(hand);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			hand.Discard(1);
			Debug.Log(hand);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			hand.Discard(2);
			Debug.Log(hand);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4)) {
			hand.Discard(3);
			Debug.Log(hand);
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
