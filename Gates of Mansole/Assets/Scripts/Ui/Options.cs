using UnityEngine;
using System.Collections;

public class Options : MonoBehaviour {
	public GameObject optionsObject;
	public GameObject world;
	public GameObject menu;
	public string sceneName;
	// Use this for initialization
	void Start () {
	
	}

	void buttonPush(string buttonName) {
		switch (buttonName) {
		case "OpenOptions":
			world.SetActive(false);
			menu.SetActive(true);
		break;
		case "Continue":
			world.SetActive(true);
			menu.SetActive(false);
			break;
		case "Restart":
			Application.LoadLevel(sceneName);
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
