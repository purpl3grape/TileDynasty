using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Collections;
using Steamworks;
using Photon;

public class Health : PunBehaviour {

	KeyBindingManager kMgr;

	public bool steamCheck_FirstKillAchievement;

	PhotonView pv;
	FXManager fxManager;
	GuiManager guiManager;
	ScoringManager sManager;
	PlayerGuiManager playerGuiManager;
	NetworkManager nManager;
	MatchTimer matchTimer;
	ScoringManager scoringManager;
	SettingsManager settingsManager;
	TeamMember teamMember;
	EnemyMovement enemyMovement;
	public PlayerMovement pMovement;
	public PlayerShooting pShooting;
	public GameObject myCamera;
	public SkinnedMeshRenderer smr;
	public Collider PlayerHeadCollider;
	public Animator playerAnimator;

	string myPlayerName = string.Empty;

	public float enemyHitPoints = 120;
	public float enemyArmorPoints = 0f;


	public float hitPoints = 100f;
	public float armorPoints = 25;
	public float currentHitPoints;
	public float currentArmorPoints;
	public bool isPlayerDead = false;

	[HideInInspector] public int DamageReceivedCount = 0;

	//HEALTH STUFF

	WaitForSeconds waitFor1Sec = new WaitForSeconds (1f);
	WaitForSeconds waitFor4Sec = new WaitForSeconds (4f);


	//HEALTH ANIM STUFF
	public AudioClip[] health_pickup;
	public GameObject BloodSplatterPrefab;
	public GameObject HurtPrefab;

	public GameObject MiniMapBlip;

	//ItemPickups on Health Script
	[HideInInspector] public string itemPickupPlayerName = string.Empty;
	[HideInInspector] public string itemPickUpName = string.Empty;

	//Assist
	[HideInInspector] public string lastAttackerPlayerName = string.Empty;
	[HideInInspector] public string secondLastAttackerPlayerName = string.Empty;

	//Enemy Health (Sync'd var)
	[HideInInspector] public bool isEnemyDead=false;

	//HEALTH LIMITER
	private float Health_OneSec = 1f;


	//ARMOR STUFF
	private bool armorPicked = false;

	//ARMOR ANIM STUFF
	public AudioClip[] armor_pickup;

	//ARMOR LIMITER
	private float Armor_OneSec = 1f;


	//GARBAGE MAN MODE STUFF
	[HideInInspector] public int garbageCollected = 0;

	private bool gameStartFlag = false;
	private Transform tr;
	PhotonView scenePV;
	SpawnSpot[] spawnSpots;
	GameObject[] ZombieSpawnSpots;
	GameObject[] EnemySpawnSpots;


	EnemySpawnTrigger enemySpawnTrigger;
	GameObject[] BaseEnemyNodes;
	GameObject[] BaseEnemySpawnSpots;
	GameObject[] BaseHostageSpawnSpots;
	GameObject[] Base1HostageSpawnSpots;
	GameObject[] Base2HostageSpawnSpots;
	GameObject[] Base3HostageSpawnSpots;
	GameObject[] Base4HostageSpawnSpots;
	GameObject[] CivilianSpawnSpots;
	[HideInInspector] public GameObject[] BluePlayerSpawnSpots;
	[HideInInspector] public GameObject[] RedPlayerSpawnSpots;

	public SoundSource soundSource;
	Transform soundSourceTr;
    
	CapsuleCollider capsuleCollider;
	NavMeshAgent agent;
	CapsuleCollider headCapsuleCollider;
	Rigidbody rBody;
	[HideInInspector] public int shooterPhotonID = -1;
	[HideInInspector] public string shooterTag = string.Empty;
	[HideInInspector] public bool isDodgePlayerAttack = false;

	private string changeScoreRPC = "ChangeScore";
	private string UpdateStatsForKillerRPC = "UpdateStatsForKiller";
	private string syncEnemyHPRPC = "SyncEnemyHP";

	public Material[] HostageIndicatorMaterials;
	public GameObject HostageIndicator;

	// Use this for initialization
	void Start () {

		gameStartFlag = false;

		isPlayerDead = false;

		currentHitPoints = hitPoints;
		currentArmorPoints = armorPoints;

		//NETWORK / FX / MAMNAGER TYPE GAME OBJECTS
		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();
		fxManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<FXManager> ();
		guiManager = fxManager.GetComponent<GuiManager> ();
		sManager = fxManager.GetComponent<ScoringManager> ();
		playerGuiManager = GetComponent<PlayerGuiManager> ();
		nManager = fxManager.GetComponent<NetworkManager> ();
		matchTimer = fxManager.GetComponent<MatchTimer> ();
		scoringManager = fxManager.GetComponent<ScoringManager> ();
		settingsManager = fxManager.GetComponent<SettingsManager> ();
		scenePV = scoringManager.GetComponent<PhotonView> ();
		pv = GetComponent<PhotonView> ();
		myPlayerName = nManager.GetPlayerName ();

		soundSourceTr = soundSource.transform;
		teamMember = GetComponent<TeamMember> ();
		tr = transform;
		agent = GetComponent<NavMeshAgent> ();

		if (CompareTag (stringEnemyTag)) {
			if (PhotonNetwork.isMasterClient || 1==1) {
				spawnSpots = GameObject.FindObjectsOfType<SpawnSpot> ();
				ZombieSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotZombie");
				EnemySpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotEnemy");
				Base1HostageSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBase1Hostage");
				Base2HostageSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBase2Hostage");
				Base3HostageSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBase3Hostage");
				Base4HostageSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBase4Hostage");
				CivilianSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotCivilian");
				BluePlayerSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBlue");
				RedPlayerSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotRed");
				capsuleCollider = GetComponent<CapsuleCollider> ();
				headCapsuleCollider = GetComponent<CapsuleCollider> ();
				enemyMovement = GetComponent<EnemyMovement> ();
				rBody = GetComponent<Rigidbody> ();

				ResetIsEnemyUnderAttackCoroutine = ResetIsEnemyUnderAttack_Coroutine ();
				StartCoroutine (ResetIsEnemyUnderAttackCoroutine);
			
				GetSpawnGameObject ();

			}
		}
		if (CompareTag ("Player")) {
			if (pv.isMine) {
				guiManager.playerHasRespawned = true;
			}
			agent.enabled = false;
			if (teamMember) {
				if (teamMember.classID == 1) {
					//Defender
					resetHealthArmorResource (200, 200, teamMember.classID);
				} else {
					//Not Defender
					resetHealthArmorResource (100, 100, teamMember.classID);
				}
			}
			if (teamMember.classID == 1) {
				//Defender
				StartCoroutine (classHealthLimiter (200, teamMember.classID));
				StartCoroutine (classArmorLimiter (200, teamMember.classID));
			} else {
				//Not Defender
				StartCoroutine (classHealthLimiter (100, teamMember.classID));
				StartCoroutine (classArmorLimiter (100, teamMember.classID));
			}
		}

		DamageReceivedCount = sManager.GetScore (teamMember.playerName, sManager.damagereceivedStat);
	}
		

	//These guys to be called when Enemy is losing HP
	[PunRPC]
	public void updateEnemyHP(float amt, int shooterID, string tagName){
		enemyHitPoints -= amt;
		shooterPhotonID = shooterID;
		shooterTag = tagName;
	}

	[PunRPC]
	public void updateEnemyArmor(float amt){
		enemyArmorPoints -= amt;
	}

