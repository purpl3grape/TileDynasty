using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {
	/**
 * * Should keep in mind that idTech's cartisian plane is different to Unity's:
 *   Z axis in idTech is "up/down" but in Unity Z is the local equivalent to
 *   "forward/backward" and Y in Unity is considered "up/down".
*/
	WaitForSeconds waitFor0_1 = new WaitForSeconds (0.1f);
	WaitForSeconds waitFor0_125 = new WaitForSeconds (0.125f);
	WaitForSeconds waitFor3 = new WaitForSeconds (3f);

	KeyBindingManager kMgr;

	Health health;
	Animator anim;
	NetworkManager nManager;
	GuiManager guiManager;
	ControlInputManager cInputManager;
	MatchTimer matchTimer;
	PlayerShooting playerShooting;
	TeamMember teamMember;

	//CTF CAPTURE STUFF
	GameObject RedScoringObject;
	GameObject BlueScoringObject;

	/* Player view stuff */
	Transform playerView;  // Must be a camera

	public PostProcessingBehaviour filters;
	PostProcessingProfile profile;

	public float playerViewYOffset= 0.6f; // The height at which the camera is bound to
	public float xMouseSensitivity= 30.0f;
	public float yMouseSensitivity= 30.0f;

	/* Frame occuring factors */
	public float gravity = 20.0f;
	public float friction = 6;                // Ground friction

	/* Player stuff */
	public float playerHeight = 0f;

	/* Movement stuff */
	public float moveSpeed = 7.0f;  // Ground move speed
	public float runAcceleration = 14;   // Ground accel
	public float runDeacceleration = 10;   // Deacceleration that occurs when running on the ground
	public float airAcceleration = 2.0f;  // Air accel
	public float airDeacceleration = 2.0f;    // Deacceleration experienced when opposite strafing
	public float airControl = 0.3f;  // How precise air control is
	public float sideStrafeAcceleration = 50;   // How fast acceleration occurs to get up to sideStrafeSpeed when side strafing
	public float sideStrafeSpeed = 1;    // What the max speed to generate when side strafing
	public float jumpSpeed = 8.0f;  // The speed at which the character's up axis gains when hitting jump
	public float moveScale = 1.0f;
	public float walkSoundRate = 0.15f;
	public float maxSpeed = 150f;
	public int jumpAttempts = 2;
	float mouseSensitivityValue=0f;

	public bool hasJumped = false;
	public bool isStealthWalk = false;

	public float doubleJumpDeltaTime = 0.25f;
	public bool hasDoubleJumped = false;
	/* Sound stuff */
	public AudioClip[] jumpSounds;
	public AudioClip[] jumpPadSounds;
	public AudioClip[] landSounds;
	public AudioClip[] wallGlideImpactSounds;
	public AudioClip[] walkSounds;
	public AudioClip[] jetSoundLoop;

	//Item Pickups on PlayerMovement script
	[HideInInspector] public string itemPickupPlayerName = string.Empty;
	[HideInInspector] public string itemPickUpName = string.Empty;
	public float jetResource=0;


	/* FPS Stuff */
	public float fpsDisplayRate= 4.0f;  // 4 updates per sec.

	/* Prefabs */
	public GameObject TombPrefab;

	private float frameCount= 0;
	private float dt= 0.0f;
	public float fps= 0.0f;

	private Rigidbody controller;
	private Transform tr;

	// Camera rotationals
	private float rotX= 0.0f;
	private float rotY= 0.0f;

	private Vector3 moveDirection = Vector3.zero;
	private Vector3 moveDirectionNorm = Vector3.zero;
	private Vector3 _playerVelocity = Vector3.zero;
	private float playerTopVelocity = 0.0f;

	// If true then the player is fully on the ground
	private bool grounded= false;
	public bool IsGrounded{
		get{ return grounded; }
	}
	private bool landed= true;

	//If in Air, allow Air jump once. Reset when grounded.
	private bool airjump= 	false;

	// Q3: players can queue the next jump just before he hits the ground
	private bool wishJump= false;

	// Used to display real time friction values
	private float playerFriction = 0.0f;

	// Contains the command the user wishes upon the character
	class Cmd {
		public float forwardmove;
		public float rightmove;
		public float upmove;
	}
	private Cmd cmd; // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)

	/* Player statuses */
	private bool isLavaDamage = false;

	private Vector3 playerSpawnPos;
	private Quaternion playerSpawnRot;

	public Vector3 playerVelocity
	{
		get{
			return _playerVelocity;
		}
		set{
			_playerVelocity = value;
		}
	}

	bool isEnemyDamage=false;

	//CTF Scoring Multiplier State
	private int _CurrentScoringMultiplier=1;
	public int CurrentScoringMultiplier {
		get{ return this._CurrentScoringMultiplier; }
		set{ this._CurrentScoringMultiplier = value; }
	}
	Transform myCamTr;
	public AudioSource PlayerAudioSource, FootStepAudioSource, commAudioSource;
	public AudioClip playerSpawnSound;
	public AudioClip [] commAudioList;
	PhotonView pv;
	public bool isEmpEffected = false;


	void  Start (){

		profile = filters.profile;

		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();

		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		guiManager = nManager.GetComponent<GuiManager> ();
		cInputManager = nManager.GetComponent<ControlInputManager> ();
		matchTimer = nManager.GetComponent<MatchTimer> ();

		controller = GetComponent<Rigidbody>();
		tr = GetComponent<Transform> ();
		anim = GetComponent<Animator>();
		playerShooting = GetComponent<PlayerShooting> ();
		health = GetComponent<Health> ();
		teamMember = GetComponent<TeamMember> ();
		myCamTr = GetComponentInChildren<Camera> ().transform;
		//		PlayerAudioSource = GetComponentInChildren<SoundSource> ().GetComponent<AudioSource> ();
		//		FootStepAudioSource = GetComponentInChildren<FootStepSoundSource> ().GetComponent<AudioSource> ();
		cmd = new Cmd();

		mouseSensitivityValue = cInputManager.m_sens;
		pv = GetComponent<PhotonView> ();
		pv.RPC ("SpawnSoundFX", PhotonTargets.All);

		StartCoroutine (ScanEnemyCollisionRate_Coroutine ());
//		StartCoroutine (EMPEffect_Coroutine ());
		//FX Sound Coroutines
		StartCoroutine(WalkFX_Coroutine());

		playerSmoothingFrames = cInputManager.smoothingframes;

	}


	IEnumerator EMPEffect_Coroutine(){
		while (true) {
			if (isEmpEffected) {
				yield return waitFor3;
				pv.RPC ("EMPEffected", PhotonTargets.All, false);
			}
			yield return waitFor0_1;
		}
	}

	[PunRPC]
	public void EMPEffected(bool val){
		isEmpEffected = val;

		if (val) {
			gravity = 15f;
			moveSpeed = 20f;
			runAcceleration = 40f;
			//			sideStrafeAcceleration = 2.5f;
			//			sideStrafeSpeed = 2.5f;
			airAcceleration = 3f;
			airDeacceleration = 3f;
			sideStrafeAcceleration = 3f;
			sideStrafeSpeed = 3f;
			jumpSpeed = 10f;
			moveScale = 10f;
		} else {
			gravity = 65f;
			moveSpeed = 20f;
			runAcceleration = 40f;
			//			sideStrafeAcceleration = 10f;
			//			sideStrafeSpeed = 10f;
			airAcceleration = 3f;
			airDeacceleration = 3f;
			sideStrafeAcceleration = 3f;
			sideStrafeSpeed = 3f;
			jumpSpeed = 22.5f;
			moveScale = 10f;
		}
	}


	string AimAngle = "AimAngle";
	void adjustVerticalAimAngle(){
		float myAimAngle = 0.0f;
		myAimAngle = myCamTr.rotation.eulerAngles.x <= 90 ? -1 * myCamTr.rotation.eulerAngles.x : 360 - myCamTr.rotation.eulerAngles.x;

		//Set parameter on animator component during runtime
		if (myAimAngle >= 90f)
			myAimAngle = 89f;
		else if (myAimAngle <= -90f)
			myAimAngle = -89f;
		anim.SetFloat(AimAngle, myAimAngle);
	}

	void lockCursorMouseClick(){
		if (kMgr.GetKeyDownPublic(keyType.shoot) && !nManager.SettingsPanel.GetActive() && !nManager.ChatPanel.GetActive()) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	IEnumerator ScanEnemyCollisionRate_Coroutine(){
		while (true) {
			if (isEnemyDamage) {
				yield return waitFor0_1;
				isEnemyDamage = false;
			}
			yield return waitFor0_1;
		}
	}


	float staminaTime = 0f;
	bool isSprinting = false;
	void ChangeSprintStamina(){
		if (isSprinting && playerShooting.selectedAmmoType != 5) {

			if (!isStealthWalk) {
				if (jetResource > 0f)
					jetResource -= Time.fixedDeltaTime * 2.5f;
				if (jetResource < 0f)
					jetResource = 0f;
			}
		} else {			
			if (jetResource < 100f)
				jetResource += Time.fixedDeltaTime * 25f;
			if (jetResource > 100f)
				jetResource = 100f;
		}
	}

	float previousVelocityMagnitude = 0f;
	public bool isWalking=false;
	public IEnumerator WalkFX_Coroutine(){
		while (true) {

			if (!isStealthWalk) {
				previousVelocityMagnitude = _playerVelocity.magnitude;
				if (_playerVelocity.magnitude > 2f && _playerVelocity.magnitude < 12f) {
					if (grounded) {
						if (!wishJump) {
							if (!isWalking) {
								if (playerShooting.selectedAmmoType != 5) {
									if (!hasWalkedSpookedEnemy) {
										pv.RPC (SetHasWalkedSpookedEnemyRPC, PhotonTargets.All, true);
									}
								}
							}
						}
					}
				} else if (_playerVelocity.magnitude >= 12f) {
					if (grounded) {
						if (!wishJump) {
							if (!isWalking) {
								if (playerShooting.selectedAmmoType != 5) {
									if (!hasWalkedSpookedEnemy) {
										pv.RPC (SetHasWalkedSpookedEnemyRPC, PhotonTargets.All, true);
									}
								}
							}
						}
					}
				}
			}
			yield return waitFor0_125;
		}
	}




	private List<float> rotArrayY = new List<float>();
	float rotAverageX = 0F;	
	float playerSmoothingFrames = 0f;

	string inputAxis_Mouse_X = "Mouse X";
	string inputAxis_Joy1_Axis_1 = "Joy1 Axis 1";
	string inputAxis_Joy1_Axis_2 = "Joy1 Axis 2";
	string sector = "SECTOR ";
	string rescuezone = "RESCUE ZONE";
	bool hasDisplayedAreaMessage = false;
	void FixedUpdate (){

		if (!pv.isMine)
			return;

		if (controller.velocity.magnitude > 0) {
			controller.velocity = Vector3.zero;
		}


		//AREA MESSAGE COROUTINE
		if (currentBaseObject == null) {
			if (guiManager.AreaMessageCoroutine != null) {
				StopCoroutine (guiManager.AreaMessageCoroutine);
			}
			if (hasDisplayedAreaMessage) {
				if (guiManager.Msg_Area.CloseMessagePanelCoroutine == null)
					guiManager.Msg_Area.CloseMessagePanelCoroutine = guiManager.Msg_Area.CloseMessagePanel_Coroutine ();
				StartCoroutine (guiManager.Msg_Area.CloseMessagePanelCoroutine);
			}
			hasDisplayedAreaMessage = false;
		} else {
			if (!hasDisplayedAreaMessage) {
				hasDisplayedAreaMessage = true;
				guiManager.AreaMessageCoroutine = currentBaseNumber != 0 ? guiManager.AreaMessage_coroutine (sector + currentBaseNumber) : guiManager.AreaMessage_coroutine (rescuezone);
				StartCoroutine (guiManager.AreaMessageCoroutine);
			}
		}


		adjustVerticalAimAngle ();
		lockCursorMouseClick ();

		//DISABLED FOR:
		//-SETTINGS PANEL
		//-CHAT PANEL
		if (!nManager.SettingsPanel.GetActive () && !nManager.ChatPanel.GetActive () && health.currentHitPoints>0) {
			/* Using the Mouse Senitivity Inputs DIRECTLY from SettingsManager */
			//Zoom_Vs_NoZoom
			mouseSensitivityValue = kMgr.GetKeyPublic (keyType.zoom) == true ? cInputManager.zoom_sens : cInputManager.m_sens;

			if (cInputManager.usePS3Controls) {
				rotY += Input.GetAxisRaw (inputAxis_Mouse_X) * (mouseSensitivityValue / 3f) * 0.1f * Time.timeScale;
			} else {
				rotY += Input.GetAxisRaw (inputAxis_Mouse_X) * (mouseSensitivityValue / 3f) * 0.1f * Time.timeScale;
			}

			//Smoothing over X number of Frames
			rotArrayY.Add (rotY);
			playerSmoothingFrames = cInputManager.smoothingframes;
			if (rotArrayY.Count >= playerSmoothingFrames) {
				rotArrayY.RemoveAt (0);
			}
			for (int i = 0; i < rotArrayY.Count; i++) {
				rotAverageX += rotArrayY [i];
			}
			rotAverageX /= rotArrayY.Count;
			tr.rotation = Quaternion.Euler (0, rotAverageX, 0); // Rotates the collider
		}


		/* Movement, here's the important part */
		detectCollision ();
		GroundCheck ();
		CeilingCheck ();
		QueueJump ();


		if (kMgr.GetKeyPublic (keyType.HostageDisengage)) {
			if (isRecruitingNPC) {
				pv.RPC ("SetIsRecruitingNPC", PhotonTargets.AllBuffered, string.Empty, false, false);
			}
		}


		if (health.currentHitPoints > 0) {

			if (hasWalkedSpookedEnemy) {
				if (walkSpookTime > 5f) {
					walkSpookTime = 0f;
					pv.RPC (SetHasWalkedSpookedEnemyRPC, PhotonTargets.All, false);
				} else {
					walkSpookTime += Time.fixedDeltaTime;
				}
			}
			if (hasJumpSpookedEnemy) {
				if (jumpSpookTime > 5f) {
					jumpSpookTime = 0f;
					pv.RPC (SetHasJumpedSpookedEnemyRPC, PhotonTargets.All, false);
				} else {
					jumpSpookTime += Time.fixedDeltaTime;
				}
			}



//			doubleJumpDeltaTime += Time.fixedDeltaTime;

			//Stealth walk
			if (!nManager.SettingsPanel.GetActive () && !nManager.ChatPanel.GetActive ()) {

				if (kMgr.GetKeyPublic (keyType.stealth) && !kMgr.GetKeyPublic (keyType.sprint)) {
					if (!isStealthWalk) {
						pv.RPC (SetIsStealthRPC, PhotonTargets.All, true);
					}
				} else {
					if (isStealthWalk) {
						pv.RPC (SetIsStealthRPC, PhotonTargets.All, false);
					}
				}
			}

			//Just Give Walking a speed boost. Else should be set to 10 by default.

			if (kMgr.GetKeyPublic (keyType.sprint) && jetResource > 0f && (kMgr.GetKeyPublic (keyType.forward) || kMgr.GetKeyPublic (keyType.back) || kMgr.GetKeyPublic (keyType.left) || kMgr.GetKeyPublic (keyType.right))) {
				if (playerShooting.selectedAmmoType != 5) {
					isSprinting = true;
					ChangeSprintStamina ();
					staminaTime = 0f;
				} else {
					isSprinting = true;
					ChangeSprintStamina ();
				}
				if (fps > 90 && !profile.motionBlur.enabled && QualitySettings.GetQualityLevel () <= 1) {
					//DEFAULT AND TOP RES
					profile.motionBlur.enabled = true;
				} else {
					profile.motionBlur.enabled = false;
				}

				if (playerShooting.selectedAmmoType != 5) {
					moveSpeed = isStealthWalk == true ? 10 : 30f;
				} else {
					moveSpeed = isStealthWalk == true ? 10 : 30f;
				}
			} else if (kMgr.GetKeyPublic (keyType.sprint) && jetResource <= 0f) {
				isSprinting = false;

				if (playerShooting.selectedAmmoType == 5) {
					if (staminaTime < 2.5f) {
						staminaTime += Time.fixedDeltaTime;
					} else {
						ChangeSprintStamina ();
					}
				}

				if (profile.motionBlur.enabled) {
					profile.motionBlur.enabled = false;
				}

				if (playerShooting.selectedAmmoType != 5) {
					moveSpeed = isStealthWalk == true ? 10 : 20f;
				} else {
					moveSpeed = isStealthWalk == true ? 10 : 30f;
				}
			} else if (!kMgr.GetKeyPublic (keyType.sprint) && jetResource <= 100f) {
				isSprinting = false;
				if (staminaTime < 1f) {
					staminaTime += Time.fixedDeltaTime;
				} else {
					ChangeSprintStamina ();
				}

				if (profile.motionBlur.enabled) {
					profile.motionBlur.enabled = false;
				}

				if (playerShooting.selectedAmmoType != 5) {
					moveSpeed = isStealthWalk == true ? 10 : 20f;
				} else {
					moveSpeed = isStealthWalk == true ? 10 : 30f;
				}
			}


			if (grounded) {
				anim.SetBool (animJump, false);
				if (landed == false && !isWalking) {
//					pv.RPC ("LandFX", PhotonTargets.All);
					landed = true;
				}
				hasJumped = false;
				GroundMove ();
			} else if (!grounded) {
				anim.SetBool (animJump, true);
				moveSpeed = 10f;
				AirMove ();
			}

			LadderMove ();

			//FUNCTIONS THAT DO NOT DEPEND ON:
			//-CHAT PANEL
			//-SETTINGS PANEL

			speedLimiter ();
		} else {
			//DEAD SO JUST DROP WITH GRAVITY
			_playerVelocity.y -= gravity * Time.deltaTime;
			detectCollision ();
			CeilingCheck ();
			GroundCheck ();
			if (grounded) {
				ApplyFriction (.5f);
			}
		}

		// Finally Move the controller
		controller.MovePosition(controller.position + _playerVelocity * Time.fixedDeltaTime);

		/* Calculate top velocity */
		Vector3 udp= _playerVelocity;
		udp.y = 0.0f;
		if(_playerVelocity.magnitude > playerTopVelocity)
			playerTopVelocity = _playerVelocity.magnitude;

	}

	//JET RESOURCE USAGE
	bool isUseJet=false;
	bool isUseWallJet=false;
	float currentJetResource = 0f;
	int intervals = 0;
	float rateJetUsage = .5f;
	float rateJetRefuel = .1f;
	float rateJet;
	IEnumerator useJetResource_Coroutine(){
		while (true) {
			rateJet = rateJetUsage;
			if (isUseJet || isUseWallJet) {
				jetResource -= 10;
				intervals = 0;
				if (jetResource < 0) {
					jetResource = 0;
				}
			} else {
				if (intervals == 0) {
					currentJetResource = jetResource;
				}
				if (intervals > 4) {
					//					intervals = 0;
					if (!(jetResource < currentJetResource)) {
						rateJet = rateJetRefuel;
						jetResource = jetResource >= 100f ? 100f : jetResource += 1f;
						if (jetResource >= 100)
							rateJet = rateJetUsage;
					}
				} else {
					intervals++;
				}
			}
			yield return new WaitForSeconds (rateJet);
		}
	}


	/*******************************************************************************************************\
|* MOVEMENT
\*******************************************************************************************************/
	//0=Ice


	public bool knockBackOverride = false;

	[PunRPC] public void SetKnockBackOverride(bool val, Vector3 newVelocity){
		knockBackOverride = val;
		_playerVelocity = newVelocity;
	}

	int scannedFloorType=-1;
	Ray ray0, ray1, ray2, ray3, ray4;
	RaycastHit groundHit;
	Transform groundHitTransform;
	string groundHitTransformName;
	int nonItemLayerMask = ~(1 << 10);	//Do not hit itemLayer
	void GroundCheck(){

		if (knockBackOverride) {
			grounded = false;
			return;
		}

		ray0 = new Ray (tr.position, - tr.up);
		ray1 = new Ray (tr.position + tr.forward, - tr.up);
		ray2 = new Ray (tr.position - tr.forward, - tr.up);
		ray3 = new Ray (tr.position + tr.right, - tr.up);
		ray4 = new Ray (tr.position - tr.right, - tr.up);
		//DO NOT WANT NOT BEING ABLE TO JUMP UNEXPECTEDLY. MUST RAYCAST AT LEAST >= PLAYER HEIGHT=4
		if (Physics.Raycast (ray0, out groundHit, 2f + .1f, nonItemLayerMask)) {
			groundHitTransform = groundHit.transform;
			groundHitTransformName = groundHitTransform.name;
			if (!groundHit.collider.transform.CompareTag (HeadObjectName)) {
				grounded = true;
			}
			if (groundHitTransform.name.Equals(IceFloorObjectName)) {
				scannedFloorType = groundHitTransform.GetComponent<SpecialFloor> ().floorType;
			} else {
				scannedFloorType = -1;
			}
			if (groundHitTransform.CompareTag (JumpPadObjectName)) {
				grounded = false;
				_playerVelocity.y = groundHitTransform.GetComponent<JumpPadBehavior> ().thrust;
				controller.MovePosition (controller.position + _playerVelocity * Time.deltaTime);
				pv.RPC ("playJumpPadSound", PhotonTargets.All);
			}
		} else if (Physics.Raycast (ray1, out groundHit, 2f + .1f, nonItemLayerMask)) {
			groundHitTransform = groundHit.transform;
			groundHitTransformName = groundHitTransform.name;
			if (!groundHit.collider.transform.CompareTag (HeadObjectName)) {
				grounded = true;
			}
		} else if (Physics.Raycast (ray2, out groundHit, 2f + .1f, nonItemLayerMask)) {
			groundHitTransform = groundHit.transform;
			groundHitTransformName = groundHitTransform.name;
			if (!groundHit.collider.transform.CompareTag (HeadObjectName)) {
				grounded = true;
			}
		} else if (Physics.Raycast (ray3, out groundHit, 2f + .1f, nonItemLayerMask)) {
			groundHitTransform = groundHit.transform;
			groundHitTransformName = groundHitTransform.name;
			if (!groundHit.collider.transform.CompareTag (HeadObjectName)) {
				grounded = true;
			}
		} else if (Physics.Raycast (ray4, out groundHit, 2f + .1f, nonItemLayerMask)) {
			groundHitTransform = groundHit.transform;
			groundHitTransformName = groundHitTransform.name;
			if (!groundHit.collider.transform.CompareTag (HeadObjectName)) {
				grounded = true;
			}
		} else {
			scannedFloorType = -1;
			grounded = false;
			if (playerVelocity.y < -100f) {
				landed = false;
			}
		}
	}

	bool isHitCeiling = false;
	Ray ray5;
	RaycastHit hitCeiling;
	void CeilingCheck(){
		ray5 = new Ray (tr.position, tr.up);

		//DO NOT WANT NOT BEING ABLE TO JUMP UNEXPECTEDLY. MUST RAYCAST AT LEAST >= PLAYER HEIGHT=4
		if (Physics.Raycast (ray5, out hitCeiling, 4f + .1f, nonItemLayerMask)) {
			if (hitCeiling.collider.transform.CompareTag (HeadObjectName)) {
				return;
			}
			isHitCeiling = true;
			_playerVelocity.y = 0;
		} else {
			isHitCeiling = false;
		}
	}

	/**
 * Sets the movement direction based on player input
 */
	//set the animation of (run forward/strafe left/strafe right) here. We have an animation parameter of type Float, called "HorizontalMovement"

	string animVerticalMovement = "VerticalMovement";
	string animHorizontalMovement = "HorizontalMovement";
	string animJump = "Jump";
	void  SetMovementDir (){

		if (nManager.SettingsPanel.GetActive () || nManager.ChatPanel.GetActive ()) {
			cmd.forwardmove = 0;
			cmd.rightmove = 0;
			anim.SetFloat (animVerticalMovement, 0);
			anim.SetFloat (animHorizontalMovement, 0);
			return;
		}



		if (!cInputManager.usePS3Controls) {
			if (kMgr.GetKeyPublic (keyType.forward)) {
				cmd.forwardmove = 1;
				anim.SetFloat (animVerticalMovement, 1);
			} else if (kMgr.GetKeyPublic (keyType.back)) {
				anim.SetFloat (animVerticalMovement, 1);
				cmd.forwardmove = -1;
			} else {
				cmd.forwardmove = 0;
				anim.SetFloat (animVerticalMovement, 0);
			}
			if (kMgr.GetKeyPublic (keyType.right)) {
				cmd.rightmove = 1;
				anim.SetFloat (animHorizontalMovement, 1);
			} else if (kMgr.GetKeyPublic (keyType.left)) {
				cmd.rightmove = -1;
				anim.SetFloat (animHorizontalMovement, 1);
			} else {
				cmd.rightmove = 0;
				anim.SetFloat (animHorizontalMovement, 0);
				//NOT MOVING, BUT IF WE HAVE SMALL FRICTION VALUE, MAY NEED TO CONTINUE THE ANIMATION
				if (_playerVelocity.magnitude > 1f) {
					anim.SetFloat (animVerticalMovement, 1);		
				}
			}
		} else {
			if (Input.GetAxisRaw (inputAxis_Joy1_Axis_1) != 0) {
				cmd.forwardmove = Input.GetAxisRaw (inputAxis_Joy1_Axis_1);
				anim.SetFloat (animVerticalMovement, 1);
			} else {
				cmd.forwardmove = 0;
				anim.SetFloat (animVerticalMovement, 0);
			}

			if (Input.GetAxisRaw (inputAxis_Joy1_Axis_2) != 0) {
				cmd.rightmove = Input.GetAxisRaw (inputAxis_Joy1_Axis_2);
				anim.SetFloat (animHorizontalMovement, 1);
			} else {
				cmd.rightmove = 0;
				anim.SetFloat (animHorizontalMovement, 0);
				//NOT MOVING, BUT IF WE HAVE SMALL FRICTION VALUE, MAY NEED TO CONTINUE THE ANIMATION
				if (_playerVelocity.magnitude > 1f) {
					anim.SetFloat (animVerticalMovement, 1);		
				}
			}
		}


		//or initially
		//cmd.forwardmove = Input.GetAxis("Vertical");
		//cmd.rightmove   = Input.GetAxis("Horizontal");
	}

	/**
 * Queues the next jump just like in Q3
 */
	void  QueueJump (){

		if (nManager.SettingsPanel.GetActive () || nManager.ChatPanel.GetActive ())
			return;


		if (kMgr.GetKeyPublic(keyType.jump)) {
			wishJump = true;
		} else {
			wishJump = false;
		}
	}

	/**
 * Execs when the player is in the air
 */
	void  AirMove (){
		Vector3 wishdir;
		float wishvel = airAcceleration;
		float accel;

		float scale = CmdScale();

		if (isSprinting) {
			moveSpeed = 20;
		} else {
			moveSpeed = 10;
		}

		knockBackOverride = false;

		SetMovementDir();

		wishdir = new Vector3(cmd.rightmove, 0, cmd.forwardmove);
		wishdir = tr.TransformDirection(wishdir);

		float wishspeed= wishdir.magnitude;
		wishspeed *= moveSpeed;

		wishdir.Normalize();
		moveDirectionNorm = wishdir;
		wishspeed *= scale;

		// CPM: Aircontrol
		float wishspeed2= wishspeed;
		if(Vector3.Dot(_playerVelocity, wishdir) < 0)
			accel = airDeacceleration;
		else
			accel = airAcceleration;
		// If the player is ONLY strafing left or right
		if(cmd.forwardmove == 0 && cmd.rightmove != 0)
		{
			if(wishspeed > sideStrafeSpeed)
				wishspeed = sideStrafeSpeed;
			accel = sideStrafeAcceleration;
		}

		Accelerate(wishdir, wishspeed, accel);
		if(airControl > 0){
			AirControl(wishdir, wishspeed2);
		}// !CPM: Aircontrol

		//		if (!airjump) {
		//			if (kMgr.GetKeyDownPublic(keyType.jump)) {
		//				_playerVelocity.y = jumpSpeed;
		//				pv.RPC("JumpFX",PhotonTargets.All, _playerVelocity.magnitude);
		//				airjump = true;
		//			}
		//		}

		// Apply gravity
		_playerVelocity.y -= gravity * Time.deltaTime;
	}

	void  JetMove (){
		if ((kMgr.GetKeyPublic(keyType.stealth) || Input.GetMouseButton (2)) && jetResource > 0) {
			if (PlayerAudioSource.clip != jetSoundLoop [0]) {
				if (!isUseJet) {
					pv.RPC ("JetFXLoop", PhotonTargets.All, true);
					isUseJet = true;
				}
			}
			_playerVelocity.y = _playerVelocity.y > 10f ? 10f : _playerVelocity.y + 10f;
			//			_playerVelocity.y += 10f;
			isLavaDamage = false;
		} else {
			if (PlayerAudioSource.clip == jetSoundLoop [0] && PlayerAudioSource.isPlaying) {
				if (isUseJet) {
					pv.RPC ("JetFXLoop", PhotonTargets.All, false);
					isUseJet = false;
				}
			}
		}

	}

	void LadderMove(){
		if (kMgr.GetKeyPublic (keyType.jump) && isOnLadder) {
			_playerVelocity.y = _playerVelocity.y > 10f ? 10f : _playerVelocity.y + 10f;
		}
	}

	void WallMove (){


		Vector3 wishdir;
		float wishvel = airAcceleration;
		float accel;

		float scale = CmdScale();

		SetMovementDir();

		wishdir = new Vector3(cmd.rightmove, 0, cmd.forwardmove);
		wishdir = tr.TransformDirection(wishdir);

		float wishspeed= wishdir.magnitude;
		wishspeed *= moveSpeed;

		wishdir.Normalize();
		moveDirectionNorm = wishdir;
		wishspeed *= scale;

		// CPM: Aircontrol
		float wishspeed2= wishspeed;
		if(Vector3.Dot(_playerVelocity, wishdir) < 0)
			accel = airDeacceleration;
		else
			accel = airAcceleration;
		// If the player is ONLY strafing left or right
		if(cmd.forwardmove == 0 && cmd.rightmove != 0)
		{
			if(wishspeed > sideStrafeSpeed)
				wishspeed = sideStrafeSpeed;
			accel = sideStrafeAcceleration;
		}

		Accelerate(wishdir, wishspeed, accel);
		if(airControl > 0){
			AirControl(wishdir, wishspeed2);
		}// !CPM: Aircontrol



		if (kMgr.GetKeyPublic(keyType.jump)) {
			wallJump = true;
		} else {
			wallJump = false;
		}



		if (wallJump) {
			_playerVelocity.y = jumpSpeed;
			CeilingCheck ();
			isLavaDamage = false;
			wallJump = false;
			landed = false;
		} else {
			if (_playerVelocity.y > 0.0f) {
				_playerVelocity.y -= gravity * Time.deltaTime;
				if (_playerVelocity.y < 0.0f) {
					_playerVelocity.y = 0f;
				}
			} else {
				_playerVelocity.y = 0.0f;
			}
		}

		// Apply gravity
		//		_playerVelocity.y -= gravity * Time.deltaTime;
	}


	/**
 * Air control occurs when the player is in the air, it allows
 * players to move side to side much faster rather than being
 * 'sluggish' when it comes to cornering.
 */
	void  AirControl ( Vector3 wishdir ,   float wishspeed  ){
		float zspeed;
		float speed;
		float dot;
		float k;
		int i;

		// Can't control movement if not moving forward or backward
		if(cmd.forwardmove == 0 || wishspeed == 0)
			return;

		zspeed = _playerVelocity.y;
		_playerVelocity.y = 0;
		/* Next two lines are equivalent to idTech's VectorNormalize() */
		speed = _playerVelocity.magnitude;
		_playerVelocity.Normalize();

		dot = Vector3.Dot(_playerVelocity, wishdir);
		k = 32;
		k *= airControl * dot * dot * Time.deltaTime;

		// Change direction while slowing down
		if(dot > 0)
		{
			_playerVelocity.x = _playerVelocity.x * speed + wishdir.x * k;
			_playerVelocity.y = _playerVelocity.y * speed + wishdir.y * k;
			_playerVelocity.z = _playerVelocity.z * speed + wishdir.z * k;

			_playerVelocity.Normalize();
			moveDirectionNorm = _playerVelocity;
		}

		_playerVelocity.x *= speed;
		_playerVelocity.y = zspeed; // Note this line
		_playerVelocity.z *= speed;

	}

	/**
 * Called every frame when the engine detects that the player is on the ground
 */
	void  GroundMove (){
		Vector3 wishdir;
		Vector3 wishvel;


		//set airjump to false again to allow an Extra Jump in Airmove()
		airjump = false;

		// Do not apply friction if the player is queueing up the next jump
		if (!wishJump) {
			if (scannedFloorType == 1) {
				ApplyFriction (0f);
			} else {
				if (isEmpEffected) {
					ApplyFriction (0.25f);
				} else {
					ApplyFriction (1f);
				}
			}
			//			if (_playerVelocity.x < -maxSpeed/2) {
			//				_playerVelocity.x = -maxSpeed/2;
			//			}
			//			else if (_playerVelocity.x > maxSpeed/2) {
			//				_playerVelocity.x = maxSpeed/2;
			//			}
			//			if (_playerVelocity.z < -maxSpeed/2) {
			//				_playerVelocity.z = -maxSpeed/2;
			//			}
			//			else if (_playerVelocity.z > maxSpeed/2) {
			//				_playerVelocity.z = maxSpeed/2;
			//			}
		} else {
			ApplyFriction (0);
		}

		float scale = CmdScale();

		SetMovementDir();

		wishdir = new Vector3(cmd.rightmove, 0, cmd.forwardmove);
		wishdir = tr.TransformDirection(wishdir);
		wishdir.Normalize();
		moveDirectionNorm = wishdir;
		float wishspeed= wishdir.magnitude;
		wishspeed *= moveSpeed;

		Accelerate(wishdir, wishspeed, runAcceleration);

		// Reset the gravity velocity		
		_playerVelocity.y = 0;

		//SingleJump
		if(wishJump && !isHitCeiling)
		{
			if (!hasJumpSpookedEnemy && matchTimer.isReady) {
				pv.RPC (SetHasJumpedSpookedEnemyRPC, PhotonTargets.All, true);
			}
//			if (!hasDoubleJumped && kMgr.GetKeyPublic (keyType.sprint) && jetResource > 0f) {
			if (kMgr.GetKeyPublic (keyType.sprint) && jetResource > 0f) {
//				_playerVelocity.y = jumpSpeed * 2f;
				_playerVelocity.y = jumpSpeed;
				hasDoubleJumped = true;
			} else {
				hasJumped = true;
				_playerVelocity.y = jumpSpeed;
				hasDoubleJumped = false;
			}
			isLavaDamage = false;
			wishJump = false;

			landed = false;
//			pv.RPC("JumpFX",PhotonTargets.All, _playerVelocity.magnitude);
//			doubleJumpDeltaTime = 0f;
		}

	}

	/**
 * Applies friction to the player, called in both the air and on the ground
 */
	public void  ApplyFriction ( float t  ){
		Vector3 vec = _playerVelocity; // Equivalent to: VectorCopy();
		float vel;
		float speed;
		float newspeed;
		float control;
		float drop;

		vec.y = 0.0f;
		speed = vec.magnitude;
		drop = 0.0f;

		/* Only if the player is on the ground then apply friction */
		if(grounded || health.isPlayerDead)
		{
			control = speed < runDeacceleration ? runDeacceleration : speed;
			drop = control * friction * Time.deltaTime * t;
		}

		newspeed = speed - drop;
		playerFriction = newspeed;
		if(newspeed < 0)
			newspeed = 0;
		if(speed > 0)
			newspeed /= speed;

		_playerVelocity.x *= newspeed;
		// playerVelocity.y *= newspeed;
		_playerVelocity.z *= newspeed;
	}


	/**
 * Calculates wish acceleration based on player's cmd wishes i.e. WASD movement
 */
	void  Accelerate ( Vector3 wishdir ,   float wishspeed ,   float accel  ){
		float addspeed;
		float accelspeed;
		float currentspeed;

		if (_playerVelocity.magnitude >= 100) {
			return;
		}


		currentspeed = Vector3.Dot(_playerVelocity, wishdir);
		addspeed = wishspeed - currentspeed;
		if(addspeed <= 0)
			return;
		accelspeed = accel * Time.deltaTime * wishspeed;
		if(accelspeed > addspeed)
			accelspeed = addspeed;

		_playerVelocity.x += accelspeed * wishdir.x;
		_playerVelocity.z += accelspeed * wishdir.z;

	}




	void  Update (){
		/* Do FPS calculation */
		frameCount++;
		dt += Time.deltaTime;
		if(dt > 1.0f/fpsDisplayRate)
		{
			fps = Mathf.Round(frameCount / dt);
			frameCount = 0;
			dt -= 1.0f/fpsDisplayRate;
		}
		calculateItemPickupPlayerMovement ();
	}





	/*
============
PM_CmdScale
Returns the scale factor to apply to cmd movements
This allows the clients to use axial -127 to 127 values for all directions
without getting a sqrt(2) distortion in speed.
============
*/
	float  CmdScale (){
		int max;
		float total;
		float scale;

		max = (int)Mathf.Abs(cmd.forwardmove);

		if(Mathf.Abs(cmd.rightmove) > max)
			max = (int)Mathf.Abs(cmd.rightmove);
		if(max == 0){
			return 0;
		}
		total = Mathf.Sqrt(cmd.forwardmove * cmd.forwardmove + cmd.rightmove * cmd.rightmove);
		scale = moveSpeed * max / (moveScale * total);

		return scale;
	}

	Ray ray6;
	Vector3 speedReduction = Vector3.zero;
	void detectCollision(){

		ray6 = new Ray (tr.position, _playerVelocity);
		RaycastHit hit;
		speedReduction = Vector3.zero;

		if (Physics.Raycast (ray6, out hit, 2f)) {

			if(hit.transform.CompareTag(LevelObjectName)){

				speedReduction = hit.normal;

				float xMag = Mathf.Abs(_playerVelocity.x);
				float yMag = Mathf.Abs(_playerVelocity.y);
				float zMag = Mathf.Abs(_playerVelocity.z);

				speedReduction.x = speedReduction.x * xMag * 1.1f;
				speedReduction.y = speedReduction.y * yMag * 1.1f;
				speedReduction.z = speedReduction.z * zMag * 1.1f;

				_playerVelocity.x += speedReduction.x;
				_playerVelocity.y += speedReduction.y;
				_playerVelocity.z += speedReduction.z;

			}
		}

	}


	Ray ray7,ray8,ray9,ray10;
	bool wallCollide=false;
	bool wallJump=false;
	void detectWallCollision(){

		//Do not do wall Jump from here. Do it on WallMove()
		ray7 = new Ray (tr.position, tr.right);
		ray8 = new Ray (tr.position, -tr.right);
		ray9 = new Ray (tr.position, tr.forward);
		ray10 = new Ray (tr.position, -tr.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray7, out hit, 2f)) {
			if (hit.transform.CompareTag (LevelObjectName)) {
				wallCollide = true;
			} else {
				wallCollide = false;
			}
		} else if (Physics.Raycast (ray8, out hit, 2f)) {
			if (hit.transform.CompareTag (LevelObjectName)) {
				wallCollide = true;
			} else {
				wallCollide = false;
			}
		} else if (Physics.Raycast (ray9, out hit, 2f)) {
			if (hit.transform.CompareTag (LevelObjectName)) {
				wallCollide = true;
			} else {
				wallCollide = false;
			}
		} else if (Physics.Raycast (ray10, out hit, 2f)) {
			if (hit.transform.CompareTag (LevelObjectName)) {
				wallCollide = true;
			} else {
				wallCollide = false;
			}
		} else {
			wallCollide = false;
		}

		if (wallCollide && jetResource > 0) {
			isUseWallJet = true;
		} else {
			isUseWallJet = false;
		}

	}


	void speedLimiter(){

		if (teamMember.classID == 1) {
			float halfSpeed = Mathf.Round (maxSpeed / 2);

			if (_playerVelocity.x < -halfSpeed) {
				_playerVelocity.x = -halfSpeed;
			} else if (_playerVelocity.x > halfSpeed) {
				_playerVelocity.x = halfSpeed;
			}
			if (_playerVelocity.z < -halfSpeed) {
				_playerVelocity.z = -halfSpeed;
			} else if (_playerVelocity.z > halfSpeed) {
				_playerVelocity.z = halfSpeed;
			}
		}

		if (teamMember.classID == 2) {
			if (_playerVelocity.x < -maxSpeed / 2) {
				_playerVelocity.x = -maxSpeed / 2;
			}
			if (_playerVelocity.x > maxSpeed / 2) {
				_playerVelocity.x = maxSpeed / 2;
			}
			if (_playerVelocity.z < -maxSpeed / 2) {
				_playerVelocity.z = -maxSpeed / 2;
			}
			if (_playerVelocity.z > maxSpeed / 2) {
				_playerVelocity.z = maxSpeed / 2;
			}
		}
	}


	string strSurvivor = "SURVIVOR ";
	string strLocated = " LOCATED";
	string strReturnToBase = "RETURN TO BASE";
	string strEliminateTargets = "ELIMINATE ENEMY TARGETS";
	[PunRPC]
	public void SetIsRecruitingNPC(string npcName, bool val, bool isKillThresholdMet){
		NPCName = npcName;
		isRecruitingNPC = val;

		if (val) {
			commAudioSource.PlayOneShot (commAudioList [0]);

			if (guiManager.NpcMsgCO != null) {
				StopCoroutine (guiManager.NpcMsgCO);
			}
			if (isKillThresholdMet) {
				guiManager.NpcMsgCO = guiManager.NpcMsg_CO (2, strSurvivor + NPCName.ToUpper() + strLocated, strReturnToBase);
			} else {
				guiManager.NpcMsgCO = guiManager.NpcMsg_CO (2, strSurvivor + NPCName.ToUpper() + strLocated, strReturnToBase);
			}
			StartCoroutine (guiManager.NpcMsgCO);
		} else {
			if (npcName != string.Empty) {
				//CURRENTLY HAS ANOTHER HOSTAGE BEING RESCUED
				commAudioSource.PlayOneShot (commAudioList [1]);
			}
		}
	}

	[PunRPC]
	public void SetRescuedHostage(){		
		//		if (isRecruitingNPC)
		//			commAudioSource.PlayOneShot (commAudioList [2]);
		commAudioSource.PlayOneShot (commAudioList [2]);
		//		NPCName = string.Empty;
		//		isRecruitingNPC = false;
	}

	//*********************************************************************************************************************
	//ENEMY COLLISION//
	//*********************************************************************************************************************

	private string JumpPadObjectName = "JumpPad";
	private string HeadObjectName = "Head";
	private string IceFloorObjectName = "Ice_Floor";
	private string LevelObjectName = "Level";
	private string HostageObjectName = "Hostage";
	public string NPCName=string.Empty;
	public string CivilianName=string.Empty;
	public bool isRecruitingNPC = false;


	//JETPACK PICKUP MOVEMENT

	[PunRPC]
	public void setItemPickupPlayerNamePlayerMovement(string str, string myItemName){
		itemPickupPlayerName = str;
		itemPickUpName = myItemName;
	}

	void calculateItemPickupPlayerMovement(){
		if (itemPickupPlayerName.Equals (teamMember.GetPlayerName ())) {
			if (itemPickUpName.StartsWith ("Jet")) {
				jetResource += 100;
				if (jetResource >= 100) {
					jetResource = 100;
				}
			}
			itemPickupPlayerName = string.Empty;
			itemPickUpName = string.Empty;
		}
	}

	bool isOnLadder=false;


	string ladderTag = "Ladder";
	string baseTag = "Base";
	void OnTriggerExit(Collider other){
		string itemName = other.gameObject.name;
		if (other.CompareTag (ladderTag)) {
			isOnLadder = false;
		}
		if (other.CompareTag (baseTag)) {
			currentBaseObject = null;	
		}
	}

	public GameObject currentBaseObject;
	public int currentBaseNumber;
	void OnTriggerEnter(Collider other){
		string itemName = other.gameObject.name;

		if (other.CompareTag (ladderTag)) {
			isOnLadder = true;	
		}
		if (other.CompareTag (baseTag)) {
			currentBaseObject = other.gameObject;
			currentBaseNumber = currentBaseObject.GetComponent<EnemySpawnTrigger> ().baseNumber;
		}

//		if (itemName.StartsWith("Jet")) {
//			pv.RPC ("setItemPickupPlayerNamePlayerMovement", PhotonTargets.AllBuffered, teamMember.GetPlayerName (), itemName);
//			if (itemPickupPlayerName != string.Empty && itemPickUpName != string.Empty) {
//				other.GetComponentInParent<PhotonView> ().RPC ("PickupItem", PhotonTargets.AllBuffered, itemName);
//			}
//		}
	}

	/*
============
RPC Sound Effects
- Create an RPC method to load and play the Sound FX
- Call the RPC for ALL Photon Targets, source coming from the player's transform
- Jumping Sound FX dependent on playerVelocity.magnitude
============
*/
	[PunRPC]
	void JetFXLoop(bool val){
		if (val) {
			if (!PlayerAudioSource.clip == jetSoundLoop [0]) {
				PlayerAudioSource.Stop ();							
				PlayerAudioSource.loop = false;
				PlayerAudioSource.clip = jetSoundLoop [0];
			}
			if (PlayerAudioSource.clip == jetSoundLoop [0] && !PlayerAudioSource.isPlaying) {
				PlayerAudioSource.Play ();
				PlayerAudioSource.loop = true;
			}
		} else {
			if (PlayerAudioSource.clip == jetSoundLoop [0] && PlayerAudioSource.isPlaying) {
				PlayerAudioSource.clip = null;
				PlayerAudioSource.Stop ();
				PlayerAudioSource.loop = false;
			}
		}
	}

	[PunRPC]
	void JumpFX(float playerVelocityMagnitude){
		int jumpSoundElement = 0;
		if (jumpSounds == null) {
			Debug.Log ("Missing \"Jump Sounds\" on PlayerController");
			return;
		}
		if (PlayerAudioSource != null && jumpSounds [0] != null) {
			PlayerAudioSource.Stop ();
			PlayerAudioSource.loop = false;
			PlayerAudioSource.PlayOneShot (jumpSounds [0]);
		}
	}

	[PunRPC]
	void  LandFX (){
		if (landSounds [0] != null && PlayerAudioSource != null)
			PlayerAudioSource.PlayOneShot (landSounds [0]);
	}

	[PunRPC]
	void SpawnSoundFX(){
		PlayerAudioSource.PlayOneShot (playerSpawnSound);
	}


	[PunRPC]
	void WallCollideFX(int soundIndex){
		if (wallGlideImpactSounds [soundIndex] != null && PlayerAudioSource != null)
			PlayerAudioSource.PlayOneShot (wallGlideImpactSounds [soundIndex]);
	}

	[PunRPC]
	void FootStepAudioSourceEnabler(bool val){
		FootStepAudioSource.enabled = val;
	}


	[PunRPC]
	public void WalkFX (bool val, int walksoundIndex){

		isWalking = val;

		if (FootStepAudioSource == null || walkSounds [0] == null)
			return;

		if (val) {
			if (FootStepAudioSource.clip != walkSounds [walksoundIndex]) {
				FootStepAudioSource.Stop ();							
				FootStepAudioSource.loop = false;
				FootStepAudioSource.clip = walkSounds [walksoundIndex];
			}
			if (FootStepAudioSource.clip == walkSounds [walksoundIndex] && !FootStepAudioSource.isPlaying) {
				FootStepAudioSource.Play ();
				FootStepAudioSource.loop = true;
			}
		} else {
			if ((FootStepAudioSource.clip == walkSounds [0]||FootStepAudioSource.clip == walkSounds [1]) && FootStepAudioSource.isPlaying) {
				FootStepAudioSource.clip = null;
				FootStepAudioSource.Stop ();
				FootStepAudioSource.loop = false;
			}
		}
	}

	string SetHasJumpedSpookedEnemyRPC = "SetHasJumpedSpookedEnemy";
	float jumpSpookTime=0f;
	public bool hasJumpSpookedEnemy = false;
	[PunRPC]
	void SetHasJumpedSpookedEnemy(bool val){
		hasJumpSpookedEnemy = val;
	}

	string SetHasWalkedSpookedEnemyRPC = "SetHasWalkedSpookedEnemy";
	float walkSpookTime=0f;
	public bool hasWalkedSpookedEnemy = false;
	[PunRPC]
	void SetHasWalkedSpookedEnemy(bool val){
		hasWalkedSpookedEnemy = val;
	}

	string SetIsStealthRPC = "SetIsStealth";
	[PunRPC]
	void SetIsStealth(bool val){
		isStealthWalk = val;
	}


	[PunRPC]
	void playJumpPadSound(){
		if (PlayerAudioSource != null && jumpPadSounds [0] != null)
			PlayerAudioSource.PlayOneShot (jumpPadSounds [0]);
	}


	public Vector3 getPlayerVelocity(){
		return _playerVelocity;
	}

}