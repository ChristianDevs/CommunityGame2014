using UnityEngine;
using System.Collections;

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

    static public void resetPlayer() {
        level1Complete = 0;
        level2Complete = 0;
        level3Complete = 0;
        level4Complete = 0;
		spiritOrbs = 0;

        PlayerPrefs.DeleteAll();
    }

    static public void loadPlayer() {
        level1Complete = PlayerPrefs.GetInt("level1Complete");
        level2Complete = PlayerPrefs.GetInt("level2Complete");
        level3Complete = PlayerPrefs.GetInt("level3Complete");
        level4Complete = PlayerPrefs.GetInt("level4Complete");
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
		int currentOrbs = PlayerPrefs.GetInt("spiritOrbs", 0);
		PlayerPrefs.SetInt("spiritOrbs", spiritOrbs+currentOrbs);
		spiritShards = 0;
		totalShards = 0;
		Debug.Log (spiritOrbs + " orbs gained.");
	}
}
