using UnityEngine;
using System.Collections;

public abstract class GomObject : MonoBehaviour {

    // Anything which applies to all objects will be here. Everything in the game should inherit this.

    // Objects can potentially be not alive, yet still visible (dead units, arrow projectile sticking in the ground, etc)
    public bool alive = true;

    // Objects will need to be classified by side - not just units... projectiles, traps, everything...
    public enum Faction { Good, Bad };
    public Faction faction;
}
