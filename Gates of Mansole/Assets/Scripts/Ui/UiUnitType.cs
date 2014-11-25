using UnityEngine;
using System.Collections;

public class UiUnitType : MonoBehaviour {

	public string UnitName;
	public GameObject[] Units;
	public PropertyStats Stats;

	public PropertyStats getStats() {
		return Stats;
	}

	public GameObject getRandomUnit() {
		if (Units.Length < 1) {
				return null;
		}

		return Units[Random.Range(0, Units.Length - 1)];
	}
}
