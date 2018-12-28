using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	FXManager fxManager;
	ScoringManager scoringManager;
	NetworkManager nManager;
	PhotonView sceneScriptsPhotonView;
	public GameObject sceneScripts;

	private bool hasAppliedProjectileHitStat = false;
	private bool hasAppliedBullsEyeStat = false;
	private Rigidbody rbody;
	private Vector3 direction;
	private Vector3 lastPosition;
	private Vector3 rocketDirection = Vector3.zero;

	public int projectileType=1;
	public float rayCastDistance = 5f;
	public float projectileSpeed = 150.0f;
	public float blastSpeed = 30.0f;
	public float blastRadius = 20.0f;
	public float blastDamage = 250f;
	public float blastTime = 1f;
	public AudioClip[] bounceSound;
	public AudioClip[] damageSound;
	public LayerMask myLayers;

	private float grenadeTimer=2f;
	private float teleporterTimer=10f;

	private float _DistanceToTarget = Mathf.Infinity;
	private string _ShooterName = "";
	private int _teamID = 0;
	private int _shooterPhotonID;
	private string _ShooterTag = "";
	//	private bool teleportFlag = false;

	private string tagEnemy = "Enemy";
	private string tagPlayer = "Player";
	private string tagHead = "Head";
	private string tagLevel = "Level";
	private string tagJumpPad = "JumpPad";
	private string takeDamageRPC = "TakeDamage";
	private string setKnockBackOverrideRPC = "SetKnockBackOverride";
	private string changeScoreRPC = "ChangeScore";

	int DamageDealtCount;

	delegate void MultiDelegate();
	MultiDelegate projectileBehaviorDelegate;

	// Use this for initialization
	void Start () {
		fxManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<FXManager> ();
		scoringManager = fxManager.GetComponent<ScoringManager> ();
		nManager = scoringManager.GetComponent<NetworkManager> ();
		sceneScriptsPhotonView = fxManager.GetComponent<PhotonView> ();
		rbody = GetComponent<Rigidbody> (); 
		lastPosition = rbody.position;

		projectileBehaviorDelegate += applyProjectileDamage;
		projectileBehaviorDelegate += DoExplosion;

	}

	void OnEnable(){
		BounceCount = 0;
		//		teleportFlag = false;
		grenadeTimer = 2f;
		teleporterTimer = 10f;
		elapsedBlastTime = 0;

		foundShooter = false;
		directHitOverride = false;
	}

	bool foundShooter = false;
	bool directHitOverride = false;

	Transform hitTr;
	float elapsedBlastTime = 0f;
	void FixedUpdate () {

		if (!foundShooter) {
			shooterObject = GetPlayerEnemyByPhotonID ();
			foundShooter = true;
		}

		if (shooterObject == null) {
			Debug.Log ("Shooter not found");
			DoExplosion ();
			return;
		}

		elapsedBlastTime += Time.fixedDeltaTime;
		direction = rbody.position - lastPosition;
		Ray ray = new Ray(lastPosition, direction);
		RaycastHit hit;

		if (projectileType == 1 || projectileType == 3) {

			if (_DistanceToTarget <= 2.5f) {
				projectileBehaviorDelegate ();
			}

			if (Physics.Raycast (ray, out hit, rayCastDistance, myLayers)) {
				hitTr = hit.transform;
				if (hitTr.CompareTag (tagEnemy) || hitTr.CompareTag (tagPlayer) || hitTr.CompareTag(tagHead)) {

					if (shooterTag.Equals (tagPlayer)) {
						if (hitTr.GetComponent<PhotonView> ().viewID == shooterPhotonID) {
							rbody.MovePosition (rbody.position + rocketDirection * projectileSpeed * Time.fixedDeltaTime);
							this.lastPosition = rbody.position;
						} else {
							directHitOverride = true;
							fxManager.isProjectileDirectHit = true;
						}
					}
				}
				if (hitTr.GetComponent<PhotonView> () != null) {
					if (hitTr.GetComponent<PhotonView> ().viewID != shooterPhotonID) {
						projectileBehaviorDelegate ();
					}
				} else {
					projectileBehaviorDelegate ();
				}
			} else if (elapsedBlastTime < blastTime) {
				rbody.MovePosition (rbody.position + rocketDirection * projectileSpeed * Time.fixedDeltaTime);
			} else {
				DoExplosion ();
			}
			this.lastPosition = rbody.position;
		}
		//Grenade
		else if (projectileType == 2) {
			if (grenadeTimer > 0) {
				rbody.MovePosition (rbody.position + (rocketDirection) * projectileSpeed * Time.fixedDeltaTime);
				rocketDirection.y -= 1f * Time.fixedDeltaTime;
				detectCollision ();
				grenadeTimer -= Time.fixedDeltaTime;
			} else {
				projectileBehaviorDelegate ();
			}
		} else if (projectileType == 4) {
			if (teleporterTimer > 0 && BounceCount < 1) {
				rbody.MovePosition (rbody.position + (rocketDirection) * projectileSpeed * Time.fixedDeltaTime);
				//				rocketDirection.y -= .25f * Time.fixedDeltaTime;
				rocketDirection.y -= 1f * Time.fixedDeltaTime;
				detectCollision ();
				teleporterTimer -= Time.fixedDeltaTime;
			} else {
				applyTeleportation ();
				DoExplosion ();
			}
		}
	}

	void BounceSound(){
		GetComponent<AudioSource> ().PlayOneShot (bounceSound [0]);
	}


	string strExplosionFx = "ExplosionFx";
	string strEMPExplosionFx = "EMPExplosionFx";
	string strTeleportExplosionFx = "TeleportExplosionFx";
	void DoExplosion(){

		Vector3 explosionPos = rbody.position;
		if (fxManager != null) {
			if (projectileType == 1 || projectileType == 3) {
				fxManager.ExplosionFx (explosionPos);
			} else if (projectileType == 2) {
				fxManager.EMPExplosionFx (explosionPos);
			} else if (projectileType == 4) {
				fxManager.TeleportExplosionFx (explosionPos);
			}
		}
		if (projectileType == 1) {
			fxManager.DestroyFXPrefab (gameObject, fxManager.rocketList);
		} else if (projectileType == 2) {
			fxManager.DestroyFXPrefab (gameObject, fxManager.grenadeList);
		} else if (projectileType == 3) {
			fxManager.DestroyFXPrefab (gameObject, fxManager.enemyRocketList);
		} else if (projectileType == 4) {
			fxManager.DestroyFXPrefab (gameObject, fxManager.teleportGrenadeList);
		}
	}

	int layerMask = 1 << 8; // Layer8

	Vector3 TossDirection;
	Vector3 explosionPosition;
	float explosionRadius;
	Collider[] colliders;
	PhotonView pv;
	PlayerMovement pMovement;
	PlayerShooting pShooting;
	EnemyMovement eMovement;
	Health health;

	RaycastHit hit;
	float damage = 0f;
	float distance = 0f;
	float speedMultiplier = 0f;
	Vector3 rocketHeightCorrection = Vector3.zero;
	Vector3 newPlayerVelocity = Vector3.zero;
	void applyProjectileDamage () {
		//Only Masterclient performs this

		explosionPosition = rbody.position;
		explosionRadius = blastRadius;
		colliders = Physics.OverlapSphere (explosionPosition, explosionRadius, layerMask);
		damage = 0f;
		distance = 0f;
		speedMultiplier = 0f;
		rocketHeightCorrection = Vector3.zero;

		if (shooterTag.Equals (tagPlayer)) {
			pShooting = shooterObject.GetComponent<PlayerShooting> ();
		} else {
			eMovement = shooterObject.GetComponent<EnemyMovement> ();
		}


		foreach (Collider hitCollider in colliders) {

			Physics.Raycast (rbody.position, (hitCollider.transform.position - rbody.position), out hit);
			if (hit.collider.CompareTag (tagLevel)) {
				continue;
			}

			distance = Vector3.Distance (hitCollider.transform.position, explosionPosition);
			damage = 1 - Mathf.Clamp01 (distance / explosionRadius);

			health = hitCollider.GetComponent<Health> ();
			pv = hitCollider.GetComponent<PhotonView> ();
			pMovement = hitCollider.GetComponent<PlayerMovement> ();

			if (health != null && projectileType != 3) {
				if (hitCollider.CompareTag (tagPlayer) || hitCollider.CompareTag (tagEnemy)) {
					//This line below is probably important to dealing damage to the correct player
					if (health && hitCollider.GetComponent<TeamMember> ().teamID != this.teamID) {
						if (directHitOverride) {
//								damage = damage * 2f;
							if (shooterTag.Equals (tagPlayer)) {
								applyBullsEyeHitStat ();
							}
						}
						if (projectileType == 1) {
							if (fxManager.localClient.viewID == shooterPhotonID) {
								pv.RPC (takeDamageRPC, PhotonTargets.All, Mathf.Round (Mathf.Clamp (100 * damage, 1, 100)), _ShooterName, shooterPhotonID, shooterTag);
								pShooting.DamageDealtCount = scoringManager.GetScore (_ShooterName, scoringManager.damagedealtStat);
								scoringManager.SetScore (_ShooterName, scoringManager.damagedealtStat, pShooting.DamageDealtCount + Mathf.RoundToInt (Mathf.Clamp (100 * damage, 1, 100)));

//								shooterObject.GetComponent<PlayerShooting> ().PlayerScoreTable [shooterObject.GetComponent<PlayerShooting> ().DamageDealtProp] = shooterObject.GetComponent<PlayerShooting> ().DamageDealtCount + Mathf.RoundToInt (Mathf.Clamp (100 * damage, 1, 100));
//								PhotonNetwork.player.SetCustomProperties (shooterObject.GetComponent<PlayerShooting> ().PlayerScoreTable);


								applyProjectileHitStat (1);
							}
						} else if (projectileType == 2) {							
							if (fxManager.localClient.viewID == shooterPhotonID) {
								pv.RPC (takeDamageRPC, PhotonTargets.All, Mathf.Round (Mathf.Clamp (150 * damage, 1, 150)), _ShooterName, shooterPhotonID, shooterTag);
								pShooting.DamageDealtCount = scoringManager.GetScore (_ShooterName, scoringManager.damagedealtStat);
								scoringManager.SetScore (_ShooterName, scoringManager.damagedealtStat, pShooting.DamageDealtCount + Mathf.RoundToInt (Mathf.Clamp (150 * damage, 1, 150)));

//								shooterObject.GetComponent<PlayerShooting> ().PlayerScoreTable [shooterObject.GetComponent<PlayerShooting> ().DamageDealtProp] = shooterObject.GetComponent<PlayerShooting> ().DamageDealtCount + Mathf.RoundToInt (Mathf.Clamp (150 * damage, 1, 150));
//								PhotonNetwork.player.SetCustomProperties (shooterObject.GetComponent<PlayerShooting> ().PlayerScoreTable);
								applyProjectileHitStat (2);
							}
						}
					}
				}
			}
			//PROJECTILE TYPE=3 IS FOR ENEMY BOT PROJECTILE, DOES NOT CONSIDER TEAM FOR FRIENDLY FIRE BECAUSE ALL PLAYERS SHOULD BE SAME TEAM
			else if (health != null && projectileType == 3) {
				applyProjectileHitStat (3);
				if (hitCollider.CompareTag (tagPlayer)) {
					if (health && hitCollider.GetComponent<TeamMember> ().teamID != _teamID) {
						if (PhotonNetwork.isMasterClient) {
							pv.RPC (takeDamageRPC, PhotonTargets.All, Mathf.Round (Mathf.Clamp (100 * damage, 1, 100)), _ShooterName, shooterPhotonID, shooterTag);
							eMovement.DamageDealtCount = scoringManager.GetScore (_ShooterName, scoringManager.damagedealtStat);
							scoringManager.SetScore (_ShooterName, scoringManager.damagedealtStat, eMovement.DamageDealtCount + Mathf.RoundToInt (Mathf.Clamp (100 * damage, 1, 100)));
						}
					}
				} else if (hitCollider.CompareTag (tagEnemy)) {
					if (health && hitCollider.GetComponent<TeamMember> ().teamID != _teamID) {
						if (PhotonNetwork.isMasterClient) {
							if (nManager.roomSize == 1) {
								if (nManager.localPlayerTeamID != _teamID) {
									pv.RPC (takeDamageRPC, PhotonTargets.All, Mathf.Round (Mathf.Clamp (100 * damage, 1, 100)), _ShooterName, shooterPhotonID, shooterTag);
									eMovement.DamageDealtCount = scoringManager.GetScore (_ShooterName, scoringManager.damagedealtStat);
									scoringManager.SetScore (_ShooterName, scoringManager.damagedealtStat, eMovement.DamageDealtCount + Mathf.RoundToInt (Mathf.Clamp (100 * damage, 1, 100)));
								} else {
									pv.RPC (takeDamageRPC, PhotonTargets.All, Mathf.Round (Mathf.Clamp (50 * damage, 1, 50)), _ShooterName, shooterPhotonID, shooterTag);
									eMovement.DamageDealtCount = scoringManager.GetScore (_ShooterName, scoringManager.damagedealtStat);
									scoringManager.SetScore (_ShooterName, scoringManager.damagedealtStat, eMovement.DamageDealtCount + Mathf.RoundToInt (Mathf.Clamp (50 * damage, 1, 50)));
								}
							} else {
								pv.RPC (takeDamageRPC, PhotonTargets.All, Mathf.Round (Mathf.Clamp (50 * damage, 1, 50)), _ShooterName, shooterPhotonID, shooterTag);
								eMovement.DamageDealtCount = scoringManager.GetScore (_ShooterName, scoringManager.damagedealtStat);
								scoringManager.SetScore (_ShooterName, scoringManager.damagedealtStat, eMovement.DamageDealtCount + Mathf.RoundToInt (Mathf.Clamp (50 * damage, 1, 50)));
							}
						}
					}
				}

			}

			if (fxManager.localClient.viewID == shooterPhotonID || shooterTag.Equals (tagEnemy)) {
				//TOSS DIRECTION PART FOR MASTER CLIENT
				if (hitCollider.CompareTag (tagPlayer) && pMovement != null) {
					rocketHeightCorrection = new Vector3 (0, rayCastDistance, 0f);
					speedMultiplier = 1 - Mathf.Clamp01 (Vector3.Distance (hitCollider.transform.position, rbody.position) / explosionRadius);
					TossDirection = hitCollider.transform.position - rbody.position + rocketHeightCorrection;
					newPlayerVelocity = pMovement.playerVelocity + (TossDirection * 10 * speedMultiplier);

					pMovement.SetKnockBackOverride (true, newPlayerVelocity);
				}
			} else if (PhotonNetwork.isMasterClient) {
				if (hitCollider.CompareTag (tagPlayer) && pMovement != null) {
					rocketHeightCorrection = new Vector3 (0, rayCastDistance, 0f);
					speedMultiplier = 1 - Mathf.Clamp01 (Vector3.Distance (hitCollider.transform.position, rbody.position) / explosionRadius);
					TossDirection = hitCollider.transform.position - rbody.position + rocketHeightCorrection;
					newPlayerVelocity = pMovement.playerVelocity + (TossDirection * 10 * speedMultiplier);
					//					pv.RPC (setKnockBackOverrideRPC, PhotonTargets.All, true, newPlayerVelocity);

					pMovement.SetKnockBackOverride (true, newPlayerVelocity);
				}					
			}

		}

		shooterObject = null;

	}
		
	string stat_shothit_bullseye = "shothit_bullseye";
	string stat_shothit_rocket = "shothit_rocket";
	string stat_shothit_emp = "shothit_emp";

	int RocketHitCount=0;
	int GrenadeHitCount=0;

	void applyBullsEyeHitStat(){
		if (!hasAppliedBullsEyeStat) {
			if (shooterTag.Equals (tagEnemy)) {
				eMovement.BullsEyeCount = scoringManager.GetScore (shooterName, stat_shothit_bullseye);
				scoringManager.SetScore (shooterName, stat_shothit_bullseye, eMovement.BullsEyeCount + 1);
			} else if (shooterTag.Equals (tagPlayer)) {
				pShooting.BullsEyeCount = scoringManager.GetScore (shooterName, stat_shothit_bullseye);
				scoringManager.SetScore (shooterName, stat_shothit_bullseye, pShooting.BullsEyeCount + 1);
			}
			//			sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, shooterName, stat_shothit_bullseye, 1);
			hasAppliedBullsEyeStat = true;
		}
	}

	void applyProjectileHitStat(int bulletType){
		if (!hasAppliedProjectileHitStat) {
			if (bulletType == 1) {		
				//				sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, shooterName, stat_shothit_rocket, 1);
				if (shooterTag.Equals (tagEnemy)) {
					eMovement.RocketHitCount = scoringManager.GetScore (shooterName, stat_shothit_rocket);
					scoringManager.SetScore (shooterName, stat_shothit_rocket, eMovement.RocketHitCount + 1);
				} else if (shooterTag.Equals (tagPlayer)) {
					pShooting.RocketHitCount = scoringManager.GetScore (shooterName, stat_shothit_rocket);
					scoringManager.SetScore (shooterName, stat_shothit_rocket, pShooting.RocketHitCount + 1);
				}
			} else if (bulletType == 2) {
				//				sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, shooterName, stat_shothit_emp, 1);
				if (shooterTag.Equals (tagEnemy)) {
					eMovement.EMPHitCount = scoringManager.GetScore (shooterName, stat_shothit_emp);
					scoringManager.SetScore(shooterName, stat_shothit_emp, eMovement.EMPHitCount + 1);
				} else if (shooterTag.Equals (tagPlayer)) {
					pShooting.EMPHitCount = scoringManager.GetScore (shooterName, stat_shothit_emp);
					scoringManager.SetScore(shooterName, stat_shothit_emp, pShooting.EMPHitCount + 1);
				}
			} else if (bulletType == 3) {
				//				sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, shooterName, stat_shothit_rocket, 1);
				if (shooterObject != null) {
					if (shooterTag.Equals (tagEnemy)) {
						eMovement.RocketHitCount = scoringManager.GetScore (shooterName, stat_shothit_rocket);
						scoringManager.SetScore (shooterName, stat_shothit_rocket, eMovement.RocketHitCount + 1);
					} else if (shooterTag.Equals (tagPlayer)) {
						pShooting.RocketHitCount = scoringManager.GetScore (shooterName, stat_shothit_rocket);
						scoringManager.SetScore (shooterName, stat_shothit_rocket, pShooting.RocketHitCount + 1);
					}
				}
			}
			hasAppliedProjectileHitStat = true;
		}
	}


	GameObject[] PlayerEnemyList;
	GameObject shooterObject;
	GameObject GetPlayerEnemyByPhotonID(){
		if (_shooterPhotonID == null)
			return null;

		if (shooterTag.Equals (tagPlayer)) {
			PlayerEnemyList = GameObject.FindGameObjectsWithTag (tagPlayer);
		} else if (shooterTag.Equals (tagEnemy)) {
			PlayerEnemyList = GameObject.FindGameObjectsWithTag (tagEnemy);
		}

		foreach (GameObject playerOrEnemy in PlayerEnemyList) {
			if (playerOrEnemy.GetComponent<PhotonView> ().viewID == _shooterPhotonID) {
				return playerOrEnemy;
			}
		}
		return null;
	}


	void applyTeleportation (){
		//Only Masterclient performs this
		if (_shooterPhotonID == null)
			return;

		GameObject[] players = GameObject.FindGameObjectsWithTag (tagPlayer);
		foreach (GameObject player in players) {
			if (player.GetComponent<PhotonView> ().viewID == _shooterPhotonID) {
				player.transform.position = rbody.position + 1 * (Vector3.up);
				break;
			}
		}
	}

	public void setDistanceToTarget(float dist){
		this._DistanceToTarget = dist;
	}

	public void setShooterName(string ShooterName){
		this._ShooterName = ShooterName;
	}

	public void setTeamID(int id){
		this._teamID = id;
	}

	public void setShooterPhotonID(int id){
		this._shooterPhotonID = id;
	}

	public void setShooterTag(string tag){
		this._ShooterTag = tag;
	}

	public int teamID{
		get{ return _teamID;}
	}
	public string shooterName{
		get{ return _ShooterName;}
	}
	public int shooterPhotonID {
		get{ return _shooterPhotonID; }
	}
	public string shooterTag {
		get{ return _ShooterTag; }
	}


	public void setProjectileDirection(Vector3 direction){
		this.rocketDirection = direction;
	}

	int BounceCount = 0;
	void detectCollision(){
		//Send a ray in the direction of the projectile
		Ray ray = new Ray (rbody.position, rocketDirection);
		RaycastHit hit;
		Vector3 speedReduction = Vector3.zero;

		float detectRangeRocket = 2.5f;
		float detectRangeEmp = 2.5f;
		float detectRangeTelePorter = 5f;
		float range = detectRangeRocket;

		if (projectileType == 4) {
			range = detectRangeTelePorter;
		}

		if (Physics.Raycast (ray, out hit, range)) {
			if (hit.transform.CompareTag (tagLevel) || hit.transform.CompareTag (tagJumpPad)) {

				speedReduction = hit.normal;

				float xDamper = 0;
				float yDamper = 0;
				float zDamper = 0;

				float bounceDamper = 0;

				if (hit.normal == Vector3.up || hit.normal == -Vector3.up) {
					rocketDirection.y = bounceDamper != 0 ? bounceDamper : rocketDirection.y * 0.5f;
				} else if (hit.normal == Vector3.left || hit.normal == -Vector3.left) {
					rocketDirection.x = bounceDamper != 0 ? bounceDamper : rocketDirection.x * 0.5f;
				} else if (hit.normal == Vector3.forward || hit.normal == -Vector3.forward) {
					rocketDirection.z = bounceDamper != 0 ? bounceDamper : rocketDirection.z * 0.5f;
				} else {
					xDamper = hit.normal.x != 0.0f ? hit.normal.x * 0.2f + rocketDirection.x : rocketDirection.x;
					yDamper = hit.normal.y != 0.0f ? hit.normal.y * 0.2f + rocketDirection.y : rocketDirection.y;
					zDamper = hit.normal.z != 0.0f ? hit.normal.z * 0.2f + rocketDirection.z : rocketDirection.z;
					rocketDirection.x = xDamper;
					rocketDirection.y = yDamper;
					rocketDirection.z = zDamper;
				}
				if (Mathf.Abs (rocketDirection.x) > .075f || Mathf.Abs (rocketDirection.y) > .075f || Mathf.Abs (rocketDirection.z) > .075f) {
					BounceSound ();
				}

				float xMag = Mathf.Abs (rocketDirection.x);
				float yMag = Mathf.Abs (rocketDirection.y);
				float zMag = Mathf.Abs (rocketDirection.z);

				//Reverse the direction of the projectile
				speedReduction.x = speedReduction.x * xMag * 2;
				speedReduction.y = speedReduction.y * yMag * 2;
				speedReduction.z = speedReduction.z * zMag * 2;
				rocketDirection.x += speedReduction.x;
				rocketDirection.y += speedReduction.y;
				rocketDirection.z += speedReduction.z;

				if (projectileType == 4) {
					//					if (hit.normal.y > 0f) {
					BounceCount++;
					//					}
					//					if (BounceCount > 0) {
					//						teleportFlag = true;
					//					}
				}

			} else if (hit.transform.CompareTag (tagPlayer) || hit.transform.CompareTag (tagEnemy)) {
				if (hit.transform.GetComponent<PhotonView> ().viewID != shooterPhotonID) {
					if (projectileType == 2) {

						if (shooterTag.Equals (tagPlayer)) {
							directHitOverride = true;
							fxManager.isProjectileDirectHit = true;
						}

						grenadeTimer = 0;
					}
				}
			}
		}
	}

}