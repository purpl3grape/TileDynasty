using System.IO;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MatchTimer : PunBehaviour
{
	KeyBindingManager kMgr;
	WaitForSeconds waitFor0_01 = new WaitForSeconds (0.1f * .6f);
	WaitForSeconds waitFor0_1 = new WaitForSeconds (0.1f * .6f);
	WaitForSeconds waitFor0_5 = new WaitForSeconds (0.5f * .6f);
	WaitForSeconds waitFor1 = new WaitForSeconds (1f * .6f);
	WaitForSeconds waitFor600 = new WaitForSeconds (600f);

	NetworkManager nManager;
	GuiManager guiManager;
	ScoringManager scoringManager;

	public PlayerInGamePanel playerInGamePanelInputs;
	public EndGamePanelInputs endGameInputs;
	public ControllerCanvasManager controllerCanvas;
	public AudioClip[] gameEndClips;
	public AudioClip[] readyUpClips;

	//STEAM STATS MAIN
	private bool steamKillStatSaved = false;
	private int statKill = 0;
	private bool steamDeathsStatSaved = false;
	private int statDeaths = 0;
	private bool steamBestKillstreakStatSaved = false;
	private int statBestKillstreak = 0;
	private bool steamHostagesRescuedStatSaved = false;
	private int statHostagesRescued = 0;
	private bool steamDamageDealtStatSaved = false;
	private int statDamageDealt = 0;
	private bool steamDamageReceivedStatSaved = false;
	private int statDamageReceived = 0;
	//STEAM STATS GAMES PLAYED
	private bool steamRescueMissionAttemptsSaved = false;
	private int statRescueMissionAttempts = 0;
	private bool steamRescueMissionSuccessSaved = false;
	private int statRescueMissionSuccess = 0;
	private bool steamRescueMissionFailureSaved = false;
	private int statRescueMissionFailure = 0;

	private bool steamGamesPlayedSaved = false;
	private int statGamesPlayed = 0;
	private bool steamGamesWonSaved = false;
	private int statGamesWon = 0;
	private bool steamGamesLostSaved = false;
	private int statGamesLost = 0;
	private bool steamGamesTiedSaved = false;
	private int statGamesTied = 0;
	private bool steamTDMGamesPlayedSaved = false;
	private int statTDMGamesPlayed = 0;
	private bool steamTileDynastyGamesPlayedSaved = false;
	private int statTileDynastyGamesPlayed = 0;
	private bool steamGarbageManGamesPlayedSaved = false;
	private int statGarbageManGamesPlayed = 0;
	private bool steamSurvivalGamesPlayedSaved = false;
	private int statSurvivalGamesPlayed = 0;
	//STEAM STATS ACCURACY (FIRED / HIT)
	private bool steamShotFired = false;
	private int statShotFired = 0;
	private bool steamShotHit = false;
	private int statShotHit = 0;
	private bool steamShotFiredHMG = false;
	private int statShotFiredHMG = 0;
	private bool steamShotHitHMG = false;
	private int statShotHitHMG = 0;
	private bool steamShotFiredRocket = false;
	private int statShotFiredRocket = 0;
	private bool steamShotHitRocket = false;
	private int statShotHitRocket = 0;
	private bool steamShotFiredRail = false;
	private int statShotFiredRail = 0;
	private bool steamShotHitRail = false;
	private int statShotHitRail = 0;
	private bool steamShotFiredEMP = false;
	private int statShotFiredEMP = 0;
	private bool steamShotHitEMP = false;
	private int statShotHitEMP = 0;
	//STEAM STATS SPECIAL ATTACKS
	private bool steamMeleeHitSaved = false;
	private int statMeleeHit = 0;
	private bool steamBullsEyeHitSaved = false;
	private int statBullsEyeHit = 0;
	private bool steamHeadShotHitSaved = false;
	private int statHeadShotHit = 0;

	//STEAM STATS ACCURACY (PERCENTAGE)
	private bool steamAccuracySaved = false;
	private float statAccuracy = 0f;
	private bool steamAccuracyHMGSaved = false;
	private float statAccuracyHMG = 0f;
	private bool steamAccuracyRocketSaved = false;
	private float statAccuracyRocket = 0f;
	private bool steamAccuracyRailSaved = false;
	private float statAccuracyRail = 0f;
	private bool steamAccuracyEMPSaved = false;
	private float statAccuracyEMP = 0f;

	[HideInInspector] public double SecondsBeforeStart = 300.0f;   // set in inspector
	[HideInInspector] public bool refreshKDStat = false;
	private double desiredGameTime = 999.0f;
	private bool gameTimeSyncd = false;
	private const string TimeToStartProp = "st";
	private double timeToStart = 0.0f;
	private bool masterclientSpawned = false;
	private bool readyUp = false;
	private float restartTimer = 8f;
	private bool displayScoreBoard = false;

	public bool isReady
	{
		get{
			return readyUp;
		}
	}

	public bool IsItTimeYet
	{
		get { return IsTimeToStartKnown && PhotonNetwork.time > this.timeToStart; }
	}
	
	public bool IsTimeToStartKnown
	{
		get { return this.timeToStart > 0.001f; }
	}
	
	public double SecondsUntilItsTime
	{
		get
		{
			if (this.IsTimeToStartKnown)
			{
				if(readyUp == true){
					double delta = 0;
					delta = this.timeToStart - (PhotonNetwork.time);
					return (delta > 0.0f) ? delta : 0.0f;
				}
				else{
					double delta = 0;
					delta = this.timeToStart - PhotonNetwork.time;
					return (delta > 0.0f) ? delta : 0.0f;
				}
			}
			else
			{
				return desiredGameTime;
			}
		}
		set{ this.timeToStart = value; }
	}
		

	[PunRPC]
	public void syncGameTimeForClients(int time, float minutesPerDay){
		//5 minutes per day

		int secs = 60;
		desiredGameTime = time * secs * minutesPerDay;
		SecondsBeforeStart = desiredGameTime;
		gameTimeSyncd = true;
	}

	public double GetFullGameTime(){
		return desiredGameTime;
	}

	[PunRPC]
	public void GameOverTimeEqualsZero(){
		SecondsBeforeStart = 0.0f;
	}

	public bool isGameOver = false;
	public IEnumerator GameOverCoroutine;
	public IEnumerator GameOver_Coroutine(){

		guiManager.Msg_Obj.gameObject.SetActive (false);
		guiManager.Msg_NPC.gameObject.SetActive (false);
		guiManager.Msg_Kill.gameObject.SetActive (false);
		guiManager.Msg_Death.gameObject.SetActive (false);
		guiManager.Msg_Area.gameObject.SetActive (false);
		guiManager.Msg_Ambush.gameObject.SetActive (false);
		guiManager.Msg_Item.gameObject.SetActive (false);
		guiManager.Msg_Spawn.gameObject.SetActive (false);
		guiManager.Msg_NPCAbandon.gameObject.SetActive (false);
		GameEndedWindowDisplay = true;
		endGameInputs.gameObject.SetActive (true);

		if (pMovement == null && nManager.PlayerObject) {
			pMovement = nManager.PlayerObject.GetComponentInChildren<PlayerMovement> ();
		}
		if (pShooting == null && nManager.PlayerObject) {
			pShooting = nManager.PlayerObject.GetComponentInChildren<PlayerShooting> ();
		}
		if (pHealth == null && nManager.PlayerObject) {
			pHealth = nManager.PlayerObject.GetComponentInChildren<Health> ();
		}
		if (pMovement) {
			pMovement.enabled = false;
		}
		if (pShooting) {
			pShooting.enabled = false;
		}
		if (pHealth) {
			pHealth.enabled = false;
		}

		playerInGamePanelInputs.Image_CrossHair.enabled = false;
			
		if (guiManager.CloseMessagePanelCoroutine != null) {
			StopCoroutine (guiManager.CloseMessagePanelCoroutine);
		}
		if (guiManager.MessageBuilderCoroutine != null) {
			StopCoroutine (guiManager.MessageBuilderCoroutine);
		}
		if (guiManager.MessageBuilderCoroutine2 != null) {
			StopCoroutine (guiManager.MessageBuilderCoroutine2);
		}
		guiManager.DisplayMessagePanelCoroutine = guiManager.DisplayMessagePanel_Coroutine ();
		StartCoroutine (guiManager.DisplayMessagePanelCoroutine);
		if (guiManager.currentGameMode == 1) {
			if (guiManager.redTeamKills < guiManager.blueTeamKills) {
				guiManager.initMessage = "BLUE TEAM WINS";
				guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#6496FAFF");
				scoringManager.GetComponent<AudioSource> ().PlayOneShot (gameEndClips [1]);
			} else if (guiManager.redTeamKills > guiManager.blueTeamKills) {
				guiManager.initMessage = "RED TEAM WINS";
				guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#FA3232FF");
				scoringManager.GetComponent<AudioSource> ().PlayOneShot (gameEndClips [2]);
			} else {
				guiManager.initMessage = "DRAW";
				guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#96FA32FF");
				scoringManager.GetComponent<AudioSource> ().PlayOneShot (gameEndClips [3]);
			}
		} else if (guiManager.currentGameMode == 2) {
			if (guiManager.HostagesRescued == 1) {
				guiManager.initMessage = "SAVED " + guiManager.HostagesRescued.ToString () + " SURVIVOR";
			} else {
				guiManager.initMessage = "SAVED " + guiManager.HostagesRescued.ToString () + " SURVIVORS";
			}
			guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#96FA32FF");
			scoringManager.GetComponent<AudioSource> ().PlayOneShot (gameEndClips [0]);
		}

		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return waitFor0_01;
		}
		yield return true;
		playerInGamePanelInputs.Image_CrossHair.enabled = false;
		GameOverCoroutine = null;
	}

	public IEnumerator DelayReadyUpCoroutine;
	public IEnumerator DelayReadyUp_Coroutine(){

		photonView.RPC ("SetReadyCountDown", PhotonTargets.AllBuffered, true);

		guiManager.Msg_Obj.gameObject.SetActive (false);
		guiManager.Msg_NPC.gameObject.SetActive (false);
		guiManager.Msg_Kill.gameObject.SetActive (false);
		guiManager.Msg_Death.gameObject.SetActive (false);
		guiManager.Msg_Area.gameObject.SetActive (false);
		guiManager.Msg_Ambush.gameObject.SetActive (false);
		guiManager.Msg_Item.gameObject.SetActive (false);
		guiManager.Msg_Spawn.gameObject.SetActive (false);
		guiManager.Msg_NPCAbandon.gameObject.SetActive (false);

		if (pMovement == null && nManager.PlayerObject) {
			pMovement = nManager.PlayerObject.GetComponentInChildren<PlayerMovement> ();
		}
		if (pShooting == null && nManager.PlayerObject) {
			pShooting = nManager.PlayerObject.GetComponentInChildren<PlayerShooting> ();
		}
		if (pShooting) {
			pShooting.enabled = false;
		}
		if (pHealth == null && nManager.PlayerObject) {
			pHealth = nManager.PlayerObject.GetComponentInChildren<Health> ();
		}
			
		pHealth.resetHealthArmorResource (200, 200, 2);
		guiManager.INITIALIZE_GAMESTART_FLAG = true;
		photonView.RPC ("PlayerStatsResetScore", PhotonTargets.AllBuffered, PhotonNetwork.player.NickName);
		pHealth.DieNoStatChange ();	

		yield return waitFor0_5;

		guiManager.Msg_Obj.gameObject.SetActive (false);
		guiManager.Msg_NPC.gameObject.SetActive (false);
		guiManager.Msg_Kill.gameObject.SetActive (false);
		guiManager.Msg_Death.gameObject.SetActive (false);
		guiManager.Msg_Area.gameObject.SetActive (false);
		guiManager.Msg_Ambush.gameObject.SetActive (false);
		guiManager.Msg_Item.gameObject.SetActive (false);
		guiManager.Msg_Spawn.gameObject.SetActive (false);
		guiManager.Msg_NPCAbandon.gameObject.SetActive (false);


		if (guiManager.CloseMessagePanelCoroutine != null) {
			StopCoroutine (guiManager.CloseMessagePanelCoroutine);
		}
		if (guiManager.MessageBuilderCoroutine != null) {
			StopCoroutine (guiManager.MessageBuilderCoroutine);
		}
		if (guiManager.MessageBuilderCoroutine2 != null) {
			StopCoroutine (guiManager.MessageBuilderCoroutine2);
		}
		//3 SECONDS
		guiManager.DisplayMessagePanelCoroutine = guiManager.DisplayMessagePanel_Coroutine ();
		StartCoroutine (guiManager.DisplayMessagePanelCoroutine);
		guiManager.initMessage = "3";
		guiManager.MessageBuilderCoroutine2 = guiManager.MessageBuilder_Coroutine2 (guiManager.initMessage, "#96FA32FF");
		StartCoroutine (guiManager.MessageBuilderCoroutine2);
		while (!guiManager.isMessageOver) {
			yield return waitFor0_01;
		}
		scoringManager.GetComponent<AudioSource> ().PlayOneShot (readyUpClips [0]);
		yield return waitFor1;

		//2 SECONDS
		guiManager.initMessage = "2";
		guiManager.MessageBuilderCoroutine2 = guiManager.MessageBuilder_Coroutine2 (guiManager.initMessage, "#96FA32FF");
		StartCoroutine (guiManager.MessageBuilderCoroutine2);
		while (!guiManager.isMessageOver) {
			yield return waitFor0_01;
		}
		scoringManager.GetComponent<AudioSource> ().PlayOneShot (readyUpClips [1]);
		yield return waitFor1;

		//1 SECOND
		guiManager.initMessage = "1";
		guiManager.MessageBuilderCoroutine2 = guiManager.MessageBuilder_Coroutine2 (guiManager.initMessage, "#96FA32FF");
		StartCoroutine (guiManager.MessageBuilderCoroutine2);
		while (!guiManager.isMessageOver) {
			yield return waitFor0_01;
		}
		scoringManager.GetComponent<AudioSource> ().PlayOneShot (readyUpClips [2]);
		yield return waitFor1;

		scoringManager.GetComponent<AudioSource> ().PlayOneShot (readyUpClips [3]);
		photonView.RPC ("DemoReady", PhotonTargets.AllBuffered);
		photonView.RPC ("SetReadyCountDown", PhotonTargets.AllBuffered, false);

		//BEGIN
		if (guiManager.currentGameMode == 1) {
			guiManager.initMessage = "TEAM DEATH MATCH";
		} else if (guiManager.currentGameMode == 2) {
			guiManager.initMessage = "SEARCH AND RESCUE";
		} else {
			guiManager.initMessage = "BEGIN";
		}
		guiManager.MessagePanel.GetComponent<MessagePanelInputs> ().MessageObject.GetComponent<Text> ().text = string.Empty;
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#96FA32FF");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return waitFor0_01;
		}
		yield return waitFor1;
		guiManager.CloseMessagePanelCoroutine = guiManager.CloseMessagePanel_Coroutine ();
		StartCoroutine (guiManager.CloseMessagePanelCoroutine);

		yield return true;
		DelayReadyUpCoroutine = null;
	}

	public bool READYCOUNTDOWN = false;
	[PunRPC]
	public void SetReadyCountDown(bool val){
		READYCOUNTDOWN = val;
	}

	[PunRPC]
	public void Ready(){

		int numberPlayersReady = GameObject.FindGameObjectsWithTag ("Ready").Length;
		int numberPlayers = GameObject.FindGameObjectsWithTag ("Player").Length;
		if (numberPlayers > 1) {
			if (((numberPlayersReady / numberPlayers) > 0.5)) {

				if (!nManager.ChatPanel.GetActive () && !nManager.SettingsPanel.GetActive ()) {
					readyUp = true;

					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "tilescaptured");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "killstreak");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "kills");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "deaths");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "damaged");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "damager");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shotfired_hmg");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shotfired_rocket");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shotfired_rail");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shotfired_emp");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shothit_hmg");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shothit_rocket");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shothit_rail");
					scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shothit_emp");
				}
			}

		}
		//Debug.Log("ready player: " + numberPlayersReady + ", num Players: " + numberPlayers + ", ready: " + readyUp);

	}
	
	[PunRPC]
	public void DemoReady(){

//		if (!nManager.ChatPanel.GetActive () && !nManager.SettingsPanel.GetActive ()) {

		if (guiManager.currentGameMode == 99)
			return;
		
			readyUp = true;

		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "tilescaptured");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "killstreak");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "kills");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "deaths");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "damaged");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "damager");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shotfired_hmg");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shotfired_rocket");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shotfired_rail");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shotfired_emp");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shothit_hmg");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shothit_rocket");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shothit_rail");
		scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, nManager.networkPlayerName, "shothit_emp");



