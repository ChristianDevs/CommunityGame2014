using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiTitle : MonoBehaviour {

	public GameObject[] unitTypes;
    public GameObject[] abilities;

	// Use this for initialization
	void Start () {
        string[] levelData;

		Screen.orientation = ScreenOrientation.Landscape;

        levelData = System.IO.File.ReadAllLines("Data.gom");

        Player.levelFileNames = new List<string>();
        foreach (string ln in levelData) {
            Player.levelFileNames.Add(ln);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void buttonPush(string buttonName) {
        switch (buttonName) {
            case "New":
                Player.resetPlayer(unitTypes, abilities);
                Application.LoadLevel("LevelSelect");
                break;
            case "Continue":
                Player.loadPlayer(unitTypes, abilities);
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
