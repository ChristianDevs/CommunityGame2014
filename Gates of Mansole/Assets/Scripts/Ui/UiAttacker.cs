﻿using UnityEngine;
using System.Collections;

public class UiAttacker : MonoBehaviour {

	public GameObject world;

	// Update is called once per frame
	void Update () {
		TextMesh txt = this.GetComponent<TextMesh> ();
		txt.text = "Attackers: " + world.GetComponent<WorldController>().letThroughAttackers;
	}
}