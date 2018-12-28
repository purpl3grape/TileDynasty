using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Steamworks;
using Photon;

public enum enumTeamStatus{
	teamsTied,
	BlueLeads, 
	RedLeads,
	CaptureTheFlagCommand,
	FlagReturned,
	BlueTeamCapturedFlag,
	RedTeamCapturedFlag,
};

public enum enumGameStatus{
	finalTeamsTied,
	finalBlueWins,
	finalRedWins,
	finalGameOver,
	GameStart
}

public enum enumTimeStatus{
	Minute5,
	Minute1, 
	Second10,
	Second9,
	Second8,
	Second7,
	Second6,
	Second5,
	Second4,
	Second3,
	Second2,
	Second1
};


public class GuiManager : PunBehaviour {

	public string RedTeamScoreProps = "redteamscoreprops";
	public string BlueTeamScoreProps = "blueteamscoreprops";

	WaitForSeconds waitFor0_01 = new WaitForSeconds (0.01f * .6f);
	WaitForSeconds waitFor0_05 = new WaitForSeconds (0.05f * .6f);
	WaitForSeconds waitFor0_1 = new WaitForSeconds (0.1f * .6f);
	WaitForSeconds waitFor0_5 = new WaitForSeconds (0.5f * .6f);
	WaitForSeconds waitFor1 = new WaitForSeconds (1f * 0.6f);
	WaitForSeconds waitFor3 = new WaitForSeconds (3f * 0.6f);
	WaitForSeconds waitFor5 = new WaitForSeconds (5f * 0.6f);
	WaitForSeconds waitFor600 = new WaitForSeconds (600f);


	[HideInInspector] public NetworkFlagBlue networkFlagBlue = null;
	[HideInInspector] public NetworkFlagRed networkFlagRed = null;
	private MatchTimer mTimer;
	private ScoringManager scoringManager;
	private NetworkManager nManager;
	private SettingsPanelInput settingsPanelInput;
	public WindowManager windowManager;
	public GameObject CampaignTrigger;
	KeyBindingManager kMgr;
	//GAMEMODE=0 (CAPTURE THE TILES) ITEMS
	private GameObject _TileConnectionPrefab;
	//GAMEMODE=1 (TDM) NO ITEMS
	//GAMEMODE=2 (ZOMBIE MODE) ITEMS
	private GameObject LevelUpPrefab;
	private GameObject LevelUpSpawnSpot_Container;
	//GAMEMODE=3 (CTF) 2 FLAG RESOURCE SPAWNS, 2 CAPTURE POINTS
	private GameObject TrashCanObject;
	private GameObject[] garbageObjects;

	private bool gameModeSyncd = false;

	[HideInInspector] public bool isMiniMapOn = false;

	//StandbyPanel
	public RenderTexture miniMapTexture;
	public GameObject MiniMapPanel;
	public GameObject MiniMapImage;
	public GameObject StandbyPanel;
	public GameObject InGamePanel;
	public GameObject SettingsPanel;
	public GameObject PlayerInGamePanel;
	public GameObject ChatPanel;
	public GameObject DisplayChatPanel;
	public GameObject MessagePanel;
	public MessagePanelInputs Msg_NPC;
	public MessagePanelInputs Msg_NPCAbandon;
	public MessagePanelInputs Msg_Obj;
	public MessagePanelInputs Msg_Ambush;
	public MessagePanelInputs Msg_Area;
	public MessagePanelInputs Msg_Item;
	public MessagePanelInputs Msg_Spawn;
	public MessagePanelInputs Msg_Death;
	public MessagePanelInputs Msg_Kill;

	public Color greenTimeColor;
	public Color yellowTimeColor;
	public Color redTimeColor;
	public Color flagCapturedColor = new Color (255f, 20f, 90f);
	public Color flagReturnedColor;
	public Color flagStolenColor;
	public Color blueTeamScoreColor = new Color(0,0,0,255);
	public Color redTeamScoreColor = new Color(0, 0,0,225);
	public Color tiedTeamScoreColor = new Color(0, 0,0,225);
	public Color ObjectiveCompleteColor;
	public Color ObjectiveIncompleteColor;
	public AudioClip[] Audio_TeamStatus;

	//CURRENT GAME MODE
	[HideInInspector] public int currentGameMode = -1;
	//TDM SCORING
	private int _blueTeamKills = 0;
	private int _redTeamKills = 0;
	public int redTeamKills{ get { return _redTeamKills; } set { _redTeamKills = value; } }
	public int blueTeamKills{ get { return _blueTeamKills; } set { _blueTeamKills = value; } }

	//TILE ITEMS SCORING
	private float bluePoints = 0f;
	private float redPoints = 0f;
	private float blueTiles;
	private float redTiles;
	//HOSTAGES SCORING
	public int currentHostagesRemaining = 4;
	public int HostagesRescued = 0;

	private int _currentEnemyKills = 0;
	public int currentEnemyKills{ get { return _currentEnemyKills; } set { this._currentEnemyKills = value; } }

	private bool enableGameStartIconONCE = false;
	private bool displayGameStartIcon = false;

	private bool _DisplayInGameGUI = false;
	public bool DisplayInGameGUI { get { return this._DisplayInGameGUI; } set { this._DisplayInGameGUI = value; } }

	private bool _GAMESTART = false;
	public bool GAMESTART { get { return this._GAMESTART; } set { this._GAMESTART = value; } }
	private bool _INITIALIZE_GAMESTART_FLAG = false;
	public bool INITIALIZE_GAMESTART_FLAG{ get { return this._INITIALIZE_GAMESTART_FLAG; } set { this._INITIALIZE_GAMESTART_FLAG = value; } }
	private double fullTime = 0;
	private double currTime = 0;

	private int _announceValue=-1;
	public int announceValue { get { return this._announceValue; } set { this._announceValue = value; } }
	private int _timeAnnounceValue=-1;
	public int timeAnnounceValue { get { return this._timeAnnounceValue; } set { this._timeAnnounceValue = value; } }
	private int _gameAnnounceValue=-1;
	public int gameAnnounceValue { get { return this._gameAnnounceValue; } set { this._gameAnnounceValue = value; } }


//	private bool _GAMEEND = false;
//	public bool GAMEEND { get { return this._GAMEEND; } set { this._GAMEEND = value; } }

	public bool GAMEEND = false;
	[PunRPC] public void SetGameEnd(bool val){
		GAMEEND = val;
	}
	//GAME MODE == 2 (SURVIVAL)
	[HideInInspector] public int currentCreditsRemaining = 5;
	[HideInInspector] public int currentWave = 0;
	[HideInInspector] public int currentWaveTarget = 100;
	[HideInInspector] public int nextCreditIncrease = 0; 
	[HideInInspector] public bool isBase1Captured = false;

	public float time;
	public int hourOfDay;
	public TimeSpan currentDayTime;
	public Transform sunTransform;
	public Light sunLight;
	public Text timeText;
	public Text timeTextStandby;
	public Text daysPassedText;
	public Text standbyDaysPassedText;

	public int days;

	public float intensity;
	public float playerLightIntensity;
	public Color fogDayHomeBase;
	public Color fogNightHomeBase;
	public Color fogDay;
	public Color fogNight;

	public Color MidnightColor;
	public Color DawnColor;
	public Color NoonColor;
	public Color DuskColor;

	public Color[] SunColors;

	public int speed;

	public ExitGames.Client.Photon.Hashtable TDMScoreTable;

