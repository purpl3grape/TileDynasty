using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum enumEnemyType{
	TeamEnemy,
	ZombieEnemy,
	NeutralEnemy,
}

public class EnemyMovement : MonoBehaviour {

	WaitForSeconds waitfor0_1;
	WaitForSeconds waitfor0_25;
	WaitForSeconds waitfor0_5;
	WaitForSeconds waitforRandom05_06;
	WaitForSeconds waitfor1;
	WaitForSeconds waitforRandom1_2;
	WaitForSeconds waitforRandom01_02;
	WaitForSeconds waitfor3;
	WaitForSeconds waitfor5;


	//Public Variables
	public string targetTag = "Player";
	public GameObject BaseNodeTarget;
	public GameObject ObjectTarget;
	public GameObject ThreatTarget;
	public GameObject SpookedTarget;
	public GameObject RescueTarget;
	public GameObject lastRescueTarget;
	public GameObject CivilianTarget;

	public Vector3 destinationPosition;

	//TileDynasty Classes
	private FXManager fxManager;
	private NetworkManager nManager;
	private GuiManager guiManager;
	private ScoringManager scoringManager;
	private MatchTimer mTimer;
	private HostageResueZoneTrigger hostageRescueZoneTrigger;
	private Transform tr;
	private Health health;
	public EnemySpawnTrigger subscribedEnemySpawnTrigger;
	//Private variables
	private Vector3 aimDirection = Vector3.zero;
	private Vector3 lookAtPos = Vector3.zero;
	private Vector3 randomPos = Vector3.zero;
	private GameObject[] civilianNodeList = null;
	private GameObject[] nodeList = null;

	private GameObject[] base0List = null;
	private GameObject[] base1List = null;
	private GameObject[] base2List = null;
	private GameObject[] base3List = null;
	private GameObject[] base4List = null;
	public bool isEmpEffected = false;
	public bool isSppoked = false;
	public float gunSpookDistance=300f;
	public float movementSpookDistance=150f;
	public float threatViewDistance=150f;
	public int EnemyType = 0;
	public int baseNumber = 0;
	public int enemyScale = 2;
	public GameObject[] enemySpawnTriggerList;

	GameObject[] noisyPlayerList;

	[HideInInspector] public PhotonView enemyPhotonView;
	PhotonView scenePhotonView;

	TeamMember teamMember;
	int teamID = -999;
	NavMeshAgent agent;
	Rigidbody rbody;
	Animator anim;
	string hostageTag = "Hostage";
	string civilianTag = "Civilian";
	string enemyTag = "Enemy";
	string playerTag = "Player";
	string levelTag = "Level";
	string tagHostageRescueZone = "HostageRescueZone";
	string tagSceneScripts = "SceneScripts";
	string tagBase = "Base";

	void Start () {

		waitfor0_1 = new WaitForSeconds(0.1f);
		waitfor0_25 = new WaitForSeconds(0.25f);
		waitfor0_5 = new WaitForSeconds(0.5f);
		waitforRandom05_06 = new WaitForSeconds(Random.Range (0.5f, 0.6f));
		waitfor1 = new WaitForSeconds(1f);
		waitforRandom1_2 = new WaitForSeconds(Random.Range (1f, 2f));
		waitforRandom01_02 = new WaitForSeconds (Random.Range (0.1f, 0.2f));
		waitfor3 = new WaitForSeconds(3f);
		waitfor5 = new WaitForSeconds(5f);

		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		enemyPhotonView = GetComponent<PhotonView> ();
		teamMember = GetComponent<TeamMember> ();
		hostageRescueZoneTrigger = GameObject.FindGameObjectWithTag (tagHostageRescueZone).GetComponent<HostageResueZoneTrigger> ();
		fxManager = GameObject.FindGameObjectWithTag (tagSceneScripts).GetComponent<FXManager> ();
		scenePhotonView = fxManager.GetComponent<PhotonView> ();
		nManager = fxManager.GetComponent<NetworkManager> ();
		guiManager = fxManager.GetComponent<GuiManager> ();
		scoringManager = fxManager.GetComponent<ScoringManager> ();
		mTimer = fxManager.GetComponent<MatchTimer> ();
		tr = GetComponent<Transform> ();
		rbody = GetComponent<Rigidbody> ();
		rbody.isKinematic = true;
		health = GetComponent<Health> ();

		enemySpawnTriggerList = GameObject.FindGameObjectsWithTag (tagBase);
		SetSubscribedSpawnTrigger (baseNumber);


		if (guiManager.currentGameMode == 1 || guiManager.currentGameMode == 2) {
			nodeList = InitBaseNodes (9999, nodeList);
			civilianNodeList =  InitBaseNodes (9998, civilianNodeList);
			base0List = InitBaseNodes (0, base0List);
			base1List = InitBaseNodes (1, base1List);
			base2List =  InitBaseNodes (2, base2List);
			base3List =  InitBaseNodes (3, base3List);
			base4List =  InitBaseNodes (4, base4List);
			if (teamMember.teamID != 3 && teamMember.teamID != 5) {
				EnemyBehaviorCoroutine = StartCoroutine (EnemyBehavior_Coroutine ());
			} else if (teamMember.teamID == 3) {
				HostageBehaviorCoroutine = StartCoroutine (HostageBehavior_Coroutine ());
			} else if (teamMember.teamID == 5) {
				CivilianBehaviorCoroutine = StartCoroutine (CivilianBehavior_Coroutine ());
			}
		}
	}


	public bool hasSpawned = false;