	[PunRPC]
	public void setLastAttackerName(string val){
		if (val == string.Empty) {
			secondLastAttackerPlayerName = string.Empty;
			lastAttackerPlayerName = string.Empty;
		} else {
			if (lastAttackerPlayerName != val) {
				secondLastAttackerPlayerName = lastAttackerPlayerName;
				lastAttackerPlayerName = val;
			} else {
				lastAttackerPlayerName = val;
			}
		}
	}


	private bool hasCommittedSuicide = false;
	private string stringPlayer = "Player";
	private string stringEnemyTag = "Enemy";
	private string stringBotName = "Bot";
	private string stringRed = "Red_";
	private string stringBlue = "Blue_";
	private string stringHostage = "Hostage";
	private string stringCivilian = "Civilian";
	private string stringZombie = "Zombie";
	void Update(){

		if (!nManager.ChatPanel.GetActive () && !nManager.SettingsPanel.GetActive ()) {
			if(kMgr.GetKeyPublic(keyType.SelfDestruct) && !guiManager.GAMEEND){
				if (pv.isMine) {
					if (CompareTag (stringPlayer)) {
						if (currentHitPoints > 0 && !hasCommittedSuicide) {
							hasCommittedSuicide = true;
							Suicide ();
						}
					}
				}
			}
		}

		if (tr.position.y < -200 && !hasCommittedSuicide) {
			hasCommittedSuicide = true;
			Suicide();
		}

		if (PhotonNetwork.isMasterClient && name.StartsWith (stringZombie)) {
			if ((int)enemyHitPoints > 0) {
				if (guiManager.hourOfDay >= 10 && guiManager.hourOfDay < 20) {
					if (!dieEnemyCoroutineStarted) {
						//Zombie currently awake, so currently isZombieAwake=true

						enumDieEnemyCoroutine = DieEnemy_Coroutine (string.Empty, true);
						StartCoroutine (enumDieEnemyCoroutine);
						//Then we kill off zombie
					}
				}
			} else {
				if (guiManager.hourOfDay < 10 || guiManager.hourOfDay >= 20) {
					if (!dieEnemyCoroutineStarted) {
						//Zombie is waking up, so currently isZombieAwake=false

						enumDieEnemyCoroutine = DieEnemy_Coroutine (string.Empty, false);
						StartCoroutine (enumDieEnemyCoroutine);
						//Then we revive the zombie
					}
				}
			}
		}



		calculateItemPickupHealth ();

		//Match Time Runs Out, RESPAWN PLAYER AND REFRESH Scoreboard Stats
		if(matchTimer.refreshKDStat == true){
			if(pv.isMine){
				if(CompareTag(stringPlayer)){
					matchTimer.refreshKDStat = false;
					scenePV.RPC ("PlayerStatsResetScore", PhotonTargets.AllBuffered, PhotonNetwork.player.name);
					DieNoStatChange ();
				}
			}
		}
	}
		
	public void resetHealthArmorResource(float hpBase, float armorBase, int classID){
		if (CompareTag (stringPlayer)) {
			if (classID == 1) {
				currentHitPoints = hpBase;
				currentArmorPoints = armorBase;
			}
			if (classID == 2) {
				currentHitPoints = hpBase;
				currentArmorPoints = armorBase;
			}
		}
	}


	IEnumerator classHealthLimiter(float HPBase, int classID){
		while (true) {
			if (CompareTag (stringPlayer)) {
				if (classID == 1 || classID == 2) {
					while (currentHitPoints >= HPBase) {
						currentHitPoints -= 1;
						if (currentHitPoints < HPBase) {
							currentHitPoints = HPBase;
						}
						yield return waitFor1Sec;
					}
				}
			}

			yield return waitFor1Sec;
		}
	}

	IEnumerator classArmorLimiter(float ArmorBase, int classID){
		while (true) {
			if (CompareTag (stringPlayer)) {
				if (classID == 1 || classID == 2) {
					while (currentArmorPoints >= ArmorBase) {
						currentArmorPoints -= 1;
						if (currentArmorPoints < ArmorBase) {
							currentArmorPoints = ArmorBase;
						}
						yield return waitFor1Sec;
					}
				}
			}
			yield return waitFor1Sec;
		}
	}




