using UnityEngine;
using System.Collections;

public class UiLevelSelect : MonoBehaviour {

    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public GameObject level4;

    public GameObject BeatGameMessage;

	// Use this for initialization
	void Start () {

        // Always let the player access level 1
        level1.SetActive(true);

        // Let the player access level 2 when they beat level 1
        if (Player.level1Complete > 0) {
            level2.SetActive(true);
        } else {
            level2.SetActive(false);
        }

        // Let the player access level 3 when they beat level 2
        if (Player.level2Complete > 0) {
            level3.SetActive(true);
        }
        else {
            level3.SetActive(false);
        }

        // Let the player access level 4 when they beat level 3
        if (Player.level3Complete > 0) {
            level4.SetActive(true);
        }
        else {
            level4.SetActive(false);
        }

        if (Player.level4Complete > 0) {
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
            case "Level1":
                Player.currentLevel = 1;
                Application.LoadLevel("DefendSceneLeft");
                break;
            case "Level2":
                Player.currentLevel = 2;
                Application.LoadLevel("DefendSceneRight");
                break;
            case "Level3":
                Player.currentLevel = 3;
                Application.LoadLevel("AttackSceneLeft");
                break;
            case "Level4":
                Player.currentLevel = 4;
                Application.LoadLevel("AttackSceneRight");
                break;
            default:
                break;
        }
    }
}
