using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectController : MonoBehaviour {

    public GameObject level1;
    public GameObject BeatGameMessage;

    private List<GameObject> levelButtons;

	// Use this for initialization
	void Start () {
        float x;
        float y;

        // Always let the player access level 1
        level1.SetActive(true);
        levelButtons = new List<GameObject>();
        levelButtons.Add(level1);

        x = level1.transform.position.x + 3f;
        y = level1.transform.position.y;

        for (int i = 1; i < Player.levelFileNames.Count; i++) {
            if (Player.levelComplete[i - 1] > 0) {
                levelButtons.Add(Instantiate(level1, level1.transform.position, Quaternion.identity) as GameObject);
                levelButtons[i].name = "Level " + (i + 1).ToString();
                levelButtons[i].GetComponent<UiButton>().buttonName = levelButtons[i].name;
                levelButtons[i].GetComponent<UiButton>().textMeshObj.GetComponent<TextMesh>().text = levelButtons[i].name;
                levelButtons[i].GetComponent<UiButton>().controller = gameObject;
                levelButtons[i].transform.position = new Vector3(x, y, levelButtons[i].transform.position.z);

                x += 3;

                if (x > 6) {
                    y -= 1;
                    x = level1.transform.position.x;
                }
            }
        }

        if (Player.getNumLevelsBeaten() >= Player.levelComplete.Count) {
            BeatGameMessage.SetActive(true);
        } else {
            BeatGameMessage.SetActive(false);
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
                if (buttonName.StartsWith("Level")) {
                    Player.currentLevel = int.Parse(buttonName.Split(' ')[1]) - 1;
                    Player.nextLevelFile = Player.levelFileNames[Player.currentLevel];
                    Debug.Log(Player.nextLevelFile);
                    Application.LoadLevel("AutoLevel");
                }
                break;
        }
    }
}