	void Start () {
		GAMESTART = false;
		mTimer = GetComponent<MatchTimer> ();
		scoringManager = GetComponent<ScoringManager> ();
		nManager = GetComponent<NetworkManager> ();
		standbyPanelInputs = StandbyPanel.GetComponent<StandbyInput> ();
		settingsPanelInput = SettingsPanel.GetComponent<SettingsPanelInput> ();
		currentGameMode = -1;
		fullTime = (MainMenu.gameTimeDays * 60 / 5);

		_TileConnectionPrefab = GameObject.FindGameObjectWithTag ("ConnectionTileSetContainer");
		LevelUpPrefab = GameObject.FindGameObjectWithTag ("LevelUp");
		LevelUpSpawnSpot_Container = GameObject.FindGameObjectWithTag ("LevelUpSpawnSpotContainer");
		TrashCanObject = GameObject.FindGameObjectWithTag ("TrashCanContainer");
		garbageObjects = GameObject.FindGameObjectsWithTag ("Garbage_Item");
		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();

		if (GameObject.FindGameObjectWithTag ("Sun") != null) {
			sunLight = GameObject.FindGameObjectWithTag ("Sun").GetComponent<Light> ();
			sunTransform = sunLight.transform;
		}
		timeText = InGamePanel.GetComponent<InGamePanel> ().Value_DayTime;
		timeTextStandby = standbyPanelInputs.Value_StandbyDayTime;
		daysPassedText = InGamePanel.GetComponent<InGamePanel> ().Value_DaysPassed;
		standbyDaysPassedText = standbyPanelInputs.Value_StandDaysPassed;
		days = 1;


		StartCoroutine (PingCalculation_Coroutine ());
		StartCoroutine (GameStartMessage_Coroutine ());
		StartCoroutine (GameModeItemsActivation_Coroutine ());
		StartCoroutine (MainGUI_Coroutine ());
		StartCoroutine (CountDownTime_Coroutine ());

		TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { 
			{ RedTeamScoreProps, 0 },
			{ BlueTeamScoreProps, 0 }
		};

	}


	[PunRPC]
	public void syncGameModeForClients(int mode){
		currentGameMode = mode;
		gameModeSyncd = true;
	}


	[PunRPC]
	public void networkGameStart(bool val){
		GAMESTART = val;
	}
	//Called Here in FixedUpdate // Tiles Scoring
	[PunRPC]
	public void rpcUpdatePoints(int blueP, int redP){
		bluePoints = blueP;
		redPoints = redP;
	}

	//Obselete?
	[PunRPC]
	public void rpcUpdateTDMPoints(int blueP, int redP){
		_blueTeamKills = blueP;
		_redTeamKills = redP;
	}

	[PunRPC]
	public void rpcRedScoreIncrease(){
		_redTeamKills += 1;
	}

	[PunRPC]
	public void rpcBlueScoreIncrease(){
		_blueTeamKills += 1;
	}

	[PunRPC]
	public void rpcRedScoreDecrease(){
		_redTeamKills = _redTeamKills <= 0 ? 0 : _redTeamKills -= 1;
	}

	[PunRPC]
	public void rpcBlueScoreDecrease(){
		_blueTeamKills = _blueTeamKills <= 0 ? 0 : _blueTeamKills -= 1;
	}

	[PunRPC]
	public void DecreaseHostagesRemaining(){
		currentHostagesRemaining -= 1;
		HostagesRescued += 1;
	}

	[PunRPC]
	public void IncreaseHostagesRemaining(){
		currentHostagesRemaining += 1;
		HostagesRescued -= 1;
	}
		
	IEnumerator PingCalculation_Coroutine(){
		while (true) {
			if (currTime > 0f) {
				if (PhotonNetwork.offlineMode) {
					scoringManager.GetComponent<PhotonView> ().RPC ("SetScore", PhotonTargets.All, PhotonNetwork.player.NickName, scoringManager.pingStat, 0);
				} else {
					scoringManager.GetComponent<PhotonView> ().RPC ("SetScore", PhotonTargets.All, PhotonNetwork.player.NickName, scoringManager.pingStat, (int)PhotonNetwork.GetPing ());
				}
			}
			yield return waitFor5;
		}
	}
		
	IEnumerator GameStartMessage_Coroutine(){
		//GAMESTART_ICON NOTIFICATION
		while (true) {
			if (GAMESTART && enableGameStartIconONCE == false) {
				displayGameStartIcon = true;
				yield return waitFor3;
				enableGameStartIconONCE = true;
				displayGameStartIcon = false;
			}
			if (!GAMESTART) {
				yield return waitFor0_5;
			} else {
				yield return waitFor600;
			}
		}
	}

	public IEnumerator DeathMsgCO;
	public IEnumerator DeathMsg_CO(string deathMsg){
		if (!MessagePanel.GetActive() && !Msg_Obj.gameObject.GetActive () && !Msg_NPC.gameObject.GetActive ()) {

			Msg_NPC.gameObject.SetActive (false);
			Msg_NPCAbandon.gameObject.SetActive (false);
			Msg_Kill.gameObject.SetActive (false);
			Msg_Spawn.gameObject.SetActive (false);
			Msg_Item.gameObject.SetActive (false);
			Msg_Area.gameObject.SetActive (false);
			Msg_Ambush.gameObject.SetActive (false);

			if (Msg_Death.CloseMessagePanelCoroutine != null) {
				StopCoroutine (Msg_Death.CloseMessagePanelCoroutine);
			}
			if (Msg_Death.MessageBuilderCoroutine != null) {
				StopCoroutine (Msg_Death.MessageBuilderCoroutine);
			}
			if (Msg_Death.MessageBuilderCoroutine2 != null) {
				StopCoroutine (Msg_Death.MessageBuilderCoroutine2);
			}
			Msg_Death.DisplayMessagePanelCoroutine = Msg_Death.DisplayMessagePanel_Coroutine ();
			StartCoroutine (Msg_Death.DisplayMessagePanelCoroutine);

			Msg_Death.initMessage = deathMsg;
			Msg_Death.MessageBuilderCoroutine = Msg_Death.MessageBuilder_Coroutine (Msg_Death.initMessage, "#FA3232FF");
			StartCoroutine (Msg_Death.MessageBuilderCoroutine);
			while (!Msg_Death.isMessageOver) {
				yield return waitFor0_01;
			}
			yield return waitFor1;
			yield return waitFor1;
			Msg_Death.CloseMessagePanelCoroutine = Msg_Death.CloseMessagePanel_Coroutine ();
			StartCoroutine (Msg_Death.CloseMessagePanelCoroutine);
			yield return true;
		}
		DeathMsgCO = null;
	}

	public IEnumerator KillMsgCO;
	public IEnumerator KillMsg_CO(string killMsg){
		if (!MessagePanel.GetActive() && !Msg_Obj.gameObject.GetActive () && !Msg_NPC.gameObject.GetActive () && !Msg_Area.gameObject.GetActive() && !Msg_Ambush.gameObject.GetActive()) {
			
			if (Msg_Kill.CloseMessagePanelCoroutine != null) {
				StopCoroutine (Msg_Kill.CloseMessagePanelCoroutine);
			}
			if (Msg_Kill.MessageBuilderCoroutine != null) {
				StopCoroutine (Msg_Kill.MessageBuilderCoroutine);
			}
			if (Msg_Kill.MessageBuilderCoroutine2 != null) {
				StopCoroutine (Msg_Kill.MessageBuilderCoroutine2);
			}
			Msg_Kill.DisplayMessagePanelCoroutine = Msg_Kill.DisplayMessagePanel_Coroutine ();
			StartCoroutine (Msg_Kill.DisplayMessagePanelCoroutine);

			Msg_Kill.initMessage = killMsg;
			Msg_Kill.MessageBuilderCoroutine = Msg_Kill.MessageBuilder_Coroutine (Msg_Kill.initMessage, "#FA3232FF");
			StartCoroutine (Msg_Kill.MessageBuilderCoroutine);
			while (!Msg_Kill.isMessageOver) {
				yield return waitFor0_01;
			}
			yield return waitFor1;
			Msg_Kill.CloseMessagePanelCoroutine = Msg_Kill.CloseMessagePanel_Coroutine ();
			StartCoroutine (Msg_Kill.CloseMessagePanelCoroutine);
			yield return true;
		}
		KillMsgCO = null;
	}
		