	[PunRPC]
	public void TakeDamageLava(float amt, string receiverName){


		//DEALING WITH TAKING DAMAGE GIVEN VARIABLES LIKE:
		//	1) ARMOR
		//	2) HEALTH
		//* SPLIT UP THE DAMAGE TO ARMOR AND HEALTH. AND IF NO ARMOR, THEN JUST TO HEALTH

		//Only deal the damage if the Receiver is not the Player. (In other words, no-selfdamage) - especially when using rocket projectiles, as it tends to collide with Shooter, and have unwanted affects.

		DamageReceivedCount = sManager.GetScore (teamMember.playerName, sManager.damagereceivedStat);
		DamageReceivedCount += (int)amt;
		sManager.SetScore (teamMember.playerName, sManager.damagereceivedStat, DamageReceivedCount);

		//		scenePV.RPC(changeScoreRPC,PhotonTargets.AllBuffered, PhotonNetwork.player.name, damagereceivedRPC, (int)amt);

		if(currentArmorPoints <= 0){
			currentHitPoints -= amt;
		}
		else if (currentArmorPoints > 0){
			currentArmorPoints -= (amt * 5 / 6);
			currentHitPoints -= (amt * 1 / 6);
		}

		if(currentArmorPoints < 0){
			currentHitPoints += currentArmorPoints;
			currentArmorPoints = 0;
		}

//		DoHurtingSoundFX ();

		if(currentHitPoints <= 0){
			Die(PhotonNetwork.player.NickName);

			//FOR TDM SCORING, ONLY DO SO WHEN GAME BEGINS
			if (guiManager.currentGameMode == 1) {
				if (teamMember.teamID == 1) {
					//PLAYER DYING IS BLUE TEAM, SO GIVE RED SCORE AN INCREASE OF 1
//					scenePV.RPC("rpcRedScoreDecrease",PhotonTargets.AllBuffered);

//					guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//							guiManager.RedTeamScoreProps,
//							guiManager.redTeamKills - 1
//						} };
					if (guiManager.redTeamKills > 0) {
						guiManager.TDMScoreTable [guiManager.RedTeamScoreProps] = guiManager.redTeamKills - 1;
						PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
					}
				} else if (teamMember.teamID == 2) {
					//PLAYER DYING IS RED TEAM, SO GIVE BLUE SCORE AN INCREASE OF 1
//					scenePV.RPC("rpcBlueScoreDecrease",PhotonTargets.AllBuffered);

//					guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//							guiManager.BlueTeamScoreProps,
//							guiManager.blueTeamKills - 1
//						} };
					if (guiManager.blueTeamKills > 0) {
						guiManager.TDMScoreTable [guiManager.BlueTeamScoreProps] = guiManager.blueTeamKills - 1;
						PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
					}
				}
			}
		}
	}

//	ExitGames.Client.Photon.Hashtable guiManager.TDMScoreTable;
	int tempTDMScore = 0;
	[PunRPC]
	public void TakeDamageEnemyCollision(float amt, string attackerName){

		if (scoringManager == null)
			return;
		//DEALING WITH TAKING DAMAGE GIVEN VARIABLES LIKE:
		//	1) ARMOR
		//	2) HEALTH
		//* SPLIT UP THE DAMAGE TO ARMOR AND HEALTH. AND IF NO ARMOR, THEN JUST TO HEALTH

		//Only deal the damage if the Receiver is not the Player. (In other words, no-selfdamage) - especially when using rocket projectiles, as it tends to collide with Shooter, and have unwanted affects.

		DamageReceivedCount = sManager.GetScore (teamMember.playerName, sManager.damagereceivedStat);
		DamageReceivedCount += (int)amt;
		sManager.SetScore (teamMember.playerName, sManager.damagereceivedStat, DamageReceivedCount);

//		scenePV.RPC(changeScoreRPC,PhotonTargets.AllBuffered, PhotonNetwork.player.NickName, damagereceivedRPC, (int)amt);

		if(currentArmorPoints <= 0){
			currentHitPoints -= amt;
		}
		else if (currentArmorPoints > 0){
			currentArmorPoints -= (amt * 5 / 6);
			currentHitPoints -= (amt * 1 / 6);
		}

		if(currentArmorPoints < 0){
			currentHitPoints += currentArmorPoints;
			currentArmorPoints = 0;
		}

//		DoHurtingSoundFX ();

		if(currentHitPoints <= 0){
			if (!isPlayerDead) {
				Die (attackerName);
			}

			//FOR TDM SCORING, ONLY DO SO WHEN GAME BEGINS
			if (guiManager.currentGameMode == 1) {
				if (teamMember.teamID == 1) {
					//PLAYER DYING IS BLUE TEAM, SO GIVE RED SCORE AN INCREASE OF 1
//					scenePV.RPC("rpcRedScoreIncrease",PhotonTargets.AllBuffered);
//					tempTDMScore = TDMScoreProps = guiManager.TDMScoreTable.

//						guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//							guiManager.RedTeamScoreProps,
//							guiManager.redTeamKills + 1
//						} };
					guiManager.TDMScoreTable [guiManager.RedTeamScoreProps] = guiManager.redTeamKills + 1;
					PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);


				} else if (teamMember.teamID == 2) {
					//PLAYER DYING IS RED TEAM, SO GIVE BLUE SCORE AN INCREASE OF 1
//					scenePV.RPC("rpcBlueScoreIncrease",PhotonTargets.AllBuffered);
//					guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//							guiManager.BlueTeamScoreProps,
//							guiManager.blueTeamKills + 1
//						} };
					guiManager.TDMScoreTable [guiManager.BlueTeamScoreProps] = guiManager.blueTeamKills + 1;
					PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);

				}
			}
		}
	}

	void OnEnable(){
		if (matchTimer == null)
			return;
		if (!matchTimer.isReady)
			return;
		if (!PhotonNetwork.isMasterClient)
			return;
		if (!CompareTag (stringEnemyTag))
			return;
		if ((int)enemyHitPoints <= 0) {
			GetSpawnGameObject ();
			if (BaseEnemySpawnSpots != null) {
				pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 120f, BaseEnemySpawnSpots [Random.Range (0, BaseEnemySpawnSpots.Length)].transform.position, teamMember.teamID);
			} else {
				pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 120f, tr.transform.position, teamMember.teamID);
			}
		} else {
			pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, enemyHitPoints, tr.position, teamMember.teamID);
		}
		ResetIsEnemyUnderAttackCoroutine = ResetIsEnemyUnderAttack_Coroutine ();
		StartCoroutine (ResetIsEnemyUnderAttackCoroutine);
	}

	public IEnumerator ResetIsEnemyUnderAttackCoroutine;
	public IEnumerator ResetIsEnemyUnderAttack_Coroutine(){
		while (true) {
			if (isDodgePlayerAttack)
				isDodgePlayerAttack = false;
			yield return new WaitForSeconds (5f);
		}
	}

	//Remote Procedure Call (RPC): Send messages to client when called manually. Like manual update once.
	string damagereceivedRPC = "damager";
	string damagedealtRPD = "damaged";
	[PunRPC]
	public void TakeDamage(float amt, string attackerName,int photonViewID, string tagName){


		if (pv.isMine) {
			if (CompareTag (stringPlayer)) {
				//This is my network instantiated object that is tagged stringPlayer on the unity resource model prefab.
				DoHurtingSoundFX ();
				if (playerGuiManager != null) {
					playerGuiManager.isHit = true;
				}
					
				if (attackerName != myPlayerName) {
					DamageReceivedCount = sManager.GetScore (teamMember.playerName, sManager.damagereceivedStat);
					DamageReceivedCount += (int)amt;
					sManager.SetScore (teamMember.playerName, sManager.damagereceivedStat, DamageReceivedCount);

//					scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.playerName, damagereceivedRPC, (int)amt);
				}

				if (teamMember.playerName != attackerName) {
//					scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, attackerName, damagedealtRPD, (int)amt);
				}

			}
		}

		//DEALING WITH TAKING DAMAGE GIVEN VARIABLES LIKE:
		//	1) ARMOR
		//	2) HEALTH
		//* SPLIT UP THE DAMAGE TO ARMOR AND HEALTH. AND IF NO ARMOR, THEN JUST TO HEALTH

		//Only deal the damage if the Receiver is not the Player. (In other words, no-selfdamage) - especially when using rocket projectiles, as it tends to collide with Shooter, and have unwanted affects.
		if (attackerName != myPlayerName) {
			if (currentArmorPoints <= 0) {
				currentHitPoints -= amt;
			} else if (currentArmorPoints > 0) {
				currentArmorPoints -= (amt * 5 / 6);
				currentHitPoints -= (amt * 1 / 6);
			}

			if (currentArmorPoints < 0) {
				currentHitPoints += currentArmorPoints;
				currentArmorPoints = 0;
			}
		}

		if (CompareTag (stringEnemyTag)) {
			if (PhotonNetwork.isMasterClient) {
				if ((int)enemyHitPoints > 0) {
//					if (teamMember.teamID == 3) {
//						if (enemyMovement.isHostageRescued)
//							return;
//						if (!enemyMovement.isFollowingRecruiter)
//							return;
//					}
//					scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, attackerName, damagedealtRPD, (int)amt);
//					pv.RPC ("updateEnemyHP", PhotonTargets.AllBuffered, amt, photonViewID, tagName);
					updateEnemyHP(amt, photonViewID, tagName);
					if (attackerName != name) {
						DamageReceivedCount = sManager.GetScore (name, sManager.damagereceivedStat);
						DamageReceivedCount += (int)amt;
						sManager.SetScore (name, sManager.damagereceivedStat, DamageReceivedCount);

//						scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, name, damagereceivedRPC, (int)amt);
					}

					if (!name.StartsWith (stringHostage)) {
						isDodgePlayerAttack = true;
					}

				}
			}
		}

		if (CompareTag (stringPlayer)) {
			if ((int)currentHitPoints <= 0) {
				//Pass on Attacker Name, so can record Attacker Kill for killing Player
				if (!isPlayerDead) {
					Die (attackerName);
				}
			}
		} else if (CompareTag (stringEnemyTag)) {
			if ((int)enemyHitPoints <= 0 && PhotonNetwork.isMasterClient) {	
				//Pass on Attacker Name, so can record Attacker Kill for killing Enemy
				if (!dieEnemyCoroutineStarted) {
					enumDieEnemyCoroutine = DieEnemy_Coroutine (attackerName, false);
					StartCoroutine (enumDieEnemyCoroutine);
				}
			}
		}
	}

	[PunRPC]
	public void SetPlayerDeadStatus(bool val){
		isPlayerDead = val;
	}

	public void DieNoStatChange(){
		//scenario for objects not instantiated over network.
		if(pv.instantiationId == 0){
			Destroy (gameObject);
		}
		//scenario for objects instantiated over the network.
		else{
			if(pv.isMine){

				//If this is my actual player object, then initiate the respawn process.
				if(CompareTag(stringPlayer)){
					//Disable CharacterController THEN BLOOD THEN DieFX THEN Destroy Player
					nManager.PlayerObject.GetComponent<Health>().enabled = false;
					nManager.deathCam.transform.position = tr.position;
					nManager.deathCam.transform.rotation = tr.rotation;
					nManager.deathCam.SetActive (true);
					nManager.requestFreshSpawn = true;
					pv.RPC ("SetPlayerDeadStatus", PhotonTargets.AllBuffered, true);
				}
				PhotonNetwork.Destroy(gameObject);
				//TURN OFF IN-GAME GUI STUFF
				guiManager.DisplayInGameGUI = false;
			}
		}
	}


	public void Suicide(){

		//scenario for objects not instantiated over network.
		if(pv.instantiationId == 0){
			Destroy (gameObject);
		}
		//scenario for objects instantiated over the network.
		else{
			if(pv.isMine){

				//If this is my actual player object, then initiate the respawn process.
				if(CompareTag(stringPlayer)){
					//grab Network manager object. To use the standby camera and the player respawn timer.

					//Before Dying however, we want to display Death acknowledgement. That means we must DISABLE Character controller first.
					//We're not going to make an animation. But just instantiate a tombstone that destroys in say 3 seconds.
					//And after 3 seconds, we invoke the destroy game object from Photon network. (RPC THROUGH PLAYERMOVEMENT3)

//					pv.RPC ("setLastAttackerName", PhotonTargets.AllBuffered, string.Empty);
					currentHitPoints=0f;
					currentArmorPoints=0f;
					scenePV.RPC(changeScoreRPC,PhotonTargets.AllBuffered, teamMember.playerName, scoringManager.deathsStat, 1);
//					int currentKillStreak = scoringManager.GetScore (teamMember.playerName, "killstreak");
//					int bestKillStreak = scoringManager.GetScore (teamMember.playerName, "killstreak_highest");
//					if (bestKillStreak < currentKillStreak) {
//						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, teamMember.playerName, "killstreak_highest", currentKillStreak);
//					}
//					scenePV.RPC("ResetScore",PhotonTargets.AllBuffered, teamMember.playerName, "killstreak");
//					if (guiManager.currentGameMode == 3) {
//						scenePV.RPC ("ResetScore", PhotonTargets.AllBuffered, teamMember.playerName, "garbagecollected");
//					}
						
					if (matchTimer.isReady) {


						if (guiManager.currentGameMode == 1) {							
							if (teamMember.teamID == 1) {
								//PLAYER DYING IS BLUE TEAM, SO GIVE RED SCORE AN INCREASE OF 1
//								scenePV.RPC("rpcBlueScoreDecrease",PhotonTargets.AllBuffered);
//								guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//										guiManager.BlueTeamScoreProps,
//										guiManager.blueTeamKills - 1
//									} };
								if (guiManager.blueTeamKills > 0) {
									guiManager.TDMScoreTable [guiManager.BlueTeamScoreProps] = guiManager.blueTeamKills - 1;
									PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
								}

							} else if (teamMember.teamID == 2) {
								//PLAYER DYING IS RED TEAM, SO GIVE BLUE SCORE AN INCREASE OF 1
//								scenePV.RPC("rpcRedScoreDecrease",PhotonTargets.AllBuffered);
//								guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//										guiManager.RedTeamScoreProps,
//										guiManager.redTeamKills - 1
//									} };
								if (guiManager.redTeamKills > 0) {
									guiManager.TDMScoreTable [guiManager.RedTeamScoreProps] = guiManager.redTeamKills - 1;
									PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
								}
							}				
						}
//						if (guiManager.currentGameMode == 2) {
//							scoringManagerPV.RPC ("decreaseCredits", PhotonTargets.AllBuffered);
//						}
					}

					DoBloodSplatterFX();
					DoDyingFX();

					MiniMapBlip.SetActive (false);
					PlayerHeadCollider.enabled = false;
					nManager.deathCam.transform.position = tr.position - (tr.forward * 5);
					nManager.deathCam.transform.rotation = tr.rotation;
					nManager.deathCam.SetActive (true);

					if (guiManager.currentGameMode == 2) {
						if (guiManager.currentCreditsRemaining > 0 || guiManager.currentWave < guiManager.currentWaveTarget) {
//							nManager.requestRespawnPlayer = true;
						} else {
							matchTimer.controllerCanvas.backgroundImage.SetActive (true);
							Cursor.lockState = CursorLockMode.None;
							Cursor.visible = true;
						}
					} else {
//						nManager.requestRespawnPlayer = true;
					}
					pv.RPC ("SetPlayerDeadStatus", PhotonTargets.AllBuffered, true);
//					dead = true;
					if (guiManager.currentGameMode == 2) {
						if (CompareTag (stringPlayer)) {
							if (pMovement.isRecruitingNPC) {
								pv.RPC ("SetIsRecruitingNPC", PhotonTargets.AllBuffered, string.Empty, false, false);
							}
						}
					}
				}
				if (DeathAnimCO == null) {
					DeathAnimCO = DeathAnimationAndDestroyPlayer("DEATH BY SUICIDE");
					StartCoroutine (DeathAnimCO);
				}

//				PhotonNetwork.Destroy(gameObject);
//				//TURN OFF IN-GAME GUI STUFF
//				guiManager.DisplayInGameGUI=false;

				nManager.AddChatMessage(teamMember.playerName.ToUpper() + "\tCOMMITED SUICIDE");
			}
		}
	}

	public Coroutine enemyModelActivationCoroutine;
	public IEnumerator ActivateEnemyModel_Coroutine(bool val){

		//The purpose of this is to have the Enemy Game Object De-activate (In 2 Seconds).
		//Upon de-activation (After 2 seconds), we reset the state of Animation to 'Alive' - So...
		//The initial state on Respawn would be 'Alive' rather than, having to see that animation transition from 'Dead to Alive' when they respawn.

		if (!val) {
			yield return new WaitForSeconds (2f);
			if (name.StartsWith (stringHostage)) {
				HostageIndicator.SetActive (val);
			}
		}
		if (teamMember.EnemyModel) {
			teamMember.EnemyModel.SetActive (val);
			if (name.StartsWith (stringHostage)) {
				HostageIndicator.SetActive (val);
			}
		}
		yield return null;
	}


	[PunRPC]
	public void SyncEnemyHP(float amt, Vector3 newPos, int teamID){
		enemyHitPoints = amt;
		if (amt <= 0) {

			if (name.StartsWith (stringHostage)) {
				HostageIndicator.GetComponent<MeshRenderer> ().material = HostageIndicatorMaterials [0];
			}
			//If dead, then reset shooterID (It's for 'attacking its threat)
			shooterPhotonID = -1;
			shooterTag = string.Empty;
			if (agent.enabled)
				agent.isStopped = true;
			agent.velocity = Vector3.zero;
			agent.enabled = false;
			enemyMovement.canShoot = false;
			playerAnimator.SetBool ("Dead", true);
			MiniMapBlip.SetActive (false);

			if (teamMember.teamID != 3) {
				if (enemyMovement) {
					if (enemyMovement.subscribedEnemySpawnTrigger) {
						if (enemyMovement.subscribedEnemySpawnTrigger.triggerType == EnemySpawnTrigger.TriggerType.AreaTrigger) {
							if (enemyMovement.subscribedEnemySpawnTrigger.initHostage [0] != null) {
								if (enemyMovement.subscribedEnemySpawnTrigger.initHostage [0].GetComponent<EnemyMovement> ().isFollowingRecruiter) {
									if (enemyMovement.subscribedEnemySpawnTrigger.currentKills < enemyMovement.subscribedEnemySpawnTrigger.killThreshold) {
//										enemyMovement.subscribedEnemySpawnTrigger.GetComponent<PhotonView> ().RPC ("SetEnemySpawnTriggerKills", PhotonTargets.AllBuffered, true, 1);
									}
								}
							}
						}
//						Debug.Log ("Enemy dead: " + enemyMovement.subscribedEnemySpawnTrigger.currentKills);
					}
				}
			} else if (teamMember.teamID == 3) {
//				enemyMovement.subscribedEnemySpawnTrigger.GetComponent<PhotonView> ().RPC ("SetEnemySpawnTriggerKills", PhotonTargets.AllBuffered, false, 0);
			}

			if (enemyModelActivationCoroutine == null) {
				enemyModelActivationCoroutine = StartCoroutine (ActivateEnemyModel_Coroutine (false));
			} else {
//				Debug.Log ("Enemy Respawn Model Activation Coroutine Conflict Situation DEAD");
				StopCoroutine (enemyModelActivationCoroutine);
				enemyModelActivationCoroutine = StartCoroutine (ActivateEnemyModel_Coroutine (false));
			}

			capsuleCollider.enabled = false;
			headCapsuleCollider.enabled = false;
			rBody.detectCollisions = false;
		} else {

			if (name.StartsWith (stringHostage)) {
				HostageIndicator.GetComponent<MeshRenderer> ().material = HostageIndicatorMaterials [1];
			}
			tr.position = newPos;

			agent.enabled = true;
			if (agent.isOnNavMesh) {
				agent.isStopped = false;
			}
			enemyMovement.canShoot = true;
			playerAnimator.SetBool ("Dead", false);
			MiniMapBlip.SetActive (true);

//			StartCoroutine (ActivateEnemyModel_Coroutine (true));

			if (enemyModelActivationCoroutine == null) {
				enemyModelActivationCoroutine = StartCoroutine (ActivateEnemyModel_Coroutine (true));
			} else {
//				Debug.Log ("Enemy Respawn Model Activation Coroutine Conflict Situation ALIVE");
				StopCoroutine (enemyModelActivationCoroutine);
				enemyModelActivationCoroutine = StartCoroutine (ActivateEnemyModel_Coroutine (true));
			}


			capsuleCollider.enabled = true;
			headCapsuleCollider.enabled = true;
			rBody.detectCollisions = true;

			enemyMovement.ThreatTarget = null;
			enemyMovement.SpookedTarget = null;

			if (teamMember.teamID == 3) {
				enemyMovement.isFollowingRecruiter = false;
			}

		}


	}

	string SpawnSpotBase0Enemy = "SpawnSpotBase0Enemy";
	string SpawnSpotBase1Enemy = "SpawnSpotBase1Enemy";
	string SpawnSpotBase2Enemy = "SpawnSpotBase2Enemy";
	string SpawnSpotBase3Enemy = "SpawnSpotBase3Enemy";
	string SpawnSpotBase4Enemy = "SpawnSpotBase4Enemy";

	void GetSpawnGameObject(){

		if (BaseEnemySpawnSpots != null)
			return;

		if (enemyMovement.baseNumber == 0)
			BaseEnemySpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase0Enemy);
		else if (enemyMovement.baseNumber == 1)
			BaseEnemySpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase1Enemy);
		else if (enemyMovement.baseNumber == 2)
			BaseEnemySpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase2Enemy);
		else if (enemyMovement.baseNumber == 3)
			BaseEnemySpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase3Enemy);										
		else if (enemyMovement.baseNumber == 4)
			BaseEnemySpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase4Enemy);
	}

	string SpawnSpotBase0Hostage = "SpawnSpotBase0Hostage";
	string SpawnSpotBase1Hostage = "SpawnSpotBase1Hostage";
	string SpawnSpotBase2Hostage = "SpawnSpotBase2Hostage";
	string SpawnSpotBase3Hostage = "SpawnSpotBase3Hostage";
	string SpawnSpotBase4Hostage = "SpawnSpotBase4Hostage";

	void GetHostageSpawnGameObject(){

		if (BaseHostageSpawnSpots != null)
			return;		

		if (enemyMovement.baseNumber == 0)
			BaseHostageSpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase0Hostage);
		else if (enemyMovement.baseNumber == 1)
			BaseHostageSpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase1Hostage);
		else if (enemyMovement.baseNumber == 2)
			BaseHostageSpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase2Hostage);
		else if (enemyMovement.baseNumber == 3)
			BaseHostageSpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase3Hostage);										
		else if (enemyMovement.baseNumber == 4)
			BaseHostageSpawnSpots = GameObject.FindGameObjectsWithTag (SpawnSpotBase4Hostage);		
	}

