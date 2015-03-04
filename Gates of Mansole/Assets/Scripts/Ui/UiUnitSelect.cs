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
	public GameObject[] unitIcons;

    private GameObject selection;

	// Use this for initialization
	void Start () {
		nameText.GetComponent<TextMesh>().text = "Press unit to\nview details";

		foreach(GameObject ui in unitIcons) {
			ui.SetActive(false);
		}
	}
	
	// Update is called once per frame
    void Update() {
		if (selection != null) {
			GomUnit gom = selection.GetComponent<GomUnit>();
			Ability ab = selection.GetComponent<Ability>();

			if (gom != null) {
				PropertyStats stats;
				UiUnitType uut = world.GetComponent<WorldController>().unitTypes[gom.getType(gom.unitType)].GetComponent<UiUnitType>();
	            if (gom.faction==GomObject.Faction.Player)
					stats = uut.getPlayerStats();
				else
					stats = uut.getEnemyStats();

		        nameText.GetComponent<TextMesh>().text = "Name: " + gom.unitType;
		        upgradeText.GetComponent<TextMesh>().text = "To Upgrade: " + stats.upgradeCost;
		        expText.GetComponent<TextMesh>().text = "Lvl: " + stats.level;
		        attackText.GetComponent<TextMesh>().text = "x" + stats.attack;
		        defenseText.GetComponent<TextMesh>().text = "x" + stats.defense;
		        spiritText.GetComponent<TextMesh>().text = "x" + stats.spirit;
		        speedText.GetComponent<TextMesh>().text = "x" + stats.speed;
			} else if (ab != null) {
				nameText.GetComponent<TextMesh>().text = "Ability: " + ab.abilityName;
				upgradeText.GetComponent<TextMesh>().text = "Lvl: " + ab.level;

				switch(ab.abilityType) {
				case Ability._type.rowDamage:
					expText.GetComponent<TextMesh>().text = "Damage: " + ab.getDamage();
					break;
				case Ability._type.radiusDamage:
					expText.GetComponent<TextMesh>().text = "Damage: " + ab.getDamage();
					expText.GetComponent<TextMesh>().text += "\nRadius: " + ab.getAreaOfEffect();
					break;
				case Ability._type.freezeEnemyUnit:
					expText.GetComponent<TextMesh>().text = "Duration: " + ab.getDuration();
					break;
				case Ability._type.ShieldUnit:
					expText.GetComponent<TextMesh>().text = "Duration: " + ab.getDuration();
					break;
				}
			}
        }
	}

	void MakeSelection(GameObject go){
		if (go.GetComponent<GomUnit>() != null) {
			selection = go;
			
			foreach(GameObject ui in unitIcons) {
				ui.SetActive(true);
			}
		} else if (go.GetComponent<Ability>() != null) {
			selection = go;

			foreach(GameObject ui in unitIcons) {
				ui.SetActive(false);
			}

			attackText.GetComponent<TextMesh>().text = "";
			defenseText.GetComponent<TextMesh>().text = "";
			spiritText.GetComponent<TextMesh>().text = "";
			speedText.GetComponent<TextMesh>().text = "";
		}
	}
}