	public IEnumerator ObjMsgCO;
	public IEnumerator ObjMsg_CO(string objMsg){

		if (!MessagePanel.GetActive ()) {
			Msg_NPC.gameObject.SetActive (false);
			Msg_NPCAbandon.gameObject.SetActive (false);
			Msg_Kill.gameObject.SetActive (false);
			Msg_Death.gameObject.SetActive (false);
			Msg_Area.gameObject.SetActive (false);
			Msg_Ambush.gameObject.SetActive (false);
			Msg_Item.gameObject.SetActive (false);
			Msg_Spawn.gameObject.SetActive (false);

			PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = false;
			if (Msg_Obj.CloseMessagePanelCoroutine != null) {
				StopCoroutine (Msg_Obj.CloseMessagePanelCoroutine);
			}
			if (Msg_Obj.MessageBuilderCoroutine != null) {
				StopCoroutine (Msg_Obj.MessageBuilderCoroutine);
			}
			if (Msg_Obj.MessageBuilderCoroutine2 != null) {
				StopCoroutine (Msg_Obj.MessageBuilderCoroutine2);
			}
			Msg_Obj.DisplayMessagePanelCoroutine = Msg_Obj.DisplayMessagePanel_Coroutine ();
			StartCoroutine (Msg_Obj.DisplayMessagePanelCoroutine);

			Msg_Obj.initMessage = objMsg;
			Msg_Obj.MessageBuilderCoroutine = Msg_Obj.MessageBuilder_Coroutine (Msg_Obj.initMessage, "#96FA32FF");
			StartCoroutine (Msg_Obj.MessageBuilderCoroutine);
			while (!Msg_Obj.isMessageOver) {
				yield return waitFor0_01;
			}
			yield return waitFor1;
			Msg_Obj.CloseMessagePanelCoroutine = Msg_Obj.CloseMessagePanel_Coroutine ();
			StartCoroutine (Msg_Obj.CloseMessagePanelCoroutine);
			PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = true;
			yield return true;
		}
		ObjMsgCO = null;
	}

	public IEnumerator NpcMsgAbandonCO;
	public IEnumerator NpcMsgAbandon_CO(string msg){

		if (!MessagePanel.GetActive ()) {
			Msg_NPC.gameObject.SetActive (false);
			Msg_Kill.gameObject.SetActive (false);
			Msg_Death.gameObject.SetActive (false);
			Msg_Area.gameObject.SetActive (false);
			Msg_Ambush.gameObject.SetActive (false);
			Msg_Item.gameObject.SetActive (false);
			Msg_Spawn.gameObject.SetActive (false);


			PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = false;
			if (Msg_NPCAbandon.CloseMessagePanelCoroutine != null) {
				StopCoroutine (Msg_NPCAbandon.CloseMessagePanelCoroutine);
			}
			if (Msg_NPCAbandon.MessageBuilderCoroutine != null) {
				StopCoroutine (Msg_NPCAbandon.MessageBuilderCoroutine);
			}
			if (Msg_NPCAbandon.MessageBuilderCoroutine2 != null) {
				StopCoroutine (Msg_NPCAbandon.MessageBuilderCoroutine2);
			}
			Msg_NPCAbandon.DisplayMessagePanelCoroutine = Msg_NPCAbandon.DisplayMessagePanel_Coroutine ();
			StartCoroutine (Msg_NPCAbandon.DisplayMessagePanelCoroutine);

			Msg_NPCAbandon.initMessage = msg;
			Msg_NPCAbandon.MessageBuilderCoroutine = Msg_NPCAbandon.MessageBuilder_Coroutine (Msg_NPCAbandon.initMessage, "#FA3232FF");
			StartCoroutine (Msg_NPCAbandon.MessageBuilderCoroutine);
			while (!Msg_NPCAbandon.isMessageOver) {
				yield return waitFor0_01;
			}
			yield return waitFor1;
			Msg_NPCAbandon.CloseMessagePanelCoroutine = Msg_NPCAbandon.CloseMessagePanel_Coroutine ();
			StartCoroutine (Msg_NPCAbandon.CloseMessagePanelCoroutine);
			PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = true;
			yield return true;
		}
		NpcMsgAbandonCO = null;
	}


	public IEnumerator NpcMsgCO;
	public IEnumerator NpcMsg_CO(int msgCount, string npcMsg, string npcMsg2){
		if (!MessagePanel.GetActive() && !Msg_Obj.gameObject.GetActive ()) {

			Msg_NPCAbandon.gameObject.SetActive (false);
			Msg_Kill.gameObject.SetActive (false);
			Msg_Death.gameObject.SetActive (false);
			Msg_Area.gameObject.SetActive (false);
			Msg_Ambush.gameObject.SetActive (false);
			Msg_Item.gameObject.SetActive (false);
			Msg_Spawn.gameObject.SetActive (false);

			PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = false;
			if (Msg_NPC.CloseMessagePanelCoroutine != null) {
				StopCoroutine (Msg_NPC.CloseMessagePanelCoroutine);
			}
			if (Msg_NPC.MessageBuilderCoroutine != null) {
				StopCoroutine (Msg_NPC.MessageBuilderCoroutine);
			}
			if (Msg_NPC.MessageBuilderCoroutine2 != null) {
				StopCoroutine (Msg_NPC.MessageBuilderCoroutine2);
			}
			Msg_NPC.DisplayMessagePanelCoroutine = Msg_NPC.DisplayMessagePanel_Coroutine ();
			StartCoroutine (Msg_NPC.DisplayMessagePanelCoroutine);
			if (msgCount == 1) {
				Msg_NPC.initMessage = npcMsg;
				Msg_NPC.MessageBuilderCoroutine = Msg_NPC.MessageBuilder_Coroutine (Msg_NPC.initMessage, "#FA3232FF");
				StartCoroutine (Msg_NPC.MessageBuilderCoroutine);
				while (!Msg_NPC.isMessageOver) {
					yield return waitFor0_01;
				}
			} else if (msgCount == 2) {
				Msg_NPC.initMessage = npcMsg;
				Msg_NPC.MessageBuilderCoroutine = Msg_NPC.MessageBuilder_Coroutine (Msg_NPC.initMessage, "#96FA32FF");
				StartCoroutine (Msg_NPC.MessageBuilderCoroutine);
				while (!Msg_NPC.isMessageOver) {
					yield return waitFor0_01;
				}
				yield return waitFor1;
				Msg_NPC.GetComponent<MessagePanelInputs> ().MessageObject.GetComponent<Text> ().text = string.Empty;
				Msg_NPC.initMessage = npcMsg2;
				Msg_NPC.MessageBuilderCoroutine = Msg_NPC.MessageBuilder_Coroutine (Msg_NPC.initMessage, "#FA3232FF");
				StartCoroutine (Msg_NPC.MessageBuilderCoroutine);
				while (!Msg_NPC.isMessageOver) {
					yield return waitFor0_01;
				}
			}
			yield return waitFor1;
			Msg_NPC.CloseMessagePanelCoroutine = Msg_NPC.CloseMessagePanel_Coroutine ();
			StartCoroutine (Msg_NPC.CloseMessagePanelCoroutine);
			PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = true;
			yield return true;
		}
		NpcMsgCO = null;
	}

	public IEnumerator AmbushMessageCoroutine;
	public IEnumerator AmbushMessage_coroutine(string areaMessage){

		if (Msg_Ambush.CloseMessagePanelCoroutine != null) {
			StopCoroutine (Msg_Ambush.CloseMessagePanelCoroutine);
		}
		if (Msg_Ambush.MessageBuilderCoroutine != null) {
			StopCoroutine (Msg_Ambush.MessageBuilderCoroutine);
		}
		if (Msg_Ambush.MessageBuilderCoroutine2 != null) {
			StopCoroutine (Msg_Ambush.MessageBuilderCoroutine2);
		}
		Msg_Ambush.DisplayMessagePanelCoroutine = Msg_Ambush.DisplayMessagePanel_Coroutine ();
		StartCoroutine (Msg_Ambush.DisplayMessagePanelCoroutine);

		Msg_Ambush.initMessage = areaMessage;
		Msg_Ambush.MessageBuilderCoroutine = Msg_Ambush.MessageBuilder_Coroutine (Msg_Ambush.initMessage, "#FA3232FF");
		StartCoroutine (Msg_Ambush.MessageBuilderCoroutine);
		while (!Msg_Ambush.isMessageOver) {
			yield return waitFor0_01;
		}
		yield return waitFor1;
		Msg_Ambush.CloseMessagePanelCoroutine = Msg_Ambush.CloseMessagePanel_Coroutine ();
		StartCoroutine (Msg_Ambush.CloseMessagePanelCoroutine);
		yield return true;
		AmbushMessageCoroutine = null;
	}

