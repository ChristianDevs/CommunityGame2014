using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSpawnMenuController : MonoBehaviour {
	public string[] unitsName;
	public GameObject[] unitsPrefab;
	public GameObject menuPrefab;
	public GameObject buttonPrefab;
	public GameObject textPrefab;
	public World world;

	private List<GameObject> units;

	void Start(){
		//Load each unit
		world.unitsUIinst = new List<GameObject>();
		world.squares = new List<GameObject>();
		units = new List<GameObject>();
		for(int i=0;i<unitsPrefab.Length;++i){
			GameObject square = Instantiate(menuPrefab,new Vector3((float)(-6+(1.5*i)),(float)-5.2,(float)0),Quaternion.identity) as GameObject;
			GameObject uiUnit = Instantiate(unitsPrefab[i].GetComponent<UiUnitType>().getRandomUnit(),new Vector3((float)(-6+(1.5*i)),(float)-5.2,(float)0),Quaternion.identity) as GameObject;
			GameObject button = Instantiate(buttonPrefab,new Vector3((float)(-6+(1.5*i)),(float)-6,(float)0),Quaternion.identity) as GameObject;
			GameObject text = Instantiate(textPrefab,new Vector3((float)(-6+(1.5*i)),(float)-6,(float)0),Quaternion.identity) as GameObject;
			square.name = unitsPrefab[i].GetComponent<UiUnitType>().UnitName;
			world.squares.Add(square);
			GomUnit gom2 = uiUnit.GetComponent<GomUnit>();
			gom2.enabled = false;
			world.unitsUIinst.Add(uiUnit);
			world.unitTypes.Add(unitsPrefab[i]);
			units.Add(uiUnit);
			Button buttonCom = button.GetComponent<Button>();
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
	}
	void buttonPush(string buttonName){
		int unitType = int.Parse(buttonName);
		PropertyStats unitStats = units[unitType].GetComponent<GomUnit>().playerStats;
		if ((Player.spiritShards >= unitStats.upgradeCost) &&
		    (unitStats.level <= unitsPrefab[unitType].GetComponent<UiUnitType>().level)) {

			unitStats.upgradeUnit(units[unitType].GetComponent<GomUnit>().entityName);
		}
	}
}