using UnityEngine;
using System.Collections;

public class RewardSpiritShard : MonoBehaviour {

	public GameObject travelPlace;
	public float createTime;
	public int shardAmount;
	public GameObject world;

	private bool isClicked;
	private GameObject bonusShardText;

	// Use this for initialization
	void Start () {
		createTime = Time.time;
		isClicked = false;
	}
	
	// Update is called once per frame
	void Update () {
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
						
						if ((world != null) &&
						    (world.GetComponent<WorldController>() != null)) {
							
							if (Player.tutorialState == 2) {
								world.GetComponent<WorldController>().advanceTutorial = true;
							}

							if ((world.GetComponent<WorldController>().lastShardPickupTime + world.GetComponent<WorldController>().extraSwipeShardTimeout) >= Time.time) {
								if (world.GetComponent<WorldController>().maxExtraSwipeShard >= (world.GetComponent<WorldController>().totalSwipeShards / 3)) {
									world.GetComponent<WorldController>().totalSwipeShards++;
								}
								
								if ((world.GetComponent<WorldController>().totalSwipeShards / 3) > 0) {
									shardAmount += world.GetComponent<WorldController>().totalSwipeShards / 3;
									bonusShardText = Instantiate(world.GetComponent<WorldController>().swipeExtraRewardTextPrefab, transform.position, Quaternion.identity) as GameObject;
									bonusShardText.GetComponent<TextMesh>().text = "+" + (world.GetComponent<WorldController>().totalSwipeShards / 3).ToString();
								}
							} else {
								world.GetComponent<WorldController>().totalSwipeShards = 0;
							}
							
							world.GetComponent<WorldController>().lastShardPickupTime = Time.time;
						}
					}
				}
			}
		} else {
			transform.position = Vector3.Lerp(transform.position, travelPlace.transform.position, Time.deltaTime * 4);
		}
	}
}