	void OnEnable(){
		if (hasSpawned) {
			waitfor0_1 = new WaitForSeconds (0.1f);
			waitfor0_25 = new WaitForSeconds (0.25f);
			waitfor0_5 = new WaitForSeconds (0.5f);
			waitforRandom05_06 = new WaitForSeconds (Random.Range (0.5f, 0.6f));
			waitfor1 = new WaitForSeconds (1f);
			waitforRandom1_2 = new WaitForSeconds (Random.Range (1f, 2f));
			waitforRandom01_02 = new WaitForSeconds (Random.Range (0.1f, 0.2f));
			waitfor3 = new WaitForSeconds (3f);
			waitfor5 = new WaitForSeconds (5f);

			isNoObstacleToPlayer1 = false;
			isNoObstacleToPlayer2 = false;
			isNoObstacleToPlayer3 = false;

			agent = GetComponent<NavMeshAgent> ();
			anim = GetComponent<Animator> ();
			enemyPhotonView = GetComponent<PhotonView> ();
			teamMember = GetComponent<TeamMember> ();
			hostageRescueZoneTrigger = GameObject.FindGameObjectWithTag (tagHostageRescueZone).GetComponent<HostageResueZoneTrigger> ();
			fxManager = GameObject.FindGameObjectWithTag (tagSceneScripts).GetComponent<FXManager> ();
			scenePhotonView = fxManager.GetComponent<PhotonView> ();
			nManager = fxManager.GetComponent<NetworkManager> ();
			guiManager = fxManager.GetComponent<GuiManager> ();
			scoringManager = fxManager.GetComponent<ScoringManager> ();
			mTimer = fxManager.GetComponent<MatchTimer> ();
			tr = GetComponent<Transform> ();
			rbody = GetComponent<Rigidbody> ();
			rbody.isKinematic = true;
			health = GetComponent<Health> ();
			if (guiManager.currentGameMode == 1 || guiManager.currentGameMode == 2) {
				nodeList = InitBaseNodes (9999, nodeList);
				civilianNodeList =  InitBaseNodes (9998, civilianNodeList);
				base0List = InitBaseNodes (0, base0List);
				base1List = InitBaseNodes (1, base1List);
				base2List =  InitBaseNodes (2, base2List);
				base3List =  InitBaseNodes (3, base3List);
				base4List =  InitBaseNodes (4, base4List);
				if (teamMember.teamID != 3 && teamMember.teamID != 5) {
					EnemyBehaviorCoroutine = StartCoroutine (EnemyBehavior_Coroutine ());
				} else if (teamMember.teamID == 3) {
					HostageBehaviorCoroutine = StartCoroutine (HostageBehavior_Coroutine ());
				} else if (teamMember.teamID == 5) {
					CivilianBehaviorCoroutine = StartCoroutine (CivilianBehavior_Coroutine ());
				}
			}
		}
	}

	[PunRPC]
	public void ModifyTransformSize(int size){
		tr.localScale = new Vector3 (size, size, size);
	}

	public Coroutine EMPEffectCoroutine;
	public IEnumerator EMPEffect_Coroutine(){
		while (true) {
			if (isEmpEffected) {
				yield return waitfor3;
				enemyPhotonView.RPC ("EMPEffected_Enemy", PhotonTargets.All, false);
			}
			yield return waitfor0_1;
		}
	}

	[PunRPC]
	public void EMPEffected_Enemy(bool val){
		isEmpEffected = val;
		if (val) {
			//			agent.speed = 1f;
		} else {
			if (EnemyType == (int)enumEnemyType.ZombieEnemy) {
				agent.speed = nManager.BotSpeed;;
			} else {
				agent.speed = nManager.BotSpeed;
			}
		}
	}

	int ResetPathIntervalSpooked=0;
	int ResetPathIntervalThreat=0;
	[PunRPC]
	public void ResetPathRPC(){
		agent.ResetPath ();
	}

	[PunRPC]
	public void StoppingDistanceRPC(float val){
		agent.stoppingDistance = val;
	}

	[PunRPC]
	public void UpdateEnemyDestination(Vector3 pos){
		destinationPosition = pos;
	}


	[PunRPC]
	public void SetIsFollowingPlayer(bool isFollowingPlayer, int playerPhotonViewId){
		isFollowingRecruiter = isFollowingPlayer;
		rescuerPhotonViewId = playerPhotonViewId;
	}

	Vector3 distanceToRescuePlayer;
	public bool isFollowingRecruiter = false;
	public int rescuerPhotonViewId = -999;
	string RescuerPlayerName = string.Empty;
	public Coroutine HostageBehaviorCoroutine;
	public IEnumerator HostageBehavior_Coroutine(){
		while (true) {
			if (PhotonNetwork.isMasterClient) {
				if ((mTimer.SecondsUntilItsTime <= 0) || guiManager.GAMEEND || (guiManager.currentGameMode == 2 && guiManager.currentHostagesRemaining <= 0)) {
					if (agent.enabled) {
						agent.isStopped = true;
					}
				}

				if (mTimer.SecondsUntilItsTime > 0 && !guiManager.GAMEEND) {

					if (isFollowingRecruiter && !isHostageRescued) {
						RescueTarget = getClosestRescuePlayerObject (out RescuerPlayerName);
						if (RescueTarget) {
							if (RescueTarget.GetActive ()) {
								if (agent.enabled == true) {
									if (agent.isStopped) {
										agent.isStopped = false;
									}
									lastRescueTarget = RescueTarget;
									if (subscribedEnemySpawnTrigger) {
										if (subscribedEnemySpawnTrigger.currentKills >= subscribedEnemySpawnTrigger.killThreshold || 1==1) {
											agent.SetDestination (RescueTarget.transform.position);
											if (agent.stoppingDistance != 1f) {
												StoppingDistanceRPC (1f);
											}
										}
									}
								}
							}

						}
					}else if(isHostageRescued){
						agent.SetDestination (hostageRescueZoneTrigger.transform.position);
					}
					else {
						if (lastRescueTarget) {
							lastRescueTarget = null;
							if (agent.enabled == true) {
								agent.isStopped = true;
								agent.velocity = Vector3.zero;
							}
						}
					}
				}
			}

			yield return waitfor0_25;
		}
	}





