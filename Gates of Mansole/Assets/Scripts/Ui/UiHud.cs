using UnityEngine;
using System.Collections;

public class UiHud : MonoBehaviour {

	public GameObject bowUnit;
	public GameObject swordUnit;
	public GameObject upgradeSword;
	public GameObject upgradeBow;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void buttonPush (string buttonName) {
		GameObject unit;
		GameObject button;

		switch (buttonName) {
			case "UpgradeSword":
				unit = swordUnit;
				button = upgradeSword;
				break;
			case "UpgradeBow":
				unit = bowUnit;
				button = upgradeBow;
				break;
			default:
				unit = null;
				button = null;
				break;
		}

		if (unit) {
			PropertyStats unitStats = unit.GetComponent<GomUnit>().stats;
			if (Player.spiritShards >= unitStats.upgradeCost) {
				if ((unitStats.level < unitStats.maxLevel)) {
					unitStats.upgradeUnit(unit.GetComponent<GomUnit>().entityName); 
				}
				else if (button)
					button.SetActive(false);
			}
		}
	}
}
