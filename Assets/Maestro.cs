using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Maestro : MonoBehaviour {
	AudioSource player;
	public static Maestro instance;

	public AudioClip gunShot;
	public AudioClip bulletHit;
	public AudioClip movement;
	public AudioClip[] deaths;
	public AudioClip[] hits;

	// Start is called before the first frame update
	void Start() {
		player = GetComponent<AudioSource>();
		instance = this;
	}

	public void PlayGunshot() {
		player.PlayOneShot(gunShot);
	}

	public void PlayHit() {
		var hitIndex = Random.Range(0, hits.Length);
		player.PlayOneShot(bulletHit);
		player.PlayOneShot(hits[hitIndex]);
	}

	public void PlayDeath() {
		var deathIndex = Random.Range(0, deaths.Length);
		player.PlayOneShot(deaths[deathIndex]);
	}

	public void PlayMovement() {
		player.PlayOneShot(movement);	}

}

