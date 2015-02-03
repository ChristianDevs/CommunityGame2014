using UnityEngine;
using System.Collections;

public class UiLosePopup : MonoBehaviour {
	void Start () {
		
	}
	
	void buttonPush(string buttonName) {
		switch (buttonName) {
		case "Retry":
			Application.LoadLevel(Application.loadedLevelName);
			break;
		case "Quit":
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