	float distToThreat = 0f;
	int shootIntervalCount = 0;

	RaycastHit hit1;
	RaycastHit hit2;
	RaycastHit hit3;
	Ray ray1;
	Ray ray2;
	Ray ray3;
	Transform hitTransform2;
	Transform hitTransform3;
	public bool isNoObstacleToPlayer1 = false;
	public bool isNoObstacleToPlayer2 = false;
	public bool isNoObstacleToPlayer3 = false;
	public bool canShoot = true;
	public Coroutine EnemyBehaviorCoroutine;
	public IEnumerator EnemyBehavior_Coroutine(){
		ray1 = new Ray (tr.position, tr.forward);
		while (true) {
			if (PhotonNetwork.isMasterClient) {

				if ((mTimer.SecondsUntilItsTime <= 0) || guiManager.GAMEEND || (guiManager.currentGameMode == 2 && guiManager.currentHostagesRemaining <= 0)) {
					if (agent.enabled) {
						agent.isStopped = true;
					}
				}

				if (mTimer.SecondsUntilItsTime > 0 && !guiManager.GAMEEND) {
					//GAME IS RUNNING

					SetEnemyTimeOfDayResponsiveness ();

					if (EnemyType == (int)enumEnemyType.TeamEnemy) {
						//The primary object target is 'Closest enemy teammember' instead of nodes

						threatViewDistance *= 5;
						gunSpookDistance *= 5;
						movementSpookDistance *= 5;

						ObjectTarget = getClosestPlayerOrEnemyObject (out distToThreat);
						BaseNodeTarget = getRandomBaseNode (false);
					} else if (EnemyType == (int)enumEnemyType.NeutralEnemy) {						
						ObjectTarget = getRandomBaseNode (true);
						BaseNodeTarget = ObjectTarget;
					} else if (EnemyType == (int)enumEnemyType.ZombieEnemy) {
						if (hostageRescueZoneTrigger.hostageCount > 0) {
							if (Random.Range (0, 100) < 50) {
								ObjectTarget = getRandomHostagePlayer ();
							} else {
								ObjectTarget = getRandomBaseNode (true);
							}
							BaseNodeTarget = ObjectTarget;
						} else {
							ObjectTarget = getRandomBaseNode (true);
							BaseNodeTarget = ObjectTarget;
						}
					}

					GameObject tempObj;
					if (EnemyType == (int)enumEnemyType.ZombieEnemy) {
						tempObj = getClosestPlayerOrEnemyObject (out distToThreat);
						ThreatTarget = IsEnemyTooFarFromBase () == true ? null : tempObj;
						tempObj = getPlayerWhoIsMakingNoise ();
						SpookedTarget = IsEnemyTooFarFromBase () == true ? null : tempObj;

					} else if (EnemyType == (int)enumEnemyType.NeutralEnemy) {
						tempObj = getClosestPlayerOrEnemyObject (out distToThreat);
						ThreatTarget = IsEnemyTooFarFromBase () == true ? null : tempObj;
						tempObj = getPlayerWhoIsMakingNoise ();
						SpookedTarget = IsEnemyTooFarFromBase () == true ? null : tempObj;

					} else if (EnemyType == (int)enumEnemyType.TeamEnemy) {
						//Not Tied to their respective bases as they spawn at any Team Spawn Location
						tempObj = getClosestPlayerOrEnemyObject (out distToThreat);
						ThreatTarget = tempObj;
						SpookedTarget = tempObj;
					}

					//					distToThreat = ThreatTarget != null ? Vector3.Distance (ThreatTarget.transform.position, tr.position) : Mathf.Infinity;


					//RAY TO THREAT TARGET
					if (distToThreat < threatViewDistance) {

					}

					//PRIORITY: 1) SPOOKED, 2) THREAT IN SIGHT 3) RANDOM PATROLING

					////////////////////////////////////////////////////////////////////////////////////////////
					////////////////////////////////////////////////////////////////////////////////////////////
					//SPOOKED ENEMY BEHAVIOR SECTION SPOOKED ENEMY BEHAVIOR SECTION SPOOKED ENEMY BEHAVIOR SECTION
					////////////////////////////////////////////////////////////////////////////////////////////
					////////////////////////////////////////////////////////////////////////////////////////////

					if (SpookedTarget) {

						ray3 = new Ray (tr.position, SpookedTarget.transform.position - tr.position);
						isNoObstacleToPlayer3 = false;					

						if (SpookedTarget.CompareTag (enemyTag) || SpookedTarget.CompareTag (playerTag)) {
							if (Physics.Raycast (ray3, out hit3, gunSpookDistance)) {
								//Adding this if statement
								if (hit3.transform.CompareTag (enemyTag) || hit3.transform.CompareTag (playerTag)) {
									if (hit3.transform.GetComponent<TeamMember> ().teamID != GetComponent<TeamMember> ().teamID) {
										isNoObstacleToPlayer3 = true;
									}
								}
							}
						}

						if (agent.enabled) {
							if (ResetPathIntervalSpooked > 1) {
								ResetPathIntervalSpooked = 0;
								SetEnemyDestination (SpookedTarget);
							} else {
								ResetPathIntervalSpooked += 1;
							}
						}


						if (SpookedTarget.GetActive ()) {
							if (agent.enabled == true) {

								//SHOOTING BEHAVIOR
								if (shootIntervalCount > 1) {

									if ((int)health.enemyHitPoints > 0) {
										agent.updateRotation = false;
										//						tr.LookAt (objTransform);
										StartCoroutine (RotateToObject (SpookedTarget.transform.position, 0.1f));
										//SHOOT YOU IN RANGE
										if (SpookedTarget != null) {
											if (SpookedTarget.GetComponent<TeamMember> () != null) {
												if (SpookedTarget.GetActive () && SpookedTarget.GetComponent<TeamMember> ().teamID != teamMember.teamID) {
													if (SpookedTarget.CompareTag (enemyTag)) {
														if ((int)health.enemyHitPoints > 0 && isNoObstacleToPlayer3 && canShoot) {
															//Enemy Types Established So Type0 can shoot, others are Zombies for now
															if (EnemyType == (int)enumEnemyType.TeamEnemy || EnemyType == (int)enumEnemyType.NeutralEnemy) {
																if (SpookedTarget.GetComponent<EnemyMovement> ().EnemyType == (int)enumEnemyType.TeamEnemy && (int)SpookedTarget.GetComponent<Health> ().enemyHitPoints > 0) {
																	DoEnemyRocketBulletFx (SpookedTarget.transform.position);
																}
															}
														}
													} else if (SpookedTarget.CompareTag (playerTag)) {
														if (!health.isPlayerDead && isNoObstacleToPlayer3 && canShoot) {
															//Enemy Types Established So Type0 can shoot, others are Zombies for now
															if (EnemyType == (int)enumEnemyType.TeamEnemy || EnemyType == (int)enumEnemyType.NeutralEnemy) {
																if (!SpookedTarget.GetComponent<Health> ().isPlayerDead) {
																	DoEnemyRocketBulletFx (SpookedTarget.transform.position);
																}
															}
														}
													}
												}
											}
											aimDirection = (SpookedTarget.transform.position - tr.position);
											aimDirection = ApplyAimVariance (aimDirection);
											aimDirection.Normalize ();
										}

										agent.updateRotation = true;

									}
									shootIntervalCount = 0;
								} else {
									shootIntervalCount++;
								}
								//END OF SHOOTING BEHAVIOR
								SetEnemyDestination (SpookedTarget);

							}
						}
						yield return waitforRandom01_02;
					} else if (ThreatTarget) {


						////////////////////////////////////////////////////////////////////////////////////////////
						////////////////////////////////////////////////////////////////////////////////////////////
						//THREAT ENEMY BEHAVIOR SECTION THREAT ENEMY BEHAVIOR SECTION THREAT ENEMY BEHAVIOR SECTION
						////////////////////////////////////////////////////////////////////////////////////////////
						////////////////////////////////////////////////////////////////////////////////////////////

						if (ThreatTarget.GetActive ()) {
							if (agent.enabled == true) {

								//DON'T SHOOT IF PLAYER TARGET IS BEHIND A WALL RESPECTIVE TO ENEMY
								isNoObstacleToPlayer2 = false;
								ray2 = new Ray (tr.position, ThreatTarget.transform.position - tr.position);

								if (Physics.Raycast (ray2, out hit2, threatViewDistance)) {
									if (hit2.collider.CompareTag (playerTag) || hit2.collider.CompareTag (enemyTag)) {
										isNoObstacleToPlayer2 = true;
										if (agent.enabled) {
											if (ResetPathIntervalThreat > 1) {
												ResetPathIntervalThreat = 0;
												SetEnemyDestination (ThreatTarget);
											} else {
												ResetPathIntervalThreat++;
											}
										}
									}

									if (EnemyType == (int)enumEnemyType.ZombieEnemy) {
										isNoObstacleToPlayer2 = true;
									}
								}

								//SHOOTING BEHAVIOR
								if (shootIntervalCount > 1) {

									if ((int)health.enemyHitPoints > 0) {
										agent.updateRotation = false;
										//						tr.LookAt (objTransform);
										StartCoroutine (RotateToObject (ThreatTarget.transform.position, 0.1f));
										//SHOOT YOU IN RANGE
										if (ThreatTarget != null) {
											if (ThreatTarget.GetComponent<TeamMember> () != null) {
												if (ThreatTarget.GetActive () && ThreatTarget.GetComponent<TeamMember> ().teamID != teamMember.teamID) {
													if ((int)health.enemyHitPoints > 0 && isNoObstacleToPlayer2 && canShoot) {
														if (EnemyType == (int)enumEnemyType.TeamEnemy || EnemyType == (int)enumEnemyType.NeutralEnemy) {
															if (ThreatTarget.CompareTag (playerTag) && !ThreatTarget.GetComponent<Health> ().isPlayerDead) {
																DoEnemyRocketBulletFx (ThreatTarget.transform.position);
															} else if (ThreatTarget.CompareTag (enemyTag) && ThreatTarget.GetComponent<EnemyMovement> ().EnemyType == (int)enumEnemyType.TeamEnemy && (int)ThreatTarget.GetComponent<Health> ().enemyHitPoints > 0) {
																DoEnemyRocketBulletFx (ThreatTarget.transform.position);
															}
														}
													}
												}
											}
											aimDirection = (ThreatTarget.transform.position - tr.position);
											aimDirection = ApplyAimVariance (aimDirection);
											aimDirection.Normalize ();
										}

										agent.updateRotation = true;

									}
									shootIntervalCount = 0;
								} else {
									shootIntervalCount++;
								}
								//END OF SHOOTING BEHAVIOR
								SetEnemyDestination (ThreatTarget);
							}
						}
						yield return waitforRandom01_02;
					} else {
						//PATROL BEHAVIOR
						if (ObjectTarget) {
							if (ObjectTarget.GetActive ()) {
								if (agent.enabled == true) {
									// Check if we've reached the destination

									if (isTooFarFromBase) {
										if (BaseNodeTarget)
											agent.SetDestination (BaseNodeTarget.transform.position);
										isTooFarFromBase = false;
									}

									if (health.isDodgePlayerAttack) {
										if (BaseNodeTarget)
											agent.SetDestination (BaseNodeTarget.transform.position);
									} else {
										if (EnemyType == (int)enumEnemyType.NeutralEnemy) {
											//											if (agent.remainingDistance <= 20f) {
											agent.SetDestination (BaseNodeTarget.transform.position);
											//											}
										} else if (EnemyType == (int)enumEnemyType.TeamEnemy) {
											if (teamMember.teamID == 1) {
												agent.SetDestination (subscribedEnemySpawnTrigger.BlueTeamSpawnSpots [Random.Range (0, subscribedEnemySpawnTrigger.BlueTeamSpawnSpots.Length)].transform.position);
											} else if (teamMember.teamID == 2) {
												agent.SetDestination (subscribedEnemySpawnTrigger.RedTeamSpawnSpots [Random.Range (0, subscribedEnemySpawnTrigger.RedTeamSpawnSpots.Length)].transform.position);
											}
										} else {
											if (BaseNodeTarget)
												agent.SetDestination (BaseNodeTarget.transform.position);
										}



									}
								}
							}
							yield return waitforRandom01_02;
						}
					}
				} else {
					//GAME HAS STOPPED
					if (agent.enabled == true) {
						agent.isStopped = true;
						agent.velocity = Vector3.zero;
					}
				}
			}
			yield return waitforRandom01_02;
		}
	}