	public IEnumerator AreaMessageCoroutine;
	public IEnumerator AreaMessage_coroutine(string areaMessage){
		if (!MessagePanel.GetActive() && !Msg_Obj.gameObject.GetActive () && !Msg_NPC.gameObject.GetActive () && !Msg_Death.gameObject.GetActive()) {

			Msg_Kill.gameObject.SetActive (false);
			Msg_Death.gameObject.SetActive (false);
			Msg_Item.gameObject.SetActive (false);

			if (Msg_Area.CloseMessagePanelCoroutine != null) {
				StopCoroutine (Msg_Area.CloseMessagePanelCoroutine);
			}
			if (Msg_Area.MessageBuilderCoroutine != null) {
				StopCoroutine (Msg_Area.MessageBuilderCoroutine);
			}
			if (Msg_Area.MessageBuilderCoroutine2 != null) {
				StopCoroutine (Msg_Area.MessageBuilderCoroutine2);
			}
			Msg_Area.DisplayMessagePanelCoroutine = Msg_Area.DisplayMessagePanel_Coroutine ();
			StartCoroutine (Msg_Area.DisplayMessagePanelCoroutine);

			Msg_Area.initMessage = areaMessage;
			Msg_Area.MessageBuilderCoroutine = Msg_Area.MessageBuilder_Coroutine (Msg_Area.initMessage, "#FA3232FF");
			StartCoroutine (Msg_Area.MessageBuilderCoroutine);
			while (!Msg_Area.isMessageOver) {
				yield return waitFor0_01;
			}
			yield return waitFor1;
			Msg_Area.CloseMessagePanelCoroutine = Msg_Area.CloseMessagePanel_Coroutine ();
			StartCoroutine (Msg_Area.CloseMessagePanelCoroutine);
			yield return true;
		}
		AreaMessageCoroutine = null;
	}

	public IEnumerator ItemPickupMessageCoroutine;
	public IEnumerator ItemPickupMessage_Coroutine(string message){
		if (!MessagePanel.GetActive() && !Msg_Obj.gameObject.GetActive () && !Msg_NPC.gameObject.GetActive () && !Msg_Area.gameObject.GetActive () && !Msg_Ambush.gameObject.GetActive () && !Msg_Death.gameObject.GetActive()) {
			
			if (Msg_Item.CloseMessagePanelCoroutine != null) {
				StopCoroutine (Msg_Item.CloseMessagePanelCoroutine);
			}
			if (Msg_Item.MessageBuilderCoroutine != null) {
				StopCoroutine (Msg_Item.MessageBuilderCoroutine);
			}
			if (Msg_Item.MessageBuilderCoroutine2 != null) {
				StopCoroutine (Msg_Item.MessageBuilderCoroutine2);
			}

			Msg_Item.DisplayMessagePanelCoroutine = Msg_Item.DisplayMessagePanel_Coroutine ();
			StartCoroutine (Msg_Item.DisplayMessagePanelCoroutine);

			
			Msg_Item.initMessage = message;
			Msg_Item.MessageBuilderCoroutine = Msg_Item.MessageBuilder_Coroutine (Msg_Item.initMessage, "#9632FAFF");
			StartCoroutine (Msg_Item.MessageBuilderCoroutine);
			while (!Msg_Item.isMessageOver) {
				yield return waitFor0_01;
			}
			yield return waitFor1;

			Msg_Item.CloseMessagePanelCoroutine = Msg_Item.CloseMessagePanel_Coroutine ();
			StartCoroutine (Msg_Item.CloseMessagePanelCoroutine);
			yield return true;
		}
		ItemPickupMessageCoroutine = null;
	}


	/// <summary>
	/// MESSAGE PANEL INPUTS [MAIN GAME MESSAGE PANEL]
	/// </summary>
	public bool isMessageOver = false;
	public bool isDisplayMessagePanel = false;
	public Vector3 initialMessagePanelDimensions = new Vector3 (1, 0, 1);
	public Vector3 finalMessagePanelDimensions = new Vector3 (1, 1, 1);
	public IEnumerator DisplayMessagePanelCoroutine;
	public IEnumerator DisplayMessagePanel_Coroutine(){

		if (CloseMessagePanelCoroutine != null) {
			StopCoroutine (CloseMessagePanelCoroutine);
		}

		PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = false;

		float progress = 0f; //This float will serve as the 3rd parameter of the lerp function.
		float smoothness = 0.02f;
		float duration = .1f;
		float increment = 0.2f;
		increment =	smoothness / duration; //The amount of change to apply.
		MessagePanel.transform.localScale = new Vector3 (1, 0, 1);
		MessagePanel.GetComponent<MessagePanelInputs> ().MessageObject.GetComponent<Text> ().text = string.Empty;
		isMessageOver = false;

		MessagePanel.SetActive (true);
		while (progress < 1 && !isDisplayMessagePanel) {
			MessagePanel.transform.localScale = Vector3.Lerp (initialMessagePanelDimensions, finalMessagePanelDimensions, progress);
			progress += increment;
//			Debug.Log ("Progress test: " + progress);
			yield return new WaitForSeconds (smoothness);
			if (progress >= 1)
				isDisplayMessagePanel = true;
		}
		MessagePanel.transform.localScale = finalMessagePanelDimensions;

		yield return waitFor0_1;
		DisplayMessagePanelCoroutine = null;

	}
	public IEnumerator CloseMessagePanelCoroutine;
	public IEnumerator CloseMessagePanel_Coroutine(){

		float progress = 0f; //This float will serve as the 3rd parameter of the lerp function.
		float smoothness = 0.02f;
		float duration = .1f;
		float increment = 0.2f;
		increment =	smoothness / duration; //The amount of change to apply.

		while (progress < 1 && isDisplayMessagePanel) {
			MessagePanel.transform.localScale = Vector3.Lerp (finalMessagePanelDimensions, initialMessagePanelDimensions, progress);
			progress += increment;
//			Debug.Log ("Progress test: " + progress);
			yield return new WaitForSeconds (smoothness);
			if (progress >= 1)
				isDisplayMessagePanel = false;
		}
		MessagePanel.transform.localScale = initialMessagePanelDimensions;
		MessagePanel.GetComponent<MessagePanelInputs> ().MessageObject.GetComponent<Text> ().text = string.Empty;
		MessagePanel.gameObject.SetActive (false);
		isMessageOver = true;

		PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = true;

		yield return true;
		CloseMessagePanelCoroutine = null;
	}

	//APPEND ON EACH MESSAGE
	public string initMessage = string.Empty;
	public IEnumerator MessageBuilderCoroutine;
	public IEnumerator MessageBuilder_Coroutine(string msg, string HexColor){

		if (CloseMessagePanelCoroutine != null) {
			StopCoroutine (CloseMessagePanelCoroutine);
		}

		PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = false;

		string tempstr = string.Empty;
		isMessageOver = false;

		foreach (char c in msg) {
			if (HexColor == string.Empty) {				
				MessagePanel.GetComponent<MessagePanelInputs> ().MessageObject.GetComponent<Text> ().text += c;
			} else {
				tempstr = "<color=" + HexColor + ">" + c + "</color>";
				MessagePanel.GetComponent<MessagePanelInputs> ().MessageObject.GetComponent<Text> ().text += tempstr;
			}
			if (!c.Equals (" ")) {
				yield return waitFor0_01;
			}
		}

		if (DisplayMessagePanelCoroutine != null) {
			StopCoroutine (DisplayMessagePanelCoroutine);
		}
		isMessageOver = true;
		MessageBuilderCoroutine = null;
	}

