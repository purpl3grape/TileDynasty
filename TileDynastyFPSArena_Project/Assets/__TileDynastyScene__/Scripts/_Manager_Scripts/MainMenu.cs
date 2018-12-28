using System.IO;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Steamworks;






public class MainMenu : Photon.MonoBehaviour
{
	private string serverIP = "127.0.0.1";
	private int serverPort = 5055;
	private string defaultAppId = "b1ebc8d2-ec9b-4243-b29b-dd90f35cf9ac";
	private string roomName = "Room";
	private string joinRoomName = "Room";
	private string joinOnlineRoomName = "";
	private bool overridePhotonConfigFile = false;
	private string gameVersion = "v1.0.2.";
	private string connectionText = "WAITING TO CONNECT TO...\n";

	private string startFromHomeBaseKey = "startFromHomeBaseKey";

	private bool photonConnectionFailed = false;

	int totalPlayers = 0;   // sum of players in all listed rooms
	float screenWidth = 960;


	private Vector2 scrollPos = Vector2.zero;

	public GameObject MainMenuOptionGO;
	public GameObject ChooseRoomOptionGO;
	public GameObject RoomsOnlineGO;

	public GameObject panelOptionMain;
	public GameObject panelOptionMode;
	public GameObject panelOptionDays;
	public GameObject panelOptionSize;
	public GameObject panelOptionMap;
	public GameObject LabelMode;
	public GameObject LabelDays;
	public GameObject LabelSize;
	public GameObject LabelMap;
	public GameObject panelRoomName;
	public GameObject panelRoomMode;
	public GameObject panelRoomSize;
	public GameObject panelRoomMap;
	public GameObject LabelRoomName;
	public GameObject LabelRoomMode;
	public GameObject LabelRoomSize;
	public GameObject LabelRoomMap;
	public GameObject LabelRoomCount;

	public Color[] chooseRoomOptionColor;
	enum OptionLevelEnum{
		OptionLevel1,
		OptionLevel2,
		OptionLevel3,
	}
	[SerializeField] OptionLevelEnum optionLevel = OptionLevelEnum.OptionLevel1;

	enum MainMenuGameOptionEnum{
		GameMode,
		GameTime,
		GameSize,
		GameMap,
	}
	[SerializeField] MainMenuGameOptionEnum mainMenuGameOption = MainMenuGameOptionEnum.GameMode;

	enum MainMenuOptionEnum{
		HomeBase,
		SinglePlayer,
		MultiPlayer,
		QuickPlay,
		JoinOnlineGames,
	}
	[SerializeField] MainMenuOptionEnum mainMenuOption = MainMenuOptionEnum.SinglePlayer;

	enum GameModeEnum{
		SearchAndRescue,
		TDM,
	}
	[SerializeField] GameModeEnum gameModeEnum = GameModeEnum.SearchAndRescue;
	enum GameDaysEnum{
		Two,
		Three,
		Four,
		Five,
		Six,
		Seven,
	}
	[SerializeField] GameDaysEnum gameDaysEnum = GameDaysEnum.Two;
	enum GameSizeEnum{
		One,
		Two,
		Three,
		Four,
		Five,
		Six,
	}
	[SerializeField] GameSizeEnum gameSizeEnum = GameSizeEnum.One;
	enum GameMapEnum{
		Sector02,
	}
	[SerializeField] GameMapEnum gameMapEnum = GameMapEnum.Sector02;



	public GameObject LobbySoundObj;
	public AudioClip[] LobbySounds;
	public GameObject RoomInfoPrefab;
	[HideInInspector] public float RoomInfoPos_V = 0f;
	[HideInInspector] public float RoomInfoPos_H = 0f;
	[HideInInspector] public float RoomInfoScale = 1f;

	public int RoomSelectionIndex = 0;
	public int TotalRoomCount = 0;
	public RoomInfo[] roomList;
	public RoomInfo currentRoom;
	public Sprite[] MapPreviewSpriteList;

	public GameObject ErrorPanel;
	public GameObject SteamErrorPanel;
	public GameObject SteamBeginPanel;
	public GameObject LobbyPanel;
	public GameObject UsernamePanel;
	public GameObject PlayerStatsPanel;
	public bool hasProvidedUserName = false;
	public GameObject ConnectingPanel;
	public GameObject FailedConnectionPanel;
	public GameObject LoadingPanel;
	public ControllerCanvasManager controllerCanvas;
	public Sprite LoginUsernameBackground;
	public Sprite LobbyBackground;
	public bool isRememberMe = false;
	public bool isCreateMenu = false;
	bool isViewingRoomList = false;
	public float roomSize = 6;
	public int loadingPercent;
	public int mapSelectNumber = 0;
	public static int gameMode = 0;
	public static int gameTimeDays = 10;
	public static int regionIndex = 6;
	public static bool sceneSync = false;

	public GameObject UIContainer;
	public RoomListManager roomListManager;

	CloudRegionCode clientRegion = CloudRegionCode.asia;
	string regionText = "ASIA";

	private bool hasInitializedSteamAdvancedStatisticsDisplay = false;
	private bool hasInitializedSteamMainStatisticsDisplay = false;
	private int statKills = 0;
	private int statDeaths = 0;
	private int statAssists = 0;
	private int statDamageDealt = 0;
	private int statDamageReceived = 0;
	private int statBestKillStreak = 0;
	private int statGamesPlayed = 0;
	private int statGamesWon = 0;
	private int statGamesLost = 0;
	private int statGamesTied = 0;
	private int statFired;
	private int statFiredHMG;
	private int statFiredRocket;
	private int statFiredRail;
	private int statFiredEMP;
	private int statHit;
	private int statHitHMG;
	private int statHitRocket;
	private int statHitRail;
	private int statHitEMP;
	private float statAccuracy;
	private float statAccuracyHMG;
	private float statAccuracyRocket;
	private float statAccuracyRail;
	private float statAccuracyEMP;

	AudioSource menuAudio;

	void Awake()
	{
		/* TESTING PURPOSE ONLY MAY NEED TO RESET THESE VALUES */
		/*
		PlayerPrefs.SetInt ("BeginChallengeFromHomeBase", 0);
		PlayerPrefs.SetInt ("startMultiplayerFromHomeBaseKey", 0);
		*/

		gameMode = 0;
		mapSelectNumber = 0;
		gameTimeDays = 4;
		roomSize = 1;
		LabelMode.GetComponent<Text> ().text = "TEAM DEATH MATCH";
		LabelDays.GetComponent<Text> ().text = "4 DAYS";
		LabelSize.GetComponent<Text> ().text = "SINGLE PLAYER";
		LabelMap.GetComponent<Text> ().text = "MILITARY BASE";
		//Load name from PlayerPrefs
		if (PlayerPrefs.GetInt ("IsRememberMe") == 1) {
			UsernamePanel.GetComponent<UsernamePanelInput> ().toggle_rememberMe.GetComponent<Toggle> ().isOn = true;
			UsernamePanel.GetComponent<UsernamePanelInput> ().usernameInput.GetComponent<InputField>().text = PlayerPrefs.GetString ("playerName");
			PhotonNetwork.playerName = PlayerPrefs.GetString ("playerName");
		} else {
			PhotonNetwork.playerName = "";
			UsernamePanel.GetComponent<UsernamePanelInput> ().usernameInput.GetComponent<InputField> ().text = "";
			UsernamePanel.GetComponent<UsernamePanelInput> ().toggle_rememberMe.GetComponent<Toggle> ().isOn = false;
		}
		roomName = roomName + Random.Range (1, 99999).ToString();
		roomListManager = GameObject.FindGameObjectWithTag ("RoomListManager").GetComponent<RoomListManager> ();
	}

	void OnJoinedLobby(){
	}

	void OnJoinedRoom()
	{

		PhotonNetwork.isMessageQueueRunning = false;

		if (mapSelectNumber == 0) {
			SceneManager.LoadScene(2);
		} else if (mapSelectNumber == 1) {
			SceneManager.LoadScene(3);
		}

		PhotonNetwork.automaticallySyncScene = true;
		sceneSync = true;
	}

	IEnumerator steamAutoLogin(){
		while (true) {
			Debug.Log ("steam login: " + PhotonNetwork.playerName + " username = : " + hasProvidedUserName);
			if (SteamFriends.GetPersonaName () != null && !hasProvidedUserName) {
				PhotonNetwork.playerName = SteamFriends.GetPersonaName ();
				if (PhotonNetwork.playerName != "" && !PhotonNetwork.playerName.StartsWith(" ")) {
					hasProvidedUserName = true;
					LobbyPanel.SetActive (true);
					UsernamePanel.SetActive (false);
					PhotonNetwork.ConnectToRegion (clientRegion, this.gameVersion);
					Debug.Log ("Steam Client Connect to Photon Region " + clientRegion);
				}
			}
			yield return new WaitForSeconds (2f);
		}
	}

