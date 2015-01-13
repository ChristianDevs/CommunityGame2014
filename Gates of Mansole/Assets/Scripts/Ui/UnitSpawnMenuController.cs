using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSpawnMenuController : MonoBehaviour {
	public GameObject[] unitsPrefab;
	public GameObject menuPrefab;
	public GameObject buttonPrefab;
	public GameObject textPrefab;
    public GameObject[] abilityPrefab;
	public WorldController world;
    public float unitMenuInterval;

	private List<GameObject> units;

	void Start(){
		//Load each unit
		world.unitsUIinst = new List<GameObject>();
		world.squares = new List<GameObject>();
		units = new List<GameObject>();
		for(int i=0;i<unitsPrefab.Length;++i){
            GameObject square = Instantiate(menuPrefab, new Vector3((float)(-6 + (unitMenuInterval * i)), (float)-5.2, (float)0), Quaternion.identity) as GameObject;
            GameObject uiUnit = Instantiate(unitsPrefab[i].GetComponent<UiUnitType>().getRandomUnit(), new Vector3((float)(-6 + (unitMenuInterval * i)), (float)-5.2, (float)0), Quaternion.identity) as GameObject;
            GameObject button = Instantiate(buttonPrefab, new Vector3((float)(-6 + (unitMenuInterval * i)), (float)-5.85, (float)0), Quaternion.identity) as GameObject;
            GameObject text = Instantiate(textPrefab, new Vector3((float)(-6 + (unitMenuInterval * i)), (float)-5.85, (float)0), Quaternion.identity) as GameObject;
			square.name = unitsPrefab[i].GetComponent<UiUnitType>().UnitName;
			world.squares.Add(square);
			GomUnit gom2 = uiUnit.GetComponent<GomUnit>();
			gom2.enabled = false;
			world.unitsUIinst.Add(uiUnit);
			world.unitTypes.Add(unitsPrefab[i]);
			units.Add(uiUnit);
			UiButton buttonCom = button.GetComponent<UiButton>();
			button.transform.localScale = new Vector3((float)0.5,(float)0.75,(float)1);
			buttonCom.buttonName=""+i;
			button.name=""+i;
			buttonCom.controller = this.gameObject;
			TextMesh textCom = text.GetComponent<TextMesh>();
			text.transform.localScale = new Vector3((float)0.5,(float)0.5,(float)1);
			textCom.fontSize = 48;
			textCom.text = "Upgrade";
			
			if (unitsPrefab[i].GetComponent<UiUnitType>().level <= 0){
				square.SetActive(false);
				uiUnit.SetActive(false);
				button.SetActive(false);
				text.SetActive(false);
			}
		}

        // Load each ability
        world.abilityUIinst = new List<GameObject>();
        world.abilities = new List<GameObject>();
        for (int i = 0; i < abilityPrefab.Length; ++i) {
            GameObject square = Instantiate(menuPrefab, new Vector3((float)(-6 + (unitMenuInterval * (i + unitsPrefab.Length))), (float)-5.2, (float)0), Quaternion.identity) as GameObject;
            GameObject uiAbility = Instantiate(abilityPrefab[i].GetComponent<Ability>().sprite, new Vector3((float)(-6 + (unitMenuInterval * (i + unitsPrefab.Length))), (float)-5.2, (float)0), Quaternion.identity) as GameObject;
            square.name = abilityPrefab[i].GetComponent<Ability>().abilityName;
            world.squares.Add(square);
            world.abilityUIinst.Add(uiAbility);
            world.abilities.Add(abilityPrefab[i]);

            if (abilityPrefab[i].GetComponent<Ability>().level <= 0) {
                square.SetActive(false);
                uiAbility.SetActive(false);
            }
        }

        world.unitMenuInterval = unitMenuInterval;
	}

	void buttonPush(string buttonName){
		int unitType = int.Parse(buttonName);
		PropertyStats unitStats = units[unitType].GetComponent<GomUnit>().playerStats;

		if ((Player.spiritShards >= unitStats.upgradeCost) &&
		    (unitStats.level < unitsPrefab[unitType].GetComponent<UiUnitType>().maxLevel)) {
			for (int i = 0; i < unitsPrefab[unitType].GetComponent<UiUnitType>().Units.Length; i++) {
				PropertyStats stats = unitsPrefab[unitType].GetComponent<UiUnitType>().Units[i].GetComponent<GomUnit>().playerStats;
				stats.upgradeUnit(units[unitType].GetComponent<GomUnit>().entityName);
			
				Debug.Log ("upgraded "+unitsPrefab[unitType].GetComponent<UiUnitType>().Units[i].name);
			}
			unitStats.purchaseUpgrade(unitStats.upgradeCost);
			Debug.Log ("shards left "+Player.spiritShards);
		}
	}
}