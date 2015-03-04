using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour {
    public enum _type {
        rowDamage,
        radiusDamage,
        freezeEnemyUnit,
        ShieldUnit
    }

    public string abilityName;
    public _type abilityType;
    public GameObject sprite;
    public int damage;
    public int duration;
    public int areaOfEffect;
    public int upgradeCost;
	public int useCost;
    public int level;

	public int incDamage;
	public int incDuration;
	public int incAreaOfEffect;
	public int incUpgradeCost;
	public int incUseCost;

	public int getDamage() {
		return damage + (incDamage * level);
	}

	public int getDuration() {
		return duration + (incDuration * level);
	}

	public int getAreaOfEffect() {
		return areaOfEffect + (incAreaOfEffect * level);
	}

	public int getUpgradeCost() {
		return upgradeCost + (incUpgradeCost * level);
	}

	public int getUseCost() {
		return useCost + (incUseCost * level);
	}
}
