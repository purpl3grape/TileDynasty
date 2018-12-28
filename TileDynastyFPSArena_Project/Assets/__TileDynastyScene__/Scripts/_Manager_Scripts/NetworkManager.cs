using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Photon;

public class NetworkManager : PunBehaviour {

	KeyBindingManager kMgr;

	WaitForSeconds waitFor0_1 = new WaitForSeconds(0.1f);
	WaitForSeconds waitFor0_2 = new WaitForSeconds(0.2f);
	WaitForSeconds waitFor0_5 = new WaitForSeconds(0.5f);
	WaitForSeconds waitFor1 = new WaitForSeconds(1);
	WaitForSeconds waitFor9999 = new WaitForSeconds(9999);

	public GameObject SceneLoadingPanel;
	public GameObject TeamClassPanel;
	public GameObject SettingsPanel;
	public GameObject LeaderBoardPanel;
	public GameObject StandbyPanel;
	public GameObject InGamePanel;
	public GameObject PlayerInGamePanel;
	public GameObject ChatPanel;
	public GameObject DisplayChatPanel;
	public GameObject KickPlayerPanel;
	public GameObject KeyInputPanel;
	public GameObject PS3MovementLabel;

	PlayerInGamePanel PlayerInGamePanelInputs;

	public ControllerCanvasManager controllerCanvas;
	private GuiManager guiManager;
	private ScoringManager scoringManger;
	private WindowManager windowManager;
	private MatchTimer mTimer;
	private SettingsManager settingsMgr;
	private ControlInputManager cInputManager;

	private string GameVersionName = "TileDynasty";

	public GameObject standbyCamera;
	public GameObject deathCam;
	private GameObject[] PlayerSpawnSpots;
	private GameObject[] CivilianSpawnSpots;
	private GameObject[] BluePlayerSpawnSpots;
	private GameObject[] RedPlayerSpawnSpots;
	private GameObject[] HostageSpawnSpots;
	private GameObject[] EnemySpawnSpots;
	private GameObject[] ZombieSpawnSpots;
	private GameObject[] PatrolNodes;
	private string _PlayerName = string.Empty;
	[HideInInspector] public GameObject PlayerObject;
	public GameObject playerPrefab;
	[HideInInspector] public float clientPredVal = 0f;
	[HideInInspector] public float enemyPredVal = 0f;
	[HideInInspector] public float velocityPredVal = 0f;
	[HideInInspector] public float syncDisnceVal = 1f;
	[HideInInspector] public float musicVolume = 0f;
	[HideInInspector] public bool requestRespawnPlayer = false;
	[HideInInspector] public bool requestFreshSpawn = false;
	[HideInInspector] public string pendingName = string.Empty;
	[HideInInspector] public bool usePS3Controls = false;
	[HideInInspector] public GameObject[] blueEnemyList = new GameObject[20];
	[HideInInspector] public GameObject[] redEnemyList = new GameObject[20];
	[HideInInspector] public GameObject[] survivalEnemyList = new GameObject[60];
	[HideInInspector] public GameObject[] baseEnemyList = new GameObject[60];
	[HideInInspector] public GameObject[] hostageList = new GameObject[10];
	[HideInInspector] public GameObject[] civilianList = new GameObject[10];
	[HideInInspector] public GameObject[] zombieList = new GameObject[50];
	private int currsceneIndex;

	private List<string> chatMessages;
	private int maxChatMessages = 10;
	public Color orangeColor = new Color (250f, 150f, 50f, 155f);
	public Color magentaColor = new Color (255f, 50f, 150f, 155f);
	public Color tealColor = new Color (50f, 200f, 200f, 155f);

	public int blueBotCount = 0;
	public int redBotCount = 0;
	public int BotSpawnDistance = 150;
	public int BotSpeed = 15;
	private bool duplicateName = false;
	private bool _hasPickedTeam = false;
	public bool hasPickedTeam {
		get{ return this._hasPickedTeam; }
		set{ this._hasPickedTeam = value; }
	}

	private bool _isRoomCreated = false;
	public bool isRoomCreated {
		get{ return this._isRoomCreated; }
		set{ this._isRoomCreated = value; }
	}

	private int _teamSelID = -1;
	public int localPlayerTeamID {
		get{ return _teamSelID; }
		set{ _teamSelID = value; }
	}
	private int classSelID = -1;
	private int _primaryWeap = 0;
	public int primaryWeap{
		get{ return _primaryWeap; }
		set{ this._primaryWeap = value; }
	}

	private float dupeCount;
	private int _CoopTeamAvailibility = 8;
	private int _blueTeamAvailibility = 4;
	private int _redTeamAvailibility = 4;

	public int CoopTeamAvailibility { get { return _CoopTeamAvailibility; } }
	public int BlueTeamAvailibility { get { return _blueTeamAvailibility; } }
	public int RedTeamAvailibility { get { return _redTeamAvailibility; } }

	private bool _EnableChat = false;
	public bool EnableChat {
		get{ return _EnableChat; }
		set{ _EnableChat = value; }
	}

	private bool _DisplayChatEnabled = true;
	public bool DisplayChatEnabled {
		get{ return _DisplayChatEnabled; }
		set{ _DisplayChatEnabled = value; }
	}

	private string textMessage = "Say something...";

		
	void Awake(){

		SettingsPanel.SetActive (false);
		StandbyPanel.SetActive (false);
		InGamePanel.SetActive (false);
		PlayerInGamePanel.SetActive (false);
		ChatPanel.SetActive (false);
		DisplayChatPanel.SetActive (false);
		deathCam.SetActive (false);
		KickPlayerPanel.SetActive (false);
		KickPlayerPanel.GetComponent<KickPlayerPanelInput> ().Text_DuplicateName.text = string.Empty;


		//IF MASTER CLIENT AUTOMATICALLY GO TO TEAM CLASS PANEL OTHERWISE,
		//- NON-MASTER CLIENTS WAIT FOR SCENE SYNC TO HAPPEN IN GUIMANAGER THEN TEAMCLASS PANEL IS ACTIVATED FOR THEM
		if(!PhotonNetwork.isMasterClient)
			TeamClassPanel.SetActive (false);

		chatMessages = new List<string> ();

		PhotonNetwork.UseRpcMonoBehaviourCache = true;
	}