	public void SetEnemyDestination(GameObject target){
		if (health.enemyHitPoints > 0) {
			if (health.isDodgePlayerAttack) {
				if (BaseNodeTarget)
					agent.SetDestination (BaseNodeTarget.transform.position);
			} else {
				if (EnemyType == (int)enumEnemyType.TeamEnemy || EnemyType == (int)enumEnemyType.NeutralEnemy || EnemyType == (int)enumEnemyType.ZombieEnemy) {
					if (target.CompareTag (playerTag)) {
						if (!target.GetComponent<Health> ().isPlayerDead) {																			
							agent.SetDestination (target.transform.position);
						} else {
							if (EnemyType == (int)enumEnemyType.TeamEnemy) {
								agent.SetDestination (subscribedEnemySpawnTrigger.BlueTeamSpawnSpots [Random.Range (0, subscribedEnemySpawnTrigger.BlueTeamSpawnSpots.Length)].transform.position);
							} else {
								if (BaseNodeTarget)
									agent.SetDestination (BaseNodeTarget.transform.position);
							}
						}

					} else if (target.CompareTag (enemyTag)) {
						if (target.GetComponent<Health> ().enemyHitPoints > 0) {																			
							agent.SetDestination (target.transform.position);
						} else {
							if (EnemyType == (int)enumEnemyType.TeamEnemy) {
								agent.SetDestination (subscribedEnemySpawnTrigger.BlueTeamSpawnSpots [Random.Range (0, subscribedEnemySpawnTrigger.BlueTeamSpawnSpots.Length)].transform.position);
							} else {
								if (BaseNodeTarget)
									agent.SetDestination (BaseNodeTarget.transform.position);
							}
						}															
					}
				} else {
					if (BaseNodeTarget)
						agent.SetDestination (BaseNodeTarget.transform.position);
				}
			}
		}												

	}

