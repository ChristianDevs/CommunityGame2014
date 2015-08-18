using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiUnitCounter : MonoBehaviour {

	public Transform SelUnitLoc;
	public Transform Beat1Loc;
	public Transform Beat2Loc;

	public TextMesh SelText;
	public TextMesh Beat1Text;
	public TextMesh Beat2Text;

	private GameObject SelUnitInst;
	private GameObject Beat1Inst;
	private GameObject Beat2Inst;

	// Use this for initialization
	void Awake () {
		SelUnitInst = null;
		Beat1Inst = null;
		Beat2Inst = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetCounterDisplay(GameObject SelUnit, List<GameObject> unitTypes, bool showSelUnit) {
		if (SelUnitInst != null) {
			Destroy(SelUnitInst);
		}
		if (Beat1Inst != null) {
			Destroy(Beat1Inst);
		}
		if (Beat2Inst != null) {
			Destroy(Beat2Inst);
		}

		if (showSelUnit == true) {
			SelUnitInst = Instantiate(SelUnit.GetComponent<UiUnitType> ().getRandomUnit (), SelUnitLoc.position, Quaternion.identity) as GameObject;
			SelUnitInst.GetComponent<GomUnit>().enabled = false;
		}
		SelText.text = SelUnit.GetComponent<UiUnitType>().UnitName;

		switch(SelUnit.GetComponent<UiUnitType>().UnitName) {
		case "OrcBow":
		case "Shepherd":
			// Shepherd counters Evangelist and Orator
			Beat1Inst = Instantiate(getUnitByType("Evangelist", unitTypes).getRandomUnit(), Beat1Loc.position, Quaternion.identity) as GameObject;
			Beat2Inst = Instantiate(getUnitByType("Orator", unitTypes).getRandomUnit(), Beat2Loc.position, Quaternion.identity) as GameObject;
			break;
		case "OrcWand":
		case "Orator":
			// Orator counters Evangelist and Elder
			Beat1Inst = Instantiate(getUnitByType("Evangelist", unitTypes).getRandomUnit(), Beat1Loc.position, Quaternion.identity) as GameObject;
			Beat2Inst = Instantiate(getUnitByType("Elder", unitTypes).getRandomUnit(), Beat2Loc.position, Quaternion.identity) as GameObject;
			break;
		case "OrcSword":
		case "Teacher":
			// Teacher counters Shepherd and Orator
			Beat1Inst = Instantiate(getUnitByType("Shepherd", unitTypes).getRandomUnit(), Beat1Loc.position, Quaternion.identity) as GameObject;
			Beat2Inst = Instantiate(getUnitByType("Orator", unitTypes).getRandomUnit(), Beat2Loc.position, Quaternion.identity) as GameObject;
			break;
		case "OrcSpear":
		case "Evangelist":
			// Evangelist counters Teacher and Elder
			Beat1Inst = Instantiate(getUnitByType("Teacher", unitTypes).getRandomUnit(), Beat1Loc.position, Quaternion.identity) as GameObject;
			Beat2Inst = Instantiate(getUnitByType("Elder", unitTypes).getRandomUnit(), Beat2Loc.position, Quaternion.identity) as GameObject;
			break;
		case "OrcStaff":
		case "Elder":
			// Elder counters Shepherd and Teacher
			Beat1Inst = Instantiate(getUnitByType("Shepherd", unitTypes).getRandomUnit(), Beat1Loc.position, Quaternion.identity) as GameObject;
			Beat2Inst = Instantiate(getUnitByType("Teacher", unitTypes).getRandomUnit(), Beat2Loc.position, Quaternion.identity) as GameObject;
			break;
		}

		Beat1Text.text = Beat1Inst.GetComponent<GomUnit>().unitType;
		Beat2Text.text = Beat2Inst.GetComponent<GomUnit>().unitType;
		Beat1Inst.GetComponent<GomUnit>().enabled = false;
		Beat2Inst.GetComponent<GomUnit>().enabled = false;

		foreach (SpriteRenderer sr in Beat1Inst.GetComponentsInChildren<SpriteRenderer>()) {
			sr.sortingLayerName = "UI";
			sr.sortingOrder = 5;
		}
		foreach (SpriteRenderer sr in Beat2Inst.GetComponentsInChildren<SpriteRenderer>()) {
			sr.sortingLayerName = "UI";
			sr.sortingOrder = 5;
		}

		if (SelUnitInst != null) {
			SelUnitInst.transform.parent = transform;
		}
		Beat1Inst.transform.parent = transform;
		Beat2Inst.transform.parent = transform;
		SelText.transform.parent = transform;
		Beat1Text.transform.parent = transform;
		Beat2Text.transform.parent = transform;
	}

	UiUnitType getUnitByType(string unitType, List<GameObject> unitTypes) {
		UiUnitType ret = null;

		foreach (GameObject ut in unitTypes) {
			if (ut.GetComponent<UiUnitType>().UnitName == unitType) {
				ret = ut.GetComponent<UiUnitType>();
			}
		}

		return ret;
	}
}