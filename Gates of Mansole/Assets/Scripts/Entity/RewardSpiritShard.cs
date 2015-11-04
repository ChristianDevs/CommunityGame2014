using UnityEngine;
using System.Collections;

public class RewardSpiritShard : MonoBehaviour {

	public GameObject travelPlace;
	public float createTime;
	public int shardAmount;
	public WorldController world;
	public GameObject Shard;

	private bool isClicked;
	private GameObject bonusShardText;

	// Use this for initialization
	void Start () {
		createTime = Time.time;
		isClicked = false;
	}
	
	// Update is called once per frame
	void Update () {
		if ((Time.time - createTime) > 10) {
			Color tempCol = Shard.GetComponent<SpriteRenderer>().color;
			tempCol.a = 0.5f;
			Shard.GetComponent<SpriteRenderer>().color = tempCol;
		}

		if ((Time.time - createTime) > 15) {
			Destroy(gameObject);
		}

		if (Vector3.Distance (transform.position, travelPlace.transform.position) < 0.1f) {
			Player.spiritShards += shardAmount;
			Player.totalShards += shardAmount;

			Debug.Log ("Player now has " + Player.spiritShards + " spirit shards and " + Player.totalShards + " total shards.");
			Destroy(bonusShardText);
			Destroy(gameObject);
		}

		if (isClicked == false) {
			if (Input.GetMouseButton(0) == true) {
				RaycastHit hitObj;

				if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitObj)) {
					if (hitObj.transform.name == transform.name) {
						isClicked = true;
						
						if (world != null) {
							if ((Player.tutorialState == 1) && (world.GetComponent<WorldController>().shardTutorial == true)) {
								world.GetComponent<WorldController>().shardTutorial = false;
								world.GetComponent<WorldController>().advanceTutorial = true;
							}

							if ((world.lastShardPickupTime + world.extraSwipeShardTimeout) >= Time.time) {
								world.totalSwipeShards++;
								switch(world.totalSwipeShards) {
								case 0:
									break;
								case 1:
									shardAmount += 1;
									break;
								case 2:
									shardAmount += 1;
									break;
								case 3:
									shardAmount += 2;
									break;
								case 4:
									shardAmount += 2;
									break;
								case 5:
									shardAmount += 2;
									break;
								default:
									shardAmount += 3;
									break;
								}
							} else {
								world.totalSwipeShards = 0;
							}
							
							bonusShardText = Instantiate(world.swipeExtraRewardTextPrefab, transform.position, Quaternion.identity) as GameObject;
							bonusShardText.GetComponent<TextMesh>().text = "+" + shardAmount.ToString();
							world.lastShardPickupTime = Time.time;
						}
					}
				}
			}
		} else {
			transform.position = Vector3.Lerp(transform.position, travelPlace.transform.position, Time.deltaTime * 4);
		}
	}
}