	void Start(){

		if (GameObject.FindGameObjectWithTag ("MapGenerator") == null) {			
			Init ();
		}

	}

	bool isInit = false;
	[HideInInspector] public int roomSize = 0;
	public void Init()
	{
		Debug.Log ("INIT");

		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();

		standbyCamera.SetActive (true);

		guiManager = GetComponent<GuiManager> ();
		scoringManger = GetComponent<ScoringManager> ();
		windowManager = GameObject.FindObjectOfType<WindowManager> ();
		mTimer = GetComponent<MatchTimer> ();
		settingsMgr = GetComponent<SettingsManager> ();
		cInputManager = GetComponent<ControlInputManager> ();

		PlayerInGamePanelInputs = PlayerInGamePanel.GetComponent<PlayerInGamePanel> ();


//		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
		PlayerSpawnSpots = GameObject.FindGameObjectsWithTag("SpawnSpotPlayer");
		BluePlayerSpawnSpots = GameObject.FindGameObjectsWithTag("SpawnSpotBlue");
		RedPlayerSpawnSpots = GameObject.FindGameObjectsWithTag("SpawnSpotRed");
		HostageSpawnSpots = GameObject.FindGameObjectsWithTag("SpawnSpotHostage");
		CivilianSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotCivilian");
		EnemySpawnSpots = GameObject.FindGameObjectsWithTag("SpawnSpotEnemy");
		ZombieSpawnSpots = GameObject.FindGameObjectsWithTag("SpawnSpotZombie");

		PatrolNodes = GameObject.FindGameObjectsWithTag ("PatrolNode");
		pendingName = PlayerPrefs.GetString("Alias", "Alias");


		clientPredVal = PlayerPrefs.HasKey ("ClientPrediction") == true ? PlayerPrefs.GetFloat ("ClientPrediction") : 8;
		enemyPredVal = PlayerPrefs.HasKey ("EnemyPrediction") == true ? PlayerPrefs.GetFloat ("EnemyPrediction") : 15;
		velocityPredVal = PlayerPrefs.HasKey ("VelocityPrediction") == true ? PlayerPrefs.GetFloat ("VelocityPrediction") : 0.1f;
		syncDisnceVal = PlayerPrefs.HasKey ("SyncDistance") == true ? PlayerPrefs.GetFloat ("SyncDistance") : 0f;
		Set_ClientPrediction ();
		Set_EnemyPrediction ();
		Set_VelocityPrediction ();
		Set_Volume ();
		cInputManager.Initialize_KeyInputPanel ();

		classSelID = 1;

		if(SceneManager.sceneCount < 1)
		{
			Debug.LogError("Configuration error: You have not yet added any scenes to your buildsettings. The current scene should be preceded by the mainmenu scene. Please see the README file for instructions on setting up the buildsettings.");
			return;
		}

		//Connect to the main photon server. This is the only IP and port we ever need to set(!)
		if (!PhotonNetwork.connected || PhotonNetwork.room == null)
		{
			//			Application.LoadLevel(0);
			SceneManager.LoadScene(0);
			return;
		}

		PhotonNetwork.isMessageQueueRunning = true;


		//BASED ON ROOM SIZE WE WILL SPLIT UP (2 TEAMS, SO DIVIDE BY 2 AND FLOOR THE VALUE)
		initChatMsgs ();
		if (PhotonNetwork.isMasterClient) {
			if (_isRoomCreated == false) {
				AddChatMessage ("LOCATION:\t\t" + PhotonNetwork.room.CustomProperties [RoomProperties.Region].ToString().ToUpper());
				AddChatMessage ("ROOM:\t\t" + PhotonNetwork.room.CustomProperties [RoomProperties.Room].ToString().ToUpper());
				AddChatMessage ("MAP:\t\t" + PhotonNetwork.room.CustomProperties [RoomProperties.Map].ToString().ToUpper());
				AddChatMessage ("MODE:\t\t" + PhotonNetwork.room.CustomProperties [RoomProperties.GameMode].ToString().ToUpper());
				AddChatMessage ("MASTER CLIENT:\t" + PhotonNetwork.masterClient.NickName.ToUpper());
				AddChatMessage ("MAX PLAYERS:\t" + PhotonNetwork.room.MaxPlayers);
				_isRoomCreated = true;
			}
		}

		roomSize = PhotonNetwork.room.MaxPlayers;


		if (PhotonNetwork.offlineMode) {
			if(!_hasPickedTeam)
				TeamClassPanel.SetActive (true);
		}

		TeamClassPanel.GetComponent<TeamButtonList>().Message_Team.text = string.Empty;
		TeamClassPanel.GetComponent<TeamButtonList>().Message_PrimaryWeapon.text = string.Empty;
		TeamClassPanel.GetComponent<TeamButtonList>().Message_Class.text = string.Empty;
		TeamClassPanel.GetComponent<TeamButtonList>().Button_Spawn.SetActive (false);

		if (!usePS3Controls) {
			PS3MovementLabel.GetComponent<Text> ().text = "P.C.";
		} else {
			PS3MovementLabel.GetComponent<Text> ().text = "PS3";
		}

		if (SettingsPanel.GetComponent<Slider> () != null) {
			SettingsPanel.GetComponent<Slider> ().value = 60;
		}
		StartCoroutine (TeamSelectionProcess_Coroutine ());
		StartCoroutine(ScanTeamMembers_Coroutine());
		StartCoroutine(PlayerRespawn_Coroutine ());
		StartCoroutine (DisplayChatMessages_Coroutine ());

		initBotSpawnDistance ();
		initBotSpeed ();

		isInit = true;
	}
		
