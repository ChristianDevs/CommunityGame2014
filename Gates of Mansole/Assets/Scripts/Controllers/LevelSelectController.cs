using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectController : MonoBehaviour {

    public GameObject level1;
    public GameObject BeatGameMessage;
	public GameObject[] Maps;
	public GameObject ChapterNumUI;
	public GameObject highlightPS;

    private List<GameObject> levelButtons;

	// Use this for initialization
	void Start () {
		UpdateMap();
	}
	
	// Update is called once per frame
	void Update () {
		ChapterNumUI.GetComponent<TextMesh> ().text = (Player.currentChapter + 1).ToString ();
	}

	void UpdateMap() {
		foreach (GameObject map in Maps) {
			map.SetActive(false);
		}
		if (Player.currentChapter < Maps.Length) {
			Maps[Player.maps[Player.currentChapter]].SetActive(true);
		}

		if (levelButtons != null) {
			for (int i = 1; i < levelButtons.Count; i++) {
				Destroy(levelButtons[i]);
			}
		}
		
		// Always let the player access level 1
		level1.SetActive(true);
		level1.transform.position = new Vector3 (Player.levelLocs[Player.currentChapter][0].x, Player.levelLocs[Player.currentChapter][0].y, level1.transform.position.z);
		levelButtons = new List<GameObject>();
		levelButtons.Add(level1);
		levelButtons [levelButtons.Count - 1].transform.localScale = new Vector3 (0.6f, 0.6f, 1);

		for (int i = 1; i < Player.levelFileNames[Player.currentChapter].Count; i++) {
			if (Player.levelComplete[Player.currentChapter][i - 1] > 0) {
				levelButtons.Add(Instantiate(level1, level1.transform.position, Quaternion.identity) as GameObject);
				levelButtons[i].name = (i + 1).ToString();
				levelButtons[i].GetComponent<UiButton>().buttonName = levelButtons[i].name;
				levelButtons[i].GetComponent<UiButton>().textMeshObj.GetComponent<TextMesh>().text = levelButtons[i].name;
				levelButtons[i].GetComponent<UiButton>().controller = gameObject;
				levelButtons[i].transform.position = new Vector3(Player.levelLocs[Player.currentChapter][i].x, Player.levelLocs[Player.currentChapter][i].y, levelButtons[i].transform.position.z);
				levelButtons [levelButtons.Count - 1].transform.localScale = new Vector3 (0.6f, 0.6f, 1);
			}
		}
		
		if (Player.getNumLevelsBeaten(Player.currentChapter) >= Player.levelComplete[Player.currentChapter].Count) {
			BeatGameMessage.SetActive(true);
		} else {
			BeatGameMessage.SetActive(false);
			
			// Make the last level bigger
			levelButtons [levelButtons.Count - 1].transform.localScale = new Vector3 (0.75f, 0.75f, 1);

			// Particle System to draw user's attention to the new level
			Instantiate(highlightPS, levelButtons[levelButtons.Count - 1].transform.position, Quaternion.identity);
		}
	}

    void buttonPush(string buttonName) {
		switch (buttonName) {
			case "Back":
				Application.LoadLevel("Title");
				break;
			case "Upgrade":
				Application.LoadLevel("Upgrade");
			break;
			case "Previous":
				if (Player.currentChapter > 0) {
					Player.currentChapter--;
					UpdateMap();
				}
			break;
			case "Next":
				if (Player.currentChapter < Player.chapterProgression) {
					Player.currentChapter++;
					UpdateMap();
					if (Player.introCinematicsWatched[Player.currentChapter] == false) {
						Player.isWatchingIntro = true;
						Application.LoadLevel("Cinematic");
					}
				}
			break;
            default:
	            Player.currentLevel = int.Parse(buttonName) - 1;
				Player.nextLevelFile = Player.levelFileNames[Player.currentChapter][Player.currentLevel];
	            Debug.Log(Player.nextLevelFile);
				Application.LoadLevel("AutoLevel");
                break;
        }
    }
}
