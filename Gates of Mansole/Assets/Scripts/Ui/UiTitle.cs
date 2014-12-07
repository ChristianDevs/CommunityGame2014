using UnityEngine;
using System.Collections;

public class UiTitle : MonoBehaviour {

	public GameObject[] unitTypes;

	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.Landscape;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void buttonPush(string buttonName) {
        switch (buttonName) {
            case "New":
				Player.resetPlayer(unitTypes);
                Application.LoadLevel("LevelSelect");
                break;
            case "Continue":
				Player.loadPlayer(unitTypes);
                Application.LoadLevel("LevelSelect");
                break;
            case "Quit":
                Application.Quit();
                break;
            default:
                break;
        }
    }
}
