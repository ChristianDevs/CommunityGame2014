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
			Player.levelFileNames.Add(Application.streamingAssetsPath + "/" + ln);
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
