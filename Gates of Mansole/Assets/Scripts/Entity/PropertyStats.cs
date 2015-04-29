using UnityEngine;
using System.Collections;

[System.Serializable]
public class PropertyStats {
    public int maxHealth = 50;
    public int speed = 10;  // movement + time between attacks?

    public int attack = 10;  // melee offense
    public int defense = 0;  // melee defense
    
    public int armor = 0;  // spiritual defense
    public int spirit = 7;  // spiritual offense, not to be confused with Spirit Shards (currency)

	public int level = 1; // increases upon upgrade
	public int maxLevel = 0;
	public int upgradeCost = 10; // in Spirit Shards

	public void upgradeUnit(string unit) {
		level++;
		attack += 5;
		maxHealth += 10;
	}

	public void resetUnitStats(UiUnitType ut) {
		// NOTE: if property stats are changed in the prefabs, these must be changed as well
		level = 1;
		switch (ut.UnitName) {
				case "Shepherd": // bow
						maxHealth = 60;
						attack = 4;
						break;
				case "Evangelist": // spear
						maxHealth = 50;
						attack = 10;
						break;
				case "Elder": // staff
						maxHealth = 60;
						attack = 4;
						break;
				case "Teacher": // sword
						maxHealth = 50;
						attack = 10;
						break;
				case "Orator": // wand
						maxHealth = 60;
						attack = 4;
						break;
				}
	}

	public void hardResetStats() {
		level = 1;
		maxLevel = 0;
	}

	public void updateUnitStats(GameObject unit) {
		PropertyStats unitStats = unit.GetComponent<GomUnit> ().getStats();
		speed = unitStats.speed;
		attack = unitStats.attack;
		defense = unitStats.defense;
		armor = unitStats.armor;
		spirit = unitStats.spirit;
		level = unitStats.level;
		//Debug.Log ("updated unit stats");
	}

}