	public void SetEnemyTimeOfDayResponsiveness(){
		if (guiManager.hourOfDay >= 0 && guiManager.hourOfDay < 6) {
			gunSpookDistance = 100f;
			movementSpookDistance = 100f;
			threatViewDistance = 50f;
		} else if (guiManager.hourOfDay >= 6 && guiManager.hourOfDay < 9) {
			gunSpookDistance = 150f;
			movementSpookDistance = 150f;
			threatViewDistance = 75f;
		} else if (guiManager.hourOfDay >= 9 && guiManager.hourOfDay < 15) {
			gunSpookDistance = 200f;
			movementSpookDistance = 200f;
			threatViewDistance = 100f;
		} else if (guiManager.hourOfDay >= 15 && guiManager.hourOfDay < 18) {
			gunSpookDistance = 150f;
			movementSpookDistance = 150f;
			threatViewDistance = 75f;
		} else if (guiManager.hourOfDay >= 18 && guiManager.hourOfDay <= 24) {
			gunSpookDistance = 100f;
			movementSpookDistance = 100f;
			threatViewDistance = 50f;
		}
	}

	public bool isHostageRescued = false;


	public Coroutine CivilianBehaviorCoroutine;
	public IEnumerator CivilianBehavior_Coroutine(){
		while (true) {
			if (PhotonNetwork.isMasterClient) {

				if ((mTimer.SecondsUntilItsTime <= 0) || guiManager.GAMEEND || (guiManager.currentGameMode == 2 && guiManager.currentHostagesRemaining <= 0)) {
					yield return true;
				}

				if (mTimer.SecondsUntilItsTime > 0 || guiManager.GAMEEND) {


					if (isFollowingRecruiter) {
						RescueTarget = getClosestRescuePlayerObject (out RescuerPlayerName);
						if (RescueTarget) {
							if (RescueTarget.GetActive ()) {
								if (agent.enabled == true) {
									if (agent.isStopped) {
										agent.isStopped = false;
									}
									lastRescueTarget = RescueTarget;
									agent.SetDestination (RescueTarget.transform.position);
								}
							}

						}
					} else {
						if (lastRescueTarget) {
							enemyPhotonView.RPC ("SetIsFollowingPlayer", PhotonTargets.AllBuffered, false, -9);
							lastRescueTarget = null;
						}
						CivilianTarget = getRandomCivilianNodeObject ();
						if (CivilianTarget) {
							if (agent.enabled) {
								agent.SetDestination (CivilianTarget.transform.position);
							}
						}
					}


				}
			}
			yield return waitfor0_25;
		}
	}


