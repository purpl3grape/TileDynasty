using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum enumTileDynastyMode{
	Singleplayer,
	Coop,
	Multiplayer,
	Viewrooms,
	RefreshRooms,
}

public enum enumTileDynastyMultiplayerSetting{
	Map,
	Mode,
	Roomsize,
	Days,
}

public enum enumTileDynastyMap_DONOTUSE{
	MilitaryBase,
	CampgroundsMod,
	WinterCamp,
	HomeBase,
	WasteLands,
}

public enum enumTileDynastyMapList{
	MilitaryBase,
	CampgroundsMod,
	WinterCamp,
	HomeBase,
	WasteLands,
}

public enum enumTileDynastyMultiplayerMode{
	NULL,
	TDM,
	SEARCHANDRESCUE,
}

public class CampaignTrigger_Behavior : MonoBehaviour {

	NetworkManager nManager;
	GuiManager guiManager;
	public RoomListManager singleTonRoomListManager;
	public StatsManager statManager;

	public GameObject GameOptionText;
	public GameObject GameOptionValue;
	public GameObject OnlineGameModeText;
	public GameObject OnlineGamePlayerCountText;
	public GameObject GameInstructionText;
	public GameObject GamePlayText;
	public GameObject NextOption;
	public GameObject PrevOption;
	public GameObject NextSetting;
	public GameObject PrevSetting;
	public GameObject currentRoomLabel;
	public GameObject totalRoomLabel;

	public Color OriginalColor;
	public Color ClickColor;
	public Color OpaqueInstructionColor;
	public Color InstructionColor;
	public Color OpaquePlayColor;
	public Color OpaqueRoomNameColor;
	public Color RoomNameColor;
	public Color PlayColor;
	public Color PlayReadyColor;
	public Color OpaqueGameOptionTextColor;
	public Color OpaqueGameOptionValueColor;
	public Color GameOptionTextColor;
	public Color GameOptionValueColor;
	public Color OptionColor;
	public Color OpaqueOptionColor;
	public Color SettingColor;
	public Color OpaqueSettingColor;

	public AudioClip [] ClickSound;
	[HideInInspector] public int TileDynastyGameMode = 0;
	[HideInInspector] public int TileDynastyMultiplayerMode = 0;
	[HideInInspector] public int TileDynastyGameSetting = 0;
	[HideInInspector] public int TileDynastyMap = 0;
	[HideInInspector] public int TileDynastyRoomsize = 2;
	[HideInInspector] public int TileDynastyDays = 2;
	[HideInInspector] public int viewRoomIndex = 0;
	[HideInInspector] public bool isTriggerTrue;
	[HideInInspector] public bool isChooseSettings = false;
	[HideInInspector] public bool isChooseRooms = false;
	[HideInInspector] public bool hasRefreshedRoomList = false;

	GameObject playerObject;

	private Color tempColor;
	GameObject triggerPlayer;
	Health pHealth;
	bool SinglePlayerMode = false;

	// Use this for initialization
	void Start () {
		playerObject = GameObject.FindGameObjectWithTag ("Player");
		singleTonRoomListManager = GameObject.FindGameObjectWithTag ("RoomListManager").GetComponent<RoomListManager> ();
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		guiManager = nManager.GetComponent<GuiManager> ();

		if (PlayerPrefs.GetInt ("HasRefreshedRoomList") == 0) {				
			TileDynastyGameMode = (int)enumTileDynastyMode.Singleplayer;
			statManager.CampaignMode_Text.GetComponent<Text> ().text = "SINGLE PLAYER";
			TileDynastyGameSetting = (int)enumTileDynastyMultiplayerSetting.Map;
			GameOptionText.GetComponent<Text> ().text = "";
			GameOptionValue.GetComponent<Text> ().text = "";
			OnlineGamePlayerCountText.GetComponent<Text> ().text = "";
			OnlineGameModeText.GetComponent<Text> ().text = "";			
		} else {
			PlayerPrefs.SetInt ("HasRefreshedRoomList", 0);
			PlayerPrefs.Save ();
			hasRefreshedRoomList = true;

			TileDynastyGameMode = (int)enumTileDynastyMode.Viewrooms;
			statManager.CampaignMode_Text.GetComponent<Text> ().text = "ONLINE GAMES";
			TileDynastyGameSetting = (int)enumTileDynastyMultiplayerSetting.Map;
			GameOptionText.GetComponent<Text> ().text = "";
			GameOptionValue.GetComponent<Text> ().text = "";
			OnlineGamePlayerCountText.GetComponent<Text> ().text = "";
			OnlineGameModeText.GetComponent<Text> ().text = "";			

		}

		initGameMenu ();

	}



	IEnumerator ClickColorToDeactivate_Coroutine (Image img, Color endColor){
		img.color = ClickColor;
		yield return new WaitForSeconds (0.05f);
		img.color = endColor;
	}

	IEnumerator ClickColor_Coroutine(Image img){
		img.color = ClickColor;
		yield return new WaitForSeconds (0.05f);
		img.color = OptionColor;
	}

