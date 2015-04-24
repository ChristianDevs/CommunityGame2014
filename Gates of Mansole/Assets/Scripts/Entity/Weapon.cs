using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour {
    public AudioClip sound;
    public float rechargeTime = 1.0f;
	public float recharge { get; set; }
	public int range;
	public int damageRadius;
	public UnitAnimation._action actionAnim;
	public GameObject projectile;
	public float projFireDelay = 0f;
}
