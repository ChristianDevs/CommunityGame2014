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

    private GameObject unit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update() {
        if (unit != null) {
            GomUnit gom = unit.GetComponent<GomUnit>();

            if (gom != null) {
                nameText.GetComponent<TextMesh>().text = "Name: " + gom.entityName;
                upgradeText.GetComponent<TextMesh>().text = "To Upgrade: " + gom.getStats().upgradeCost;
                expText.GetComponent<TextMesh>().text = "Lvl: " + gom.getStats().level;
                attackText.GetComponent<TextMesh>().text = "x" + gom.getStats().attack;
                defenseText.GetComponent<TextMesh>().text = "x" + gom.getStats().defense;
                spiritText.GetComponent<TextMesh>().text = "x" + gom.getStats().spirit;
                speedText.GetComponent<TextMesh>().text = "x" + gom.getStats().speed;
            }
        }
	}

	void SelectUnit(GameObject go){
        unit = go;

	}
}