	bool reverseColorChange = false;
	Coroutine playReadyColorCoroutine;
	IEnumerator PlayReadyColor_Coroutine(Text text){
		while (isTriggerTrue) {
			float progress = 0f; //This float will serve as the 3rd parameter of the lerp function.
			float smoothness = 0.05f;
			float reverseSmoothness = 0.1f;
			float duration = 1f;
			float increment = 0.1f;
			increment =	smoothness / duration; //The amount of change to apply.

			while (progress < 1 && !reverseColorChange) {
				text.color = Color.Lerp (PlayColor, PlayReadyColor, progress);
				progress += increment;
				yield return new WaitForSeconds (smoothness);
				if (progress >= 1)
					reverseColorChange = true;
			}
			while (progress < 1 && reverseColorChange) {
				text.color = Color.Lerp (PlayReadyColor, PlayColor, progress);
				progress += increment;
				yield return new WaitForSeconds (reverseSmoothness);
				if (progress >= 1)
					reverseColorChange = false;
			}


			yield return true;

		}
	}

	public bool hasReadInstructions = false;
	public bool deActivateTrigger = false;
	public IEnumerator HomeBaseInstructionsCoroutine;
	public IEnumerator HomeBaseInstructions_Coroutine(){

		guiManager.Msg_Obj.gameObject.SetActive (false);
		guiManager.Msg_NPC.gameObject.SetActive (false);
		guiManager.Msg_NPC.gameObject.SetActive (false);
		guiManager.Msg_Kill.gameObject.SetActive (false);
		guiManager.Msg_Death.gameObject.SetActive (false);
		guiManager.Msg_Area.gameObject.SetActive (false);
		guiManager.Msg_Ambush.gameObject.SetActive (false);
		guiManager.Msg_Item.gameObject.SetActive (false);
		guiManager.Msg_Spawn.gameObject.SetActive (false);


		deActivateTrigger = true;

		if (guiManager.SpawnMessageCoroutine != null) {
			StopCoroutine (guiManager.SpawnMessageCoroutine);
		}
		if (guiManager.CloseMessagePanelCoroutine != null) {
			StopCoroutine (guiManager.CloseMessagePanelCoroutine);
		}
		if (guiManager.MessageBuilderCoroutine != null) {
			StopCoroutine (guiManager.MessageBuilderCoroutine);
		}
		if (guiManager.MessageBuilderCoroutine2 != null) {
			StopCoroutine (guiManager.MessageBuilderCoroutine2);
		}

		hasReadInstructions = false;
		guiManager.DisplayMessagePanelCoroutine = guiManager.DisplayMessagePanel_Coroutine ();
		StartCoroutine (guiManager.DisplayMessagePanelCoroutine);

		//MESSAGE 1
		guiManager.initMessage = "PRESS ";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return new WaitForSeconds (0.02f);
		}
		guiManager.initMessage = "ENTER ";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#96FA32FF");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return new WaitForSeconds (0.02f);
		}
		guiManager.initMessage = "TO SELECT OPTIONS";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "");
		StartCoroutine (guiManager.MessageBuilderCoroutine);

		while (!hasReadInstructions) {
			if (Input.GetKey (KeyCode.Return)) {
				if (guiManager.MessageBuilderCoroutine != null) {
					StopCoroutine (guiManager.MessageBuilderCoroutine);
				}
				hasReadInstructions = true;
			}
			yield return true;
		}
		guiManager.CloseMessagePanelCoroutine = guiManager.CloseMessagePanel_Coroutine ();
		StartCoroutine (guiManager.CloseMessagePanelCoroutine);

		yield return new WaitForSeconds (.25f);


		if (guiManager.CloseMessagePanelCoroutine != null) {
			StopCoroutine (guiManager.CloseMessagePanelCoroutine);
		}
		hasReadInstructions = false;
		guiManager.DisplayMessagePanelCoroutine = guiManager.DisplayMessagePanel_Coroutine ();
		StartCoroutine (guiManager.DisplayMessagePanelCoroutine);

		//MESSAGE 2
		guiManager.initMessage = "PRESS ";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return new WaitForSeconds (0.02f);
		}
		guiManager.initMessage = "BACKSPACE ";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#96FA32FF");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return new WaitForSeconds (0.02f);
		}
		guiManager.initMessage = "TO EXIT OPTIONS";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "");
		StartCoroutine (guiManager.MessageBuilderCoroutine);

		while (!hasReadInstructions) {
			if (Input.GetKey (KeyCode.Return)) {
				if (guiManager.MessageBuilderCoroutine != null) {
					StopCoroutine (guiManager.MessageBuilderCoroutine);
				}
				hasReadInstructions = true;
			}
			yield return true;
		}
		guiManager.CloseMessagePanelCoroutine = guiManager.CloseMessagePanel_Coroutine ();
		StartCoroutine (guiManager.CloseMessagePanelCoroutine);

		yield return new WaitForSeconds (.25f);


		if (guiManager.CloseMessagePanelCoroutine != null) {
			StopCoroutine (guiManager.CloseMessagePanelCoroutine);
		}
		hasReadInstructions = false;
		guiManager.DisplayMessagePanelCoroutine = guiManager.DisplayMessagePanel_Coroutine ();
		StartCoroutine (guiManager.DisplayMessagePanelCoroutine);

		//MESSAGE 3
		guiManager.initMessage = "PRESS ";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return new WaitForSeconds (0.02f);
		}
		guiManager.initMessage = "LEFT ";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#96FA32FF");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return new WaitForSeconds (0.02f);
		}
		guiManager.initMessage = "OR ";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return new WaitForSeconds (0.02f);
		}
		guiManager.initMessage = "RIGHT ";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#96FA32FF");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return new WaitForSeconds (0.02f);
		}
		guiManager.initMessage = "TO TOGGLE OPTIONS";
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "");
		StartCoroutine (guiManager.MessageBuilderCoroutine);

		while (!hasReadInstructions) {
			if (Input.GetKey (KeyCode.Return)) {
				hasReadInstructions = true;
			}
			yield return true;
		}
		guiManager.CloseMessagePanelCoroutine = guiManager.CloseMessagePanel_Coroutine ();
		StartCoroutine (guiManager.CloseMessagePanelCoroutine);

		deActivateTrigger = false;
		HomeBaseInstructionsCoroutine = null;
	}


	void OnTriggerEnter(Collider other){
		//Againt Make this isTrigger true in all scenarios for now to give users freedom of choice (but with sound cue warning)


		if (other.CompareTag ("Player")) {

			return;

			triggerPlayer = other.gameObject;
			pHealth = triggerPlayer.GetComponent<Health> ();

			HomeBaseInstructionsCoroutine = HomeBaseInstructions_Coroutine ();
			StartCoroutine (HomeBaseInstructionsCoroutine);


			if (Steamworks.SteamFriends.GetPersonaState () == Steamworks.EPersonaState.k_EPersonaStateOffline) {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
			} else {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
			}
			NextOption.GetComponent<Image> ().color = OptionColor;
			PrevOption.GetComponent<Image> ().color = OptionColor;

			isTriggerTrue = true;

			if (hasRefreshedRoomList) {				
				TileDynastyGameMode = (int)enumTileDynastyMode.Viewrooms;
				hasRefreshedRoomList = false;
			} else {
				TileDynastyGameMode = (int)enumTileDynastyMode.Singleplayer;
			}
		}

	}
		
	void OnTriggerExit(Collider other){
		if (other.CompareTag ("Player")) {

			return;

			triggerPlayer = null;

			if (guiManager.CloseMessagePanelCoroutine == null)
				guiManager.CloseMessagePanelCoroutine = guiManager.CloseMessagePanel_Coroutine ();
			StartCoroutine (guiManager.CloseMessagePanelCoroutine);
			if (guiManager.MessageBuilderCoroutine != null) {
				StopCoroutine (guiManager.MessageBuilderCoroutine);
				guiManager.MessageBuilderCoroutine = null;
			}
			if (guiManager.MessageBuilderCoroutine2 != null) {
				StopCoroutine (guiManager.MessageBuilderCoroutine2);
				guiManager.MessageBuilderCoroutine2 = null;
			}
			if (HomeBaseInstructionsCoroutine != null) {
				StopCoroutine (HomeBaseInstructionsCoroutine);
				HomeBaseInstructionsCoroutine = null;
			}

			hasReadInstructions = false;
			deActivateTrigger = false;

			isTriggerTrue = false;
//			playerObject = null;
			initGameMenu();
		}
	}

	void initGameMenu(){
		TileDynastyGameMode = (int)enumTileDynastyMode.Singleplayer;
		statManager.CampaignMode_Text.GetComponent<Text> ().text = "SINGLE PLAYER";
		TileDynastyGameSetting = (int)enumTileDynastyMultiplayerSetting.Map;
		TileDynastyMap = (int)enumTileDynastyMapList.MilitaryBase;
		TileDynastyRoomsize = 2;
		TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.TDM;
		TileDynastyDays = 2;
		GameInstructionText.GetComponent<Text> ().color = OpaqueInstructionColor;
		if (playReadyColorCoroutine != null) {				
			StopCoroutine (playReadyColorCoroutine);
		}
		playReadyColorCoroutine = null;
		GamePlayText.GetComponent<Text> ().color = OpaquePlayColor;
		OnlineGamePlayerCountText.GetComponent<Text> ().text = "";
		OnlineGameModeText.GetComponent<Text> ().text = "";
		OnlineGamePlayerCountText.GetComponent<Text> ().color = OpaqueRoomNameColor;
		OnlineGameModeText.GetComponent<Text> ().color = OpaqueRoomNameColor;
		currentRoomLabel.GetComponent<Text> ().color = OpaqueRoomNameColor;
		totalRoomLabel.GetComponent<Text> ().color = OpaqueRoomNameColor;
		GameOptionText.GetComponent<Text> ().color = OpaqueGameOptionTextColor;
		GameOptionValue.GetComponent<Text> ().color = OpaqueGameOptionValueColor;
		NextOption.GetComponent<Image> ().color = OpaqueOptionColor;
		PrevOption.GetComponent<Image> ().color = OpaqueOptionColor;
		NextSetting.GetComponent<Image> ().color = OpaqueOptionColor;
		PrevSetting.GetComponent<Image> ().color = OpaqueOptionColor;
		TileDynastyGameSetting = (int)enumTileDynastyMultiplayerSetting.Map;
		GameOptionText.GetComponent<Text> ().text = "";
		GameOptionValue.GetComponent<Text> ().text = "";
		isChooseSettings = false;
		isChooseRooms = false;
		viewRoomIndex = 0;
	}

	void BackTrackSetting(){
		if (playReadyColorCoroutine != null) {
			StopCoroutine (playReadyColorCoroutine);
		}
		playReadyColorCoroutine = null;
		GamePlayText.GetComponent<Text> ().color = OpaquePlayColor;
		GameInstructionText.GetComponent<Text> ().color = OpaqueInstructionColor;
		OnlineGamePlayerCountText.GetComponent<Text> ().text = "";
		OnlineGameModeText.GetComponent<Text> ().text = "";
		OnlineGamePlayerCountText.GetComponent<Text> ().color = OpaqueRoomNameColor;
		OnlineGameModeText.GetComponent<Text> ().color = OpaqueRoomNameColor;
		currentRoomLabel.GetComponent<Text> ().color = OpaqueRoomNameColor;
		totalRoomLabel.GetComponent<Text> ().color = OpaqueRoomNameColor;
		GameOptionText.GetComponent<Text> ().color = OpaqueGameOptionTextColor;
		GameOptionValue.GetComponent<Text> ().color = OpaqueGameOptionValueColor;
		NextOption.GetComponent<Image> ().color = OptionColor;
		PrevOption.GetComponent<Image> ().color = OptionColor;
		NextSetting.GetComponent<Image> ().color = OpaqueOptionColor;
		PrevSetting.GetComponent<Image> ().color = OpaqueOptionColor;
		TileDynastyGameSetting = (int)enumTileDynastyMultiplayerSetting.Map;
		GameOptionText.GetComponent<Text> ().text = "";
		TileDynastyMap = (int)enumTileDynastyMapList.MilitaryBase;
		GameOptionValue.GetComponent<Text> ().text = "";
		TileDynastyRoomsize = 2;
		TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.TDM;
		TileDynastyDays = 2;
		isChooseSettings = false;
		isChooseRooms = false;
		viewRoomIndex = 0;
		statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
	}


	void SinglePlayerModeUpdate(){

		if (!isTriggerTrue)
			return;

		GameInstructionText.GetComponent<Text> ().color = InstructionColor;
		if (playReadyColorCoroutine == null)
			GamePlayText.GetComponent<Text> ().color = PlayColor;

		if ((Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.LeftArrow)) && (!isChooseSettings&& !isChooseRooms)) {
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (TileDynastyGameMode == (int)enumTileDynastyMode.Singleplayer) {
					TileDynastyGameMode = (int)enumTileDynastyMode.Coop;
				} else if (TileDynastyGameMode == (int)enumTileDynastyMode.Coop) {
					TileDynastyGameMode = (int)enumTileDynastyMode.Multiplayer;
				} else if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer) {
					TileDynastyGameMode = (int)enumTileDynastyMode.Viewrooms;
				} else if (TileDynastyGameMode == (int) enumTileDynastyMode.Viewrooms) {
					TileDynastyGameMode = (int) enumTileDynastyMode.RefreshRooms;
				} else if (TileDynastyGameMode == (int) enumTileDynastyMode.RefreshRooms) {
					TileDynastyGameMode = (int) enumTileDynastyMode.Singleplayer;
				}
				StartCoroutine (ClickColor_Coroutine (NextOption.GetComponent<Image> ()));
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (TileDynastyGameMode == (int) enumTileDynastyMode.Viewrooms) {
					TileDynastyGameMode = (int) enumTileDynastyMode.Multiplayer;
				} else if (TileDynastyGameMode == (int) enumTileDynastyMode.Multiplayer) {
					TileDynastyGameMode = (int) enumTileDynastyMode.Coop;
				} else if (TileDynastyGameMode == (int) enumTileDynastyMode.Coop) {
					TileDynastyGameMode = (int) enumTileDynastyMode.Singleplayer;
				} else if (TileDynastyGameMode == (int) enumTileDynastyMode.Singleplayer) {
					TileDynastyGameMode = (int) enumTileDynastyMode.RefreshRooms;
				} else if (TileDynastyGameMode == (int) enumTileDynastyMode.RefreshRooms) {
					TileDynastyGameMode = (int) enumTileDynastyMode.Viewrooms;
				}
				StartCoroutine (ClickColor_Coroutine (PrevOption.GetComponent<Image> ()));
			}
			statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
		}
		if (Input.GetKeyDown (KeyCode.Backspace)) {
			BackTrackSetting ();
		}

		if (TileDynastyGameMode == (int)enumTileDynastyMode.RefreshRooms) {

			statManager.CampaignMode_Text.GetComponent<Text> ().text = "REFRESH ROOM LIST";
			ChooseSettingsUpdate (TileDynastyGameMode);

			if (Input.GetKeyDown (KeyCode.Return)) {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);

				StartCoroutine (RefreshRoomList_Coroutine ());
			}
		}
		else if (TileDynastyGameMode == (int)enumTileDynastyMode.Singleplayer) {

			statManager.CampaignMode_Text.GetComponent<Text> ().text = "SINGLE PLAYER";
			ChooseSettingsUpdate (TileDynastyGameMode);

		} else if (TileDynastyGameMode == (int)enumTileDynastyMode.Coop) {

			statManager.CampaignMode_Text.GetComponent<Text> ().text = "CO-OP PLAY";

			ChooseSettingsUpdate (TileDynastyGameMode);
		}


		//MULTIPLAYER FUNCTIONALITY HAS ADDITIONAL COMPONENTS
		//1) MAP 2) PLAYERS 3) MODE 4) GAME TIME

		else if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer) {
			statManager.CampaignMode_Text.GetComponent<Text> ().text = "MULTIPLAYER";

			ChooseSettingsUpdate (TileDynastyGameMode);
		
		}





		else if(TileDynastyGameMode == (int) enumTileDynastyMode.Viewrooms) {

			isChooseSettings = false;
			statManager.CampaignMode_Text.GetComponent<Text> ().text = "ONLINE GAMES";

			ChooseRoomsUpdate ();
		}
	}


	bool stopGameOptionsCoroutine = false;
	void Update () {
		if (!deActivateTrigger) {
			SinglePlayerModeUpdate ();
		}
		if (playerObject == null) {
			playerObject = GameObject.FindGameObjectWithTag ("Player");
		}

		//TRIGGER PLAYER OBJECT
		if (triggerPlayer == null) {

			initGameMenu ();

			if (!stopGameOptionsCoroutine) {
				stopGameOptionsCoroutine = true;
				if (guiManager.CloseMessagePanelCoroutine == null)
					guiManager.CloseMessagePanelCoroutine = guiManager.CloseMessagePanel_Coroutine ();
				StartCoroutine (guiManager.CloseMessagePanelCoroutine);
				if (guiManager.MessageBuilderCoroutine != null) {
					StopCoroutine (guiManager.MessageBuilderCoroutine);
					guiManager.MessageBuilderCoroutine = null;
				}
				if (guiManager.MessageBuilderCoroutine2 != null) {
					StopCoroutine (guiManager.MessageBuilderCoroutine2);
					guiManager.MessageBuilderCoroutine2 = null;
				}
				if (HomeBaseInstructionsCoroutine != null) {
					StopCoroutine (HomeBaseInstructionsCoroutine);
					HomeBaseInstructionsCoroutine = null;
				}
			}
		} else {
			stopGameOptionsCoroutine = false;
		}
	}

	void ChooseRoomsUpdate(){

		if (!isChooseRooms) {
			if (Input.GetKeyDown (KeyCode.Return)) {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				if (!isChooseRooms) {
					StartCoroutine (ClickColorToDeactivate_Coroutine (PrevOption.GetComponent<Image> (), OpaqueOptionColor));
					StartCoroutine (ClickColorToDeactivate_Coroutine (NextOption.GetComponent<Image> (), OpaqueOptionColor));
				}
				//Display the initial room first then do the toggle functions
				if (singleTonRoomListManager.roomNameList.Length > 0 && viewRoomIndex > 0) {
					GameOptionText.GetComponent<Text> ().text = singleTonRoomListManager.roomNameList [viewRoomIndex];
					GameOptionValue.GetComponent<Text> ().text = singleTonRoomListManager.roomMapList [viewRoomIndex];
					OnlineGameModeText.GetComponent<Text> ().text = singleTonRoomListManager.roomModeList [viewRoomIndex];
					OnlineGamePlayerCountText.GetComponent<Text> ().text = singleTonRoomListManager.roomCurrentSizeList [viewRoomIndex] + "\t/\t" + singleTonRoomListManager.roomMaxSizeList [viewRoomIndex] + " PLAYERS";
					currentRoomLabel.GetComponent<Text> ().text = viewRoomIndex.ToString();
					totalRoomLabel.GetComponent<Text> ().text = singleTonRoomListManager.roomNameList.Length.ToString();
				} else {
					GameOptionText.GetComponent<Text> ().text = "NO GAMES ONLINE";
					GameOptionValue.GetComponent<Text> ().text = "CURRENTLY";
					OnlineGameModeText.GetComponent<Text> ().text = "";
					OnlineGamePlayerCountText.GetComponent<Text> ().text = "";
					currentRoomLabel.GetComponent<Text> ().text = "0";
					totalRoomLabel.GetComponent<Text> ().text = "0";
				}

				StartCoroutine (SetChooseRoomTrue_Coroutine ());
			}
		}

		if (!isChooseRooms)
			return;		


		if (singleTonRoomListManager.roomNameList.Length > 0 && viewRoomIndex < singleTonRoomListManager.roomNameList.Length) {			
			GameOptionText.GetComponent<Text> ().text = singleTonRoomListManager.roomNameList [viewRoomIndex];
			GameOptionValue.GetComponent<Text> ().text = singleTonRoomListManager.roomMapList [viewRoomIndex];
			OnlineGameModeText.GetComponent<Text> ().text = singleTonRoomListManager.roomModeList [viewRoomIndex];
			OnlineGamePlayerCountText.GetComponent<Text> ().text = singleTonRoomListManager.roomCurrentSizeList [viewRoomIndex] + "\t/\t" + singleTonRoomListManager.roomMaxSizeList [viewRoomIndex] + " PLAYERS";
			currentRoomLabel.GetComponent<Text> ().text = (viewRoomIndex + 1).ToString ();
			totalRoomLabel.GetComponent<Text> ().text = (singleTonRoomListManager.roomNameList.Length).ToString ();
		} else {
			GameOptionText.GetComponent<Text> ().text = "NO GAMES ONLINE";
			GameOptionValue.GetComponent<Text> ().text = "CURRENTLY";
			OnlineGameModeText.GetComponent<Text> ().text = "";
			OnlineGamePlayerCountText.GetComponent<Text> ().text = "";
			currentRoomLabel.GetComponent<Text> ().text = "0";
			totalRoomLabel.GetComponent<Text> ().text = "0";
		}

		if (!(currentRoomLabel.GetComponent<Text> ().color == GameOptionValueColor)) {
			OnlineGamePlayerCountText.GetComponent<Text> ().color = RoomNameColor;
			OnlineGameModeText.GetComponent<Text> ().color = RoomNameColor;
			currentRoomLabel.GetComponent<Text> ().color = GameOptionValueColor;
			totalRoomLabel.GetComponent<Text> ().color = GameOptionValueColor;
			GameOptionText.GetComponent<Text> ().color = GameOptionTextColor;
			GameOptionValue.GetComponent<Text> ().color = GameOptionValueColor;
			NextOption.GetComponent<Image> ().color = OpaqueOptionColor;
			PrevOption.GetComponent<Image> ().color = OpaqueOptionColor;
			NextSetting.GetComponent<Image> ().color = OptionColor;
			PrevSetting.GetComponent<Image> ().color = OptionColor;
		}


		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (viewRoomIndex > 0) {
				if (viewRoomIndex <= 0) {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
					viewRoomIndex -= 1;
				}
			} else {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
			}
			StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (viewRoomIndex < singleTonRoomListManager.roomNameList.Length) {
				if (viewRoomIndex >= (singleTonRoomListManager.roomNameList.Length-1)) {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [0]);
					viewRoomIndex += 1;
				}
			} else {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
			}
			StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (singleTonRoomListManager.roomNameList.Length > 0) {
				PlayerPrefs.SetString ("JoinOnlineRoomFromHomeBase", singleTonRoomListManager.roomNameList [viewRoomIndex]);
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);

				StartCoroutine (SaveViewRoomListPref_CoroutineOBSELETE ());
			} else {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [2]);
			}
			StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
			StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
		}
		

	}

	void ChooseSettingsUpdate(int GameModeOption){

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (!isChooseSettings) {
				StartCoroutine (ClickColorToDeactivate_Coroutine (PrevOption.GetComponent<Image> (), OpaqueOptionColor));
				StartCoroutine (ClickColorToDeactivate_Coroutine (NextOption.GetComponent<Image> (), OpaqueOptionColor));
			}
//			NextOption.GetComponent<Image> ().color = OpaqueOptionColor;
//			PrevOption.GetComponent<Image> ().color = OpaqueOptionColor;
			NextSetting.GetComponent<Image> ().color = OptionColor;
			PrevSetting.GetComponent<Image> ().color = OptionColor;

			GameOptionText.GetComponent<Text> ().color = GameOptionTextColor;
			GameOptionValue.GetComponent<Text> ().color = GameOptionValueColor;
			if (Steamworks.SteamFriends.GetPersonaState () == Steamworks.EPersonaState.k_EPersonaStateOffline) {
				if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer || TileDynastyGameMode == (int)enumTileDynastyMode.Coop || TileDynastyGameMode == (int)enumTileDynastyMode.Viewrooms || TileDynastyGameMode == (int)enumTileDynastyMode.RefreshRooms) {				
					BackTrackSetting ();
					StartCoroutine (ClickColorToDeactivate_Coroutine (PrevOption.GetComponent<Image> (), OptionColor));
					StartCoroutine (ClickColorToDeactivate_Coroutine (NextOption.GetComponent<Image> (), OptionColor));
					return;
				} else {
					statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				}
			} else {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
			}
			StartCoroutine (SetChooseSettingsTrue_Coroutine ());
		}

		if (!isChooseSettings)
			return;

		//SETTING: MAP
		if (TileDynastyGameSetting == (int)enumTileDynastyMultiplayerSetting.Map) {
			GameOptionText.GetComponent<Text> ().text = "MAP";

			if (TileDynastyMap == (int)enumTileDynastyMapList.MilitaryBase) {
				GameOptionValue.GetComponent<Text> ().text = "MILITARY BASE";
			} else if (TileDynastyMap == (int)enumTileDynastyMapList.WasteLands) {
				GameOptionValue.GetComponent<Text> ().text = "WASTELANDS";
			}

			//MAP --> NEXT / PREV (2 maps) Use 1 for now
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				AudioClip clipToPlay = null;
				if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer) {
					clipToPlay = TileDynastyMap == (int)enumTileDynastyMapList.MilitaryBase ? ClickSound [2] : ClickSound [0];
				} else {
					clipToPlay = TileDynastyMap == (int)enumTileDynastyMapList.MilitaryBase ? ClickSound [2] : ClickSound [2];
				}
				if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer) {
					if (TileDynastyMap == (int)enumTileDynastyMapList.WasteLands) {
						TileDynastyMap = (int)enumTileDynastyMapList.MilitaryBase;
					}
				} else {
					TileDynastyMap = (int)enumTileDynastyMapList.MilitaryBase;
				}
				StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (clipToPlay);

			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				AudioClip clipToPlay = null;
				if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer) {
					clipToPlay = TileDynastyMap == (int)enumTileDynastyMapList.WasteLands ? ClickSound [2] : ClickSound [0];
				} else {
					clipToPlay = TileDynastyMap == (int)enumTileDynastyMapList.WasteLands ? ClickSound [2] : ClickSound [2];
				}
				if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer) {
					if (TileDynastyMap == (int)enumTileDynastyMapList.MilitaryBase) {
						TileDynastyMap = (int)enumTileDynastyMapList.WasteLands;
					}
				} else {
					TileDynastyMap = (int)enumTileDynastyMapList.MilitaryBase;
				}
				StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (clipToPlay);
			}					

			//SETTING --> NEXT (MODE)
			if (Input.GetKeyDown (KeyCode.Return)) {
				StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
				StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				TileDynastyGameSetting = (int)enumTileDynastyMultiplayerSetting.Mode;
			}
		} else if (TileDynastyGameSetting == (int)enumTileDynastyMultiplayerSetting.Mode) {
			GameOptionText.GetComponent<Text> ().text = "MODE";

			if (TileDynastyGameMode == (int)enumTileDynastyMode.Coop) {
				TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE;
			} else if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer) {
				TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.TDM;
			}
			if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE) {
				GameOptionValue.GetComponent<Text> ().text = "SEARCH AND RESCUE";
			} else if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.TDM) {
				GameOptionValue.GetComponent<Text> ().text = "TEAM DEATH MATCH";
			}
				
			//MULTIPLAYER MODE --> NEXT / PREV (TDM 1 mode for now) (SO PLAY ERROR SOUND WHEN TRYING TO SWITCH TDM MODE)
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {

				AudioClip clipToPlay = null;
				if (TileDynastyGameMode == (int)enumTileDynastyMode.Singleplayer) {
					clipToPlay = TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE ? ClickSound [2] : ClickSound [0];
				} else {					
					clipToPlay = ClickSound [2];
				}
				StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (clipToPlay);
					
				if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer) {
					//MULTIPLAYER GAME MODES (LEFT ARROW)
					if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.TDM)
						TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.TDM;
				} else if (TileDynastyGameMode == (int)enumTileDynastyMode.Singleplayer) {
					//SINGLE PLAYER GAME MODES (LEFT ARROW)
					if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.TDM)
						TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE;
					if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE)
						TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE;
				} else if (TileDynastyGameMode == (int)enumTileDynastyMode.Coop) {
					//COOP GAME MODES (LEFT ARROW)
					if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE)
						TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE;
				}

			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				AudioClip clipToPlay = null;
				if (TileDynastyGameMode == (int)enumTileDynastyMode.Singleplayer) {
					clipToPlay = TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.TDM ? ClickSound [2] : ClickSound [0];
				} else {
					clipToPlay = ClickSound [2];
				}
				StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (clipToPlay);

				if (TileDynastyGameMode == (int)enumTileDynastyMode.Multiplayer) {
					//MULTIPLAYER GAME MODES (RIGHT ARROW)
					if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.TDM)
						TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.TDM;
				} else if (TileDynastyGameMode == (int)enumTileDynastyMode.Singleplayer) {
					//SINGLE PLAYER GAME MODES (RIGHT ARROW)
					if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE)
						TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.TDM;
					if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.TDM)
						TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.TDM;					
				} else if (TileDynastyGameMode == (int)enumTileDynastyMode.Coop) {
					//COOP GAME MODES
					if (TileDynastyMultiplayerMode == (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE)
						TileDynastyMultiplayerMode = (int)enumTileDynastyMultiplayerMode.SEARCHANDRESCUE;
				}

			}					

			//SETTING --> NEXT (PLAYERS)
			if (Input.GetKeyDown (KeyCode.Return)) {
				StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
				StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				TileDynastyGameSetting = (int)enumTileDynastyMultiplayerSetting.Roomsize;
			}

		} else if (TileDynastyGameSetting == (int)enumTileDynastyMultiplayerSetting.Roomsize) {
			GameOptionText.GetComponent<Text> ().text = "PLAYERS";

			if (TileDynastyGameMode == (int)enumTileDynastyMode.Singleplayer)
				TileDynastyRoomsize = 1;

			GameOptionValue.GetComponent<Text> ().text = TileDynastyRoomsize.ToString();

			//MULTIPLAYER MODE --> NEXT / PREV (TDM 1 mode for now)
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				AudioClip clipToPlay = null;
				if (TileDynastyGameMode != (int)enumTileDynastyMode.Singleplayer) {
					clipToPlay = TileDynastyRoomsize > 2 ? ClickSound [0] : ClickSound [2];
					TileDynastyRoomsize = TileDynastyRoomsize > 2 ? TileDynastyRoomsize -= 1 : 2;
				}
				else if (TileDynastyGameMode == (int)enumTileDynastyMode.Singleplayer) {
					TileDynastyRoomsize = 1;
					clipToPlay = ClickSound [2];
				}
				StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (clipToPlay);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				AudioClip clipToPlay = null;
				if (TileDynastyGameMode != (int)enumTileDynastyMode.Singleplayer) {					
					clipToPlay = TileDynastyRoomsize < 6 ? ClickSound [0] : ClickSound [2];
					TileDynastyRoomsize = TileDynastyRoomsize < 6 ? TileDynastyRoomsize += 1 : 6;
				}
				else if (TileDynastyGameMode == (int)enumTileDynastyMode.Singleplayer) {
					TileDynastyRoomsize = 1;
					clipToPlay = ClickSound [2];
				}
				StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (clipToPlay);
			}

			//SETTING --> NEXT (PLAYERS)
			if (Input.GetKeyDown (KeyCode.Return)) {
				StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
				StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				TileDynastyGameSetting = (int)enumTileDynastyMultiplayerSetting.Days;
			}

		} else if (TileDynastyGameSetting == (int)enumTileDynastyMultiplayerSetting.Days) {
			GameOptionText.GetComponent<Text> ().text = "DAYS";

			GameOptionValue.GetComponent<Text> ().text = TileDynastyDays.ToString();

			//MULTIPLAYER MODE --> NEXT / PREV (TDM 1 mode for now)
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				AudioClip clipToPlay = TileDynastyDays > 2 ? ClickSound [0] : ClickSound [2];
				TileDynastyDays = TileDynastyDays > 2 ? TileDynastyDays -= 1 : 2;
				StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (clipToPlay);				
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				AudioClip clipToPlay = TileDynastyDays < 10 ? ClickSound [0] : ClickSound [2];
				TileDynastyDays = TileDynastyDays < 10 ? TileDynastyDays += 1 : 10;			
				StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (clipToPlay);
			}

			if (playReadyColorCoroutine == null)
				playReadyColorCoroutine = StartCoroutine (PlayReadyColor_Coroutine (GamePlayText.GetComponent<Text> ()));
