using UnityEngine;
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

	public GameObject gutterflowPrefab;
	public GameObject CursorPrefab;
	public GameObject UnitCounterMsgTutorial;
	public GameObject SpiritOrbMsgTutorial;
	public GameObject UnitUpgradeMsgTutorial;
	public GameObject unitCounterPrefab;

	private List<GameObject> upgrades;
	private List<GameObject> upgradeWindows;
	private int selectedType;
	private int maxUnitUpgrades;
	private int maxAbilityUpgrades;

	private float nextFlow;
	private List<GameObject> gutterFlows;
	private int flowCounter;
	private GameObject cursorInst;
	private bool inTutorial;
	private GameObject unitCounterInst;

	// Use this for initialization
	void Start () {
		upgrades = new List<GameObject>();
		upgradeWindows = new List<GameObject>();
		maxUnitUpgrades = 10;
		maxAbilityUpgrades = 5;

		UnitCounterMsgTutorial.SetActive (false);
		SpiritOrbMsgTutorial.SetActive (false);
		UnitUpgradeMsgTutorial.SetActive (false);

        buildUnitUpgrade();
		buildAbilityUpgrade();

		Debug.Log (Player.tutorialState);

		gutterFlows = new List<GameObject> ();
		nextFlow = Random.Range (1, 5) + Time.time;
		flowCounter = 0;
		
		// Show the player to click the shepherd
		if (Player.tutorialState == 2) {
			cursorInst = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			cursorInst.transform.position = upgradeWindows[0].transform.position;
			cursorInst.GetComponentInChildren<Animator>().SetTrigger("DoTut2");
			inTutorial = true;
		} else if (Player.tutorialState == 3) {
			Player.tutorialState = 5;
		} else if (Player.tutorialState == 4) {
			Player.tutorialState = 5;
		} else if (Player.tutorialState == 6) {
			cursorInst = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			cursorInst.transform.position = upgradeWindows[1].transform.position;
			cursorInst.GetComponentInChildren<Animator>().SetTrigger("DoTut2");
			inTutorial = true;
		} else {
			cursorInst = null;
		}

		unitInfoText.text = "Press Unit or Ability\nto Upgrade it.";
	}

    void buildUnitUpgrade() {
        float x = startUnitX;
        float y = startUnitY;

        for (int i = 0; i < Player.unitTypes.Count; i++) {
			if(Player.unitAvailable[i]){
	            upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y - 0.5f), Quaternion.identity) as GameObject);
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

			if ((Player.tutorialState == 3) && (inTutorial)) {
				Player.completeTutorialState();
				UnitCounterMsgTutorial.SetActive(false);
				SpiritOrbMsgTutorial.SetActive(true);
			} else if ((Player.tutorialState == 4) && (inTutorial)) {
				Player.completeTutorialState();
				SpiritOrbMsgTutorial.SetActive(false);
				
				cursorInst = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				cursorInst.transform.position = new Vector3(-5.29f, 4.19f, 0f);
				cursorInst.GetComponentInChildren<Animator>().SetTrigger("DoTut2");
				inTutorial = false;
			}
			
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
								UpdateCounter();
	                        }
	                    }

						for (int j = 0; j < Player.abilities.Count; j++) {
	                        if (typeName == Player.abilities[j].GetComponent<Ability>().abilityName) {
								selectedType = j + Player.unitTypes.Count;
	                            UpdateDisplay();
	                        }
						}

						if ((Player.tutorialState == 2) && (inTutorial == true)) {
							if (typeName == "Shepherd") {
								cursorInst.transform.position = new Vector3(2.37f, 3.66f, 0);
							}
						} else if ((Player.tutorialState == 6) && (inTutorial == true)) {
							if (typeName == "Evangelist") {
								cursorInst.transform.position = new Vector3(2.37f, 3.66f, 0);
							}
						}
					}
				}
			}
		}


		if (Time.time >= nextFlow) {
			gutterFlows.Add(Instantiate(gutterflowPrefab, new Vector3(-7f, -4.8f), Quaternion.identity) as GameObject);
			gutterFlows[gutterFlows.Count - 1].name = "GutterFlow" + flowCounter.ToString();
			nextFlow = Random.Range (3, 8) + Time.time;
			flowCounter++;
		}

		GameObject removeFlow = null;
		foreach (GameObject flow in gutterFlows) {
			flow.transform.position += new Vector3(Time.deltaTime, 0);
			
			if (flow.transform.position.x >= 7) {
				removeFlow = flow;
			}
		}

		// Only need to remove 1 flow at a time
		if (removeFlow != null) {
			gutterFlows.Remove(removeFlow);
			Destroy(removeFlow);
		}
	}
	
	void buttonPush(string buttonName) {
		switch (buttonName) {
		case "Back":
			if (inTutorial == false) {
				Application.LoadLevel("LevelSelect");
			}
			break;
		case "Upgrade":
			if (selectedType < Player.unitTypes.Count) {
                int orbCost = Player.unitTypes[selectedType].GetComponent<UiUnitType>().getUpgradeCost();

				if (Player.unitTypes[selectedType].GetComponent<UiUnitType>().getPlayerStats().level >= Player.unitTypes[selectedType].GetComponent<UiUnitType>().UpgradeCosts.Length) {
					return;
				}

                if ((Player.spiritOrbs >= orbCost) && (Player.unitTypes[selectedType].GetComponent<UiUnitType>().getPlayerStats().maxLevel < maxUnitUpgrades)  ) {
                    Player.unitTypes[selectedType].GetComponent<UiUnitType>().getPlayerStats().maxLevel++;
                    Player.upgradeUnit(Player.unitTypes[selectedType].GetComponent<UiUnitType>());
                    Debug.Log("Upgraded Unit " + selectedType);
                    Player.AddOrbs(-orbCost);
                    UpdateDisplay();
					upgrades[selectedType].SendMessage("SetColor", Color.white, SendMessageOptions.DontRequireReceiver);

					if ((Player.tutorialState == 2) && (inTutorial) && (selectedType == 0)) {						
						if (cursorInst != null) {
							Destroy(cursorInst);
							cursorInst = null;
						}
						
						Player.completeTutorialState();

						UnitCounterMsgTutorial.SetActive(true);
					} else if ((Player.tutorialState == 6) && (inTutorial) && (selectedType == 1)) {						
						if (cursorInst != null) {
							Destroy(cursorInst);
							cursorInst = null;
						}
						
						Player.completeTutorialState();
						
						cursorInst = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						cursorInst.transform.position = new Vector3(-5.29f, 4.19f, 0f);
						cursorInst.GetComponentInChildren<Animator>().SetTrigger("DoTut2");
						inTutorial = false;
					}
                }
            } else {
				int abilityIndex = selectedType - Player.unitTypes.Count;
				int orbCost = Player.abilities[abilityIndex].GetComponent<Ability>().getUpgradeCost();

				if (Player.abilities[abilityIndex].GetComponent<Ability>().level >= Player.abilities[abilityIndex].GetComponent<Ability>().upgradeCost.Length) {
					return;
				}

                if ((Player.spiritOrbs >= orbCost) && (Player.abilities[abilityIndex].GetComponent<Ability>().level < maxAbilityUpgrades) ) {
					Player.abilities[abilityIndex].GetComponent<Ability>().level++;
					Player.upgradeAbility(Player.abilities[abilityIndex].GetComponent<Ability>());
					Debug.Log("Upgraded Ability " + abilityIndex);
                    Player.AddOrbs(-orbCost);
					UpdateDisplay();
					upgrades[selectedType - 5].GetComponent<SpriteRenderer>().color = Color.white;
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
            textToDisplay += ut.getUpgradeCost();
			textToDisplay += "\n";
			
			textToDisplay += "Health: ";
			if (ut.getPlayerStats().maxLevel > 0) {
				textToDisplay += ut.getPlayerStats().maxHealth + (ut.getPlayerStats().getUnitHealthUpgrade(ut.UnitName) * (ut.getPlayerStats().maxLevel - 1));
				textToDisplay += " (+" + ut.getPlayerStats().getUnitHealthUpgrade(ut.UnitName) + ")";
			} else {
				textToDisplay += ut.getPlayerStats().maxHealth;
			}
			textToDisplay += "\n";
			
			textToDisplay += "Damage: ";
			if (ut.getPlayerStats().maxLevel > 0) {
				textToDisplay += ut.getPlayerStats().attack + (ut.getPlayerStats().getUnitAttackUpgrade(ut.UnitName) * (ut.getPlayerStats().maxLevel - 1));
				textToDisplay += " (+" + ut.getPlayerStats().getUnitAttackUpgrade(ut.UnitName) + ")";
			} else {
				textToDisplay += ut.getPlayerStats().attack;
			}
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
            textToDisplay += ab.getUpgradeCost();
			textToDisplay += "\n";

			switch(ab.abilityType) {
			case Ability._type.rowDamage:
				textToDisplay += "Damage: ";
				textToDisplay += ab.getDamage();
				textToDisplay += "\n";
				break;
			case Ability._type.radiusDamage:
				textToDisplay += "Damage: ";
				textToDisplay += ab.getDamage();
				textToDisplay += "\n";
				textToDisplay += "Radius: ";
				textToDisplay += ab.getAreaOfEffect();
				textToDisplay += "\n";
				break;
			case Ability._type.freezeEnemyUnit:
				textToDisplay += "Duration: ";
				textToDisplay += ab.getDuration();
				textToDisplay += "\n";
				break;
			case Ability._type.ShieldUnit:
				textToDisplay += "Duration: ";
				textToDisplay += ab.getDuration();
				textToDisplay += "\n";
				break;
			}
        }

		unitInfoText.text = textToDisplay;
	}

	void UpdateCounter() {
		if (unitCounterInst == null) {
			unitCounterInst = Instantiate (unitCounterPrefab, new Vector3(-0.5f,-0.25f,0), Quaternion.identity) as GameObject;
		}

		unitCounterInst.GetComponent<UiUnitCounter> ().SetCounterDisplay (Player.unitTypes[selectedType], Player.unitTypes, true);
	}
}
