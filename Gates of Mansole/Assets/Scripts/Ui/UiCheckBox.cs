using UnityEngine;
using System.Collections;

public class UiCheckBox : MonoBehaviour {

	public string CheckBoxItem;
	public GameObject BoxObject;
	public Sprite EnableImage;
	public Sprite DisableImage;

	private bool boxChecked;

	// Use this for initialization
	void Start () {
		if (CheckBoxItem == "Music") {
			boxChecked = Player.MusicEnable;
		} else if (CheckBoxItem == "Sound") {
			boxChecked = Player.SoundEnable;
		}

		UpdateBox();
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hitButton;

		if (Input.GetMouseButtonDown(0)) {
			if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitButton)) {
				if (hitButton.transform.name == transform.name) {
					boxChecked = !boxChecked;
					UpdateBox();
					
					if (CheckBoxItem == "Music") {
						Player.MusicEnable = boxChecked;
					} else if (CheckBoxItem == "Sound") {
						Player.SoundEnable = boxChecked;
					}
				}
			}
		}
	}

	void UpdateBox() {
		if (boxChecked) {
			BoxObject.GetComponent<SpriteRenderer>().sprite = EnableImage;
		} else {
			BoxObject.GetComponent<SpriteRenderer>().sprite = DisableImage;
		}
	}
}