//			GamePlayText.GetComponent<Text> ().color = PlayReadyColor;

			if (Input.GetKeyDown (KeyCode.Return)) {
				StartCoroutine (ClickColor_Coroutine (PrevSetting.GetComponent<Image> ()));
				StartCoroutine (ClickColor_Coroutine (NextSetting.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ClickSound [1]);
				PlayerPrefs.SetInt ("playerPrefsGameMapKey", TileDynastyMap);

				PlayerPrefs.SetInt ("playerPrefsGameModeKey", TileDynastyMultiplayerMode);
				PlayerPrefs.SetInt ("playerPrefsPlayerCountKey", TileDynastyRoomsize);
				PlayerPrefs.SetInt ("playerPrefsGameTimeKey", TileDynastyDays);
				PlayerPrefs.Save ();


				if (GameModeOption == (int)enumTileDynastyMode.Multiplayer) {
					StartCoroutine (StartMultiplayerSavePlayerPref_Coroutine ());
				} else if (GameModeOption == (int)enumTileDynastyMode.Singleplayer) {
					StartCoroutine (SaveCampaignGamePlayerPrefAndJoin_Coroutine (1));
				} else if (GameModeOption == (int)enumTileDynastyMode.Coop) {
					StartCoroutine (SaveCampaignGamePlayerPrefAndJoin_Coroutine (2));
				}
			}
		}
	}


	IEnumerator SetChooseSettingsTrue_Coroutine(){
		yield return new WaitForSeconds (0.05f);
		isChooseSettings = true;
	}

	IEnumerator SetChooseRoomTrue_Coroutine(){
		yield return new WaitForSeconds (0.05f);
		isChooseRooms = true;
	}

	IEnumerator StartMultiplayerSavePlayerPref_Coroutine(){

		playerObject.GetComponent<PlayerShooting> ().enabled = false;
		playerObject.GetComponent<PlayerMovement> ().enabled = false;

		PlayerPrefs.SetInt ("startMultiplayerFromHomeBaseKey", 1);
		PlayerPrefs.Save ();
		nManager.SceneLoadingPanel.SetActive (true);
		yield return new WaitForSeconds (1f);
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);
	}
		


	IEnumerator SaveViewRoomListPref_Coroutine(){

		playerObject.GetComponent<PlayerShooting> ().enabled = false;
		playerObject.GetComponent<PlayerMovement> ().enabled = false;

		PlayerPrefs.SetInt ("ViewRoomList", 1);
		PlayerPrefs.Save ();
		Debug.Log ("View Room List Player Pref saved: " + PlayerPrefs.GetInt ("ViewRoomList"));
		nManager.SceneLoadingPanel.SetActive (true);
		yield return new WaitForSeconds (1f);
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);

		yield return true;
	}


	IEnumerator SaveViewRoomListPref_CoroutineOBSELETE(){
		
		playerObject.GetComponent<PlayerShooting> ().enabled = false;
		playerObject.GetComponent<PlayerMovement> ().enabled = false;

		PlayerPrefs.SetInt ("ViewRoomList", 1);
		PlayerPrefs.Save ();
		Debug.Log ("View Room List Player Pref saved: " + PlayerPrefs.GetInt ("ViewRoomList"));
		nManager.SceneLoadingPanel.SetActive (true);
		yield return new WaitForSeconds (1f);
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);

		yield return true;
	}

	IEnumerator RefreshRoomList_Coroutine(){


		playerObject.GetComponent<PlayerShooting> ().enabled = false;
		playerObject.GetComponent<PlayerMovement> ().enabled = false;

		PlayerPrefs.SetInt ("RefreshRoomList", 1);
		PlayerPrefs.Save ();
		nManager.SceneLoadingPanel.SetActive (true);
		yield return new WaitForSeconds (1f);
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);
		yield return true;

	}

	IEnumerator SaveCampaignGamePlayerPrefAndJoin_Coroutine(int CampaignMode){

		playerObject.GetComponent<PlayerShooting> ().enabled = false;
		playerObject.GetComponent<PlayerMovement> ().enabled = false;

		PlayerPrefs.SetInt ("BeginChallengeFromHomeBase", CampaignMode);
		PlayerPrefs.Save ();
		Debug.Log ("Begin Challenge Player Pref saved: " + PlayerPrefs.GetInt ("BeginChallengeFromHomeBase"));
		nManager.SceneLoadingPanel.SetActive (true);
		yield return new WaitForSeconds (1f);
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);

		yield return true;
	}
}
