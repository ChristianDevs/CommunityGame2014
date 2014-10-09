using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {

	public Vector2 gridSize;
	public float TileUnitOffset;
	public GameObject playerUnit;
	public GameObject enemyUnit;
	public GameObject[] Levels;
	public int maxTile;

	private GameObject[][] tileContents;
	private GameObject selectedUnit;
	private Vector2 selectedTile;

	// Use this for initialization
	void Start () {
		selectedUnit = null;

		tileContents = new GameObject[(int)gridSize.y][];
		for (int col = 0; col < (int)gridSize.y; col++) {
			tileContents[col] = new GameObject[(int)gridSize.x];
			
			for (int row = 0; row < (int) gridSize.x; row++) {
				tileContents[col][row] = null;
			}
		}
	}

	void TileClick(Vector2 tile) {
		if ((selectedUnit != null) &&
		    (tileContents[(int)tile.y][(int)tile.x] == null)) {
			selectedUnit.SendMessage("Move", tile, SendMessageOptions.DontRequireReceiver);

			tileContents[(int)tile.y][(int)tile.x] = selectedUnit;
			tileContents[(int)selectedTile.y][(int)selectedTile.x] = null;
			selectedUnit = null;
			//Debug.Log("Move to <" + tile.x + "," + tile.y + ">");
		} else if (tileContents[(int)tile.y][(int)tile.x] == null) {
			Vector3 worldPos;
			
			worldPos.x = Mathf.Ceil(tile.x) - (gridSize.x * 0.5f) + TileUnitOffset;
			worldPos.y = (Mathf.Ceil(tile.y) - (gridSize.y * 0.5f)) + TileUnitOffset;
			worldPos.z = 0;
			
			tileContents[(int)tile.y][(int)tile.x] = Instantiate(playerUnit, worldPos, Quaternion.identity) as GameObject;
			tileContents[(int)tile.y][(int)tile.x].SendMessage("SetCurrentTile", tile, SendMessageOptions.DontRequireReceiver);
			//Debug.Log("Spawn <" + tile.x + "," + tile.y + ">");
		} else {
			selectedUnit = tileContents[(int)tile.y][(int)tile.x];
			selectedTile = tile;
			//Debug.Log("Select <" + tile.x + "," + tile.y + ">");
		}
	}
	
	// Update is called once per frame
	void Update () {

		// Check if any waves need releasing
		foreach (GameObject level in Levels) {
			WaveList wl;

			wl = level.GetComponent<WaveList>();

			foreach (Wave wv in wl.waves) {
				if (wv.waitTime < Time.time) {
					foreach(WaveUnit ut in wv.units) {
						if (ut.created == false) {
							if ((wv.waitTime + ut.time) < Time.time) {
								Vector3 worldPos;
								int row;

								row = Random.Range(0, (int)gridSize.y);
								
								worldPos.x = Mathf.Ceil(0) - (gridSize.x * 0.5f) + TileUnitOffset;
								worldPos.y = (Mathf.Ceil(row) - (gridSize.y * 0.5f)) + TileUnitOffset;
								worldPos.z = 0;

								tileContents[row][0] = Instantiate(enemyUnit, worldPos, Quaternion.identity) as GameObject;
								tileContents[row][0].SendMessage("SetCurrentTile", new Vector2(0, row), SendMessageOptions.DontRequireReceiver);
								ut.created = true;
							}
						}
					}
				}
			}
		}
	}

	void LateUpdate() {
		
		// Update units in the tiles
		for (int row = 0; row < gridSize.y; row++) {
			for (int col = 0; col < gridSize.x; col++) {
				if (tileContents[row][col] != null) {
					if (tileContents[row][col].GetComponent<GomUnit>().health <= 0) {
						// Unit is defeated
						tileContents[row][col].SendMessage("Die", null, SendMessageOptions.DontRequireReceiver);
						tileContents[row][col] = null;
					} else if (tileContents[row][col].GetComponent<GomUnit>().CanMove()) {
						if (col >= maxTile) {
							// Unit passed off the screen
							Destroy (tileContents[row][col]);
							tileContents[row][col] = null;
						} else if (CanUnitAttackLeftRight(row, col) == true) {
							// Unit can attack
							AttackLeftNearestEnemy(row, col);
						} else if (tileContents[row][col + 1] == null) {
							// Unit can move
							if (tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Bad) {
								// Advance "bad" units to the right
								tileContents[row][col].SendMessage("Move", new Vector2(col + 1, row), SendMessageOptions.DontRequireReceiver);
								tileContents[row][col + 1] = tileContents[row][col];
								tileContents[row][col] = null;
							}
						}
					}
				}
			}
		}
	}

	bool CanUnitAttackLeftRight(int row, int col) {
		GomUnit attacker;

		attacker = tileContents [row] [col].GetComponent<GomUnit>();

		if (attacker.weapon == null) {
			return false;
		}

		// Check left
		for (int i = col; i >= (col - attacker.weapon.range); i--) {

			if (i < 0) {
				return false;
			}

			if (tileContents[row][i] != null) {
			    if ((tileContents[row][i].GetComponent<GomUnit>().faction != attacker.faction) &&
				    (tileContents[row][i].GetComponent<GomUnit>().health > 0)){
					return true;
				}
			}
		}

		// Check right
		for (int i = col; i <= (col + attacker.weapon.range); i++) {
			
			if (i > (gridSize.x) - 1) {
				return false;
			}

			if (tileContents[row][i] != null) {
				if ((tileContents[row][i].GetComponent<GomUnit>().faction != attacker.faction) &&
				    (tileContents[row][i].GetComponent<GomUnit>().health > 0)){
					return true;
				}
			}
		}

		return false;
	}

	void AttackLeftNearestEnemy(int row, int col) {
		GomUnit attacker;
		
		attacker = tileContents [row] [col].GetComponent<GomUnit>();

		// Attack to the left
		for (int i = col; i >= (col - attacker.weapon.range); i--) {
			
			if (i < 0) {
				return;
			}
			
			if (tileContents[row][i] != null) {
				if (tileContents[row][i].GetComponent<GomUnit>().faction != attacker.faction) {
					GameObject projectile;
					attacker.SendMessage("Attack", null, SendMessageOptions.DontRequireReceiver);

					if ((attacker.weapon != null) && (attacker.weapon.projectile != null)) {
						projectile = Instantiate(attacker.weapon.projectile, attacker.transform.position, Quaternion.identity) as GameObject;
						projectile.SendMessage("SetTarget", tileContents[row][i], SendMessageOptions.DontRequireReceiver);
					} else {
						tileContents[row][i].SendMessage("DamageMelee", attacker.stats, SendMessageOptions.DontRequireReceiver);
					}

					return;
				}
			}
		}

		// Attack to the right
		for (int i = col; i <= (col + attacker.weapon.range); i++) {
			
			if (i > (gridSize.x) - 1) {
				return;
			}
			
			if (tileContents[row][i] != null) {
				if (tileContents[row][i].GetComponent<GomUnit>().faction != attacker.faction) {
					GameObject projectile;
					attacker.SendMessage("Attack", null, SendMessageOptions.DontRequireReceiver);
					
					if ((attacker.weapon != null) && (attacker.weapon.projectile != null)) {
						projectile = Instantiate(attacker.weapon.projectile, attacker.transform.position, Quaternion.identity) as GameObject;
						projectile.SendMessage("SetTarget", tileContents[row][i], SendMessageOptions.DontRequireReceiver);
					} else {
						tileContents[row][i].SendMessage("DamageMelee", attacker.stats, SendMessageOptions.DontRequireReceiver);
					}

					return;
				}
			}
		}
	}
}
