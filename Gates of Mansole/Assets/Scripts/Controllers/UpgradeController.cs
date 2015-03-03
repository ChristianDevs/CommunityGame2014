﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpgradeController : MonoBehaviour {

	public TextMesh unitInfoText;
	public GameObject unitWindow;
	public float startUnitX;
	public float startUnitY;
	public float startAbilityX;
	public float startAbilityY;
	public float incX;

	private List<GameObject> upgrades;
	private List<GameObject> upgradeWindows;
	private int selectedType;
	private int maxUnitUpgrades;
	private int maxAbilityUpgrades;

	// Use this for initialization
	void Start () {
		upgrades = new List<GameObject>();
		upgradeWindows = new List<GameObject>();
		maxUnitUpgrades = 10;
		maxAbilityUpgrades = 5;

        buildUnitUpgrade();
		buildAbilityUpgrade();
	}

    void buildUnitUpgrade() {
        float x = startUnitX;
        float y = startUnitY;

        for (int i = 0; i < Player.unitTypes.Count; i++) {
            upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
			upgradeWindows[upgradeWindows.Count - 1].transform.name = Player.unitTypes[i].GetComponent<UiUnitType>().UnitName;
			upgradeWindows[upgradeWindows.Count - 1].transform.localScale *= 1.5f;
            upgrades.Add(Instantiate(Player.unitTypes[i].GetComponent<UiUnitType>().getRandomUnit(), new Vector3(x, y), Quaternion.identity) as GameObject);
            upgrades[upgrades.Count - 1].GetComponent<GomUnit>().enabled = false;
			upgrades[upgrades.Count - 1].transform.localScale *= 1.5f;

			if (Player.unitTypes[i].GetComponent<UiUnitType>().getPlayerStats().maxLevel == 0) {
				upgrades[upgrades.Count - 1].SendMessage("SetColor", Color.black, SendMessageOptions.DontRequireReceiver);
			}

            x += incX;
        }
    }

    void buildAbilityUpgrade() {
        float x = startAbilityX;
        float y = startAbilityY;

        for (int i = 0; i < Player.abilities.Count; i++) {
            upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
			upgradeWindows[upgradeWindows.Count - 1].transform.name = Player.abilities[i].GetComponent<Ability>().abilityName;
			upgradeWindows[upgradeWindows.Count - 1].transform.localScale *= 1.5f;
			upgrades.Add(Instantiate(Player.abilities[i].GetComponent<Ability>().sprite, new Vector3(x, y), Quaternion.identity) as GameObject);
			upgrades[upgrades.Count - 1].transform.localScale *= 1.5f;

			if (Player.abilities[i].GetComponent<Ability>().level == 0) {
				upgrades[upgrades.Count - 1].GetComponent<SpriteRenderer>().color = Color.black;
			}

            x += incX;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hitSquare;
			
			if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitSquare)) {
				for(int i = 0; i < upgradeWindows.Count; i++) {
					if (hitSquare.transform.name == upgradeWindows[i].transform.name) {
						string typeName;

						char[] charsTrim = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

						typeName = hitSquare.transform.name.TrimEnd(charsTrim);

	                    for (int j = 0; j < Player.unitTypes.Count; j++) {
	                        if (typeName == Player.unitTypes[j].GetComponent<UiUnitType>().UnitName) {
	                            selectedType = j;
	                            UpdateDisplay();
	                        }
	                    }

						for (int j = 0; j < Player.abilities.Count; j++) {
	                        if (typeName == Player.abilities[j].GetComponent<Ability>().abilityName) {
								selectedType = j + Player.unitTypes.Count;
	                            UpdateDisplay();
	                        }
						}
					}
				}
			}
		}
	}
	
	void buttonPush(string buttonName) {
		switch (buttonName) {
		case "Back":
			Application.LoadLevel("LevelSelect");
			break;
		case "Upgrade":
			if (selectedType < Player.unitTypes.Count) {
                int orbCost = (Player.unitTypes[selectedType].GetComponent<UiUnitType>().getPlayerStats().maxLevel * 5) + 5;
                if ((Player.spiritOrbs >= orbCost) && (Player.unitTypes[selectedType].GetComponent<UiUnitType>().getPlayerStats().maxLevel < maxUnitUpgrades)  ) {
                    Player.unitTypes[selectedType].GetComponent<UiUnitType>().getPlayerStats().maxLevel++;
                    Player.upgradeUnit(Player.unitTypes[selectedType].GetComponent<UiUnitType>());
                    Debug.Log("Upgraded Unit " + selectedType);
                    Player.AddOrbs(-orbCost);
                    UpdateDisplay();
					upgrades[selectedType].SendMessage("SetColor", Color.white, SendMessageOptions.DontRequireReceiver);
                }
            } else {
				int abilityIndex = selectedType - Player.unitTypes.Count;
				int orbCost = (Player.abilities[abilityIndex].GetComponent<Ability>().level==0) ?  Player.abilities[abilityIndex].GetComponent<Ability>().cost : (Player.abilities[abilityIndex].GetComponent<Ability>().level * 2 + 2);
                if ((Player.spiritOrbs >= orbCost) && (Player.abilities[abilityIndex].GetComponent<Ability>().level < maxAbilityUpgrades) ) {
					Player.abilities[abilityIndex].GetComponent<Ability>().level++;
					Player.upgradeAbility(Player.abilities[abilityIndex].GetComponent<Ability>());
					Debug.Log("Upgraded Ability " + abilityIndex);
                    Player.AddOrbs(-orbCost);
					UpdateDisplay();
					upgrades[selectedType].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
			break;
		default:
			break;
		}
	}

	void UpdateDisplay() {
		string textToDisplay = "";

		if (selectedType < Player.unitTypes.Count) {
            UiUnitType ut;

            ut = Player.unitTypes[selectedType].GetComponent<UiUnitType>();

            textToDisplay = "Unit: ";
            textToDisplay += ut.UnitName;
            textToDisplay += "\n";

            textToDisplay += "Level: ";
            textToDisplay += ut.getPlayerStats().maxLevel;
            textToDisplay += "\n";

            textToDisplay += "Upgrade Cost: ";
            textToDisplay += ut.getPlayerStats().maxLevel* 5 + 5;
            textToDisplay += "\n";
        } else {
            Ability ab;

			ab = Player.abilities[selectedType - Player.unitTypes.Count].GetComponent<Ability>();

            textToDisplay = "Ability: ";
            textToDisplay += ab.abilityName;
            textToDisplay += "\n";

            textToDisplay += "Level: ";
            textToDisplay += ab.level;
            textToDisplay += "\n";

            textToDisplay += "Upgrade Cost: ";
            textToDisplay += ((ab.level==0)?ab.cost:ab.level*5+15);
            textToDisplay += "\n";
        }

		unitInfoText.text = textToDisplay;
	}
}