	public void SetSubscribedSpawnTrigger(int val){
		foreach (GameObject est in enemySpawnTriggerList) {
			if (est.GetComponent<EnemySpawnTrigger> ().baseNumber == val) {
				subscribedEnemySpawnTrigger = est.GetComponent<EnemySpawnTrigger> ();
//				Debug.Log ("Subscribed to: " + val);
			}
		}
	}

	[PunRPC]
	public void SetEnemyMovementBaseNumber(int val){
		baseNumber = val;
		//		subscribedEnemySpawnTrigger = EST;
	}

	string tagBase0Node = "Base0Node";
	string tagBase1Node = "Base1Node";
	string tagBase2Node = "Base2Node";
	string tagBase3Node = "Base3Node";
	string tagBase4Node = "Base4Node";
	string tagCivilianNode = "CivilianNode";
	string tagPatrolNode = "PatrolNode";

	public GameObject[] InitBaseNodes(int baseNumber, GameObject[] BaseList){
		if (baseNumber == 0)
			BaseList = base0List == null ? GameObject.FindGameObjectsWithTag (tagBase0Node) : base0List;
		else if (baseNumber == 1)
			BaseList = base1List == null ? GameObject.FindGameObjectsWithTag (tagBase1Node) : base1List;
		else if (baseNumber == 2)
			BaseList = base2List == null ? GameObject.FindGameObjectsWithTag (tagBase2Node) : base2List;
		else if (baseNumber == 3)
			BaseList = base3List == null ? GameObject.FindGameObjectsWithTag (tagBase3Node) : base3List;
		else if (baseNumber == 4)
			BaseList = base4List == null ? GameObject.FindGameObjectsWithTag (tagBase4Node) : base4List;
		else if (baseNumber == 9998)
			BaseList = civilianNodeList == null ? GameObject.FindGameObjectsWithTag (tagCivilianNode) : civilianNodeList;
		else if (baseNumber == 9999)
			BaseList = base4List == null ? GameObject.FindGameObjectsWithTag (tagPatrolNode) : nodeList;
		return BaseList;
	}

	public GameObject getRandomCivilianNodeObject(){

		GameObject randomCivilianNode = null;

		if (civilianNodeList != null)
			randomCivilianNode = civilianNodeList [Random.Range (0, (civilianNodeList.Length - 1))];
		return randomCivilianNode;
	}

	public GameObject getRandomPatrolNodeObject(){

		GameObject node = null;

		if (nodeList != null)
			node = nodeList [Random.Range (0, (nodeList.Length - 1))];
		return node;
	}


	public GameObject getRandomHostagePlayer(){
		GameObject hostage = null;

		if (hostageRescueZoneTrigger.hostageList != null) {
			hostage = hostageRescueZoneTrigger.hostageList [Random.Range (0, (hostageRescueZoneTrigger.hostageList.Length - 1))];
		}
		return hostage;
	}

	bool isTooFarFromBase = false;
	public bool IsEnemyTooFarFromBase(){
		float closestDistance = Mathf.Infinity;
		GameObject[] generalBaseList = base0List;

		if (baseNumber == 0)
			generalBaseList = base0List;
		else if (baseNumber == 1)
			generalBaseList = base1List;
		else if (baseNumber == 2)
			generalBaseList = base2List;
		else if (baseNumber == 3)
			generalBaseList = base3List;
		else if (baseNumber == 4)
			generalBaseList = base4List;

		foreach (GameObject currentNode in generalBaseList) {
			if (generalBaseList != null) {
				if (Vector3.Distance (tr.position, currentNode.transform.position) < closestDistance)
					closestDistance = Vector3.Distance (tr.position, currentNode.transform.position);			
			}
		}


		if (closestDistance > 50f) {
			isTooFarFromBase = true;
			return true;
		} else {
			return false;
		}
	}

	public GameObject getRandomBaseNode(bool isRestrictedToBase){

		GameObject node = null;
		int RandomBase = Random.Range (0, 5);
		RandomBase = 0;
		if (isRestrictedToBase) {
			if (baseNumber == 0) {
				if (base0List != null)
					node = base0List [Random.Range (0, (base0List.Length - 1))];
			} else if (baseNumber == 1) {
				if (base1List != null)
					node = base1List [Random.Range (0, (base1List.Length - 1))];
			} else if (baseNumber == 2) {
				if (base2List != null)
					node = base2List [Random.Range (0, (base2List.Length - 1))];
			} else if (baseNumber == 3) {
				if (base3List != null)
					node = base3List [Random.Range (0, (base3List.Length - 1))];
			} else if (baseNumber == 4) {
				if (base4List != null)
					node = base4List [Random.Range (0, (base4List.Length - 1))];
			} else {
				node = base0List [Random.Range (0, (base0List.Length - 1))];
			}
		} else {
			if (RandomBase == 0) {
				if (base0List != null)
					node = base0List [Random.Range (0, (base0List.Length - 1))];
			} else if (RandomBase == 1) {
				if (base1List != null)
					node = base1List [Random.Range (0, (base1List.Length - 1))];
			} else if (RandomBase == 2) {
				if (base2List != null)
					node = base2List [Random.Range (0, (base2List.Length - 1))];
			} else if (RandomBase == 3) {
				if (base3List != null)
					node = base3List [Random.Range (0, (base3List.Length - 1))];
			} else if (RandomBase == 4) {
				if (base4List != null)
					node = base4List [Random.Range (0, (base4List.Length - 1))];
			} else {
				node = base0List [Random.Range (0, (base0List.Length - 1))];
			}
		}
		return node;
	}


