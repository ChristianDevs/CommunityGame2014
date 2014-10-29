using UnityEngine;
using System.Collections;

public class UiTitle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void buttonPush(string buttonName) {
        switch (buttonName) {
            case "Level1":
                Application.LoadLevel("AttackSceneLeft");
                break;
            case "Level2":
                Application.LoadLevel("AttackSceneRight");
                break;
            case "Level3":
                Application.LoadLevel("DefendSceneLeft");
                break;
            case "Level4":
                Application.LoadLevel("DefendSceneRight");
                break;
            case "Quit":
                Application.Quit();
                break;
            default:
                break;
        }
    }
}
