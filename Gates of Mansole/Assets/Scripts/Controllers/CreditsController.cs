using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreditsController : MonoBehaviour {

	public float CreditChangeTime;
	public float ImageChangeTime;
	public float BackgroundChangeTime;
	public GameObject BlackScreen;
	public TextMesh CurrentCredit;
	public SpriteRenderer CurrentImage;
	public GameObject[] BackgroundImages;
	public Sprite[] CharacterImages;
	public GameObject[] CameraWaypoints;
	public GameObject cam;

	private List<string> Credits;
	private float nextCreditTime;
	private float nextImageTime;
	private float nextBackgroundTime;
	private int nextCredit;
	private int nextImage;
	private int nextBackground;
	private float alpha;
	private int currentCameraWaypoint;
	private bool isDoneCredits;

	void Start() {
		Color clr = Color.black;

		Credits = new List<string> ();
		
		Credits.Add ("Kris Murray \n Project Lead \n Programmer \n Level Designer");
		Credits.Add ("Raul Rivera \n Lead Graphic Artist");
		Credits.Add ("Arron Swan \n Graphic Artist");
		Credits.Add ("Josh Niehenke \n Programmer \n Level Designer");
		Credits.Add ("Stephen Sleeper \n Musical Score Composition");
		Credits.Add ("Ricardo Lee \n Graphic Artist");
		Credits.Add ("Brandon McCowan \n Programmer");
		Credits.Add ("b-o \n Graphic Artist");
		Credits.Add ("** God **");
		Credits.Add ("pix3m \n http://opengameart.org/\n content/\n pixel-fonts-by-pix3m");
		Credits.Add ("superjoe \n http://opengameart.org/\n content/\n electrical-disintegration-animation");
		Credits.Add ("Cookie \n http://opengameart.org/\n content/\n fire-blaston");
		Credits.Add ("Buch \n http://opengameart.org/\n content/\n golden-ui-bigger-than-ever-edition");
		Credits.Add ("makrohn,jrconway3,joewhite,Gaurav0 \n https://github.com/\n jrconway3/\n Universal-LPC-spritesheet");
		Credits.Add ("http://gaurav.munjal.us/ \n Universal-LPC-Spritesheet-Character-Generator/");
		Credits.Add ("qubodup \n http://opengameart.org/\n content/ \n rotating-crystal-animation-8-step");
		Credits.Add ("yd \n http://opengameart.org/\n content/catapult");
		Credits.Add ("Lanea Zimmerman, Stephen Challener, Charles Sanchez, Manuel Riecke, Daniel Armstrong \n http://opengameart.org/\n content/\n lpc-tile-atlas");
		Credits.Add ("artisticdude \n http://opengameart.org/\n content/\n rpg-sound-pack");
		Credits.Add ("bart \n http://opengameart.org/\n content/\n level-up-sound-effects");
		Credits.Add ("dklon \n http://opengameart.org/\n content/\n rocket-launch-0");
		Credits.Add ("haelDB \n http://opengameart.org/\n content/\n male-gruntyelling-sounds");
		Credits.Add ("MichelBaradari \n http://opengameart.org/\n content/\n 4-projectile-launches");
		Credits.Add ("NenadSimic \n http://opengameart.org/\n content/\n picked-coin-echo-2");
		Credits.Add ("Matthew Nash \n http://opengameart.org/\n content/\n misc-32x32-tiles");
		Credits.Add ("Nushio \n http://opengameart.org/\n content/ \n lpc-colorful-sand-deep-water");
		Credits.Add ("William.Thompsonj \n http://opengameart.org/\n content/\n lpc-sandrock-alt-colors");
		Credits.Add ("mishonis \n http://opengameart.org/\n content/ \n tileset-and-assets-for-a-scorched-earth-type-game");
		Credits.Add ("David Revoy \n http://opengameart.org/\n content/\n young-knight-portrait");
		Credits.Add ("Justin Nichol \n http://opengameart.org/\n content/\n princewarrior-graphic");
		Credits.Add ("Santiago Iborra \n http://opengameart.org/\n content/\n border-patrol \n http://opengameart.org/\n content/\n imperial-guard");
		Credits.Add ("Kirill777 \n http://opengameart.org/\n content/\n soldier-0 \n http://opengameart.org/\n content/stels");
		Credits.Add ("Adrix89 \n Recolor Nushio snow");
		Credits.Add ("julien-matthy, spookymodem, tinyworlds, reemax \n Misc. Sound Effects");
		Credits.Add ("** God **");

		foreach(GameObject bg in BackgroundImages) {
			bg.SetActive(false);
		}

		if (BackgroundImages.Length >= 1) {
			BackgroundImages[0].SetActive (true);
		}

		nextCreditTime = 2f;
		nextImageTime = 2f;
		nextBackgroundTime = 2f + BackgroundChangeTime;

		nextBackground = 0;
		nextImage = 0;
		nextCredit = 0;

		alpha = 1;
		clr.a = alpha;

		if (BlackScreen != null) {
			BlackScreen.SetActive(true);
			BlackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = clr;
		}

		currentCameraWaypoint = 0;
		cam.transform.position = CameraWaypoints [0].transform.position;
		isDoneCredits = false;
	}
	
	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetMouseButtonDown(0)) {
			isDoneCredits = true;
		}

		if (isDoneCredits) {
			if (alpha < 1) {
				Color clr = Color.black;
				
				// Fade in
				alpha += Time.deltaTime / 2;
				clr.a = alpha;
				
				if (BlackScreen != null) {
					BlackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = clr;
				}
			} else {
				Application.LoadLevel("Title");
			}
		} else if (alpha >= 0) {
			Color clr = Color.black;

			// Fade in
			alpha -= Time.deltaTime / 2;
			clr.a = alpha;

			if (BlackScreen != null) {
				BlackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = clr;
			}
		} else {

			if (Time.time >= nextCreditTime) {
				nextCreditTime = Time.time + CreditChangeTime;
				CurrentCredit.text = processDialogue("None", Credits[nextCredit]);
				nextCredit++;

				if (nextCredit >= Credits.Count) {
					isDoneCredits = true;
					return;
				}
			}

			if (Time.time >= nextImageTime) {
				nextImageTime = Time.time + ImageChangeTime;
				CurrentImage.sprite = CharacterImages[nextImage];
				nextImage++;
				
				if (nextImage >= CharacterImages.Length) {
					nextImage = 0;
				}
			}

			if (Time.time >= nextBackgroundTime) {
				nextBackgroundTime = Time.time + BackgroundChangeTime;
				BackgroundImages[nextBackground].SetActive(false);
				nextBackground++;

				if (nextBackground >= BackgroundImages.Length) {
					nextBackground = 0;
				}

				BackgroundImages[nextBackground].SetActive(true);
			}

			if (cam.transform.position == CameraWaypoints[currentCameraWaypoint].transform.position) {
				currentCameraWaypoint++;

				if (currentCameraWaypoint >= CameraWaypoints.Length) {
					currentCameraWaypoint = 0;
				}
			} else {
				cam.transform.position = Vector3.MoveTowards(cam.transform.position, CameraWaypoints[currentCameraWaypoint].transform.position, Time.deltaTime * 0.5f);
			}
		}
	}
	
	string processDialogue(string speaker, string text) {
		string newText = "";
		int lineLenth = 0;
		char[] seps = { ' ' };
		
		if (speaker != "None") {
			newText += speaker;
			newText += ":\n\n";
		}
		
		if (text == null) {
			return newText;
		}
		
		foreach (string word in text.Split(seps)) {
			if ((lineLenth + word.Length) >= 60) {
				newText += "\n";
				lineLenth = word.Length + 1;
			} else {
				lineLenth += word.Length + 1;
			}
			
			newText += word;
			newText += " ";
		}
		
		return newText;
	}
}
