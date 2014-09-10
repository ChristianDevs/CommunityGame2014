using UnityEngine;
using System.Collections;

[System.Serializable]
public class PropertyExp {

    public int level = 1;
    public int exp = 0;  // faith?
    public float expMultiplier = 1;
    public int nextLevel = 100;
    public float nextLevelMultiplier = 2.1f;

    public delegate void OnNextLevel();
    public event OnNextLevel eventNextLevel;

    public void AddExp(int amt) {
        exp = exp + (int)(amt * expMultiplier);
        while (exp >= nextLevel) {
            level++;
            nextLevel = (int)(nextLevel * nextLevelMultiplier);  // Arbitrary - make something up
            eventNextLevel();
        }
    }
}