//		}
	}



	void Start(){
		readyUp = false;
		nManager = GetComponent<NetworkManager> ();
		guiManager = GetComponent<GuiManager> ();
		scoringManager = GetComponent<ScoringManager> ();
		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();

		StartCoroutine (MatchStatesInitialize_Coroutine());
	}

	//Syncs the starting time, so want to invoke early on.
	IEnumerator MatchStatesInitialize_Coroutine(){
		while (true) {

			//Syncing game time
			if (PhotonNetwork.isMasterClient && !gameTimeSyncd) {
				float minPerDay = guiManager.minutesPerDay;
				GetComponent<PhotonView> ().RPC ("syncGameTimeForClients", PhotonTargets.AllBuffered, MainMenu.gameTimeDays, minPerDay);
			}

			//Calling the "Game Start" Message on the GUIManager's side when SetNameTag on Player calls DemoReadyUp to set this readyUp to true
			if (readyUp) {
				if (PhotonNetwork.isMasterClient) {
					//The master client checks if a start time is set. we check a min value
					if (!this.IsTimeToStartKnown && PhotonNetwork.time > 0.0001f) {
						//No startTime set for room. calculate and set it as property of this room
						this.timeToStart = PhotonNetwork.time + SecondsBeforeStart;

						ExitGames.Client.Photon.Hashtable timeProps = new ExitGames.Client.Photon.Hashtable () { {
								TimeToStartProp,
								this.timeToStart
							} };
						PhotonNetwork.room.SetCustomProperties (timeProps);

						//Do not allow to be visible upon starting for now (Can be turned into room option)
						PhotonNetwork.room.IsVisible = false;
						PhotonNetwork.room.IsOpen = false;
						guiManager.GetComponent<PhotonView> ().RPC ("networkGameStart", PhotonTargets.AllBuffered, true);

					}
				}
			}
			if (!readyUp) {
				yield return waitFor0_1;
			} else {
				yield return waitFor600;
			}
		}
	}

	bool RunOnce = false;
	bool BeginCountDownCoroutine = false;
	[PunRPC]
	public void ReadyAllPlayersForCountDown(){
		BeginCountDownCoroutine = true;
	}

	PlayerMovement pMovement;
	PlayerShooting pShooting;
	Health pHealth;
	MouseLook_TileDynasty mLookTD;
	GameObject mainCam;

	public bool GameEndedWindowDisplay = false;
	void FixedUpdate(){

		if (kMgr.GetKeyDownPublic (keyType.readyUp) && !BeginCountDownCoroutine) {
			photonView.RPC ("ReadyAllPlayersForCountDown", PhotonTargets.AllBuffered);
		}

		if(BeginCountDownCoroutine && !RunOnce){
			if (guiManager.currentGameMode == 99)
				return;

			if (nManager.PlayerObject) {
				if (DelayReadyUpCoroutine == null) {
					RunOnce = true;
					DelayReadyUpCoroutine = DelayReadyUp_Coroutine ();
					StartCoroutine (DelayReadyUpCoroutine);
				}
			}
			//			mTimer.GetComponent<PhotonView> ().RPC ("DemoReady", PhotonTargets.AllBuffered);
		}

		//TIMER IS UP, GAME OVER. NOW SHOULD BE A GOOD TIME TO PULL UP SCOREBOARD
		if (this.SecondsUntilItsTime <= 0 || guiManager.GAMEEND || (guiManager.currentGameMode == 2 && guiManager.currentHostagesRemaining <= 0)) {

			//SAVING GAME STATS TO STEAM
			if (!steamHostagesRescuedStatSaved) {
				Steamworks.SteamUserStats.GetStat ("HostagesRescued", out statHostagesRescued);
				steamHostagesRescuedStatSaved = Steamworks.SteamUserStats.SetStat ("HostagesRescued", statHostagesRescued + scoringManager.GetScore (PhotonNetwork.player.NickName, "hostagescaptured"));
			}
			if (!steamKillStatSaved) {
				Steamworks.SteamUserStats.GetStat ("Kills", out statKill);
				steamKillStatSaved = Steamworks.SteamUserStats.SetStat ("Kills", statKill + scoringManager.GetScore (PhotonNetwork.player.NickName, "kills"));
			}
			if (!steamDeathsStatSaved) {
				Steamworks.SteamUserStats.GetStat ("Deaths", out statDeaths);
				steamDeathsStatSaved = Steamworks.SteamUserStats.SetStat ("Deaths", statDeaths + scoringManager.GetScore (PhotonNetwork.player.NickName, "deaths"));
			}
			if (!steamDamageDealtStatSaved) {
				Steamworks.SteamUserStats.GetStat ("DamageDealt", out statDamageDealt);
				steamDamageDealtStatSaved = Steamworks.SteamUserStats.SetStat ("DamageDealt", statDamageDealt + scoringManager.GetScore (PhotonNetwork.player.NickName, "damaged"));
			}
			if (!steamDamageReceivedStatSaved) {
				Steamworks.SteamUserStats.GetStat ("DamageReceived", out statDamageReceived);
				steamDamageReceivedStatSaved = Steamworks.SteamUserStats.SetStat ("DamageReceived", statDamageReceived + scoringManager.GetScore (PhotonNetwork.player.NickName, "damager"));
			}
			if (!steamBestKillstreakStatSaved) {
				Steamworks.SteamUserStats.GetStat ("BestKillStreak", out statBestKillstreak);
				if (scoringManager.GetScore (PhotonNetwork.player.NickName, "killstreak_highest") > statBestKillstreak) {
					steamBestKillstreakStatSaved = Steamworks.SteamUserStats.SetStat ("BestKillStreak", scoringManager.GetScore (PhotonNetwork.player.NickName, "killstreak_highest"));
				}
			}

			//GAMES PLAYED STATS
			if (!steamGamesPlayedSaved) {

				Steamworks.SteamUserStats.GetStat ("GamesPlayed", out statGamesPlayed);
				steamGamesPlayedSaved = Steamworks.SteamUserStats.SetStat ("GamesPlayed", statGamesPlayed + 1);
			}
			if ((!steamTileDynastyGamesPlayedSaved) && guiManager.currentGameMode == 0) {
				Steamworks.SteamUserStats.GetStat ("TileDynastyGamesPlayed", out statTileDynastyGamesPlayed);
				steamTileDynastyGamesPlayedSaved = Steamworks.SteamUserStats.SetStat ("TileDynastyGamesPlayed", statTileDynastyGamesPlayed + 1);
				Debug.Log ("TileDynasty: " + statTileDynastyGamesPlayed);
			}
			if ((!steamTDMGamesPlayedSaved) && guiManager.currentGameMode == 1) {
				Steamworks.SteamUserStats.GetStat ("TDMGamesPlayed", out statTDMGamesPlayed);
				steamTDMGamesPlayedSaved = Steamworks.SteamUserStats.SetStat ("TDMGamesPlayed", statTDMGamesPlayed + 1);
				Debug.Log ("TDM: " + statTDMGamesPlayed);
			}
			if ((!steamSurvivalGamesPlayedSaved) && guiManager.currentGameMode == 2) {
				Steamworks.SteamUserStats.GetStat ("SurvivalGamesPlayed", out statSurvivalGamesPlayed);
				steamSurvivalGamesPlayedSaved = Steamworks.SteamUserStats.SetStat ("SurvivalGamesPlayed", statSurvivalGamesPlayed + 1);
				Debug.Log ("Survival: " + statSurvivalGamesPlayed);
			}
			if ((!steamGarbageManGamesPlayedSaved) && guiManager.currentGameMode == 3) {
				Steamworks.SteamUserStats.GetStat ("GarbageManGamesPlayed", out statGarbageManGamesPlayed);
				steamGarbageManGamesPlayedSaved = Steamworks.SteamUserStats.SetStat ("GarbageManGamesPlayed", statGarbageManGamesPlayed + 1);
				Debug.Log ("GarbageMan: " + statGarbageManGamesPlayed);
			}
			//WINS/LOSS
			if (guiManager.currentGameMode == 1) {
				if (guiManager.gameAnnounceValue == (int)enumGameStatus.finalBlueWins) {
					if (nManager.PlayerObject.GetComponentInChildren<TeamMember> ().teamID == 1) {
						if (!steamGamesWonSaved) {
							Steamworks.SteamUserStats.GetStat ("Wins", out statGamesWon);
							steamGamesWonSaved = Steamworks.SteamUserStats.SetStat ("Wins", statGamesWon + 1);
						}
					} else if (nManager.PlayerObject.GetComponentInChildren<TeamMember> ().teamID == 2) {
						if (!steamGamesLostSaved) {
							Steamworks.SteamUserStats.GetStat ("Losses", out statGamesLost);
							steamGamesLostSaved = Steamworks.SteamUserStats.SetStat ("Losses", statGamesLost + 1);
						}
					}
				} else if (guiManager.gameAnnounceValue == (int)enumGameStatus.finalRedWins) {
					if (nManager.PlayerObject.GetComponentInChildren<TeamMember> ().teamID == 2) {
						if (!steamGamesWonSaved) {
							Steamworks.SteamUserStats.GetStat ("Wins", out statGamesWon);
							steamGamesWonSaved = Steamworks.SteamUserStats.SetStat ("Wins", statGamesWon + 1);
						}
					} else if (nManager.PlayerObject.GetComponentInChildren<TeamMember> ().teamID == 1) {
						if (!steamGamesLostSaved) {
							Steamworks.SteamUserStats.GetStat ("Losses", out statGamesLost);
							steamGamesLostSaved = Steamworks.SteamUserStats.SetStat ("Losses", statGamesLost + 1);
						}
					}
				} else if (guiManager.gameAnnounceValue == (int)enumGameStatus.finalTeamsTied) {
					if (!steamGamesTiedSaved) {
						Steamworks.SteamUserStats.GetStat ("Draws", out statGamesTied);
						steamGamesTiedSaved = Steamworks.SteamUserStats.SetStat ("Draws", statGamesTied + 1);
					}
				} else if (guiManager.gameAnnounceValue == (int)enumGameStatus.finalGameOver) {
					if (guiManager.currentHostagesRemaining <= 0) {
						if (!steamGamesWonSaved) {
							Steamworks.SteamUserStats.GetStat ("Wins", out statGamesWon);
							steamGamesWonSaved = Steamworks.SteamUserStats.SetStat ("Wins", statGamesWon + 1);
						}
					} else {
						if (!steamGamesLostSaved) {
							Steamworks.SteamUserStats.GetStat ("Losses", out statGamesLost);
							steamGamesLostSaved = Steamworks.SteamUserStats.SetStat ("Losses", statGamesLost + 1);
						}
					}
				}
			} else if (guiManager.currentGameMode == 2) {
				if (!steamRescueMissionAttemptsSaved) {
					Steamworks.SteamUserStats.GetStat ("ResuceMissionAttempts", out statRescueMissionAttempts);
					steamRescueMissionAttemptsSaved = Steamworks.SteamUserStats.SetStat ("ResuceMissionAttempts", statRescueMissionAttempts + 1);					
				}
				if (guiManager.HostagesRescued > 0) {
					if (!steamRescueMissionSuccessSaved) {
						Steamworks.SteamUserStats.GetStat ("ResuceMissionSuccess", out statRescueMissionSuccess);
						steamRescueMissionSuccessSaved = Steamworks.SteamUserStats.SetStat ("ResuceMissionSuccess", statRescueMissionSuccess + 1);					
					}
				} else {
					if (!steamRescueMissionFailureSaved) {
						Steamworks.SteamUserStats.GetStat ("ResuceMissionFailure", out statRescueMissionFailure);
						steamRescueMissionFailureSaved = Steamworks.SteamUserStats.SetStat ("ResuceMissionFailure", statRescueMissionFailure + 1);					
					}
				}
			}

			//ACCURACY (FIRED)
			if (!steamShotFiredHMG) {
				Steamworks.SteamUserStats.GetStat ("ShotsFiredHMG", out statShotFiredHMG);
				steamShotFiredHMG = Steamworks.SteamUserStats.SetStat ("ShotsFiredHMG", statShotFiredHMG + scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_hmg"));
			}
			if (!steamShotFiredRocket) {
				Steamworks.SteamUserStats.GetStat ("ShotsFiredRocket", out statShotFiredRocket);
				steamShotFiredRocket = Steamworks.SteamUserStats.SetStat ("ShotsFiredRocket", statShotFiredRocket + scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_rocket"));
			}
			if (!steamShotFiredRail) {
				Steamworks.SteamUserStats.GetStat ("ShotsFiredRail", out statShotFiredRail);
				steamShotFiredRail = Steamworks.SteamUserStats.SetStat ("ShotsFiredRail", statShotFiredRail + scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_rail"));
			}
			if (!steamShotFiredEMP) {
				Steamworks.SteamUserStats.GetStat ("ShotsFiredEMP", out statShotFiredEMP);
				steamShotFiredEMP = Steamworks.SteamUserStats.SetStat ("ShotsFiredEMP", statShotFiredEMP + scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_emp"));
			}
			if (!steamShotFired) {
				Steamworks.SteamUserStats.GetStat ("ShotsFired", out statShotFired);
				steamShotFired = Steamworks.SteamUserStats.SetStat ("ShotsFired", statShotFired
					+ scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_hmg")
					+ scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_rocket")
					+ scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_rail")
					+ scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_emp")
				);
			}
			//ACCURACY (HIT)
			if (!steamShotHitHMG) {
				Steamworks.SteamUserStats.GetStat ("ShotsHitHMG", out statShotHitHMG);
				steamShotHitHMG = Steamworks.SteamUserStats.SetStat ("ShotsHitHMG", statShotHitHMG + scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_hmg"));
			}
			if (!steamShotHitRocket) {
				Steamworks.SteamUserStats.GetStat ("ShotsHitRocket", out statShotHitRocket);
				steamShotHitRocket = Steamworks.SteamUserStats.SetStat ("ShotsHitRocket", statShotHitRocket + scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_rocket"));
			}
			if (!steamShotHitRail) {
				Steamworks.SteamUserStats.GetStat ("ShotsHitRail", out statShotHitRail);
				steamShotHitRail = Steamworks.SteamUserStats.SetStat ("ShotsHitRail", statShotHitRail + scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_rail"));
			}
			if (!steamShotHitEMP) {
				Steamworks.SteamUserStats.GetStat ("ShotsHitEMP", out statShotHitEMP);
				steamShotHitEMP = Steamworks.SteamUserStats.SetStat ("ShotsHitEMP", statShotHitEMP + scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_emp"));
			}
			if (!steamShotHit) {
				Steamworks.SteamUserStats.GetStat ("ShotsHit", out statShotHit);
				steamShotHit = Steamworks.SteamUserStats.SetStat ("ShotsHit", statShotHit
					+ scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_hmg")
					+ scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_rocket")
					+ scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_rail")
					+ scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_emp")
				);
			}
			if (!steamMeleeHitSaved) {
				Steamworks.SteamUserStats.GetStat ("MeleeHits", out statMeleeHit);
				steamMeleeHitSaved = Steamworks.SteamUserStats.SetStat ("MeleeHits", statMeleeHit + scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_melee"));
			}
			if (!steamBullsEyeHitSaved) {
				Steamworks.SteamUserStats.GetStat ("BullsEyeHits", out statBullsEyeHit);
				steamBullsEyeHitSaved = Steamworks.SteamUserStats.SetStat ("BullsEyeHits", statBullsEyeHit + scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_bullseye"));
			}
			if (!steamHeadShotHitSaved) {
				Steamworks.SteamUserStats.GetStat ("HeadShotHits", out statHeadShotHit);
				steamHeadShotHitSaved = Steamworks.SteamUserStats.SetStat ("HeadShotHits", statHeadShotHit + scoringManager.GetScore (PhotonNetwork.player.NickName, "shothit_headshot"));
			}

			//ACCURACY (CALCULATION)
			if (!steamAccuracyHMGSaved && scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_hmg") > 0) {
				Steamworks.SteamUserStats.GetStat ("ShotsHitHMG", out statShotHitHMG);
				Steamworks.SteamUserStats.GetStat ("ShotsFiredHMG", out statShotFiredHMG);
				statAccuracyHMG = (float)statShotHitHMG / (float)statShotFiredHMG;
				steamAccuracyHMGSaved = Steamworks.SteamUserStats.SetStat ("AccuracyHMG", statAccuracyHMG);
			}
			if (!steamAccuracyRocketSaved && scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_rocket") > 0) {
				Steamworks.SteamUserStats.GetStat ("ShotsHitRocket", out statShotHitRocket);
				Steamworks.SteamUserStats.GetStat ("ShotsFiredRocket", out statShotFiredRocket);
				statAccuracyRocket = (float)statShotHitRocket / (float)statShotFiredRocket;
				steamAccuracyRocketSaved = Steamworks.SteamUserStats.SetStat ("AccuracyRocket", statAccuracyRocket);
			}
			if (!steamAccuracyRailSaved && scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_rail") > 0) {
				Steamworks.SteamUserStats.GetStat ("ShotsHitRail", out statShotHitRail);
				Steamworks.SteamUserStats.GetStat ("ShotsFiredRail", out statShotFiredRail);
				statAccuracyRail = (float)statShotHitRail / (float)statShotFiredRail;
				steamAccuracyRailSaved = Steamworks.SteamUserStats.SetStat ("AccuracyRail", statAccuracyRail);
			}
			if (!steamAccuracyEMPSaved && scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_emp") > 0) {
				Steamworks.SteamUserStats.GetStat ("ShotsHitEMP", out statShotHitEMP);
				Steamworks.SteamUserStats.GetStat ("ShotsFiredEMP", out statShotFiredEMP);
				statAccuracyEMP = (float)statShotHitEMP / (float)statShotFiredEMP;
				steamAccuracyEMPSaved = Steamworks.SteamUserStats.SetStat ("AccuracyEMP", statAccuracyEMP);
			}
			if (!steamAccuracySaved && (scoringManager.GetScore (PhotonNetwork.player.NickName, "shotfired_hmg") > 0 || scoringManager.GetScore (PhotonNetwork.player.name, "shotfired_rocket") > 0 || scoringManager.GetScore (PhotonNetwork.player.name, "shotfired_rail") > 0 || scoringManager.GetScore (PhotonNetwork.player.name, "shotfired_emp") > 0)) {
				Steamworks.SteamUserStats.GetStat ("ShotsHit", out statShotHit);
				Steamworks.SteamUserStats.GetStat ("ShotsFired", out statShotFired);
				statAccuracy = (float)statShotHit / (float)statShotFired;
				steamAccuracySaved = Steamworks.SteamUserStats.SetStat ("Accuracy", statAccuracy);
			}

			Steamworks.SteamUserStats.StoreStats ();


//			Debug.Log ("statHeadShotHit: " + statHeadShotHit + ":" + scoringManager.GetScore (PhotonNetwork.player.name, "shothit_headshot") + ", statBullsEyeHit: " + statBullsEyeHit + ":" + scoringManager.GetScore (PhotonNetwork.player.name, "shothit_bullseye") + ", statMeleeHit: " + statMeleeHit + ":" + scoringManager.GetScore (PhotonNetwork.player.name, "shothit_melee"));

			if (pMovement == null && nManager.PlayerObject) {
				pMovement = nManager.PlayerObject.GetComponentInChildren<PlayerMovement> ();
			}
			if (pShooting == null && nManager.PlayerObject) {
				nManager.PlayerObject.GetComponentInChildren<PlayerShooting> ();
			}
			if (mLookTD == null && nManager.PlayerObject) {
				mLookTD = nManager.PlayerObject.GetComponentInChildren<MouseLook_TileDynasty> ();
			}
//			if (mainCam == null && nManager.PlayerObject) {
//				mainCam = nManager.PlayerObject.transform.Find ("Main Camera").gameObject;
//			}

			if (nManager.PlayerObject) {
				//FINAL SCORE TO USE THE STANDBYGUI (3 Seconds should be long enough)
//				GetComponent<GuiManager> ().DisplayInGameGUI = false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;


				if (!isGameOver) {
					isGameOver = true;
					GameOverCoroutine = GameOver_Coroutine ();
					StartCoroutine (GameOverCoroutine);
				}
					
				if (pMovement)
					pMovement.enabled = false;
				if (pShooting)
					pShooting.enabled = false;
				if (mLookTD)
					mLookTD.enabled = false;
			}

			//Restart Match Timer After Given Amount of Seconds//
			restartTimer -= Time.fixedDeltaTime;
		}

	}


	public void Button_EndGameReturnToHomeBase(){
		nManager.SceneLoadingPanel.SetActive (true);
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.DestroyAll ();
		}
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene(0);
	}

	public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey(TimeToStartProp))
		{
			this.timeToStart = (double) propertiesThatChanged[TimeToStartProp];
//			Debug.Log("Got StartTime: " + this.timeToStart + ", is it time yet: " + this.IsItTimeYet);
		}
	}

	public bool GetDisplayStatus(){
		return displayScoreBoard;
	}

	public void setMasterClientSpawned(bool val){
		masterclientSpawned = val;
	}
	
}