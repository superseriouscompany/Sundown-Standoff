using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rules : MonoBehaviour {
	public int gridSize = 5;
	public int hp = 3;

	public bool doubleShot;
	public bool minimum2;
	public bool incrementalResolution;
	public bool diagonalShots;
	public bool diagonalMove;
	public bool moveBeatsShot;
	public bool obstacles;
	public bool mines;

	public bool debugPositions;

	public int startingAmmo = 2;
	public int maxAmmo = 3;

	public int handSize = 4;
	public int deckSize = 8;

	public float turnDelay = 1f;
	public float moveSpeed = 1f;

	public static Rules instance;

	void Awake() {
		instance = this;
	}
}
