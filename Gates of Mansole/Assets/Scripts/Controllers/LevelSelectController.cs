using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectController : MonoBehaviour {

    public GameObject level1;
    public GameObject BeatGameMessage;
	public GameObject[] Maps;

    private List<GameObject> levelButtons;

	// Use this for initialization
	void Start () {

        // Always let the player access level 1
		level1.SetActive(true);
		level1.transform.position = new Vector3 (Player.levelLocs [0].x, Player.levelLocs [0].y, level1.transform.position.z);
        levelButtons = new List<GameObject>();
        levelButtons.Add(level1);

		foreach (GameObject map in Maps) {
			map.SetActive(false);
		}
		if (Player.map < Maps.Length) {
			Maps[Player.map].SetActive(true);
		}

        for (int i = 1; i < Player.levelFileNames.Count; i++) {
            if (Player.levelComplete[i - 1] > 0) {
                levelButtons.Add(Instantiate(level1, level1.transform.position, Quaternion.identity) as GameObject);
                levelButtons[i].name = (i + 1).ToString();
                levelButtons[i].GetComponent<UiButton>().buttonName = levelButtons[i].name;
                levelButtons[i].GetComponent<UiButton>().textMeshObj.GetComponent<TextMesh>().text = levelButtons[i].name;
                levelButtons[i].GetComponent<UiButton>().controller = gameObject;
				levelButtons[i].transform.position = new Vector3(Player.levelLocs[i].x, Player.levelLocs[i].y, levelButtons[i].transform.position.z);
            }
        }

        if (Player.getNumLevelsBeaten() >= Player.levelComplete.Count) {
            BeatGameMessage.SetActive(true);
        } else {
			BeatGameMessage.SetActive(false);

			// Make the last level bigger
			levelButtons [levelButtons.Count - 1].transform.localScale = new Vector3 (0.75f, 0.75f, 1);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void buttonPush(string buttonName) {
		switch (buttonName) {
			case "Back":
				Application.LoadLevel("Title");
				break;
			case "UpgradeShop":
				Application.LoadLevel("Upgrade");
				break;
            default:
	            Player.currentLevel = int.Parse(buttonName) - 1;
	            Player.nextLevelFile = Player.levelFileNames[Player.currentLevel];
	            Debug.Log(Player.nextLevelFile);
	            Application.LoadLevel("AutoLevel");
                break;
        }
    }
}
