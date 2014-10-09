using UnityEngine;
using System.Collections;

public class UnitAnimation : MonoBehaviour {
	
	public enum _direction {
		DirUp,
		DirLeft,
		DirDown,
		DirRight
	}
	
	public enum _action {
		Idle,
		Walk,
		Slash,
		Bow,
		Die
	}
	
	public GameObject[] animObjects;

	void SetDirection(_direction changeDir) {
		foreach (GameObject go in animObjects) {
			go.GetComponent<Animator>().SetInteger("Direction", (int)changeDir);
		}
	}

	void SetAction(_action changeAct) {
		foreach (GameObject go in animObjects) {
			go.GetComponent<Animator>().SetInteger("Action", (int)changeAct);
		}
	}

	void StartAnimation() {
		foreach (GameObject go in animObjects) {
			go.GetComponent<Animator>().SetBool("ChangeAnim", true);
		}
	}

	public bool isAnimationChanged() {
		bool ret;

		ret = true;

		foreach (GameObject go in animObjects) {
			if (go.GetComponent<Animator>().GetBool("ChangeAnim") == true) {
				ret = false;
			}
		}

		return ret;
	}
}
