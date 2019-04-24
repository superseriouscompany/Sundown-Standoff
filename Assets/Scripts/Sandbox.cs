using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbox : MonoBehaviour {
	public float speed;

	// Start is called before the first frame update
	void Start() {
		StartCoroutine(DoMovement());
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
				Debug.Log($"Moving by {Time.time - startTime}");
				transform.position = Vector3.Lerp(origin, destination, (Time.time - startTime) * speed);
				yield return null;
			} while (deltaTime < distance / speed);

			transform.position = destination;
			yield return new WaitForSeconds(1);
		}
	}
}
