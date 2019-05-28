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

			for (int i = 0; i < Rules.instance.maxHp; i++) {
				var heart = Instantiate(heartPrefab);
				heart.transform.SetParent(heartContainer.transform, false);
				var isEmpty = id == 0 ? i + 1 > player.hp : Rules.instance.maxHp - i > player.hp;
				if (isEmpty) { heart.GetComponent<Image>().color = Color.black; }
			}
		}

		if (bulletContainer != null) {
			foreach (Transform child in bulletContainer.transform) {
				Destroy(child.gameObject);
			}

			for (int i = 0; i < Rules.instance.maxAmmo; i++) {
				var bullet = Instantiate(bulletPrefab);
				bullet.transform.localScale = Vector3.one;
				bullet.transform.SetParent(bulletContainer.transform, false);
				var isEmpty = id == 0 ? i + 1 > player.ammo : Rules.instance.maxAmmo - i > player.ammo;
				if (isEmpty) { bullet.GetComponent<Image>().color = Color.black; }
			}
		}
	}
}
