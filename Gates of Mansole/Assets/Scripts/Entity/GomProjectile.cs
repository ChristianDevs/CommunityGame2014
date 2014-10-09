using UnityEngine;
using System.Collections;

public class GomProjectile : GomObject {

    // We'll probably use our own basic game physics and not try to hack around box2d
    public Vector3 dir;
    public float speed = 1;

    // Note this could be positive or negative...
    public float accel;

    public bool isAir = true;
    public bool isGround = true;

    public PropertyStats stats;

	private GameObject tgt;

	void SetTarget(GameObject newTarget) {
		tgt = newTarget;
		dir = tgt.transform.position;
		dir.Normalize();
	}

	// Use this for initialization
	void Start () {
        dir.Normalize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate () {
        // @@ It may make more sense to keep projectile movement data in the Logic* code for each projectile type, unless
        // they all act a common way.

        // We may end up doing work on rigidbody2d with no friction instead of directly to transform
        // Obviously this is just an example... projectiles could have any form of complex movement depending
        // on what is designed.
        // This is just an example
        transform.position = transform.position + (new Vector3(-1, 0, 0) * (speed + stats.speed) * Time.fixedDeltaTime);
        speed += accel * Time.fixedDeltaTime;

        if (speed < 0) {
            Destroy(gameObject);
			return;
		}

		if (tgt == null) {
			Destroy(gameObject);
			return;
		}

		if ((transform.position.x > 25) || (transform.position.x < -25) || (transform.position.y > 25) || (transform.position.y < -25)) {
			Destroy(gameObject);
			return;
		}

		if (Vector3.Distance (transform.position, tgt.transform.position) < 1) {
			tgt.SendMessage("DamageMelee", stats, SendMessageOptions.DontRequireReceiver);
			Destroy(gameObject);
			return;
		}
    }
}