	void Start(){

		if (!SteamManager.Initialized) {
			SteamErrorPanel.SetActive (true);
			SteamBeginPanel.SetActive (false);
			return;
		}


		LobbyPanel.GetComponent<LobbyInput> ().Lobby_VersionText.GetComponent<Text> ().text = gameVersion;
		regionIndex = PlayerPrefs.GetInt ("Region");
		//Init
		gameTimeDays = 4;
		roomSize = 4;
		gameMode = 2;
		gameModeEnum = GameModeEnum.TDM;

		if (SteamFriends.GetPersonaName () != null && !hasProvidedUserName) {
			PhotonNetwork.playerName = SteamFriends.GetPersonaName ();
			if (PhotonNetwork.playerName != "" && !PhotonNetwork.playerName.StartsWith(" ")) {
				hasProvidedUserName = true;
				LobbyPanel.SetActive (true);
				UsernamePanel.SetActive (false);
				RegionAssign (regionIndex);
				PhotonNetwork.ConnectToRegion (clientRegion, this.gameVersion);
				Debug.Log ("Steam Client Connect to Photon Region " + clientRegion);
			}
		}




		menuAudio = GetComponent<AudioSource> ();
		menuAudio.volume = PlayerPrefs.HasKey ("MusicVolume") == true ? PlayerPrefs.GetFloat ("MusicVolume") : 1;
		LobbySoundObj.GetComponent<AudioSource> ().volume = PlayerPrefs.HasKey ("MusicVolume") == true ? PlayerPrefs.GetFloat ("MusicVolume") : 1;
		if (PlayerPrefs.GetInt ("Region") != -1) {

			if (StartHomeBaseCO == null) {
				StartHomeBaseCO = StartHomeBase_CO ();
				StartCoroutine (StartHomeBaseCO);
			}
		}

		if (PlayerPrefs.GetInt ("ViewRoomList") == 1) {
			isViewingRoomList = true;
//			LobbyPanel.SetActive (true);
			SteamBeginPanel.SetActive (false);
		}

//		} else if (PlayerPrefs.GetInt ("startMultiplayerFromHomeBaseKey") == 0 && PlayerPrefs.GetInt ("BeginChallengeFromHomeBase") == 0) {
//			Debug.Log ("home base only");
//			UIContainer.SetActive (false);
//			Button_HomeBase ();
//		}

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	void OnPhotonJoinRoomFailed() {
		optionLevel = OptionLevelEnum.OptionLevel1;
		if (StartHomeBaseCO != null)
			StopCoroutine (StartHomeBaseCO);
		StartHomeBaseCO = null;
		StartHomeBaseCO = StartHomeBase_CO ();
		StartCoroutine (StartHomeBaseCO);
		LoadingPanel.SetActive (false);
	}

	public IEnumerator StartHomeBaseCO;
	public IEnumerator StartHomeBase_CO(){

		if (PlayerPrefs.GetInt ("BeginChallengeFromHomeBase") == 0 && PlayerPrefs.GetInt ("startMultiplayerFromHomeBaseKey") == 0 & PlayerPrefs.GetInt ("ViewRoomList") == 0) {
			//OPTION LEVEL 1
			while (!(PhotonNetwork.connected && PhotonNetwork.insideLobby)) {
				yield return true;
			}


			MainMenuOptionGO.SetActive (true);
			ChooseRoomOptionGO.SetActive (false);
			RoomsOnlineGO.SetActive (false);
			gotRoomsUpdate = false;
			RoomCountIndex = 0;

//			OnReceivedRoomListUpdate ();

			while (optionLevel == OptionLevelEnum.OptionLevel1) {
				MainMenuOptions ();
				if (Input.GetKeyDown (KeyCode.Return)) {
					OnReceivedRoomListUpdate ();
					optionLevel = OptionLevelEnum.OptionLevel2;
				}
				yield return true;
			}
			panelOptionMain.GetComponent<Image> ().color = chooseRoomOptionColor [1];
			LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [0]);
			yield return new WaitForSeconds (0.25f);
			panelOptionMain.GetComponent<Image> ().color = chooseRoomOptionColor [0];

			//OPTION LEVEL 2
			while (optionLevel == OptionLevelEnum.OptionLevel2) {

				if (mainMenuOption == MainMenuOptionEnum.HomeBase) {
					EnteredUserName ();
				} else if (mainMenuOption == MainMenuOptionEnum.JoinOnlineGames) {
//					OnReceivedRoomListUpdate ();
					NavigateRoomOptions ();
				} else {
					GameOptions ();
				}
				if (Input.GetKeyDown (KeyCode.Return)) {
					LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [0]);
					optionLevel = OptionLevelEnum.OptionLevel3;
				}
				yield return true;
			}





			yield return new WaitForSeconds (0.25f);
		}
		if (optionLevel == OptionLevelEnum.OptionLevel3) {
			if (mainMenuOption == MainMenuOptionEnum.JoinOnlineGames) {
				if (Steamworks.SteamFriends.GetPersonaState () != Steamworks.EPersonaState.k_EPersonaStateOffline) {										
					if (TotalRoomCount == 0) {
						optionLevel = OptionLevelEnum.OptionLevel1;
						//Restart the process again
						if (StartHomeBaseCO != null)
							StopCoroutine (StartHomeBaseCO);
						StartHomeBaseCO = null;
						StartHomeBaseCO = StartHomeBase_CO ();
						StartCoroutine (StartHomeBaseCO);
					} else {
						LoadingPanel.SetActive (true);
						PhotonNetwork.JoinRoom (roomList [RoomCountIndex].Name);					

						LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [0]);
					}
				} else {
					LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [2]);
					optionLevel = OptionLevelEnum.OptionLevel1;
					//Restart the process again
					if (StartHomeBaseCO != null)
						StopCoroutine (StartHomeBaseCO);
					StartHomeBaseCO = null;
					StartHomeBaseCO = StartHomeBase_CO ();
					StartCoroutine (StartHomeBaseCO);
				}					
			} else {
				LoadingPanel.SetActive (true);
				EnteredUserName ();
			}
		}
		StartHomeBaseCO = null;
	}


	void MainMenuOptions(){

		MainMenuOptionGO.SetActive (true);
		ChooseRoomOptionGO.SetActive (false);
		RoomsOnlineGO.SetActive (false);

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (mainMenuOption == MainMenuOptionEnum.HomeBase)
				mainMenuOption = MainMenuOptionEnum.JoinOnlineGames;
			else if (mainMenuOption == MainMenuOptionEnum.JoinOnlineGames)
				mainMenuOption = MainMenuOptionEnum.MultiPlayer;
			else if (mainMenuOption == MainMenuOptionEnum.MultiPlayer)
				mainMenuOption = MainMenuOptionEnum.SinglePlayer;
			else if (mainMenuOption == MainMenuOptionEnum.SinglePlayer)
				mainMenuOption = MainMenuOptionEnum.HomeBase;
			
			LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (mainMenuOption == MainMenuOptionEnum.HomeBase)
				mainMenuOption = MainMenuOptionEnum.SinglePlayer;
			else if (mainMenuOption == MainMenuOptionEnum.SinglePlayer)
				mainMenuOption = MainMenuOptionEnum.MultiPlayer;
			else if (mainMenuOption == MainMenuOptionEnum.MultiPlayer)
				mainMenuOption = MainMenuOptionEnum.JoinOnlineGames;
			else if (mainMenuOption == MainMenuOptionEnum.JoinOnlineGames)
				mainMenuOption = MainMenuOptionEnum.HomeBase;
			
			LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
		} else if (Input.GetKeyDown (KeyCode.Backspace)) {
			optionLevel = OptionLevelEnum.OptionLevel1;
			//Already at option level 1, so produce the error sound
			LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [2]);
		}

		if (mainMenuOption == MainMenuOptionEnum.HomeBase)
			SteamBeginPanel.GetComponent<LobbyInput> ().Text_LobbyOption.GetComponent<Text> ().text = "HOME BASE";
		else if (mainMenuOption == MainMenuOptionEnum.JoinOnlineGames)
			SteamBeginPanel.GetComponent<LobbyInput> ().Text_LobbyOption.GetComponent<Text> ().text = "JOIN ONLINE GAMES";
		else if (mainMenuOption == MainMenuOptionEnum.MultiPlayer)
			SteamBeginPanel.GetComponent<LobbyInput> ().Text_LobbyOption.GetComponent<Text> ().text = "MULTIPLAYER";
		else if (mainMenuOption == MainMenuOptionEnum.SinglePlayer)
			SteamBeginPanel.GetComponent<LobbyInput> ().Text_LobbyOption.GetComponent<Text> ().text = "SINGLE PLAYER";
		


		if (mainMenuOption == MainMenuOptionEnum.SinglePlayer) {
			roomSize = 1;
		} else if (mainMenuOption == MainMenuOptionEnum.MultiPlayer) {
			roomSize = 4;
		}
	}




	void GameOptions(){

		MainMenuOptionGO.SetActive (false);
		ChooseRoomOptionGO.SetActive (true);
		RoomsOnlineGO.SetActive (false);

		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (mainMenuGameOption == MainMenuGameOptionEnum.GameMode) {
				mainMenuGameOption = MainMenuGameOptionEnum.GameTime;
			} else if (mainMenuGameOption == MainMenuGameOptionEnum.GameTime) {
				mainMenuGameOption = MainMenuGameOptionEnum.GameSize;
			} else if (mainMenuGameOption == MainMenuGameOptionEnum.GameSize) {
				mainMenuGameOption = MainMenuGameOptionEnum.GameMap;
			} else if (mainMenuGameOption == MainMenuGameOptionEnum.GameMap) {
				mainMenuGameOption = MainMenuGameOptionEnum.GameMode;
			}
			LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (mainMenuGameOption == MainMenuGameOptionEnum.GameMode) {
				mainMenuGameOption = MainMenuGameOptionEnum.GameMap;
			} else if (mainMenuGameOption == MainMenuGameOptionEnum.GameMap) {
				mainMenuGameOption = MainMenuGameOptionEnum.GameSize;
			} else if (mainMenuGameOption == MainMenuGameOptionEnum.GameSize) {
				mainMenuGameOption = MainMenuGameOptionEnum.GameTime;
			} else if (mainMenuGameOption == MainMenuGameOptionEnum.GameTime) {
				mainMenuGameOption = MainMenuGameOptionEnum.GameMode;
			}
			LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
		} else if (Input.GetKeyDown (KeyCode.Backspace)) {
			optionLevel = OptionLevelEnum.OptionLevel1;
			//Restart the process again
			if (StartHomeBaseCO != null)
				StopCoroutine (StartHomeBaseCO);
			StartHomeBaseCO = null;
			StartHomeBaseCO = StartHomeBase_CO ();
			StartCoroutine (StartHomeBaseCO);

		}


		panelOptionMode.GetComponent<Image> ().color = mainMenuGameOption == MainMenuGameOptionEnum.GameMode ? chooseRoomOptionColor [1] : chooseRoomOptionColor [0];
		panelOptionDays.GetComponent<Image> ().color = mainMenuGameOption == MainMenuGameOptionEnum.GameTime ? chooseRoomOptionColor [1] : chooseRoomOptionColor [0];
		panelOptionSize.GetComponent<Image> ().color = mainMenuGameOption == MainMenuGameOptionEnum.GameSize ? chooseRoomOptionColor [1] : chooseRoomOptionColor [0];
		panelOptionMap.GetComponent<Image> ().color = mainMenuGameOption == MainMenuGameOptionEnum.GameMap ? chooseRoomOptionColor [1] : chooseRoomOptionColor [0];

		if (mainMenuOption == MainMenuOptionEnum.SinglePlayer) {
			roomSize = 1;
			LabelSize.GetComponent<Text> ().text = "SINGLE PLAYER";
		} else {
			LabelSize.GetComponent<Text> ().text = roomSize + " PLAYERS";
		}

		NavigateGameOptions ();
	}
		
	void NavigateGameOptions(){
		if (mainMenuGameOption == MainMenuGameOptionEnum.GameMode) {
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (gameModeEnum == GameModeEnum.SearchAndRescue) {
					gameModeEnum = GameModeEnum.TDM;			
					LabelMode.GetComponent<Text> ().text = "TEAM DEATH MATCH";
				} else if (gameModeEnum == GameModeEnum.TDM) {
					gameModeEnum = GameModeEnum.SearchAndRescue;
					LabelMode.GetComponent<Text> ().text = "SEARCH AND RESCUE";
				}
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (gameModeEnum == GameModeEnum.SearchAndRescue) {
					gameModeEnum = GameModeEnum.TDM;
					LabelMode.GetComponent<Text> ().text = "TEAM DEATH MATCH";
				} else if (gameModeEnum == GameModeEnum.TDM) {
					gameModeEnum = GameModeEnum.SearchAndRescue;
					LabelMode.GetComponent<Text> ().text = "SEARCH AND RESCUE";
				}
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
			}

			if (gameModeEnum == GameModeEnum.TDM) {
				gameMode = 1;
			} else if (gameModeEnum == GameModeEnum.SearchAndRescue) {
				gameMode = 2;
			}

		} else if (mainMenuGameOption == MainMenuGameOptionEnum.GameTime) {
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				gameTimeDays = gameTimeDays <= 2 ? 2 : gameTimeDays - 1;
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				gameTimeDays = gameTimeDays >= 10 ? 10 : gameTimeDays + 1;
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
			}
			LabelDays.GetComponent<Text> ().text = gameTimeDays + " DAYS";
		} else if (mainMenuGameOption == MainMenuGameOptionEnum.GameSize) {
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (mainMenuOption == MainMenuOptionEnum.SinglePlayer)
					roomSize = roomSize <= 1 ? 1 : roomSize - 1;
				else
					roomSize = roomSize <= 2 ? 2 : roomSize - 1;
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (mainMenuOption == MainMenuOptionEnum.SinglePlayer)
					roomSize = roomSize >= 1 ? 1 : roomSize + 1;
				else
					roomSize = roomSize >= 6 ? 6 : roomSize + 1;

				if (mainMenuOption == MainMenuOptionEnum.SinglePlayer) {
					roomSize = 1;
					LabelSize.GetComponent<Text> ().text = "SINGLE PLAYER";
				} else {
					LabelSize.GetComponent<Text> ().text = roomSize + " PLAYERS";
				}
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
			}			
		} else if (mainMenuGameOption == MainMenuGameOptionEnum.GameMap) {
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				mapSelectNumber = 0;
				LabelMap.GetComponent<Text> ().text = "MILITARY BASE";
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				mapSelectNumber = 0;
				LabelMap.GetComponent<Text> ().text = "MILITARY BASE";
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
			}			
		}

	}


	int RoomCountIndex = 0;
	void OnReceivedRoomListUpdate(){
		
		if (!gotRoomsUpdate) {
//			if (PhotonNetwork.connected && PhotonNetwork.insideLobby) {
				gotRoomsUpdate = true;
				roomList = PhotonNetwork.GetRoomList ();
				TotalRoomCount = roomList.Length;
				Debug.Log ("Number of Rooms Online: " + TotalRoomCount);
//			} else {
//				Debug.Log ("Connected: " + PhotonNetwork.connected + ", In Lobby: " + PhotonNetwork.insideLobby);
//				return;
//			}
		}


 
	}

	void NavigateRoomOptions(){
		MainMenuOptionGO.SetActive (false);
		ChooseRoomOptionGO.SetActive (false);
		RoomsOnlineGO.SetActive (true);
		if (roomList.Length < 1)
			LabelRoomCount.GetComponent<Text> ().text = roomList.Length + " Rooms Online";
		else if (roomList.Length == 1)
			LabelRoomCount.GetComponent<Text> ().text = RoomCountIndex + " :: " + roomList.Length + " Room Online";
		else
			LabelRoomCount.GetComponent<Text> ().text = RoomCountIndex + " :: " + roomList.Length + " Rooms Online";
		panelRoomName.GetComponent<Image> ().color = chooseRoomOptionColor [1];
		panelRoomMode.GetComponent<Image> ().color = chooseRoomOptionColor [0];
		panelRoomSize.GetComponent<Image> ().color = chooseRoomOptionColor [0];
		panelRoomMap.GetComponent<Image> ().color = chooseRoomOptionColor [0];

		Debug.Log ("RoomLength: " + roomList.Length);

		if (roomList.Length == 0) {
			RoomCountIndex = 0;

			LabelRoomName.GetComponent<Text> ().text = "NO ROOMS CURRENTLY ONLINE. TRY AGAIN LATER.";
			LabelRoomMode.GetComponent<Text> ().text = "";
			LabelRoomSize.GetComponent<Text> ().text = "";
			LabelRoomMap.GetComponent<Text> ().text = "";

			panelRoomMode.SetActive (false);
			panelRoomSize.SetActive (false);
			panelRoomMap.SetActive (false);

			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [2]);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [2]);
			}
			else if (Input.GetKeyDown (KeyCode.Backspace)) {
				optionLevel = OptionLevelEnum.OptionLevel1;
				//Restart the process again
				if (StartHomeBaseCO != null)
					StopCoroutine (StartHomeBaseCO);
				StartHomeBaseCO = null;
				StartHomeBaseCO = StartHomeBase_CO ();
				StartCoroutine (StartHomeBaseCO);

			}
		} else {
			
			foreach (RoomInfo game in roomList) {
				totalPlayers += game.PlayerCount;
			}

			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (RoomCountIndex > 0) {
					RoomCountIndex -= 1;
					LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
				} else {
					LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [2]);
				}
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (RoomCountIndex < (TotalRoomCount - 1)) {
					LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [1]);
					RoomCountIndex += 1;
				} else {
					LobbySoundObj.GetComponent<AudioSource> ().PlayOneShot (LobbySounds [2]);
				}
			} else if (Input.GetKeyDown (KeyCode.Backspace)) {
				optionLevel = OptionLevelEnum.OptionLevel1;
				//Restart the process again
				if (StartHomeBaseCO != null)
					StopCoroutine (StartHomeBaseCO);
				StartHomeBaseCO = null;
				StartHomeBaseCO = StartHomeBase_CO ();
				StartCoroutine (StartHomeBaseCO);

			}
			RoomCountIndex = RoomCountIndex <= 0 ? 0 : RoomCountIndex;
			RoomCountIndex = RoomCountIndex >= TotalRoomCount ? TotalRoomCount : RoomCountIndex;

			panelRoomMode.SetActive (true);
			panelRoomSize.SetActive (true);
			panelRoomMap.SetActive (true);

			if (roomList != null) {
				LabelRoomName.GetComponent<Text> ().text = roomList [RoomCountIndex].Name.ToString ().ToUpper ();
				LabelRoomMode.GetComponent<Text> ().text = roomList [RoomCountIndex].CustomProperties [RoomProperties.GameMode].ToString ().ToUpper ();
				LabelRoomSize.GetComponent<Text> ().text = roomList [RoomCountIndex].PlayerCount.ToString () + "  ::  " + roomList [0].MaxPlayers.ToString () + " PLAYERS";
				LabelRoomMap.GetComponent<Text> ().text = roomList [RoomCountIndex].CustomProperties [RoomProperties.Map].ToString ().ToUpper ();
			}

		}

	}


	public void InitializeMainStatistics(){
		if (!hasInitializedSteamMainStatisticsDisplay) {
			hasInitializedSteamMainStatisticsDisplay = true;
			//Main Statistics Display
			if (Steamworks.SteamUserStats.GetStat ("Kills", out statKills))
				LobbyPanel.GetComponent<LobbyInput> ().Stat_Kills.GetComponent<Text> ().text = statKills.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("Deaths", out statDeaths))
				LobbyPanel.GetComponent<LobbyInput> ().Stat_Deaths.GetComponent<Text> ().text = statDeaths.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("Assists", out statAssists))
				LobbyPanel.GetComponent<LobbyInput> ().Stat_Assists.GetComponent<Text> ().text = statAssists.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("DamageDealt", out statDamageDealt))
				LobbyPanel.GetComponent<LobbyInput> ().Stat_DamageDealt.GetComponent<Text> ().text = statDamageDealt.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("DamageReceived", out statDamageReceived))
				LobbyPanel.GetComponent<LobbyInput> ().Stat_DamageReceived.GetComponent<Text> ().text = statDamageReceived.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("BestKillStreak", out statBestKillStreak))
				LobbyPanel.GetComponent<LobbyInput> ().Stat_Killstreak.GetComponent<Text> ().text = statBestKillStreak.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("GamesPlayed", out statGamesPlayed))
				LobbyPanel.GetComponent<LobbyInput> ().Stat_GamesPlayed.GetComponent<Text> ().text = statGamesPlayed.ToString ();
		}
	}

	public void InitializeAdvancedStatistics(){
		if (!hasInitializedSteamAdvancedStatisticsDisplay) {
			hasInitializedSteamAdvancedStatisticsDisplay = true;
			//Main Statistics Display
			if (Steamworks.SteamUserStats.GetStat ("Kills", out statKills))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatKills.GetComponent<Text> ().text = statKills.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("Deaths", out statDeaths))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatDeaths.GetComponent<Text> ().text = statDeaths.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("Assists", out statAssists))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAssists.GetComponent<Text> ().text = statAssists.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("DamageDealt", out statDamageDealt))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatDamageDealt.GetComponent<Text> ().text = statDamageDealt.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("DamageReceived", out statDamageReceived))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatDamageReceived.GetComponent<Text> ().text = statDamageReceived.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("BestKillStreak", out statBestKillStreak))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatBestKillStreak.GetComponent<Text> ().text = statBestKillStreak.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("GamesPlayed", out statGamesPlayed))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().GamesPlayed.GetComponent<Text> ().text = statGamesPlayed.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("Wins", out statGamesWon))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().GamesWon.GetComponent<Text> ().text = statGamesWon.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("Losses", out statGamesLost))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().GamesLost.GetComponent<Text> ().text = statGamesLost.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("Draws", out statGamesTied))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().GamesTied.GetComponent<Text> ().text = statGamesTied.ToString ();
			//ACCURACY STATS
			if (Steamworks.SteamUserStats.GetStat ("ShotsFired", out statFired))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredTotal.GetComponent<Text> ().text = statFired.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("ShotsFiredHMG", out statFiredHMG))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredHMG.GetComponent<Text> ().text = statFiredHMG.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("ShotsFiredRocket", out statFiredRocket))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredRocket.GetComponent<Text> ().text = statFiredRocket.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("ShotsFiredRail", out statFiredRail))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredRail.GetComponent<Text> ().text = statFiredRail.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("ShotsFiredEMP", out statFiredEMP))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredEMP.GetComponent<Text> ().text = statFiredEMP.ToString ();

			if (Steamworks.SteamUserStats.GetStat ("ShotsHit", out statHit))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitTotal.GetComponent<Text> ().text = statHit.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("ShotsHitHMG", out statHitHMG))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitHMG.GetComponent<Text> ().text = statHitHMG.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("ShotsHitRocket", out statHitRocket))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitRocket.GetComponent<Text> ().text = statHitRocket.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("ShotsHitRail", out statHitRail))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitRail.GetComponent<Text> ().text = statHitRail.ToString ();
			if (Steamworks.SteamUserStats.GetStat ("ShotsHitEMP", out statHitEMP))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitEMP.GetComponent<Text> ().text = statHitEMP.ToString ();

			if (Steamworks.SteamUserStats.GetStat ("Accuracy", out statAccuracy))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracy.GetComponent<Text> ().text = ((int)(statAccuracy * 100)).ToString () + " %";
			if (Steamworks.SteamUserStats.GetStat ("AccuracyHMG", out statAccuracyHMG))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracyHMG.GetComponent<Text> ().text = ((int)(statAccuracyHMG * 100)).ToString () + " %";
			if (Steamworks.SteamUserStats.GetStat ("AccuracyRocket", out statAccuracyRocket))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracyRocket.GetComponent<Text> ().text = ((int)(statAccuracyRocket * 100)).ToString () + " %";
			if (Steamworks.SteamUserStats.GetStat ("AccuracyRail", out statAccuracyRail))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracyRail.GetComponent<Text> ().text = ((int)(statAccuracyRail * 100)).ToString () + " %";
			if (Steamworks.SteamUserStats.GetStat ("AccuracyEMP", out statAccuracyEMP))
				PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracyEMP.GetComponent<Text> ().text = ((int)(statAccuracyEMP * 100)).ToString () + " %";
		}
	}

	private bool isResetKill = false;
	private bool isResetDeaths = false;
	private bool isResetDamageD = false;
	private bool isResetDamageR = false;
	private bool isResetAssists = false;
	private bool isResetKillStreak = false;
	private bool isResetGamesPlayed = false;
	private bool isResetGamesWon = false;
	private bool isResetGamesLost = false;
	private bool isResetGamesTied = false;
	private bool isResetFired = false;
	private bool isResetFiredHMG = false;
	private bool isResetFiredRocket = false;
	private bool isResetFiredRail = false;
	private bool isResetFiredEMP = false;
	private bool isResetHit = false;
	private bool isResetHitHMG = false;
	private bool isResetHitRocket = false;
	private bool isResetHitRail = false;
	private bool isResetHitEMP = false;
	private bool isResetAccuracy = false;
	private bool isResetAccuracyHMG = false;
	private bool isResetAccuracyRocket = false;
	private bool isResetAccuracyRail = false;
	private bool isResetAccuracyEMP = false;


	public void resetSteamStatsistics(){
		Steamworks.SteamUserStats.ResetAllStats (true);
		//MAIN STATS
		if (Steamworks.SteamUserStats.GetStat ("Kills", out statKills)) {
			LobbyPanel.GetComponent<LobbyInput> ().Stat_Kills.GetComponent<Text> ().text = statKills.ToString ();
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatKills.GetComponent<Text> ().text = statKills.ToString ();
			isResetKill = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("Deaths", out statDeaths)) {
			LobbyPanel.GetComponent<LobbyInput> ().Stat_Deaths.GetComponent<Text> ().text = statDeaths.ToString ();
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatDeaths.GetComponent<Text> ().text = statDeaths.ToString ();
			isResetDeaths = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("Assists", out statAssists)) {
			LobbyPanel.GetComponent<LobbyInput> ().Stat_Assists.GetComponent<Text> ().text = statAssists.ToString ();
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAssists.GetComponent<Text> ().text = statAssists.ToString ();
			isResetAssists = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("DamageDealt", out statDamageDealt)) {
			LobbyPanel.GetComponent<LobbyInput> ().Stat_DamageDealt.GetComponent<Text> ().text = statDamageDealt.ToString ();
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatDamageDealt.GetComponent<Text> ().text = statDamageDealt.ToString ();
			isResetDamageD = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("DamageReceived", out statDamageReceived)) {
			LobbyPanel.GetComponent<LobbyInput> ().Stat_DamageReceived.GetComponent<Text> ().text = statDamageReceived.ToString ();
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatDamageReceived.GetComponent<Text> ().text = statDamageReceived.ToString ();
			isResetDamageR = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("BestKillStreak", out statBestKillStreak)) {
			LobbyPanel.GetComponent<LobbyInput> ().Stat_Killstreak.GetComponent<Text> ().text = statBestKillStreak.ToString ();
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatBestKillStreak.GetComponent<Text> ().text = statBestKillStreak.ToString ();
			isResetKillStreak = true;
		}
		//GAMES PLAYED
		if (Steamworks.SteamUserStats.GetStat ("GamesPlayed", out statGamesPlayed)) {
			LobbyPanel.GetComponent<LobbyInput> ().Stat_GamesPlayed.GetComponent<Text> ().text = statGamesPlayed.ToString ();
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().GamesPlayed.GetComponent<Text> ().text = statGamesPlayed.ToString ();
			isResetGamesPlayed = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("Wins", out statGamesWon)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().GamesWon.GetComponent<Text> ().text = statGamesWon.ToString ();
			isResetGamesWon = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("Losses", out statGamesLost)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().GamesLost.GetComponent<Text> ().text = statGamesLost.ToString ();
			isResetGamesLost = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("Draws", out statGamesTied)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().GamesTied.GetComponent<Text> ().text = statGamesTied.ToString ();
			isResetGamesTied = true;
		}
		//ACCURACY FIRED / HIT / PERCENT
		if (Steamworks.SteamUserStats.GetStat ("ShotsHit", out statHit)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitTotal.GetComponent<Text> ().text = statHit.ToString ();
			isResetHit = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("ShotsFired", out statFired)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredTotal.GetComponent<Text> ().text = statFired.ToString ();
			isResetFired = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("ShotsHitHMG", out statHitHMG)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitHMG.GetComponent<Text> ().text = statHitHMG.ToString ();
			isResetHitHMG = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("ShotsHitRocket", out statHitRocket)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitRocket.GetComponent<Text> ().text = statHitRocket.ToString ();
			isResetHitRocket = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("ShotsHitRail", out statHitRail)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitRail.GetComponent<Text> ().text = statHitRail.ToString ();
			isResetHitRail = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("ShotsHitEMP", out statHitEMP)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatHitEMP.GetComponent<Text> ().text = statHitEMP.ToString ();
			isResetHitEMP = true;
		}
		//Fired
		if (Steamworks.SteamUserStats.GetStat ("ShotsFiredHMG", out statFiredHMG)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredHMG.GetComponent<Text> ().text = statFiredHMG.ToString ();
			isResetFiredHMG = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("ShotsFiredRocket", out statFiredRocket)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredRocket.GetComponent<Text> ().text = statFiredRocket.ToString ();
			isResetFiredRocket = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("ShotsFiredRail", out statFiredRail)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredRail.GetComponent<Text> ().text = statFiredRail.ToString ();
			isResetFiredRail = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("ShotsFiredEMP", out statFiredEMP)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatFiredEMP.GetComponent<Text> ().text = statFiredEMP.ToString ();
			isResetFiredEMP = true;
		}
		//Acc
		if (Steamworks.SteamUserStats.GetStat ("Accuracy", out statAccuracy)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracy.GetComponent<Text> ().text = statAccuracy.ToString () + " %";
			isResetAccuracy = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("AccuracyHMG", out statAccuracyHMG)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracyHMG.GetComponent<Text> ().text = statAccuracyHMG.ToString () + " %";
			isResetAccuracyHMG = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("AccuracyRocket", out statAccuracyRocket)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracyRocket.GetComponent<Text> ().text = statAccuracyRocket.ToString () + " %";
			isResetAccuracyRocket = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("AccuracyRail", out statAccuracyRail)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracyRail.GetComponent<Text> ().text = statAccuracyRail.ToString () + " %";
			isResetAccuracyRail = true;
		}
		if (Steamworks.SteamUserStats.GetStat ("AccuracyEMP", out statAccuracyEMP)) {
			PlayerStatsPanel.GetComponent<PlayerStatsPanelInput> ().StatAccuracyEMP.GetComponent<Text> ().text = statAccuracyEMP.ToString () + " %";
			isResetAccuracyEMP = true;
		}



		Steamworks.SteamUserStats.StoreStats ();
		Button_HomeBase ();

	}
		


	void Update()
	{
		if (roomListManager == null) {
			if (PlayerPrefs.GetInt ("ViewRoomList") == 1) {
				SteamBeginPanel.SetActive (false);
			}
			return;
		}




		if(!SteamManager.Initialized) {
			SteamErrorPanel.SetActive (true);
			SteamBeginPanel.SetActive (false);
			return;
		}

//		if (StartHomeBaseCO == null) {
//			StartHomeBaseCO = StartHomeBase_CO ();
//			StartCoroutine (StartHomeBaseCO);
//		}


		if (Steamworks.SteamFriends.GetPersonaState () == Steamworks.EPersonaState.k_EPersonaStateOnline) {			
			if (!gotRoomsUpdate) {
				if (PlayerPrefs.GetInt ("ViewRoomList") == 1) {
					SteamBeginPanel.SetActive (false);
				}
				return;
			}
		}

		if (PlayerPrefs.GetInt ("BeginChallengeFromHomeBase") == 1) {
//			Debug.Log ("PlayerPrefs.GetInt (\"BeginChallengeFromHomeBase\")  " + PlayerPrefs.GetInt ("BeginChallengeFromHomeBase"));
			UIContainer.SetActive (false);
			//The Player Pref Setting will be reset to 0 in Network Manager, once game has loaded.
			if (PhotonNetwork.connected && PhotonNetwork.insideLobby) {
				Button_StartCampaign (1);
			} else {
				try {
					Button_StartCampaign (1);
				} catch {
					Debug.Log ("Failed To Start Campaign in Offline mode");
				}
			}
		} else if (PlayerPrefs.GetInt ("BeginChallengeFromHomeBase") == 2) {
			UIContainer.SetActive (false);
			//The Player Pref Setting will be reset to 0 in Network Manager, once game has loaded.
			if (PhotonNetwork.connected && PhotonNetwork.insideLobby) {
				Button_StartCampaign (2);
			} else {
				try {
					Button_StartCampaign (2);
				} catch {
					Debug.Log ("Failed To Start Campaign in Offline mode");
				}
			}
		}

		if (PlayerPrefs.GetInt ("startMultiplayerFromHomeBaseKey") == 1) {
			UIContainer.SetActive (false);
			if (PhotonNetwork.connected && PhotonNetwork.insideLobby) {
				//The Player Pref Setting will be reset to 0 in Network Manager, once game has loaded.
				Button_StartMultiplayer ();
			}
		}




//		//Get out of View Room List UI
//		if (PlayerPrefs.GetInt ("ViewRoomList") == 1) {
//			SteamBeginPanel.SetActive (false);
//			LobbyPanel.SetActive(true);
//			if (Input.GetKeyDown (KeyCode.Escape)) {
//				PlayerPrefs.SetInt ("ViewRoomList", 0);
//				PlayerPrefs.Save ();
//			} else if (Input.GetKeyDown (KeyCode.Return)) {
//				PlayerPrefs.SetInt ("ViewRoomList", 0);
//				PlayerPrefs.Save ();
//				JoinOnlineRoom ();	
//			}
//
//			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
//				Button_RoomPrev ();
//			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
//				Button_RoomNext ();
//			}
//
//
//		}

		//Get out of View Room List UI
		if (PlayerPrefs.GetInt ("ViewRoomList") == 1) {
			UIContainer.SetActive (false);
			if (PhotonNetwork.connected && PhotonNetwork.insideLobby) {
				PlayerPrefs.SetInt ("ViewRoomList", 0);
				PlayerPrefs.Save ();
				JoinOnlineRoom ();
			}
		}



			
		if (mapSelectNumber == 0) {
			loadingPercent = (int)(Application.GetStreamProgressForLevel (2) * 100);
		} else if (mapSelectNumber == 1) {
			loadingPercent = (int)(Application.GetStreamProgressForLevel (3) * 100);
		}
	}

	public void SetPlayerNameInput(string val){
		if (isRememberMe) {
			PlayerPrefs.SetString ("playerName", val);
		}
		PhotonNetwork.playerName = val;
	}

	public void SetRegionDropDownInput(){

		regionIndex = UsernamePanel.GetComponent<UsernamePanelInput> ().regionDropDownInput.GetComponent<Dropdown> ().value;
		if (regionIndex == 0) {
			clientRegion = CloudRegionCode.asia;
			regionText = "ASIA";
		} else if (regionIndex == 1) {
			clientRegion = CloudRegionCode.au;
			regionText = "AUSTRALIA";
		} else if (regionIndex == 2) {
			clientRegion = CloudRegionCode.cae;
			regionText = "CANADA EAST";
		} else if (regionIndex == 3) {
			clientRegion = CloudRegionCode.eu;
			regionText = "EUROPE";
		} else if (regionIndex == 4) {
			clientRegion = CloudRegionCode.jp;
			regionText = "JAPAN";
		} else if (regionIndex == 5) {
			clientRegion = CloudRegionCode.sa;
			regionText = "SOUTH AMERICA";
		} else if (regionIndex == 6) {
			clientRegion = CloudRegionCode.us;
			regionText = "UNITED STATES EAST";
		} else if (regionIndex == 7) {
			clientRegion = CloudRegionCode.usw;
			regionText = "UNITED STATES WEST";
		}
	}

	public void SetMapNameDropDownInput(){
		mapSelectNumber = LobbyPanel.GetComponent<LobbyInput> ().Create_MapNameDropDownInput.GetComponent<Dropdown> ().value;

		if (LobbyPanel.GetComponent<LobbyInput> ().Create_ImageMapPreview.GetActive()==false)
			LobbyPanel.GetComponent<LobbyInput> ().Create_ImageMapPreview.SetActive (true);
		LobbyPanel.GetComponent<LobbyInput> ().Create_ImageMapPreview.GetComponent<Image> ().sprite = LobbyPanel.GetComponent<LobbyInput> ().mapSpriteList [mapSelectNumber];
	}

	public void SetGameModeDropDownInput(){
		gameMode = LobbyPanel.GetComponent<LobbyInput> ().Create_GameModeDropDownInput.GetComponent<Dropdown> ().value;
	}

	public void SetRoomNameInput(string val){
		roomName = val + Random.Range(1,99999).ToString();
		Debug.Log ("Room Name input: " + roomName);
	}


	void RegionAssign(int playerPrefRegion){
		if (playerPrefRegion == 0) {
			clientRegion = CloudRegionCode.asia;
			regionText = "ASIA";
		} else if (playerPrefRegion == 1) {
			clientRegion = CloudRegionCode.au;
			regionText = "AUSTRALIA";
		} else if (playerPrefRegion == 2) {
			clientRegion = CloudRegionCode.cae;
			regionText = "CANADA EAST";
		} else if (playerPrefRegion == 3) {
			clientRegion = CloudRegionCode.eu;
			regionText = "EUROPE";
		} else if (playerPrefRegion == 4) {
			clientRegion = CloudRegionCode.jp;
			regionText = "JAPAN";
		} else if (playerPrefRegion == 5) {
			clientRegion = CloudRegionCode.sa;
			regionText = "SOUTH AMERICA";
		} else if (playerPrefRegion == 6) {
			clientRegion = CloudRegionCode.us;
			regionText = "UNITED STATES EAST";
		} else if (playerPrefRegion == 7) {
			clientRegion = CloudRegionCode.usw;
			regionText = "UNITED STATES WEST";
		}
	}

	public void EnteredUserName(){
		int playerPrefRegion = PlayerPrefs.GetInt ("Region");

		if (playerPrefRegion == -1) {
			PlayerPrefs.SetInt ("Region", regionIndex);
			PlayerPrefs.Save ();
		}

		RegionAssign (playerPrefRegion);
//		if (playerPrefRegion == 0) {
//			clientRegion = CloudRegionCode.asia;
//			regionText = "ASIA";
//		} else if (playerPrefRegion == 1) {
//			clientRegion = CloudRegionCode.au;
//			regionText = "AUSTRALIA";
//		} else if (playerPrefRegion == 2) {
//			clientRegion = CloudRegionCode.cae;
//			regionText = "CANADA EAST";
//		} else if (playerPrefRegion == 3) {
//			clientRegion = CloudRegionCode.eu;
//			regionText = "EUROPE";
//		} else if (playerPrefRegion == 4) {
//			clientRegion = CloudRegionCode.jp;
//			regionText = "JAPAN";
//		} else if (playerPrefRegion == 5) {
//			clientRegion = CloudRegionCode.sa;
//			regionText = "SOUTH AMERICA";
//		} else if (playerPrefRegion == 6) {
//			clientRegion = CloudRegionCode.us;
//			regionText = "UNITED STATES EAST";
//		} else if (playerPrefRegion == 7) {
//			clientRegion = CloudRegionCode.usw;
//			regionText = "UNITED STATES WEST";
//		}

		PhotonNetwork.playerName = "id_" + Random.Range (0, 999999);

		SteamBeginPanel.SetActive (false);

		#if UNITY_EDITOR
		if (SteamFriends.GetPersonaName () != null) {
			PhotonNetwork.playerName = SteamFriends.GetPersonaName ();
			hasProvidedUserName = true;
			SteamErrorPanel.SetActive (false);
		}
		#endif

		#if UNITY_STANDALONE
		if (SteamFriends.GetPersonaName () != null) {
			PhotonNetwork.playerName = SteamFriends.GetPersonaName ();
			hasProvidedUserName = true;
			SteamErrorPanel.SetActive (false);
		}
		#endif

		PlayerPrefs.SetInt ("ViewRegion", 0);
		PlayerPrefs.Save ();

		if (PlayerPrefs.GetInt ("ViewRoomList") != 1 && PlayerPrefs.GetInt ("startMultiplayerFromHomeBaseKey") != 1 && PlayerPrefs.GetInt ("BeginChallengeFromHomeBase") != 1 && PlayerPrefs.GetInt ("BeginChallengeFromHomeBase") != 2) {
			if (PlayerPrefs.GetInt ("RefreshRoomList") == 1) {
				PlayerPrefs.SetInt ("RefreshRoomList", 0);
				PlayerPrefs.SetInt ("HasRefreshedRoomList", 1);
				PlayerPrefs.Save ();
				UIContainer.SetActive (false);
//				Button_HomeBase ();
			}


			UIContainer.SetActive (false);
			if (mainMenuOption == MainMenuOptionEnum.HomeBase) {
				Button_HomeBase ();
			} else if (mainMenuOption == MainMenuOptionEnum.SinglePlayer || mainMenuOption == MainMenuOptionEnum.MultiPlayer) {
				SinglePlayerCO = SinglePlayer_CO ();
				StartCoroutine (SinglePlayerCO);
			}
		}

		UsernamePanel.SetActive (false);
		if (Steamworks.SteamFriends.GetPersonaState () != Steamworks.EPersonaState.k_EPersonaStateOffline) {			
			PhotonNetwork.ConnectToRegion (clientRegion, this.gameVersion);
		} else { 
			//Do nothing... 
		}

	}

	IEnumerator SinglePlayerCO;
	IEnumerator SinglePlayer_CO(){
		if (Steamworks.SteamFriends.GetPersonaState () != Steamworks.EPersonaState.k_EPersonaStateOffline) {						
			while (!(PhotonNetwork.connected && PhotonNetwork.insideLobby)) {
				yield return true;
			}
			Debug.Log ("Connected: " + PhotonNetwork.connected + ", InLobby: " + PhotonNetwork.insideLobby);
			Button_SinglePlayer (1);
		} else {
			Debug.Log ("steam offline");
			Button_SinglePlayer (1);
		}
		SinglePlayerCO = null;
	}

	public void SetJoinRoomNameInput(string val){
		joinRoomName = val;
		Debug.Log ("Join Roon Name Input: " + joinRoomName);
	}

	public void SetRoomSizeSliderInput(float numberPlayers){
		roomSize = numberPlayers;
		Debug.Log ("Room Size input: " + (byte)roomSize);
	}

	public void SetGameTimeSliderInput(float timeValue){
		gameTimeDays = (int)timeValue;
		Debug.Log ("Game Time input: " + gameTimeDays);
	}

	public void ErrorMessageConfirmButtonBehavior(){
		ErrorPanel.SetActive (false);
	}

	public void Button_ActivatePlayerStats(){
		PlayerStatsPanel.SetActive (true);	
	}	
	public void Button_DeactivatePlayerStats(){
		PlayerStatsPanel.SetActive (false);	
	}

	public void QuitFromLobby(){
		Application.Quit ();
	}

	public void Button_CreateNewRoomMenu(){

		LobbyInput input = LobbyPanel.GetComponent<LobbyInput> ();
		isCreateMenu = true;
		OnReceivedRoomListUpdate__OLD ();

		Debug.Log ("iscreatedmenu: " + isCreateMenu);
		input.Lobby_Div.SetActive (false);
		input.Lobby_GameMode.SetActive (false);
		input.Lobby_RoomName.SetActive (false);
		input.Lobby_Map.SetActive (false);
		input.Lobby_MasterClient.SetActive (false);
		input.Lobby_Region.SetActive (false);
		input.Lobby_RoomCapCount.SetActive (false);
		input.Menu_ButtonCreateNew.SetActive (false);
		input.Menu_ButtonHomeBase.SetActive (false);
		input.Menu_ButtonCampaign.SetActive (false);

		input.Create_Div.SetActive (true);
		input.Create_RoomSizeCount.gameObject.SetActive (true);
		input.Create_GameTimeCount.gameObject.SetActive (true);
		input.Create_RoomsizeSlider.SetActive (true);
		input.Create_TimeSlider.SetActive (true);
		input.Create_GameModeDropDownInput.SetActive (true);
		input.Create_MapNameDropDownInput.SetActive (true);
		input.Create_GameRoomInput.SetActive (true);
		input.Create_GameRoomLabel.SetActive (true);
		input.Create_ButtonCreateRoom.SetActive (true);
		input.Create_ButtonCreateLocalRoom.SetActive (true);
		input.Create_ButtonCreateRoom.SetActive (true);
		input.Create_ButtonCancel.SetActive (true);
		input.Create_ImageMapPreview.SetActive (true);
		input.Hidden_Div.SetActive (true);
		input.Hidden_InputRoomName.SetActive (true);
		input.Hidden_LabelHiddenRoomName.SetActive (true);
		input.Hidden_ButtonJoin.SetActive (true);
	}

	public void Button_CancelCreate(){

		LobbyInput input = LobbyPanel.GetComponent<LobbyInput> ();
		isCreateMenu = false;
		OnReceivedRoomListUpdate__OLD ();

		input.Lobby_Div.SetActive (true);
		input.Lobby_GameMode.SetActive (true);
		input.Lobby_RoomName.SetActive (true);
		input.Lobby_Map.SetActive (true);
		input.Lobby_MasterClient.SetActive (true);
		input.Lobby_Region.SetActive (true);
		input.Lobby_RoomCapCount.SetActive (true);
		input.Menu_ButtonCreateNew.SetActive (true);
		input.Menu_ButtonHomeBase.SetActive (true);
		input.Menu_ButtonCampaign.SetActive (true);

		input.Create_Div.SetActive (false);
		input.Create_RoomSizeCount.gameObject.SetActive (false);
		input.Create_GameTimeCount.gameObject.SetActive (false);
		input.Create_RoomsizeSlider.SetActive (false);
		input.Create_TimeSlider.SetActive (false);
		input.Create_GameModeDropDownInput.SetActive (false);
		input.Create_MapNameDropDownInput.SetActive (false);
		input.Create_GameRoomInput.SetActive (false);
		input.Create_GameRoomLabel.SetActive (false);
		input.Create_ButtonCreateRoom.SetActive (false);
		if (!PhotonNetwork.connected) {
			input.Create_ButtonCreateLocalRoom.SetActive (false);
			input.Create_ButtonCreateRoom.SetActive (false);
			Debug.Log ("PhotonNetwork is not connected..");
		} else {
			input.Create_ButtonCreateLocalRoom.SetActive (false);
			input.Create_ButtonCreateRoom.SetActive (false);
		}
		input.Create_ButtonCancel.SetActive (false);
		input.Create_ImageMapPreview.SetActive (false);
		input.Hidden_Div.SetActive (false);
		input.Hidden_InputRoomName.SetActive (false);
		input.Hidden_LabelHiddenRoomName.SetActive (false);
		input.Hidden_ButtonJoin.SetActive (false);
	}

	public void Button_RegionReset(){
		PlayerPrefs.SetInt ("Region", -1);
		PlayerPrefs.Save();
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);
	}

	public void ToggleRememberMe(){
		if (UsernamePanel.GetComponent<UsernamePanelInput> ().toggle_rememberMe.GetComponent<Toggle> ().isOn) {
			isRememberMe = true;
		} else {
			isRememberMe = false;
			PlayerPrefs.SetString ("playerName", "");
		}
		int rememberflag = isRememberMe == true ? 1 : 0;
		PlayerPrefs.SetInt ("IsRememberMe", rememberflag);
	}

	//TEMP CAMPAIGN

	public void Button_SinglePlayer(int sizeOfRoom){
		//		gameMode = 2;



//		gameMode = 2;
//		mapSelectNumber = 0;
//		roomSize = 1;
//		gameTimeDays = 7;
		roomName = PhotonNetwork.player.name + "'s Room";

		//		gameMode = 2;
		////		mapSelectNumber = 4;
		//		mapSelectNumber = 0;
		//		roomSize = sizeOfRoom;
		//		//10 DAYS
		//		gameTimeDays = 10;
		//		roomName = PhotonNetwork.player.name + "'s Campaign";
		if (Steamworks.SteamFriends.GetPersonaState () == Steamworks.EPersonaState.k_EPersonaStateOffline || roomSize <= 1) {
			Debug.Log ("Campaign is in Offline mode");
			ButtonBehavior_OfflineMode ();
		} else {
			Debug.Log("Campaign is connected online");
			StartGameButtonBehavior ();
		}
	}


	public void Button_StartCampaign(int sizeOfRoom){
//		gameMode = 2;
		gameMode = PlayerPrefs.GetInt ("playerPrefsGameModeKey");
		mapSelectNumber = PlayerPrefs.GetInt ("playerPrefsGameMapKey");
		roomSize = PlayerPrefs.GetInt ("playerPrefsPlayerCountKey");
		gameTimeDays = PlayerPrefs.GetInt ("playerPrefsGameTimeKey");
		roomName = PhotonNetwork.player.name + "'s Room";

//		gameMode = 2;
////		mapSelectNumber = 4;
//		mapSelectNumber = 0;
//		roomSize = sizeOfRoom;
//		//10 DAYS
//		gameTimeDays = 10;
//		roomName = PhotonNetwork.player.name + "'s Campaign";
		if (Steamworks.SteamFriends.GetPersonaState () == Steamworks.EPersonaState.k_EPersonaStateOffline) {
			Debug.Log("Campaign is in Offline mode");
			ButtonBehavior_OfflineMode ();
		} else {
			Debug.Log("Campaign is connected online");
			StartGameButtonBehavior ();
		}
	}
		

	public void Button_HomeBase(){

		if (!hasProvidedUserName)
			return;

		gameMode = 99;
		mapSelectNumber = 1;
		roomSize = 1;
		roomName = PhotonNetwork.player.name + "'s Home Base";
		ButtonBehavior_OfflineMode ();
	}

	public void Button_StartMultiplayer(){
		gameMode = PlayerPrefs.GetInt ("playerPrefsGameModeKey");
		mapSelectNumber = PlayerPrefs.GetInt ("playerPrefsGameMapKey");
		roomSize = PlayerPrefs.GetInt ("playerPrefsPlayerCountKey");
		gameTimeDays = PlayerPrefs.GetInt ("playerPrefsGameTimeKey");
		roomName = PhotonNetwork.player.name + "'s Room";

		Debug.Log("Multiplayer Game is connected online");
		StartGameButtonBehavior ();
	}

	public void StartGameButtonBehavior(){
		if (!(PhotonNetwork.connected && PhotonNetwork.insideLobby)) {
			ErrorPanel.GetComponent<ErrorPanelInputs> ().ErrorPanelText.GetComponent<Text> ().text = "YOU CANNOT START ONLINE MODE WITH NO INTERNET CONNECTIOn";
			ErrorPanel.SetActive (true);
			return;
		}
		Debug.Log ("Joined online room");

		string gmString = "";
		if (gameMode == 0) {
			gmString = "TILE DYNASTY";
		} else if (gameMode == 1) {
			gmString = "TEAM DEATH MATCH";
		} else if (gameMode == 2) {
			gmString = "SURVIVAL";
		} else if (gameMode == 3) {
			gmString = "GARBAGE MAN";
		} else if (gameMode == 99) {
			gmString = "HOME BASE MODE";
		}

		string mapString = "";
		if (mapSelectNumber == 0) {
			mapString = "MILITARY BASE";
		} else if (mapSelectNumber == 1) {
			mapString = "HOME BASE";
		}



		RoomOptions roomOptions = new RoomOptions();
		roomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
		roomOptions.maxPlayers = (byte)roomSize;
		roomOptions.customRoomProperties.Add (RoomProperties.Room, roomName);
		roomOptions.customRoomProperties.Add (RoomProperties.Map, mapString);
		roomOptions.customRoomProperties.Add (RoomProperties.GameMode, gmString);
		roomOptions.customRoomProperties.Add (RoomProperties.BlueTeamKills, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.RedTeamKills, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.BlueTilePoints, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.RedTilePoints, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.BlueTilesOwned, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.RedTilesOwned, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.CurrentWave, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.CurrentEnemyKills, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.CreditsRemaining, 3);
		roomOptions.customRoomProperties.Add (RoomProperties.KillsPerWave, 5);
		roomOptions.customRoomProperties.Add (RoomProperties.MasterClient, PhotonNetwork.playerName);
		roomOptions.CustomRoomProperties.Add (RoomProperties.Region, regionText);
		roomOptions.customRoomProperties.Add (RoomProperties.blueAvailableCount, Mathf.Max (1, Mathf.FloorToInt (roomSize / 2f)));
		roomOptions.customRoomProperties.Add (RoomProperties.redAvailableCount, Mathf.Max (1, Mathf.FloorToInt (roomSize / 2f)));

		roomOptions.customRoomPropertiesForLobby = new string[] {
			RoomProperties.Map,
			RoomProperties.GameMode,
			RoomProperties.blueAvailableCount,
			RoomProperties.redAvailableCount,
			RoomProperties.MasterClient,
			RoomProperties.Region,
			roomOptions.maxPlayers.ToString(),
		};

		PhotonNetwork.CreateRoom (roomName, roomOptions, TypedLobby.Default);
//		LoadingPanel.SetActive (true);
	}

	public void JoinGameButtonBehavior(){

		if (PhotonNetwork.GetRoomList ().Length > 0) {
			PhotonNetwork.JoinRoom (joinRoomName);
		} else {
			if (!PhotonNetwork.connected) {
				ErrorPanel.GetComponent<ErrorPanelInputs> ().ErrorPanelText.GetComponent<Text> ().text = "YOU CANNOT START ONLINE MODE WITH NO INTERNET CONNECTIOn";
				ErrorPanel.SetActive (true);
				return;
			} else {
				ErrorPanel.GetComponent<ErrorPanelInputs> ().ErrorPanelText.GetComponent<Text> ().text = "THERE ARE NO ROOMS CURRENTLY AVAILABLE";
				ErrorPanel.SetActive (true);
			}
		}
	}

	public void JoinRandomButtonBehavior(){

		if (PhotonNetwork.GetRoomList ().Length > 0) {
			PhotonNetwork.JoinRandomRoom ();
		} else {
			if (!PhotonNetwork.connected) {
				ErrorPanel.GetComponent<ErrorPanelInputs> ().ErrorPanelText.GetComponent<Text> ().text = "YOU CANNOT START ONLINE MODE WITH NO INTERNET CONNECTIOn";
				ErrorPanel.SetActive (true);
				return;
			} else {
				ErrorPanel.GetComponent<ErrorPanelInputs> ().ErrorPanelText.GetComponent<Text> ().text = "THERE ARE NO ROOMS CURRENTLY AVAILABLE";
				ErrorPanel.SetActive (true);
			}
		}
	}

	public void ButtonBehavior_OfflineMode(){

		PhotonNetwork.Disconnect ();
		PhotonNetwork.offlineMode = true;

		string gmString = "";
		if (gameMode == 0) {
			gmString = "TILE DYNASTY";
		} else if (gameMode == 1) {
			gmString = "TEAM DEATH MATCH";
		} else if (gameMode == 2) {
			gmString = "SURVIVAL";
		} else if (gameMode == 3) {
			gmString = "GARBAGE MAN";
		} else if (gameMode == 99) {
			gmString = "HOME BASE MODE";
		}

		string mapString = "";
		if (mapSelectNumber == 0) {
			mapString = "MILITARY BASE";
		} else if (mapSelectNumber == 1) {
			mapString = "HOME BASE";
		}

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
		roomOptions.maxPlayers = (byte)roomSize;
		roomOptions.customRoomProperties.Add (RoomProperties.Room, roomName);
		roomOptions.customRoomProperties.Add (RoomProperties.Map, mapString);
		roomOptions.customRoomProperties.Add (RoomProperties.GameMode, gmString);
		roomOptions.customRoomProperties.Add (RoomProperties.BlueTeamKills, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.RedTeamKills, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.BlueTilePoints, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.RedTilePoints, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.BlueTilesOwned, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.RedTilesOwned, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.CurrentWave, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.CurrentEnemyKills, 0);
		roomOptions.customRoomProperties.Add (RoomProperties.CreditsRemaining, 3);
		roomOptions.customRoomProperties.Add (RoomProperties.KillsPerWave, 5);
		roomOptions.customRoomProperties.Add (RoomProperties.MasterClient, PhotonNetwork.playerName);
		roomOptions.CustomRoomProperties.Add (RoomProperties.Region, regionText);
		roomOptions.customRoomProperties.Add (RoomProperties.blueAvailableCount, Mathf.Max (1, Mathf.FloorToInt (roomSize / 2f)));
		roomOptions.customRoomProperties.Add (RoomProperties.redAvailableCount, Mathf.Max (1, Mathf.FloorToInt (roomSize / 2f)));

		roomOptions.customRoomPropertiesForLobby = new string[] {
			RoomProperties.Map,
			RoomProperties.GameMode,
			RoomProperties.blueAvailableCount,
			RoomProperties.redAvailableCount,
			RoomProperties.BlueTeamKills,
			RoomProperties.RedTeamKills,
			RoomProperties.MasterClient,
			RoomProperties.Region,
			roomOptions.maxPlayers.ToString(),
		};

		PhotonNetwork.CreateRoom (roomName, roomOptions, TypedLobby.Default);

//		LoadingPanel.SetActive (true);

	}

	void OnGUI()
	{
		if (!Application.CanStreamedLevelBeLoaded (5) || !Application.CanStreamedLevelBeLoaded (4) || !Application.CanStreamedLevelBeLoaded (3) || !Application.CanStreamedLevelBeLoaded (2) || Application.GetStreamProgressForLevel (2) < 1 || Application.GetStreamProgressForLevel (3) < 1 || Application.GetStreamProgressForLevel (4) < 1 || Application.GetStreamProgressForLevel (5) < 1)
		{	//	if (!Application.CanStreamedLevelBeLoaded(2) || Application.GetStreamProgressForLevel(2) < 1)

			controllerCanvas.backgroundImage.SetActive (false);
			LobbyPanel.SetActive (false);
			FailedConnectionPanel.SetActive (false);
			ConnectingPanel.SetActive (true);
			if (photonConnectionFailed) {
				controllerCanvas.backgroundImage.SetActive (false);
				LobbyPanel.SetActive (false);
				FailedConnectionPanel.SetActive (true);
			}
//			LoadingPanel.SetActive (true);


			return;
		}

		screenWidth = Mathf.Min(Screen.width, 960);

		if (!PhotonNetwork.connected) {
			controllerCanvas.backgroundImage.SetActive (true);
			if (!hasProvidedUserName)
				return;

			LobbyPanel.SetActive (false);
			LoadingPanel.SetActive (false);
			FailedConnectionPanel.SetActive (false);
			ConnectingPanel.SetActive(false);
		}
		else if (PhotonNetwork.room == null) {
			//INSTEAD OF MAINGUI() BEING CALLED, WANT TO 'ENABLE' MAIN_PANEL GUI SYSTEM
			controllerCanvas.backgroundImage.SetActive (true);
			if (!hasProvidedUserName)
				return;

			FailedConnectionPanel.SetActive (false);
			ConnectingPanel.SetActive (false);

		} else {
			controllerCanvas.backgroundImage.SetActive (false);
			LobbyPanel.SetActive (false);
			FailedConnectionPanel.SetActive (false);
			ConnectingPanel.SetActive (false);

		}
	}



	public void JoinOnlineRoom(){

		joinOnlineRoomName = PlayerPrefs.GetString ("JoinOnlineRoomFromHomeBase");
		Debug.Log ("Player Pref Saved Room Name is: " + joinOnlineRoomName);

		if (TotalRoomCount > 0) {
			PhotonNetwork.JoinRoom (joinOnlineRoomName);
			UIContainer.SetActive (false);
			Debug.Log ("Joining Online Room: " + joinOnlineRoomName);
		} else {
			if (!PhotonNetwork.connected) {
				ErrorPanel.GetComponent<ErrorPanelInputs> ().ErrorPanelText.GetComponent<Text> ().text = "YOU CANNOT START ONLINE MODE WITH NO INTERNET CONNECTIOn";
				ErrorPanel.SetActive (true);
				return;
			} else {
				ErrorPanel.GetComponent<ErrorPanelInputs> ().ErrorPanelText.GetComponent<Text> ().text = "THERE ARE NO ROOMS CURRENTLY AVAILABLE";
				ErrorPanel.SetActive (true);
			}
		}
	}

	public void Button_RoomNext(){
		if (TotalRoomCount > 0) {
			RoomSelectionIndex += 1;
			if (RoomSelectionIndex > (TotalRoomCount-1)) {
				RoomSelectionIndex = (TotalRoomCount - 1);
			}

			Debug.Log ("Next Room: " + RoomSelectionIndex);

			UpdateDisplayCurrentRoom ();
		}
	}

	public void Button_RoomPrev(){
		if (TotalRoomCount > 0) {
			RoomSelectionIndex -= 1;
			if (RoomSelectionIndex < 0) {
				RoomSelectionIndex = 0;
			}

			Debug.Log ("Prev Room: " + RoomSelectionIndex);

			UpdateDisplayCurrentRoom ();
		}
	}

	void UpdateDisplayCurrentRoom(){
		if (TotalRoomCount > 0) {

			if (LobbyPanel.GetComponent<LobbyInput> ().Lobby_NoRoomsMessage.GetActive () == true) {
				RoomSelectionIndex = 0;
			}
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomName.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapText.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapImage.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMode.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMasterClient.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomPlayerCount.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomButtonNext.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomButtonPrev.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().TotalRoomCount.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomViewIndex.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().TotalPlayerCount.gameObject.SetActive (true);
			LobbyPanel.GetComponent<LobbyInput> ().Lobby_NoRoomsMessage.SetActive (false);
				
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomName.GetComponent<Text> ().text = roomList [RoomSelectionIndex].Name.ToString ();
			joinOnlineRoomName = LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomName.GetComponent<Text> ().text;
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapText.GetComponent<Text> ().text = roomList [RoomSelectionIndex].CustomProperties [RoomProperties.Map].ToString ();

			if (LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapText.GetComponent<Text> ().text == "MILITARY BASE") {
				LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapImage.GetComponent<Image> ().sprite = MapPreviewSpriteList [0];				
			} else if (LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapText.GetComponent<Text> ().text == "CAMPGROUNDS MOD") {
				LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapImage.GetComponent<Image> ().sprite = MapPreviewSpriteList [1];				
			} else if (LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapText.GetComponent<Text> ().text == "WINTER CAMP") {
				LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapImage.GetComponent<Image> ().sprite = MapPreviewSpriteList [2];				
			} else if (LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapText.GetComponent<Text> ().text == "WASTE LANDS") {
				LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapImage.GetComponent<Image> ().sprite = MapPreviewSpriteList [3];				
			}

			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMode.GetComponent<Text> ().text = roomList [RoomSelectionIndex].CustomProperties [RoomProperties.GameMode].ToString ();
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMasterClient.GetComponent<Text> ().text = roomList [RoomSelectionIndex].CustomProperties [RoomProperties.MasterClient].ToString ();
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomPlayerCount.GetComponent<Text> ().text = roomList [RoomSelectionIndex].PlayerCount.ToString () + " / " + roomList [RoomSelectionIndex].MaxPlayers.ToString ();

			if (TotalRoomCount == 1) {
				LobbyPanel.GetComponent<LobbyInput> ().TotalRoomCount.GetComponent<Text> ().text = TotalRoomCount.ToString () + " ROOM";
			} else {
				LobbyPanel.GetComponent<LobbyInput> ().TotalRoomCount.GetComponent<Text> ().text = TotalRoomCount.ToString () + " ROOMS";
			}
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomViewIndex.GetComponent<Text> ().text = "ROOM " + (RoomSelectionIndex + 1).ToString ();
			LobbyPanel.GetComponent<LobbyInput> ().TotalPlayerCount.GetComponent<Text>().text = totalPlayers.ToString () + " PLAYERS ONLINE";
		} else {
			RoomSelectionIndex = 0;
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomName.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapText.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMapImage.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMode.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomMasterClient.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomPlayerCount.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomButtonNext.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomButtonPrev.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().TotalRoomCount.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().CurrentRoomViewIndex.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().TotalPlayerCount.gameObject.SetActive (false);
			LobbyPanel.GetComponent<LobbyInput> ().Lobby_NoRoomsMessage.SetActive (true);
		}
	}
		
	bool gotRoomsUpdate = false;
	void OnReceivedRoomListUpdate__OLD(){
		roomList = PhotonNetwork.GetRoomList ();
		TotalRoomCount = roomList.Length;

		string[] tempStrArrayName = new string[TotalRoomCount];
		string[] tempStrArrayMode = new string[TotalRoomCount];
		string[] tempStrArrayMap = new string[TotalRoomCount];
		string[] tempStrArrayCurrentSize = new string[TotalRoomCount];
		string[] tempStrArrayMaxSize = new string[TotalRoomCount];

		int roomCount = 0;
		foreach (RoomInfo game in roomList) {

			tempStrArrayName [roomCount] = game.name.ToString ();
			tempStrArrayMode [roomCount] = game.customProperties [RoomProperties.GameMode].ToString ();
			tempStrArrayMap [roomCount] = game.customProperties [RoomProperties.Map].ToString ();
			tempStrArrayCurrentSize [roomCount] = game.PlayerCount.ToString ();
			tempStrArrayMaxSize [roomCount] = game.MaxPlayers.ToString ();
			totalPlayers += game.PlayerCount;
			roomCount += 1;
		}
		roomListManager.roomNameList = tempStrArrayName;
		roomListManager.roomModeList = tempStrArrayMode;
		roomListManager.roomMapList = tempStrArrayMap;
		roomListManager.roomCurrentSizeList = tempStrArrayCurrentSize;
		roomListManager.roomMaxSizeList = tempStrArrayMaxSize;

		Debug.Log ("Rooms online: " + TotalRoomCount);
		if (RoomSelectionIndex > TotalRoomCount) {
			RoomSelectionIndex = TotalRoomCount;
		}
		gotRoomsUpdate = true;
		//INITIALIZE ROOM SELECTION INDEX AND DISPLAY
		UpdateDisplayCurrentRoom ();
	}


	void DONOTUSE_OnReceivedRoomListUpdate()
	{
		//Recalculate playercount
		totalPlayers = 0;
		RoomInfoPos_V = -70f;
		RoomInfoPos_H = 350f;
		RoomInfoScale = 1f;

		GameObject[] RoomInfoPrefabList = GameObject.FindGameObjectsWithTag ("RoomInfoTag");
		foreach (GameObject roomInfoPrefab in RoomInfoPrefabList) {
			Destroy (roomInfoPrefab);
		}


		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		if (roomList.Length == 0 && !isCreateMenu) {
			LobbyPanel.GetComponent<LobbyInput> ().Lobby_NoRoomsMessage.GetComponent<Text>().text="NO ROOMS CURRENTLY RUNNING. YOU CAN CREATE A NEW ROOM.";
			Debug.Log("menu " + isCreateMenu + ", room len: " + roomList.Length);
		} else {
			LobbyPanel.GetComponent<LobbyInput> ().Lobby_NoRoomsMessage.GetComponent<Text> ().text = "";
		}
		foreach (RoomInfo game in roomList)
		{
			if (!isCreateMenu) {
				GameObject go = (GameObject)Instantiate (RoomInfoPrefab);
				Transform goTr = go.transform;
				goTr.SetParent (LobbyPanel.transform, false);
				Debug.Log ("Vert: " + RoomInfoPos_V);
				Debug.Log ("Horz: " + RoomInfoPos_H);
				goTr.localScale = new Vector3 (RoomInfoScale, RoomInfoScale, RoomInfoScale);
				goTr.GetComponent<RectTransform> ().anchorMin = new Vector2 (0.5f, 1f);
				goTr.GetComponent<RectTransform> ().anchorMax = new Vector2 (0.5f, 1f);
				goTr.GetComponent<RectTransform> ().pivot = new Vector2 (0.5f, 1f);

				goTr.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (RoomInfoPos_H, RoomInfoPos_V, 0);

				RoomInfoPos_V -= 25f;

				goTr.GetComponent<RoomInfoInput> ().RoomNamePrefab.GetComponent<Text> ().text = game.name.ToString ();
				goTr.GetComponent<RoomInfoInput> ().MapName.GetComponent<Text> ().text = game.customProperties [RoomProperties.Map].ToString ();
				goTr.GetComponent<RoomInfoInput> ().GameMode.GetComponent<Text> ().text = game.customProperties [RoomProperties.GameMode].ToString ();
				goTr.GetComponent<RoomInfoInput> ().MasterClient.GetComponent<Text> ().text = game.customProperties [RoomProperties.MasterClient].ToString ();
				goTr.GetComponent<RoomInfoInput> ().RoomCountPrefab.GetComponent<Text> ().text = game.playerCount.ToString () + " / " + game.maxPlayers.ToString ();
				goTr.GetComponent<RoomInfoInput> ().RoomRegion.GetComponent<Text> ().text = game.customProperties [RoomProperties.Region].ToString ();
			}
			totalPlayers += game.playerCount;
			LobbyPanel.GetComponent<LobbyInput> ().TotalPlayerCount.GetComponent<Text>().text = totalPlayers.ToString() + " ONLINE";
		}
	}



	void ConnectGUI()
	{
		GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
		GUILayout.Label("Connecting to the Photon server..");
		GUILayout.Label("See \"Photon Unity Networking/readme.txt\" for installation instructions.");
		if (photonConnectionFailed)
		{
			GUILayout.Label("Connection to Photon Failed.");
			GUILayout.Label("Possible reasons:");
			GUILayout.Label("-No internet connection");
			GUILayout.Label("-Wrong hostname");

		}
		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		if (overridePhotonConfigFile)
		{
			serverIP = GUILayout.TextField(serverIP, GUILayout.Width(120));
			serverPort = int.Parse("0" + GUILayout.TextField(serverPort.ToString(), GUILayout.Width(60)));
			if (GUILayout.Button("Retry", GUILayout.Width(75)))
			{
				PhotonNetwork.ConnectToMaster(serverIP, serverPort, defaultAppId, this.gameVersion);
				photonConnectionFailed = false;
			}
		}
		else
		{
			if (GUILayout.Button("Retry", GUILayout.Width(75)))
			{
				PhotonNetwork.ConnectUsingSettings(this.gameVersion);
				photonConnectionFailed = false;
			}
		}

		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	void OnFailedToConnectToPhoton()
	{
		photonConnectionFailed = true;
	}
}
