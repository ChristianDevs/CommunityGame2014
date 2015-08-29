using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	private bool lastState;

	// Use this for initialization
	void Start () {
		lastState = Player.MusicEnable;

		if (Player.MusicEnable == false) {
			GetComponent<AudioSource>().volume = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Player.MusicEnable != lastState) {
			if (Player.MusicEnable == false) {
				GetComponent<AudioSource>().volume = 0;
			} else {
				GetComponent<AudioSource>().volume = 1;
			}

			lastState = Player.MusicEnable;
		}
	}
}
