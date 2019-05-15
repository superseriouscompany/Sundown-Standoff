using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card {
	[HideInInspector]
	public int id;
	public int actions;
	public Effect effect;
	public string name;
	public string description;

	public override string ToString() {
		return $"{name} ({actions})";
	}
}

public enum Effect {
	None,
	Heal,
	GoldenGun,
	TeleportCenter
}
