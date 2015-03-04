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
    public GameObject shardDisplayPrefab;

	private List<GameObject> units;

	void Start(){
		//Load each unit
		world.unitsUIinst = new List<GameObject>();
		world.squares = new List<GameObject>();
		units = new List<GameObject>();
		for(int i=0;i<unitsPrefab.Length;++i){
            GameObject square = Instantiate(menuPrefab, new Vector3((float)(-6 + (unitMenuInterval * i)), (float)-5.45, (float)0), Quaternion.identity) as GameObject;
            GameObject uiUnit = Instantiate(unitsPrefab[i].GetComponent<UiUnitType>().getRandomUnit(), new Vector3((float)(-6 + (unitMenuInterval * i)), (float)-5.5, (float)0), Quaternion.identity) as GameObject;
            GameObject unitCost = Instantiate(shardDisplayPrefab, new Vector3((float)(-6.3f + (unitMenuInterval * i)), (float)-5f, (float)0), Quaternion.identity) as GameObject;
            unitCost.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            square.name = unitsPrefab[i].GetComponent<UiUnitType>().UnitName;
            square.transform.localScale = new Vector3(square.transform.localScale.x, square.transform.localScale.y * 1.5f, 1f);
			world.squares.Add(square);
            uiUnit.GetComponent<GomUnit>().enabled = false;
            unitCost.transform.SendMessage("SetCustomValue", uiUnit.GetComponent<GomUnit>().cost, SendMessageOptions.DontRequireReceiver);
            world.unitsUIinst.Add(uiUnit);
			world.unitTypes.Add(unitsPrefab[i]);
			units.Add(uiUnit);
			
			if (unitsPrefab[i].GetComponent<UiUnitType>().getPlayerStats().maxLevel <= 0){
				square.SetActive(false);
				uiUnit.SetActive(false);
                unitCost.SetActive(false);
			}
		}

        // Load each ability
        world.abilityUIinst = new List<GameObject>();
        world.abilities = new List<GameObject>();
        for (int i = 0; i < abilityPrefab.Length; ++i) {
            GameObject square = Instantiate(menuPrefab, new Vector3((float)(-6 + (unitMenuInterval * (i + unitsPrefab.Length))), (float)-5.45, (float)0), Quaternion.identity) as GameObject;
            GameObject uiAbility = Instantiate(abilityPrefab[i].GetComponent<Ability>().sprite, new Vector3((float)(-6f + (unitMenuInterval * (i + unitsPrefab.Length))), (float)-5.5, (float)0), Quaternion.identity) as GameObject;
            GameObject abilityCost = Instantiate(shardDisplayPrefab, new Vector3((float)(-6.3f + (unitMenuInterval * (i + unitsPrefab.Length))), (float)-5f, (float)0), Quaternion.identity) as GameObject;
            abilityCost.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
			abilityCost.transform.SendMessage("SetCustomValue", abilityPrefab[i].GetComponent<Ability>().getUseCost(), SendMessageOptions.DontRequireReceiver);
            square.name = abilityPrefab[i].GetComponent<Ability>().abilityName;
            square.transform.localScale = new Vector3(square.transform.localScale.x, square.transform.localScale.y * 1.5f, 1f);
            world.squares.Add(square);
            world.abilityUIinst.Add(uiAbility);
            world.abilities.Add(abilityPrefab[i]);

            if (abilityPrefab[i].GetComponent<Ability>().level <= 0) {
                square.SetActive(false);
                uiAbility.SetActive(false);
                abilityCost.SetActive(false);
            }
        }

        world.unitMenuInterval = unitMenuInterval;
	}
}