//	void GetNodeGameObject(){
//		if (BaseEnemyNodes != null)
//			return;
//
//		if (enemyMovement.baseNumber == 0)
//			BaseEnemyNodes = GameObject.FindGameObjectsWithTag ("Base0Node");
//		else if (enemyMovement.baseNumber == 1)
//			BaseEnemyNodes = GameObject.FindGameObjectsWithTag ("Base1Node");
//		else if (enemyMovement.baseNumber == 2)
//			BaseEnemyNodes = GameObject.FindGameObjectsWithTag ("Base2Node");
//		else if (enemyMovement.baseNumber == 3)
//			BaseEnemyNodes = GameObject.FindGameObjectsWithTag ("Base3Node");										
//		else if (enemyMovement.baseNumber == 4)
//			BaseEnemyNodes = GameObject.FindGameObjectsWithTag ("Base4Node");
//	}

	public bool dieEnemyCoroutineStarted = false;
	public IEnumerator enumDieEnemyCoroutine;
	public IEnumerator DieEnemy_Coroutine(string attackerPlayerName, bool isZombieAwake){
		//scenario for objects not instantiated over network.
		if (pv.instantiationId == 0) {
			//Destroy (gameObject);
		}
		//DESTROYING ENEMY OBJECT SHOULD BE HANDLED AS MUCH AS POSSIBLE BY MASTERCLIENT TO AVOID MULTIPLE CALLS.
		else {
			if (CompareTag (stringEnemyTag)) {
				dieEnemyCoroutineStarted = true;
				if (matchTimer.isReady) {

					if (teamMember.playerName == attackerPlayerName) {
						SteamUserStats.GetAchievement ("FIRST_KILL", out steamCheck_FirstKillAchievement);
						if (!steamCheck_FirstKillAchievement) {
							SteamUserStats.SetAchievement ("FIRST_KILL");
							SteamUserStats.StoreStats ();
						}
					}

					if (PhotonNetwork.isMasterClient) {

						if (attackerPlayerName != string.Empty) {
							if (guiManager.currentGameMode == 1) {
								if (!(attackerPlayerName.StartsWith (stringBotName) || attackerPlayerName.StartsWith (stringZombie))) {
									if (teamMember.teamID == 1) {
										//PLAYER DYING IS BLUE TEAM, SO GIVE RED SCORE AN INCREASE OF 1
//										scenePV.RPC ("rpcRedScoreIncrease", PhotonTargets.AllBuffered);
//										guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//												guiManager.RedTeamScoreProps,
//												guiManager.redTeamKills + 1
//											} };
										guiManager.TDMScoreTable [guiManager.RedTeamScoreProps] = guiManager.redTeamKills + 1;
										PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);


//										scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, attackerPlayerName, "tdmpoints", 1);

									} else if (teamMember.teamID == 2) {
										//PLAYER DYING IS RED TEAM, SO GIVE BLUE SCORE AN INCREASE OF 1
//										scenePV.RPC ("rpcBlueScoreIncrease", PhotonTargets.AllBuffered);
//										guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//												guiManager.BlueTeamScoreProps,
//												guiManager.blueTeamKills + 1
//											} };
										guiManager.TDMScoreTable [guiManager.BlueTeamScoreProps] = guiManager.blueTeamKills + 1;
										PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);


//										scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, attackerPlayerName, "tdmpoints", 1);
									}
								} else {
									if (teamMember.teamID == 1) {
//										scenePV.RPC ("rpcBlueScoreDecrease", PhotonTargets.AllBuffered);
//										guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//												guiManager.BlueTeamScoreProps,
//												guiManager.blueTeamKills - 1
//											} };
										if (guiManager.blueTeamKills > 0) {
											guiManager.TDMScoreTable [guiManager.BlueTeamScoreProps] = guiManager.blueTeamKills - 1;
											PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
										}
									} else if (teamMember.teamID == 2) {
//										scenePV.RPC ("rpcRedScoreDecrease", PhotonTargets.AllBuffered);
//										guiManager.TDMScoreTable = new ExitGames.Client.Photon.Hashtable () { {
//												guiManager.RedTeamScoreProps,
//												guiManager.redTeamKills - 1
//											} };
										if (guiManager.redTeamKills > 0) {
											guiManager.TDMScoreTable [guiManager.RedTeamScoreProps] = guiManager.redTeamKills - 1;
											PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
										}
									}
								}
							}

							//Store Dead Player Name and Killer Name
							scenePV.RPC (UpdateStatsForKillerRPC, PhotonTargets.AllBuffered, attackerPlayerName, 1, gameObject.name);
							scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, gameObject.name, scoringManager.deathsStat, 1);
						}
					}
				}
				if (PhotonNetwork.isMasterClient) {
					if ((guiManager.hourOfDay < 10 || guiManager.hourOfDay >= 20) && name.StartsWith (stringZombie)) {
//						if (isZombieAwake) {
							DoBloodSplatterFX ();
							DoDyingFX ();
//						}
					}else {
						DoBloodSplatterFX ();
						DoDyingFX ();
					}
				}
			}

			if (PhotonNetwork.isMasterClient) {
				if (guiManager.currentGameMode == 1 || guiManager.currentGameMode == 2) {
					//MISSION
					if (name.StartsWith (stringZombie)) {
						enemyMovement.hasZombieDamagedPlayer = false;
						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, BaseEnemySpawnSpots [Random.Range (0, BaseEnemySpawnSpots.Length)].transform.position, teamMember.teamID);
					} else if (name.StartsWith (stringBotName)) {
						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, BaseEnemySpawnSpots [Random.Range (0, BaseEnemySpawnSpots.Length)].transform.position, teamMember.teamID);
					} else if (name.StartsWith (stringHostage)) {

						if (enemyMovement.isFollowingRecruiter) {
							pv.RPC ("SetIsFollowingPlayer", PhotonTargets.AllBuffered, false, -9);
						}
						if (enemyMovement.isHostageRescued) {
							scenePV.RPC ("IncreaseHostagesRemaining", PhotonTargets.AllBuffered);
							enemyMovement.isHostageRescued = false;
						}

						if (enemyMovement.baseNumber == 1) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, Base1HostageSpawnSpots [Random.Range (0, Base1HostageSpawnSpots.Length)].transform.position, teamMember.teamID);
						} else if (enemyMovement.baseNumber == 2) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, Base2HostageSpawnSpots [Random.Range (0, Base2HostageSpawnSpots.Length)].transform.position, teamMember.teamID);
						} else if (enemyMovement.baseNumber == 3) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, Base3HostageSpawnSpots [Random.Range (0, Base3HostageSpawnSpots.Length)].transform.position, teamMember.teamID);
						} else if (enemyMovement.baseNumber == 4) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, Base4HostageSpawnSpots [Random.Range (0, Base4HostageSpawnSpots.Length)].transform.position, teamMember.teamID);
						}
					} else if (name.StartsWith (stringCivilian)) {
						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, CivilianSpawnSpots [Random.Range (0, CivilianSpawnSpots.Length)].transform.position, teamMember.teamID);
					}

					//TDM
					if (teamMember.teamID == 1) {
						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, BluePlayerSpawnSpots [Random.Range (0, BluePlayerSpawnSpots.Length)].transform.position, teamMember.teamID);
					} else if(teamMember.teamID == 2) {
						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, RedPlayerSpawnSpots [Random.Range (0, RedPlayerSpawnSpots.Length)].transform.position, teamMember.teamID);
					}



				} else {
					//TDM MULTIPLAYER
//					if (teamMember.teamID == 1) {
//						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, BluePlayerSpawnSpots [Random.Range (0, BluePlayerSpawnSpots.Length)].transform.position, teamMember.teamID);
//					} else if(teamMember.teamID == 2) {
//						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, RedPlayerSpawnSpots [Random.Range (0, RedPlayerSpawnSpots.Length)].transform.position, teamMember.teamID);
//					}

					if (name.StartsWith (stringZombie)) {
						enemyMovement.hasZombieDamagedPlayer = false;
						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, BaseEnemySpawnSpots [Random.Range (0, BaseEnemySpawnSpots.Length)].transform.position, teamMember.teamID);
					} else if (name.StartsWith (stringBotName)) {
						if (teamMember.teamID == 1) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, BluePlayerSpawnSpots [Random.Range (0, BluePlayerSpawnSpots.Length)].transform.position, teamMember.teamID);
						} else if (teamMember.teamID == 2) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, BluePlayerSpawnSpots [Random.Range (0, BluePlayerSpawnSpots.Length)].transform.position, teamMember.teamID);
						} else {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, BaseEnemySpawnSpots [Random.Range (0, BaseEnemySpawnSpots.Length)].transform.position, teamMember.teamID);
						}
					} else if (name.StartsWith (stringHostage)) {
						if (enemyMovement.baseNumber == 1) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, Base1HostageSpawnSpots [Random.Range (0, Base1HostageSpawnSpots.Length)].transform.position, teamMember.teamID);
						} else if (enemyMovement.baseNumber == 2) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, Base2HostageSpawnSpots [Random.Range (0, Base2HostageSpawnSpots.Length)].transform.position, teamMember.teamID);
						} else if (enemyMovement.baseNumber == 3) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, Base3HostageSpawnSpots [Random.Range (0, Base3HostageSpawnSpots.Length)].transform.position, teamMember.teamID);
						} else if (enemyMovement.baseNumber == 4) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, Base4HostageSpawnSpots [Random.Range (0, Base4HostageSpawnSpots.Length)].transform.position, teamMember.teamID);
						}
					} else if (name.StartsWith (stringCivilian)) {
						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, 0f, CivilianSpawnSpots [Random.Range (0, CivilianSpawnSpots.Length)].transform.position, teamMember.teamID);
					}


				}
				enemyMovement.ThreatTarget = null;
				enemyMovement.SpookedTarget = null;
				if (guiManager.currentGameMode == 2 && enemyMovement.subscribedEnemySpawnTrigger.killThresholdMet || 1==1) {
					yield return new WaitForSeconds (8f);
				} else {
					yield return waitFor4Sec;
				}

				if (guiManager.currentGameMode == 1 || guiManager.currentGameMode == 2) {
					if ((guiManager.hourOfDay < 10 || guiManager.hourOfDay >= 20) && name.StartsWith (stringZombie)) {

						GetSpawnGameObject ();

						enemyHitPoints = (120f + 120 * (enemyMovement.enemyScale - 1) / 2);
						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, enemyHitPoints, BaseEnemySpawnSpots [Random.Range (0, BaseEnemySpawnSpots.Length)].transform.position, teamMember.teamID);
					} else if (name.StartsWith (stringCivilian)) {
						enemyHitPoints = 120f;
						pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, enemyHitPoints, CivilianSpawnSpots [Random.Range (0, CivilianSpawnSpots.Length)].transform.position, teamMember.teamID);
					} else if (name.StartsWith (stringBotName) || name.StartsWith(stringBlue) || name.StartsWith(stringRed)) {
						enemyHitPoints = 120f;
						BaseEnemySpawnSpots = null;

						GetSpawnGameObject ();
						if (BaseEnemySpawnSpots != null) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, enemyHitPoints, BaseEnemySpawnSpots [Random.Range (0, BaseEnemySpawnSpots.Length)].transform.position, teamMember.teamID);
						}
					} else if (name.StartsWith (stringHostage)) {
						enemyHitPoints = 400;
						BaseEnemySpawnSpots = null;

						GetHostageSpawnGameObject ();
						if (BaseHostageSpawnSpots != null) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, enemyHitPoints, BaseHostageSpawnSpots [Random.Range (0, BaseHostageSpawnSpots.Length)].transform.position, teamMember.teamID);
						}
					}
				} else {
					if (name.StartsWith (stringZombie)) {
						if ((guiManager.hourOfDay < 10 || guiManager.hourOfDay >= 20)) {
							enemyHitPoints = (120f + 120 * (enemyMovement.enemyScale - 1) / 2);

							GetSpawnGameObject ();

							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, enemyHitPoints, BaseEnemySpawnSpots [Random.Range (0, BaseEnemySpawnSpots.Length)].transform.position, teamMember.teamID);
						}
					} else if (name.StartsWith (stringBotName)) {
						enemyHitPoints = 120f;
						BaseEnemySpawnSpots = null;

						GetSpawnGameObject ();

						if (BaseEnemySpawnSpots != null) {
							pv.RPC (syncEnemyHPRPC, PhotonTargets.AllBuffered, enemyHitPoints, BaseEnemySpawnSpots [Random.Range (0, BaseEnemySpawnSpots.Length)].transform.position, teamMember.teamID);
						}
					}
				}
			}
		}
		dieEnemyCoroutineStarted = false;
		yield return true;
	}		


	void Die(string attackerPlayerName){
		if (pv.instantiationId == 0) {
			Destroy (gameObject);
		} else {
			if (pv.isMine) {

				//If this is my actual player object, then initiate the respawn process.
				if (CompareTag (stringPlayer)) {

					scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.playerName, scoringManager.deathsStat, 1);

					if (teamMember.playerName == attackerPlayerName) {
						SteamUserStats.GetAchievement ("FIRST_KILL", out steamCheck_FirstKillAchievement);
						if (!steamCheck_FirstKillAchievement) {
							SteamUserStats.SetAchievement ("FIRST_KILL");
							Debug.Log ("Awarded player with the First Kill Achievement Test");
							SteamUserStats.StoreStats ();
						}
					}

					if (teamMember.playerName != attackerPlayerName) {
						//FOR TDM SCORING, ONLY DO SO WHEN GAME BEGINS
						if (matchTimer.isReady) {
							if (guiManager.currentGameMode == 1) {
								if (!(attackerPlayerName.StartsWith (stringBlue) || attackerPlayerName.StartsWith (stringZombie))) {
									if (teamMember.teamID == 1) {
										//PLAYER DYING IS BLUE TEAM, SO GIVE BLUE SCORE AN INCREASE OF 1
										guiManager.TDMScoreTable [guiManager.RedTeamScoreProps] = guiManager.redTeamKills + 1;
										PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
									} else if (teamMember.teamID == 2) {
										//PLAYER DYING IS RED TEAM, SO GIVE BLUE SCORE AN INCREASE OF 1
										guiManager.TDMScoreTable [guiManager.BlueTeamScoreProps] = guiManager.blueTeamKills + 1;
										PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
									}
								} else {
									if (teamMember.teamID == 1) {
										if (guiManager.blueTeamKills > 0) {
											guiManager.TDMScoreTable [guiManager.BlueTeamScoreProps] = guiManager.blueTeamKills - 1;
											PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
										}
									} else if (teamMember.teamID == 2) {
										if (guiManager.redTeamKills > 0) {
											guiManager.TDMScoreTable [guiManager.RedTeamScoreProps] = guiManager.redTeamKills - 1;
											PhotonNetwork.room.SetCustomProperties (guiManager.TDMScoreTable);
										}
									}
								}
							}

						}
						//1) Update Dead Player Name and Killer Name
						//2) Update Kills by 1
						scenePV.RPC (UpdateStatsForKillerRPC, PhotonTargets.AllBuffered, attackerPlayerName, 1, teamMember.GetPlayerName ());
					}

					DoBloodSplatterFX ();
					DoDyingFX ();

					MiniMapBlip.SetActive (false);
					PlayerHeadCollider.enabled = false;
					nManager.deathCam.transform.position = tr.position - (tr.forward * 5);
					nManager.deathCam.transform.rotation = tr.rotation;
					nManager.deathCam.SetActive (true);
					pv.RPC ("SetPlayerDeadStatus", PhotonTargets.AllBuffered, true);
					if (guiManager.currentGameMode == 2) {
						if (CompareTag (stringPlayer)) {
							if (pMovement.isRecruitingNPC) {
								pv.RPC ("SetIsRecruitingNPC", PhotonTargets.AllBuffered, string.Empty, false, false);
							}
						}
					}

					if (teamMember.playerName != attackerPlayerName) {

						if (DeathAnimCO == null) {
							DeathAnimCO = DeathAnimationAndDestroyPlayer ("FRAGGED BY " + attackerPlayerName.ToUpper());
							StartCoroutine (DeathAnimCO);
						}
					}

				}
				if (CompareTag (stringPlayer)) {
					nManager.AddChatMessage (PhotonNetwork.player.NickName.ToUpper() + "\tWAS FRAGGED BY\t" + attackerPlayerName.ToUpper());
				}
			}
		}
	}

	Ray ray;
	RaycastHit groundHit;
	Transform groundHitTransform;
	int nonItemLayerMask = ~(1 << 10);	//Do not hit itemLayer
	IEnumerator DeathAnimCO;
	IEnumerator DeathAnimationAndDestroyPlayer(string deathMessage){
		myCamera.SetActive (false);
		smr.enabled = true;
		pv.RPC ("FootStepAudioSourceEnabler", PhotonTargets.All, false);
		StartCoroutine (MoveDeathCamForPlayer ());
		guiManager.DisplayInGameGUI = false;
		pv.RPC ("PlayerDeathAnimation", PhotonTargets.All, true);
		guiManager.DeathMsgCO = guiManager.DeathMsg_CO (deathMessage);
		StartCoroutine (guiManager.DeathMsgCO);
		DeadPlayerColliderCO = DeadPlayerCollider_CO ();
		StartCoroutine (DeadPlayerColliderCO);

		yield return waitFor1Sec;
		yield return waitFor1Sec;

		nManager.requestRespawnPlayer = true;
		PhotonNetwork.Destroy (gameObject);
		DeathAnimCO = null;
	}

	IEnumerator DeadPlayerColliderCO;
	IEnumerator DeadPlayerCollider_CO(){
		while (guiManager.DeathMsgCO != null) {
			if (pMovement.IsGrounded) {
			}
			pMovement.ApplyFriction (0.2f);
			yield return true;
		}

	}

	IEnumerator MoveDeathCamForPlayer(){
		while (true) {
			nManager.deathCam.transform.position = tr.position - (tr.forward * 5) + (tr.up);
			yield return new WaitForEndOfFrame ();
		}
	}

	[PunRPC]
	void PlayerDeathAnimation(bool val){
		playerAnimator.SetBool ("Dead", val);
	}

	void DoHurtingSoundFX(){
		//Local
		fxManager.HurtingFx (soundSourceTr.position);
	}

	string DyingFxRPC = "DyingFx";
	void DoDyingFX(){
		scenePV.RPC(DyingFxRPC, PhotonTargets.All, soundSourceTr.position);
	}

	//*********************************************************************************************************************
	//TRIGGER ITEM STUFF//
	//*********************************************************************************************************************
	//Additional Functions

	[PunRPC]
	public void setItemPickupPlayerNameHealth(string str, string myItemName){
		itemPickupPlayerName = str;
		itemPickUpName = myItemName;
	}

	void calculateItemPickupHealth(){
		if (itemPickupPlayerName.Equals (teamMember.GetPlayerName())) {
//			Debug.Log ("Player picked up: " + itemPickupPlayerName + ", my Name: " + teamMember.GetPlayerName ());
			//Do all the calculation based on itemPickupName
			if (itemPickUpName.StartsWith(strHealthItem)){
				
				if (CompareTag (stringPlayer)) {
					scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), scoringManager.pickup_healthStat, 1);
				} else if (CompareTag (stringEnemyTag)) {
					scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, gameObject.name, scoringManager.pickup_healthStat, 1);
				}

				if (itemPickUpName.Equals (strHealth10)) {
					currentHitPoints = classStatGainBehavior (currentHitPoints, 10, 200, teamMember.classID);
				} else if (itemPickUpName.Equals (strHealth25)) {
					currentHitPoints = classStatGainBehavior (currentHitPoints, 25, 200, teamMember.classID);
				} else if (itemPickUpName.Equals (strHealth50)) {
					currentHitPoints = classStatGainBehavior (currentHitPoints, 50, 200, teamMember.classID);
				} else if (itemPickUpName.Equals (strHealth100)) {
					currentHitPoints = classStatGainBehavior (currentHitPoints, 100, 200, teamMember.classID);
				}

			}
			if (itemPickUpName.StartsWith(strArmorItem)){

				if (CompareTag (stringPlayer)) {
					scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), scoringManager.pickup_armorStat, 1);
				} else if (CompareTag (stringEnemyTag)) {
					scenePV.RPC (changeScoreRPC, PhotonTargets.AllBuffered, gameObject.name, scoringManager.pickup_armorStat, 1);
				}

				if (itemPickUpName.Equals (strArmor10)) {
					currentArmorPoints = classStatGainBehavior (currentArmorPoints, 10, 200, teamMember.classID);
				} else if (itemPickUpName.Equals (strArmor25)) {
					currentArmorPoints = classStatGainBehavior (currentArmorPoints, 25, 200, teamMember.classID);
				} else if (itemPickUpName.Equals (strArmor50)) {
					currentArmorPoints = classStatGainBehavior (currentArmorPoints, 50, 200, teamMember.classID);
				} else if (itemPickUpName.Equals (strArmor100)) {
					currentArmorPoints = classStatGainBehavior (currentArmorPoints, 100, 200, teamMember.classID);
				}
			}
			itemPickupPlayerName = string.Empty;
			itemPickUpName = string.Empty;
		}
	}

	IEnumerator SaveViewRoomListPref_Coroutine(){
		PlayerPrefs.SetInt ("ViewRoomList", 1);
		PlayerPrefs.Save ();
		Debug.Log ("View Room List Player Pref saved: " + PlayerPrefs.GetInt ("ViewRoomList"));
		nManager.SceneLoadingPanel.SetActive (true);
		yield return waitFor1Sec;
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);

		yield return true;
	}

	IEnumerator SaveCampaignGamePlayerPrefAndJoin_Coroutine(){
		PlayerPrefs.SetInt ("BeginChallengeFromHomeBase", 1);
		PlayerPrefs.Save ();
		Debug.Log ("Begin Challenge Player Pref saved: " + PlayerPrefs.GetInt ("BeginChallengeFromHomeBase"));
		nManager.SceneLoadingPanel.SetActive (true);
		yield return waitFor1Sec;
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);

		yield return true;
	}

	string strHealthItem = "Health_Item";
	string strHealth10 = "Health_Item10";
	string strHealth25 = "Health_Item25";
	string strHealth50 = "Health_Item50";
	string strHealth100 = "Health_Item100";
	string strArmorItem = "Armor_Item";
	string strArmor10 = "Armor_Item10";
	string strArmor25 = "Armor_Item25";
	string strArmor50 = "Armor_Item50";
	string strArmor100 = "Armor_Item100";

	string PickupItemRPC = "PickupItem";
	string setItemPickupPlayerNameHealthRPC = "setItemPickupPlayerNameHealth";

	void OnTriggerEnter(Collider other){

		//*********************************************************************************************************************
		//HEALTH / ARMOR / GARBAGE ITEM / JET RESOURCES//
		//*********************************************************************************************************************
		string itemName = other.gameObject.name;


//		if (!PhotonNetwork.isMasterClient)
//			return;
		if (CompareTag (stringEnemyTag))
			return;

		if (itemName.StartsWith(strHealthItem) || itemName.StartsWith(strArmorItem)) {
			pv.RPC (setItemPickupPlayerNameHealthRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), itemName);
			if (itemPickupPlayerName != string.Empty && itemPickUpName != string.Empty) {
				other.GetComponentInParent<PhotonView> ().RPC (PickupItemRPC, PhotonTargets.AllBuffered, itemName);
			}
		}
	}


	private float classStatGainBehavior(float currentStatValue, float statGain, float statCap, int classID){
		currentStatValue += statGain;
		if (currentStatValue >= statCap) {
			currentStatValue = statCap;
		}
		return currentStatValue;
	}


	//RPC Set of functions for Tombstone
