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
        float newX;
        float newY;
		tgt = newTarget;

        newX = 0;
        newY = 0;

        if (tgt.transform.position.x > transform.position.x) {
            newX = 1;
        } else if (tgt.transform.position.x < transform.position.x) {
            newX = -1;
        }

		// No more attacking targets up or down
        //if (tgt.transform.position.y > transform.position.y) {
        //    newY = 1;
        //} else if (tgt.transform.position.y < transform.position.y) {
        //    newY = -1;
        //}

        dir = new Vector3(newX, newY, 0);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate () {

        transform.position = transform.position + (dir * (speed + stats.speed) * Time.fixedDeltaTime);
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

		if (Mathf.Abs(transform.position.x - tgt.transform.position.x) < 1) {
			tgt.SendMessage("DamageMelee", stats, SendMessageOptions.DontRequireReceiver);
			Destroy(gameObject);
			return;
		}
    }
}
