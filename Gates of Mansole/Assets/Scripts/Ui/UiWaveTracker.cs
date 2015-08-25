using UnityEngine;
using System.Collections;

public class UiWaveTracker : MonoBehaviour {

	public GameObject world;
	public TextMesh waveText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		int curWave = world.GetComponent<WorldController> ().CurWave;
		int maxWave = world.GetComponent<WorldController> ().Levels [0].GetComponent<WaveList> ().waves.Count;

		waveText.text = curWave.ToString () + "/" + maxWave.ToString ();
	}
}
