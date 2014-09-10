using UnityEngine;
using System.Collections;

public class GomUnit : GomObject {

    public string entityName;
    public PropertyStats stats;
    public PropertyExp exp;
    public int health;

    private Weapon weapon;


    public void DamageMelee(PropertyStats stats) {
        // Whatever - arbitrary damage calculation
        int minDamage = Random.Range(0, 2);  // always a chance of doing something
        int variation = Random.Range(0, stats.attack / 5 + 1);
        int baseDamage = stats.attack - this.stats.defense;
        Damage(minDamage + baseDamage + variation);
    }

    public void DamageSpirit(PropertyStats stats) {
        // Whatever - arbitrary damage calculation
        int minDamage = Random.Range(0, 1);  // always a chance of doing something
        int variation = 0;
        int baseDamage = stats.spirit - this.stats.armor;
        Damage(minDamage + baseDamage + variation);
    }

    public void Heal(int amt) {
        health += amt + this.stats.armor;  // Side effect of spiritual armor (I'm just making stuff up here...)
        if (health > this.stats.maxHealth)
            health = this.stats.maxHealth;
    }

    void Damage(int amt) {
        health -= amt;
        if (health <= 0) {
            health = 0;
            alive = false;
        }
    }

	// Use this for initialization
	void Start () {
        weapon = gameObject.GetComponent<Weapon>();
	}
	
}
