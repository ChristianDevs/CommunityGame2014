using UnityEngine;
using System.Collections;

public class UiAttacker : MonoBehaviour {

	public GameObject world;
	public GameObject BarEmptyLeft;
	public GameObject BarEmptyMid;
	public GameObject BarEmptyRight;
	public GameObject BarGreen;
	
	private GameObject HpLeftBar;
	private GameObject HpMidBar;
	private GameObject HpRightBar;
	private GameObject HpBarFill;
	private Vector3 HpBarPos;
	private float HpBarMidScale;
	private float moveXScale;

	void Start() {
		HpBarPos = new Vector3(transform.position.x, transform.position.y);
		HpBarMidScale = 19;
		
		// Instantiate the health bars
		HpLeftBar = Instantiate(BarEmptyLeft, HpBarPos + new Vector3(-1.25f, 0, 0), Quaternion.identity) as GameObject;
		HpLeftBar.transform.localScale = new Vector3 (1, 1.5f, 1);
		HpMidBar = Instantiate(BarEmptyMid, HpBarPos + new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		HpMidBar.transform.localScale = new Vector3 (4.5f, 1.5f, 1);
		HpRightBar = Instantiate(BarEmptyRight, HpBarPos + new Vector3(1.25f, 0, 0), Quaternion.identity) as GameObject;
		HpRightBar.transform.localScale = new Vector3 (1, 1.5f, 1);
		HpBarFill = Instantiate(BarGreen, HpBarPos + new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		HpBarFill.transform.localScale = new Vector3(HpBarMidScale, 1.5f, 1);
	}
	
	void updateBar() {
		float percentLeft;
		float BarXPos;
		int currentDamage = world.GetComponent<WorldController> ().letThroughAttackers;
		int possibleAttacks = world.GetComponent<WorldController> ().currentLevel.GetComponent<WaveList> ().AttackersLetThrough;

		if (world.GetComponent<WorldController>().isPlayerAttacker == false) {
			percentLeft = (float)(possibleAttacks - currentDamage) / (float)possibleAttacks;
		} else {
			percentLeft = (float)currentDamage / (float)possibleAttacks;
		}

		BarXPos = HpMidBar.transform.position.x - ((1 - percentLeft) * 0.5f * (HpRightBar.transform.position.x - HpLeftBar.transform.position.x + 0.15f));
		HpBarFill.transform.position = new Vector3(BarXPos, HpMidBar.transform.position.y, HpMidBar.transform.position.z);
		HpBarFill.transform.localScale = new Vector3(HpBarMidScale * percentLeft, 1.5f, 1);
	}

	// Update is called once per frame
	void Update () {
		TextMesh txt = this.GetComponent<TextMesh> ();
		int currentDamage = world.GetComponent<WorldController> ().letThroughAttackers;
		int possibleAttacks = world.GetComponent<WorldController> ().currentLevel.GetComponent<WaveList> ().AttackersLetThrough;

		if (currentDamage <= possibleAttacks) {
			txt.text = currentDamage + " / " + possibleAttacks;
			updateBar();
		}
	}
}