	void GameModeRequest_ChallengeModePlayerPrefReset(){
		if (PlayerPrefs.HasKey ("BeginChallengeFromHomeBase")) {
			if (PlayerPrefs.GetInt ("BeginChallengeFromHomeBase") != 0) {
				PlayerPrefs.SetInt ("BeginChallengeFromHomeBase", 0);
				PlayerPrefs.Save ();
			}
		}
		if (PlayerPrefs.HasKey ("startMultiplayerFromHomeBaseKey")) {
			if (PlayerPrefs.GetInt ("startMultiplayerFromHomeBaseKey") != 0) {
				PlayerPrefs.SetInt ("startMultiplayerFromHomeBaseKey", 0);
				PlayerPrefs.Save ();
			}
		}
	}

	IEnumerator ScanTeamMembers_Coroutine(){
		while (true) {
			if (!mTimer.isReady) {
				TeamMember[] teamMemberList = GameObject.FindObjectsOfType<TeamMember> ();
				int bTeamCount = 0, rTeamCount = 0;
				foreach (TeamMember tM in teamMemberList) {
					bTeamCount = tM.teamID == 1 ? bTeamCount + 1 : bTeamCount;
					rTeamCount = tM.teamID == 2 ? rTeamCount + 1 : rTeamCount;

				}
				int blueAvailableCount = 0;
				int redAvailableCount = 0;
				blueAvailableCount = (int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount];
				redAvailableCount = (int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount];

				_blueTeamAvailibility = Mathf.Max (1, (int)Mathf.Floor (PhotonNetwork.room.MaxPlayers / 2)) - bTeamCount;
				PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount] = _blueTeamAvailibility;

				_redTeamAvailibility = Mathf.Max (1, (int)Mathf.Floor (PhotonNetwork.room.MaxPlayers / 2)) - rTeamCount;
				PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount] = _redTeamAvailibility;
//				Debug.Log ("red: " + (int)PhotonNetwork.room.customProperties [RoomProperties.redAvailableCount] + ", blue: " + (int)PhotonNetwork.room.customProperties [RoomProperties.blueAvailableCount]);
			}
			yield return waitFor0_5;
		}
	}


	public void setPS3Controls(){
		usePS3Controls = !usePS3Controls;
		if (!usePS3Controls) {
			PS3MovementLabel.GetComponent<Text> ().text = "DISABLED";
		} else {
			PS3MovementLabel.GetComponent<Text> ().text = "ENABLED";
		}
	}

	public void requestBlueBotCount(float val){
		blueBotCount = (int)val;
	}
	public void requestRedBotCount(float val){
		redBotCount = (int)val;
	}

	public void setBlueBotCount(float val){
		blueBotCount = (int)val;
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_BlueBotCount.GetComponent<Text> ().text = blueBotCount.ToString ();
	}
	public void setRedBotCount(float val){
		redBotCount = (int)val;
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_RedBotCount.GetComponent<Text> ().text = redBotCount.ToString ();
	}
	public void initBotSpawnDistance(){
		if (!PlayerPrefs.HasKey ("BotSpawnDistance")) {
			PlayerPrefs.SetInt ("BotSpawnDistance", 125);
			PlayerPrefs.Save ();
		}
		BotSpawnDistance = (int)PlayerPrefs.GetInt ("BotSpawnDistance");
		SettingsPanel.GetComponent<SettingsPanelInput> ().ValueBotSpawnDistance.GetComponent<Text> ().text = BotSpawnDistance.ToString ();
	}
	public void initBotSpeed(){
		if (!PlayerPrefs.HasKey ("BotSpeed")) {
			PlayerPrefs.SetInt ("BotSpeed", 15);
			PlayerPrefs.Save ();
		}
		BotSpeed = (int)PlayerPrefs.GetInt ("BotSpeed");
		SettingsPanel.GetComponent<SettingsPanelInput> ().ValueBotSpeed.GetComponent<Text> ().text = BotSpeed.ToString ();
	}
	public void SetChatMessageInput(string val){
		textMessage = val;
	}

	public void Button_CancelChat(){
		PlayerInGamePanelInputs.isChatEnabled = false;
		PlayerInGamePanelInputs.doNormalCrossHair ();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		ChatPanel.GetComponent<ChatPanelInput> ().Input_Message.text = "SAY SOMETHING...";

		_EnableChat = false;
	}

	public void Button_SendChat(){
		PlayerInGamePanelInputs.isChatEnabled = false;
		PlayerInGamePanelInputs.doNormalCrossHair ();
		AddChatMessage (PhotonNetwork.player.NickName.ToUpper() + ":\t" + textMessage);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		ChatPanel.GetComponent<ChatPanelInput> ().Input_Message.text = "SAY SOMETHING...";

		_EnableChat = false;
	}

	public void DisableKeyInputPanel(){
		KeyInputPanel.SetActive (false);
		cInputManager.KeyInputPageNumber = 0;
		hasInitializedKeyInputPanel = false;
		GameModeRequest_ChallengeModePlayerPrefReset ();
	}
		
	public void EnableKeyInputPanel() {
		KeyInputPanel.SetActive(true);
	}

	public void PauseGame() {
		//SETTINGS Panel True
		SettingsPanel.SetActive(true);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

	}

	public void DisableChatDisplay(){
		 DisplayChatPanel.SetActive (false);
		_DisplayChatEnabled = false;
	}

	bool chatPanelDisplayed = false;
	public void ToggleChatDisplay(){
		if (!chatPanelDisplayed) {
			DisplayChatPanel.SetActive (true);
			_DisplayChatEnabled = true;
			chatPanelDisplayed = true;
		} else {
			DisplayChatPanel.SetActive (false);
			_DisplayChatEnabled = false;
			chatPanelDisplayed = false;
		}
	}

	public void BackToLobby(){

		if (guiManager.currentGameMode == 99) {
			//Steamworks.SteamUserStats.ResetAllStats (true);
			//Steamworks.SteamUserStats.StoreStats ();
			SceneLoadingPanel.SetActive (true);
			scoringManger.RemovePlayer (PhotonNetwork.player.NickName);
			PhotonNetwork.Disconnect();
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			//		Application.LoadLevel(0);
			SceneManager.LoadScene(0);
			return;
		}

		SceneLoadingPanel.SetActive (true);
		scoringManger.RemovePlayer (PhotonNetwork.player.NickName);
		PhotonNetwork.Disconnect();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		//		Application.LoadLevel(0);
		SceneManager.LoadScene(0);
	}

	public void ReturnToGame(){
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		cInputManager.MainMenuPageNumber = 0;
		SettingsPanel.SetActive (false);
	}

	public void QuitGame(){
		Application.Quit ();
	}

	public void SupportMe(){
		Application.OpenURL ("http://store.steampowered.com/app/524350/TileDynasty_FPS_Arena/");
		//Application.OpenURL ("mailto:purpl3grape@gmail.com?subject=Feedback and Support");
	}


	//CLIENT PREDICTION BUTTONS
	public void Button_ClientPredictionIncreaseMethod(){
		if (clientPredVal < 100) {
			clientPredVal += 1;
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_ClientPrediction.GetComponent<Text> ().text = clientPredVal.ToString ();
		PlayerPrefs.SetFloat ("ClientPrediction", clientPredVal);
		PlayerPrefs.Save ();
	}	
	public void Button_ClientPredictionDecreaseMethod(){
		if (clientPredVal > 1) {
			clientPredVal -= 1;
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_ClientPrediction.GetComponent<Text> ().text = clientPredVal.ToString ();
		PlayerPrefs.SetFloat ("ClientPrediction", clientPredVal);
		PlayerPrefs.Save ();
	}	
	public void Set_ClientPrediction(){
		clientPredVal = PlayerPrefs.GetFloat ("ClientPrediction");
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_ClientPrediction.GetComponent<Text> ().text = clientPredVal.ToString ();
	}	

	//ENEMY PREDICTION BUTTONS
	public void Button_EnemyPredictionIncreaseMethod(){
		if (enemyPredVal < 100) {
			enemyPredVal += 1;
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_EnemyPrediction.GetComponent<Text> ().text = enemyPredVal.ToString ();
		PlayerPrefs.SetFloat ("EnemyPrediction", enemyPredVal);
		PlayerPrefs.Save ();
	}	
	public void Button_EnemyPredictionDecreaseMethod(){
		if (enemyPredVal > 1) {
			enemyPredVal -= 1;
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_EnemyPrediction.GetComponent<Text> ().text = enemyPredVal.ToString ();
		PlayerPrefs.SetFloat ("EnemyPrediction", enemyPredVal);
		PlayerPrefs.Save ();
	}	
	public void Set_EnemyPrediction(){
		enemyPredVal = PlayerPrefs.GetFloat ("EnemyPrediction");
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_EnemyPrediction.GetComponent<Text> ().text = enemyPredVal.ToString ();
	}	

	//SYNC DISTANCE BUTTONS
	public void Button_SyncDistanceIncreaseMethod(){
		if (syncDisnceVal < 100) {
			syncDisnceVal += 1;
		}
		if (syncDisnceVal >= 100) {
			syncDisnceVal = 100;
		}		
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_SyncDistance.GetComponent<Text> ().text = syncDisnceVal.ToString ();
		PlayerPrefs.SetFloat ("SyncDistance", syncDisnceVal);
		PlayerPrefs.Save ();
	}

	public void Button_SyncDistanceDecreaseMethod(){
		if (syncDisnceVal > 0) {
			syncDisnceVal -= 1;
		}
		if (syncDisnceVal <= 0) {
			syncDisnceVal = 0;
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_SyncDistance.GetComponent<Text> ().text = syncDisnceVal.ToString ();
		PlayerPrefs.SetFloat ("SyncDistance", syncDisnceVal);
		PlayerPrefs.Save ();
	}

	//VELOCITY PREDICTION BUTTONS
	public void Button_VelocityPredictionIncreaseMethod(){
		if (velocityPredVal < 1) {
			velocityPredVal += 0.01f;
		}
		if (velocityPredVal >= 1)
			velocityPredVal = 1;
		
		int val	= Mathf.RoundToInt (velocityPredVal * 100);
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_VelocityPrediction.GetComponent<Text> ().text = val.ToString ();
		PlayerPrefs.SetFloat ("VelocityPrediction", velocityPredVal);
		PlayerPrefs.Save ();
	}	
	public void Button_VelocityPredictionDecreaseMethod(){
		if (velocityPredVal > 0) {
			velocityPredVal -= 0.01f;
		}
		if (velocityPredVal <= 0)
			velocityPredVal = 0;
		
		int val	= Mathf.RoundToInt (velocityPredVal * 100);
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_VelocityPrediction.GetComponent<Text> ().text = val.ToString ();
		PlayerPrefs.SetFloat ("VelocityPrediction", velocityPredVal);
		PlayerPrefs.Save ();
	}	
	public void Set_VelocityPrediction(){
		velocityPredVal = PlayerPrefs.GetFloat ("VelocityPrediction");
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_VelocityPrediction.GetComponent<Text> ().text = velocityPredVal.ToString ();
	}	
		
	//VOLUME BUTTONS
	public void Button_VolumeOff(){
		musicVolume = 0;
		GetComponent<AudioSource> ().volume = musicVolume;
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_MusicVolume.GetComponent<Text> ().text = "0";
		PlayerPrefs.SetFloat ("MusicVolume", musicVolume);
		PlayerPrefs.Save ();
	}

	public void Button_VolumeOn(){
		musicVolume = 1;
		GetComponent<AudioSource> ().volume = musicVolume;
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_MusicVolume.GetComponent<Text> ().text = "100";
		PlayerPrefs.SetFloat ("MusicVolume", musicVolume);
		PlayerPrefs.Save ();
	}

	public void Button_VolumeIncreaseMethod(){
		if (musicVolume < 1) {
			musicVolume += 0.01f;
		}
		if (musicVolume >= 1)
			musicVolume = 1;

		GetComponent<AudioSource> ().volume = musicVolume;
		int val = Mathf.RoundToInt(musicVolume * 100);
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_MusicVolume.GetComponent<Text> ().text = val.ToString ();
		PlayerPrefs.SetFloat ("MusicVolume", musicVolume);
		PlayerPrefs.Save ();
	}	
	public void Button_VolumeDecreaseMethod(){
		if (musicVolume > 0) {
			musicVolume -= 0.01f;
		}
		if (musicVolume <= 0)
			musicVolume = 0;
		
		GetComponent<AudioSource> ().volume = musicVolume;
		int val = Mathf.RoundToInt(musicVolume * 100);
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_MusicVolume.GetComponent<Text> ().text = val.ToString ();
		PlayerPrefs.SetFloat ("MusicVolume", musicVolume);
		PlayerPrefs.Save ();
	}	
	public void Set_Volume(){
		musicVolume = PlayerPrefs.GetFloat ("MusicVolume");
		GetComponent<AudioSource> ().volume = musicVolume;
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_MusicVolume.GetComponent<Text> ().text = musicVolume.ToString ();
	}




	//BLUE BOT BUTTONS
	public void Button_BlueBotIncreaseMethod(){
		if (blueBotCount < 3) {
			blueBotCount += 1;
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_BlueBotCount.GetComponent<Text> ().text = blueBotCount.ToString ();
	}	
	public void Button_BlueBotDecreaseMethod(){
		if (blueBotCount > 0) {
			blueBotCount -= 1;
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_BlueBotCount.GetComponent<Text> ().text = blueBotCount.ToString ();
	}	

	//RED BOT BUTTONS
	public void Button_RedBotIncreaseMethod(){
		if (redBotCount < 3) {
			redBotCount += 1;
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_RedBotCount.GetComponent<Text> ().text = redBotCount.ToString ();
	}	
	public void Button_RedBotDecreaseMethod(){
		if (redBotCount > 0) {
			redBotCount -= 1;
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().Value_RedBotCount.GetComponent<Text> ().text = redBotCount.ToString ();
	}	

	public void Button_BotSpawnDistanceIncrease(){
		if (BotSpawnDistance >= 200) {
			BotSpawnDistance = 200;
		}
		if (BotSpawnDistance < 200) {
			BotSpawnDistance += 1;
			PlayerPrefs.SetInt ("BotSpawnDistance", BotSpawnDistance);
			PlayerPrefs.Save ();
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().ValueBotSpawnDistance.GetComponent<Text> ().text = BotSpawnDistance.ToString ();
	}
	public void Button_BotSpawnDistanceDecrease(){
		if (BotSpawnDistance <= 0) {
			BotSpawnDistance = 0;
		}
		if (BotSpawnDistance > 0) {
			BotSpawnDistance -= 1;
			PlayerPrefs.SetInt ("BotSpawnDistance", BotSpawnDistance);
			PlayerPrefs.Save ();
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().ValueBotSpawnDistance.GetComponent<Text> ().text = BotSpawnDistance.ToString ();
	}

	public void Button_BotSpeedIncrease(){
		if (BotSpeed >= 30) {
			BotSpeed = 30;
		}
		if (BotSpeed < 30) {
			BotSpeed += 1;
			PlayerPrefs.SetInt ("BotSpeed", BotSpeed);
			PlayerPrefs.Save ();
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().ValueBotSpeed.GetComponent<Text> ().text = BotSpeed.ToString ();
	}
	public void Button_BotSpeedDecrease(){
		if (BotSpeed <= 5) {
			BotSpeed = 5;
		}
		if (BotSpeed > 5) {
			BotSpeed -= 1;
			PlayerPrefs.SetInt ("BotSpeed", BotSpeed);
			PlayerPrefs.Save ();
		}
		SettingsPanel.GetComponent<SettingsPanelInput> ().ValueBotSpeed.GetComponent<Text> ().text = BotSpeed.ToString ();
	}

	//TEAM SELECTION PANEL:: (TEAM -> PRIMARY WEAPON -> CLASS -> SPAWN)
	public void Button_TeamSelCoop(){
		_teamSelID = 0;
		TeamClassPanel.GetComponent<TeamButtonList>().Message_Team.text = "RENEGAGE";
		TeamClassPanel.GetComponent<TeamButtonList> ().Message_Team.color = orangeColor;
	}
	public void Button_TeamSelBlue(){
		_teamSelID = 1;
		TeamClassPanel.GetComponent<TeamButtonList>().Message_Team.text = "BLUE";
		TeamClassPanel.GetComponent<TeamButtonList> ().Message_Team.color = tealColor;
	}
	public void Button_TeamSelRed(){
		_teamSelID = 2;
		TeamClassPanel.GetComponent<TeamButtonList>().Message_Team.text = "RED";
		TeamClassPanel.GetComponent<TeamButtonList> ().Message_Team.color = magentaColor;
	}


	public void Button_Spawn(){
		Spawn_TeamClass (_teamSelID, classSelID);
		scoringManger.GetComponent<PhotonView>().RPC("ChangeScore",PhotonTargets.AllBuffered, PhotonNetwork.player.NickName, "team", _teamSelID);
	}

	public void Button_ClassSelTank(){
		classSelID = 1;
		TeamClassPanel.GetComponent<TeamButtonList>().Message_Class.text = "Defender";
	}
	public void Button_ClassSelScout(){
		classSelID = 2;
		TeamClassPanel.GetComponent<TeamButtonList>().Message_Class.text = "Scout";
	}

	public void Button_PrimaryWeapMG(){
		_primaryWeap = 0;	//WeapSelected=3=Rail PlayerShooting Script
		TeamClassPanel.GetComponent<TeamButtonList>().Message_PrimaryWeapon.text = "Machine Gun";
	}
	public void Button_PrimaryWeapHMG(){
		_primaryWeap = 1;	//WeapSelected=1=HMG (QuadAmmo) PlayerShooting Script
		TeamClassPanel.GetComponent<TeamButtonList>().Message_PrimaryWeapon.text = "Heavy Machine Gun";
	}
	public void Button_PrimaryWeapRocket(){
		_primaryWeap = 2;	//WeapSelected=3=Rocket PlayerShooting Script
		TeamClassPanel.GetComponent<TeamButtonList>().Message_PrimaryWeapon.text = "Rocket";
	}
	public void Button_PrimaryWeapRail(){
		_primaryWeap = 3;	//WeapSelected=3=Rail PlayerShooting Script
		TeamClassPanel.GetComponent<TeamButtonList>().Message_PrimaryWeapon.text = "Rail";
	}
	public void Button_PrimaryWeapGrenade(){
		_primaryWeap = 4;	//WeapSelected=3=Grenade PlayerShooting Script
		TeamClassPanel.GetComponent<TeamButtonList>().Message_PrimaryWeapon.text = "Grenade";
	}

	public void Spawn_TeamClass(int team_ID, int class_ID){
		GameObject GO = SpawnMyPlayer (playerPrefab.name, team_ID, class_ID);
		AddChatMessage (_PlayerName.ToUpper() + " JOINED THE GAME...");

		//initialize kills and deaths stats to 0 upon first spawning into server;
		if (duplicateName == false) {
			scoringManger.GetComponent<PhotonView> ().RPC ("SetScore", PhotonTargets.AllBuffered, _PlayerName, "kills", 0);
			scoringManger.GetComponent<PhotonView> ().RPC ("SetScore", PhotonTargets.AllBuffered, _PlayerName, "deaths", 0);
		}
		if (PhotonNetwork.isMasterClient) {
			mTimer.setMasterClientSpawned (true);
		}
		TeamClassPanel.SetActive(false);
		controllerCanvas.backgroundImage.SetActive (false);
	}

	GameObject SpawnMyPlayer(string playerPrefabName, int teamID, int classID) {
		this._teamSelID = teamID;
		this.classSelID = classID;
		_hasPickedTeam = true;

		if (!SettingsPanel.GetActive ()) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		if (PlayerSpawnSpots == null) {
			Debug.LogError ("NO SPAWNSPOTS!");
			GameObject EmptyGO = null;
			return EmptyGO;
		}

		GameObject mySpawnSpot = PlayerSpawnSpots [Random.Range (0, PlayerSpawnSpots.Length)];

		//Non-Challenge mode (for now, all to be tidied up later)
		if (guiManager.currentGameMode != 2) {

			if (teamID == 1)
				mySpawnSpot = BluePlayerSpawnSpots [Random.Range (0, BluePlayerSpawnSpots.Length)];
			else if (teamID == 2)
				mySpawnSpot = RedPlayerSpawnSpots [Random.Range (0, BluePlayerSpawnSpots.Length)];
		} else {
			//Campaign mode spawns
			mySpawnSpot = PlayerSpawnSpots[Random.Range (0, PlayerSpawnSpots.Length)];
		}
		GameObject myPlayerGameObject = (GameObject)PhotonNetwork.Instantiate (playerPrefabName, mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
		PlayerObject = myPlayerGameObject;
		standbyCamera.SetActive (false);
		deathCam.SetActive (false);
		myPlayerGameObject.transform.Find ("Main Camera").gameObject.SetActive (true);
		myPlayerGameObject.GetComponent<PlayerMovement> ().enabled = true;
		myPlayerGameObject.GetComponent<PlayerShooting> ().enabled = true;
		myPlayerGameObject.GetComponent<PhotonView> ().RPC ("SetTeamID", PhotonTargets.AllBuffered, teamID);
		myPlayerGameObject.GetComponent<PhotonView> ().RPC ("SetClassID", PhotonTargets.AllBuffered, classID);

		//Disable Player Model and Disable Cannon on client end
		Component[] SkinnedMeshRenderers = myPlayerGameObject.GetComponentsInChildren<SkinnedMeshRenderer> ();
		foreach (SkinnedMeshRenderer smr in SkinnedMeshRenderers) {
			if (smr.gameObject.name.Equals ("weapon_meshLocal") || smr.gameObject.name.Equals ("arms_mesh") || smr.gameObject.name.Equals ("weapon_meshNetworked")) {
				smr.enabled = true;
			} else {
				smr.enabled = false;
			}
		}
		_PlayerName = PhotonNetwork.playerName;	//AGAIN NEEDED TOBE RPC CALL... TO MAKE MORE EFFICIENT, THIS IS STORED IN TEAMMEMBER NOW
		myPlayerGameObject.GetComponent<PhotonView> ().RPC ("SetPlayerName", PhotonTargets.AllBuffered, _PlayerName);
		myPlayerGameObject.transform.Find ("NameTag").GetComponent<PhotonView> ().RPC ("TagPlayerName", PhotonTargets.AllBuffered, _PlayerName);

		guiManager.DisplayInGameGUI = true;

		return myPlayerGameObject;
	}

	void dupeCheck(){
		dupeCount = 0f;
		foreach(PhotonPlayer p in PhotonNetwork.playerList){
			if(p.name == PhotonNetwork.player.NickName && p.name != string.Empty){
				dupeCount += 1f;
			}
		}
		if (PhotonNetwork.player.NickName == string.Empty) {
			dupeCount += 1f;
		}
		if (dupeCount > 1f) {
			duplicateName = true;
		}

	}

	bool hasInitializedKeyInputPanel = false;
	void FixedUpdate(){

		if (!isInit)
			return;

		if (KeyInputPanel.GetActive () && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))) {
			DisableKeyInputPanel ();
		}

		//BASICALLY SPAWNED IN GAME FROM HERE ON
		if (_hasPickedTeam) {
			if ((kMgr.GetKeyPublic (keyType.settings) || Input.GetKey (KeyCode.BackQuote)) && !_EnableChat) {
				SettingsPanel.SetActive (true);
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			if (KeyInputPanel.GetActive ()) {
				if (!hasInitializedKeyInputPanel) {
					cInputManager.Initialize_KeyInputPanel ();
					hasInitializedKeyInputPanel = true;
				}
				//Setting Volume Calculation Update
				SettingsPanel.GetComponent<SettingsPanelInput> ().Value_MusicVolume.GetComponent<Text> ().text = Mathf.RoundToInt(GetComponent<AudioSource> ().volume * 100).ToString ();
			}
			//ABLE TO ESCAPE WITH SHOOT CONTROLLER BUTTON (R2=JOYSTICKBUTTON9)
			if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyUp (KeyCode.JoystickButton9)) {
				cInputManager.MainMenuPageNumber = 0;
				SettingsPanel.SetActive (false);
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}

			//DISPLAY CHAT MESSAGES
			if (kMgr.GetKeyDownPublic (keyType.talk)) {
				_EnableChat = true;
			}

			//ENTERING TEXT FOR CHAT GUI
			if (_EnableChat) {
				PlayerInGamePanelInputs.isChatEnabled = true;
				PlayerInGamePanelInputs.doNullCrossHair ();
				ChatPanel.SetActive (true);
				ChatPanel.GetComponent<ChatPanelInput> ().Input_Message.ActivateInputField ();
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;

				if (Input.GetKeyUp (KeyCode.Return)) {
					//Send Message
					Button_SendChat ();
				}
				if (Input.GetKeyUp (KeyCode.Escape) || Input.GetKeyUp (KeyCode.JoystickButton2)) {
					//Cancel Message
					Button_CancelChat ();
				}
			} else {
				if (guiManager.ChatPanel.GetActive ()) {
					guiManager.ChatPanel.SetActive (false);
				}
			}		
		}

	}

	IEnumerator TeamSelectionProcess_Coroutine(){
		while (true) {
			//START BY SELECTING A TEAM FROM TEAMCLASS PANEL. ENABLE TEAMCLASSPANEL
			if (!PhotonNetwork.isMasterClient) {
				if (MainMenu.sceneSync && !hasPickedTeam) {
					TeamClassPanel.SetActive (true);
				}		
			}
			//Disable Host Options for BOTS when not Master Client
//			if (!PhotonNetwork.isMasterClient && SettingsPanel.GetComponent<SettingsPanelInput> ().HostOptionLabel.GetActive () == true) {
//				SettingsPanel.GetComponent<SettingsPanelInput> ().Value_BlueBotCount.SetActive (false);
//				SettingsPanel.GetComponent<SettingsPanelInput> ().Value_RedBotCount.SetActive (false);
//			}

			if (!_hasPickedTeam) {
				dupeCheck ();
				if (PhotonNetwork.connected) {
					if (duplicateName == true) {
						TeamClassPanel.SetActive (false);
						KickPlayerPanel.SetActive (true);
						KickPlayerPanel.GetComponent<KickPlayerPanelInput> ().Text_DuplicateName.text = "THE USERNAME, " + PhotonNetwork.player.NickName + ", IS CURRENTLY IN GAME";
					}
				}

				TeamButtonList teamClassPanelInputs = TeamClassPanel.GetComponent<TeamButtonList> ();

				if (guiManager.currentGameMode == 1) {

					teamClassPanelInputs.GameModeText.text = "TEAM DEATH MATCH";
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount] < (int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount]) {
						//Disable Blue Team Picking
						teamClassPanelInputs.Button_Blue.SetActive (false);
					}
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount] < (int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount]) {	
						//Disable Red Team Picking
						teamClassPanelInputs.Button_Red.SetActive (false);
					}
					if (_teamSelID != -1 && _primaryWeap != -1 && classSelID != -1) {
						//Conditions have been met for spawn (Team / Class / Primary Weap)
						teamClassPanelInputs.Button_Spawn.SetActive (true);
					}
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount] == 0 && (int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount] == 0) {	
						//Conditions have been met to kick player (Duplicate name / Game Already running)
						KickPlayerPanel.SetActive (true);
						Debug.Log ("red: " + (int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount] + ", blue: " + (int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount]);

						KickPlayerPanel.GetComponent<KickPlayerPanelInput> ().Text_GameIsRunning.text = "THIS GAME IS CURRENTLY RUNNING";
						TeamClassPanel.SetActive (false);
					}
				}


				if (guiManager.currentGameMode == 0) {

					teamClassPanelInputs.GameModeText.text = "TILE DYNASTY";
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount] < (int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount]) {
						teamClassPanelInputs.Button_Blue.SetActive (false);
					}
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount] < (int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount]) {	
						teamClassPanelInputs.Button_Red.SetActive (false);
					}
					if (_teamSelID != -1 && _primaryWeap != -1 && classSelID != -1) {
						teamClassPanelInputs.Button_Spawn.SetActive (true);
					}
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount] == 0 && (int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount] == 0) {	
						KickPlayerPanel.SetActive (true);
						KickPlayerPanel.GetComponent<KickPlayerPanelInput> ().Text_GameIsRunning.text = "THIS GAME IS CURRENTLY RUNNING";
						TeamClassPanel.SetActive (false);
					}

				}

				if (guiManager.currentGameMode == 2) {
					teamClassPanelInputs.GameModeText.text = "SEARCH AND RESCUE";
					teamClassPanelInputs.Button_Red.SetActive (false);
					teamClassPanelInputs.Button_Blue.SetActive (false);
//					SettingsPanel.GetComponent<SettingsPanelInput> ().Value_BlueBotCount.SetActive (false);
//					SettingsPanel.GetComponent<SettingsPanelInput> ().Value_RedBotCount.SetActive (false);
					_teamSelID = 0;
					if (_teamSelID != -1 && _primaryWeap != -1 && classSelID != -1) {
						teamClassPanelInputs.Button_Spawn.SetActive (true);
					}
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount] == 0) {
						KickPlayerPanel.SetActive (true);
						KickPlayerPanel.GetComponent<KickPlayerPanelInput> ().Text_GameIsRunning.text = "THIS GAME IS CURRENTLY RUNNING";
						TeamClassPanel.SetActive (false);
					}
				}

				if (guiManager.currentGameMode == 3) {
					teamClassPanelInputs.GameModeText.text = "GARBAGE MAN";
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount] < (int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount]) {
						teamClassPanelInputs.Button_Blue.SetActive (false);
					}
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount] < (int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount]) {	
						teamClassPanelInputs.Button_Red.SetActive (false);
					}
					if (_teamSelID != -1 && _primaryWeap != -1 && classSelID != -1) {
						teamClassPanelInputs.Button_Spawn.SetActive (true);
					}
					if ((int)PhotonNetwork.room.CustomProperties [RoomProperties.blueAvailableCount] == 0 && (int)PhotonNetwork.room.CustomProperties [RoomProperties.redAvailableCount] == 0) {	
						KickPlayerPanel.SetActive (true);
						KickPlayerPanel.GetComponent<KickPlayerPanelInput> ().Text_GameIsRunning.text = "THIS GAME IS CURRENTLY RUNNING";
						TeamClassPanel.SetActive (false);
					}
				}
				if (guiManager.currentGameMode == 99) {
					TeamClassPanel.SetActive (false);
					KeyInputPanel.SetActive (false);
					teamClassPanelInputs.GameModeText.text = "HOME BASE";
					teamClassPanelInputs.Button_Blue.SetActive (false);
					teamClassPanelInputs.Button_Red.SetActive (false);
					teamClassPanelInputs.Button_CoopTeam.SetActive (false);

					_teamSelID = 0;
					classSelID = 1;
					Button_Spawn ();
					if (_teamSelID != -1 && _primaryWeap != -1 && classSelID != -1) {
						if (teamClassPanelInputs.Button_Spawn_Label.GetComponent<Text> () != null) {
							teamClassPanelInputs.Button_Spawn_Label.GetComponent<Text> ().text = "PROCEED";
//						teamClassPanelInputs.Button_Spawn.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, 100, 0);
							teamClassPanelInputs.Button_Spawn.SetActive (true);
						}
					}
				}
			}
			yield return waitFor0_1;
		}
	}

	string colorEnd = "</color>\n";
	string colorblue0 = "<color=#32C8C800>";
	string colorblue1 = "<color=#32C8C80F>";
	string colorblue2 = "<color=#32C8C82D>";
	string colorblue3 = "<color=#32C8C84B>";
	string colorblue4 = "<color=#32C8C869>";
	string colorblue5 = "<color=#32C8C887>";
	string colorblue6 = "<color=#32C8C8A5>";
	string colorblue7 = "<color=#32C8C8C3>";
	string colorblue8 = "<color=#32C8C8E1>";
	string colorblue9 = "<color=#32C8C8FF>";
	void initChatMsgs(){
		for (int i = 0; i < 10; i++) {
			AddChatMessage_RPC (string.Empty);
		}
	}
	IEnumerator DisplayChatMessages_Coroutine(){
		while (true) {
			if (_hasPickedTeam) {
				// We are fully connected, make sure to display the chat box.
				if (guiManager.DisplayInGameGUI == true) {
					string displayedChat = string.Empty;
					int messageCounter = 0;
					foreach (string msg in chatMessages) {

						if (messageCounter == 0) {
							displayedChat += (colorblue0 + msg + colorEnd);
						} else if (messageCounter == 1) {
							displayedChat += (colorblue1 + msg + colorEnd);
						} else if (messageCounter == 2) {
							displayedChat += (colorblue2 + msg + colorEnd);
						} else if (messageCounter == 3) {
							displayedChat += (colorblue3 + msg + colorEnd);
						} else if (messageCounter == 4) {
							displayedChat += (colorblue4 + msg + colorEnd);
						} else if (messageCounter == 5) {
							displayedChat += (colorblue5 + msg + colorEnd);
						} else if (messageCounter == 6) {
							displayedChat += (colorblue6 + msg + colorEnd);
						} else if (messageCounter == 7) {
							displayedChat += (colorblue7 + msg + colorEnd);
						} else if (messageCounter == 8) {
							displayedChat += (colorblue8 + msg + colorEnd);
						} else if (messageCounter == 9) {
							displayedChat += (colorblue9 + msg + colorEnd);
						}
						messageCounter++;
						DisplayChatPanel.GetComponent<DisplayChatPanelInputs> ().Text_ChatMessage.text = displayedChat;
					}
				}

			}
			yield return waitFor1;
		}
	}

	[PunRPC]
	public void nameEnemyBots(GameObject enemy, string nameBase, int i){
		enemy.GetComponent<PhotonView> ().name = nameBase + (i + 1).ToString ();
	}

	IEnumerator PlayerRespawn_Coroutine(){
		while (true) {
			if(requestRespawnPlayer){
				SpawnMyPlayer(playerPrefab.name, _teamSelID, classSelID);
				requestRespawnPlayer = false;
				windowManager.respawnDisplayScoreBoard = true;
			}
			if(requestFreshSpawn){
				SpawnMyPlayer(playerPrefab.name, _teamSelID, classSelID);
				requestFreshSpawn = false;
				windowManager.respawnDisplayScoreBoard = true;
			}
			yield return waitFor0_2;
		}
	}

	[PunRPC]
	void updateTeamAvailibility(int teamID, int teamAvailibility){
		if (teamID == 0) {
			_CoopTeamAvailibility -= 1;
		} else if (teamID == 1) {
			_blueTeamAvailibility = teamAvailibility;
		} else if (teamID == 2) {
			_redTeamAvailibility = teamAvailibility;
		}
	}

	void OnDestroy(){
		PlayerPrefs.SetString ("Alias", pendingName);
	}

	public void AddChatMessage(string m){
		//Decided to not make it bufferedAll since people joining way later would buffer a lot of messages/s.

		GetComponent<PhotonView> ().RPC (AddChatMessageRPC, PhotonTargets.AllBuffered, m);
	}

	string AddChatMessageRPC = "AddChatMessage_RPC";
	[PunRPC]
	void AddChatMessage_RPC(string m){
		while(chatMessages.Count >= maxChatMessages){
			chatMessages.RemoveAt(0);
		}
		chatMessages.Add (m);
	}




	public string GetPlayerName(){
		return _PlayerName;
	}

	public string networkPlayerName{
		get{
			return _PlayerName;
		}
	}

}﻿