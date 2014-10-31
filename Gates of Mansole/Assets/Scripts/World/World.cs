using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {

	public float TileUnitOffset;
	public GameObject bowUnit;
	public GameObject swordUnit;
    public GameObject bowUI;
    public GameObject bowSquare;
    public GameObject swordUI;
    public GameObject swordSquare;
    public GameObject map;
	public GameObject[] Levels;
    public GameObject winMessage;
    public GameObject loseMessage;
    public bool isLevelDone;

    public int totalAI;
    public int totalAttackers;
    public int totalDefenders;
    public int defeatedAttackers;
    public int defeatedDefenders;
    public int letThroughAttackers;
    public bool isPlayerAttacker;

	private GameObject[][] tileContents;
	private GameObject selectedUnit;
	private Vector2 selectedTile;
    private GameObject currentLevel;
    private int currentLevelNum;
    private WaveList._direction curLevelAttackerDir;
    private UnitAnimation._direction attackerDir;
    private UnitAnimation._direction defenderDir;
    private List<GameObject> unitsUIinst;
    private GameObject selectedUiUnit;
    private UiTile gridSize;

    int GetTotalWaveUnits(GameObject lvl) {
        int ret = 0;

        foreach (Wave wv in currentLevel.GetComponent<WaveList>().waves) {
            ret += wv.units.Length;
        }

        return ret;
    }

	// Use this for initialization
	void Start () {
        selectedUnit = null;

        gridSize = new UiTile();
        gridSize.row = map.GetComponent<UiTiles>().lanes.Length;

        if (map.GetComponent<UiTiles>().lanes.Length > 0) {
            gridSize.col = map.GetComponent<UiTiles>().lanes[0].GetComponent<UiRow>().rowTiles.Length;
        }
        else {
            gridSize.col = 0;
        }

        // Initialize Tiles
        tileContents = new GameObject[gridSize.row][];
        for (int row = 0; row < gridSize.row; row++) {
            tileContents[row] = new GameObject[gridSize.col];

            for (int col = 0; col < gridSize.col; col++) {
				tileContents[row][col] = null;
			}
		}

        if (Levels.Length > 0) {
            // Initialize Current Level
            currentLevelNum = 0;
            currentLevel = Levels[currentLevelNum];
        } else {
            currentLevel = null;
        }

        letThroughAttackers = 0;
        defeatedAttackers = 0;
        defeatedDefenders = 0;
        totalAttackers = 0;
        totalDefenders = 0;
        totalAI = GetTotalWaveUnits(currentLevel);

        isPlayerAttacker = currentLevel.GetComponent<WaveList>().isPlayerAttacker;
        curLevelAttackerDir = currentLevel.GetComponent<WaveList>().attackerDir;
        isLevelDone = false;

        if (curLevelAttackerDir == WaveList._direction.Left) {
            attackerDir = UnitAnimation._direction.DirLeft;
            defenderDir = UnitAnimation._direction.DirRight;
        } else if (curLevelAttackerDir == WaveList._direction.Right) {
            attackerDir = UnitAnimation._direction.DirRight;
            defenderDir = UnitAnimation._direction.DirLeft;
        }

        unitsUIinst = new List<GameObject>();

        unitsUIinst.Add(Instantiate(bowUI, bowSquare.transform.position, Quaternion.identity) as GameObject);
        unitsUIinst.Add(Instantiate(swordUI, swordSquare.transform.position, Quaternion.identity) as GameObject);

        selectedUiUnit = null;

        winMessage.SetActive(false);
        loseMessage.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		// Check if any waves need releasing
		if (currentLevel != null) {
			WaveList wl;

            wl = currentLevel.GetComponent<WaveList>();

            // Go through each wave and see if it is time to start that wave
			foreach (Wave wv in wl.waves) {
				if (wv.waitTime < Time.time) {

                    // Go through each unit and see if it is time to spawn it
					foreach(WaveUnit ut in wv.units) {
						if (ut.created == false) {
                            if ((wv.waitTime + ut.time) < Time.time) {
                                int row;
                                int col;

                                switch (ut.SpawnLocType) {
                                case WaveUnit._spawnLocType.RandRow:
                                    if (curLevelAttackerDir == WaveList._direction.Right) {
                                        col = 0;
                                    } else if (curLevelAttackerDir == WaveList._direction.Left) {
                                        col = gridSize.col - 1;
                                    } else {
                                        col = 0;
                                    }
                                    row = Random.Range(0, (int)gridSize.row);
                                    break;
                                case WaveUnit._spawnLocType.RandTile:
                                    row = 0;
                                    col = 0;
                                    break;
                                case WaveUnit._spawnLocType.SpecifiedRow:
                                    row = 0;
                                    col = 0;
                                    break;
                                case WaveUnit._spawnLocType.SpecifiedTile:
                                    row = (int)ut.Tile.x;
                                    col = (int)ut.Tile.y;
                                    break;
                                default:
                                    row = 0;
                                    col = 0;
                                    break;
                                }

                                ut.created = true;
                                if ((row < gridSize.row) && (col < gridSize.col)) {
                                    if (isPlayerAttacker == true) {
                                        if (SpawnUnit(ut.prefab, row, col, GomObject.Faction.Bad)) {
                                            tileContents[row][col].SendMessage("SetIdleDirection", defenderDir, SendMessageOptions.DontRequireReceiver);
                                            totalDefenders++;
                                        }
                                    } else {
                                        if (SpawnUnit(ut.prefab, row, col, GomObject.Faction.Bad)) {
                                            tileContents[row][col].SendMessage("SetIdleDirection", attackerDir, SendMessageOptions.DontRequireReceiver);
                                            totalAttackers++;
                                        } else {
                                            // Just to keep the game from getting stuck
                                            defeatedAttackers++;
                                        }
                                    }
                                }
							}
						}
					}
				}
			}

            // Check game over conditions
            if (isLevelDone == false) {
                if (isPlayerAttacker == true) {
                    if (letThroughAttackers >= wl.AttackersLetThrough) {
                        winMessage.SetActive(true);
                        Player.completeLevel(Player.currentLevel);
                        isLevelDone = true;
                    }
                } else {
                    if (letThroughAttackers >= wl.AttackersLetThrough) {
                        loseMessage.SetActive(true);
                        isLevelDone = true;
                    } else if ((defeatedAttackers + letThroughAttackers) >= totalAI) {
                        winMessage.SetActive(true);
                        Player.completeLevel(Player.currentLevel);
                        isLevelDone = true;
                    }
                }
            }

            // Check Menu Squares
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hitSquare;

                if (isLevelDone == true) {
                    Application.LoadLevel("LevelSelect");
                }

                if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitSquare)) {
                    if (hitSquare.transform.name == bowSquare.transform.name) {
                        selectedUiUnit = unitsUIinst[0];
                    }

                    if (hitSquare.transform.name == swordSquare.transform.name) {
                        selectedUiUnit = unitsUIinst[1];
                    }
                }
            }

            if (selectedUiUnit != null) {
                UiTile tile;
                Vector3 newPos;

                tile = map.GetComponent<UiTiles>().GetMouseOverTile();

                if ((tile.row != -1) && (tile.col != -1)) {
                    newPos = new Vector3(map.transform.position.x + (float)tile.col + 0.5f, map.transform.position.y + (float)tile.row + 0.5f);
                    selectedUiUnit.transform.position = newPos;
                }
            }

            if (Input.GetMouseButtonUp(0) && (selectedUiUnit != null)) {
                UiTile tile;

                tile = map.GetComponent<UiTiles>().GetMouseOverTile();

                if ((tile.col < gridSize.col) && (tile.row < gridSize.row) && (tile.col >= 0) && (tile.row >= 0)) {
                    UnitAnimation._direction dir;

                    if (isPlayerAttacker == true) {
                        totalAttackers++;
                        dir = attackerDir;
                    } else {
                        totalDefenders++;
                        dir = defenderDir;
                    }

                    if (selectedUiUnit == unitsUIinst[0]) {
                        SpawnUnit(bowUnit, (int)tile.row, (int)tile.col, GomObject.Faction.Good);
                        tileContents[(int)tile.row][(int)tile.col].SendMessage("SetIdleDirection", dir, SendMessageOptions.DontRequireReceiver);
                    } else {
                        SpawnUnit(swordUnit, (int)tile.row, (int)tile.col, GomObject.Faction.Good);
                        tileContents[(int)tile.row][(int)tile.col].SendMessage("SetIdleDirection", dir, SendMessageOptions.DontRequireReceiver);
                    }
                }

                selectedUiUnit = null;
                unitsUIinst[0].transform.position = bowSquare.transform.position;
                unitsUIinst[1].transform.position = swordSquare.transform.position;
            }
		}
	}

	void LateUpdate() {
		
		// Update units in the tiles
		for (int row = 0; row < gridSize.row; row++) {
			for (int col = 0; col < gridSize.col; col++) {
                // If the tile is not empty then see if the unit needs to do something
				if (tileContents[row][col] != null) {
					if (tileContents[row][col].GetComponent<GomUnit>().health <= 0) {
						// Unit is defeated
                        if (isPlayerAttacker == true) {
                            if (tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Good) {
                                totalAttackers--;
                                defeatedAttackers++;
                            } else {
                                totalDefenders--;
                            }
                        } else {
                            if (tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Good) {
                                totalDefenders--;
                            } else {
                                totalAttackers--;
                                defeatedAttackers++;
                            }
                        }

						tileContents[row][col].SendMessage("Die", null, SendMessageOptions.DontRequireReceiver);
						tileContents[row][col] = null;
					} else if (tileContents[row][col].GetComponent<GomUnit>().CanMove()) {
						if (CanUnitAttackLeftRight(row, col) == true) {
							// Unit can attack
                            AttackLeftRightNearestEnemy(row, col);
						} else {
							// Unit can move
                            if (((tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Good) &&
                                currentLevel.GetComponent<WaveList>().isPlayerAttacker) ||
                                ((tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Bad) &&
                                !currentLevel.GetComponent<WaveList>().isPlayerAttacker)) {

                                if (curLevelAttackerDir == WaveList._direction.Right) {
                                    if ((col + 1) >= gridSize.col) {
                                        // Unit passed off the screen
                                        Destroy(tileContents[row][col]);
                                        tileContents[row][col] = null;
                                        letThroughAttackers++;
                                    } else {
                                        if (tileContents[row][col + 1] == null) {
                                            // Advance attacker units to the right
                                            tileContents[row][col].SendMessage("Move", new Vector2(col + 1, row), SendMessageOptions.DontRequireReceiver);
                                            tileContents[row][col + 1] = tileContents[row][col];
                                            tileContents[row][col] = null;
                                        }
                                    }
                                } else if (curLevelAttackerDir == WaveList._direction.Left) {
                                    if (col == 0) {
                                        // Unit passed off the screen
                                        Destroy(tileContents[row][col]);
                                        tileContents[row][col] = null;
                                        letThroughAttackers++;
                                    } else {
                                        if (tileContents[row][col - 1] == null) {
                                            // Advance attacker units to the right
                                            tileContents[row][col].SendMessage("Move", new Vector2(col - 1, row), SendMessageOptions.DontRequireReceiver);
                                            tileContents[row][col - 1] = tileContents[row][col];
                                            tileContents[row][col] = null;
                                        }
                                    }
                                }
							}
						}
					}
				}
			}
		}
	}

	bool CanUnitAttackLeftRight(int row, int col) {
		GomUnit attacker;

		attacker = tileContents[row][col].GetComponent<GomUnit>();

		if (attacker.weapon == null) {
			return false;
		}

		// Check left
		for (int i = col; i >= (col - attacker.weapon.range); i--) {

			if (i < 0) {
				break;
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
			
			if (i > (gridSize.col) - 1) {
				break;
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
	
	
	void AttackLeftRightNearestEnemy(int row, int col) {
		GomUnit attacker;
		
		attacker = tileContents[row][col].GetComponent<GomUnit>();
		
		// Attack to the left
		for (int i = col; i >= (col - attacker.weapon.range); i--) {
			
			if (i < 0) {
				break;
			}
			
			if (tileContents[row][i] != null) {
				if (tileContents[row][i].GetComponent<GomUnit>().faction != attacker.faction) {
					GameObject projectile;
					tileContents[row][i].SendMessage ("SetAttacker", attacker, SendMessageOptions.DontRequireReceiver);
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
			
			if (i > (gridSize.col) - 1) {
				break;
			}
			
			if (tileContents[row][i] != null) {
				if (tileContents[row][i].GetComponent<GomUnit>().faction != attacker.faction) {
					GameObject projectile;
					tileContents[row][i].SendMessage ("SetAttacker", attacker, SendMessageOptions.DontRequireReceiver);
					attacker.SendMessage("Attack", null, SendMessageOptions.DontRequireReceiver);
					
					if ((attacker.weapon != null) && (attacker.weapon.projectile != null)) {
						projectile = Instantiate(attacker.weapon.projectile, attacker.transform.position, Quaternion.identity) as GameObject;
						projectile.SendMessage("SetTarget", tileContents[row][i], SendMessageOptions.DontRequireReceiver);
                        projectile.transform.Rotate(new Vector3(0, 0, 180));
					} else {
						tileContents[row][i].SendMessage("DamageMelee", attacker.stats, SendMessageOptions.DontRequireReceiver);
					}
					
					return;
				}
			}
		}
	}

    bool SpawnUnit(GameObject unitPrefab, int tileRow, int tileCol, GomObject.Faction faction) {
        Vector3 worldPos;
        float xTilePos;
        float yTilePos;

        // 2 units cannot occupy the same tile
        if (tileContents[tileRow][tileCol] != null) {
            return false;
        }

        xTilePos = map.GetComponent<UiTiles>().lanes[tileRow].GetComponent<UiRow>().rowTiles[tileCol].transform.position.x;
        yTilePos = map.GetComponent<UiTiles>().lanes[tileRow].GetComponent<UiRow>().rowTiles[tileCol].transform.position.y;

        worldPos.x = xTilePos + TileUnitOffset;
        worldPos.y = yTilePos + TileUnitOffset;
        worldPos.z = 0;

        tileContents[tileRow][tileCol] = Instantiate(unitPrefab, worldPos, Quaternion.identity) as GameObject;
        tileContents[tileRow][tileCol].SendMessage("SetCurrentTile", new Vector2(tileCol, tileRow), SendMessageOptions.DontRequireReceiver);
        tileContents[tileRow][tileCol].SendMessage("SetFaction", faction, SendMessageOptions.DontRequireReceiver);

        return true;
    }
}