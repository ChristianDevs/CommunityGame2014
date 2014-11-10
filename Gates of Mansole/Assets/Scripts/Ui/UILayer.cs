using UnityEngine;
using System.Collections;

public class UILayer : MonoBehaviour {
	public string sortingLayer;
	public int orderInSortingLayer;

	// Use this for initialization
	void Start () {
		this.renderer.sortingLayerName = sortingLayer;
		this.renderer.sortingOrder = orderInSortingLayer;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
