using UnityEngine;
using System.Collections;

public class Upgrade : MonoBehaviour {
	public GameObject icon;
	public string[] upgradeRevealRequirements;
	public string upgradeName;
	public int cost;
	public string[] upgradeRequirements;
	public string description;
	
	void Start () {
		bool reveal = true;
		for(int i=0;i<upgradeRevealRequirements.Length;++i){
			if(PlayerPrefs.GetInt(upgradeRevealRequirements[i],0)==0){
				reveal=false;
				break;
			}
		}
		icon.SetActive(reveal);

	}

	bool RequirementsFilled(){
		bool buy = true;
		for(int i=0;i<upgradeRequirements.Length;++i){
			if(PlayerPrefs.GetInt(upgradeRequirements[i],0)==0){
				buy=false;
				break;
			}
		}
		if(PlayerPrefs.GetInt("spiritOrbs",0)<cost){
			buy=false;
		}
		return buy;
	}

	void GenerateHoverText(){
		GameObject hoverText = GameObject.Find("Text");
		TextMesh text = hoverText.GetComponent<TextMesh> ();
		text.text = upgradeName+"\nCost "+cost+"\n";
		foreach(string required in upgradeRequirements){
			if(PlayerPrefs.GetInt(required,0)==0){
				text.text+="Requires "+required+"\n";
			}
		}
		text.text += description;

	}

	void Update () {
		RaycastHit hit;
		if(icon.activeInHierarchy){
			if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
				if(hit.transform.name==this.transform.name){
					if(Input.GetMouseButton(0)&&PlayerPrefs.GetInt(upgradeName,0)==0){
						if(RequirementsFilled()){
							int orbsLeft = PlayerPrefs.GetInt("spiritOrbs")-cost;
							PlayerPrefs.SetInt("spiritOrbs",orbsLeft);
							PlayerPrefs.SetInt(upgradeName,1);
							Application.LoadLevel(Application.loadedLevelName);
						}
					}
					else{
						GenerateHoverText();
					}
				}
			}
		}
	}
}
