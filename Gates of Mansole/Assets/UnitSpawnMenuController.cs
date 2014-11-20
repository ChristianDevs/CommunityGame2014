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

	private GameObject[] units;

	void Start(){
		//Load each unit
		world.unitsUIinst = new List<GameObject>();
		world.squares = new GameObject[unitsPrefab.Length];
		units = new GameObject[unitsPrefab.Length];
		for(int i=0;i<unitsPrefab.Length;++i){
			GameObject square = Instantiate(menuPrefab,new Vector3((float)(-6+(1.5*i)),(float)-5.2,(float)0),Quaternion.identity) as GameObject;
			GameObject uiUnit = Instantiate(unitsPrefab[i],new Vector3((float)(-6+(1.5*i)),(float)-5.2,(float)0),Quaternion.identity) as GameObject;
			GameObject button = Instantiate(buttonPrefab,new Vector3((float)(-6+(1.5*i)),(float)-6,(float)0),Quaternion.identity) as GameObject;
			GameObject text = Instantiate(textPrefab,new Vector3((float)(-6+(1.5*i)),(float)-6,(float)0),Quaternion.identity) as GameObject;
			square.name = unitsName[i];
			world.squares[i] = square;
			GomUnit gom2 = uiUnit.GetComponent<GomUnit>();
			gom2.enabled = false;
			world.unitsUIinst.Add(uiUnit);
			units[i]=uiUnit;
			Button buttonCom = button.GetComponent<Button>();
			button.transform.localScale = new Vector3((float)0.5,(float)0.5,(float)1);
			buttonCom.buttonName=""+i;
			button.name=""+i;
			buttonCom.controller = this.gameObject;
			TextMesh textCom = text.GetComponent<TextMesh>();
			text.transform.localScale = new Vector3((float)0.5,(float)0.5,(float)1);
			textCom.fontSize = 32;
			textCom.text = "Upgrade";
		}
		
		world.unitTypes = units;
	}
	void buttonPush(string buttonName){
		int unitType = int.Parse(buttonName);
		PropertyStats unitStats = units[unitType].GetComponent<GomUnit>().playerStats;
		if (Player.spiritShards >= unitStats.upgradeCost) {
			if ((unitStats.level < unitStats.maxLevel)) {
				unitStats.upgradeUnit(units[unitType].GetComponent<GomUnit>().entityName); 
			}
		}
	}
	void Update(){

	}
}