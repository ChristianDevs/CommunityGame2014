using UnityEngine;
using System.Collections;

[System.Serializable]
public class Wave {
    public float waitTime = 30;  // Before deploying wave
    public WaveUnit[] units;

    public bool finished { get; set; }  // True once all WaveUnits are deployed
    public float waited { get; set; }
}
