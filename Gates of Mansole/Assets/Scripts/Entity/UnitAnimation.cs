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
		Attack,
		Die
	}
	
	public GameObject[] animObjects;
    private float dieTime = -1;

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

    void DieTimer(float timeout) {
        dieTime = Time.time + timeout;
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

    void Update() {
        if (dieTime > 0) {
            if (Time.time > dieTime) {
                Destroy(gameObject);
            }
        }
    }
}
