using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleController : MonoBehaviour {

	public GameObject[] unitTypes;
    public GameObject[] abilities;

	private bool isDone;

	// Use this for initialization
	void Start () {
		isDone = false;
		StartCoroutine (Init ());
	}

	public IEnumerator Init() {
		string filePath;
		string[] levelData;
		string[] seps = {"\n"};
		
		filePath = Application.streamingAssetsPath + "/Data.gom";

        // Web bug where streamingAssetsPath is "Raw" instead of "StreamingAssets"
        filePath = filePath.Replace("Raw", "StreamingAssets");

		if (filePath.Contains("://"))
		{
			WWW www = new WWW (filePath);
			yield return www;
			levelData = www.text.Split(seps, System.StringSplitOptions.RemoveEmptyEntries);
		} else {
			levelData = System.IO.File.ReadAllLines(filePath);
		}

		Player.levelFileNames = new List<string>();
		foreach (string ln in levelData) {
            // Apply web fix to level files
			Player.levelFileNames.Add(Application.streamingAssetsPath.Replace("Raw", "StreamingAssets") + "/" + ln);
		}
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
