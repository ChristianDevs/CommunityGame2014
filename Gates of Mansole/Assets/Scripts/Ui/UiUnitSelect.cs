using UnityEngine;
using System.Collections;

public class UiUnitSelect : MonoBehaviour {
	
	public GameObject nameText;
    public GameObject upgradeText;
	public GameObject expText;
	public GameObject attackText;
	public GameObject defenseText;
	public GameObject spiritText;
	public GameObject speedText;
	public GameObject world;

    private GameObject unit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update() {
        if (unit != null) {
            GomUnit gom = unit.GetComponent<GomUnit>();
			PropertyStats stats;
			UiUnitType uut = world.GetComponent<WorldController>().unitTypes[gom.getType (gom.unitType)].GetComponent<UiUnitType>();
            if (gom.faction==GomObject.Faction.Player)
				stats = uut.getPlayerStats();
			else
				stats = uut.getEnemyStats();


			if (gom != null) {
                nameText.GetComponent<TextMesh>().text = "Name: " + gom.unitType;
                upgradeText.GetComponent<TextMesh>().text = "To Upgrade: " + stats.upgradeCost;
                expText.GetComponent<TextMesh>().text = "Lvl: " + stats.level;
                attackText.GetComponent<TextMesh>().text = "x" + stats.attack;
                defenseText.GetComponent<TextMesh>().text = "x" + stats.defense;
                spiritText.GetComponent<TextMesh>().text = "x" + stats.spirit;
                speedText.GetComponent<TextMesh>().text = "x" + stats.speed;
            }
        }
	}

	void SelectUnit(GameObject go){
        unit = go;

	}
}
