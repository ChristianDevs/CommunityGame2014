using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreditsController : MonoBehaviour {
	void buttonPush(string buttonName) {
		Application.LoadLevel("Title");
	}
}
