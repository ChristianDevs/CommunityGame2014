using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiSpiritShard : MonoBehaviour {

	public GameObject textTexture;


	// Use this for initialization
	void Start () {
		textTexture.GetComponent<TextMesh>().text="x"+Player.spiritShards;
	}
	
	// Update is called once per frame
	void Update () {
		textTexture.GetComponent<TextMesh>().text="x"+Player.spiritShards;
	}
}
