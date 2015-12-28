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

	public int incBowAtt = 5;
	public int incBowHealth = 6;
	public int incSpearAtt = 10;
	public int incSpearHealth = 5;
	public int incStaffAtt = 4;
	public int incStaffHealth = 5;
	public int incSwordAtt = 5;
	public int incSwordHealth = 10;
	public int incWandAtt = 6;
	public int incWandHealth = 7;

	public int getUnitHealthUpgrade(string unit) {
		switch (unit) {
		case "OrcArcher":
		case "Shepherd": // bow
			return incBowHealth;
			break;
		case "OrcSpear":
		case "Evangelist": // spear
			return incSpearHealth;
			break;
		case "OrcStaff":
		case "Elder": // staff
			return incStaffHealth;
			break;
		case "OrcSword":
		case "Teacher": // sword
			return incSwordHealth;
			break;
		case "OrcWand":
		case "Orator": // wand
			return incWandHealth;
			break;
		}

		return 0;
	}
	
	public int getUnitAttackUpgrade(string unit) {
		switch (unit) {
		case "OrcArcher":
		case "Shepherd": // bow
			return incBowAtt;
			break;
		case "OrcSpear":
		case "Evangelist": // spear
			return incSpearAtt;
			break;
		case "OrcStaff":
		case "Elder": // staff
			return incStaffAtt;
			break;
		case "OrcSword":
		case "Teacher": // sword
			return incSwordAtt;
			break;
		case "OrcWand":
		case "Orator": // wand
			return incWandAtt;
			break;
		}
		
		return 0;
	}


	public void upgradeUnit(string unit) {
		level++;

		switch (unit) {
		case "OrcArcher":
		case "Shepherd": // bow
			attack += incBowAtt;
			maxHealth += incBowHealth;
			break;
		case "OrcSpear":
		case "Evangelist": // spear
			attack += incSpearAtt;
			maxHealth += incSpearHealth;
			break;
		case "OrcStaff":
		case "Elder": // staff
			attack += incStaffAtt;
			maxHealth += incStaffHealth;
			break;
		case "OrcSword":
		case "Teacher": // sword
			attack += incSwordAtt;
			maxHealth += incSwordHealth;
			break;
		case "OrcWand":
		case "Orator": // wand
			attack += incWandAtt;
			maxHealth += incWandHealth;
			break;
		}
	}

	public void resetUnitStats(UiUnitType ut) {
		// NOTE: if property stats are changed in the prefabs, these must be changed as well
		level = 1;
		switch (ut.UnitName) {
				case "OrcArcher":
				case "Shepherd": // bow
						maxHealth = 50;
						attack = 5;
						break;
				case "OrcSpear":
				case "Evangelist": // spear
						maxHealth = 60;
						attack = 8;
						break;
				case "OrcStaff":
				case "Elder": // staff
						maxHealth = 40;
						attack = 4;
						break;
				case "OrcSword":
				case "Teacher": // sword
						maxHealth = 70;
						attack = 7;
						break;
				case "OrcWand":
				case "Orator": // wand
						maxHealth = 60;
						attack = 6;
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
