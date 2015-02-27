using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleController : MonoBehaviour {

	public GameObject BlackScreen;
	public GameObject[] unitTypes;
    public GameObject[] abilities;

	private bool isDone;
	private float alpha;

	// Use this for initialization
	void Start () {
		Color color = Color.black;
		isDone = false;
		StartCoroutine (Init ());

		alpha = 1;
		color.a = alpha;
		BlackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = color;
	}

	void Update() {
		Color color = Color.black;

		if (alpha > 0) {
			alpha -= 0.025f;
			color.a = alpha;
			BlackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = color;
		}
	}

	public IEnumerator Init() {
		string filePath;
		string[] levelData;
		string[] seps = {"\n"};
		string[] sepsLine = {":"};
		
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

		Player.levelFileNames = new List<List<string>>();
		Player.levelLocs = new List<List<Vector2>> ();
		Player.maps = new List<int> ();
		foreach (string ln in levelData) {
			if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[0].ToLower().TrimStart() == "map") {
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length >= 2) {
					Player.maps.Add (int.Parse(ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1]));
				}
			} else if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[0].ToLower() == "chapter") {
				Player.levelFileNames.Add(new List<string>());
				Player.levelLocs.Add(new List<Vector2>());
			} else if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length >= 3) {
				if (Player.levelFileNames.Count > 0) {
		            // Apply web fix to level files
					Player.levelFileNames[Player.levelFileNames.Count-1].Add(Application.streamingAssetsPath.Replace("Raw", "StreamingAssets") + "/" + ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[0].TrimStart());
					Player.levelLocs[Player.levelFileNames.Count-1].Add(new Vector2(float.Parse(ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1]), float.Parse(ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[2])));
				}
			}
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
			case "Credits":
				Player.loadPlayer(unitTypes, abilities);
				Application.LoadLevel("Credits");
				break;
            case "Quit":
                Application.Quit();
                break;
            default:
                break;
        }
    }
}
