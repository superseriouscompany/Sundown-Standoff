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
		Debug.Log($"Hello {id}");
		avatarImage.color = id == 0 ? Color.blue : Color.green;
	}
}
