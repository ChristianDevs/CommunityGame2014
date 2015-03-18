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
	
	public class _placeable {
		public int num;
		public float x;
		public float y;
	}
	
	public bool started = false;  // When true, waves begin
	public int AttackersLetThrough;
	public bool isPlayerAttacker = false;
	public _direction attackerDir = _direction.Right;
	public List<Wave> waves;
	public List<bool> waveStarted;
	public UiUnitType[] enemyUnitTypes;
	public int startShards;
	public List<_statement> preLevelDialogue;
	public List<_statement> postLevelDialogue;
	public List<int> Lanes;
	public List<bool> laneEnable;
	public List<float> enemyLevelUpTimes;
	public List<_placeable> placeables;
	public List<int> upgradeAtWave;
	public int firstTimeBonus;
	
	// Wall:Col:StartRow:EndRow:Health:FallPercent:HealthRegen
	public bool usingWall;
	public int wallCol;
	public int wallTopRow;
	public int wallBotRow;
	public int wallHealth;
	public float wallFallPercent;
	public int wallHealthRegen;
	
	public List<GameObject> unitTypes;

	void Start() {
		unitTypes = null;
	}
	
	public bool loadGameFile(string fileName, List<GameObject> newUnitTypes) {
		Debug.Log("Loading Game File: " + fileName);
		unitTypes = newUnitTypes;
		Lanes = new List<int>();
		placeables = new List<_placeable> ();
		upgradeAtWave = new List<int> ();
		waveStarted = new List<bool> ();
		waves = new List<Wave>();

		StartCoroutine (parseLevelFile(fileName));
		return true;
	}
	
	public IEnumerator parseLevelFile(string fileName) {
		string[] levelData;
		string[] LineSeps = {"\n"};
		
		if (fileName.Contains("://"))
		{
			WWW www = new WWW (fileName);
			yield return www;
			levelData = www.text.Split(LineSeps, System.StringSplitOptions.RemoveEmptyEntries);
		} else {
			levelData = System.IO.File.ReadAllLines(fileName);
		}

		for (int i = 0; i < levelData.Length; i++) {
			string key = "";
			string val = "";
			
			if (levelData[i].Split(seps, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
				key = levelData[i].Split(seps)[0].TrimStart().ToLower();
			} else {
				key = levelData[i].TrimStart().TrimEnd().ToLower();
			}
			
			if (levelData[i].Split(seps).Length > 1) {
				val = levelData[i].Split(seps)[1].TrimEnd();
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
				}
				break;
			case "wave":
				i = parseAllWaves(levelData, i + 1);
				break;
			case "wall":
				if (levelData[i].Split(seps).Length > 6) {
					// Wall:Col:StartRow:EndRow:Health:FallPercent:HealthRegen
					usingWall = true;
					wallCol = int.Parse(levelData[i].Split(seps)[1].ToLower());
					wallBotRow = int.Parse(levelData[i].Split(seps)[2].ToLower());
					wallTopRow = int.Parse(levelData[i].Split(seps)[3].ToLower());
					wallHealth = int.Parse(levelData[i].Split(seps)[4].ToLower());
					wallFallPercent = float.Parse(levelData[i].Split(seps)[5].ToLower());
					wallHealthRegen = int.Parse(levelData[i].Split(seps)[6].TrimEnd ().ToLower());
				}
				break;
			case "predialogue":
				preLevelDialogue = new List<_statement>();
				i = parseDialogue(levelData, preLevelDialogue, i + 1);
				break;
			case "postdialogue":
				postLevelDialogue = new List<_statement>();
				i = parseDialogue(levelData, postLevelDialogue, i + 1);
				break;
			case "lane":
				Lanes.Add(int.Parse(val));
				if (levelData[i].Split(seps).Length > 2) {
					if (levelData[i].Split(seps)[2].TrimStart().TrimEnd().ToLower() == "disable") {
						laneEnable.Add(false);
					} else {
						laneEnable.Add(true);
					}
				} else {
					laneEnable.Add(true);
				}
				break;
			case "placeable":
				if (levelData[i].Split(seps).Length > 3) {
					_placeable pl = new _placeable();
					
					pl.num = int.Parse(val);
					pl.x = float.Parse(levelData[i].Split(seps)[2]);
					pl.y = float.Parse(levelData[i].Split(seps)[3].TrimEnd());
					
					placeables.Add(pl);
				}
				break;
			case "upgradeatwave":
				upgradeAtWave.Add(int.Parse(val));
				break;
			case "firsttimebonus":
				firstTimeBonus = int.Parse(val);
				break;
			default:
				break;
			}
		}
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
				val = levelData[i].Split(seps)[1].TrimStart().TrimEnd();
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
		int i = index;
		
		do {
			waveStarted.Add(false);
			i = parseWave(levelData, i, waves);
			
			if ((i >= levelData.Length) || (levelData[i].Split(seps)[0].Trim().ToLower() != "wave")) {
				break;
			}
			
			i++;
		} while (i < levelData.Length);
		
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
			
			if (levelData[i].Split(seps, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
				key = levelData[i].Split(seps)[0].Trim().ToLower();
			} else {
				key = levelData[i].TrimStart().TrimEnd();
			}
			
			if (levelData[i].Split(seps).Length > 1) {
				val = levelData[i].Split(seps)[1].TrimStart().TrimEnd();
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
				val = levelData[i].Split(seps)[1].TrimStart().TrimEnd();
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
