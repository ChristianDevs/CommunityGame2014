using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectController : MonoBehaviour {

    public GameObject level1;
    public GameObject BeatGameMessage;
	public GameObject[] Maps;
	public GameObject ChapterNumUI;
	public GameObject highlightPS;
	public GameObject CursorPrefab;
	public GameObject BlackScreen;
	public GameObject[] ChapterButtons;

    private List<GameObject> levelButtons;
	private GameObject particleSystem;
	private GameObject cursorInst;
	private bool inTutorial;
	private float alpha;

	// Use this for initialization
	void Start () {
		Color color = Color.black;
		
		alpha = 1;
		color.a = alpha;
		BlackScreen.SetActive (true);
		BlackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = color;

		particleSystem = null;
		inTutorial = false;
		UpdateMap();

		if (Player.levelComplete[0][14] <= 0) {
			foreach(GameObject btn in ChapterButtons) {
				btn.SetActive(false);
			}
		} else {
			foreach(GameObject btn in ChapterButtons) {
				btn.SetActive(true);
			}
		}
		
		// Show player to click the level
		if ((Player.tutorialState == 0) && (inTutorial == false)) {
			cursorInst = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			cursorInst.transform.position = levelButtons[0].transform.position;
			cursorInst.GetComponentInChildren<Animator>().SetTrigger("DoTut2");
			inTutorial = true;
		} else if ((Player.tutorialState == 5) && (inTutorial == false)) {
			cursorInst = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			cursorInst.transform.position = new Vector3(5.2f, 4.6f, 0);
			cursorInst.GetComponentInChildren<Animator>().SetTrigger("DoTut2");
			inTutorial = true;
		} else if ((Player.tutorialState == 10) && (inTutorial == false)) {
			cursorInst = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			cursorInst.transform.position = new Vector3(2.1f, 1.8f, 0);
			cursorInst.GetComponentInChildren<Animator>().SetTrigger("DoTut2");
			inTutorial = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Color color = Color.black;

		ChapterNumUI.GetComponent<TextMesh> ().text = (Player.currentChapter + 1).ToString ();
		
		if (alpha > 0) {
			alpha -= Time.deltaTime;
			color.a = alpha;
			BlackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = color;
		}
	}

	void UpdateMap() {
		foreach (GameObject map in Maps) {
			map.SetActive(false);
		}
		if (Player.currentChapter < Maps.Length) {
			Maps[Player.maps[Player.currentChapter]].SetActive(true);
		}

		if (levelButtons != null) {
			for (int i = 1; i < levelButtons.Count; i++) {
				Destroy(levelButtons[i]);
			}
		}

		if (particleSystem != null) {
			Destroy(particleSystem);
		}
		
		// Always let the player access level 1
		level1.SetActive(true);
		level1.transform.position = new Vector3 (Player.levelLocs[Player.currentChapter][0].x, Player.levelLocs[Player.currentChapter][0].y, level1.transform.position.z);
		levelButtons = new List<GameObject>();
		levelButtons.Add(level1);
		levelButtons [levelButtons.Count - 1].transform.localScale = new Vector3 (0.6f, 0.6f, 1);

		for (int i = 1; i < Player.levelFileNames[Player.currentChapter].Count; i++) {
			if (Player.levelComplete[Player.currentChapter][i - 1] > 0) {
				levelButtons.Add(Instantiate(level1, level1.transform.position, Quaternion.identity) as GameObject);
				levelButtons[i].name = (i + 1).ToString();
				levelButtons[i].GetComponent<UiButton>().buttonName = levelButtons[i].name;
				levelButtons[i].GetComponent<UiButton>().textMeshObj.GetComponent<TextMesh>().text = levelButtons[i].name;
				levelButtons[i].GetComponent<UiButton>().controller = gameObject;
				levelButtons[i].transform.position = new Vector3(Player.levelLocs[Player.currentChapter][i].x, Player.levelLocs[Player.currentChapter][i].y, levelButtons[i].transform.position.z);
				levelButtons [levelButtons.Count - 1].transform.localScale = new Vector3 (0.6f, 0.6f, 1);

				if ((Player.tutorialState == 5) && (i == 1)) {
					levelButtons[i].SetActive(false);
				}
			}
		}

		if (Player.chapterProgression >= Player.levelFileNames.Count) {
			// Chapter 3 is the latest implemented, use commented out if statement when ch4 is implemented
			// And change the message to indicate the game has been won.
			BeatGameMessage.SetActive(true);
		} else {
			BeatGameMessage.SetActive(false);
			
			// Make the last level bigger
			levelButtons [levelButtons.Count - 1].transform.localScale = new Vector3 (0.75f, 0.75f, 1);

			if (Player.tutorialState != 5) {
				// Particle System to draw user's attention to the new level
				particleSystem = Instantiate(highlightPS, levelButtons[levelButtons.Count - 1].transform.position, Quaternion.identity) as GameObject;
			}
		}
	}

    void buttonPush(string buttonName) {
		switch (buttonName) {
			case "Back":
				Application.LoadLevel("Title");
				break;
			case "Upgrade":
				// End Tutorial when player clicks the market
				if ((Player.tutorialState == 3) && (inTutorial)) {
					inTutorial = false;
					
					if (cursorInst != null) {
						Destroy(cursorInst);
						cursorInst = null;
					}
					
					Player.completeTutorialState();
				} else if ((Player.tutorialState == 10) && (inTutorial)) {
					inTutorial = false;
					
					if (cursorInst != null) {
						Destroy(cursorInst);
						cursorInst = null;
					}
					
					Player.completeTutorialState();
				}

				Application.LoadLevel("Upgrade");
			break;
			case "Previous":
				if (Player.currentChapter > 0) {
					Player.currentChapter--;
					UpdateMap();
				}
			break;
			case "Next":
				if (Player.currentChapter < Player.chapterProgression) {
					Player.currentChapter++;
					UpdateMap();
					if (Player.introCinematicsWatched[Player.currentChapter] == false) {
						Player.isWatchingIntro = true;
						Application.LoadLevel("Cinematic");
					}
				}
			break;
		default:
				// End Tutorial when player clicks the fist level
				if ((Player.tutorialState == 0) && (inTutorial)) {
					inTutorial = false;
					
					if (cursorInst != null) {
						Destroy(cursorInst);
						cursorInst = null;
					}
					
					Player.completeTutorialState();
				}

	            Player.currentLevel = int.Parse(buttonName) - 1;
				Player.nextLevelFile = Player.levelFileNames[Player.currentChapter][Player.currentLevel];
	            Debug.Log(Player.nextLevelFile);
				Application.LoadLevel("AutoLevel");
                break;
        }
    }
}
