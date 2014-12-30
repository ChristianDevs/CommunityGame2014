using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveList : MonoBehaviour {

    public char[] seps = { ':' };

    public enum _direction {
        Up,
        Down,
        Left,
        Right
    }

    public class _statement {
        public string Speaker;
        public string LeftImage;
        public string RightImage;
        public string dialogue;
    };

    public bool started = false;  // When true, waves begin
    public int AttackersLetThrough;
    public bool isPlayerAttacker = false;
    public _direction attackerDir = _direction.Right;
    public Wave[] waves;
	public UiUnitType[] enemyUnitTypes;
    public string map;
    public int startShards;
    public List<_statement> preLevelDialogue;
    public List<_statement> postLevelDialogue;

    private List<GameObject> unitTypes;

    public bool loadGameFile(string fileName, List<GameObject> newUnitTypes) {
        Debug.Log("Loading Game File: " + fileName);
        unitTypes = newUnitTypes;
        return parseLevelFile(fileName);
    }

    bool parseLevelFile(string fileName) {
        string[] levelData;

        levelData = System.IO.File.ReadAllLines(fileName);

        if (levelData.Length < 1) {
            Debug.Log("Level Parser: Game File Empty");
            return false;
        }

        for (int i = 0; i < levelData.Length; i++) {
            string key = "";
            string val = "";

            key = levelData[i].Split(seps)[0].Trim().ToLower();

            if (levelData[i].Split(seps).Length > 1) {
                val = levelData[i].Split(seps)[1];
            }

            switch (key) {
                case "startshards":
                    startShards = int.Parse(val);
                    break;
                case "attackersletthrough":
                    AttackersLetThrough = int.Parse(val);
                    break;
                case "isplayerattacker":
                    isPlayerAttacker = bool.Parse(val);
                    break;
                case "attackerdir":
                    if (val.ToLower() == "up") {
                        attackerDir = _direction.Up;
                    } else if (val.ToLower() == "left") {
                        attackerDir = _direction.Left;
                    } else if (val.ToLower() == "down") {
                        attackerDir = _direction.Down;
                    } else if (val.ToLower() == "right") {
                        attackerDir = _direction.Right;
                    } else {
                        return false;
                    }
                    break;
                case "map":
                    map = val.ToLower();
                    break;
                case "wave":
                    i = parseAllWaves(levelData, i + 1);
                    break;
                case "predialogue":
                    preLevelDialogue = new List<_statement>();
                    i = parseDialogue(levelData, preLevelDialogue, i + 1);
                    break;
                case "postdialogue":
                    postLevelDialogue = new List<_statement>();
                    i = parseDialogue(levelData, postLevelDialogue, i + 1);
                    break;
                default:
                    return false;
            }
        }

        return true;
    }

    int parseDialogue(string[] levelData, List<_statement> dialogue, int index) {
        bool isDone = false;
        int i = 0;
        _statement stmt;

        Debug.Log("Parsing Dialogue");

        stmt = new _statement();

        for (i = index + 1; i < levelData.Length; i++) {
            string key = "";
            string val = "";

            key = levelData[i].Split(seps)[0].Trim().ToLower();

            if (levelData[i].Split(seps).Length > 1) {
                val = levelData[i].Split(seps)[1];
            }

            switch (key) {
                case "speaker":
                    stmt.Speaker = val;
                    break;
                case "leftimage":
                    stmt.LeftImage = val;
                    break;
                case "rightimage":
                    stmt.RightImage = val;
                    break;
                case "dialogue":
                    stmt.dialogue = val;
                    break;
                case "statement":
                    dialogue.Add(stmt);
                    stmt = new _statement();
                    break;
                default:
                    dialogue.Add(stmt);
                    isDone = true;
                    break;
            }

            if (isDone) {
                break;
            }
        }

        if (i >= levelData.Length) {
            dialogue.Add(stmt);
        }

        return i - 1;
    }

    int parseAllWaves(string[] levelData, int index) {
        List<Wave> fileWaves;
        int i = index;

        fileWaves = new List<Wave>();

        do {
            i = parseWave(levelData, i, fileWaves);

            if ((i >= levelData.Length) || (levelData[i].Split(seps)[0].Trim().ToLower() != "wave")) {
                break;
            }

            i++;
        } while (i < levelData.Length);

        waves = fileWaves.ToArray();

        return i - 1;
    }

    int parseWave(string[] levelData, int index, List<Wave> fileWaves) {
        Wave wv;
        List<WaveUnit> fileUnits;
        bool isDone = false;
        int i;

        wv = new Wave();
        fileUnits = new List<WaveUnit>();

        for (i = index; i < levelData.Length; i++) {
            string key = "";
            string val = "";

            key = levelData[i].Split(seps)[0].Trim().ToLower();

            if (levelData[i].Split(seps).Length > 1) {
                val = levelData[i].Split(seps)[1];
            }

            switch (key) {
                case "waittime":
                    wv.waitTime = int.Parse(val);
                    break;
                case "waveunit":
                    i = parseWaveUnit(levelData, i + 1, fileUnits);
                    i--;
                    break;
                default:
                    isDone = true;
                    break;
            }

            if (isDone) {
                break;
            }
        }

        wv.units = fileUnits.ToArray();
        fileWaves.Add(wv);

        return i;
    }

    int parseWaveUnit(string[] levelData, int index, List<WaveUnit> fileUnits) {
        WaveUnit fileUnit;
        fileUnit = new WaveUnit();
        int i;
        bool isDone = false;

        for (i = index; i < levelData.Length; i++) {
            string key = "";
            string val = "";

            key = levelData[i].Split(seps)[0].Trim().ToLower();

            if (levelData[i].Split(seps).Length > 1) {
                val = levelData[i].Split(seps)[1];
            }

            switch (key) {
                case "time":
                    fileUnit.time = float.Parse(val);
                    break;
                case "unit":
                    foreach (GameObject ut in unitTypes) {
                        if (val == ut.GetComponent<UiUnitType>().UnitName) {
                            fileUnit.prefab = ut.GetComponent<UiUnitType>().getRandomUnit();
                        }
                    }
                    break;
                case "spawnloc":
                    if (val.ToLower() == "randrow") {
                        fileUnit.SpawnLocType = WaveUnit._spawnLocType.RandRow;
                    } else if (val.ToLower() == "randtile") {
                        fileUnit.SpawnLocType = WaveUnit._spawnLocType.RandTile;
                    } else if (val.ToLower() == "specifiedtile") {
                        fileUnit.SpawnLocType = WaveUnit._spawnLocType.SpecifiedTile;
                    } else if (val.ToLower() == "specifiedrow") {
                        fileUnit.SpawnLocType = WaveUnit._spawnLocType.SpecifiedRow;
                    } else {
                        fileUnit.SpawnLocType = WaveUnit._spawnLocType.RandRow;
                    }

                    break;
                case "tile":
                    string val2 = "";

                    if (levelData[i].Split(seps).Length > 2) {
                        val2 = levelData[i].Split(seps)[2];
                    }

                    fileUnit.Tile.x = float.Parse(val);
                    fileUnit.Tile.y = float.Parse(val2);
                    break;
                default:
                    isDone = true;
                    break;
            }

            if (isDone) {
                break;
            }
        }

        fileUnits.Add(fileUnit);

        return i;
    }
}
