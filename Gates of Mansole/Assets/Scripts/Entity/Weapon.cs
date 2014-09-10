using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : MonoBehaviour {
    public AudioClip[] sounds;
    public float rechargeTime = 1.0f;
    public float recharge { get; set; }

    public abstract void Trigger();
}
