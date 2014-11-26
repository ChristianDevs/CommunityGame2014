using UnityEngine;
using System.Collections;

public class UiSpiritOrbs : MonoBehaviour {
	
	public GameObject textTexture;
	
	
	// Use this for initialization
	void Start () {
		textTexture.GetComponent<TextMesh>().text="x"+Player.spiritOrbs;
	}
	
	// Update is called once per frame
	void Update () {
		textTexture.GetComponent<TextMesh>().text="x"+Player.spiritOrbs;
	}
}
