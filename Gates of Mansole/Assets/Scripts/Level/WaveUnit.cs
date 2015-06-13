using UnityEngine;
using System.Collections;

[System.Serializable]
public class WaveUnit {

	public enum _spawnLocType
    {
        RandRow,
		RandTile,
		SpecifiedTile,
		SpecifiedRow
	};

    public float time = 0;
    public GameObject prefab;
	public bool created = false;
	public _spawnLocType SpawnLocType;
	public Vector2 Tile;
	public float RespawnTime;
	public float DeathTime;
}
