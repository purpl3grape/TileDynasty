using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerTriggerBehavior : MonoBehaviour {

	NetworkManager nManager;
	public StatsManager statManager;
	public Sprite [] MapSpriteList;
	public AudioClip [] ClickSound;
	public Color OriginalColor;
	public Color ClickColor;
	public Color ReadyColor;
	public Color ReadyTextColor;
	public Color notReadyColor;
	public Color notReadyTextColor;
	private Color tempColor;
	private string playerPrefsGameMapKey = "playerPrefsGameMapKey";
	private string playerPrefsGameModeKey = "playerPrefsGameModeKey";
	private string playerPrefsPlayerCountKey = "playerPrefsPlayerCountKey";
	private string playerPrefsGameTimeKey = "playerPrefsGameTimeKey";
	private string startMultiplayerFromHomeBaseKey = "startMultiplayerFromHomeBaseKey";

	[HideInInspector] public bool isTriggerTrue;
	[HideInInspector] public int OptionProgress = 0;
	[HideInInspector] public int MapNumber = 0;
	[HideInInspector] public int GameModeNumber = 0;
	[HideInInspector] public int PlayerCount = 0;
	[HideInInspector] public int GameTime = 0;
	[HideInInspector] public bool hasPickedMap = false;
	[HideInInspector] public bool hasPickedGameMode = false;
	[HideInInspector] public bool hasPickedPlayerCount = false;
	[HideInInspector] public bool hasPickedGameTime = false;
	[HideInInspector] public string selectedMapText = "";
	[HideInInspector] public string selectedGameMode = "";
	[HideInInspector] public string selectedPlayerCount = "";
	[HideInInspector] public string selectedGameTime = "";
	[HideInInspector] public int optionsSelected = 0;
	[HideInInspector] public int pickedMapVal = 0;
	[HideInInspector] public int pickedGameModeVal = 0;
	[HideInInspector] public int pickedPlayerCountVal = 0;
	[HideInInspector] public int pickedGameTimeVal = 0;

	void init(){
		statManager.MultiplayerMap.SetActive (false);
		statManager.MultiplayerMode.SetActive (false);
		statManager.MultiplayerPlayers.SetActive (false);
		statManager.MultiplayerTime.SetActive (false);
		statManager.ChoiceBack.SetActive (false);
		statManager.ChoiceNext.SetActive (false);
		statManager.OptionBack.SetActive (false);
		statManager.OptionNext.SetActive (false);
		statManager.CheckMark.SetActive (false);
		statManager.Selection.SetActive (false);
		statManager.Selection.GetComponent<Text> ().text = "";
		statManager.CurrentOption.SetActive (false);
		statManager.SelectedOption.SetActive (false);
		statManager.CheckMark2.SetActive (false);
		statManager.OptionNext.GetComponent<Image> ().color = OriginalColor;
		OptionProgress = 0;
		MapNumber = 0;
		//For now start at 1 we just want to have TDM mode used for now, maybe open up the other game modes after..
		GameModeNumber = 1;
		PlayerCount = 2;
		GameTime = 10;
		hasPickedMap = false;
		hasPickedGameMode = false;
		hasPickedPlayerCount = false;
		hasPickedGameTime = false;
		optionsSelected = 0;

		PlayerPrefs.SetInt (playerPrefsGameMapKey, 0);
		PlayerPrefs.SetInt (playerPrefsGameModeKey, 0);
		PlayerPrefs.SetInt (playerPrefsPlayerCountKey, 2);
		PlayerPrefs.SetInt (playerPrefsGameTimeKey, 10);

	}

	void Start () {
		init ();
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
	}

	IEnumerator ClickColor_Coroutine(Image img){
		statManager.OptionBack.GetComponent<Image> ().color = OriginalColor;
		statManager.OptionNext.GetComponent<Image> ().color = OriginalColor;
		statManager.ChoiceBack.GetComponent<Image> ().color = OriginalColor;
		statManager.ChoiceNext.GetComponent<Image> ().color = OriginalColor;

		tempColor = img.color;
		img.color = ClickColor;
		yield return new WaitForSeconds (0.1f);
		img.color = tempColor;
	}

	void MenuItemsUpdate(){

		if (!isTriggerTrue)
			return;

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (OptionProgress > 0) {
				OptionProgress -= 1;
				StartCoroutine (ClickColor_Coroutine (statManager.OptionBack.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
			} else {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
			}
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (OptionProgress < 3) {
				OptionProgress += 1;
				StartCoroutine (ClickColor_Coroutine (statManager.OptionNext.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
			} else {
				if (optionsSelected < 4) {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			}
		}
	}

	void CountSelectedOptionsUpdate(){
		pickedMapVal = hasPickedMap == true ? 1 : 0;
		pickedGameModeVal = hasPickedGameMode == true ? 1 : 0;
		pickedPlayerCountVal = hasPickedPlayerCount == true ? 1 : 0;
		pickedGameTimeVal = hasPickedGameTime == true ? 1 : 0;
		optionsSelected = pickedMapVal + pickedGameModeVal + pickedPlayerCountVal + pickedGameTimeVal;
		if (optionsSelected == 4) {
			statManager.SelectedOption.GetComponent<Text> ().color = ReadyTextColor;
			statManager.CheckMark2.GetComponent<Image> ().color = ReadyColor;
		} else {
			statManager.SelectedOption.GetComponent<Text> ().color = notReadyTextColor;
			statManager.CheckMark2.GetComponent<Image> ().color = notReadyColor;
		}
		statManager.SelectedOption.GetComponent<Text> ().text = optionsSelected.ToString () + " / 4";

	}

	void ChoiceOptionsUpdate(){

		if (!isTriggerTrue)
			return;

		//Map Selection 3 maps
		if (OptionProgress == 0) {
			statManager.CurrentOption.GetComponent<Text> ().text = "1 / 4";
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				if (MapNumber > 0) {
					MapNumber -= 1;
					if (MapNumber == 3) {
						//Just to bypass the home base for now
						MapNumber--;
					}
					StartCoroutine (ClickColor_Coroutine (statManager.ChoiceBack.GetComponent<Image> ()));
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if (MapNumber < 4) {
					MapNumber += 1;
					if (MapNumber == 3) {
						MapNumber++;
					}
					StartCoroutine (ClickColor_Coroutine (statManager.ChoiceNext.GetComponent<Image> ()));
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			}
				

			if (MapNumber == 0) {
				statManager.MultiplayerMap_Image.GetComponent<Image> ().sprite = MapSpriteList [0];
				statManager.MultiplayerMap_Text.GetComponent<Text> ().text = "MILITARY BASE";
			} else if (MapNumber == 1) {
				statManager.MultiplayerMap_Image.GetComponent<Image> ().sprite = MapSpriteList [1];
				statManager.MultiplayerMap_Text.GetComponent<Text> ().text = "CAMPGROUNDS";
			} else if (MapNumber == 2) {
				statManager.MultiplayerMap_Image.GetComponent<Image> ().sprite = MapSpriteList [2];
				statManager.MultiplayerMap_Text.GetComponent<Text> ().text = "WINTER CAMP";
			} else if (MapNumber == 4) {
				statManager.MultiplayerMap_Image.GetComponent<Image> ().sprite = MapSpriteList [3];
				statManager.MultiplayerMap_Text.GetComponent<Text> ().text = "WASTE LANDS";
			}

			if (Input.GetKeyDown (KeyCode.Return)) {
				StartCoroutine (ClickColor_Coroutine (statManager.CheckMark.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
				selectedMapText = statManager.MultiplayerMap_Text.GetComponent<Text> ().text;
				hasPickedMap = true;

			}
			if (hasPickedMap) {
				statManager.CheckMark.SetActive (true);
				statManager.Selection.GetComponent<Text> ().text = selectedMapText;
			} else {
				statManager.CheckMark.SetActive (false);
				statManager.Selection.GetComponent<Text> ().text = "";
			}
		}
		//Game Mode Selection 3 game modes
		if (OptionProgress == 1) {
			statManager.CurrentOption.GetComponent<Text> ().text = "2 / 4";
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				if (GameModeNumber > 1) {
					GameModeNumber -= 1;
					StartCoroutine (ClickColor_Coroutine (statManager.ChoiceBack.GetComponent<Image> ()));
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if (GameModeNumber < 1) {
					GameModeNumber += 1;
					StartCoroutine (ClickColor_Coroutine (statManager.ChoiceNext.GetComponent<Image> ()));
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			}
			if (GameModeNumber == 0) {
				statManager.MultiplayerMode_Text.GetComponent<Text> ().text = "TILE DYNASTY";
			} else if (GameModeNumber == 1) {
				statManager.MultiplayerMode_Text.GetComponent<Text> ().text = "TEAM DEATH MATCH";
			} else if (GameModeNumber == 2) {
				statManager.MultiplayerMode_Text.GetComponent<Text> ().text = "SURVIVAL";
			}

			if (Input.GetKeyDown (KeyCode.Return)) {
				StartCoroutine (ClickColor_Coroutine (statManager.CheckMark.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
				selectedGameMode = statManager.MultiplayerMode_Text.GetComponent<Text> ().text;
				hasPickedGameMode = true;
			}
			if (hasPickedGameMode) {
				statManager.CheckMark.SetActive (true);
				statManager.Selection.GetComponent<Text> ().text = selectedGameMode;
			} else {
				statManager.CheckMark.SetActive (false);
				statManager.Selection.GetComponent<Text> ().text = "";
			}

		}
		//Player Selection Max 6
		if (OptionProgress == 2) {
			statManager.CurrentOption.GetComponent<Text> ().text = "3 / 4";
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				if (PlayerCount > 1) {
					PlayerCount -= 1;
					StartCoroutine (ClickColor_Coroutine (statManager.ChoiceBack.GetComponent<Image> ()));
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if (PlayerCount < 6) {
					PlayerCount += 1;
					StartCoroutine (ClickColor_Coroutine (statManager.ChoiceNext.GetComponent<Image> ()));
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			}
			if (PlayerCount == 1) {
				statManager.MultiplayerPlayers_Text.GetComponent<Text> ().text = "1 PLAYER";
			} else if (PlayerCount == 2) {
				statManager.MultiplayerPlayers_Text.GetComponent<Text> ().text = "2 PLAYERS";
			} else if (PlayerCount == 3) {
				statManager.MultiplayerPlayers_Text.GetComponent<Text> ().text = "3 PLAYERS";
			} else if (PlayerCount == 4) {
				statManager.MultiplayerPlayers_Text.GetComponent<Text> ().text = "4 PLAYERS";
			} else if (PlayerCount == 5) {
				statManager.MultiplayerPlayers_Text.GetComponent<Text> ().text = "5 PLAYERS";
			} else if (PlayerCount == 6) {
				statManager.MultiplayerPlayers_Text.GetComponent<Text> ().text = "6 PLAYERS";
			}

			if (Input.GetKeyDown (KeyCode.Return)) {
				StartCoroutine (ClickColor_Coroutine (statManager.CheckMark.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
				selectedPlayerCount = statManager.MultiplayerPlayers_Text.GetComponent<Text> ().text;
				hasPickedPlayerCount = true;
			}
			if (hasPickedPlayerCount) {
				statManager.CheckMark.SetActive (true);
				statManager.Selection.GetComponent<Text> ().text = selectedPlayerCount;
			} else {
				statManager.CheckMark.SetActive (false);
				statManager.Selection.GetComponent<Text> ().text = "";
			}

		}
		//Game Time Max 10
		if (OptionProgress == 3) {
			statManager.CurrentOption.GetComponent<Text> ().text = "4 / 4";
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				if (GameTime > 1) {
					GameTime -= 1;
					StartCoroutine (ClickColor_Coroutine (statManager.ChoiceBack.GetComponent<Image> ()));
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if (GameTime < 10) {
					GameTime += 1;
					StartCoroutine (ClickColor_Coroutine (statManager.ChoiceNext.GetComponent<Image> ()));
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			}
			if (GameTime == 1) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "1 DAY";
			} else if (GameTime == 2) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "2 DAYS";
			} else if (GameTime == 3) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "3 DAYS";
			} else if (GameTime == 4) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "4 DAYS";
			} else if (GameTime == 5) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "5 DAYS";
			} else if (GameTime == 6) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "6 DAYS";
			} else if (GameTime == 7) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "7 DAYS";
			} else if (GameTime == 8) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "8 DAYS";
			} else if (GameTime == 9) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "9 DAYS";
			} else if (GameTime == 10) {
				statManager.MultiplayerTime_Text.GetComponent<Text> ().text = "10 DAYS";
			}

			if (Input.GetKeyDown (KeyCode.Return)) {
				StartCoroutine (ClickColor_Coroutine (statManager.CheckMark.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
				selectedGameTime = statManager.MultiplayerTime_Text.GetComponent<Text> ().text;
				hasPickedGameTime = true;
			}
			if (hasPickedGameTime) {
				statManager.CheckMark.SetActive (true);
				statManager.Selection.GetComponent<Text> ().text = selectedGameTime;
			} else {
				statManager.CheckMark.SetActive (false);
				statManager.Selection.GetComponent<Text> ().text = "";
			}

		}
	}

	void Update () {
		enableMultiplayerScreen (isTriggerTrue);
		MenuItemsUpdate ();
		ChoiceOptionsUpdate ();
		CountSelectedOptionsUpdate ();
//		if (Input.GetKeyDown (KeyCode.K)) {

//			PlayerPrefs.SetInt ("ViewRoomList", 1);
//			PlayerPrefs.Save ();
//			nManager.SceneLoadingPanel.SetActive (true);
//			PhotonNetwork.Disconnect ();
//			SceneManager.LoadScene (0);
//			Steamworks.SteamUserStats.ResetAllStats (true);
//		}
	}

	void OnTriggerEnter(){
		//Just let them play multiplayer either way so make istrigger true nomatter what
		if (Steamworks.SteamFriends.GetPersonaState () == Steamworks.EPersonaState.k_EPersonaStateOffline) {
			isTriggerTrue = true;
			statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
		} else {
			isTriggerTrue = true;
			statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
		}
			
	}

	void OnTriggerExit(){
		isTriggerTrue = false;
		statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
	}

	public void enableMultiplayerScreen(bool val){

		if (!val) {
			init ();
			return;
		}

		statManager.ChoiceBack.SetActive (val);
		statManager.ChoiceNext.SetActive (val);
		statManager.OptionBack.SetActive (val);
		statManager.OptionNext.SetActive (val);
		statManager.Selection.SetActive (val);
		statManager.CurrentOption.SetActive (val);
		statManager.SelectedOption.SetActive (val);
		statManager.CheckMark2.SetActive (val);

		if (OptionProgress == 0) {
			statManager.MultiplayerMap.SetActive (val);
			statManager.MultiplayerMode.SetActive (false);
			statManager.MultiplayerPlayers.SetActive (false);
			statManager.MultiplayerTime.SetActive (false);

		} else if (OptionProgress == 1) {
			statManager.MultiplayerMap.SetActive (false);
			statManager.MultiplayerMode.SetActive (val);
			statManager.MultiplayerPlayers.SetActive (false);
			statManager.MultiplayerTime.SetActive (false);
		} else if (OptionProgress == 2) {
			statManager.MultiplayerMap.SetActive (false);
			statManager.MultiplayerMode.SetActive (false);
			statManager.MultiplayerPlayers.SetActive (val);
			statManager.MultiplayerTime.SetActive (false);
		} else if (OptionProgress == 3) {
			statManager.MultiplayerMap.SetActive (false);
			statManager.MultiplayerMode.SetActive (false);
			statManager.MultiplayerPlayers.SetActive (false);
			statManager.MultiplayerTime.SetActive (val);

			if (optionsSelected == 4) {
				statManager.OptionNext.GetComponent<Image> ().color = ReadyColor;
				if (Input.GetKeyDown (KeyCode.RightArrow)) {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [3]);
					PlayerPrefs.SetInt (playerPrefsGameMapKey, MapNumber);
					PlayerPrefs.SetInt (playerPrefsGameModeKey, GameModeNumber);
					PlayerPrefs.SetInt (playerPrefsPlayerCountKey, PlayerCount);
					PlayerPrefs.SetInt (playerPrefsGameTimeKey, GameTime);
					PlayerPrefs.SetInt (startMultiplayerFromHomeBaseKey, 1);
//					StartCoroutine (StartMultiplayer_Coroutine ());
					StartMultiplayerSavePlayerPref();
				}
			}
		}

	}

	void StartMultiplayerSavePlayerPref(){
		PlayerPrefs.SetInt ("startMultiplayerFromHomeBaseKey", 1);
		PlayerPrefs.Save ();
		Debug.Log ("SAVING StartMultiplayerGame: " + PlayerPrefs.GetInt ("startMultiplayerFromHomeBaseKey"));
		nManager.SceneLoadingPanel.SetActive (true);
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);
	}


}