	//OVERWRITE EACH MESSAGE
	public IEnumerator MessageBuilderCoroutine2;
	public IEnumerator MessageBuilder_Coroutine2(string c, string HexColor){

		if (CloseMessagePanelCoroutine != null) {
			StopCoroutine (CloseMessagePanelCoroutine);
		}

		PlayerInGamePanel.GetComponent<PlayerInGamePanel> ().Image_CrossHair.enabled = false;

		string tempstr = string.Empty;
		isMessageOver = false;

		if (HexColor == string.Empty) {			
			MessagePanel.GetComponent<MessagePanelInputs> ().MessageObject.GetComponent<Text> ().text = c;
		} else {
			tempstr = "<color=" + HexColor + ">" + c + "</color>";
			MessagePanel.GetComponent<MessagePanelInputs> ().MessageObject.GetComponent<Text> ().text = tempstr;
		}
		if (!c.Equals (" ")) {
			yield return waitFor0_01;
		}

		if (DisplayMessagePanelCoroutine != null) {
			StopCoroutine (DisplayMessagePanelCoroutine);
		}
		isMessageOver = true;
		MessageBuilderCoroutine2 = null;
	}


	bool gameModeItemsSet = false;
	IEnumerator GameModeItemsActivation_Coroutine(){
		//SYNC PLAYERS TO CURRENT GAME MODE MASTER CLIENT IS ON
		//DO SO UNTIL GAME STARTS BECAUSE, NO OTHER PLAYERS CAN JOIN WHEN GAME STARTS
		while (!gameModeItemsSet) {
			if (!mTimer.isReady) {
				if (PhotonNetwork.isMasterClient) {
					if (!gameModeSyncd) {
						//INITIAL GAME MODE SYNC
						GetComponent<PhotonView> ().RPC ("syncGameModeForClients", PhotonTargets.AllBuffered, MainMenu.gameMode);
					}
				}
				//TDM
				if (currentGameMode == 1) {
					if (LevelUpPrefab) {
						LevelUpPrefab.SetActive (false);
					}
					if (LevelUpSpawnSpot_Container) {
						LevelUpSpawnSpot_Container.SetActive (false);
					}
					if (_TileConnectionPrefab) {
						_TileConnectionPrefab.SetActive (false);
					}
					if (TrashCanObject) {
						TrashCanObject.SetActive (false);
					}
					foreach (GameObject garbageItem in garbageObjects) {
						garbageItem.SetActive (false);
					}

				} else if (currentGameMode == 2) {
//					LevelUpPrefab.SetActive (true);
					if (LevelUpSpawnSpot_Container) {
						LevelUpSpawnSpot_Container.SetActive (false);
					}
					if (_TileConnectionPrefab) {
						_TileConnectionPrefab.SetActive (false);
					}
					if (TrashCanObject) {
						TrashCanObject.SetActive (false);
					}
					foreach (GameObject garbageItem in garbageObjects) {
						garbageItem.SetActive (false);
					}

				}
			}
			yield return waitFor0_1;
		}
	}

	PlayerGuiManager playerGUIManager;

	bool initMinimap = false;
	IEnumerator MiniMapActivateCO;
	IEnumerator MiniMapActivate_CO(){
		while (true) {
			if (nManager.PlayerObject != null) {

				if (playerGUIManager == null)
					playerGUIManager = nManager.PlayerObject.GetComponent<PlayerGuiManager> ();

					if (PlayerPrefs.GetInt ("DisplayMiniMap") == 1) {
						playerGUIManager.miniMapCamera.targetTexture = miniMapTexture;
						MiniMapImage.GetComponent<RawImage> ().texture = miniMapTexture;
						MiniMapPanel.SetActive (true);
						playerGUIManager.miniMapCamera.enabled = true;
					} else {
						MiniMapPanel.SetActive (false);
						playerGUIManager.miniMapCamera.enabled = false;
					}
							

				if (MiniMapPanel.GetActive ()) {

					if (Input.GetKey (KeyCode.Z)) {						
						PlayerPrefs.SetInt ("DisplayMiniMap", 0);
						PlayerPrefs.Save ();

						MiniMapPanel.SetActive (false);
						playerGUIManager.miniMapCamera.enabled = false;
						yield return waitFor0_5;
					}
				} else {
					if (Input.GetKey (KeyCode.Z)) {

						PlayerPrefs.SetInt ("DisplayMiniMap", 1);
						PlayerPrefs.Save ();

						playerGUIManager.miniMapCamera.targetTexture = miniMapTexture;
						MiniMapImage.GetComponent<RawImage> ().texture = miniMapTexture;
						MiniMapPanel.SetActive (true);
						playerGUIManager.miniMapCamera.enabled = true;
						yield return waitFor0_5;
					}

				}
			} else {
				initMinimap = false;
			}
			yield return true;
		}
	}

	Light playerLight;
	void FixedUpdate () {

		if (MiniMapActivateCO == null) {
			MiniMapActivateCO = MiniMapActivate_CO ();
			StartCoroutine (MiniMapActivateCO);
		}


		if (nManager.PlayerObject != null) {

			if (!isMiniMapOn) {
				if (windowManager.scoreBoard.GetActive () == false) {
					isMiniMapOn = true;
//					MiniMapPanel.SetActive (true);
				}
			} else if (isMiniMapOn) {
				if (windowManager.scoreBoard.GetActive ()) {
					isMiniMapOn = false;
					MiniMapPanel.SetActive (false);
				}	
			}
		} else {			
			if (isMiniMapOn) {
				isMiniMapOn = false;
				MiniMapPanel.SetActive (false);
			}
		}

		if (currentGameMode == 2) {
			if (!(currentHostagesRemaining <= 0) && !GAMEEND) {
				currTime = mTimer.SecondsUntilItsTime;
			} else {
				if (!GAMEEND) {
					if (PhotonNetwork.isMasterClient) {
						GetComponent<PhotonView> ().RPC ("SetGameEnd", PhotonTargets.AllBuffered, true);
					}
				}
				currTime = 0;
			}
		} else {
			currTime = mTimer.SecondsUntilItsTime;
			if (currTime <= 0) {
				if (!GAMEEND) {
					if (PhotonNetwork.isMasterClient) {
						GetComponent<PhotonView> ().RPC ("SetGameEnd", PhotonTargets.AllBuffered, true);
					}
				}
			}
		}


		//For Home Base Day (every 5 min approx)
		if (currentGameMode == 99) {
			ChangeTime (currentGameMode);
		} else {

			if (!mTimer.isReady) {
				sunLight.GetComponent<Light> ().color = SunColors [1];
				sunTransform.rotation = Quaternion.Euler (270, 0, 0);
			} else {
				ChangeTime (currentGameMode);
			}
		}


		if (playerHasRespawned) {
			playerHasRespawned = false;
			SpawnMessageCoroutine = SpawnMessage_Coroutine ();
			StartCoroutine (SpawnMessageCoroutine);
		}

	}
		

