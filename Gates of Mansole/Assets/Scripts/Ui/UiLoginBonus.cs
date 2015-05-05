using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiLoginBonus : MonoBehaviour {

	public TextMesh Greeting;
	public GameObject EnabledBoxPrefab;
	public GameObject DisabledBoxPrefab;
	public GameObject[] BoxLocations;

	private List<GameObject> BoxInst;
	private int OrbBonus;

	// Use this for initialization
	void Start () {
		System.DateTime lastLogon;
		OrbBonus = 0;

		if (System.DateTime.TryParse(Player.getLastLogon(), out lastLogon) == false) {
			// If no valid string then no previous logon
			Greeting.text = "Login tomorrow for a Spirit Orb Bonus!";
			Player.resetLogon();
			BoxInst = new List<GameObject> ();
			for (int i = 0; i < BoxLocations.Length; i++) {
				GameObject box;
				
				box = Instantiate(DisabledBoxPrefab, BoxLocations[i].transform.position, Quaternion.identity) as GameObject;
				box.GetComponentInChildren<TextMesh>().text = (i + 1).ToString() + " Day\n" + (i + 1).ToString() + " :";
				BoxInst.Add(box);
			}
		} else if (System.DateTime.Compare(System.DateTime.Now, lastLogon.AddDays(1)) < 0) {
			// If last logon is less than a day then destroy this object
			Destroy (gameObject);
		} else if ((System.DateTime.Compare(System.DateTime.Now, lastLogon.AddDays(1)) >= 0) &&
		           (System.DateTime.Compare(System.DateTime.Now, lastLogon.AddDays(2)) < 0)) {
			// If last logon is more than a day but less than two days then give the bonus
			Greeting.text = "Keep logging on for a bigger bonus!";
			BoxInst = new List<GameObject> ();
			for (int i = 0; i < BoxLocations.Length; i++) {
				GameObject box;

				if (Player.getNumConsLogons() == i) {
					box = Instantiate(EnabledBoxPrefab, BoxLocations[i].transform.position, Quaternion.identity) as GameObject;
					OrbBonus = i + 1;
				} else if ((i == (BoxLocations.Length - 1)) && (Player.getNumConsLogons() > BoxLocations.Length)) {
					box = Instantiate(EnabledBoxPrefab, BoxLocations[i].transform.position, Quaternion.identity) as GameObject;
					OrbBonus = BoxLocations.Length;
				} else {
					box = Instantiate(DisabledBoxPrefab, BoxLocations[i].transform.position, Quaternion.identity) as GameObject;
				}

				box.GetComponentInChildren<TextMesh>().text = (i + 1).ToString() + " Day\n" + (i + 1).ToString() + " :";
				BoxInst.Add(box);
			}
			Player.addLogon ();
		} else {
			// If the last logon is more than two days then reset the bonus
			Greeting.text = "Login tomorrow for a Spirit Orb Bonus!";
			Player.resetLogon();
			BoxInst = new List<GameObject> ();
			for (int i = 0; i < BoxLocations.Length; i++) {
				GameObject box;

				box = Instantiate(DisabledBoxPrefab, BoxLocations[i].transform.position, Quaternion.identity) as GameObject;
				box.GetComponentInChildren<TextMesh>().text = (i + 1).ToString() + " Day\n" + (i + 1).ToString() + " :";
				BoxInst.Add(box);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Player.AddOrbs(OrbBonus);
			foreach(GameObject bi in BoxInst) {
				Destroy(bi);
			}
			Destroy(gameObject);
		}
	}
}
