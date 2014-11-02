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
	public int maxLevel = 3;
	public int upgradeCost = 10; // in Spirit Shards

	public void upgradeUnit(string unit) {
		Player.spiritShards -= upgradeCost;
		level++;
		attack += 5;
		maxHealth += 10;

	}
	
	public void resetUnitStats() {
		level = 1;
		attack = 10;
		maxHealth = 50;
	}

	public void updateUnitStats(GameObject unit) {
		PropertyStats unitStats = unit.GetComponent<GomUnit> ().stats;
		unitStats.speed = speed;
		unitStats.attack = attack;
		unitStats.defense = defense;
		unitStats.armor = armor;
		unitStats.spirit = spirit;
		unitStats.level = level;
		//Debug.Log ("updated unit stats");
	}

}
