using UnityEngine;
using System.Collections;

public class UiNextWave : MonoBehaviour {

	public GameObject world;
	public int currentWave;
	
	// Update is called once per frame
	void Update () {
		TextMesh txt = this.GetComponent<TextMesh> ();
		float nextWaveTime = 0;
		float levelStartTime = 0;
		//int currentWave = 0;

		currentWave = world.GetComponent<WorldController> ().CurWave;
		levelStartTime = world.GetComponent<WorldController> ().levelStartTime;

		if ((levelStartTime > 0) && ((currentWave) < world.GetComponent<WorldController>().currentLevel.GetComponent<WaveList>().waves.Count)) {
			nextWaveTime = world.GetComponent<WorldController>().currentLevel.GetComponent<WaveList>().waves[currentWave].waitTime;
			nextWaveTime = nextWaveTime - (Time.time - levelStartTime);
		}

		txt.text = "Next Wave: " + (int)nextWaveTime;
	}
}
