using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public const float DEFEND_CONVERSION_RATE = 0.05f; // 20 shards for 1 orb
	public const float ASSAULT_CONVERSION_RATE = 0.5f; // 2 shards for 1 orb

    static public List<List<int>> levelComplete;
    static public int currentLevel;		// Level the player is currently playing
	static public int currentChapter;	// Chapter of the level the player is currently playing
	static public int spiritShards; // currency to purchase units/upgrades during gameplay
	static public int totalShards;  // tracks shards gained during gameplay - used for conversions
	static public int spiritOrbs;   // currency to purchase upgrades outside of gameplay
	static public List<GameObject> unitTypes;
    static public List<GameObject> abilities;
    static public string nextLevelFile;
    static public List<List<string>> levelFileNames;
	static public List<List<Vector2>> levelLocs;
	static public List<int> maps;
	static public List<string> chapterIntroCinematicFiles;
	static public List<bool> introCinematicsWatched;
	static public List<string> chapterExitCinematicFiles;
	static public List<bool> exitCinematicsWatched;
	static public bool isWatchingIntro;
	static public int chapterProgression;
	static public List<bool> unitAvailable;
	// Tutorials
	// ------------------------------------------------------------
	// 1) Click Level 1
	//  a) Place a Shepherd
	//  b) Explain unit will attack when enemies in range
	//  c) Pickup a Spirit Shard
	//  d) Explain Spirit Shards
	//  e) Explain the Attacker Bar
	//  f) Explain the Wave Bar
	// 2) Go to the Market
	//  a) Select the shepherd
	//  b) Upgrade the shepherd
	// 3) Explain Spirit Orbs
	// 4) Explain Unit Counters
	//  a) Exit the Market
	// 5) Click Level 2
	// a) Place a shepherd
	//  b) Upgrade the shepherd
	//  c) Explain the early release
	//  d) Explain shard swiping for bonus
	//  e) Go to the Market
	// 6) Unlock the evangelist
	//  a) Exit the Market
	// 7) At level 6 explain that placed units will advance
	// 8) At level 17 explain the wall needs all lanes occupied to bring it down
	static public int tutorialState;

	static public bool MusicEnable;
	static public bool SoundEnable;

    static public void resetPlayer(GameObject[] newUnitTypes, GameObject[] newAbilities) {
        PlayerPrefs.DeleteAll();
        levelComplete = new List<List<int>>();
		foreach(List<string> chapters in levelFileNames) {
			levelComplete.Add(new List<int>());
			foreach (string fileName in chapters) {
				levelComplete[levelComplete.Count-1].Add(0);
	        }
		}

		introCinematicsWatched = new List<bool> ();
		foreach(string temp in chapterIntroCinematicFiles) {
			introCinematicsWatched.Add(false);
		}
		
		exitCinematicsWatched = new List<bool> ();
		foreach(string temp in chapterExitCinematicFiles) {
			exitCinematicsWatched.Add(false);
		}

        spiritOrbs = 0;
        AddOrbs(0);

		tutorialState = 0;
		
		unitTypes = new List<GameObject>();
		unitAvailable = new List<bool>();
		foreach (GameObject ut in newUnitTypes) {
			ut.GetComponent<UiUnitType>().getPlayerStats().hardResetStats();
			if (ut.GetComponent<UiUnitType>().UnitName.Equals("Shepherd")){
				ut.GetComponent<UiUnitType>().getPlayerStats().maxLevel=1;
				unitAvailable.Add (true);
			} else if(ut.GetComponent<UiUnitType>().UnitName.Equals("OrcBow")||
			        ut.GetComponent<UiUnitType>().UnitName.Equals("OrcSpear")||
			        ut.GetComponent<UiUnitType>().UnitName.Equals("OrcStaff")||
			        ut.GetComponent<UiUnitType>().UnitName.Equals("OrcSword")||
			        ut.GetComponent<UiUnitType>().UnitName.Equals("OrcWand")){
				
				unitAvailable.Add (false);
			} else{
				
				unitAvailable.Add (true);
			}
            unitTypes.Add(ut);
            upgradeUnit(ut.GetComponent<UiUnitType>());
		}

        abilities = new List<GameObject>();
        foreach (GameObject ab in newAbilities) {
            ab.GetComponent<Ability>().level = 0;
            abilities.Add(ab);
            upgradeAbility(ab.GetComponent<Ability>());
            Debug.Log(ab.GetComponent<Ability>().abilityName + ":" + ab.GetComponent<Ability>().level);
        }

		chapterProgression = 0;
		currentChapter = 0;
		PlayerPrefs.SetInt ("chapterProgression", chapterProgression);
		PlayerPrefs.SetFloat ("LastLogon", 0);
		PlayerPrefs.SetInt ("NumberConsecutiveLogons", 0);
		PlayerPrefs.Save ();
    }

	static public void loadPlayer(GameObject[] newUnitTypes, GameObject[] newAbilities) {
		unitAvailable = new List<bool>();
		foreach (GameObject ut in newUnitTypes) {
			if(ut.GetComponent<UiUnitType>().UnitName.Equals("OrcBow")||
			        ut.GetComponent<UiUnitType>().UnitName.Equals("OrcSpear")||
			        ut.GetComponent<UiUnitType>().UnitName.Equals("OrcStaff")||
			        ut.GetComponent<UiUnitType>().UnitName.Equals("OrcSword")||
			        ut.GetComponent<UiUnitType>().UnitName.Equals("OrcWand")){
				
				unitAvailable.Add (false);
			} else{
				
				unitAvailable.Add (true);
			}
		}

		levelComplete = new List<List<int>>();
		foreach(List<string> chapters in levelFileNames) {
			levelComplete.Add(new List<int>());
			foreach (string fileName in chapters) {
				levelComplete[levelComplete.Count-1].Add(PlayerPrefs.GetInt(fileName));
			}
		}
		spiritOrbs = PlayerPrefs.GetInt("spiritOrbs");
		
		introCinematicsWatched = new List<bool> ();
		foreach(string temp in chapterIntroCinematicFiles) {
			introCinematicsWatched.Add(PlayerPrefs.GetInt(temp) != 0);
		}
		
		exitCinematicsWatched = new List<bool> ();
		foreach(string temp in chapterExitCinematicFiles) {
			exitCinematicsWatched.Add(PlayerPrefs.GetInt(temp) != 0);
		}

		unitTypes = new List<GameObject>();
		foreach (GameObject ut in newUnitTypes) {
			unitTypes.Add(ut);
		}

		for (int i = 0; i < unitTypes.Count; i++) {
			UiUnitType uiUnit;

			uiUnit = unitTypes[i].GetComponent<UiUnitType>();
        	uiUnit.getPlayerStats().maxLevel = PlayerPrefs.GetInt(uiUnit.UnitName + "level");
		}

        abilities = new List<GameObject>();
        foreach (GameObject ab in newAbilities) {
            abilities.Add(ab);
        }

        for (int i = 0; i < abilities.Count; i++) {
            Ability tempAbility;

            tempAbility = abilities[i].GetComponent<Ability>();
            tempAbility.level = PlayerPrefs.GetInt(tempAbility.abilityName + "level");
        }

		chapterProgression = PlayerPrefs.GetInt ("chapterProgression");
		currentChapter = chapterProgression;

		tutorialState = PlayerPrefs.GetInt ("tutorialstate");
    }

	static public void watchedIntroCinematic() {
		introCinematicsWatched [currentChapter] = true;
		PlayerPrefs.SetInt (chapterIntroCinematicFiles [currentChapter], 1);
		PlayerPrefs.Save ();
	}
	
	static public void watchedExitCinematic() {
		exitCinematicsWatched [currentChapter] = true;
		PlayerPrefs.SetInt (chapterExitCinematicFiles [currentChapter], 1);
		PlayerPrefs.Save ();
	}

	static public void beatChapter() {
		chapterProgression = currentChapter + 1;
		PlayerPrefs.SetInt ("chapterProgression", chapterProgression);
		PlayerPrefs.Save ();
	}

    static public void upgradeUnit(UiUnitType upgradeUnit) {
		for (int i = 0; i < unitTypes.Count; i++) {
			if (unitTypes[i].GetComponent<UiUnitType>().UnitName == upgradeUnit.UnitName) {
				UiUnitType uiUnit;

                uiUnit = unitTypes[i].GetComponent<UiUnitType>();
                PlayerPrefs.SetInt(uiUnit.UnitName + "level", uiUnit.getPlayerStats().maxLevel);
				break;
			}
		}
		PlayerPrefs.Save ();
	}

    static public void upgradeAbility(Ability upgradeAbility) {
        for (int i = 0; i < abilities.Count; i++) {
            if (abilities[i].GetComponent<Ability>().abilityName == upgradeAbility.abilityName) {
                Ability uiAbility;

                uiAbility = abilities[i].GetComponent<Ability>();
                PlayerPrefs.SetInt(uiAbility.abilityName + "level", uiAbility.level);
                break;
            }
		}
		PlayerPrefs.Save ();
    }

	static public void completeLevel() {
		//convertShards ();

		if (currentChapter >= levelFileNames.Count) {
			return;
		}

		if (currentLevel >= levelFileNames[currentChapter].Count) {
            return;
        }

		Debug.Log("Completing " + levelFileNames[currentChapter][currentLevel]);
		levelComplete[currentChapter][currentLevel] = 1;
		PlayerPrefs.SetInt(levelFileNames[currentChapter][currentLevel], 1);

		if ((currentLevel == (levelFileNames[currentChapter].Count - 1)) &&
		    (currentChapter == chapterProgression)) {
			beatChapter();
		}
		PlayerPrefs.Save ();
    }

	static public int getNumLevelsBeaten(int chapterNum) {
        int num = 0;

		foreach (int lvl in levelComplete[chapterNum]) {
            if (lvl > 0) {
                num++;
            }
        }

        return num;
    }

	static public void AddOrbs(int amt) {
		spiritOrbs += amt;
		PlayerPrefs.SetInt("spiritOrbs", spiritOrbs);
		PlayerPrefs.Save ();
	}

	static public void completeTutorialState() {
		tutorialState++;
		PlayerPrefs.SetInt ("tutorialstate", tutorialState);
		Debug.Log ("Tutorial : " + tutorialState);
	}

	static public int getNumConsLogons() {
		return PlayerPrefs.GetInt ("NumberConsecutiveLogons");
	}

	static public string getLastLogon() {
		return PlayerPrefs.GetString ("LastLogon");
	}

	static public void addLogon() {
		int lastLogon;

		lastLogon = getNumConsLogons ();
		lastLogon++;

		PlayerPrefs.SetInt ("NumberConsecutiveLogons", lastLogon);
		PlayerPrefs.SetString ("LastLogon", System.DateTime.Parse(getLastLogon()).AddDays(1).ToString());
	}

	static public void resetLogon() {
		PlayerPrefs.SetInt ("NumberConsecutiveLogons", 0);
		PlayerPrefs.SetString ("LastLogon", System.DateTime.Now.ToString());
	}
}
