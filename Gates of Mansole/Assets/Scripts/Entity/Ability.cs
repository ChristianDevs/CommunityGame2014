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
    public int damageRange;
    public int duration;
    public int areaOfEffect;
    public int cost;
    public int level;
}
