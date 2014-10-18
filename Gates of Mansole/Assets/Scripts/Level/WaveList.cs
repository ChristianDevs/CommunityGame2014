using UnityEngine;
using System.Collections;

public class WaveList : MonoBehaviour {

    public enum _direction {
        Up,
        Down,
        Left,
        Right
    }

    public bool started = false;  // When true, waves begin
    public int AttackersLetThrough;
    public bool isPlayerAttacker = false;
    public _direction attackerDir = _direction.Right;
    public Wave[] waves;
}
