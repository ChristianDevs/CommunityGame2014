using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiUpgrades : MonoBehaviour {

    enum _UpgradeMode {
        Units,
        Abilities
    }

	public TextMesh unitInfoText;
	public GameObject unitWindow;
	public float startUnitX;
	public float startUnitY;
	public float incUnitX;
	public float incUnitY;

	private List<GameObject> upgrades;
	private List<GameObject> upgradeWindows;
	private int selectedType;
    private _UpgradeMode mode;

	// Use this for initialization
	void Start () {
        mode = _UpgradeMode.Units;
        buildUnitUpgrade();
	}

    void buildUnitUpgrade() {
        float x = startUnitX;
        float y = startUnitY;

        if (upgrades != null) {
            foreach (GameObject upgrade in upgrades) {
                Destroy(upgrade);
            }
        }

        if (upgradeWindows != null) {
            foreach (GameObject upWin in upgradeWindows) {
                Destroy(upWin);
            }
        }

        upgrades = new List<GameObject>();
        upgradeWindows = new List<GameObject>();
        for (int i = 0; i < Player.unitTypes.Count; i++) {
            upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
            upgradeWindows[upgradeWindows.Count - 1].transform.name = Player.unitTypes[i].GetComponent<UiUnitType>().UnitName + "0";
            upgrades.Add(Instantiate(Player.unitTypes[i].GetComponent<UiUnitType>().getRandomUnit(), new Vector3(x, y), Quaternion.identity) as GameObject);
            upgrades[upgrades.Count - 1].GetComponent<GomUnit>().enabled = false;

            for (int j = 0; j < Player.unitTypes[i].GetComponent<UiUnitType>().level; j++) {
                y += incUnitY;

                upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
                upgradeWindows[upgradeWindows.Count - 1].transform.name = Player.unitTypes[i].GetComponent<UiUnitType>().UnitName + (j + 1);
                upgrades.Add(Instantiate(Player.unitTypes[i].GetComponent<UiUnitType>().getRandomUnit(), new Vector3(x, y), Quaternion.identity) as GameObject);
                upgrades[upgrades.Count - 1].GetComponent<GomUnit>().enabled = false;
            }

            x += incUnitX;
            y = startUnitY;
        }
    }

    void buildAbilityUpgrade() {
        float x = startUnitX;
        float y = startUnitY;

        if (upgrades != null) {
            foreach (GameObject upgrade in upgrades) {
                Destroy(upgrade);
            }
        }

        if (upgradeWindows != null) {
            foreach (GameObject upWin in upgradeWindows) {
                Destroy(upWin);
            }
        }

        upgrades = new List<GameObject>();
        upgradeWindows = new List<GameObject>();

        for (int i = 0; i < Player.abilities.Count; i++) {
            upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
            upgradeWindows[upgradeWindows.Count - 1].transform.name = Player.abilities[i].GetComponent<Ability>().abilityName + "0";
            upgrades.Add(Instantiate(Player.abilities[i].GetComponent<Ability>().sprite, new Vector3(x, y), Quaternion.identity) as GameObject);

            for (int j = 0; j < Player.abilities[i].GetComponent<Ability>().level; j++) {
                y += incUnitY;

                upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
                upgradeWindows[upgradeWindows.Count - 1].transform.name = Player.abilities[i].GetComponent<Ability>().abilityName + (j + 1);
                upgrades.Add(Instantiate(Player.abilities[i].GetComponent<Ability>().sprite, new Vector3(x, y), Quaternion.identity) as GameObject);
            }

            x += incUnitX;
            y = startUnitY;
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

                        if (mode == _UpgradeMode.Units) {
                            for (int j = 0; j < Player.unitTypes.Count; j++) {
                                if (typeName == Player.unitTypes[j].GetComponent<UiUnitType>().UnitName) {
                                    selectedType = j;
                                    UpdateDisplay();
                                }
                            }
                        }
                        else if (mode == _UpgradeMode.Abilities) {
                            for (int j = 0; j < Player.abilities.Count; j++) {
                                if (typeName == Player.abilities[j].GetComponent<Ability>().abilityName) {
                                    selectedType = j;
                                    UpdateDisplay();
                                }
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
            if (mode == _UpgradeMode.Units) {
                int orbCost = (Player.unitTypes[selectedType].GetComponent<UiUnitType>().level * 2) + 2;
                if (Player.spiritOrbs >= orbCost) {
                    float x = startUnitX + (selectedType * incUnitX);
                    float y = startUnitY + ((Player.unitTypes[selectedType].GetComponent<UiUnitType>().level + 1) * incUnitY);

                    Player.unitTypes[selectedType].GetComponent<UiUnitType>().level++;
                    Player.upgradeUnit(Player.unitTypes[selectedType].GetComponent<UiUnitType>());
                    Debug.Log("Upgraded Unit " + selectedType);

                    upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
                    upgradeWindows[upgradeWindows.Count - 1].transform.name =
                        Player.unitTypes[selectedType].GetComponent<UiUnitType>().UnitName +
                        Player.unitTypes[selectedType].GetComponent<UiUnitType>().level;
                    upgrades.Add(Instantiate(Player.unitTypes[selectedType].GetComponent<UiUnitType>().getRandomUnit(), new Vector3(x, y), Quaternion.identity) as GameObject);
                    upgrades[upgrades.Count - 1].GetComponent<GomUnit>().enabled = false;
                    Player.AddOrbs(-orbCost);
                    UpdateDisplay();
                }
            } else if (mode == _UpgradeMode.Abilities) {
                int orbCost = (Player.abilities[selectedType].GetComponent<Ability>().level * 2) + 2;
                if (Player.spiritOrbs >= orbCost) {
                    float x = startUnitX + (selectedType * incUnitX);
                    float y = startUnitY + ((Player.abilities[selectedType].GetComponent<Ability>().level + 1) * incUnitY);

                    Player.abilities[selectedType].GetComponent<Ability>().level++;
                    Player.upgradeAbility(Player.abilities[selectedType].GetComponent<Ability>());
                    Debug.Log("Upgraded Ability " + selectedType);

                    upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
                    upgradeWindows[upgradeWindows.Count - 1].transform.name =
                        Player.unitTypes[selectedType].GetComponent<UiUnitType>().UnitName +
                        Player.unitTypes[selectedType].GetComponent<UiUnitType>().level;
                    upgrades.Add(Instantiate(Player.abilities[selectedType].GetComponent<Ability>().sprite, new Vector3(x, y), Quaternion.identity) as GameObject);
                    Player.AddOrbs(-orbCost);
                    UpdateDisplay();
                }
            }
			break;
        case "Switch":
            Debug.Log("Switch");

            if (mode == _UpgradeMode.Units) {
                mode = _UpgradeMode.Abilities;
                buildAbilityUpgrade();
            } else if (mode == _UpgradeMode.Abilities) {
                mode = _UpgradeMode.Units;
                buildUnitUpgrade();
            }

            break;
		default:
			break;
		}
	}

	void UpdateDisplay() {
		string textToDisplay = "";

        if (mode == _UpgradeMode.Units) {
            UiUnitType ut;

            ut = Player.unitTypes[selectedType].GetComponent<UiUnitType>();

            textToDisplay = "Name: ";
            textToDisplay += ut.UnitName;
            textToDisplay += "\n";

            textToDisplay += "Level: ";
            textToDisplay += ut.level;
            textToDisplay += "\n";

            textToDisplay += "Upgrade Cost: ";
            textToDisplay += ut.level * 2 + 2;
            textToDisplay += "\n";
        } else if (mode == _UpgradeMode.Abilities) {
            Ability ab;

            ab = Player.abilities[selectedType].GetComponent<Ability>();

            textToDisplay = "Ability: ";
            textToDisplay += ab.abilityName;
            textToDisplay += "\n";

            textToDisplay += "Level: ";
            textToDisplay += ab.level;
            textToDisplay += "\n";

            textToDisplay += "Upgrade Cost: ";
            textToDisplay += ab.level * 2 + 2;
            textToDisplay += "\n";
        }

		unitInfoText.text = textToDisplay;
	}
}
