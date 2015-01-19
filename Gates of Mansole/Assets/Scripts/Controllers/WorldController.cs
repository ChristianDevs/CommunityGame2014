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
    private GameObject selectedUiUnit;
    private GameObject selectedUiAbility;
    private GameObject selectedAbility;
    private UiTile gridSize;

    public List<GameObject> squares;
	public List<GameObject> unitsUIinst;
	public List<GameObject> unitTypes;
    public List<GameObject> abilityUIinst;
    public List<GameObject> abilities;
	private float levelStartTime;
	public GameObject unitInfoUi;
    public GameObject[] maps;
    private int dialogueIndex;
    public TextMesh dialogueText;
    public SpriteRenderer leftImage;
    public SpriteRenderer rightImage;
    public GameObject dialogueWindow;
    public int dialogueLineSize;
    public Sprite[] Images;
    public float unitMenuInterval;

	private float OrbCurAmount;
	private float ShardCurAmount;
	private float ConvertStepIntervalTime;
	private float convertTime;

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

        winMessage.SetActive(false);
        loseMessage.SetActive(false);

		levelStartTime = 0;
        state = _WorldState.Setup;

        foreach (GameObject mp in maps) {
            mp.SetActive(false);
        }

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

        switch (state) {
            case _WorldState.Setup:
                handleSetup();
                state = _WorldState.PreDialogue;
                dialogueIndex = -1;
				dialogueWindow.SetActive(true);
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
                    Application.LoadLevel("LevelSelect");
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
	}

    void handleSetup() {
        if (Player.nextLevelFile != "") {
            currentLevel.GetComponent<WaveList>().loadGameFile(Player.nextLevelFile, unitTypes);

            foreach (GameObject unitType in unitTypes) {
                UiUnitType uType;
                uType = unitType.GetComponent<UiUnitType>();

                if (uType != null) {
                    uType.getPlayerStats().resetUnitStats();
					uType.getEnemyStats().resetUnitStats();
                }
            }

            Player.spiritShards = Player.totalShards = currentLevel.GetComponent<WaveList>().startShards;

            totalAI = GetTotalWaveUnits(currentLevel);

            isPlayerAttacker = currentLevel.GetComponent<WaveList>().isPlayerAttacker;
            curLevelAttackerDir = currentLevel.GetComponent<WaveList>().attackerDir;

            foreach (GameObject mp in maps) {
				if (mp.GetComponent<UiTiles>().mapName.TrimEnd() == currentLevel.GetComponent<WaveList>().map) {
                    mp.SetActive(true);
                    map = mp;
                }
            }

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

            if (curLevelAttackerDir == WaveList._direction.Left) {
                attackerDir = UnitAnimation._direction.DirLeft;
                defenderDir = UnitAnimation._direction.DirRight;
            }
            else if (curLevelAttackerDir == WaveList._direction.Right) {
                attackerDir = UnitAnimation._direction.DirRight;
                defenderDir = UnitAnimation._direction.DirLeft;
            }
        }
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
            dialogueIndex++;

            if (dialogueIndex < dialogue.Count) {
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

        newText += speaker;
        newText += ":\n\n";

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

            // Go through each wave and see if it is time to start that wave
            foreach (Wave wv in wl.waves) {
                if (wv.waitTime + levelStartTime < Time.time) {

                    // Go through each unit and see if it is time to spawn it
                    foreach (WaveUnit ut in wv.units) {
                        if (ut.created == false) {
                            if ((wv.waitTime + ut.time + levelStartTime) < Time.time) {
                                int row;
                                int col;

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
                                        if (SpawnUnit(ut.prefab, row, col, GomObject.Faction.Enemy)) {
                                            tileContents[row][col].SendMessage("SetIdleDirection", defenderDir, SendMessageOptions.DontRequireReceiver);
                                            totalDefenders++;
                                        }
                                    }
									else {
                                        if (SpawnUnit(ut.prefab, row, col, GomObject.Faction.Enemy)) {
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
                            if (Player.spiritShards >= ab.GetComponent<Ability>().cost) {
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

            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hitSquare;

                // Handle the level being done
                if (isLevelDone == true) {
                    if (winMessage.activeSelf == true) {
                        state = _WorldState.CollectOrbs;
                        if (currentLevel.GetComponent<WaveList>().postLevelDialogue.Count > 0) {
                            dialogueWindow.SetActive(true);
                        }
                    } else {
                        Application.LoadLevel("LevelSelect");
                    }
                }

                // Check Menu Squares
                if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitSquare)) {
                    foreach (GameObject square in squares) {
                        // User selected a menu, find out what it is
                        if (hitSquare.transform.name == square.transform.name) {
                            // Check if user selected a unit type
                            for (int i = 0; i < unitTypes.Count; ++i) {
                                if (hitSquare.transform.name == unitTypes[i].GetComponent<UiUnitType>().UnitName) {
                                    selectedUiUnit = unitsUIinst[i];
                                    break;
                                }
                            }

                            // Check if user selected an ability
                            for (int i = 0; i < abilities.Count; ++i) {
                                if (hitSquare.transform.name == abilities[i].GetComponent<Ability>().abilityName) {
                                    selectedUiAbility = abilityUIinst[i];
                                    selectedAbility = abilities[i];
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

                if ((tile.row != -1) && (tile.col != -1)) {
                    newPos = new Vector3(map.transform.position.x + ((float)tile.col * map.transform.localScale.x),
                                         map.transform.position.y + ((float)tile.row * map.transform.localScale.y) + TileUnitOffset);
                    selectedUiUnit.transform.position = newPos;
                }

                if (Input.GetMouseButtonUp(0)) {
                    handleSelectedUnitType();
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

                        foreach (GameObject unit in unitTypes) {
                            Debug.Log(tileContents[(int)tile.row][(int)tile.col].GetComponent<GomUnit>().unitType + "::" + unit.GetComponent<UiUnitType>().UnitName);
                            if (tileContents[(int)tile.row][(int)tile.col].GetComponent<GomUnit>().unitType == unit.GetComponent<UiUnitType>().UnitName) {
                                ut = unit.GetComponent<UiUnitType>().getRandomUnit();
                                break;
                            }
                        }

                        if (ut != null) {
                            unitInfoUi.SendMessage("SelectUnit", ut, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
        }
    }

	void winLevel() {
		winMessage.SetActive(true);
		ShardCurAmount = Player.totalShards;
		Player.completeLevel(Player.currentLevel);
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
			if ((convertTime + 0.3f) < Time.time) {
				OrbCurAmount++;
				ShardCurAmount -= 1 / Player.CONVERSION_RATE;
			}

			if (ShardCurAmount <= 0) {
				ShardCurAmount = 0;
				cvState = _ConvertState.Done;
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

            if ((tile.row >= 0) && (tile.row < gridSize.row) && (selectedAbility.GetComponent<Ability>().cost <= Player.spiritShards)) {
                float xTilePos;
                float yTilePos;

                Debug.Log(selectedAbility.GetComponent<Ability>().abilityName);

                Player.spiritShards -= selectedAbility.GetComponent<Ability>().cost;

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
                            tileContents[tile.row][colInd].SendMessage("Damage", selectedAbility.GetComponent<Ability>().damage, SendMessageOptions.DontRequireReceiver);
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

    void handleRadiusDamageAbility() {
        Debug.Log("Raduis Damage");
        selectedAbility = null;
        selectedUiAbility = null;
    }

    void handleFreezeEnemyUnitAbility() {
        Debug.Log("Freeze Enemy Unit");
        selectedAbility = null;
        selectedUiAbility = null;
    }

    void handleShieldUnitAbility() {
        Debug.Log("Shield Unit");
        selectedAbility = null;
        selectedUiAbility = null;
    }

    public void UnitAI(int row, int col) {
        // If the tile is not empty then see if the unit needs to do something
        if (tileContents[row][col] != null) {
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

                tileContents[row][col].SendMessage("Die", null, SendMessageOptions.DontRequireReceiver);
                tileContents[row][col] = null;
            }
            else if (tileContents[row][col].GetComponent<GomUnit>().CanMove()) {
                if (CanUnitAttackLeftRight(row, col) == true) {
                    // Unit can attack
                    AttackLeftRightNearestEnemy(row, col);
                }
                else {
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
						tileContents[row][i].SendMessage("DamageMelee", attacker.getStats(), SendMessageOptions.DontRequireReceiver);
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
					unitStats.upgradeUnit(unitTypes[unitType].GetComponent<UiUnitType>().UnitName);
				    //Debug.Log("upgraded " + unitTypes[unitType].GetComponent<UiUnitType>().UnitName);
					//Debug.Log("shards left " + Player.spiritShards);
               	}
				break;
        }
    }
}
