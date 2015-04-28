using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleController : MonoBehaviour {

	public GameObject BlackScreen;
	public GameObject[] unitTypes;
    public GameObject[] abilities;
	public GameObject Background;
	public GameObject Logo;
	public GameObject[] Buttons;

	private bool isDone;
	private float alpha;

	// Use this for initialization
	void Start () {
		Color color = Color.black;
		isDone = false;
		StartCoroutine (Init ());

		alpha = 1;
		color.a = alpha;
		BlackScreen.SetActive (true);
		BlackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = color;
		Logo.SetActive (false);
		foreach(GameObject btn in Buttons) {
			btn.SetActive(false);
		}
	}

	void Update() {
		Color color = Color.black;

		if (alpha > 0) {
			alpha -= 0.03f;
			color.a = alpha;
			BlackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = color;
		} else if (alpha < 0) {
			Logo.SetActive (true);
			foreach(GameObject btn in Buttons) {
				btn.SetActive(true);
			}
			alpha = 0;
		} else if (Background.transform.position.y > -20.5f) {
			Background.transform.position -= new Vector3(0, Time.deltaTime * 0.5f, 0);
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
		Player.chapterIntroCinematicFiles = new List<string> ();
		Player.chapterExitCinematicFiles = new List<string> ();
		foreach (string ln in levelData) {
			string key = "";
			
			if (ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
				key = ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[0].ToLower().TrimStart();
			} else {
				key = ln.ToLower().TrimStart().TrimEnd();
			}
			
			if (key == "map") {
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length >= 2) {
					Player.maps.Add (int.Parse(ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1]));
				}
			} else if (key == "chapter") {
				Player.levelFileNames.Add(new List<string>());
				Player.levelLocs.Add(new List<Vector2>());
			} else if (key == "intro") {
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length >= 2) {
					Player.chapterIntroCinematicFiles.Add (Application.streamingAssetsPath.Replace("Raw", "StreamingAssets") + "/" + ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].TrimStart());
				}
			} else if (key == "exit") {
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length >= 2) {
					Player.chapterExitCinematicFiles.Add (Application.streamingAssetsPath.Replace("Raw", "StreamingAssets") + "/" + ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].TrimStart());
				}
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
				Player.isWatchingIntro = true;
                Application.LoadLevel("Cinematic");
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
