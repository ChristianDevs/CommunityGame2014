using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiTiles : MonoBehaviour {
	
	public GameObject[] LaneTerrain;

    public List<GameObject> lanes;

	private bool isTilePress;
	private Vector2 downTile;
	private Vector2 upTile;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void CreateLane(int terrainType) {
		GameObject lane;

		lane = Instantiate (LaneTerrain [terrainType]) as GameObject;
		lane.transform.parent = transform;
		lane.transform.localPosition = new Vector3 (0, lanes.Count);
		lane.name = "Lane" + lanes.Count;

		lanes.Add (lane);
	}

    public UiTile GetMouseOverTile() {
        UiTile retTile;
        RaycastHit hitTile;

        retTile = new UiTile();
        retTile.row = -1;
        retTile.col = -1;

        if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitTile)) {
            for (int row = 0; row < lanes.Count; row++) {
                UiRow thisRow;

                thisRow = lanes[row].GetComponent<UiRow>();

                for (int col = 0; col < thisRow.rowTiles.Length; col++) {

                    if ((hitTile.transform.name == thisRow.rowTiles[col].transform.name) &&
                        (hitTile.transform.parent.name == thisRow.transform.name)) {
                        retTile.row = row;
                        retTile.col = col;
                    }
                }
            }
        }

        return retTile;
    }
}
