using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnTrigger : MonoBehaviour {

	public enum TriggerType{
		AmbushTrigger,
		AreaTrigger
	}

	WaitForSeconds waitFor10 = new WaitForSeconds (10f);
	WaitForSeconds waitFor1 = new WaitForSeconds (1f);
	WaitForSeconds waitFor0_5 = new WaitForSeconds (0.5f);
	WaitForSeconds waitFor0_1 = new WaitForSeconds (0.1f);

	NetworkManager nManager;
	MatchTimer mTimer;
	GuiManager guiManager;
	ScoringManager scoringManager;
	PhotonView scenePV;
	EnemyMovement hostageMovement;
	public TriggerType triggerType = TriggerType.AmbushTrigger;
	public GameObject [] EnemyGameObjectPrefab;
	public GameObject [] ZombieGameObjectPrefab;
	public GameObject HostageGameObjectPrefab;
	public bool killThresholdMet = false;
	public int killThreshold = 5;
	public int currentKills = 0;
	public int BotCount = 7;
	public int ZombieCount = 5;
	public int BaseTeamID = 0;
	public bool[] isEnemyOutofRange = new bool[10];
	public int totalBotIndex = 0;
	public int totalMissionBotIndex = 0;
	public int totalZombieIndex = 0;
	public int totalRedBotIndex = 0;
	public int totalBlueBotIndex = 0;

	public int botSpawnDist = 0;

	public GameObject[] allTriggerBots = new GameObject[50];
	public GameObject[] triggerEnemies = new GameObject[10];
	public GameObject[] initHostage = new GameObject[1];
	public GameObject[] triggerZombies = new GameObject[10];

	public GameObject[] blueEnemies = new GameObject[15];
	public GameObject[] redEnemies = new GameObject[15];

	private GameObject[] playerList;
	private bool allPlayersDead = false;

	GameObject[] EnemySpawnSpots;
	[HideInInspector] public GameObject[] BlueTeamSpawnSpots;
	[HideInInspector] public GameObject[] RedTeamSpawnSpots;
	GameObject[] HostageSpawnSpot;
	public bool hasSpawnedTriggerEnemies = false;

	TeamMember teamMember;
	public int baseNumber = 0;
	public string baseName = "";

	public GameObject triggerObject;
	public GameObject playerObject;

	private string playerTag = "Player";
	private string enemyTag = "Enemy";

	[PunRPC]
	public void SetEnemySpawnTriggerKills(bool isIncrement, int val){
		if (isIncrement) {
			currentKills += val;
		} else {
			currentKills = val;
		}
	}

	void Start () {
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		scenePV = nManager.GetComponent<PhotonView> ();
		mTimer = nManager.GetComponent<MatchTimer> ();
		guiManager = nManager.GetComponent<GuiManager> ();
		scoringManager = nManager.GetComponent<ScoringManager> ();

		if (name.Equals ("BASE0")) {
			baseNumber = 0;
			baseName = "Base" + baseNumber;

			EnemySpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBase0Enemy");

		} else if (name.Equals ("BASE1")) {
			baseNumber = 1;
			baseName = "Base" + baseNumber;

			EnemySpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBase1Enemy");
			HostageSpawnSpot = GameObject.FindGameObjectsWithTag ("SpawnSpotBase1Hostage");


		} else if (name.Equals ("BASE2")) {
			baseNumber = 2;
			baseName = "Base" + baseNumber;

			EnemySpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBase2Enemy");
			HostageSpawnSpot = GameObject.FindGameObjectsWithTag ("SpawnSpotBase2Hostage");


		} else if (name.Equals ("BASE3")) {
			baseNumber = 3;
			baseName = "Base" + baseNumber;

			EnemySpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBase3Enemy");
			HostageSpawnSpot = GameObject.FindGameObjectsWithTag ("SpawnSpotBase3Hostage");


		} else if (name.Equals ("BASE4")) {
			baseNumber = 4;
			baseName = "Base" + baseNumber;

			EnemySpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBase4Enemy");
			HostageSpawnSpot = GameObject.FindGameObjectsWithTag ("SpawnSpotBase4Hostage");


		} else {
			EnemySpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotEnemy");
		}

		BlueTeamSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotBlue");
		RedTeamSpawnSpots = GameObject.FindGameObjectsWithTag ("SpawnSpotRed");

		if (PhotonNetwork.isMasterClient) {

			if (EnemyResourceManagement_CO == null) {
				EnemyResourceManagement_CO = EnemyResourceManagement_Coroutine ();
				StartCoroutine (EnemyResourceManagement_CO);
			}
		}
	}

	public bool hasAbandonedSurvivor = false;

//	string 
	void FixedUpdate(){

		if (!PhotonNetwork.isMasterClient)
			return;
		if (!mTimer.isReady)
			return;
		if (!(mTimer.SecondsUntilItsTime > 0))
			return;
		if (guiManager.GAMEEND)
			return;

		if (guiManager.currentGameMode == 1) {
			SpawningBehaviorUpdate ();
//			ReinstantiateBehaviorUpdate ();
		} else if (guiManager.currentGameMode == 2) {

//			//UI FOR KILLTHRESHOLD MET
//			if (baseNumber != 0 && hostageMovement != null) {
//				if (currentKills >= killThreshold || 1 == 1) {
//					if (!killThresholdMet) {
//						killThresholdMet = true;
//						if (hostageMovement.isFollowingRecruiter) {
//							if (guiManager.NpcMsgCO != null) {
//								StopCoroutine (guiManager.NpcMsgCO);
//							}
//							guiManager.NpcMsgCO = guiManager.NpcMsg_CO (2, "TARGET LOCATED", "RETURN TO BASE");
//							StartCoroutine (guiManager.NpcMsgCO);
//						}
//
//					}
//
//				} else {
//					killThresholdMet = false;
//				}
//			}


			if (triggerType == (int)TriggerType.AmbushTrigger) {
				SpawningBehaviorUpdate ();
			} else {
				//LOCATE AND THE SURVIVOR FIRST. THE SURVIVOR WILL NOT MOVE WHILE THE WAVE OF ENEMIES ARE ALIVE.
				//SO WE SPAWN THE TRIGGER ENEMIES NOW. PERHAPS DO THIS IN WAVES? -- GAMEMANAGER KEEPS THIS INFO
				SpawningBehaviorUpdate ();
//				ReinstantiateBehaviorUpdate ();
			}
		}

		if (playerList != null) {
			foreach (GameObject p in playerList) {
				allPlayersDead = true;
				if (p) {
					if (!p.GetComponent<Health> ().isPlayerDead) {
						allPlayersDead = false;
						break;
					}
				}
			}
		} else {
			allPlayersDead = false;
		}

		if (allPlayersDead) {
			if (currentKills > 0) {

				if (PhotonNetwork.isMasterClient) {
					if (currentKills < killThreshold) {
						//						scenePV.RPC ("SetEnemySpawnTriggerKills", PhotonTargets.AllBuffered, false, 0);
					}
				}
			}

			if (initHostage [0] != null) {
				if (initHostage [0].GetComponent<EnemyMovement> ().isFollowingRecruiter) {
					if (!hasAbandonedSurvivor) {
						//						if (guiManager.NpcMsgAbandonCO != null) {
						//							StopCoroutine (guiManager.NpcMsgAbandonCO);
						//						}
						hasAbandonedSurvivor = true;
						//						guiManager.NpcMsgAbandonCO = guiManager.NpcMsgAbandon_CO ("YOU HAVE ABANDONED " + initHostage [0].name.ToUpper ());
						//						StartCoroutine (guiManager.NpcMsgAbandonCO);
					}
				}
			}
		}


	}



	void OnTriggerEnter(Collider other){

		if (!PhotonNetwork.isMasterClient)
			return;
		if (!mTimer.isReady)
			return;
		if (!(mTimer.SecondsUntilItsTime > 0))
			return;
		if (guiManager.GAMEEND)
			return;

		if (other.CompareTag (playerTag)) {
			playerObject = other.gameObject;
		}
		if (other.CompareTag (enemyTag)) {
			triggerObject = other.gameObject;
		}

	}

	void OnTriggerStay(Collider other){
		if (!PhotonNetwork.isMasterClient)
			return;
		if (!mTimer.isReady)
			return;
		if (!(mTimer.SecondsUntilItsTime > 0))
			return;
		if (guiManager.GAMEEND)
			return;

		if (other.CompareTag (playerTag)) {
			playerObject = other.gameObject;
		}
		if (other.CompareTag (enemyTag)) {
			triggerObject = other.gameObject;
		}
	}

	void OnTriggerExit(Collider other){

		if (!PhotonNetwork.isMasterClient)
			return;
		if (!mTimer.isReady)
			return;
		if (!(mTimer.SecondsUntilItsTime > 0))
			return;
		if (guiManager.GAMEEND)
			return;

		if (other.CompareTag (playerTag)) {
			playerObject = null;
		}
		if (other.CompareTag (enemyTag)) {
			triggerObject = null;
		}
	}

	void SpawningBehaviorUpdate(){
		if (!hasSpawnedTriggerEnemies) {
			hasSpawnedTriggerEnemies = true;
			TriggerEnemyNPCSpawnsCO = TriggerEnemyNPCSpawns_CO (baseName);
			StartCoroutine (TriggerEnemyNPCSpawnsCO);
			InitZombieNPCSpawnsCO = InitZombieNPCSpawns_CO (baseName);
			StartCoroutine (InitZombieNPCSpawnsCO);
			if (baseNumber != 0) {
				InitHostageNPCSpawnsCO = InitHostageNPCSpawns_CO (baseName);
				StartCoroutine (InitHostageNPCSpawnsCO);
			}
		}

	}		

	GameObject tempbot;
	void ReinstantiateBehaviorUpdate(){
		if (Input.GetKeyDown (KeyCode.J)) {
			hasSpawnedTriggerEnemies = false;
			for (int i = 0; i <= totalBotIndex; i++) {
				tempbot = allTriggerBots [i];
				if (tempbot) {
					if (tempbot.GetComponent<PhotonView> ()) {
						scoringManager.RemovePlayer (tempbot.name);
						PhotonNetwork.Destroy (tempbot);
					}
				}
			}
			totalBotIndex = 0;
			totalZombieIndex = 0;
		}
	}


	int tempCount = 0;
	static int currBlueBots = 0;
	static int currRedBots = 0;

	public IEnumerator TriggerEnemyNPCSpawnsCO;
	public IEnumerator TriggerEnemyNPCSpawns_CO(string baseNum){
		if (guiManager.currentGameMode == 2) {
			for (int i = 0; i < BotCount; i++) {
				if (triggerEnemies [i] == null) {
					yield return waitFor0_1;
					GameObject enemy;
					if (PhotonNetwork.isMasterClient) {
						int RandNum = Random.Range (0, EnemySpawnSpots.Length);
						GameObject botSpawnSpot = EnemySpawnSpots [RandNum];
						enemy = (GameObject)PhotonNetwork.InstantiateSceneObject (EnemyGameObjectPrefab [0].name, botSpawnSpot.transform.position, Quaternion.identity, 0, null);

						PhotonView enemyPV = enemy.GetComponent<PhotonView> ();

						enemy.GetComponent<EnemyMovement> ().hasSpawned = true;
						enemy.name = "Bot_" + enemyPV.viewID;
						if (enemy.name == "Bot_" + enemyPV.viewID) {
							enemyPV.RPC ("synchronizeRenamedEnemyName", PhotonTargets.AllBuffered, enemy.GetComponent<PhotonView> ().name);
						}

						float randomEnemySize = Random.Range (2, 10);
						enemyPV.RPC ("SetEnemyMovementBaseNumber", PhotonTargets.AllBuffered, baseNumber);
						enemyPV.RPC ("SetTeamID", PhotonTargets.AllBuffered, 4);

						triggerEnemies [i] = enemy;
						allTriggerBots [totalBotIndex] = enemy;
						totalBotIndex++;

						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, enemy.GetComponent<PhotonView> ().name, "killstreak", 0);
						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, enemy.GetComponent<PhotonView> ().name, "team", 4);
					}
				}
			}
		}
		if (guiManager.currentGameMode == 1 && BaseTeamID == 0) {
			int halfBotCount = Mathf.FloorToInt (BotCount);
			for (int i = 0; i < halfBotCount; i++) {
				if (triggerEnemies [i] == null) {
					yield return waitFor0_1;
					GameObject enemy;
					if (PhotonNetwork.isMasterClient) {
						int RandNum = Random.Range (0, EnemySpawnSpots.Length);
						GameObject botSpawnSpot = EnemySpawnSpots [RandNum];
						enemy = (GameObject)PhotonNetwork.InstantiateSceneObject (EnemyGameObjectPrefab [0].name, botSpawnSpot.transform.position, Quaternion.identity, 0, null);

						PhotonView enemyPV = enemy.GetComponent<PhotonView> ();

						enemy.GetComponent<EnemyMovement> ().hasSpawned = true;
						enemy.name = "Bot_" + enemyPV.viewID;
						if (enemy.name == "Bot_" + enemyPV.viewID) {
							enemyPV.RPC ("synchronizeRenamedEnemyName", PhotonTargets.AllBuffered, enemy.GetComponent<PhotonView> ().name);
						}
						triggerEnemies [i] = enemy;
						allTriggerBots [totalBotIndex] = enemy;
						totalBotIndex++;

						enemyPV.RPC ("SetEnemyMovementBaseNumber", PhotonTargets.AllBuffered, baseNumber);
						enemyPV.RPC ("SetTeamID", PhotonTargets.AllBuffered, 4);
						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, enemy.GetComponent<PhotonView> ().name, "killstreak", 0);
						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, enemy.GetComponent<PhotonView> ().name, "team", 4);
					}
				}
			}
		}
		if (guiManager.currentGameMode == 1) {

			if (SteamManager.Initialized) {
				if (PhotonNetwork.room.MaxPlayers == 1 || Steamworks.SteamFriends.GetPersonaState () == Steamworks.EPersonaState.k_EPersonaStateOffline) {									
					if (nManager.blueBotCount < 2)
						tempCount = 2;
					else
						tempCount = nManager.blueBotCount;
				} else {
					tempCount = nManager.blueBotCount;
				}
			}
			for (int i = 0; i < tempCount; i++) {

				if (blueEnemies [i] == null) {
					yield return waitFor0_1;
					GameObject blueEnemy;
					if (PhotonNetwork.isMasterClient) {
						int RandNum = Random.Range (0, BlueTeamSpawnSpots.Length);
						GameObject botSpawnSpot = BlueTeamSpawnSpots [RandNum];
						blueEnemy = (GameObject)PhotonNetwork.InstantiateSceneObject (EnemyGameObjectPrefab [1].name, botSpawnSpot.transform.position, Quaternion.identity, 0, null);

						PhotonView enemyPV = blueEnemy.GetComponent<PhotonView> ();

						blueEnemy.GetComponent<EnemyMovement> ().hasSpawned = true;
						blueEnemy.GetComponent<PhotonView> ().name = "Blue_" + enemyPV.viewID;
						if (blueEnemy.GetComponent<PhotonView> ().name == "Blue_" + enemyPV.viewID) {
							blueEnemy.GetComponent<PhotonView> ().RPC ("synchronizeRenamedEnemyName", PhotonTargets.AllBuffered, blueEnemy.GetComponent<PhotonView> ().name);
						}
						blueEnemies [i] = blueEnemy;
						allTriggerBots [totalBotIndex] = blueEnemy;
						totalBotIndex++;

						enemyPV.RPC ("SetEnemyMovementBaseNumber", PhotonTargets.AllBuffered, baseNumber);
						enemyPV.RPC ("SetTeamID", PhotonTargets.AllBuffered, 1);
						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, blueEnemy.GetComponent<PhotonView> ().name, "killstreak", 0);
						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, blueEnemy.GetComponent<PhotonView> ().name, "team", 1);
					}
				}

			}

			if (SteamManager.Initialized) {
				if (PhotonNetwork.room.MaxPlayers == 1 || Steamworks.SteamFriends.GetPersonaState () == Steamworks.EPersonaState.k_EPersonaStateOffline) {									
					if (nManager.redBotCount < 2)
						tempCount = 2;
					else
						tempCount = nManager.redBotCount;
				} else {
					tempCount = nManager.redBotCount;
				}
			}

			for (int i = 0; i < tempCount; i++) {
				if (redEnemies [i] == null) {
					yield return waitFor0_1;
					GameObject redEnemy;
					if (PhotonNetwork.isMasterClient) {
						int RandNum = Random.Range (0, RedTeamSpawnSpots.Length);
						GameObject botSpawnSpot = RedTeamSpawnSpots [RandNum];
						redEnemy = (GameObject)PhotonNetwork.InstantiateSceneObject (EnemyGameObjectPrefab [2].name, botSpawnSpot.transform.position, Quaternion.identity, 0, null);

						PhotonView enemyPV = redEnemy.GetComponent<PhotonView> ();

						redEnemy.GetComponent<EnemyMovement> ().hasSpawned = true;
						redEnemy.GetComponent<PhotonView> ().name = "Red_" + enemyPV.viewID;
						if (redEnemy.GetComponent<PhotonView> ().name == "Red_" + enemyPV.viewID) {
							redEnemy.GetComponent<PhotonView> ().RPC ("synchronizeRenamedEnemyName", PhotonTargets.AllBuffered, redEnemy.GetComponent<PhotonView> ().name);
						}
						redEnemies [i] = redEnemy;
						allTriggerBots [totalBotIndex] = redEnemy;
						totalBotIndex++;

						enemyPV.RPC ("SetEnemyMovementBaseNumber", PhotonTargets.AllBuffered, baseNumber);
						enemyPV.RPC ("SetTeamID", PhotonTargets.AllBuffered, 2);
						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, redEnemy.GetComponent<PhotonView> ().name, "killstreak", 0);
						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, redEnemy.GetComponent<PhotonView> ().name, "team", 2);
					}
				}

			}
				
		}
		yield return null;
		TriggerEnemyNPCSpawnsCO = null;
	}

	IEnumerator InitZombieNPCSpawnsCO;
	IEnumerator InitZombieNPCSpawns_CO(string baseNum){
		int zombieCount = ZombieCount;
		if (guiManager.currentGameMode == 2) {
			for (int i = 0; i < zombieCount; i++) {
				if (triggerZombies [i] == null) {
					yield return waitFor0_1;
					GameObject zombie;
					if (PhotonNetwork.isMasterClient) {
						int RandNum = Random.Range (0, EnemySpawnSpots.Length);		
						GameObject zombieSpawnSpot = EnemySpawnSpots [RandNum];

						zombie = (GameObject)PhotonNetwork.InstantiateSceneObject (ZombieGameObjectPrefab [0].name, zombieSpawnSpot.transform.position, Quaternion.identity, 0, null);

						PhotonView enemyPV = zombie.GetComponent<PhotonView> ();

						zombie.GetComponent<EnemyMovement> ().hasSpawned = true;
						zombie.GetComponent<PhotonView> ().name = "Zombie_" + enemyPV.viewID;
						if (zombie.GetComponent<PhotonView> ().name == "Zombie_" + enemyPV.viewID) {
							zombie.GetComponent<PhotonView> ().RPC ("synchronizeRenamedEnemyName", PhotonTargets.AllBuffered, zombie.GetComponent<PhotonView> ().name);
						}
						triggerZombies [i] = zombie;
						allTriggerBots [totalBotIndex] = zombie;
						totalZombieIndex++;
						totalBotIndex++;

						enemyPV.RPC ("SetEnemyMovementBaseNumber", PhotonTargets.AllBuffered, baseNumber);
						enemyPV.RPC ("SetTeamID", PhotonTargets.AllBuffered, 4);
						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, zombie.GetComponent<PhotonView> ().name, "team", 4);
					}
				}
			}
		}
		yield return null;
		InitZombieNPCSpawnsCO = null;
	}

	IEnumerator InitHostageNPCSpawnsCO;
	IEnumerator InitHostageNPCSpawns_CO(string baseNum){
		int hostageCount = 1;
		if (guiManager.currentGameMode == 2) {
			for (int i = 0; i < hostageCount; i++) {
				if (initHostage [i] == null) {
					yield return waitFor0_1;
					GameObject hostage;
					if (PhotonNetwork.isMasterClient) {
						int RandNum = Random.Range (0, HostageSpawnSpot.Length);		
						GameObject hostageSpawnSpot = HostageSpawnSpot [RandNum];

						hostage = (GameObject)PhotonNetwork.InstantiateSceneObject (HostageGameObjectPrefab.name, hostageSpawnSpot.transform.position, Quaternion.identity, 0, null);

						PhotonView enemyPV = hostage.GetComponent<PhotonView> ();

						hostage.GetComponent<EnemyMovement> ().hasSpawned = true;
						hostage.GetComponent<PhotonView> ().name = "Hostage_" + enemyPV.viewID;
						if (hostage.GetComponent<PhotonView> ().name == "Hostage_" + baseNum + "_" + enemyPV.viewID) {
							hostage.GetComponent<PhotonView> ().RPC ("synchronizeRenamedEnemyName", PhotonTargets.AllBuffered, hostage.GetComponent<PhotonView> ().name);
						}
						initHostage [i] = hostage;
						allTriggerBots [totalBotIndex] = hostage;
						totalBotIndex++;

						hostageMovement = initHostage [i].GetComponent<EnemyMovement> ();
						enemyPV.RPC ("SetEnemyMovementBaseNumber", PhotonTargets.AllBuffered, baseNumber);
						enemyPV.RPC ("SetTeamID", PhotonTargets.AllBuffered, 3);
						scenePV.RPC ("SetScore", PhotonTargets.AllBuffered, hostage.GetComponent<PhotonView> ().name, "team", 3);
					}
				}
			}
		}
		yield return null;
		InitHostageNPCSpawnsCO = null;
	}


	void UpdatePlayerList(){
		playerList = GameObject.FindGameObjectsWithTag (playerTag);
	}

	int sanityCheckIndex = 0;
	bool outOfRange = true;

	float distance = 0f;
	float shortestDist = 0f;

	int enemyIndex=0;
	EnemyMovement eMovement;
	Health eHealth;
	IEnumerator EnemyResourceManagement_CO;
	IEnumerator EnemyResourceManagement_Coroutine(){
		while (true) {
			if (mTimer.isReady) {
				if (PhotonNetwork.isMasterClient) {
					UpdatePlayerList ();
					distance = 0f;
					shortestDist = 0f;
					enemyIndex = 0;
					foreach (GameObject enemy in allTriggerBots) {

						eMovement = null;
						eHealth = null;
						shortestDist = Mathf.Infinity;

						if (enemy != null) {

							foreach (GameObject player in playerList) {
								distance = Vector3.Distance (player.transform.position, enemy.transform.position);
								if (shortestDist >= distance)
									shortestDist = distance;								
							}

							if (shortestDist > nManager.BotSpawnDistance) {
								if (enemy.GetActive ()) {
									//									GetComponent<PhotonView> ().RPC (StopNPCCoroutinesString, PhotonTargets.AllBuffered, enemy.GetComponent<PhotonView>().viewID);
									StopNPCCoroutines (enemy);
								}
							} else {
								if (!enemy.GetActive ()) {
									enemy.SetActive (true);
									//									GetComponent<PhotonView>().RPC(ActivateNPCString,PhotonTargets.AllBuffered,enemyIndex);
								}
								enemy.GetComponent<NavMeshAgent> ().speed = nManager.BotSpeed;
							}
						}

						enemyIndex++;
					}

					foreach (GameObject hostage in initHostage) {
						foreach (GameObject player in playerList) {
							if (player != null && hostage != null) {

								distance = Vector3.Distance (player.transform.position, hostage.transform.position);
								if (distance > nManager.BotSpawnDistance) {
									if (hostage.GetActive ()) {
										//										StopNPCCoroutines (hostage);	
									}
								} else {
									if (!hostage.GetActive ()) {
										//										hostage.SetActive (true);
									}
									hostage.GetComponent<NavMeshAgent> ().speed = nManager.BotSpeed;
								}

							}
						}
					}


				}
			}
			yield return waitFor0_5;
		}
	}


	string ActivateNPCString = "ActivateNPC";
	[PunRPC]
	void ActivateNPC(int triggerEnemyIndex){
		if (PhotonNetwork.isMasterClient)
			allTriggerBots [triggerEnemyIndex].SetActive (true);
	}

	GameObject npc;

	string StopNPCCoroutinesString = "StopNPCCoroutines";
	[PunRPC]
	//	void StopNPCCoroutines(int photonViewID){
	void StopNPCCoroutines(GameObject npc){		
		if (!PhotonNetwork.isMasterClient)
			return;

		//		foreach (GameObject enemy in allTriggerBots) {
		//			if (enemy.GetComponent<PhotonView> ().viewID == photonViewID) {
		//				npc = enemy;
		//				break;
		//			}
		//		}

		eHealth = npc.GetComponent<Health> ();
		eMovement = npc.GetComponent<EnemyMovement> ();

		if (eHealth.enumDieEnemyCoroutine != null) {											
			StopCoroutine (eHealth.enumDieEnemyCoroutine);
			eHealth.dieEnemyCoroutineStarted = false;
		}
		if (eHealth.ResetIsEnemyUnderAttackCoroutine != null) {
			StopCoroutine (eHealth.ResetIsEnemyUnderAttackCoroutine);
		}
		if (eHealth.enemyModelActivationCoroutine != null)
			StopCoroutine (eHealth.enemyModelActivationCoroutine);
		if (eMovement.EnemyBehaviorCoroutine != null)
			eMovement.StopCoroutine (eMovement.EnemyBehaviorCoroutine);
		if (eMovement.HostageBehaviorCoroutine != null)
			eMovement.StopCoroutine (eMovement.HostageBehaviorCoroutine);
		if (eMovement.CivilianBehaviorCoroutine != null)
			eMovement.StopCoroutine (eMovement.CivilianBehaviorCoroutine);
		if (eMovement.ZombieDamageCoroutine != null)
			eMovement.StopCoroutine (eMovement.ZombieDamageCoroutine);
		if (eMovement.ThreatTarget != null)
			eMovement.ThreatTarget = null;
		if (eMovement.SpookedTarget != null)
			eMovement.SpookedTarget = null;
		if (eMovement.ObjectTarget != null)
			eMovement.ObjectTarget = null;

		eHealth.enumDieEnemyCoroutine = null;
		eHealth.enemyModelActivationCoroutine = null;
		eHealth.ResetIsEnemyUnderAttackCoroutine = null;

		eMovement.EnemyBehaviorCoroutine = null;
		eMovement.HostageBehaviorCoroutine = null;
		eMovement.CivilianBehaviorCoroutine = null;
		eMovement.ZombieDamageCoroutine = null;
		npc.SetActive (false);

	}

}