//	[PunRPC]
//	void  BloodSplatterFX (Vector3 DeathPosition){
//		GameObject tFX = (GameObject)Instantiate (BloodSplatterPrefab, DeathPosition, Quaternion.identity);
//	}

	string BloodFXRPC = "BloodFX";
	void DoBloodSplatterFX(){
		scenePV.RPC(BloodFXRPC, PhotonTargets.All, tr.position);
	}


	public GameObject getNearestTaggedObject(string stringTag){
		var nearestDistanceSqr = Mathf.Infinity;
		GameObject nearestObj = null;
		GameObject[] ObjectList = null;
		ObjectList = GameObject.FindGameObjectsWithTag (stringTag);
		foreach (var obj in ObjectList) {	
			float distanceSqr = (obj.transform.position - tr.position).sqrMagnitude;
			if (distanceSqr < nearestDistanceSqr) {
				nearestObj = obj;
				nearestDistanceSqr = distanceSqr;
			}
		}
		return nearestObj;
	}

	public float getDistanceFromObject(Transform player, GameObject obj){
		float distanceSqr = (obj.transform.position - player.position).sqrMagnitude;
		return distanceSqr;
	}		

	public bool GetDead(){
		return isPlayerDead;
	}

	public string getMyPlayerName(){
		return myPlayerName;
	}

}