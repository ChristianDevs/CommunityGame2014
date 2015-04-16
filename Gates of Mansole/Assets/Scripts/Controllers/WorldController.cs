using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {
	//Comment to overwrite
	public float TileUnitOffset;
	public GameObject map;
	public GameObject[] Levels;
	public GameObject winMessage;
	public GameObject loseMessage;
	public bool isLevelDone;
	public GameObject VertWallTop;
	public GameObject VertWallMid;
	public GameObject VertWallBot;
	public GameObject CursorPrefab;
	
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
	public GameObject currentLevel;
	private int currentLevelNum;
	private WaveList._direction curLevelAttackerDir;
	private UnitAnimation._direction attackerDir;
	private UnitAnimation._direction defenderDir;
	private GameObject selectedUiUnit;
	private GameObject selectedUiAbility;
	private GameObject selectedAbility;
	private UiTile gridSize;
	
	public List<GameObject> squares;
	public List<GameObject> unitsUIinst;
	public List<GameObject> unitTypes;
	public List<GameObject> abilityUIinst;
	public List<GameObject> abilities;
	public float levelStartTime;
	public GameObject unitInfoUi;
	private int dialogueIndex;
	public TextMesh dialogueText;
	public SpriteRenderer leftImage;
	public SpriteRenderer rightImage;
	public GameObject dialogueWindow;
	public int dialogueLineSize;
	public Sprite[] Images;
	public float unitMenuInterval;
	public GameObject[] Placeables;
	public AudioClip[] Music;
	public int CurWave;
	
	public GameObject BarEmptyLeft;
	public GameObject BarEmptyMid;
	public GameObject BarEmptyRight;
	public GameObject BarGreen;
	public GameObject BarRed;
	
	public int earlyReleaseShards;
	public GameObject rewardShardPrefab;
	public GameObject spiritShardUI;
	private int rwCounter;

	public GameObject UnitCounterPrefab;
	private GameObject UnitCounterInst;

	public GameObject ReleaseButton;
	private int numUnitsSpawnedLeftInWave;

	public AudioClip AttCrossClip;
	public AudioClip UnitDieClip;
	public AudioClip VictoryClip;
	public AudioClip DefeatClip;

	private bool isFirstWin;
	
	private int OrbCurAmount;
	private float ShardCurAmount;
	private float ConvertStepIntervalTime;
	private float convertTime;

	private GameObject wall;
	private List<GameObject> wallSegments;
	
	// For Unit Freeze ability
	private List<GameObject> freezeIcons;
	private float freezeEndTime;
	private float camNextMoveTime;
	private Vector3 origMapPos;

	private bool inTutorial;
	private GameObject cursorInst;
	private float tutorialTime;
	
	private enum _ConvertState {
		PreWait,
		Converting,
		Done
	}
	_ConvertState cvState;
	
	public enum _WorldState {
		Setup,
		PreDialogue,
		Play,
		CollectOrbs,
		PostDialogue
	}
	_WorldState state;
	
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
		letThroughAttackers = 0;
		defeatedAttackers = 0;
		defeatedDefenders = 0;
		totalAttackers = 0;
		totalDefenders = 0;
		isLevelDone = false;
		selectedUiUnit = null;
		freezeEndTime = -1f;
		origMapPos = map.transform.position;
		CurWave = 0;
		earlyReleaseShards = 0;
		rwCounter = 0;
		isFirstWin = false;
		inTutorial = false;
		tutorialTime = 0;
		cursorInst = null;
		UnitCounterInst = null;

		ReleaseButton.SetActive(false);
		numUnitsSpawnedLeftInWave = 0;
		
		winMessage.SetActive(false);
		loseMessage.SetActive(false);
		
		levelStartTime = 0;
		state = _WorldState.Setup;
		
		if (Levels.Length > 0) {
			// Initialize Current Level
			currentLevelNum = 0;
			currentLevel = Levels[currentLevelNum];
		}
		else {
			currentLevel = null;
		}
	}
	
	// Update is called once per frame
	void Update () {

		// End Tutorial after a specific time
		if (inTutorial) {
			if (tutorialTime < Time.time) {
				inTutorial = false;

				if (cursorInst != null) {
					Destroy(cursorInst);
					cursorInst = null;
				}

				Player.completeTutorialState();
			}
		}
		
		switch (state) {
		case _WorldState.Setup:
			if (handleSetup() == true) {
				state = _WorldState.PreDialogue;
				dialogueIndex = -1;
				dialogueWindow.SetActive(true);
			}
			break;
		case _WorldState.PreDialogue:
			if (handleDialogue(currentLevel.GetComponent<WaveList>().preLevelDialogue)) {
				state = _WorldState.Play;
				levelStartTime = Time.time;
				dialogueIndex = -1;
				dialogueWindow.SetActive(false);
			}
			break;
		case _WorldState.Play:
			handlePlay();
			break;
		case _WorldState.CollectOrbs:
			handleCollectOrbs();
			break;
		case _WorldState.PostDialogue:
			if (handleDialogue(currentLevel.GetComponent<WaveList>().postLevelDialogue)) {
				dialogueWindow.SetActive(false);
				winMessage.SetActive(true);
			}
			break;
		}
	}
	
	void LateUpdate() {
		if (gridSize == null) {
			return;
		}
		
		// Update units in the tiles
		for (int row = 0; row < gridSize.row; row++) {
			for (int col = 0; col < gridSize.col; col++) {
				UnitAI(row, col);
			}
		}

		if (wall != null) {
			WallAI();
		}
	}

	void WallAI() {
		int numAttackers = 0;
		
		// Update the wall to see if all lanes have attackers
		if (isPlayerAttacker == true) {
			if (curLevelAttackerDir == WaveList._direction.Left) {
				for (int row = currentLevel.GetComponent<WaveList>().wallBotRow;
				     row <= currentLevel.GetComponent<WaveList>().wallTopRow;
				     row++) {
					
					if (tileContents[row][currentLevel.GetComponent<WaveList>().wallCol + 1] != null) {
						GameObject unit;
						
						unit = tileContents[row][currentLevel.GetComponent<WaveList>().wallCol + 1];
						
						if (unit.GetComponent<GomUnit>().faction == GomObject.Faction.Player) {
							numAttackers++;
						}
					}
				}
			} else {
				for (int row = currentLevel.GetComponent<WaveList>().wallBotRow;
				     row <= currentLevel.GetComponent<WaveList>().wallTopRow;
				     row++) {
					
					if (tileContents[row][currentLevel.GetComponent<WaveList>().wallCol - 1] != null) {
						GameObject unit;
						
						unit = tileContents[row][currentLevel.GetComponent<WaveList>().wallCol - 1];
						
						if (unit.GetComponent<GomUnit>().faction == GomObject.Faction.Player) {
							numAttackers++;
						}
					}
				}
			}
		} else {
			if (curLevelAttackerDir == WaveList._direction.Left) {
				for (int row = currentLevel.GetComponent<WaveList>().wallBotRow;
				     row <= currentLevel.GetComponent<WaveList>().wallTopRow;
				     row++) {

					if (tileContents[row][currentLevel.GetComponent<WaveList>().wallCol + 1] != null) {
						GameObject unit;

						unit = tileContents[row][currentLevel.GetComponent<WaveList>().wallCol + 1];
						
						if (unit.GetComponent<GomUnit>().faction == GomObject.Faction.Enemy) {
							numAttackers++;
						}
					}
				}
			} else {
				for (int row = currentLevel.GetComponent<WaveList>().wallBotRow;
				     row <= currentLevel.GetComponent<WaveList>().wallTopRow;
				     row++) {
					
					if (tileContents[row][currentLevel.GetComponent<WaveList>().wallCol - 1] != null) {
						GameObject unit;
						
						unit = tileContents[row][currentLevel.GetComponent<WaveList>().wallCol - 1];
						
						if (unit.GetComponent<GomUnit>().faction == GomObject.Faction.Enemy) {
							numAttackers++;
						}
					}
				}
			}
		}
		
		int requiredAttackers = currentLevel.GetComponent<WaveList>().wallTopRow - currentLevel.GetComponent<WaveList>().wallBotRow + 1;
		if (numAttackers >= requiredAttackers) {
			wall.GetComponent<GomUnit>().allLanesAttacking = true;
		} else {
			wall.GetComponent<GomUnit>().allLanesAttacking = false;
		}
	}
	
	void PauseUnits() {
		for (int row = 0; row < gridSize.row; row++) {
			for (int col = 0; col < gridSize.col; col++) {
				if (tileContents[row][col] != null) {
					tileContents[row][col].SendMessage("Pause", null, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
	
	void UnpauseUnits() {
		for (int row = 0; row < gridSize.row; row++) {
			for (int col = 0; col < gridSize.col; col++) {
				if (tileContents[row][col] != null) {
					tileContents[row][col].SendMessage("Unpause", null, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
	
	bool handleSetup() {
		if (Player.nextLevelFile != "") {
			if (currentLevel.GetComponent<WaveList>().unitTypes == null) {
				currentLevel.GetComponent<WaveList>().loadGameFile(Player.nextLevelFile, unitTypes);
			}

			if (currentLevel.GetComponent<WaveList>().Lanes.Count == 0) {
				return false;
			}

			// Reset all stats
			foreach (GameObject unitType in unitTypes) {
				UiUnitType uType;
				uType = unitType.GetComponent<UiUnitType>();
				
				if (uType != null) {
					uType.getPlayerStats().resetUnitStats(uType);
					uType.getEnemyStats().resetUnitStats(uType);
				}
			}
			
			// Generate map lanes
			map.GetComponent<UiTiles>().lanes = new List<GameObject>();
			for (int i = 0; i < currentLevel.GetComponent<WaveList>().Lanes.Count; i++) {
				map.GetComponent<UiTiles>().CreateLane(currentLevel.GetComponent<WaveList>().Lanes[i]);
			}
			
			// Place the placeables
			foreach(WaveList._placeable pl in currentLevel.GetComponent<WaveList>().placeables) {
				Placeables[pl.num] = Instantiate(Placeables[pl.num], new Vector3(pl.x, pl.y), Quaternion.identity) as GameObject;
				Placeables[pl.num].transform.parent = map.transform;
			}
			
			// Play music
			if ((currentLevel.GetComponent<WaveList>().backGroundMusic >= 0) && (currentLevel.GetComponent<WaveList>().backGroundMusic < Music.Length)) {
				AudioSource.PlayClipAtPoint(Music[currentLevel.GetComponent<WaveList>().backGroundMusic], transform.position);
			}
			
			Player.spiritShards = Player.totalShards = currentLevel.GetComponent<WaveList>().startShards;
			
			totalAI = GetTotalWaveUnits(currentLevel);
			
			isPlayerAttacker = currentLevel.GetComponent<WaveList>().isPlayerAttacker;
			curLevelAttackerDir = currentLevel.GetComponent<WaveList>().attackerDir;
			
			gridSize = new UiTile();
			gridSize.row = map.GetComponent<UiTiles>().lanes.Count;
			
			if (map.GetComponent<UiTiles>().lanes.Count > 0) {
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
			
			if (curLevelAttackerDir == WaveList._direction.Left) {
				attackerDir = UnitAnimation._direction.DirLeft;
				defenderDir = UnitAnimation._direction.DirRight;
			}
			else if (curLevelAttackerDir == WaveList._direction.Right) {
				attackerDir = UnitAnimation._direction.DirRight;
				defenderDir = UnitAnimation._direction.DirLeft;
			}

			// Create Wall
			if (currentLevel.GetComponent<WaveList>().usingWall == true) {
				GameObject wallTile = map.GetComponent<UiTiles>().lanes[currentLevel.GetComponent<WaveList>().wallBotRow].GetComponent<UiRow>().rowTiles[currentLevel.GetComponent<WaveList>().wallCol];
				Vector3 wallPos = new Vector3(wallTile.transform.position.x - 0.125f, wallTile.transform.position.y - 0.25f, 0);

				wall = new GameObject();
				wallSegments = new List<GameObject>();

				wallSegments.Add(Instantiate(VertWallBot, wallPos, Quaternion.identity) as GameObject);
				tileContents[currentLevel.GetComponent<WaveList>().wallBotRow][currentLevel.GetComponent<WaveList>().wallCol] = wall;

				int numWallMid = currentLevel.GetComponent<WaveList>().wallTopRow - currentLevel.GetComponent<WaveList>().wallBotRow + 1;
				for (int wallInd = 1; wallInd < numWallMid; wallInd++) {
					wallTile = map.GetComponent<UiTiles>().lanes[currentLevel.GetComponent<WaveList>().wallBotRow + wallInd].GetComponent<UiRow>().rowTiles[currentLevel.GetComponent<WaveList>().wallCol];
					wallPos = new Vector3(wallTile.transform.position.x - 0.125f, wallTile.transform.position.y + 0.375f, 0);
					wallSegments.Add(Instantiate(VertWallMid, wallPos, Quaternion.identity) as GameObject);
					tileContents[currentLevel.GetComponent<WaveList>().wallBotRow + wallInd][currentLevel.GetComponent<WaveList>().wallCol] = wall;

					// Don't make walls off the end of the screen;
					if (wallInd >= 7) {
						break;
					}
				}

				if (currentLevel.GetComponent<WaveList>().wallTopRow < 7) {
					wallTile = map.GetComponent<UiTiles>().lanes[currentLevel.GetComponent<WaveList>().wallTopRow + 1].GetComponent<UiRow>().rowTiles[currentLevel.GetComponent<WaveList>().wallCol];
					wallPos = new Vector3(wallTile.transform.position.x - 0.125f, wallTile.transform.position.y + 0.375f, 0);
					wallSegments.Add(Instantiate(VertWallTop, wallPos, Quaternion.identity) as GameObject);
					tileContents[currentLevel.GetComponent<WaveList>().wallTopRow][currentLevel.GetComponent<WaveList>().wallCol] = wall;
				}

				wall.transform.position = wallSegments[wallSegments.Count / 2].transform.position;

				foreach(GameObject ws in wallSegments) {
					ws.transform.parent = wall.transform;
				}

				wall.AddComponent<GomUnit>();
				wall.GetComponent<GomUnit>().world = gameObject;
				wall.GetComponent<GomUnit>().unitType = "Wall";
				wall.GetComponent<GomUnit>().getStats().armor = 0;
				wall.GetComponent<GomUnit>().getStats().attack = 0;
				wall.GetComponent<GomUnit>().getStats().defense = 0;
				wall.GetComponent<GomUnit>().getStats().maxHealth = currentLevel.GetComponent<WaveList>().wallHealth;
				wall.GetComponent<GomUnit>().health = wall.GetComponent<GomUnit>().getStats().maxHealth;
				wall.GetComponent<GomUnit>().BarEmptyLeft = BarEmptyLeft;
				wall.GetComponent<GomUnit>().BarEmptyRight = BarEmptyRight;
				wall.GetComponent<GomUnit>().BarEmptyMid = BarEmptyMid;
				wall.GetComponent<GomUnit>().BarGreen = BarGreen;
				wall.GetComponent<GomUnit>().BarRed = BarRed;
				wall.GetComponent<GomUnit>().regenRate = currentLevel.GetComponent<WaveList>().wallHealthRegen;
				wall.GetComponent<GomUnit>().fallPercent = currentLevel.GetComponent<WaveList>().wallFallPercent;
				wall.GetComponent<GomUnit>().alive = true;

				if (isPlayerAttacker == true) {
					wall.GetComponent<GomUnit>().faction = GomObject.Faction.Enemy;
				} else {
					wall.GetComponent<GomUnit>().faction = GomObject.Faction.Player;
				}
				wall.GetComponent<GomUnit>().setBarColor ();
			}
		}

			return true;
	}
	
	bool handleDialogue(List<WaveList._statement> dialogue) {
		if (dialogue == null) {
			return true;
		}
		
		if (dialogue.Count < 1) {
			return true;
		}
		
		if (dialogueIndex == -1) {
			dialogueIndex = 0;
			dialogueText.text = processDialogue(dialogue[dialogueIndex].Speaker, dialogue[dialogueIndex].dialogue);
			
			if (dialogue[dialogueIndex].LeftImage == "None") {
				leftImage.sprite = null;
			} else if (int.Parse(dialogue[dialogueIndex].LeftImage) < Images.Length) {
				leftImage.sprite = Images[int.Parse(dialogue[dialogueIndex].LeftImage)];
			}
			
			if (dialogue[dialogueIndex].RightImage == "None") {
				rightImage.sprite = null;
			} else if (int.Parse(dialogue[dialogueIndex].RightImage) < Images.Length) {
				rightImage.sprite = Images[int.Parse(dialogue[dialogueIndex].RightImage)];
			}
		}
		
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hitSquare;
			
			dialogueIndex++;
			
			if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitSquare)) {
				if (hitSquare.transform.name == "Back") {
					// Go backward by cancelling the ++ done earlier and then -- by doing -=2
					dialogueIndex -= 2;
					
					if (dialogueIndex < 0) {
						dialogueIndex = 0;
					}
				}
			}
			
			if (dialogueIndex < dialogue.Count) {
				if (dialogue[dialogueIndex].dialogue == null) {
					return true;
				}

				dialogueText.text = processDialogue(dialogue[dialogueIndex].Speaker, dialogue[dialogueIndex].dialogue);
				
				if (dialogue[dialogueIndex].LeftImage == "None") {
					leftImage.sprite = null;
				}
				else if (int.Parse(dialogue[dialogueIndex].LeftImage) < Images.Length) {
					leftImage.sprite = Images[int.Parse(dialogue[dialogueIndex].LeftImage)];
				}
				
				if (dialogue[dialogueIndex].RightImage == "None") {
					rightImage.sprite = null;
				}
				else if (int.Parse(dialogue[dialogueIndex].RightImage) < Images.Length) {
					rightImage.sprite = Images[int.Parse(dialogue[dialogueIndex].RightImage)];
				}
			} else {
				return true;
			}
		}

		return false;
	}
	
	string processDialogue(string speaker, string text) {
		string newText = "";
		int lineLenth = 0;
		char[] seps = { ' ' };

		if (speaker != "None") {
			newText += speaker;
			newText += ":\n\n";
		}

		if (text == null) {
			return newText;
		}
		
		foreach (string word in text.Split(seps)) {
			if ((lineLenth + word.Length) >= dialogueLineSize) {
				newText += "\n";
				lineLenth = word.Length + 1;
			} else {
				lineLenth += word.Length + 1;
			}
			
			newText += word;
			newText += " ";
		}
		
		return newText;
	}
	
	void handlePlay() {
		// Check if any waves need releasing
		if (currentLevel != null) {
			WaveList wl;
			
			wl = currentLevel.GetComponent<WaveList>();

			// Check if Release Button should be enabled
			if ((numUnitsSpawnedLeftInWave == 0) &&
			    (ReleaseButton.activeSelf == false) &&
			    (CurWave < wl.waves.Count)) {
				ReleaseButton.SetActive(true);
			}
			
			// Go through each wave and see if it is time to start that wave
			for (int wvInd = 0; wvInd < wl.waves.Count; wvInd++) {
				Wave wv = wl.waves[wvInd];
				
				if (wv.waitTime + levelStartTime < Time.time) {
					
					if (wl.waveStarted[wvInd] == false) {
						CurWave++;
						numUnitsSpawnedLeftInWave = wv.units.Length;
						foreach(int upWave in wl.upgradeAtWave) {
							if (upWave == wvInd) {
								Debug.Log("Enemy units get stronger");
								foreach(GameObject ut in unitTypes) {
									PropertyStats unitStats = ut.GetComponent<UiUnitType>().getEnemyStats();
									unitStats.upgradeUnit(ut.GetComponent<UiUnitType>().UnitName);
								}
							}
						}
						wl.waveStarted[wvInd] = true;
					}
					
					// Go through each unit and see if it is time to spawn it
					foreach (WaveUnit ut in wv.units) {
						if (ut.created == false) {
							
							if ((wv.waitTime + ut.time + levelStartTime) < Time.time) {
								bool repeat;
								
								do {
									int row;
									int col;
									
									repeat = false;
									
									switch (ut.SpawnLocType) {
									case WaveUnit._spawnLocType.RandRow:
										if (curLevelAttackerDir == WaveList._direction.Right) {
											col = 0;
										}
										else if (curLevelAttackerDir == WaveList._direction.Left) {
											col = gridSize.col - 1;
										}
										else {
											col = 0;
										}
										row = Random.Range(0, (int)gridSize.row);
										break;
									case WaveUnit._spawnLocType.RandTile:
										row = 0;
										col = 0;
										break;
									case WaveUnit._spawnLocType.SpecifiedRow:
										row = (int)ut.Tile.x;

										if (isPlayerAttacker == true) {
											col = 0;
										} else {
											if (attackerDir == UnitAnimation._direction.DirRight) {
												col = 0;
											} else {
												col = gridSize.col - 1;
											}
										}
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
									
									// Show the player how to spawn a unit
									if ((Player.tutorialState == 1) && (inTutorial == false)) {
										cursorInst = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
										tutorialTime = Time.time + 6.2f;
										inTutorial = true;
									}
									
									ut.created = true;
									if ((row < gridSize.row) && (col < gridSize.col)) {
										if (isPlayerAttacker == true) {
											if (SpawnUnit(ut.prefab, row, col, GomObject.Faction.Enemy)) {
												tileContents[row][col].SendMessage("SetIdleDirection", defenderDir, SendMessageOptions.DontRequireReceiver);
												totalDefenders++;
												numUnitsSpawnedLeftInWave--;
											}
										}
										else {
											if (SpawnUnit(ut.prefab, row, col, GomObject.Faction.Enemy)) {
												tileContents[row][col].SendMessage("SetIdleDirection", attackerDir, SendMessageOptions.DontRequireReceiver);
												totalAttackers++;
												numUnitsSpawnedLeftInWave--;
											} else {
												// Retry placing the attacking enemy
												if (ut.SpawnLocType == WaveUnit._spawnLocType.RandRow) {
													repeat = true;
												}
											}
										}
									}
								} while (repeat);
							}
						}
					}
				}

				// Give the player bonus shards to release waves early
				if (CurWave < currentLevel.GetComponent<WaveList>().waves.Count) {
					float nextWaveTime = currentLevel.GetComponent<WaveList>().waves[CurWave].waitTime - (Time.time - levelStartTime);
					int nextWaveEnemies = currentLevel.GetComponent<WaveList>().waves[CurWave].units.Length;
					earlyReleaseShards = (int)((nextWaveTime * nextWaveEnemies) * 0.1f);
				}
			}
			
			// Check game over conditions
			if (isLevelDone == false) {
				if (isPlayerAttacker == true) {
					if (letThroughAttackers >= wl.AttackersLetThrough) {
						// Player got enough attackers through
						winLevel();
					} else if (totalAttackers == 0) {
						bool canAfford = false;
						
						// Check if the player has enough spirit shards to spawn a unit
						foreach (GameObject ut in unitTypes) {
							if (Player.spiritShards >= ut.GetComponent<UiUnitType>().getRandomUnit().GetComponent<GomUnit>().cost) {
								canAfford = true;
							}
						}
						
						// Check if the player has enough spirit shards to use an ability
						foreach (GameObject ab in abilities) {
							if (Player.spiritShards >= ab.GetComponent<Ability>().getUseCost()) {
								canAfford = true;
							}
						}
						
						if (canAfford == false) {
							// Player ran out of resources
							loseLevel();
						}
					}
				} else {
					if (letThroughAttackers >= wl.AttackersLetThrough) {
						// Player let too many attackers through
						loseLevel();
					} else if ((defeatedAttackers + letThroughAttackers) >= totalAI) {
						// Player defeated all attackers
						winLevel();
					}
				}
			}
			// Handle the level being done
			if (isLevelDone == true) {
				if (loseMessage.activeSelf != true) {
					state = _WorldState.CollectOrbs;
					if (currentLevel.GetComponent<WaveList>().postLevelDialogue.Count > 0) {
						dialogueWindow.SetActive(true);
					}
				}
			}
			if (Input.GetMouseButtonDown(0)) {
				RaycastHit hitSquare;
				
				// Check Menu Squares
				if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitSquare)) {
					foreach (GameObject square in squares) {
						// User selected a menu, find out what it is
						if (hitSquare.transform.name == square.transform.name) {
							// Check if user selected a unit type
							for (int i = 0; i < unitTypes.Count; ++i) {
								if (hitSquare.transform.name == unitTypes[i].GetComponent<UiUnitType>().UnitName) {
									selectedUiUnit = unitsUIinst[i];
									unitInfoUi.SendMessage("MakeSelection", unitTypes[i].GetComponent<UiUnitType>().getRandomUnit(), SendMessageOptions.DontRequireReceiver);

									if (UnitCounterInst != null) {
										Destroy(UnitCounterInst);
									}

									UnitCounterInst = Instantiate (UnitCounterPrefab, selectedUiUnit.transform.position, Quaternion.identity) as GameObject;
									UnitCounterInst.GetComponent<UiUnitCounter> ().SetCounterDisplay (unitTypes[i], Player.unitTypes, false);
									break;
								}
							}
							
							// Check if user selected an ability
							for (int i = 0; i < abilities.Count; ++i) {
								if (hitSquare.transform.name == abilities[i].GetComponent<Ability>().abilityName) {
									selectedUiAbility = abilityUIinst[i];
									selectedAbility = abilities[i];unitInfoUi.SendMessage("MakeSelection", abilities[i], SendMessageOptions.DontRequireReceiver);

									break;
								}
							}
							
							break;
						}
					}
				}
			}
			
			// User wants to spawn a unit
			if (selectedUiUnit != null) {
				UiTile tile;
				Vector3 newPos;
				
				tile = map.GetComponent<UiTiles>().GetMouseOverTile();
				
				if ((tile.row != -1) && (tile.col != -1) && isPlayerSpawnLocValid(tile)) {
					newPos = new Vector3(map.transform.position.x + ((float)tile.col * map.transform.localScale.x),
					                     map.transform.position.y + ((float)tile.row * map.transform.localScale.y) + TileUnitOffset);
					selectedUiUnit.transform.position = newPos;
				}
				
				if (Input.GetMouseButtonUp(0)) {
					if (isPlayerSpawnLocValid(tile)) {
						handleSelectedUnitType();
					} else {
						for (int i = 0; i < unitsUIinst.Count; ++i) {
							if (selectedUiUnit == unitsUIinst[i]) {
								selectedUiUnit.transform.position = new Vector3((float)(-6 + (unitMenuInterval * i)), (float)-5.5, (float)0);
								break;
							}
						}
						
						selectedUnit = selectedUiUnit;
						selectedUiUnit = null;
					}
				}
			}
			
			// User wants to use an ability
			if (selectedUiAbility != null) {
				switch (selectedAbility.GetComponent<Ability>().abilityType) {
				case Ability._type.rowDamage:
					handleRowDamageAbility();
					break;
				case Ability._type.radiusDamage:
					handleRadiusDamageAbility();
					break;
				case Ability._type.freezeEnemyUnit:
					handleFreezeEnemyUnitAbility();
					break;
				case Ability._type.ShieldUnit:
					handleShieldUnitAbility();
					break;
				}
			}
			
			//Send unit info
			if (Input.GetMouseButtonDown(0)) {
				UiTile tile;
				
				tile = map.GetComponent<UiTiles>().GetMouseOverTile();
				
				if ((tile.col < gridSize.col) && (tile.row < gridSize.row) && (tile.col >= 0) && (tile.row >= 0)) {
					if (tileContents[(int)tile.row][(int)tile.col] != null) {
						GameObject ut = null;
						GameObject ui_ut = null;
						
						foreach (GameObject unit in unitTypes) {
							if (tileContents[(int)tile.row][(int)tile.col].GetComponent<GomUnit>().unitType == unit.GetComponent<UiUnitType>().UnitName) {
								ut = unit.GetComponent<UiUnitType>().getRandomUnit();
								ui_ut = unit;
								break;
							}
						}
						
						if (ut != null) {
							unitInfoUi.SendMessage("MakeSelection", ut, SendMessageOptions.DontRequireReceiver);

							if (UnitCounterInst != null) {
								Destroy(UnitCounterInst);
							}

							Vector3 counterPos;

							counterPos = tileContents[(int)tile.row][(int)tile.col].transform.position;
							UnitCounterInst = Instantiate (UnitCounterPrefab, counterPos, Quaternion.identity) as GameObject;
							UnitCounterInst.GetComponent<UiUnitCounter>().SetCounterDisplay (ui_ut, Player.unitTypes, false);
							UnitCounterInst.transform.parent = tileContents[(int)tile.row][(int)tile.col].transform;
						}
					}
				}
			}
			
			if (Input.GetMouseButtonUp(0)) {
				if (UnitCounterInst != null) {
					Destroy(UnitCounterInst);
				}
			}
		}
		
		if (freezeEndTime > Time.time) {
			if (map.transform.position == origMapPos) {
				switch(Random.Range(0,4)) {
				case 0:
					map.transform.position += new Vector3(0, 0.1f);
					break;
				case 1:
					map.transform.position -= new Vector3(0, 0.1f);
					break;
				case 2:
					map.transform.position += new Vector3(0.1f, 0);
					break;
				case 3:
					map.transform.position -= new Vector3(0.1f, 0);
					break;
				}
			} else {
				map.transform.position = origMapPos;
			}
		}
	}
	
	bool isPlayerSpawnLocValid(UiTile tile) {
		if (isPlayerAttacker == false) {
			// In Defense mode limit spawning to the player's half of the map
			if (attackerDir == UnitAnimation._direction.DirLeft) {
				if (tile.col <= (gridSize.col / 2)) {
					return true;
				}
			} else if (attackerDir == UnitAnimation._direction.DirRight) {
				if (tile.col >= (gridSize.col / 2)) {
					return true;
				}
			}
		} else {
			// In Assault mode spawn the unit at the end of the row
			if (attackerDir == UnitAnimation._direction.DirLeft) {
				if (tile.col >= (gridSize.col / 2)) {
					return true;
				}
			} else if (attackerDir == UnitAnimation._direction.DirRight) {
				if (tile.col <= (gridSize.col / 2)) {
					return true;
				}
			}
		}
		
		return false;
	}
	
	void winLevel() {

		// Wait until all rewards are gone
		if (GameObject.FindGameObjectWithTag ("reward") != null) {
			return;
		}

		ShardCurAmount = Player.totalShards;

		if (Player.levelComplete[Player.currentChapter][Player.currentLevel] == 0) {
			isFirstWin = true;
		}

		Player.completeLevel();
		isLevelDone = true;
		dialogueText.text = "";
		leftImage.sprite = null;
		rightImage.sprite = null;
		cvState = _ConvertState.PreWait;
		convertTime = Time.time;
	}
	
	void loseLevel() {
		loseMessage.SetActive(true);
		isLevelDone = true;
	}
	
	void handleSelectedUnitType() {
		UiTile tile;
		
		tile = map.GetComponent<UiTiles>().GetMouseOverTile();
		
		if ((tile.col < gridSize.col) && (tile.row < gridSize.row) && (tile.col >= 0) && (tile.row >= 0)) {
			UnitAnimation._direction dir;
			
			if (isPlayerAttacker == true) {
				totalAttackers++;
				dir = attackerDir;
			}
			else {
				totalDefenders++;
				dir = defenderDir;
			}
			
			for (int i = 0; i < unitsUIinst.Count; ++i) {
				if (selectedUiUnit == unitsUIinst[i]) {
					if (SpawnUnit(unitTypes[i].GetComponent<UiUnitType>().getRandomUnit(), (int)tile.row, (int)tile.col, GomObject.Faction.Player)) {
						tileContents[(int)tile.row][(int)tile.col].SendMessage("SetIdleDirection", dir, SendMessageOptions.DontRequireReceiver);
					}
					break;
				}
			}
		}
		for (int i = 0; i < unitsUIinst.Count; ++i) {
			if (selectedUiUnit == unitsUIinst[i]) {
				selectedUiUnit.transform.position = new Vector3((float)(-6 + (unitMenuInterval * i)), (float)-5.5, (float)0);
				break;
			}
		}
		
		selectedUnit = selectedUiUnit;
		selectedUiUnit = null;
	}
	
	void handleCollectOrbs() {
		
		switch(cvState) {
		case _ConvertState.PreWait:
			if ((convertTime + 2f) < Time.time) {
				cvState = _ConvertState.Converting;
				OrbCurAmount = 0;
				convertTime = Time.time;
			}
			break;
		case _ConvertState.Converting:
			if (ShardCurAmount > 0) {
				if ((convertTime + 0.15f) < Time.time) {
					OrbCurAmount++;
					ShardCurAmount -= 1 / Player.CONVERSION_RATE;
					convertTime = Time.time;
				}

				if (ShardCurAmount <= 0) {
					ShardCurAmount = 0;

					if ((isFirstWin == false) || (currentLevel.GetComponent<WaveList>().firstTimeBonus <= 0)) {
						Player.AddOrbs(OrbCurAmount);
						cvState = _ConvertState.Done;
					}
				}
			} else {
				if ((convertTime + 0.15f) < Time.time) {
					OrbCurAmount++;
					currentLevel.GetComponent<WaveList>().firstTimeBonus -= 1;
					convertTime = Time.time;
				}

				if (currentLevel.GetComponent<WaveList>().firstTimeBonus <= 0) {
					Player.AddOrbs(OrbCurAmount);
					cvState = _ConvertState.Done;
				}
			}

			break;
		case _ConvertState.Done:
			
			if (Input.GetMouseButtonDown(0)) {
				state = _WorldState.PostDialogue;
			}
			break;
		}
		
		dialogueText.text = "Total Spirit Shards\n";
		dialogueText.text += ShardCurAmount;
		dialogueText.text += "\n";

		if (isFirstWin == true) {
			dialogueText.text += "First Time Win Bonus\n";
			dialogueText.text += currentLevel.GetComponent<WaveList>().firstTimeBonus;
			dialogueText.text += "\n";
		}

		dialogueText.text += "Gained Spirit Orbs\n";
		dialogueText.text += OrbCurAmount;
	}
	
	void handleRowDamageAbility() {
		UiTile tile;
		Vector3 newPos;
		
		tile = map.GetComponent<UiTiles>().GetMouseOverTile();
		
		if ((tile.row != -1) && (tile.col != -1)) {
			newPos = new Vector3(map.transform.position.x,
			                     map.transform.position.y + ((float)tile.row * map.transform.localScale.y) + TileUnitOffset);
			selectedUiAbility.transform.position = newPos;
		}
		
		if (Input.GetMouseButtonUp(0)) {
			
			if ((tile.row >= 0) && (tile.row < gridSize.row) && (selectedAbility.GetComponent<Ability>().getUseCost() <= Player.spiritShards)) {
				float xTilePos;
				float yTilePos;
				
				Debug.Log(selectedAbility.GetComponent<Ability>().abilityName);
				
				Player.spiritShards -= selectedAbility.GetComponent<Ability>().getUseCost();
				
				xTilePos = map.GetComponent<UiTiles>().lanes[tile.row].GetComponent<UiRow>().rowTiles[0].transform.position.x;
				yTilePos = map.GetComponent<UiTiles>().lanes[tile.row].GetComponent<UiRow>().rowTiles[0].transform.position.y;
				
				GameObject abilityInst = Instantiate(selectedAbility, new Vector3(xTilePos, yTilePos + TileUnitOffset, 0), Quaternion.identity) as GameObject;
				
				abilityInst.SendMessage("SetDirection", UnitAnimation._direction.DirRight, SendMessageOptions.DontRequireReceiver);
				abilityInst.SendMessage("StartAnimation", null, SendMessageOptions.DontRequireReceiver);
				abilityInst.SendMessage("DieTimer", 2f, SendMessageOptions.DontRequireReceiver);
				
				for (int colInd = 0; colInd < gridSize.col; colInd++) {
					if (tileContents[tile.row][colInd] != null) {
						if (tileContents[tile.row][colInd].GetComponent<GomUnit>().faction == GomObject.Faction.Enemy) {
							tileContents[tile.row][colInd].SendMessage("SetAttackerNoArgs", null, SendMessageOptions.DontRequireReceiver);
							tileContents[tile.row][colInd].SendMessage("Damage", selectedAbility.GetComponent<Ability>().getDamage(), SendMessageOptions.DontRequireReceiver);
						}
					}
				}

				if (selectedAbility.GetComponent<Ability>().sound != null) {
					AudioSource.PlayClipAtPoint(selectedAbility.GetComponent<Ability>().sound, transform.position);
				}
			}
			
			for (int i = 0; i < abilityUIinst.Count; ++i) {
				if (selectedUiAbility == abilityUIinst[i]) {
					selectedUiAbility.transform.position = new Vector3((float)(-6 + (unitMenuInterval * (unitsUIinst.Count + i))), (float)-5.5, (float)0);
					break;
				}
			}
			
			selectedAbility = null;
			selectedUiAbility = null;
		}
	}
	
	void handleRadiusDamageAbility() {
		UiTile tile;
		Vector3 newPos;
		
		tile = map.GetComponent<UiTiles>().GetMouseOverTile();
		
		if ((tile.row != -1) && (tile.col != -1)) {
			newPos = new Vector3(map.transform.position.x + ((float)tile.col * map.transform.localScale.x),
			                     map.transform.position.y + ((float)tile.row * map.transform.localScale.y) + TileUnitOffset);
			selectedUiAbility.transform.position = newPos;
		}
		
		if (Input.GetMouseButtonUp(0)) {
			
			if ((tile.row >= 0) && (tile.row < gridSize.row) && (selectedAbility.GetComponent<Ability>().getUseCost() <= Player.spiritShards)) {
				float xTilePos;
				float yTilePos;
				
				Debug.Log(selectedAbility.GetComponent<Ability>().abilityName);
				
				Player.spiritShards -= selectedAbility.GetComponent<Ability>().getUseCost();
				
				xTilePos = map.GetComponent<UiTiles>().lanes[tile.row].GetComponent<UiRow>().rowTiles[0].transform.position.x;
				yTilePos = map.GetComponent<UiTiles>().lanes[tile.row].GetComponent<UiRow>().rowTiles[0].transform.position.y;
				
				GameObject abilityInst = Instantiate(selectedAbility, new Vector3(xTilePos, yTilePos + TileUnitOffset, 0), Quaternion.identity) as GameObject;
				
				abilityInst.SendMessage("SetDirection", UnitAnimation._direction.DirRight, SendMessageOptions.DontRequireReceiver);
				abilityInst.SendMessage("StartAnimation", null, SendMessageOptions.DontRequireReceiver);
				abilityInst.SendMessage("DieTimer", 2f, SendMessageOptions.DontRequireReceiver);
				
				// Handle the middle row
				for (int colInd = tile.col - selectedAbility.GetComponent<Ability>().getAreaOfEffect();
				     colInd < tile.col + selectedAbility.GetComponent<Ability>().getAreaOfEffect() + 1;
				     colInd++) {
					
					if (colInd < 0) {
						continue;
					}
					
					if (colInd >= gridSize.col) {
						continue;
					}

					if (tileContents[tile.row][colInd] != null) {
						if (tileContents[tile.row][colInd].GetComponent<GomUnit>().faction == GomObject.Faction.Enemy) {
							Debug.Log("Hit Unit : " + tile.row + ":" + colInd);
							tileContents[tile.row][colInd].SendMessage("SetAttackerNoArgs", null, SendMessageOptions.DontRequireReceiver);
							tileContents[tile.row][colInd].SendMessage("Damage", selectedAbility.GetComponent<Ability>().getDamage(), SendMessageOptions.DontRequireReceiver);
						}
					}
				}
				
				// Handle each row above the middle
				for (int rowInd = 1; rowInd <= selectedAbility.GetComponent<Ability>().getAreaOfEffect(); rowInd++) {
					int rowRange = selectedAbility.GetComponent<Ability>().getAreaOfEffect() - rowInd;
					
					if ((rowInd + tile.row) >= gridSize.row) {
						continue;
					}
					
					for (int colInd = tile.col - rowRange; colInd < tile.col + rowRange + 1; colInd++) {
						if ((colInd >= 0) && (colInd < gridSize.col)) {
							
							if (tileContents[tile.row + rowInd][colInd] != null) {
								if (tileContents[tile.row + rowInd][colInd].GetComponent<GomUnit>().faction == GomObject.Faction.Enemy) {
									Debug.Log("Hit Unit : " + (tile.row + rowInd) + ":" + colInd);
									tileContents[tile.row + rowInd][colInd].SendMessage("SetAttackerNoArgs", null, SendMessageOptions.DontRequireReceiver);
									tileContents[tile.row + rowInd][colInd].SendMessage("Damage", selectedAbility.GetComponent<Ability>().getDamage(), SendMessageOptions.DontRequireReceiver);
								}
							}
						}
					}
				}
				
				// Handle each row below the middle
				for (int rowInd = 1; rowInd <= selectedAbility.GetComponent<Ability>().getAreaOfEffect(); rowInd++) {
					int rowRange = selectedAbility.GetComponent<Ability>().getAreaOfEffect() - rowInd;
					
					if ((tile.row - rowInd) < 0) {
						continue;
					}
					
					for (int colInd = tile.col - rowRange; colInd < tile.col + rowRange + 1; colInd++) {
						if ((colInd >= 0) && (colInd < gridSize.col)) {
							if (tileContents[tile.row - rowInd][colInd] != null) {
								if (tileContents[tile.row - rowInd][colInd].GetComponent<GomUnit>().faction == GomObject.Faction.Enemy) {
									Debug.Log("Hit Unit : " + (tile.row - rowInd) + ":" + colInd);
									tileContents[tile.row - rowInd][colInd].SendMessage("SetAttackerNoArgs", null, SendMessageOptions.DontRequireReceiver);
									tileContents[tile.row - rowInd][colInd].SendMessage("Damage", selectedAbility.GetComponent<Ability>().getDamage(), SendMessageOptions.DontRequireReceiver);
								}
							}
						}
					}
				}
				
				if (selectedAbility.GetComponent<Ability>().sound != null) {
					AudioSource.PlayClipAtPoint(selectedAbility.GetComponent<Ability>().sound, transform.position);
				}
			}
			
			for (int i = 0; i < abilityUIinst.Count; ++i) {
				if (selectedUiAbility == abilityUIinst[i]) {
					selectedUiAbility.transform.position = new Vector3((float)(-6 + (unitMenuInterval * (unitsUIinst.Count + i))), (float)-5.5, (float)0);
					break;
				}
			}
			
			selectedAbility = null;
			selectedUiAbility = null;
		}
	}
	
	void handleFreezeEnemyUnitAbility() {
		UiTile tile;
		Vector3 newPos;
		
		tile = map.GetComponent<UiTiles>().GetMouseOverTile();
		
		if ((tile.row != -1) && (tile.col != -1)) {
			newPos = new Vector3(map.transform.position.x + ((float)tile.col * map.transform.localScale.x),
			                     map.transform.position.y + ((float)tile.row * map.transform.localScale.y) + TileUnitOffset);
			selectedUiAbility.transform.position = newPos;
			
			if (freezeIcons == null) {
				freezeIcons = new List<GameObject>();
				for (int rowInd = 0; rowInd < gridSize.row; rowInd++) {
					for (int colInd = 0; colInd < gridSize.col; colInd++) {
						freezeIcons.Add(Instantiate(selectedAbility.GetComponent<Ability>().sprite,
						                            new Vector3(map.GetComponent<UiTiles>().lanes[rowInd].GetComponent<UiRow>().rowTiles[colInd].transform.position.x,
						            map.GetComponent<UiTiles>().lanes[rowInd].GetComponent<UiRow>().rowTiles[colInd].transform.position.y + TileUnitOffset),
						                            Quaternion.identity) as GameObject);
					}
				}
			}
		} else {
			if (freezeIcons != null) {
				foreach(GameObject ic in freezeIcons) {
					Destroy(ic);
				}
				freezeIcons = null;
			}
		}
		
		if (Input.GetMouseButtonUp (0)) {

			if ((tile.row != -1) && (tile.col != -1)) {
				if (selectedAbility.GetComponent<Ability>().getUseCost() <= Player.spiritShards) {
					freezeEndTime = Time.time + selectedAbility.GetComponent<Ability>().duration;
					Player.spiritShards -= selectedAbility.GetComponent<Ability>().getUseCost();
					
					if (selectedAbility.GetComponent<Ability>().sound != null) {
						AudioSource.PlayClipAtPoint(selectedAbility.GetComponent<Ability>().sound, transform.position);
					}
				}
			}
			
			if (freezeIcons != null) {
				foreach(GameObject ic in freezeIcons) {
					Destroy(ic);
				}
				freezeIcons = null;
			}
			
			for (int i = 0; i < abilityUIinst.Count; ++i) {
				if (selectedUiAbility == abilityUIinst[i]) {
					selectedUiAbility.transform.position = new Vector3((float)(-6 + (unitMenuInterval * (unitsUIinst.Count + i))), (float)-5.5, (float)0);
					break;
				}
			}
			
			selectedAbility = null;
			selectedUiAbility = null;
		}
	}
	
	void handleShieldUnitAbility() {
		UiTile tile;
		Vector3 newPos;
		
		tile = map.GetComponent<UiTiles>().GetMouseOverTile();
		
		if ((tile.row != -1) && (tile.col != -1)) {
			newPos = new Vector3(map.transform.position.x + ((float)tile.col * map.transform.localScale.x),
			                     map.transform.position.y + ((float)tile.row * map.transform.localScale.y) + TileUnitOffset);
			selectedUiAbility.transform.position = newPos;
		}
		
		if (Input.GetMouseButtonUp (0)) {

			if ((tile.row != -1) && (tile.col != -1)) {
				if (tileContents[tile.row][tile.col] != null) {
					if (tileContents[tile.row][tile.col].GetComponent<GomUnit>().faction == GomObject.Faction.Player) {
						if (selectedAbility.GetComponent<Ability>().getUseCost() <= Player.spiritShards) {
							tileContents[tile.row][tile.col].SendMessage("SetInvincible", selectedAbility.GetComponent<Ability>().duration, SendMessageOptions.DontRequireReceiver);
							Player.spiritShards -= selectedAbility.GetComponent<Ability>().getUseCost();
							
							if (selectedAbility.GetComponent<Ability>().sound != null) {
								AudioSource.PlayClipAtPoint(selectedAbility.GetComponent<Ability>().sound, transform.position);
							}
						}
					}
				}
			}
			
			for (int i = 0; i < abilityUIinst.Count; ++i) {
				if (selectedUiAbility == abilityUIinst[i]) {
					selectedUiAbility.transform.position = new Vector3((float)(-6 + (unitMenuInterval * (unitsUIinst.Count + i))), (float)-5.5, (float)0);
					break;
				}
			}
			
			selectedAbility = null;
			selectedUiAbility = null;
		}
	}
	
	public void UnitAI(int row, int col) {
		// If the tile is not empty then see if the unit needs to do something
		if (tileContents[row][col] != null) {
			if (tileContents[row][col].GetComponent<GomUnit>() == null) {
				return;
			}

			if (tileContents[row][col].GetComponent<GomUnit>().health <= 0) {
				// Unit is defeated
				if (isPlayerAttacker == true) {
					if (tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Player) {
						totalAttackers--;
						defeatedAttackers++;
					}
					else {
						totalDefenders--;
					}
				}
				else {
					if (tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Player) {
						totalDefenders--;
					}
					else {
						totalAttackers--;
						defeatedAttackers++;
					}
				}

				// Non-wall Enemies drop shards to pick up
				if ((tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Enemy) &&
				    (tileContents[row][col] != wall)) {
					GameObject rw = Instantiate(rewardShardPrefab, tileContents[row][col].transform.position, Quaternion.identity) as GameObject;
					rw.transform.name = "reward" + rwCounter;
					rw.tag = "reward";
					rwCounter++;
					rw.transform.position = new Vector3(rw.transform.position.x, rw.transform.position.y, rw.transform.position.z - 1);
					rw.GetComponent<RewardSpiritShard>().world = gameObject;
					rw.GetComponent<RewardSpiritShard>().shardAmount = tileContents[row][col].GetComponent<GomUnit>().value;
					rw.GetComponent<RewardSpiritShard>().travelPlace = spiritShardUI;
				}
				
				// Show the player to click on the spirit shard
				if ((Player.tutorialState == 2) && (inTutorial == false) && (tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Enemy)) {
					cursorInst = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
					cursorInst.transform.position = tileContents[row][col].transform.position;
					cursorInst.GetComponentInChildren<Animator>().SetTrigger("DoTut2");
					tutorialTime = Time.time + 4;
					inTutorial = true;
				}

				if (UnitDieClip != null) {
					AudioSource.PlayClipAtPoint(UnitDieClip, transform.position);
				}

				tileContents[row][col].SendMessage("Die", null, SendMessageOptions.DontRequireReceiver);
				tileContents[row][col] = null;
			}
			else if (tileContents[row][col].GetComponent<GomUnit>().CanMove()) {
				if (CanUnitAttackLeftRight(row, col) == true) {
					// Unit can attack
					AttackLeftRightNearestEnemy(row, col);
				} else if (freezeEndTime < Time.time) {
					// Unit can move
					if (((tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Player) &&
					     currentLevel.GetComponent<WaveList>().isPlayerAttacker) ||
					    ((tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Enemy) &&
					 !currentLevel.GetComponent<WaveList>().isPlayerAttacker)) {
						
						if (curLevelAttackerDir == WaveList._direction.Right) {
							if ((col + 1) >= gridSize.col) {
								// Unit passed off the screen
								Destroy(tileContents[row][col]);
								tileContents[row][col] = null;
								letThroughAttackers++;

								if (AttCrossClip != null) {
									AudioSource.PlayClipAtPoint(AttCrossClip, transform.position);
								}
							}
							else {
								if (tileContents[row][col + 1] == null) {
									// Advance attacker units to the right
									tileContents[row][col].SendMessage("Move", new Vector2(col + 1, row), SendMessageOptions.DontRequireReceiver);
									tileContents[row][col + 1] = tileContents[row][col];
									tileContents[row][col] = null;
								}
								else {
									tileContents[row][col].SendMessage("SetIdleDirection", tileContents[row][col].GetComponent<GomUnit>().idleDir, SendMessageOptions.DontRequireReceiver);
								}
							}
						}
						else if (curLevelAttackerDir == WaveList._direction.Left) {
							if (col == 0) {
								// Unit passed off the screen
								Destroy(tileContents[row][col]);
								tileContents[row][col] = null;
								letThroughAttackers++;
								
								if (AttCrossClip != null) {
									AudioSource.PlayClipAtPoint(AttCrossClip, transform.position);
								}
							}
							else {
								if (tileContents[row][col - 1] == null) {
									// Advance attacker units to the right
									tileContents[row][col].SendMessage("Move", new Vector2(col - 1, row), SendMessageOptions.DontRequireReceiver);
									tileContents[row][col - 1] = tileContents[row][col];
									tileContents[row][col] = null;
								}
								else {
									tileContents[row][col].SendMessage("SetIdleDirection", tileContents[row][col].GetComponent<GomUnit>().idleDir, SendMessageOptions.DontRequireReceiver);
								}
							}
						}
					}
				}
			}/*
			// update unit stats
			if (tileContents[row][col]) {

				PropertyStats playerStats = tileContents[row][col].GetComponent<GomUnit>().playerStats;
				PropertyStats enemyStats = tileContents[row][col].GetComponent<GomUnit>().enemyStats;
				PropertyStats stats = (tileContents[row][col].GetComponent<GomUnit>().faction == GomObject.Faction.Player) ? playerStats : enemyStats;

				for (int i = 0; i < unitTypes.Count; i++) {
					if (tileContents[row][col].GetComponent<GomUnit>().unitType.StartsWith(unitTypes[i].GetComponent<UiUnitType>().UnitName)){
						for (int j = 0; j < unitTypes[i].GetComponent<UiUnitType>().Units.Length; j++){
							stats.updateUnitStats(unitTypes[i].GetComponent<UiUnitType>().Units[j]);
						}
					}
				}
			}*/
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
				if (tileContents[row][i].GetComponent<GomUnit>() == null) {
					break;
				}

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
				if (tileContents[row][i].GetComponent<GomUnit>() == null) {
					break;
				}

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
				if (tileContents[row][i].GetComponent<GomUnit>() == null) {
					break;
				}

				if (tileContents[row][i].GetComponent<GomUnit>().faction != attacker.faction) {
					GameObject projectile;
					tileContents[row][i].SendMessage ("SetAttacker", attacker, SendMessageOptions.DontRequireReceiver);
					attacker.SendMessage("Attack", null, SendMessageOptions.DontRequireReceiver);
					
					if ((attacker.weapon != null) && (attacker.weapon.projectile != null)) {
						projectile = Instantiate(attacker.weapon.projectile, attacker.transform.position, Quaternion.identity) as GameObject;
						projectile.SendMessage("SetTarget", tileContents[row][i], SendMessageOptions.DontRequireReceiver);
					} else {
						tileContents[row][i].SendMessage("DamageMelee", attacker.getStats(), SendMessageOptions.DontRequireReceiver);
					}

					if (attacker.weapon.sound != null) {
						AudioSource.PlayClipAtPoint(attacker.weapon.sound, transform.position);
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
						tileContents[row][i].SendMessage("DamageMelee", attacker.getStats(), SendMessageOptions.DontRequireReceiver);
					}
					
					if (attacker.weapon.sound != null) {
						AudioSource.PlayClipAtPoint(attacker.weapon.sound, transform.position);
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
			Debug.Log ("another unit is already occupying this tile!");
			return false;
		}
		
		if (unitPrefab == null) {
			Debug.Log("Prefab is null");
			return false;
		}
		
		if (currentLevel.GetComponent<WaveList> ().laneEnable [tileRow] == false) {
			Debug.Log("Unit attempted to spawn on a disabled lane");
			return false;
		}
		
		// Charge player for the unit
		if (faction == GomObject.Faction.Player) {
			if (unitPrefab.GetComponent<GomUnit>().cost > Player.spiritShards) {
				Debug.Log ("don't have enough Spirit Shards!");
				return false;
			}
			else {
				Player.spiritShards -= unitPrefab.GetComponent<GomUnit>().cost;
				Debug.Log (Player.spiritShards + " shards left.");
			}
		}
		
		xTilePos = map.GetComponent<UiTiles>().lanes[tileRow].GetComponent<UiRow>().rowTiles[tileCol].transform.position.x;
		yTilePos = map.GetComponent<UiTiles>().lanes[tileRow].GetComponent<UiRow>().rowTiles[tileCol].transform.position.y;
		
		worldPos.x = xTilePos;
		worldPos.y = yTilePos + TileUnitOffset;
		worldPos.z = 0;
		
		tileContents[tileRow][tileCol] = Instantiate(unitPrefab, worldPos, Quaternion.identity) as GameObject;
		tileContents[tileRow][tileCol].GetComponent<GomUnit>().enabled = true;
		tileContents[tileRow][tileCol].SendMessage("SetCurrentTile", new Vector2(tileCol, tileRow), SendMessageOptions.DontRequireReceiver);
		tileContents[tileRow][tileCol].SendMessage("SetFaction", faction, SendMessageOptions.DontRequireReceiver);
		tileContents[tileRow][tileCol].SendMessage("SetWorld", gameObject, SendMessageOptions.DontRequireReceiver);
		tileContents[tileRow][tileCol].SendMessage("SetMoveXScale", map.transform.localScale.x, SendMessageOptions.DontRequireReceiver);
		return true;
	}
	
	void buttonPush(string buttonName) {
		switch(buttonName) {
		case "Upgrade":
			int unitType = -1;
			
			if (selectedUnit == null) {
				break;
			}
			
			for (int i = 0; i < unitsUIinst.Count; i++) {
				if (selectedUnit.GetComponent<GomUnit>().name == unitsUIinst[i].GetComponent<GomUnit>().name) {
					unitType = i;
				}
			}
			
			if (unitType == -1) {
				break;
			}
			
			PropertyStats unitStats = unitTypes[unitType].GetComponent<UiUnitType>().getPlayerStats();
			
			if ((Player.spiritShards >= unitStats.upgradeCost) &&
			    (unitStats.level < unitStats.maxLevel)){
				Player.spiritShards -= unitStats.upgradeCost;
				unitStats.upgradeUnit(unitTypes[unitType].GetComponent<UiUnitType>().UnitName);
				Debug.Log("Upgraded " + unitTypes[unitType].GetComponent<UiUnitType>().UnitName);
				Debug.Log("Shards left " + Player.spiritShards);
			}
			break;
		case "Release":
			// Uncomment out to make the Release button automatically win the level
			//winLevel();
			//winMessage.SetActive(true);
			//return;
			if ((CurWave) < currentLevel.GetComponent<WaveList>().waves.Count) {
				float nextWaveTime = currentLevel.GetComponent<WaveList>().waves[CurWave].waitTime;
				
				levelStartTime -= nextWaveTime - (Time.time - levelStartTime);
				Player.spiritShards += earlyReleaseShards;
				ReleaseButton.SetActive(false);
			}
			break;
		}
	}
}
