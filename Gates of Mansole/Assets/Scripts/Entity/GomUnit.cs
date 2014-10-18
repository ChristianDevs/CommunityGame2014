using UnityEngine;
using System.Collections;

public class GomUnit : GomObject {

    public string entityName;
    public PropertyStats stats;
    public PropertyExp exp;
    public int health;
	public float speed;
	public Vector2 curTile;
	public Vector2 moveTile;
	public Weapon weapon;
	
	public enum _state {
		Idle,
		Advance,
		SetupMove,
		Move,
		Attacking,
		Engage,
		Dead,
	}
	public _state State;
	public _state NextState;

	private float deltaX;
	private float deltaY;
	private float attackTimer;
	private float dieTimer;
    public UnitAnimation._direction idleDir;

    public void DamageMelee(PropertyStats stats) {
        // Whatever - arbitrary damage calculation
        int minDamage = Random.Range(0, 2);  // always a chance of doing something
        int variation = Random.Range(0, stats.attack / 5 + 1);
        int baseDamage = stats.attack - this.stats.defense;
        Damage(minDamage + baseDamage + variation);
    }

    public void DamageSpirit(PropertyStats stats) {
        // Whatever - arbitrary damage calculation
        int minDamage = Random.Range(0, 1);  // always a chance of doing something
        int variation = 0;
        int baseDamage = stats.spirit - this.stats.armor;
        Damage(minDamage + baseDamage + variation);
    }

    public void Heal(int amt) {
        health += amt + this.stats.armor;  // Side effect of spiritual armor (I'm just making stuff up here...)
        if (health > this.stats.maxHealth)
            health = this.stats.maxHealth;
    }

    void Damage(int amt) {
        health -= amt;
        if (health <= 0) {
            health = 0;
            alive = false;
        }
    }

	public bool CanMove() {
		return ((State == _state.Idle) && (NextState == _state.Idle));
	}

    void SetFaction(GomObject.Faction newFaction) {
        faction = newFaction;
    }

	void SetCurrentTile(Vector2 tile) {
		curTile = tile;
	}

    void SetIdleDirection(UnitAnimation._direction dir) {
        idleDir = dir;

        this.SendMessage("SetDirection", idleDir, SendMessageOptions.DontRequireReceiver);
        this.SendMessage("SetAction", UnitAnimation._action.Idle, SendMessageOptions.DontRequireReceiver);
        this.SendMessage("StartAnimation", null, SendMessageOptions.DontRequireReceiver);
    }

	void Move(Vector2 newTile) {
		NextState = _state.SetupMove;
		moveTile = newTile;
	}

	void Attack() {
		NextState = _state.Attacking;
	}

	void Die() {
		NextState = _state.Dead;
		dieTimer = Time.time;
		this.SendMessage("SetAction", UnitAnimation._action.Die, SendMessageOptions.DontRequireReceiver);
		this.SendMessage("StartAnimation", null, SendMessageOptions.DontRequireReceiver);
	}
	
	// Use this for initialization
	void Start () {
		State = _state.Idle;
		NextState = State;
	}
	
	void Update() {
		// Don't update the unit until the animation has been changed
		if (NextState != State) {
			if (GetComponent<UnitAnimation>().isAnimationChanged() == true) {
				State = NextState;
			} else {
				return;
			}
		}

		switch (State) {
		case _state.Idle:
			break;
		case _state.SetupMove:
			SetupMoveUnit();
			break;
		case _state.Move:
			MoveUnit ();
			break;
		case _state.Advance:
			break;
		case _state.Attacking:
			AttackUnit();
			break;
		case _state.Engage:
			EngageUnit();
			break;
		case _state.Dead:
			DeadUnit();
			break;
		}
	}

	void DeadUnit() {
		if ((dieTimer + 1.5f) < Time.time) {
			Destroy(gameObject);
		}
	}

	void AttackUnit() {
		this.SendMessage("SetAction", weapon.actionAnim, SendMessageOptions.DontRequireReceiver);
		this.SendMessage("StartAnimation", null, SendMessageOptions.DontRequireReceiver);
		NextState = _state.Engage;
		attackTimer = Time.time;
	}

	void EngageUnit() {
		if ((attackTimer + weapon.rechargeTime) < Time.time) {
			NextState = _state.Idle;
		}
	}

	void SetupMoveUnit() {
		UnitAnimation._direction dir;
		int tileXDelta;
		int tileYDelta;

		tileXDelta = (int)moveTile.x - (int)curTile.x;
		tileYDelta = (int)moveTile.y - (int)curTile.y;
		
		if ((tileXDelta == 0) && (tileYDelta == 0)) {
			return;
		}
		
		deltaX = (float)tileXDelta;
		deltaY = (float)tileYDelta;
		
		if (Mathf.Abs(tileXDelta) > Mathf.Abs(tileYDelta)) {
			if (tileXDelta > 0) {
				dir = UnitAnimation._direction.DirRight;
			} else {
				dir = UnitAnimation._direction.DirLeft;
			}
		} else {
			if (tileYDelta > 0) {
				dir = UnitAnimation._direction.DirUp;
			} else {
				dir = UnitAnimation._direction.DirDown;
			}
		}
		
		this.SendMessage("SetDirection", dir, SendMessageOptions.DontRequireReceiver);
		this.SendMessage("SetAction", UnitAnimation._action.Walk, SendMessageOptions.DontRequireReceiver);
		this.SendMessage("StartAnimation", null, SendMessageOptions.DontRequireReceiver);
		NextState = _state.Move;
	}

	void MoveUnit() {
		float xSpeed;
		float ySpeed;

		xSpeed = 0;
		ySpeed = 0;

		if (deltaX > 0) {
			xSpeed = speed * Time.deltaTime;
			deltaX -= xSpeed;

			if (deltaX < 0) {
				xSpeed = deltaX;
				deltaX = 0;
			}
		} else if (deltaX < 0) {
			xSpeed = speed * Time.deltaTime * -1;
			deltaX -= xSpeed;
			
			if (deltaX > 0) {
				xSpeed = -deltaX;
				deltaX = 0;
			}
		}

		if (deltaY > 0) {
			ySpeed = speed * Time.deltaTime;
			deltaY -= ySpeed;
			
			if (deltaY < 0) {
				ySpeed = deltaY;
				deltaY = 0;
			}
		} else if (deltaY < 0) {
			ySpeed = speed * Time.deltaTime * -1;
			deltaY -= ySpeed;
			
			if (deltaY > 0) {
				ySpeed = -deltaY;
				deltaY = 0;
			}
		}
		
		//Debug.Log(xSpeed + ":" + ySpeed + "> <" + deltaX + ":" + deltaY);
		if ((xSpeed == 0) && (ySpeed == 0)) {
			curTile = moveTile;
            this.SendMessage("SetDirection", idleDir, SendMessageOptions.DontRequireReceiver);
			this.SendMessage("SetAction", UnitAnimation._action.Idle, SendMessageOptions.DontRequireReceiver);
			this.SendMessage("StartAnimation", null, SendMessageOptions.DontRequireReceiver);
			NextState = _state.Idle;
		} else {
			transform.position = new Vector3 (transform.position.x + xSpeed, transform.position.y + ySpeed, transform.position.z);
		}
	}
}