	public bool playerHasRespawned = false;
	public IEnumerator SpawnMessageCoroutine;
	public IEnumerator SpawnMessage_Coroutine(){
		if (!MessagePanel.GetActive() && !Msg_Obj.gameObject.GetActive () && !Msg_NPC.gameObject.GetActive ()) {
			
			//PRIORITY PANEL DISPLAY (DISABLE LOWER PRIORITY)
			Msg_Kill.gameObject.SetActive (false);
			Msg_Death.gameObject.SetActive (false);
			Msg_Area.gameObject.SetActive (false);
			Msg_Ambush.gameObject.SetActive (false);
			Msg_Item.gameObject.SetActive (false);

			if (Msg_Spawn.CloseMessagePanelCoroutine != null) {
				StopCoroutine (Msg_Spawn.CloseMessagePanelCoroutine);
			}
			if (Msg_Spawn.MessageBuilderCoroutine != null) {
				StopCoroutine (Msg_Spawn.MessageBuilderCoroutine);
			}
			if (Msg_Spawn.MessageBuilderCoroutine2 != null) {
				StopCoroutine (Msg_Spawn.MessageBuilderCoroutine2);
			}
			Msg_Spawn.DisplayMessagePanelCoroutine = Msg_Spawn.DisplayMessagePanel_Coroutine ();
			StartCoroutine (Msg_Spawn.DisplayMessagePanelCoroutine);

			if (currentGameMode == 99) {
				Msg_Spawn.initMessage = SteamFriends.GetPersonaName ().ToUpper () + "'S BASE";
				Msg_Spawn.MessageBuilderCoroutine = Msg_Spawn.MessageBuilder_Coroutine (Msg_Spawn.initMessage, "#9632FAFF");
			} else if (currentGameMode == 1) {
				if (mTimer.isReady) {
					if (redTeamKills > blueTeamKills) {
						Msg_Spawn.initMessage = msgRedLeads;
						Msg_Spawn.MessageBuilderCoroutine = Msg_Spawn.MessageBuilder_Coroutine (Msg_Spawn.initMessage, "#FA3232FF");
					} else if (redTeamKills < blueTeamKills) {
						Msg_Spawn.initMessage = msgBlueLeads;
						Msg_Spawn.MessageBuilderCoroutine = Msg_Spawn.MessageBuilder_Coroutine (Msg_Spawn.initMessage, "#6496FAFF");
					} else {
						Msg_Spawn.initMessage = msgTied;
						Msg_Spawn.MessageBuilderCoroutine = Msg_Spawn.MessageBuilder_Coroutine (Msg_Spawn.initMessage, "#96FA32FF");
					}
				} else {
					Msg_Spawn.initMessage = strReadyUp;
					Msg_Spawn.MessageBuilderCoroutine = Msg_Spawn.MessageBuilder_Coroutine (Msg_Spawn.initMessage, "#96FA32FF");
				}
			} else if (currentGameMode == 2) {
				if (mTimer.isReady) {
					Msg_Spawn.initMessage = msgSearchAndRescue;
					Msg_Spawn.MessageBuilderCoroutine = Msg_Spawn.MessageBuilder_Coroutine (Msg_Spawn.initMessage, "#96FA32FF");
				} else {
					Msg_Spawn.initMessage = strReadyUp;
					Msg_Spawn.MessageBuilderCoroutine = Msg_Spawn.MessageBuilder_Coroutine (Msg_Spawn.initMessage, "#96FA32FF");
				}
			}

			StartCoroutine (Msg_Spawn.MessageBuilderCoroutine);
			while (!Msg_Spawn.isMessageOver) {
				yield return waitFor0_01;
			}
			yield return waitFor1;

			Msg_Spawn.CloseMessagePanelCoroutine = Msg_Spawn.CloseMessagePanel_Coroutine ();
			StartCoroutine (Msg_Spawn.CloseMessagePanelCoroutine);

			yield return true;
		}
		SpawnMessageCoroutine = null;
	}



	StandbyInput standbyPanelInputs;
	IEnumerator StandbyGUI_Coroutine(){
		while (!DisplayInGameGUI) {
			//Enable StandbyPanel and Disable InGamePanel
			if (!GAMEEND) {
				StandbyPanel.SetActive (true);
			} else {
				StandbyPanel.SetActive (false);
			}
			InGamePanel.SetActive (false);
			DisplayChatPanel.SetActive (false);
			PlayerInGamePanel.SetActive (false);


			if (currentGameMode == 1) {
				//StandbyPanel Stuff
				StartCoroutine(DisableUnusedStandbyGUI_Coroutine(1));

				standbyPanelInputs.TDMStandbyContainer.GetComponent<ContainerStandbyTDMInput> ().TDMStandbyRedPoints.text = _redTeamKills.ToString ();
				standbyPanelInputs.TDMStandbyContainer.GetComponent<ContainerStandbyTDMInput> ().TDMStandbyBluePoints.text = _blueTeamKills.ToString ();
			}
			else if (currentGameMode == 2) {

				//StandbyPanel Stuff
				StartCoroutine(DisableUnusedStandbyGUI_Coroutine(2));
				standbyPanelInputs.ZombieStandbyContainer.GetComponent<ContainerStandbyZombieInput> ().Value_StandbyLives.text = currentHostagesRemaining.ToString();
			}
			//HOME SCENE
			if (currentGameMode == 99) {
				standbyPanelInputs.TDMStandbyContainer.SetActive (false);
				standbyPanelInputs.TileStandbyContainer.SetActive (false);
				standbyPanelInputs.ZombieStandbyContainer.SetActive (false);
				standbyPanelInputs.CTFStandbyContainer.SetActive (false);
				standbyPanelInputs.Value_StandbyTime.gameObject.SetActive (false);
				standbyPanelInputs.Value_StandbyTimeLabel.gameObject.SetActive (false);
				standbyPanelInputs.Value_StandDaysPassed.gameObject.SetActive (false);
			}

			//STANDBY TIME GUI
			if (((currTime / fullTime) > 0.5f)) {
				standbyPanelInputs.Value_StandbyTime.color = greenTimeColor;
			} else if (((currTime / fullTime) <= 0.5f) && ((currTime / fullTime) > 0.1f) && currTime > 10f) {
				standbyPanelInputs.Value_StandbyTime.color = yellowTimeColor;
			} else if (((currTime / fullTime) <= 0.1f) || currTime <= 10f) { 
				standbyPanelInputs.Value_StandbyTime.color = redTimeColor;
			}

			if (!GAMEEND) {
				standbyPanelInputs.Value_StandbyTime.text = getMinutes (currTime) + strColon + getSeconds (currTime);
			} else {
				standbyPanelInputs.Value_StandbyTime.text = str00 + strColon + str00;
			}

			yield return waitFor0_1;
		}
	}

	bool isUnusedStandbyGUI=true;
	IEnumerator DisableUnusedStandbyGUI_Coroutine(int gamemode){
		while (isUnusedStandbyGUI) {
	 		if (gamemode == 1) {
				standbyPanelInputs.TileStandbyContainer.SetActive (false);
				standbyPanelInputs.TDMStandbyContainer.SetActive (true);
				standbyPanelInputs.ZombieStandbyContainer.SetActive (false);
				standbyPanelInputs.CTFStandbyContainer.SetActive (false);
			} else if (gamemode == 2) {
				standbyPanelInputs.TileStandbyContainer.SetActive (false);
				standbyPanelInputs.TDMStandbyContainer.SetActive (false);
				standbyPanelInputs.ZombieStandbyContainer.SetActive (true);
				standbyPanelInputs.CTFStandbyContainer.SetActive (false);
			}

			isUnusedStandbyGUI = false;
			yield return waitFor0_05;
		}
	}

	bool isUnusedMainGUI=true;
	IEnumerator DisableUnusedMainGUI_Coroutine(int gamemode){
		InGamePanel inGamePanelInputs = InGamePanel.GetComponent<InGamePanel> ();
		while (isUnusedMainGUI) {
			if (gamemode == 1) {
				inGamePanelInputs.Container_Tiles.SetActive (false);
				inGamePanelInputs.Container_TDM.SetActive (true);
				inGamePanelInputs.Container_Zombies.SetActive (false);
				inGamePanelInputs.Container_CTF.SetActive (false);
			} else if (gamemode == 2) {
				inGamePanelInputs.Container_Tiles.SetActive (false);
				inGamePanelInputs.Container_TDM.SetActive (false);
				inGamePanelInputs.Container_Zombies.SetActive (true);
				inGamePanelInputs.Container_CTF.SetActive (false);			
			}

			isUnusedMainGUI = false;
			yield return waitFor0_05;
		}
	}

