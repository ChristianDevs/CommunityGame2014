using UnityEngine;
using System.Collections;

public class UiWinPopup : MonoBehaviour {
	void Start () {
		
	}
	
	void buttonPush(string buttonName) {
		switch (buttonName) {
		case "Retry":
			Application.LoadLevel(Application.loadedLevelName);
			break;
		case "Continue":
			Application.LoadLevel("LevelSelect");
			break;
		default:
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
