using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiTiles : MonoBehaviour {
	
	public GameObject[] LaneTerrain;

    public List<GameObject> lanes;

	private bool isTilePress;
	private Vector2 downTile;
	private Vector2 upTile;

	private UiTile BlinkTile;
	private Color blinkColor;
	private float blinkTime;

	// Use this for initialization
	void Start () {
		BlinkTile = new UiTile();
		BlinkTile.row = -1;
		BlinkTile.col = -1;
	}
	
	// Update is called once per frame
	void Update () {
		if ((BlinkTile.row >= 0) && (BlinkTile.col >= 0)) {
			if (blinkTime < Time.time) {
				if (blinkColor == Color.gray) {
					blinkColor = Color.white;
				} else {
					blinkColor = Color.gray;
				}

				foreach(SpriteRenderer sr in lanes[BlinkTile.row].GetComponent<UiRow>().rowTiles[BlinkTile.col].GetComponentsInChildren<SpriteRenderer>()) {
					sr.color = blinkColor;
				}

				blinkTime = Time.time + 0.5f;
			}
		}
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

	public void blinkTile(UiTile tile) {
		blinkTime = 0;
		BlinkTile.col = tile.col;
		BlinkTile.row = tile.row;
	}

	public void stopBlink() {
		foreach(SpriteRenderer sr in lanes[BlinkTile.row].GetComponent<UiRow>().rowTiles[BlinkTile.col].GetComponentsInChildren<SpriteRenderer>()) {
			sr.color = Color.white;
		}

		BlinkTile.row = -1;
		BlinkTile.col = -1;
	}
}
