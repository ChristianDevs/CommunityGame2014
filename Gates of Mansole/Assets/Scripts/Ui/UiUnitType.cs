using UnityEngine;
using System.Collections;

public class UiUnitType : MonoBehaviour {

	public string UnitName;
	public GameObject[] Units;

	public PropertyStats playerStats;
	public PropertyStats enemyStats;

	public PropertyStats getPlayerStats() {
		return playerStats;
	}

	public PropertyStats getEnemyStats() {
		return enemyStats;
	}

	public GameObject getRandomUnit() {
		if (Units.Length < 1) {
				return null;
		}

		return Units[Random.Range(0, Units.Length - 1)];
	}
}
