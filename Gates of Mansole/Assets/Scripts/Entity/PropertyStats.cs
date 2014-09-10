using UnityEngine;
using System.Collections;

[System.Serializable]
public class PropertyStats {
    public int maxHealth = 50;
    public int speed = 10;  // movement + time between attacks?

    public int attack = 10;  // melee offense
    public int defense = 0;  // melee defense
    
    public int armor = 0;  // spiritual defense
    public int spirit = 7;  // spiritual offense, not to be confused with Spirit which might be our "currency" ?
}