	IEnumerator CountDownTime_Coroutine(){
		while (true) {

			if (mTimer.isReady) {
				if ((int)currTime == 300 && (int)fullTime != 300) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Minute5 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [10]);
						_timeAnnounceValue = (int)enumTimeStatus.Minute5;
					}
				}
				else if ((int)currTime == 60 && (int)fullTime != 60) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Minute1 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [11]);
						_timeAnnounceValue = (int)enumTimeStatus.Minute1;
					}
				}
				else if ((int)currTime == 10) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second10 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [12]);
						_timeAnnounceValue = (int)enumTimeStatus.Second10;
					}
				}
				else if ((int)currTime == 9) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second9 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [13]);
						_timeAnnounceValue = (int)enumTimeStatus.Second9;
					}
				}
				else if ((int)currTime == 8) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second8 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [14]);
						_timeAnnounceValue = (int)enumTimeStatus.Second8;
					}
				}
				else if ((int)currTime == 7) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second7 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [15]);
						_timeAnnounceValue = (int)enumTimeStatus.Second7;
					}
				}
				else if ((int)currTime == 6) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second6 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [16]);
						_timeAnnounceValue = (int)enumTimeStatus.Second6;
					}
				}
				else if ((int)currTime == 5) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second5 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [17]);
						_timeAnnounceValue = (int)enumTimeStatus.Second5;
					}
				}
				else if ((int)currTime == 4) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second4 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [18]);
						_timeAnnounceValue = (int)enumTimeStatus.Second4;
					}
				}
				else if ((int)currTime == 3) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second3 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [19]);
						_timeAnnounceValue = (int)enumTimeStatus.Second3;
					}
				}
				else if ((int)currTime == 2) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second2 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [20]);
						_timeAnnounceValue = (int)enumTimeStatus.Second2;
					}
				}
				else if ((int)currTime == 1) {
					if (_timeAnnounceValue != (int)enumTimeStatus.Second1 && mTimer.isReady) {
						scoringManager.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [21]);
						_timeAnnounceValue = (int)enumTimeStatus.Second1;
					}
				}
			}

			yield return waitFor0_1;
		}
	}


	IEnumerator MainGUI_Coroutine(){
		while (true) {
			if (DisplayInGameGUI == false) {
				if (nManager.hasPickedTeam == true) {
					StartCoroutine (StandbyGUI_Coroutine ());
				}
			} else {
				StandbyPanel.SetActive (false);
				if (nManager.hasPickedTeam) {
					InGamePanel.SetActive (true);
					if (nManager.DisplayChatEnabled) {
						DisplayChatPanel.SetActive (true);
					} else {
						DisplayChatPanel.SetActive (false);
					}
					PlayerInGamePanel.SetActive (true);
				}

				InGamePanel inGamePanelInputs = InGamePanel.GetComponent<InGamePanel> ();
				if (!mTimer.isReady) {
					inGamePanelInputs.Label_ReadyUpMessage1.gameObject.SetActive (true);
					inGamePanelInputs.Label_ReadyUpMessage2.gameObject.SetActive (true);

					inGamePanelInputs.Label_ReadyUpMessage1.text = strReadyUp;
					inGamePanelInputs.Label_ReadyUpMessage2.text = string.Empty;
				} else {
					inGamePanelInputs.Label_ReadyUpMessage1.gameObject.SetActive (false);
					inGamePanelInputs.Label_ReadyUpMessage2.gameObject.SetActive (false);
				}


				//TDM Mode
				if (currentGameMode == 1) {
					int bteamkills = _blueTeamKills;
					int rteamkills = _redTeamKills;

					StartCoroutine (DisableUnusedMainGUI_Coroutine (1));

					ContainerTDMInput tdmGUIInputs = inGamePanelInputs.Container_TDM.GetComponent<ContainerTDMInput> ();
					tdmGUIInputs.Value_BlueTDMPoints.color = blueTeamScoreColor;
					tdmGUIInputs.Value_RedTDMPoints.color = redTeamScoreColor;
					tdmGUIInputs.Value_BlueTDMPoints.text = bteamkills.ToString ();
					tdmGUIInputs.Value_RedTDMPoints.text = rteamkills.ToString ();


					if (bteamkills == rteamkills && bteamkills > 2 && rteamkills > 2) {
						if (_announceValue != (int)enumTeamStatus.teamsTied) {
							inGamePanelInputs.Announcer_TeamLead.GetComponent<Text> ().text = strTied;
							inGamePanelInputs.Announcer_TeamLead.GetComponent<Text> ().color = tiedTeamScoreColor;
							inGamePanelInputs.Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [0]);
							_announceValue = (int)enumTeamStatus.teamsTied;
						}
					}
					if (bteamkills > rteamkills && _blueTeamKills > 2) {
						if (_announceValue != (int)enumTeamStatus.BlueLeads) {
							inGamePanelInputs.Announcer_TeamLead.GetComponent<Text> ().text = strBlueLeads;
							inGamePanelInputs.Announcer_TeamLead.GetComponent<Text> ().color = blueTeamScoreColor;
							inGamePanelInputs.Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [1]);
							_announceValue = (int)enumTeamStatus.BlueLeads;
						}
					}
					if (bteamkills < rteamkills && _redTeamKills > 2) {
						if (_announceValue != (int)enumTeamStatus.RedLeads) {
							inGamePanelInputs.Announcer_TeamLead.GetComponent<Text> ().text = strRedLeads;
							inGamePanelInputs.Announcer_TeamLead.GetComponent<Text> ().color = redTeamScoreColor;
							inGamePanelInputs.Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (Audio_TeamStatus [2]);
							_announceValue = (int)enumTeamStatus.RedLeads;
						}
					}

					settingsPanelInput.ObjectiveRescueSurvivors.GetComponent<Text> ().text = strTDM;
					settingsPanelInput.ObjectiveRescueSurvivors_Status1.GetComponent<Text> ().text = string.Empty;
					if (bteamkills < rteamkills) {
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().text = strRedLeads;
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().color = redTeamScoreColor;
					} else if (bteamkills > rteamkills) {
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().text = strBlueLeads;
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().color = blueTeamScoreColor;
					} else {
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().text = strTied;
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().color = ObjectiveCompleteColor;
					}


				} 
				//Survival Mode
				else if (currentGameMode == 2) {

					StartCoroutine (DisableUnusedMainGUI_Coroutine (2));

					settingsPanelInput.ObjectiveEliminateEnemy_Status1.GetComponent<Text> ().text = " TROOPS REMAINING";
					settingsPanelInput.ObjectiveEliminateEnemy_Status2.GetComponent<Text> ().text = "OBJECTIVE COMPLETE";
					settingsPanelInput.ObjectiveEliminateEnemy_Status2.GetComponent<Text> ().color = ObjectiveCompleteColor;

					settingsPanelInput.ObjectiveRescueSurvivors_Status1.GetComponent<Text> ().text = currentHostagesRemaining.ToString () + " SURVIVORS REMAINING";
					if (currentHostagesRemaining <= 0) {
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().text = "OBJECTIVE COMPLETE";
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().color = ObjectiveCompleteColor;
					} else {
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().text = "OBJECTIVE INCOMPLETE";
						settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().color = ObjectiveIncompleteColor;
					}


					settingsPanelInput.ObjectiveCaptureEnemyBases_Status2.GetComponent<Text> ().text = "ZOMBIES";
					settingsPanelInput.ObjectiveCaptureEnemyBases_Status2.GetComponent<Text> ().text = "OBJECTIVE COMPLETE";
					settingsPanelInput.ObjectiveCaptureEnemyBases_Status2.GetComponent<Text> ().color = ObjectiveCompleteColor;


					ContainerZombieInput survivalGUIInputs = inGamePanelInputs.Container_Zombies.GetComponent<ContainerZombieInput> ();
					survivalGUIInputs.Value_CurrentHostagesRemaining.text = currentHostagesRemaining.ToString();
				}

				//HOME SCENE
				if (currentGameMode == 99) {
					settingsPanelInput.MissionLabel.GetComponent<Text> ().text = "TILEDYNASTY ARENA";
					settingsPanelInput.ObjectiveRescueSurvivors.GetComponent<Text> ().text = PhotonNetwork.playerName.ToUpper () + "'S BASE";
					settingsPanelInput.ObjectiveRescueSurvivors_Status1.GetComponent<Text> ().text = string.Empty;
					settingsPanelInput.ObjectiveRescueSurvivors_Status2.GetComponent<Text> ().text = string.Empty;

					inGamePanelInputs.Value_Time.gameObject.SetActive (false);
					inGamePanelInputs.Value_TimeLabel.gameObject.SetActive (false);
//					inGamePanelInputs.Value_DayTime.gameObject.SetActive (false);
					inGamePanelInputs.Value_DaysPassed.gameObject.SetActive (false);
					inGamePanelInputs.Label_ReadyUpMessage1.gameObject.SetActive (false);
					inGamePanelInputs.Label_ReadyUpMessage2.gameObject.SetActive (false);
					inGamePanelInputs.Label_GameStarted.gameObject.SetActive (false);
				}

				if (!PhotonNetwork.offlineMode) {
					inGamePanelInputs.Value_Ping.text = PhotonNetwork.GetPing ().ToString ();
				} else {
					inGamePanelInputs.Value_Ping.text = str0;
				}

				//Time not started is gray
				if (!mTimer.isReady) {
					inGamePanelInputs.Value_Time.color = Color.gray;
				} else {
					if (((currTime / fullTime) > 0.5f)) {
						inGamePanelInputs.Value_Time.color = greenTimeColor;
					} else if (((currTime / fullTime) <= 0.5f) && ((currTime / fullTime) > 0.1f) && currTime > 10f) {
						inGamePanelInputs.Value_Time.color = Color.yellow;
					} else if (((currTime / fullTime) <= 0.1f) || currTime <= 10f) {
						inGamePanelInputs.Value_Time.color = Color.red;
					}
				}

				if (!GAMEEND) {
					inGamePanelInputs.Value_Time.text = getMinutes (currTime) + strColon + getSeconds (currTime);
				} else {
					inGamePanelInputs.Value_Time.text = str00 + strColon + str00;
				}

				inGamePanelInputs.Label_GameStarted.gameObject.SetActive (false);
			}
			yield return waitFor0_1;
		}
	}


	string getMinutes(double timeParam){
		int minutes = 0;
		string mString = string.Empty;
		minutes = (int)Mathf.Floor((float)(timeParam / 60));
		mString = minutes.ToString();
		//Update the Clock Icon Color for warning
		return mString;
	}

	string getSeconds(double timeParam){
		int seconds = 0;
		int minutes = 0;
		string sString = string.Empty;
		minutes = Mathf.FloorToInt((float)(timeParam / 60));
		//Converting remainder seconds into 60 number system
		seconds = (int)(((timeParam / 60) - minutes) * 60);
		sString = seconds.ToString ();
		if(seconds < 10){
			sString = str0 + sString;
		}
		return sString;
	}

	//cache string data
	string str00 = "00";
	string str06 = "06";
	string str12 = "12";
	string str18 = "18";
	string str0 = "0";
	string strDay = "DAY ";
	string strColon = ":";
	string gameEndClock = "--:--";
	string strTDM = "TEAM DEATH MATCH";
	string strRedLeads = "RED LEADS";
	string strBlueLeads = "BLUE LEADS";
	string strTied = "TEAMS TIED";
	string strReadyUp = "READY UP TO BEGIN";
	string msgRedLeads = "RED TEAM LEADS";
	string msgBlueLeads = "BLUE TEAM LEADS";
	string msgTied = "TEAMS ARE TIED";
	string msgSearchAndRescue = "SEARCH AND RESCUE";



	//DayIntervals
	//300 seconds = 5 mins
	//150 seconds = 2 and a half minutes
	//60 seconds = 1 minute
	public float minutesPerDay = 2.5f;
	public float secondsPerMinute = 60f;
	float dayIntervalLength = 0;
	public void ChangeTime(int gameMode){

		if (GAMEEND) {
			timeTextStandby.color = MidnightColor;
			timeText.color = MidnightColor;
			timeTextStandby.text = gameEndClock;
			timeText.text = gameEndClock;
			return;
		}


		dayIntervalLength = secondsPerMinute * minutesPerDay;

		if (gameMode != 99) {
			if (timeTextStandby) {
				if (!timeTextStandby.gameObject.GetActive ())
					timeTextStandby.gameObject.SetActive (true);
			}
			if (timeText) {
				if (!timeText.gameObject.GetActive ())
					timeText.gameObject.SetActive (true);
			}
			if (standbyDaysPassedText) {
				if (!standbyDaysPassedText.gameObject.GetActive ())
					standbyDaysPassedText.gameObject.SetActive (true);
			}
			if (daysPassedText) {
				if (!daysPassedText.gameObject.GetActive ())
					daysPassedText.gameObject.SetActive (true);
			}
		}
		
		if (gameMode == 99) {
			if (timeTextStandby) {
				if (!timeTextStandby.gameObject.GetActive ())
					timeTextStandby.gameObject.SetActive (true);
			}
			if (timeText) {
				if (!timeText.gameObject.GetActive ())
					timeText.gameObject.SetActive (true);
			}

			time += Time.fixedDeltaTime * 1000;
			if (time > 86400) {
				days += 1;
				time = 0;
			}
		} else {
			time = (float)currTime % dayIntervalLength * (-86400f / dayIntervalLength) + 86400f;
		}
			
		days = (int)mTimer.GetFullGameTime () / (int)dayIntervalLength - Mathf.FloorToInt ((float)currTime / dayIntervalLength);

//		if (time > 86400) {
//			days += 1;
//			time = 0;
//		}

		currentDayTime = TimeSpan.FromSeconds (time);
		string[] tempTime = currentDayTime.ToString ().Split (strColon [0]);
		hourOfDay = currentDayTime.Hours;
		if (!GAMEEND) {
			if (timeText) {
				if (tempTime [0].Equals (str00)) {

					timeText.color = MidnightColor;
				} else if (tempTime [0].Equals (str06)) {
					timeText.color = DawnColor;
				} else if (tempTime [0].Equals (str12)) {
					timeText.color = NoonColor;
				} else if (tempTime [0].Equals (str18)) {
					timeText.color = DuskColor;
				}
				if (tempTime [0].Length < 2)
					timeText.text = str0 + tempTime [0] + strColon + tempTime [1];
				else
					timeText.text = tempTime [0] + strColon + tempTime [1];			
			}
			if (timeTextStandby) {
				if (tempTime [0].Equals (str00)) {
					timeTextStandby.color = MidnightColor;
				} else if (tempTime [0].Equals (str06)) {
					timeTextStandby.color = DawnColor;
				} else if (tempTime [0].Equals (str12)) {
					timeTextStandby.color = NoonColor;
				} else if (tempTime [0].Equals (str18)) {
					timeTextStandby.color = DuskColor;
				}
				if (tempTime [0].Length < 2)
					timeTextStandby.text = str0 + tempTime [0] + strColon + tempTime [1];
				else
					timeTextStandby.text = tempTime [0] + strColon + tempTime [1];
			}
		}

		if (daysPassedText)
			daysPassedText.text = strDay + days;
		if (standbyDaysPassedText)
			standbyDaysPassedText.text = strDay + days;
		
		if (sunTransform)
			sunTransform.rotation = Quaternion.Euler (new Vector3 ((time - 21600) / 86400 * 360, 0, 0));

		if (time < 43200) {
			intensity = 1 - (43200 - time) / 43200;
			playerLightIntensity = ((43200 - time) / 43200 * 3);
		} else {
			intensity = 1 - ((43200 - time) / 43200 * -1);
			playerLightIntensity = (43200 - time) / 43200 * -1 * 3;
		}

		if (gameMode != 99) {
			sunLight.GetComponent<Light> ().color = Color.Lerp (SunColors [0], SunColors [1], intensity);
			RenderSettings.fogColor = Color.Lerp (fogNight, fogDay, intensity);
		} else {
			sunLight.GetComponent<Light> ().color = Color.Lerp (SunColors [0], SunColors [1], intensity);
			RenderSettings.fogColor = Color.Lerp (fogNightHomeBase, fogDayHomeBase, intensity);
		}
		if (playerLight)
			playerLight.intensity = playerLightIntensity;

	}

	public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey(RedTeamScoreProps))
		{
			redTeamKills = (int) propertiesThatChanged[RedTeamScoreProps];
		}
		if(propertiesThatChanged.ContainsKey(BlueTeamScoreProps))
		{
			blueTeamKills = (int) propertiesThatChanged[BlueTeamScoreProps];
		}
	}


}