using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectController : MonoBehaviour {

    public GameObject level1;
    public GameObject BeatGameMessage;
	public GameObject[] Maps;
	public GameObject ChapterNumUI;

	private int curChapter;
    private List<GameObject> levelButtons;

	// Use this for initialization
	void Start () {
		
		curChapter = Player.chapterProgression;

		UpdateMap();
	}
	
	// Update is called once per frame
	void Update () {
		ChapterNumUI.GetComponent<TextMesh> ().text = (curChapter + 1).ToString ();
	}

	void UpdateMap() {
		foreach (GameObject map in Maps) {
			map.SetActive(false);
		}
		if (curChapter < Maps.Length) {
			Maps[curChapter].SetActive(true);
		}

		if (levelButtons != null) {
			for (int i = 1; i < levelButtons.Count; i++) {
				Destroy(levelButtons[i]);
			}
		}
		
		// Always let the player access level 1
		level1.SetActive(true);
		level1.transform.position = new Vector3 (Player.levelLocs[curChapter][0].x, Player.levelLocs[curChapter][0].y, level1.transform.position.z);
		levelButtons = new List<GameObject>();
		levelButtons.Add(level1);
		
		for (int i = 1; i < Player.levelFileNames[curChapter].Count; i++) {
			if (Player.levelComplete[curChapter][i - 1] > 0) {
				levelButtons.Add(Instantiate(level1, level1.transform.position, Quaternion.identity) as GameObject);
				levelButtons[i].name = (i + 1).ToString();
				levelButtons[i].GetComponent<UiButton>().buttonName = levelButtons[i].name;
				levelButtons[i].GetComponent<UiButton>().textMeshObj.GetComponent<TextMesh>().text = levelButtons[i].name;
				levelButtons[i].GetComponent<UiButton>().controller = gameObject;
				levelButtons[i].transform.position = new Vector3(Player.levelLocs[curChapter][i].x, Player.levelLocs[curChapter][i].y, levelButtons[i].transform.position.z);
			}
		}
		
		if (Player.getNumLevelsBeaten(curChapter) >= Player.levelComplete[curChapter].Count) {
			BeatGameMessage.SetActive(true);
		} else {
			BeatGameMessage.SetActive(false);
			
			// Make the last level bigger
			levelButtons [levelButtons.Count - 1].transform.localScale = new Vector3 (0.75f, 0.75f, 1);
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
				if (curChapter > 0) {
					curChapter--;
					UpdateMap();
				}
			break;
			case "Next":
				if (curChapter < Player.chapterProgression) {
					curChapter++;
					UpdateMap();
				}
			break;
            default:
	            Player.currentLevel = int.Parse(buttonName) - 1;
				Player.nextLevelFile = Player.levelFileNames[curChapter][Player.currentLevel];
	            Debug.Log(Player.nextLevelFile);
	            Application.LoadLevel("AutoLevel");
                break;
        }
    }
}
