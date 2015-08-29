using UnityEngine;
using System.Collections;

public class UiButton : MonoBehaviour {

	public string buttonName;
	public GameObject UpImage;
	public GameObject DownImage;
	public GameObject HighlightImage;
	public GameObject controller;
	public bool isPressed;
    public GameObject textMeshObj;
	public AudioClip ButtonPush;

	void Start () {
		isPressed = false;

		if (UpImage != null) {
			UpImage.SetActive(true);
		}

		if (DownImage != null) {
			DownImage.SetActive(false);
		}
		
		if (HighlightImage != null) {
			HighlightImage.SetActive(false);
		}
	}

	void setName(string newName) {
		transform.name = newName;
		buttonName = newName;
	}

	void setController(GameObject newController) {
		controller = newController;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hitButton;

		if (isPressed == true) {
			if (Input.GetMouseButtonUp(0)) {
				if (UpImage != null) {
					UpImage.SetActive(true);
				}

				if (DownImage != null) {
					DownImage.SetActive(false);
				}
				
				if (HighlightImage != null) {
					HighlightImage.SetActive(false);
				}
				isPressed = false;

                if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitButton)) {
					if ((hitButton.transform.name == buttonName) &&
					    (controller != null)) {
						controller.SendMessage("buttonPush", name, SendMessageOptions.DontRequireReceiver);
					}

					if ((ButtonPush != null) && (Player.SoundEnable == true)) {
						AudioSource.PlayClipAtPoint(ButtonPush, transform.position);
					}
				}
			}
		} else {
			if (Input.GetMouseButtonDown(0)) {
                if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitButton)) {
					if (hitButton.transform.name == buttonName) {
						if (UpImage != null) {
							UpImage.SetActive(false);
						}

						if (DownImage != null) {
							DownImage.SetActive(true);
						}
						
						if (HighlightImage != null) {
							HighlightImage.SetActive(false);
						}
						isPressed = true;
					}
				}
            } else {
                if (Physics.Raycast(UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition), out hitButton)) {
                    if (hitButton.transform.name == buttonName) {
						if (UpImage != null) {
							UpImage.SetActive(false);
						}
						
						if (DownImage != null) {
							DownImage.SetActive(false);
						}
						
						if (HighlightImage != null) {
							HighlightImage.SetActive(true);
						}
                    } else {
						if (UpImage != null) {
							UpImage.SetActive(true);
						}
						
						if (DownImage != null) {
							DownImage.SetActive(false);
						}
						
						if (HighlightImage != null) {
							HighlightImage.SetActive(false);
						}
					}
				} else {
					if (UpImage != null) {
						UpImage.SetActive(true);
					}
					
					if (DownImage != null) {
						DownImage.SetActive(false);
					}
					
					if (HighlightImage != null) {
						HighlightImage.SetActive(false);
					}
				}
			}
		}
	}
}
