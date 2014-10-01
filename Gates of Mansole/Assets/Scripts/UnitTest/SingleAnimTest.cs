using UnityEngine;
using System.Collections;

public class SingleAnimTest : MonoBehaviour {
	
	public enum _direction {
		DirUp,
		DirLeft,
		DirDown,
		DirRight
	}
	public _direction Dir;

	public enum _action {
		Idle,
		Walk,
		Slash,
		Bow,
		Die
	}
	public _action Act;

	float lastChange;

	// Use this for initialization
	void Start () {
		Dir = _direction.DirUp;
		Act = _action.Idle;
		lastChange = -1;
	}
	
	// Update is called once per frame
	void Update () {
		if (lastChange < Time.time) {
			GetComponent<Animator>().SetInteger("Direction", (int)Dir);
			GetComponent<Animator>().SetInteger("Action", (int)Act);
			GetComponent<Animator>().SetBool("ChangeAnim", true);

			if (GetComponent<Animator>().GetAnimatorTransitionInfo().

			lastChange = Time.time + 3f;

			Dir++;

			if ((int)Dir > 3) {
				Dir = 0;
				Act++;

				if ((int)Act > 4) {
					Act = 0;
				}
			}
		}
	}
}
