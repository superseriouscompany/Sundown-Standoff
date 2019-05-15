using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Rules : MonoBehaviour {
	public int gridSize = 5;

	public int hp = 3;

	public int startingAmmo = 2;
	public int maxAmmo = 3;

	[BoxGroup("Cards")]
	public int handSize = 4;
	[BoxGroup("Cards")]
	public int deckSize = 8;
	[BoxGroup("Cards")]
	public int maxDupes = 8;
	[BoxGroup("Cards")]
	public List<Card> cards = new List<Card>();

	public bool debugPositions;

	public bool incrementalResolution;

	public float turnDelay = 1f;
	public float moveSpeed = 1f;


	public static Rules instance;

	void Awake() {
		instance = this;
	}
}
