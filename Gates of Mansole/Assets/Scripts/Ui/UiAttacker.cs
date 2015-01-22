using UnityEngine;
using System.Collections;

public class UiAttacker : MonoBehaviour {

	public GameObject world;

	// Update is called once per frame
	void Update () {
		TextMesh txt = this.GetComponent<TextMesh> ();
		int currentDamage = world.GetComponent<WorldController> ().letThroughAttackers;
		int possibleAttacks = world.GetComponent<WorldController> ().currentLevel.GetComponent<WaveList> ().AttackersLetThrough;

		if (possibleAttacks > currentDamage) {
			txt.text = "Attackers: " + (possibleAttacks - currentDamage);
		} else { txt.text = "Attackers: 0";	}
	}
}