	public int DamageDealtCount = 0;

	public bool hasZombieDamagedPlayer = false;
	public Coroutine ZombieDamageCoroutine;
	public IEnumerator ApplyZombieDamage_Coroutine(GameObject player){
		while (hasZombieDamagedPlayer == true) {
			if (!PhotonNetwork.isMasterClient)
				yield return null;

			if (player) {
				if (player.GetComponent<Health> ().currentHitPoints > 0) {
					player.GetComponent<PhotonView> ().RPC (TakeDamageRPC, PhotonTargets.AllBuffered, 50f, gameObject.name, enemyPhotonView.viewID, gameObject.tag);
				
					DamageDealtCount = scoringManager.GetScore (name, scoringManager.damagedealtStat);
					scoringManager.SetScore (name, scoringManager.damagedealtStat, DamageDealtCount + (int)50);

				}
			}
			yield return waitfor1;
		}
	}


	void OnCollisionEnter(Collision collision){
		if (!PhotonNetwork.isMasterClient)
			return;

		if (collision.collider.CompareTag (playerTag) || collision.collider.CompareTag (enemyTag)) {

			if (EnemyType == (int)enumEnemyType.ZombieEnemy) {
				//ZOMBIE DAMAGE
				if (!hasZombieDamagedPlayer && (int)health.enemyHitPoints > 0) {
					if (collision.gameObject.GetComponent<Health> ().currentHitPoints > 0) {
						hasZombieDamagedPlayer = true;
						ZombieDamageCoroutine = StartCoroutine (ApplyZombieDamage_Coroutine (collision.gameObject));
					}
				}
			}
		}

	}

	void OnCollisionExit(Collision collision){
		if (!PhotonNetwork.isMasterClient)
			return;

		if (collision.collider.CompareTag (playerTag) || collision.collider.CompareTag (enemyTag)) {
			if (EnemyType == (int)enumEnemyType.ZombieEnemy) {
				hasZombieDamagedPlayer = false;
			}
		}
	}


	public GameObject getClosestRescuePlayerObject(out string rescuePlayerName){
		GameObject closestPlayer = null;
		GameObject[] playerList = null;
		float distance = Mathf.Infinity;
		float closestDistance = Mathf.Infinity;
		playerList = GameObject.FindGameObjectsWithTag (playerTag);
		rescuePlayerName = string.Empty;
		foreach (GameObject player in playerList) {
			distance = Vector3.Distance (player.transform.position, tr.position);
			if (distance < closestDistance) {
				if (player.GetComponent<PhotonView> ().viewID == rescuerPhotonViewId) {
					rescuePlayerName = player.GetComponent<TeamMember> ().GetPlayerName ();
					if (player.GetComponent<PlayerMovement> ().NPCName.StartsWith(hostageTag) || player.GetComponent<PlayerMovement> ().NPCName.StartsWith(civilianTag)) {
						closestDistance = distance;
						closestPlayer = player;
					}
				}


			}
		}
		return closestPlayer;
	}


	int hostageAggression = 10;
	public GameObject getClosestPlayerOrEnemyObject(out float closestDistance){
		GameObject closestPlayerOrEnemy = null;
		GameObject[] playerList = null;
		GameObject[] enemyList = null;
		float distance = Mathf.Infinity;
		closestDistance = Mathf.Infinity;

		playerList = GameObject.FindGameObjectsWithTag (playerTag);
		enemyList = GameObject.FindGameObjectsWithTag (enemyTag);

		//Just do not attack the Hostage
		foreach (GameObject player in playerList) {
			if (!player.name.StartsWith (hostageTag)) {
				if (player.GetComponent<TeamMember> ().teamID != teamMember.teamID) {
					distance = Vector3.Distance (player.transform.position, tr.position);
					if (distance < closestDistance && distance < threatViewDistance && distance > 2f) {
						closestDistance = distance;
						closestPlayerOrEnemy = player;
					}
				}
			}
		}


		foreach (GameObject enemy in enemyList) {
			if (!enemy.name.StartsWith (hostageTag)) {
				if (enemy.GetComponent<TeamMember> ().teamID != teamMember.teamID && enemy.GetComponent<EnemyMovement> ().EnemyType == (int)enumEnemyType.TeamEnemy) {
					distance = Vector3.Distance (enemy.transform.position, tr.position);
					if (distance < closestDistance && distance < threatViewDistance && distance > 2f) {
						closestDistance = distance;
						closestPlayerOrEnemy = enemy;
					}
				}
			} else {
				if (Random.Range (0, 100) < hostageAggression) {
					if (enemy.GetComponent<EnemyMovement> ().isHostageRescued) {
						distance = Vector3.Distance (enemy.transform.position, tr.position);
						if (distance < closestDistance && distance < threatViewDistance && distance > 2f) {
							closestDistance = distance;
							closestPlayerOrEnemy = enemy;
						}
					}
				}
			}
		}

		return closestPlayerOrEnemy;
	}

