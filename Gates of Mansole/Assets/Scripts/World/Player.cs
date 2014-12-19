using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	const float CONVERSION_RATE = 0.2f; // shards to orbs

    static public List<int> levelComplete;
    static public int currentLevel;
	static public int spiritShards; // currency to purchase units/upgrades during gameplay
	static public int totalShards;  // tracks shards gained during gameplay - used for conversions
	static public int spiritOrbs;   // currency to purchase upgrades outside of gameplay
	static public List<GameObject> unitTypes;
    static public string nextLevelFile;
    static public List<string> levelFileNames;

    static public void resetPlayer(GameObject[] newUnitTypes) {
        PlayerPrefs.DeleteAll();
        levelComplete = new List<int>();
        foreach (string fileName in levelFileNames) {
            levelComplete.Add(0);
        }

        AddOrbs(10);

		unitTypes = new List<GameObject>();
		foreach (GameObject ut in newUnitTypes) {
			if (ut.GetComponent<UiUnitType>().UnitName == "Bow") {
				ut.GetComponent<UiUnitType>().level = 2;
			} else {
				ut.GetComponent<UiUnitType>().level = 0;
			}
            unitTypes.Add(ut);
            upgradeUnit(ut.GetComponent<UiUnitType>());
		}

    }

    static public void loadPlayer(GameObject[] newUnitTypes) {
        levelComplete = new List<int>();
        foreach (string fileName in levelFileNames) {
            levelComplete.Add(PlayerPrefs.GetInt(fileName));
        }
		spiritOrbs = PlayerPrefs.GetInt("spiritOrbs");

		unitTypes = new List<GameObject>();
		foreach (GameObject ut in newUnitTypes) {
			unitTypes.Add(ut);
		}

		for (int i = 0; i < unitTypes.Count; i++) {
			UiUnitType uiUnit;

			uiUnit = unitTypes[i].GetComponent<UiUnitType>();
        		uiUnit.level = PlayerPrefs.GetInt(uiUnit.UnitName + "level");
		}
    }

	static public void upgradeUnit(UiUnitType upgradeUnit) {
		for (int i = 0; i < unitTypes.Count; i++) {
			if (unitTypes[i].GetComponent<UiUnitType>().UnitName == upgradeUnit.UnitName) {
				UiUnitType uiUnit;

                uiUnit = unitTypes[i].GetComponent<UiUnitType>();
				PlayerPrefs.SetInt(uiUnit.UnitName + "level", uiUnit.level);
				break;
			}
		}
	}

    static public void completeLevel(int levelNum) {
		convertShards ();

        if (levelNum >= levelFileNames.Count) {
            return;
        }

        Debug.Log("Completing " + levelFileNames[levelNum]);
        levelComplete[levelNum] = 1;
        PlayerPrefs.SetInt(levelFileNames[levelNum], 1);
    }

    static public int getNumLevelsBeaten() {
        int num = 0;

        foreach (int lvl in levelComplete) {
            if (lvl > 0) {
                num++;
            }
        }

        return num;
    }

	static public void convertShards() {
		spiritOrbs = (int)Mathf.Floor (totalShards * CONVERSION_RATE);
		Debug.Log (spiritOrbs + " orbs gained.");
		int currentOrbs = PlayerPrefs.GetInt("spiritOrbs", 0);
		spiritOrbs += currentOrbs;
		PlayerPrefs.SetInt("spiritOrbs", spiritOrbs);
		spiritShards = 0;
		totalShards = 0;
	}

	static public void AddOrbs(int amt) {
		spiritOrbs += amt;
		PlayerPrefs.SetInt("spiritOrbs", spiritOrbs);
	}
}
