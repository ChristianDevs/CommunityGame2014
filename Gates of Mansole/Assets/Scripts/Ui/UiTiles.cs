using UnityEngine;
using System.Collections;

public class UiTiles : MonoBehaviour {

	public Vector2 gridSize;
	public int MaxXTile;
	public GameObject world;

	bool isTilePress;
	Vector2 downTile;
	Vector2 upTile;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (isTilePress == false) {
			if (Input.GetMouseButtonDown (0)) {
				isTilePress = true;
				Vector3 viewPortDown;

				viewPortDown = UnityEngine.Camera.main.ScreenToViewportPoint(Input.mousePosition);
				downTile = new Vector2(Mathf.Floor(viewPortDown.x * gridSize.x), Mathf.Floor(viewPortDown.y * gridSize.y));
			}
		} else {
			if (Input.GetMouseButtonUp(0)) {
				isTilePress = false;
				Vector3 viewPortUp;
				
				viewPortUp = UnityEngine.Camera.main.ScreenToViewportPoint(Input.mousePosition);
				upTile = new Vector2(Mathf.Floor(viewPortUp.x * gridSize.x), Mathf.Floor(viewPortUp.y * gridSize.y));

				if ((downTile == upTile) && (upTile.x < MaxXTile)) {
					world.SendMessage("TileClick", upTile, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}