	Ray rayNoise;
	Transform hitTransformNoiseMaker;
	bool isEnemyInLineOfNoise = false;
	public GameObject getPlayerWhoIsMakingNoise(){
		noisyPlayerList = null;
		noisyPlayerList = GameObject.FindGameObjectsWithTag (playerTag);
		RaycastHit hit;
		if (noisyPlayerList != null) {
			foreach (GameObject player in noisyPlayerList) {
				//HAS PLAYER SPOOKED THIS ENEMY BY 1) SHOOTING OR 2) JUMPING OR 3) WALKING && !STEALTH

				if (player.GetComponent<TeamMember> ().teamID != teamMember.teamID) {

					if (Physics.Raycast (rayNoise, out hit, gunSpookDistance)) {
						if (hit.transform.CompareTag (playerTag)) {
							if (player.GetComponent<PlayerShooting> ().isPlayerShooting) {
								if (Vector3.Distance (tr.position, player.transform.position) < gunSpookDistance) {
									isSppoked = true;
									return player;
								}
							}				

							if (player.GetComponent<PlayerMovement> ().hasJumpSpookedEnemy ||
								(player.GetComponent<PlayerMovement> ().hasWalkedSpookedEnemy && !player.GetComponent<PlayerMovement> ().isStealthWalk)) {
								if (Vector3.Distance (tr.position, player.transform.position) < movementSpookDistance) {
									isSppoked = true;
									return player;
								}
							}

						}
					}

				}

			}

		}
		isSppoked = false;
		return null;
	}



	public void setLookAtRandom(){
		int randX = Random.Range (-100, 101);
		int randY = Random.Range (-100, 101);
		int randZ = Random.Range (-100, 101);
		randomPos = tr.position + new Vector3 (randX, randY, randZ);
		this.lookAtPos = ProjectPointOnPlane (tr.up, tr.position, randomPos);

	}

	IEnumerator RotateToObject(Vector3 objectPos, float time)
	{
		float elapsedTime =0;

		Vector3 relativePos = objectPos - tr.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);

		while (elapsedTime < time)
		{
			transform.rotation = Quaternion.Slerp (tr.rotation, rotation, (elapsedTime / time));
			elapsedTime += Time.fixedDeltaTime;
			yield return null;
		}
	}


	public Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point){
		planeNormal.Normalize();
		float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
		return point + planeNormal * distance;
	}


	int maxNPCProjectileCount=1;
	GameObject[] bulletList;
	string BulletTag = "Bullet";
	string EnemyRocketFxRPC = "EnemyRocketFx";
	string GrenadeFxRPC = "GrenadeFx";
	string TakeDamageRPC = "TakeDamage";
	string ChangeScoreRPC = "ChangeScore";
	string strRed = "Red";
	string strBlue = "Blue";
	string strBot = "Bot";

	public int RocketFiredCount = 0;
	public int EMPFiredCount = 0;
	public int HMGHitCount = 0;
	public int RocketHitCount = 0;
	public int RailHitCount = 0;
	public int EMPHitCount = 0;
	public int BullsEyeCount = 0;


	int bulletCount=0;
	void DoEnemyRocketBulletFx(Vector3 hitPoint){

		if ((int)health.enemyHitPoints <= 0)
			return;

		if ((mTimer.SecondsUntilItsTime <= 0) || guiManager.GAMEEND || (guiManager.currentGameMode == 2 && guiManager.currentHostagesRemaining <= 0)) {
			return;
		}

		bulletList = GameObject.FindGameObjectsWithTag (BulletTag);

		if (PhotonNetwork.room.PlayerCount == 1) {
			if (bulletList.Length > Mathf.Clamp ((5) * PhotonNetwork.room.PlayerCount, 5, 15))
				return;
		}

		hitPoint = ApplyAimVariance (hitPoint);
		if ((teamMember.teamID == 1 || teamMember.teamID == 2 || teamMember.teamID == 4) && (enemyPhotonView.name.StartsWith (strRed) || enemyPhotonView.name.StartsWith (strBlue) || enemyPhotonView.name.StartsWith (strBot))) {
			if (Random.Range (0, 10) < -300) {
				scenePhotonView.RPC (GrenadeFxRPC, PhotonTargets.All, tr.position, hitPoint, enemyPhotonView.name, teamMember.teamID, GetComponent<PhotonView> ().viewID, gameObject.tag);

				EMPFiredCount = scoringManager.GetScore (enemyPhotonView.name, scoringManager.shotfired_empStat);
				scoringManager.SetScore (enemyPhotonView.name, scoringManager.shotfired_empStat, EMPFiredCount + 1);
				//				scenePhotonView.RPC (ChangeScoreRPC, PhotonTargets.AllBuffered, enemyPhotonView.name, shotfired_emp, 1);
			} else {
				scenePhotonView.RPC (EnemyRocketFxRPC, PhotonTargets.All, tr.position, hitPoint, enemyPhotonView.name, teamMember.teamID, GetComponent<PhotonView> ().viewID, gameObject.tag);

				RocketFiredCount = scoringManager.GetScore (enemyPhotonView.name, scoringManager.shotfired_rocketStat);
				scoringManager.SetScore (enemyPhotonView.name, scoringManager.shotfired_rocketStat, RocketFiredCount + 1);
				//				scenePhotonView.RPC (ChangeScoreRPC, PhotonTargets.AllBuffered, enemyPhotonView.name, shotfired_rocket, 1);
			}

		}


	}


	Vector3 ApplyAimVariance(Vector3 modifiedHitVector){
		modifiedHitVector += new Vector3 (Random.Range (-100, 100) / 25f, Random.Range (-100, 100) / 25f, Random.Range (-100, 100) / 50f);
		return modifiedHitVector;
	}


}