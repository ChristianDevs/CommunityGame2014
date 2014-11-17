using UnityEngine;
using System.Collections;

public class UiBackToLevelSelect : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void buttonPush(string buttonName) {
		switch (buttonName) {
		case "Button":
			Player.loadPlayer();
			Application.LoadLevel("LevelSelect");
			break;
		default:
			break;
		}
	}
}

