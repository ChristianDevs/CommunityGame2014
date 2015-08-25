using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiSpiritShard : MonoBehaviour {

	public GameObject textTexture;

    private bool showCustom = false;
	public int m_val;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (showCustom == false) {
            textTexture.GetComponent<TextMesh>().text = "x" + Player.spiritShards;
        }
	}

    void SetCustomValue(int val) {
        showCustom = true;
		m_val = val;
        textTexture.GetComponent<TextMesh>().text = "x" + val;
    }
}
