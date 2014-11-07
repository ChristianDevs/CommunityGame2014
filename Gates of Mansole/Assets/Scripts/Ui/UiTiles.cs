using UnityEngine;
using System.Collections;

public class UiTiles : MonoBehaviour {

    public GameObject[] lanes;

	private bool isTilePress;
	private Vector2 downTile;
	private Vector2 upTile;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public UiTile GetMouseOverTile() {
        UiTile retTile;
        RaycastHit hitTile;

        retTile = new UiTile();
        retTile.row = -1;
        retTile.col = -1;

        if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitTile)) {
            for (int row = 0; row < lanes.Length; row++) {
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
