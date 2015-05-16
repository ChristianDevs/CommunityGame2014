using UnityEngine;
using System.Collections;

public class UiNextWave : MonoBehaviour {

	public GameObject world;
	public int currentWave;
	public GameObject releaseCrystals;
	
	// Update is called once per frame
	void Update () {
		TextMesh txt = this.GetComponent<TextMesh> ();
		WorldController wc;
		float nextWaveTime = 0;
		float levelStartTime = 0;

		wc = world.GetComponent<WorldController> ();

		currentWave = wc.CurWave;
		levelStartTime = wc.levelStartTime;

		nextWaveTime = wc.currentLevel.GetComponent<WaveList>().waves[currentWave].waitTime;
		nextWaveTime = nextWaveTime - (Time.time - levelStartTime);

		txt.text = ":" + (int)nextWaveTime;

		if (releaseCrystals != null) {
			releaseCrystals.SendMessage("SetCustomValue", wc.earlyReleaseShards, SendMessageOptions.DontRequireReceiver);
		}
	}
}
