using UnityEngine;
using System.Collections;

public class UiUnitType : MonoBehaviour {

	public string UnitName;
	public GameObject[] Units;

	public int level;
	public int maxLevel;

	public GameObject getRandomUnit() {
		if (Units.Length < 1) {
				return null;
		}

		return Units[Random.Range(0, Units.Length - 1)];
	}

	public void resetUnitStats() {
		foreach (GameObject unit in Units) {
			unit.GetComponent<GomUnit>().getStats().resetUnitStats();
		}
	}
}
