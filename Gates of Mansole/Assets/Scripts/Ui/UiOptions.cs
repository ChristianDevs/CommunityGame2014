using UnityEngine;
using System.Collections;

public class UiOptions : MonoBehaviour {
	public GameObject world;
	public GameObject menu;
	// Use this for initialization
	void Start () {
	
	}

	void buttonPush(string buttonName) {
		switch (buttonName) {
		case "OpenOptions":
			world.SendMessage("PauseUnits", null, SendMessageOptions.DontRequireReceiver);
			world.SetActive(false);
			menu.SetActive(true);
		break;
		case "Continue":
			world.SetActive(true);
			world.SendMessage("UnpauseUnits", null, SendMessageOptions.DontRequireReceiver);
			menu.SetActive(false);
			break;
		case "Restart":
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
