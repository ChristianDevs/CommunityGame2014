using UnityEngine;
using System.Collections;

public class GomUnit : GomObject {
	//Comment to overwrite
    public string entityName;
    public PropertyStats playerStats;
	public PropertyStats enemyStats;
    public PropertyExp exp;
    public int health;
	public float speed;
	public Vector2 curTile;
	public Vector2 moveTile;
    public Weapon weapon;
	public int value; // in shards - for rewarding
	public int cost; // in shards - for purchasing

    public GameObject BarEmptyLeft;
    public GameObject BarEmptyMid;
    public GameObject BarEmptyRight;
    public GameObject BarGreen;
	public GameObject BarRed;
	public int kills;
	
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
	private GomUnit attacker;
    public GameObject world;
    public UnitAnimation._direction idleDir;
    private GameObject HpLeftBar;
    private GameObject HpMidBar;
    private GameObject HpRightBar;
	private GameObject HpBarColor;
    private GameObject HpBarFill;
    private Vector3 HpBarPos;
    private float HpBarMidScale;
    private float moveXScale;

    public void DamageMelee(PropertyStats stats) {
        // Whatever - arbitrary damage calculation
        int minDamage = Random.Range(0, 2);  // always a chance of doing something
        int variation = Random.Range(0, attacker.getStats().attack / 5 + 1);
		int baseDamage = attacker.getStats().attack - this.getStats().defense;
        Damage(minDamage + baseDamage + variation);
    }

    public void DamageSpirit(PropertyStats stats) {
        // Whatever - arbitrary damage calculation
        int minDamage = Random.Range(0, 1);  // always a chance of doing something
        int variation = 0;
        int baseDamage = attacker.getStats().spirit - this.getStats().armor;
        Damage(minDamage + baseDamage + variation);
    }

    public void Heal(int amt) {
        health += amt + this.getStats().armor;  // Side effect of spiritual armor (I'm just making stuff up here...)
        if (health > this.getStats().maxHealth)
            health = this.getStats().maxHealth;
    }

    void Damage(int amt) {

        health -= amt;
        if (health <= 0) {
            health = 0;
            alive = false;
			attacker.IncrementKills();
			attacker.RewardShards(value);
        }
        updateHealthBars();
	}

	void SetAttacker(GomUnit src) {
		attacker = src;
	}
	
	void IncrementKills() {
		kills++;
	}

	void RewardShards(int val) {
		if (faction == Faction.Player) {
			Player.spiritShards += val;
			Player.totalShards += val;
			Debug.Log ("Player now has " + Player.spiritShards + " spirit shards and " + Player.totalShards + " total shards.");
		}
	}

    void SetWorld(GameObject newWorld) {
        world = newWorld;
    }

    void updateHealthBars() {
        float percentLeft;
        float BarXPos;

        percentLeft = (float)health / (float)getStats().maxHealth;
        BarXPos = HpLeftBar.transform.position.x + (((HpRightBar.transform.position.x - HpLeftBar.transform.position.x) * 0.5f) * percentLeft);

        HpBarFill.transform.position = new Vector3(BarXPos, HpMidBar.transform.position.y, HpMidBar.transform.position.z);
        HpBarFill.transform.localScale = new Vector3(HpBarMidScale * percentLeft, 0.5f, 1);
    }

	public bool CanMove() {
		return ((State == _state.Idle) && (NextState == _state.Idle));
	}

    void SetFaction(GomObject.Faction newFaction) {
        faction = newFaction;

		setBarColor ();
    }
	
	void setBarColor() {
		if (faction == Faction.Player)
			HpBarColor = BarGreen;
		else
			HpBarColor = BarRed;
	}

	public PropertyStats getStats() {
		if (faction == Faction.Player) {
			return playerStats;
		} else {
			return enemyStats;
		}
	}

	void SetCurrentTile(Vector2 tile) {
		curTile = tile;
	}

    void SetMoveXScale(float newScale) {
        moveXScale = newScale;
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

    void Awake() {
        State = _state.Idle;
        NextState = State;
    }
	
	// Use this for initialization
	void Start () {
        HpBarPos = new Vector3(transform.position.x - 0.4f, transform.position.y + 0.4f);
        HpBarMidScale = 7.5f;

        // Instantiate the health bars
        HpLeftBar = Instantiate(BarEmptyLeft, HpBarPos, Quaternion.identity) as GameObject;
        HpMidBar = Instantiate(BarEmptyMid, HpBarPos + new Vector3(0.4f * transform.localScale.x, 0, 0), Quaternion.identity) as GameObject;
        HpRightBar = Instantiate(BarEmptyRight, HpBarPos + new Vector3(0.8f * transform.localScale.x, 0, 0), Quaternion.identity) as GameObject;
        HpBarFill = Instantiate(HpBarColor, HpBarPos + new Vector3(0.4f * transform.localScale.x, 0, 0), Quaternion.identity) as GameObject;
        HpBarFill.transform.localScale = new Vector3(HpBarMidScale, 0.5f * transform.localScale.x, 1);

        // Make the health bars follow the unit
        HpLeftBar.transform.parent = transform;
        HpMidBar.transform.parent = transform;
        HpRightBar.transform.parent = transform;
        HpBarFill.transform.parent = transform;

        updateHealthBars();
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
            this.SendMessage("SetDirection", idleDir, SendMessageOptions.DontRequireReceiver);
			this.SendMessage("SetAction", UnitAnimation._action.Idle, SendMessageOptions.DontRequireReceiver);
			this.SendMessage("StartAnimation", null, SendMessageOptions.DontRequireReceiver);
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

        deltaX = (float)tileXDelta * moveXScale;
        deltaY = (float)tileYDelta * moveXScale;
		
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
        if ((deltaX == 0) && (deltaY == 0)) {
			curTile = moveTile;
			NextState = _state.Idle;
            State = NextState;
            world.GetComponent<World>().UnitAI((int)curTile.y, (int)curTile.x);
		}

	    transform.position = new Vector3 (transform.position.x + xSpeed, transform.position.y + ySpeed, transform.position.z);
	}
}
