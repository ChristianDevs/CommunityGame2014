using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CinematicController : MonoBehaviour {
	
	public TextMesh dialogueText;
	public SpriteRenderer leftImage;
	public SpriteRenderer rightImage;
	public GameObject dialogueWindow;
	public int dialogueLineSize;
	public GameObject blackScreen;

	public GameObject[] BackgroundImages;
	public Sprite[] CharacterImages;
	public AudioClip[] Music;

	private string cinFile;
	private string type;
	private bool isDone;
	private int lastMusic;

	public enum _action {
		None,
		FadeFromBlack,
		FadeToBlack
	}

	public class _cinematic_entry {
		public int Music;
		public string Background;
		public string ImageLeft;
		public string ImageRight;
		public string Speaker;
		public string Dialogue;
		public float ShowTime;
		public _action Action;
	};
	public List<_cinematic_entry> Cinematic;

	private int entryIndex;
	private float entryChangeTime;
	private float alpha;

	// Use this for initialization
	void Start () {
		Debug.Log (Player.currentChapter);
		if (Player.isWatchingIntro) {
			cinFile = Player.chapterIntroCinematicFiles [Player.currentChapter];
			type = "Intro";
			Player.watchedIntroCinematic();
		} else {
			cinFile = Player.chapterExitCinematicFiles [Player.currentChapter];
			type = "Exit";
			Player.watchedExitCinematic();
		}
		Debug.Log ("Chapter : " + Player.currentChapter.ToString() + " " + type +  " : " + Player.chapterIntroCinematicFiles [Player.currentChapter]);

		foreach (GameObject bg in BackgroundImages) {
			bg.SetActive(false);
		}

		blackScreen.SetActive(false);
		isDone = false;
		entryIndex = 0;
		entryChangeTime = 0;
		lastMusic = -1;
		StartCoroutine (Init (cinFile));
		dialogueWindow.SetActive(false);
	}

	public IEnumerator Init(string filePath) {
		string[] levelData;
		string[] seps = {"\n"};
		string[] sepsLine = {":"};
		
		if (filePath.Contains("://"))
		{
			WWW www = new WWW (filePath);
			yield return www;
			levelData = www.text.Split(seps, System.StringSplitOptions.RemoveEmptyEntries);
		} else {
			levelData = System.IO.File.ReadAllLines(filePath);
		}

		Cinematic = new List<_cinematic_entry> ();
		foreach(string ln in levelData) {
			string key = "";

			// Don't try to process empty lines
			if (ln == "") {
				continue;
			}
			
			if (ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
				key = ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[0].ToLower().TrimStart();
			} else {
				key = ln.ToLower().TrimStart().TrimEnd();
			}
			
			switch(key) {
			case "entry":
				Cinematic.Add(new _cinematic_entry());
				break;
			case "music":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].Music = int.Parse(ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart().TrimEnd ());
				}
				break;
			case "background":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].Background = ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart().TrimEnd ();
				}
				break;
			case "imageleft":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].ImageLeft = ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart().TrimEnd();
				}
				break;
			case "imageright":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].ImageRight = ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart().TrimEnd();
				}
				break;
			case "speaker":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].Speaker = ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].TrimStart().TrimEnd();
				}
				break;
			case "dialogue":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].Dialogue = ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].TrimStart().TrimEnd();
				}
				break;
			case "showtime":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].ShowTime = float.Parse(ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart().TrimEnd ());
				}
				break;
			case "action":
				if (ln.Split (sepsLine, System.StringSplitOptions.RemoveEmptyEntries).Length > 1) {
					Cinematic[Cinematic.Count - 1].Action = (_action)int.Parse(ln.Split(sepsLine, System.StringSplitOptions.RemoveEmptyEntries)[1].ToLower().TrimStart().TrimEnd ());
				}
				break;
			}
		}

		isDone = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (isDone == false) {
			return;
		}

		// Increment non-action Cinematic Entries on button press
		if (Input.GetMouseButtonDown(0)) {
			if (Cinematic[entryIndex-1].Action == _action.None) {
				entryChangeTime = 0;
			}
		}

		if (Time.time > entryChangeTime) {
			if (entryIndex < Cinematic.Count) {
				entryChangeTime = Time.time + Cinematic[entryIndex].ShowTime;

				foreach (GameObject bg in BackgroundImages) {
					bg.SetActive(false);
				}

				if ((Cinematic[entryIndex].Background.ToLower().TrimEnd() != "none") &&
					(int.Parse(Cinematic[entryIndex].Background) >= 0) &&
				    (int.Parse(Cinematic[entryIndex].Background) < BackgroundImages.Length)) {
					BackgroundImages[int.Parse(Cinematic[entryIndex].Background)].SetActive(true);
				}

				if (Cinematic[entryIndex].Dialogue != "None") {
					dialogueWindow.SetActive(true);
					dialogueText.text = processDialogue(Cinematic[entryIndex].Speaker, Cinematic[entryIndex].Dialogue);
				} else {
					dialogueWindow.SetActive(false);
				}

				if (Cinematic[entryIndex].ImageLeft == "none") {
					leftImage.sprite = null;
				} else if (int.Parse(Cinematic[entryIndex].ImageLeft) < CharacterImages.Length) {
					leftImage.sprite = CharacterImages[int.Parse(Cinematic[entryIndex].ImageLeft)];
				}
				
				if (Cinematic[entryIndex].ImageRight == "none") {
					rightImage.sprite = null;
				} else if (int.Parse(Cinematic[entryIndex].ImageRight) < CharacterImages.Length) {
					rightImage.sprite = CharacterImages[int.Parse(Cinematic[entryIndex].ImageRight)];
				}
				
				if ((Cinematic[entryIndex].Music >= 0) && (Cinematic[entryIndex].Music < Music.Length)) {
					if (lastMusic != Cinematic[entryIndex].Music) {
						Debug.Log("Playing Music");
						AudioSource.PlayClipAtPoint(Music[Cinematic[entryIndex].Music], transform.position);
						lastMusic = Cinematic[entryIndex].Music;
					}
				}

				entryIndex++;
				
				InitAction(Cinematic[entryIndex-1].Action);
			} else {
				Application.LoadLevel ("LevelSelect");
			}
		} else {
			// Update the Action
			HandleAction(Cinematic[entryIndex-1].Action);
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
			if ((lineLenth + word.Length) >= dialogueLineSize) {
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

	void HandleAction(_action Action) {
		Color clr = Color.black;

		switch(Action) {
		case _action.None:
			break;
		case _action.FadeFromBlack:
			alpha -= Time.deltaTime / Cinematic[entryIndex-1].ShowTime;
			clr.a = alpha;
			blackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = clr;
			break;
		case _action.FadeToBlack:
			alpha += Time.deltaTime / Cinematic[entryIndex-1].ShowTime;
			clr.a = alpha;
			blackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = clr;
			break;
		default:
			break;
		}
	}
	
	void InitAction(_action Action) {
		Color clr = Color.black;

		switch(Action) {
		case _action.None:
			break;
		case _action.FadeFromBlack:
			blackScreen.SetActive(true);
			alpha = 1;
			clr.a = alpha;
			blackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = clr;
			break;
		case _action.FadeToBlack:
			blackScreen.SetActive(true);
			alpha = 0;
			clr.a = alpha;
			blackScreen.GetComponent<SpriteRenderer> ().renderer.material.color = clr;
			break;
		default:
			break;
		}
	}
}
