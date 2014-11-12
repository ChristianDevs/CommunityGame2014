using UnityEngine;
using System.Collections;

public class UiUnitSelect : MonoBehaviour {
	
	public GameObject nameText;
	public GameObject factionText;
	public GameObject expText;
	public GameObject attackText;
	public GameObject defenseText;
	public GameObject spiritText;
	public GameObject speedText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SelectUnit(GameObject go){
		GomUnit gom = go.GetComponent<GomUnit>();
		nameText.GetComponent<TextMesh> ().text = "Name: ";//+gom.name;
		factionText.GetComponent<TextMesh>().text = "Faction: "+gom.faction.ToString();
		expText.GetComponent<TextMesh>().text = "Level: "+gom.exp.level;
		attackText.GetComponent<TextMesh>().text = "x"+gom.getStats().attack;
		defenseText.GetComponent<TextMesh>().text = "x"+gom.getStats().defense;
		spiritText.GetComponent<TextMesh>().text = "x"+gom.getStats().spirit;
		speedText.GetComponent<TextMesh>().text = "x"+gom.getStats().speed;

	}
}
