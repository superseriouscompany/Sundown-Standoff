using System.Collections;
using System.Collections.Generic;
using ReactiveUI;
using TMPro;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StatusView: UIView {
	public int id;

	public Image avatarImage;
	public GameObject heartContainer;
	public GameObject bulletContainer;
	public GameObject heartPrefab;
	public GameObject bulletPrefab;

	public override void Render(UIState state) {
		var player = state.players[id];
		if (player == null) { return; }

		Debug.Log($"Hello {id}");
		avatarImage.color = id == 0 ? Color.magenta : Color.cyan;

		if (heartPrefab != null) {
			foreach (Transform child in heartContainer.transform) {
				Destroy(child.gameObject);
			}

			for (int i = 0; i < player.hp; i++) {
				var heart = Instantiate(heartPrefab);
				heart.transform.SetParent(heartContainer.transform, false);
			}
		}

		if (bulletContainer != null) {
			foreach (Transform child in bulletContainer.transform) {
				Destroy(child.gameObject);
			}

			for (int i = 0; i < player.ammo; i++) {
				var bullet = Instantiate(bulletPrefab);
				bullet.transform.localScale = Vector3.one;
				bullet.transform.SetParent(bulletContainer.transform, false);
			}
		}
	}
}
