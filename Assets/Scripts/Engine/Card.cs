using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {
	public int id;
	public int actions;
	public Effect effect;
}

public enum Effect {
	None,
	Heal,
	GoldenGun,
	TeleportCenter
}
