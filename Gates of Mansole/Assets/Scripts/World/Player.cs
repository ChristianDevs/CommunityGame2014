using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	const float CONVERSION_RATE = 0.2f; // shards to orbs

    static public int level1Complete;
    static public int level2Complete;
    static public int level3Complete;
    static public int level4Complete;
    static public int currentLevel;
	static public int spiritShards; // currency to purchase units/upgrades during gameplay
	static public int totalShards;  // tracks shards gained during gameplay - used for conversions
	static public int spiritOrbs;   // currency to purchase upgrades outside of gameplay
	static public List<GameObject> unitTypes;

    static public void resetPlayer(GameObject[] newUnitTypes) {
        PlayerPrefs.DeleteAll();

        level1Complete = 0;
        level2Complete = 0;
        level3Complete = 0;
        level4Complete = 0;

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
        level1Complete = PlayerPrefs.GetInt("level1Complete");
        level2Complete = PlayerPrefs.GetInt("level2Complete");
        level3Complete = PlayerPrefs.GetInt("level3Complete");
        level4Complete = PlayerPrefs.GetInt("level4Complete");
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

        switch (levelNum) {
            case 1:
                level1Complete = 1;
                PlayerPrefs.SetInt("level1Complete", 1);
                break;
            case 2:
                level2Complete = 1;
                PlayerPrefs.SetInt("level2Complete", 1);
                break;
            case 3:
                level3Complete = 1;
                PlayerPrefs.SetInt("level3Complete", 1);
                break;
            case 4:
                level4Complete = 1;
                PlayerPrefs.SetInt("level4Complete", 1);
                break;
        }
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
