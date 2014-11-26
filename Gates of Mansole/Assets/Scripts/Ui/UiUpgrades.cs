using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiUpgrades : MonoBehaviour {

	public TextMesh unitInfoText;
	public GameObject unitWindow;
	public float startUnitX;
	public float startUnitY;
	public float incUnitX;
	public float incUnitY;

	private List<GameObject> upgrades;
	private List<GameObject> upgradeWindows;
	private int selectedUnitType;

	// Use this for initialization
	void Start () {
		float x = startUnitX;
		float y = startUnitY;

		upgrades = new List<GameObject>();
		upgradeWindows = new List<GameObject>();
		for (int i = 0; i < Player.unitTypes.Count; i++) {
			upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
			upgradeWindows[upgradeWindows.Count-1].transform.name = Player.unitTypes[i].GetComponent<UiUnitType>().UnitName + "0";
			upgrades.Add(Instantiate(Player.unitTypes[i].GetComponent<UiUnitType>().getRandomUnit(), new Vector3(x, y), Quaternion.identity) as GameObject);
			upgrades[upgrades.Count-1].GetComponent<GomUnit>().enabled = false;

			for (int j = 0; j < Player.unitTypes[i].GetComponent<UiUnitType>().level; j++){
				y += incUnitY;

				upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
				upgradeWindows[upgradeWindows.Count-1].transform.name = Player.unitTypes[i].GetComponent<UiUnitType>().UnitName + (j + 1);
				upgrades.Add(Instantiate(Player.unitTypes[i].GetComponent<UiUnitType>().getRandomUnit(), new Vector3(x, y), Quaternion.identity) as GameObject);
				upgrades[upgrades.Count-1].GetComponent<GomUnit>().enabled = false;
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
						string unitTypeName;

						char[] charsTrim = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

						unitTypeName = hitSquare.transform.name.TrimEnd(charsTrim);

						for (int j = 0; j < Player.unitTypes.Count; j++) {
							if (unitTypeName == Player.unitTypes[j].GetComponent<UiUnitType>().UnitName) {
								selectedUnitType = j;
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
			if (Player.spiritOrbs > ((Player.unitTypes[selectedUnitType].GetComponent<UiUnitType>().level * 2) + 2)) {
				float x = startUnitX + (selectedUnitType * incUnitX);
				float y = startUnitY + ((Player.unitTypes[selectedUnitType].GetComponent<UiUnitType>().level + 1) * incUnitY);

				Player.unitTypes[selectedUnitType].GetComponent<UiUnitType>().level++;
				Player.upgradeUnit(Player.unitTypes[selectedUnitType].GetComponent<UiUnitType>());
				Debug.Log("Upgraded Unit " + selectedUnitType);

				upgradeWindows.Add(Instantiate(unitWindow, new Vector3(x, y), Quaternion.identity) as GameObject);
				upgradeWindows[upgradeWindows.Count-1].transform.name =
					Player.unitTypes[selectedUnitType].GetComponent<UiUnitType>().UnitName +
					Player.unitTypes[selectedUnitType].GetComponent<UiUnitType>().level;
				upgrades.Add(Instantiate(Player.unitTypes[selectedUnitType].GetComponent<UiUnitType>().getRandomUnit(), new Vector3(x, y), Quaternion.identity) as GameObject);
				upgrades[upgrades.Count-1].GetComponent<GomUnit>().enabled = false;
			}
			break;
		default:
			break;
		}
	}

	void UpdateDisplay() {
		string textToDisplay;
		UiUnitType ut;

		ut = Player.unitTypes [selectedUnitType].GetComponent<UiUnitType>();

		textToDisplay = "Name: ";
		textToDisplay += ut.UnitName;
		textToDisplay += "\n";
		
		textToDisplay += "Level: ";
		textToDisplay += ut.level;
		textToDisplay += "\n";

		textToDisplay += "Upgrade Cost: ";
		textToDisplay += ut.level * 2 + 2;
		textToDisplay += "\n";

		unitInfoText.text = textToDisplay;
	}
}
