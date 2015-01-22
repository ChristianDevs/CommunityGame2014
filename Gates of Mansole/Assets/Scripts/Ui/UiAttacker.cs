using UnityEngine;
using System.Collections;

public class UiAttacker : MonoBehaviour {

	public GameObject world;

	// Update is called once per frame
	void Update () {
		TextMesh txt = this.GetComponent<TextMesh> ();
		int possibleAttacks = world.GetComponent<WorldController> ().currentLevel.GetComponent<WaveList> ().AttackersLetThrough;
		txt.text = "Attackers: " + (possibleAttacks - world.GetComponent<WorldController>().letThroughAttackers);
	}
}