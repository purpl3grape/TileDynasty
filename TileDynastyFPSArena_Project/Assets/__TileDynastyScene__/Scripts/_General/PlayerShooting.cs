using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

	public ExitGames.Client.Photon.Hashtable PlayerScoreTable;

	KeyBindingManager kMgr;

	//TeamID
	private int teamID = 0;

	//FOV
	public float normalFOV=100f;
	public float zoomFOV=60f;

	//Firing Rate
	public float regGunFireRate = 0f;
	public float lightningGunFireRate = 0f;
	public float rocketLauncherFireRate = 0f;
	public float grenadeLauncherFireRate = 0f;
	public float railGunFireRate = 0f;
	public float axeFireRate = 0f;

	//Reload Time
	public float porterReloadTIme = 0f;// lightningGunFireRate / 2f;
	public float lightningGunReloadTime = 0f;// lightningGunFireRate / 2f;
	public float rocketLauncherReloadTime = 0f; //lightningGunFireRate / 20f;
	public float grenadeLauncherReloadTime = 0f; //lightningGunFireRate / 20f;
	public float railGunReloadTime = 0f;
	public float axeReloadTime = 0f;

	float cooldown = 0;

	public float regDamage = 4f;
	public float quadDamage = 16f;
	public float rocketDamage = 50f;
	public float grenadeDamage = 50f;
	public float railDamage = 80f;
	public float axeDamage = 200f;

	//Ammo Stuff
	public float regAmmo = 5f;
	public float quadAmmo = 0f;
	public float rocketAmmo = 10f;
	public float grenadeAmmo = 10f;
	public float railAmmo = 10f;
	public float axeAmmo = 1f;

	public float selectedAmmoType = 0;
	/*/ 0= RegAmmo, 1= QuadAmmo 2= RocketAmmo 3=RailAmmo 4=GrenadeAmmo 5= AxeAmmo/*/

	public bool regAmmoSel = false;
	public bool quadAmmoSel = false;
	public bool rocketAmmoSel = false;
	public bool grenadeAmmoSel = false;
	public bool railAmmoSel = false;
	public bool axeAmmoSel = false;

	bool weapMGObtained = false;
	bool weapHMGObtained = false;
	bool weapRocketObtained = false;
	bool weapGrenadeObtained = false;
	bool weapRailObtained = false;
	bool weapAxeObtained = true;	//ALWAYS TRUE BY DEFAULT. PLAYER SHALL START WITH THIS WEAPON

	public bool isPlayerShooting = false;

	//PlayerResources
	public float repairResource = 10f;

	public bool isLightningOn=false;

	FXManager fxManager;
	NetworkManager nManager;
	ControlInputManager cInputManager;
	GuiManager guiManager;
	ScoringManager scoringManager;
	MatchTimer matchTimer;
	PlayerMovement playerMovement;
	PlayerGuiManager playerGuiManger;

	TeamMember teamMember;
	PhotonView sceneScriptsPhotonView;
	PhotonView playerPhotonView;


	public rocketData gunPositionData;
	Transform gunPositionDataTransform;

	float shootingAnimationTime = 0.5f;

	public GameObject gunMG_LocalModel;
	public GameObject gunHMG_LocalModel;
	public GameObject gunRocket_LocalModel;
	public GameObject gunGrenade_LocalModel;
	public GameObject gunRail_LocalModel;
	public GameObject meleeAxe_LocalModel;

	public Animator cannonAnimator;
	public Animator grenadeLauncherAnimator;
	public Animator mgAnimator;
	public Animator hmgAnimator;
	public Animator railAnimator;
	public Animator axeAnimator;
	public Animator playerAnimator;

	//AUDIO FX can be stored on player game object for now. Since there's no need to destroy this particular object.
	public AudioClip[] Hit_confirm;
	public AudioClip[] quadHit_confirm;
	public AudioClip[] axeHit_confirm;
	public AudioClip[] rocketHit_confirm;
	public AudioClip[] GunLaserSounds;
	public AudioClip[] Ammo_pickup;
	public AudioClip[] quadAmmo_pickup;
	public AudioClip[] rocketAmmo_pickup;
	public AudioClip[] grenadeAmmo_pickup;
	public AudioClip[] railAmmo_pickup;
	public AudioClip[] axeAmmo_pickup;
	public AudioClip[] MGSET_Sound;
	public AudioClip[] switchWeaponSounds;

	public float ammoPickupType = 0;

	//Item Pickups on PlayerShooting script
	[HideInInspector] public string itemPickupPlayerName = string.Empty;
	[HideInInspector] public string itemPickUpName = string.Empty;

	public GameObject LgRPC;
	Transform LgRPC_Target;

	private bool gameStartFlag = false;

	[PunRPC]
	public void SetLGActive(bool activeState){
		LgRPC.SetActive (activeState);
	}
	[PunRPC]
	public void SetLGScale(float scaleWidth){
		//LgRPC.GetComponent<LightningBolt> ().scale = scaleWidth;
	}

	public AudioSource PlayerAudioSource;
	public Transform playerCameraTransform;

	[PunRPC]
	public void PlayLightningGunSounds(bool val){
		if (val) {
			if (regAmmoSel) {
				PlayerAudioSource.clip = GunLaserSounds [0];
			} else if (quadAmmoSel) {
				PlayerAudioSource.clip = GunLaserSounds [1];
			}
			PlayerAudioSource.loop = true;
			PlayerAudioSource.Play ();
		} else {
			PlayerAudioSource.loop = false;
			PlayerAudioSource.Stop ();
		}
	}

	bool hasJumpedArmAnimaition = false;
	Transform tr;

	string enemyTag = "Enemy";
	string hostageTag = "Hostage";
	string playerTag = "Player";
	string takeDamageRPC = "TakeDamage";
	string changeScoreRPC = "ChangeScore";

	void Start(){

		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();

		gameStartFlag = false;

		teamMember = GetComponent<TeamMember> ();
		teamID = teamMember.teamID;

		fxManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<FXManager> ();
		nManager = fxManager.GetComponent<NetworkManager> ();
		cInputManager = fxManager.GetComponent<ControlInputManager> ();
		guiManager = fxManager.GetComponent<GuiManager> ();
		playerInGamePanelInputs = guiManager.PlayerInGamePanel.GetComponent<PlayerInGamePanel> ();
		scoringManager = fxManager.GetComponent<ScoringManager> ();
		sceneScriptsPhotonView = scoringManager.GetComponent<PhotonView> ();
		matchTimer = fxManager.GetComponent<MatchTimer> ();

		playerMovement = GetComponent<PlayerMovement> ();
		playerGuiManger = GetComponent<PlayerGuiManager> ();
		playerPhotonView = GetComponent<PhotonView> ();

		fxManager.SetLocalClient (playerPhotonView);

		tr = GetComponent<Transform> ();
		gunPositionDataTransform = gunPositionData.transform;

		ResetWeaponAmmoAndSelection (0);
		playerInGamePanelInputs.doNormalCrossHair ();

		HMGFiredCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shotfired_hmgStat);
		RocketFiredCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shotfired_rocketStat);
		RailFiredCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shotfired_railStat);
		EMPFiredCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shotfired_empStat);

		HMGHitCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shothit_hmgStat);
		RocketHitCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shothit_rocketStat);
		RailHitCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shothit_railStat);
		EMPHitCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shothit_empStat);

		MeleeHitCount = scoringManager.GetScore (teamMember.GetPlayerName (), scoringManager.shothit_meleeStat);
		HeadShotCount = scoringManager.GetScore (teamMember.GetPlayerName (), scoringManager.shothit_headshotStat);

		DamageDealtCount = scoringManager.GetScore (teamMember.playerName, scoringManager.damagedealtStat);
//		PlayerScoreTable = new ExitGames.Client.Photon.Hashtable () { 
//			{ DamageDealtProp, 0 }
//		};

	}


	void SwitchPorterUpdate(bool overrideButton){
		if (kMgr.GetKeyUpPublic (keyType.switchMGAmmo) && weapMGObtained || overrideButton) {
			if (cooldown > 0)
				return;
			gunRocket_LocalModel.SetActive (false);
			gunGrenade_LocalModel.SetActive (false);
			gunMG_LocalModel.SetActive (true);
			gunHMG_LocalModel.SetActive (false);
			gunRail_LocalModel.SetActive (false);
			meleeAxe_LocalModel.SetActive (false);

			regAmmoSel = true;
			quadAmmoSel = false;
			rocketAmmoSel = false;
			grenadeAmmoSel = false;
			railAmmoSel = false;
			axeAmmoSel = false;
			playerPhotonView.RPC (WeaponSwitchFxRPC, PhotonTargets.All);
			selectedAmmoType = 0;
		}
	}
	void SwitchQuadUpdate(bool overrideButton){
		if (kMgr.GetKeyUpPublic (keyType.switchHMGAmmo) && weapHMGObtained || overrideButton) {
			if (cooldown > 0)
				return;

			gunRocket_LocalModel.SetActive (false);
			gunGrenade_LocalModel.SetActive (false);
			gunMG_LocalModel.SetActive (false);
			gunHMG_LocalModel.SetActive (true);
			gunRail_LocalModel.SetActive (false);
			meleeAxe_LocalModel.SetActive (false);

			quadAmmoSel = true;
			regAmmoSel = false;
			rocketAmmoSel = false;
			grenadeAmmoSel = false;
			railAmmoSel = false;
			axeAmmoSel = false;
			playerPhotonView.RPC (WeaponSwitchFxRPC, PhotonTargets.All);
			selectedAmmoType = 1;
		}
	}
	void SwitchRocketUpdate(bool overrideButton){
		if (kMgr.GetKeyUpPublic (keyType.switchRocketAmmo) && weapRocketObtained || overrideButton) {
			if (cooldown > 0)
				return;
			gunRocket_LocalModel.SetActive (true);
			gunGrenade_LocalModel.SetActive (false);
			gunMG_LocalModel.SetActive (false);
			gunHMG_LocalModel.SetActive (false);
			gunRail_LocalModel.SetActive (false);
			meleeAxe_LocalModel.SetActive (false);

			quadAmmoSel = false;
			regAmmoSel = false;
			rocketAmmoSel = true;
			grenadeAmmoSel = false;
			railAmmoSel = false;
			axeAmmoSel = false;
			playerPhotonView.RPC (WeaponSwitchFxRPC, PhotonTargets.All);
			selectedAmmoType = 2;
		}
	}

	void SwitchRailUpdate(bool overrideButton){
		if (kMgr.GetKeyUpPublic (keyType.switchRailAmmo) && weapRailObtained || overrideButton) {
			if (cooldown > 0)
				return;

			gunRocket_LocalModel.SetActive (false);
			gunGrenade_LocalModel.SetActive (false);
			gunMG_LocalModel.SetActive (false);
			gunHMG_LocalModel.SetActive (false);
			gunRail_LocalModel.SetActive (true);
			meleeAxe_LocalModel.SetActive (false);

			quadAmmoSel = false;
			regAmmoSel = false;
			rocketAmmoSel = false;
			grenadeAmmoSel = false;
			railAmmoSel = true;
			axeAmmoSel = false;
			playerPhotonView.RPC (WeaponSwitchFxRPC, PhotonTargets.All);
			selectedAmmoType = 3;

		}
	}
	void SwitchGrenadeUpdate(bool overrideButton){
		if (kMgr.GetKeyUpPublic (keyType.switchGrenadeAmmo) && weapGrenadeObtained || overrideButton) {
			if (cooldown > 0)
				return;

			gunRocket_LocalModel.SetActive (false);
			gunGrenade_LocalModel.SetActive (true);
			gunMG_LocalModel.SetActive (false);
			gunHMG_LocalModel.SetActive (false);
			gunRail_LocalModel.SetActive (false);
			meleeAxe_LocalModel.SetActive (false);

			quadAmmoSel = false;
			regAmmoSel = false;
			rocketAmmoSel = false;
			grenadeAmmoSel = true;
			railAmmoSel = false;
			axeAmmoSel = false;
			playerPhotonView.RPC (WeaponSwitchFxRPC, PhotonTargets.All);
			selectedAmmoType = 4;
		}
	}

	void SwitchAxeUpdate(bool overrideButton){
		if (kMgr.GetKeyUpPublic (keyType.switchAxeAmmo) && weapAxeObtained || overrideButton) {
			if (cooldown > 0)
				return;

			gunRocket_LocalModel.SetActive (false);
			gunGrenade_LocalModel.SetActive (false);
			gunMG_LocalModel.SetActive (false);
			gunHMG_LocalModel.SetActive (false);
			gunRail_LocalModel.SetActive (false);
			meleeAxe_LocalModel.SetActive (true);

			quadAmmoSel = false;
			regAmmoSel = false;
			rocketAmmoSel = false;
			grenadeAmmoSel = false;
			railAmmoSel = false;
			axeAmmoSel = true;
			playerPhotonView.RPC (WeaponSwitchFxRPC, PhotonTargets.All);
			selectedAmmoType = 5;

		}
	}

	void Update (){

		if (!playerPhotonView.isMine)
			return;

		//Do Key or Mouse Input Stuff Here i.e, switching weapon keys, maybe firing later. To get better input response.
		//Selecting Ammunition. 'E' Button, switches to QuadAmmo and 'Q' Button, switches to regAmmo
		if (guiManager.GAMESTART == true && gameStartFlag == false) {
			ResetWeaponAmmoAndSelection(0);
			gameStartFlag = true;
		}

		if (nManager.ChatPanel.GetActive () || nManager.SettingsPanel.GetActive () || GetComponent<Health>().currentHitPoints<=0)
			return;

		if (matchTimer.READYCOUNTDOWN)
			return;

		switchNextWeap ();
		switchPreviousWeap ();

		calculateItemPickupPlayerShooting ();

		SwitchPorterUpdate (false);
		SwitchQuadUpdate (false);
		SwitchRocketUpdate (false);
		SwitchGrenadeUpdate (false);
		SwitchRailUpdate (false);
		SwitchAxeUpdate (false);
	}


	string armAnim_Porter_Status_Int = "Porter_Status_Int";
	string armAnim_Rocket_Status_Int = "Rocket_Status_Int";
	string armAnim_Grenade_Status_Int = "Grenade_Status_Int";
	string armAnim_Rail_Status_Int = "Rail_Status_Int";
	string armAnim_HMG_Status_Int = "HMG_Status_Int";
	string armAnim_Axe_Status_Int = "Axe_Status_Int";
	string strGunStatus = "GunStatus";
	string strHealth = "Health_";
	string strArmor = "Armor_";
	string strAmmo = "Ammo_";


	[HideInInspector] public int HMGFiredCount = 0;
	[HideInInspector] public int RocketFiredCount = 0;
	[HideInInspector] public int RailFiredCount = 0;
	[HideInInspector] public int EMPFiredCount = 0;
	[HideInInspector] public int HMGHitCount = 0;
	[HideInInspector] public int RocketHitCount = 0;
	[HideInInspector] public int RailHitCount = 0;
	[HideInInspector] public int EMPHitCount = 0;
	[HideInInspector] public int BullsEyeCount = 0;
	[HideInInspector] public int HeadShotCount = 0;
	[HideInInspector] public int MeleeHitCount = 0;

	int oldRocketHitCount = 0;
	int oldEMPHitCount = 0;
	float isShootingTime = 0f;
	bool hasHitCrossHair = false;
	float hitCrossHairTime = 0f;
	PlayerInGamePanel playerInGamePanelInputs;

	Ray ray0;
	RaycastHit hit;

	Transform myHitTr;
	string SetIsPlayerShootingRPC = "SetIsPlayerShooting";
	void FixedUpdate () {

		if (!playerPhotonView.isMine)
			return;

		if (nManager.ChatPanel.GetActive () || nManager.SettingsPanel.GetActive () || GetComponent<Health>().currentHitPoints<=0)
			return;

		if (matchTimer.READYCOUNTDOWN)
			return;


		//ZOOM NON-MELEE (GUN ZOOM ONLY)
		if (playerCameraTransform.gameObject.GetActive ()) {
			if (selectedAmmoType != 5) {				

				if (kMgr.GetKeyPublic (keyType.zoom)) {
					if (Camera.main) {
						if (Camera.main.fieldOfView != cInputManager.zoom_fov) {
							Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, cInputManager.zoom_fov, 8f * Time.deltaTime);
						}
					}
				} else {
					if (Camera.main) {
						if (Camera.main.fieldOfView != cInputManager.fov) {
							Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, cInputManager.fov, 8f * Time.deltaTime);
						}
					}
				}

			}

		}

		//CrossHair Reset back to normal
		if (playerInGamePanelInputs == null) {
			playerInGamePanelInputs = guiManager.PlayerInGamePanel.GetComponent<PlayerInGamePanel> ();
		}
		if (hasHitCrossHair) {
			if (hitCrossHairTime > 0.25f) {
				playerInGamePanelInputs.doNormalCrossHair ();
				hitCrossHairTime = 0f;
				hasHitCrossHair = false;
			} else {
				hitCrossHairTime += Time.fixedDeltaTime;
			}
		}


		if (isPlayerShooting) {
			if (isShootingTime > 10f) {
				//				isShootingTime = 0f;
				playerPhotonView.RPC (SetIsPlayerShootingRPC, PhotonTargets.AllBuffered, false);
			} else {
				isShootingTime += Time.fixedDeltaTime;
			}
		}
		//Using the Weapons
		if (regAmmoSel == true) {
			useRegWeap ();
		} else if (quadAmmoSel == true) {
			useQuadWeap ();
		} else if (rocketAmmoSel == true) {
			useRocketWeap ();
		} else if (grenadeAmmoSel == true) {
			useGrenadeWeap ();
		} else if (railAmmoSel == true) {
			useRailWeap ();
		} else if (axeAmmoSel == true) {
			useAxeWeap ();
		}
			
		if (oldRocketHitCount != scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shothit_rocketStat) || 
			oldEMPHitCount != scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shothit_empStat)) {

			oldRocketHitCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shothit_rocketStat);
			oldEMPHitCount = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.shothit_empStat);
			//HitConfirm Sound for Projectiles
			DoProjectileHitFX (tr.position);

			//(Crosshair Changes Color if hit enemy)
			if (playerInGamePanelInputs == null) {
				playerInGamePanelInputs = guiManager.PlayerInGamePanel.GetComponent<PlayerInGamePanel> ();
			}
			playerInGamePanelInputs.doHitCrossHair ();
			hasHitCrossHair = true;
		}

		if ((kMgr.GetKeyPublic(keyType.switchMGAmmo) || kMgr.GetKeyPublic(keyType.switchHMGAmmo) || kMgr.GetKeyPublic(keyType.switchRocketAmmo) || kMgr.GetKeyPublic(keyType.switchGrenadeAmmo) || kMgr.GetKeyPublic(keyType.switchRailAmmo) || kMgr.GetKeyPublic (keyType.switchAxeAmmo) || kMgr.GetKeyPublic (keyType.switchNextWeap) || kMgr.GetKeyPublic (keyType.switchPreviousWeap))) {
			//ARM DRAW
			if (selectedAmmoType == 0)
				mgAnimator.SetInteger (armAnim_Porter_Status_Int, 1);
			else if (selectedAmmoType == 1)
				hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 1);
			else if (selectedAmmoType == 2)
				cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 1);
			else if (selectedAmmoType == 3)
				railAnimator.SetInteger (armAnim_Rail_Status_Int, 1);
			else if (selectedAmmoType == 4)
				grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 1);
			else if (selectedAmmoType == 5)
				axeAnimator.SetInteger (armAnim_Axe_Status_Int, 1);
		} else {
			if (kMgr.GetKeyPublic (keyType.shoot)) {
				if (kMgr.GetKeyPublic (keyType.zoom)) {
					//ARM SHOOT ZOOM
					if (selectedAmmoType == 0 && regAmmo > 0)
						mgAnimator.SetInteger (armAnim_Porter_Status_Int, 7);
					else if(selectedAmmoType == 0 && regAmmo <= 0)
						mgAnimator.SetInteger (armAnim_Porter_Status_Int, 6);
					else if (selectedAmmoType == 1 && quadAmmo > 0)
						hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 7);
					else if (selectedAmmoType == 1 && quadAmmo <= 0)
						hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 6);
					else if (selectedAmmoType == 2 && rocketAmmo > 0)
						cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 7);
					else if (selectedAmmoType == 2 && rocketAmmo <= 0)
						cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 6);
					else if (selectedAmmoType == 3 && railAmmo > 0)
						railAnimator.SetInteger (armAnim_Rail_Status_Int, 7);
					else if (selectedAmmoType == 3 && railAmmo <= 0)
						railAnimator.SetInteger (armAnim_Rail_Status_Int, 6);
					else if (selectedAmmoType == 4 && grenadeAmmo > 0)
						grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 7);
					else if (selectedAmmoType == 4 && grenadeAmmo <= 0)
						grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 6);

					//AXE (WILL DO THE ATTACK 1 IN THIS SITUATION, SO NO NEED FOR 'AIM-FIRE' ANIMATION FOR AXE)

				}
				else {
					//ARM SHOOT
					if (selectedAmmoType == 0 && regAmmo > 0)
						mgAnimator.SetInteger (armAnim_Porter_Status_Int, 3);
					if (selectedAmmoType == 0 && regAmmo <= 0)
						mgAnimator.SetInteger (armAnim_Porter_Status_Int, 2);
					else if (selectedAmmoType == 1 && quadAmmo > 0)
						hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 3);
					else if (selectedAmmoType == 1 && quadAmmo <= 0)
						hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 2);
					else if (selectedAmmoType == 2 && rocketAmmo > 0)
						cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 3);
					else if (selectedAmmoType == 2 && rocketAmmo <= 0)
						cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 2);
					else if (selectedAmmoType == 3 && railAmmo > 0)
						railAnimator.SetInteger (armAnim_Rail_Status_Int, 3);
					else if (selectedAmmoType == 3 && railAmmo <= 0)
						railAnimator.SetInteger (armAnim_Rail_Status_Int, 2);
					else if (selectedAmmoType == 4 && grenadeAmmo > 0)
						grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 3);
					else if (selectedAmmoType == 4 && grenadeAmmo <= 0)
						grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 2);
					else if (selectedAmmoType == 5 && axeAmmo > 0) {
						if (!hasSwungAxe) {
							axeAnimator.SetInteger (armAnim_Axe_Status_Int, 3);
							hasSwungAxe = true;
						}
					} else if (selectedAmmoType == 5 && axeAmmo <= 0) {
						axeAnimator.SetInteger (armAnim_Axe_Status_Int, 2);
					}
				}
			} else if (kMgr.GetKeyPublic (keyType.jump)) {
				//ARM JUMP
				if (!hasJumpedArmAnimaition) {
					if (selectedAmmoType == 0)
						mgAnimator.SetInteger (armAnim_Porter_Status_Int, 5);
					else if (selectedAmmoType == 1)
						hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 5);
					else if (selectedAmmoType == 2)
						cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 5);
					else if (selectedAmmoType == 3)
						railAnimator.SetInteger (armAnim_Rail_Status_Int, 5);
					else if (selectedAmmoType == 4)
						grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 5);
					else if (selectedAmmoType == 5)
						axeAnimator.SetInteger (armAnim_Axe_Status_Int, 5);

					hasJumpedArmAnimaition = true;
				} else {
					/*WHILE IN AIR*/
					if (kMgr.GetKeyPublic (keyType.zoom)) {
						//ARM AIR ZOOM
						if (selectedAmmoType == 0)
							mgAnimator.SetInteger (armAnim_Porter_Status_Int, 6);
						else if (selectedAmmoType == 1)
							hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 6);
						else if (selectedAmmoType == 2)
							cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 6);
						else if (selectedAmmoType == 3)
							railAnimator.SetInteger (armAnim_Rail_Status_Int, 6);
						else if (selectedAmmoType == 4)
							grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 6);
						else if (selectedAmmoType == 5) {
							if (!hasSwungAxe) {
								axeAnimator.SetInteger (armAnim_Axe_Status_Int, 6);	//THIS IS THE AXE ATTACK 2 ANIMATION
								hasSwungAxe=true;
							}					
						}
					} else {

						if (kMgr.GetKeyPublic (keyType.sprint) && !kMgr.GetKeyPublic(keyType.stealth) && playerMovement.jetResource > 0f) {
							//ARM AIR MOVE RUN (SAME AS RUN)
							if (selectedAmmoType == 0)
								mgAnimator.SetInteger (armAnim_Porter_Status_Int, 8);
							else if (selectedAmmoType == 1)
								hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 8);
							else if (selectedAmmoType == 2)
								cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 8);
							else if (selectedAmmoType == 3)
								railAnimator.SetInteger (armAnim_Rail_Status_Int, 8);
							else if (selectedAmmoType == 4)
								grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 8);
							else if (selectedAmmoType == 5)
								axeAnimator.SetInteger (armAnim_Axe_Status_Int, 8);
						} else {
							//ARM AIR MOVE (SAME AS WALK)
							if (selectedAmmoType == 0)
								mgAnimator.SetInteger (armAnim_Porter_Status_Int, 4);
							else if (selectedAmmoType == 1)
								hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 4);
							else if (selectedAmmoType == 2)
								cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 4);
							else if (selectedAmmoType == 3)
								railAnimator.SetInteger (armAnim_Rail_Status_Int, 4);
							else if (selectedAmmoType == 4)
								grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 4);
							else if (selectedAmmoType == 5)
								axeAnimator.SetInteger (armAnim_Axe_Status_Int, 4);

						}
					}
				}
			} else if (kMgr.GetKeyPublic (keyType.left) || kMgr.GetKeyPublic (keyType.right) || kMgr.GetKeyPublic (keyType.forward) || kMgr.GetKeyPublic (keyType.back)) {
				if (kMgr.GetKeyPublic (keyType.zoom)) {
					//ARM WALK ZOOM
					if (selectedAmmoType == 0)
						mgAnimator.SetInteger (armAnim_Porter_Status_Int, 6);
					else if (selectedAmmoType == 1)
						hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 6);
					else if (selectedAmmoType == 2)
						cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 6);
					else if (selectedAmmoType == 3)
						railAnimator.SetInteger (armAnim_Rail_Status_Int, 6);
					else if (selectedAmmoType == 4)
						grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 6);
					else if (selectedAmmoType == 5) {
						if (!hasSwungAxe) {
							axeAnimator.SetInteger (armAnim_Axe_Status_Int, 6);
						}
					}

				} else {

					if (kMgr.GetKeyPublic (keyType.sprint) && !kMgr.GetKeyPublic(keyType.stealth) && playerMovement.jetResource > 0f) {
						//ARM RUN
						if (selectedAmmoType == 0)
							mgAnimator.SetInteger (armAnim_Porter_Status_Int, 8);
						else if (selectedAmmoType == 1)
							hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 8);
						else if (selectedAmmoType == 2)
							cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 8);
						else if (selectedAmmoType == 3)
							railAnimator.SetInteger (armAnim_Rail_Status_Int, 8);
						else if (selectedAmmoType == 4)
							grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 8);
						else if (selectedAmmoType == 5)
							axeAnimator.SetInteger (armAnim_Axe_Status_Int, 8);

					} else {
						//ARM WALK
						if (selectedAmmoType == 0)
							mgAnimator.SetInteger (armAnim_Porter_Status_Int, 4);
						else if (selectedAmmoType == 1)
							hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 4);
						else if (selectedAmmoType == 2)
							cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 4);
						else if (selectedAmmoType == 3)
							railAnimator.SetInteger (armAnim_Rail_Status_Int, 4);
						else if (selectedAmmoType == 4)
							grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 4);
						else if (selectedAmmoType == 5)
							axeAnimator.SetInteger (armAnim_Axe_Status_Int, 4);
					}
				}
			} else {
				if (kMgr.GetKeyPublic (keyType.zoom)) {
					//ARM IDLE ZOOM
					if (selectedAmmoType == 0)
						mgAnimator.SetInteger (armAnim_Porter_Status_Int, 6);
					else if (selectedAmmoType == 1)
						hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 6);
					else if (selectedAmmoType == 2)
						cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 6);
					else if (selectedAmmoType == 3)
						railAnimator.SetInteger (armAnim_Rail_Status_Int, 6);
					else if (selectedAmmoType == 4)
						grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 6);
					else if (selectedAmmoType == 5) {
						if (!hasSwungAxe) {
							axeAnimator.SetInteger (armAnim_Axe_Status_Int, 6);
						}
					}
				} else {				
					//ARM IDLE
					if (selectedAmmoType == 0)
						mgAnimator.SetInteger (armAnim_Porter_Status_Int, 2);
					else if (selectedAmmoType == 1)
						hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 2);
					else if (selectedAmmoType == 2)
						cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 2);
					else if (selectedAmmoType == 3)
						railAnimator.SetInteger (armAnim_Rail_Status_Int, 2);
					else if (selectedAmmoType == 4)
						grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 2);
					else if (selectedAmmoType == 5)
						axeAnimator.SetInteger (armAnim_Axe_Status_Int, 2);
				}
			}

			ray0 = new Ray (tr.position, - tr.up);
			//DO NOT WANT NOT BEING ABLE TO JUMP UNEXPECTEDLY. MUST RAYCAST AT LEAST >= PLAYER HEIGHT=4
			if (Physics.Raycast (ray0, out hit, 2f + .5f)) {
				myHitTr = hit.transform;
				if (!myHitTr.name.StartsWith (strHealth) && !myHitTr.name.StartsWith (strArmor) && !myHitTr.name.StartsWith (strAmmo)) {
					hasJumpedArmAnimaition = false;
				}
			}


		}


		//Networked Animation
		if (kMgr.GetKeyPublic (keyType.shoot) && selectedAmmoType != 5) {
			playerAnimator.SetInteger (strGunStatus, 1);
		} else if (!kMgr.GetKeyPublic (keyType.shoot) && selectedAmmoType != 5) {
			playerAnimator.SetInteger (strGunStatus, 0);
		} else if (kMgr.GetKeyPublic (keyType.shoot) && selectedAmmoType == 5) {
			playerAnimator.SetInteger (strGunStatus, 3);
		} else if (!kMgr.GetKeyPublic (keyType.shoot) && selectedAmmoType == 5) {
			playerAnimator.SetInteger (strGunStatus, 2);
		}

	}


	/******************************************************
	 * SHOOTING A WEAPON: OVERALL SEQUENCE OF EVENTS HAPPENING IN PLAYER SHOOTING
	 * 
	 * [1] USE WEAPON TYPE
	 * [2] FIRE WEAPON TYPE
	 * 	[2.1] APPLY DAMAGE
	 * 	[2.2] APPLY FX (Visual / Audio)
	 * 
	******************************************************/

	/*
	* SECTION 1
	*/

	void useRegWeap(){
		//Main Weapon Recharge Time... Very Fucking Fast
		if (cooldown >= 0)
			cooldown -= Time.fixedDeltaTime * regGunFireRate;

		if (kMgr.GetKeyPublic (keyType.shoot)) {
			if (regAmmo > 0f) {
				regAmmo = FireWeaponType (selectedAmmoType, regAmmo, regDamage, regGunFireRate, porterReloadTIme);
			} else {
				mgAnimator.SetInteger (armAnim_Porter_Status_Int, 2);
			}
		}
	}

	void useQuadWeap(){
		//Quad Weapon Recharge Time... Very Fucking Fast
		if (cooldown >= 0) {
			cooldown -= Time.fixedDeltaTime * lightningGunFireRate;
		}

		if(kMgr.GetKeyPublic(keyType.shoot)){
			if (quadAmmo > 0f) {
				quadAmmo = FireWeaponType (selectedAmmoType, quadAmmo, quadDamage, lightningGunFireRate, lightningGunReloadTime);
			} else {
				hmgAnimator.SetInteger (armAnim_HMG_Status_Int, 2);
			}
		}
	}

	void useRocketWeap(){
		if (cooldown >= 0)
			cooldown -= Time.fixedDeltaTime * rocketLauncherFireRate;	

		if(kMgr.GetKeyPublic(keyType.shoot)){
			if(rocketAmmo > 0f){
				rocketAmmo = FireWeaponType (selectedAmmoType, rocketAmmo, rocketDamage, rocketLauncherFireRate, rocketLauncherReloadTime);	
			} else {
				cannonAnimator.SetInteger (armAnim_Rocket_Status_Int, 2);
			}
		}
	}

	void useGrenadeWeap(){
		if (cooldown >= 0)
			cooldown -= Time.fixedDeltaTime * grenadeLauncherFireRate;

		if(kMgr.GetKeyPublic(keyType.shoot)){
			if(grenadeAmmo > 0f){
				grenadeAmmo = FireWeaponType (selectedAmmoType, grenadeAmmo, grenadeDamage, grenadeLauncherFireRate, grenadeLauncherReloadTime);	
			} else {
				grenadeLauncherAnimator.SetInteger (armAnim_Grenade_Status_Int, 2);
			}
		}
	}

	void useRailWeap(){
		if (cooldown >= 0)
			cooldown -= Time.fixedDeltaTime * railGunFireRate;

		if(kMgr.GetKeyPublic(keyType.shoot)){
			//Player wants to shoot. so shoot. If there is ammo remaining
			if(railAmmo > 0f){
				railAmmo = FireWeaponType (selectedAmmoType, railAmmo, railDamage, railGunFireRate, railGunReloadTime);	
			} else {
				railAnimator.SetInteger (armAnim_Rail_Status_Int, 2);
			}
		}
	}

	bool hasSwungAxe = false;
	void useAxeWeap(){
		if (cooldown >= 0) {
			cooldown -= Time.fixedDeltaTime * axeFireRate;

			if (cooldown < 0) {
				if (hasSwungAxe)
					hasSwungAxe = false;
			}
		}
		if (Camera.main) {
			if (Camera.main.fieldOfView != cInputManager.fov)
				Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, cInputManager.fov, 8f * Time.deltaTime);
		}

		if (kMgr.GetKeyPublic (keyType.shoot)) {
			//Player wants to shoot. so shoot. If there is ammo remaining
			if (axeAmmo > 0f) {
				axeAmmo = FireWeaponType (selectedAmmoType, axeAmmo, axeDamage, axeFireRate, axeReloadTime);	
			} else {
				axeAnimator.SetInteger (armAnim_Axe_Status_Int, 2);
			}

		} else if (kMgr.GetKeyPublic (keyType.zoom)) {			
			//AXE ATTACK 2
			if (axeAmmo > 0f) {
				axeAmmo = FireWeaponType (selectedAmmoType, axeAmmo, axeDamage, axeFireRate, axeReloadTime);
			} else {
				axeAnimator.SetInteger (armAnim_Axe_Status_Int, 2);
			}
		}
	}

	/*
	 * SECTION 2
	 */


	Ray ray;
	Transform hitTransform;
	Vector3 hitPoint;
	string CannonShootBool = "CannonShootBool";
	//Firing Beam, RETURN AMMO REMAINING AFTER FIRING
	float FireWeaponType(float ammoType, float typeOfAmmoRemaining, float damageAmmount, float weaponFireRate, float weaponReloadTime){

		if(cooldown > 0){
			//cooldown -= weaponFireRate * weaponReloadTime;
			return typeOfAmmoRemaining;
		}

		//Shooting Animation
		if (cannonAnimator != null || hmgAnimator != null || mgAnimator != null || railAnimator !=null || axeAnimator !=null) {
			if (ammoType == 2) {
				cannonAnimator.SetBool (CannonShootBool, true);
			}
			if (ammoType == 0) {
				mgAnimator.SetBool (CannonShootBool, true);
			}
			if (ammoType == 1) {
				hmgAnimator.SetBool (CannonShootBool, true);
			}
			if (ammoType == 4) {
				grenadeLauncherAnimator.SetBool (CannonShootBool, true);
			}
			if (ammoType == 3) {
				railAnimator.SetBool (CannonShootBool, true);
			}
			if (ammoType == 5) {
				axeAnimator.SetBool (CannonShootBool, true);
			}
		}

		if (ammoType != 5) {
			typeOfAmmoRemaining -= 1f;

			BulletFired(ammoType, true);
			if (!isPlayerShooting) {
				playerPhotonView.RPC (SetIsPlayerShootingRPC, PhotonTargets.AllBuffered, true);
			}
			isShootingTime = 0f;
		}

		ray = new Ray (playerCameraTransform.position, playerCameraTransform.forward);

		//		hitTransform;
		hitPoint = playerCameraTransform.forward;

		//Gather the Closest Game Objects the player's RayCast is cast upon
		hitTransform = FindClosestHitObject (ray, out hitPoint, ammoType);

		//PROJECTILE WEAPON METHOD SECTION BELOW (ammoTypes:=2)
		if (ammoType == 2) {
			//Make a blast Explosion
			//AFTER ALL THAT, Want to make the explosion physics.
			DoRocketBulletFx (hitPoint, this.teamID, playerPhotonView.viewID, gameObject.tag);
		} else if (ammoType == 4) {
			//Make a blast Explosion
			//AFTER ALL THAT, Want to make the explosion physics.
			DoGrenadeBulletFx (hitPoint, this.teamID, playerPhotonView.viewID,gameObject.tag);
		} else if (ammoType == 0) {
			DoTeleportFx (hitPoint, this.teamID, playerPhotonView.viewID,gameObject.tag);
		}

		/** Raycast (ammoTypes:=1,3,5) **/
		else{
			//RAY CAST METHOD SECTION BELOW


			//Check Hit or Miss
			if (hitTransform != null) {
				Health h = hitTransform.GetComponent<Health> ();

				if (isHeadShot) {
					if (h) {
						if (((int)h.currentHitPoints > 0 && hitTransform.CompareTag (playerTag) || ((int)h.enemyHitPoints > 0 && (hitTransform.CompareTag (enemyTag) || hitTransform.CompareTag (hostageTag))))) {
							damageAmmount = damageAmmount * 2f;
							//shothit_headshot
							//							sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), "shothit_headshot", 1);
							HeadShotCount += 1;
							scoringManager.SetScore (teamMember.GetPlayerName (), scoringManager.shothit_headshotStat, HeadShotCount);
							//					Debug.Log ("HeadShot: " + scoringManager.GetScore (PhotonNetwork.player.name, "shothit_headshot"));
							isHeadShot = false;
						}
					}
				}

				//There EXISTS a Health Component in the main object or it's children.	
				if (h != null) {
					//					PhotonView pv = h.GetComponent<PhotonView> ();
					if (h.GetComponent<PhotonView> () != null) {
						TeamMember tm = hitTransform.GetComponent<TeamMember> ();
						//						TeamMember myTM = this.teamMember;


						if (h) {
							if (((int)h.currentHitPoints > 0 && hitTransform.CompareTag (playerTag) || ((int)h.enemyHitPoints > 0 && (hitTransform.CompareTag (enemyTag) || hitTransform.CompareTag (hostageTag))))) {
								applyHitEnemyFX (ammoType, hitPoint, h);						
								applyRayCastDamage (tm, this.teamMember, h, damageAmmount);
							} else {
								applyHitEnemyFX (ammoType, hitPoint, h);						
							}
						} else {
							DoMissShotFX (hitPoint);
							applyHitEnemyFX (ammoType, hitPoint, h);						
						}

					}
				} else {
					//HIT LEVEL OR NOTHING
					if (ammoType != 5) {
						//AXES DO NOT HAVE A MISS SHOT MESH
						DoMissShotFX (hitPoint);
					}
					applyHitEnemyFX (ammoType, hitPoint, h);
				}
				//				applyHitEnemyFX (ammoType, hitPoint, h);
			} else {
				//Test
				//				DoMissShotFX (hitPoint);
				//				Debug.Log ("Missed Shot at: " + raycastHit + ", and player position is: " + tr.position);
				applyHitEmptySpaceFX (ammoType, hitPoint);
			}


		}
		cooldown = weaponReloadTime;
		return typeOfAmmoRemaining;
	}

	/*
	 * SECTION 2.1
	 */

	[HideInInspector]public string DamageDealtProp = "damagedealtprop";
	public int DamageDealtCount = 0;
	void applyRayCastDamage(TeamMember targetTM, TeamMember myTM, Health hp, float dmg){//, ImpactReceiver impact){

		if(targetTM.teamID != myTM.teamID){
			hp.GetComponent<PhotonView> ().RPC (takeDamageRPC, PhotonTargets.AllBuffered, dmg, PhotonNetwork.player.name, playerPhotonView.viewID, gameObject.tag);

			DamageDealtCount = scoringManager.GetScore (teamMember.playerName, scoringManager.damagedealtStat);
			DamageDealtCount += (int)dmg;
			scoringManager.SetScore (teamMember.playerName, scoringManager.damagedealtStat, DamageDealtCount);

//			PlayerScoreTable [DamageDealtProp] = DamageDealtCount + (int)dmg;
//			PhotonNetwork.player.SetCustomProperties (PlayerScoreTable);
			//(Crosshair Changes Color if hit enemy)
			if (playerInGamePanelInputs == null) {
				playerInGamePanelInputs = guiManager.PlayerInGamePanel.GetComponent<PlayerInGamePanel> ();
			}
			playerInGamePanelInputs.doHitCrossHair ();
			hasHitCrossHair = true;
		}
	}

	/*
	 * SECTION 2.2
	 */

	//Hit Enemy FX
	string BulletFiredRPC = "BulletFired";
	void applyHitEnemyFX(float ammoType, Vector3 hitPoint, Health hComponent){

		if (ammoType == 0) {
			DoRegBulletFX (hitPoint);
		}
		else if(ammoType == 1){
			DoQuadBulletFX(hitPoint);
		}
		else if(ammoType == 2){
			DoRocketBulletFx(hitPoint, this.teamID, playerPhotonView.viewID, gameObject.tag);
		}
		else if(ammoType == 4){
			DoGrenadeBulletFx (hitPoint, this.teamID, playerPhotonView.viewID, gameObject.tag);
		}
		else if(ammoType == 3){
			DoRailBulletFX(hitPoint);
		}
		else if(ammoType == 5){
			DoHitAxeFX (hitPoint);
		}		
		if(hComponent !=null){
			if (hComponent.GetComponent<TeamMember> ().teamID != teamMember.teamID) {
				if ((hComponent.currentHitPoints > 0 && hitTransform.CompareTag (playerTag) || (hComponent.enemyHitPoints > 0 && (hitTransform.CompareTag (enemyTag) || hitTransform.CompareTag (hostageTag))))) {
					DoBulletHitFX (tr.position);
				}
				if (ammoType != 5) {
					//					playerPhotonView.RPC (BulletFiredRPC, PhotonTargets.AllBuffered, ammoType, false);
					BulletFired(ammoType, false);
				} else {
					//					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), "shothit_melee", 1);
					MeleeHitCount += 1;
					scoringManager.SetScore (teamMember.GetPlayerName (), scoringManager.shothit_meleeStat, MeleeHitCount);
				}
			}		
		}
	}



	//Empty Space Miss FX
	void applyHitEmptySpaceFX(float ammoType, Vector3 hitPoint){
		if(fxManager != null){

			if (ammoType != 5) {
				hitPoint = playerCameraTransform.position + (playerCameraTransform.forward * 100f);
			}


			if (ammoType == 0) {
				//ammoType = 0, RegBullet
				DoRegBulletFX (hitPoint);
			} else if (ammoType == 1) {
				//ammoType = 1, QuadBullet
				DoQuadBulletFX (hitPoint);
			} else if (ammoType == 2) {
				//ammoType = 1, QuadBullet
				DoRocketBulletFx (hitPoint, this.teamID, playerPhotonView.viewID, gameObject.tag);
			} else if (ammoType == 4) {
				//ammoType = 1, QuadBullet
				DoGrenadeBulletFx (hitPoint, this.teamID, playerPhotonView.viewID, gameObject.tag);
			} else if (ammoType == 3) {
				DoRailBulletFX (hitPoint);
			} else if (ammoType == 5) {
				DoMissAxeFX (hitPoint);
				//NO BULLET FOR MELEE AXE
				//				DoRailBulletFX (hitPoint);
			}
		}
	}

	Transform closestHit;
	Transform hitTr;
	RaycastHit raycastHit;
	float hitTrHealth;
	RaycastHit[] hits; 
	string HeadTag = "Head";
	public bool isHeadShot = false;
	int nonItemLayerMask = ~(1 << 10);	//Do not hit itemLayer
	int nonPlayerLayerMask = ~(1 << 6);	//Do not hit playerLayer (Detect Headshots, so ignore the parent enemy transform)
	Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint, float ammoType){

		//		RaycastHit[] hits; 
		if (ammoType != 5) {
			hits =	Physics.RaycastAll (ray, 2000f, nonItemLayerMask);
		} else {
			hits = Physics.RaycastAll (ray, 6f, nonItemLayerMask);
		}

		closestHit = null;
		float distance = 0;
		hitPoint = Vector3.zero;
		bool foundHit = false;
		hitTrHealth = 0f;


		foreach (RaycastHit hit in hits) {
			hitTr = hit.transform;
			raycastHit = hit;
			isHeadShot = false;

			if (hitTr.CompareTag (playerTag)) {
				hitTrHealth = hitTr.GetComponent<Health> ().currentHitPoints;
			} else if (hitTr.CompareTag (enemyTag)) {
				hitTrHealth = hitTr.GetComponent<Health> ().enemyHitPoints;
			}

			if (hitTr != tr && (closestHit == null || hit.distance < distance) && hitTrHealth > 0) {
				//we have hit something that is:
				//a) not us
				//b) the first thing we hit (that is not us)
				//c) or, if not b, is at least closer than the previous closest thing

				foundHit = true;

				closestHit = hitTr;

				//MELEE AND PROJECTILES WILL NOT UNDERGO HEADSHOT CHECK (PROJECTILE HAS DIRECT HIT, AND MELEE SHOULD BE 1HK0)
				if (ammoType != 0 && ammoType != 2 && ammoType != 4 && ammoType != 5) {

					if (hit.collider.transform.CompareTag (HeadTag)) {
						isHeadShot = true;
						playerGuiManger.headShotFlag = true;
					}
				}

				distance = hit.distance;
				hitPoint = hit.point;
			} else if (hitTr != tr && (closestHit == null || hit.distance < distance) && hitTrHealth <= 0) {
				foundHit = true;
				closestHit = hitTr;
				distance = hit.distance;
				hitPoint = hit.point;
			}
		}
		//closestHit is now either still null (i.e. we hit nothing) OR it contains the closest thing that is valid to hit

		if (!foundHit && ammoType == 5) {
			hitPoint = playerCameraTransform.position + (playerCameraTransform.forward * 6f);
		} else if (!foundHit && (ammoType == 0 || ammoType == 2 || ammoType == 4)) {
			hitPoint = playerCameraTransform.position + (playerCameraTransform.forward * 2000f);
		}
		return closestHit;
	}

	public void ResetWeaponAmmoAndSelection(int primaryWeap){
		//RESETS WHICH LOCAL GUN IS DISPLAYED
		gunRocket_LocalModel.SetActive(false);
		gunGrenade_LocalModel.SetActive(false);
		gunMG_LocalModel.SetActive(false);
		gunHMG_LocalModel.SetActive(false);
		gunRail_LocalModel.SetActive(false);
		meleeAxe_LocalModel.SetActive(false);

		weapMGObtained = false;
		weapHMGObtained = true;
		weapRocketObtained = true;
		weapGrenadeObtained = true;
		weapRailObtained = true;
		weapAxeObtained = true;


		if (primaryWeap == 0) {
			if (guiManager.currentGameMode == 2) {
				regAmmo = 0;
				quadAmmo = 100;
				railAmmo = 25;
				rocketAmmo = 25;
				grenadeAmmo = 25;
				axeAmmo = 1;

				quadAmmoSel = false;
				regAmmoSel = false;
				rocketAmmoSel = false;
				grenadeAmmoSel = false;
				railAmmoSel = false;
				axeAmmoSel = true;

				selectedAmmoType = 5;
				meleeAxe_LocalModel.SetActive (true);
			} else {
				regAmmo = 0;
				quadAmmo = 100;
				railAmmo = 10;
				rocketAmmo = 10;
				grenadeAmmo = 10;
				axeAmmo = 1;

				quadAmmoSel = false;
				regAmmoSel = false;
				rocketAmmoSel = false;
				grenadeAmmoSel = false;
				railAmmoSel = false;
				axeAmmoSel = true;

				selectedAmmoType = 5;
				meleeAxe_LocalModel.SetActive (true);
			}

		}
		if (primaryWeap == 1) {

			weapHMGObtained = true;

			regAmmo = 0;
			quadAmmo = 50;
			railAmmo = 0;
			rocketAmmo = 0;
			grenadeAmmo = 0;
			axeAmmo = 1;
			repairResource = 0;
			quadAmmoSel = true;
			selectedAmmoType = 1;
			gunHMG_LocalModel.SetActive(true);
		}
		if (primaryWeap == 2) {

			weapRocketObtained = true;

			regAmmo = 0;
			quadAmmo = 0;
			railAmmo = 0;
			rocketAmmo = 25;
			grenadeAmmo = 0;
			axeAmmo = 1;
			repairResource = 0;
			rocketAmmoSel = true;
			selectedAmmoType = 2;
			gunRocket_LocalModel.SetActive(true);
		}
		if (primaryWeap == 3) {

			weapRailObtained = true;

			regAmmo = 0;
			quadAmmo = 0;
			railAmmo = 5;
			rocketAmmo = 0;
			grenadeAmmo = 0;
			axeAmmo = 1;
			repairResource = 0;
			railAmmoSel = true;
			selectedAmmoType = 3;
			gunRail_LocalModel.SetActive(true);
		}
		if (primaryWeap == 4) {

			weapGrenadeObtained = true;

			regAmmo = 0;
			quadAmmo = 0;
			railAmmo = 0;
			rocketAmmo = 0;
			grenadeAmmo = 25;
			axeAmmo = 1;
			repairResource = 0;
			grenadeAmmoSel = true;
			selectedAmmoType = 4;
			gunGrenade_LocalModel.SetActive(true);
		}
	}


	string msgHMGPickup = "PICKED UP H.M.G. AMMO";
	string msgRocketPickup = "PICKED UP ROCKET AMMO";
	string msgRiflePickup = "PICKED UP RIFLE AMMO";
	string msgGrenadePickup = "PICKED UP GRENADE AMMO";
	string msgPorterPickup = "PICKED UP PORTER AMMO";
	string msgHealthPickup = "PICKED UP HEALTH";
	string msgArmorPickup = "PICKED UP ARMOR";
	string msgMiscPickup = "PICKED UP ITEM";

	[PunRPC]
	public void setItemPickupPlayerNamePlayerShooting(string str, string myItemName){

		if (itemPickUpName.StartsWith (strAmmo_HMG)) {
			weapHMGObtained = true;
		} else if (itemPickUpName.StartsWith (strAmmo_Rocket)) {
			weapRocketObtained = true;
		} else if (itemPickUpName.StartsWith (strAmmo_Grenade)) {
			weapGrenadeObtained = true;
		} else if (itemPickUpName.StartsWith (strAmmo_Rail)) {
			weapRailObtained = true;
		} else if (itemPickUpName.StartsWith (strAmmo_MG)) {
			weapMGObtained = true;
		}
		itemPickupPlayerName = str;
		itemPickUpName = myItemName;


		if (itemPickUpName.StartsWith (strAmmo_HMG)) {
			guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine (msgHMGPickup);
		} else if (itemPickUpName.StartsWith (strAmmo_Rocket)) {
			guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine (msgRocketPickup);
		} else if (itemPickUpName.StartsWith (strAmmo_Rail)) {
			guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine (msgRiflePickup);
		} else if (itemPickUpName.StartsWith (strAmmo_Grenade)) {
			guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine (msgGrenadePickup);
		} else if (itemPickUpName.StartsWith (strAmmo_MG)) {
			guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine (msgPorterPickup);
		} else if (itemPickUpName.StartsWith (strHealth)) {
			guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine (msgHealthPickup);
		} else if (itemPickUpName.StartsWith (strArmor)) {
			guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine (msgArmorPickup);
		} else {
			guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine (msgMiscPickup);
		}
		StartCoroutine (guiManager.ItemPickupMessageCoroutine);			


	}


	string strAmmo_HMG = "Ammo_HMG";
	string strAmmo_MG = "Ammo_MG";
	string strAmmo_Rail = "Ammo_Rail";
	string strAmmo_Grenade = "Ammo_Grenade";
	string strAmmo_Rocket = "Ammo_Rocket";
	void calculateItemPickupPlayerShooting(){

		if (itemPickupPlayerName.Equals (teamMember.GetPlayerName ())) {
			if (itemPickUpName.StartsWith (strAmmo_HMG)) {
				weapHMGObtained = true;
				ammoPickupType = 1;
				quadAmmo += 100;

				if (CompareTag (playerTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), scoringManager.pickup_hmgStat, 1);
				} else if (CompareTag (enemyTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, gameObject.name, scoringManager.pickup_hmgStat, 1);
				}
				if (quadAmmo >= 400) {
					quadAmmo = 400;
				}
				if (cInputManager.switchWeaponDefault)
					SwitchQuadUpdate (true);
			}
			if (itemPickUpName.StartsWith (strAmmo_MG)) {
				weapMGObtained = true;
				ammoPickupType = 0;
				regAmmo += 5;

				if (CompareTag (playerTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), scoringManager.pickup_porterStat, 1);
				} else if (CompareTag (enemyTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, gameObject.name, scoringManager.pickup_porterStat, 1);
				}

				if (regAmmo >= 25) {
					regAmmo = 25;
				}
				if (cInputManager.switchWeaponDefault)
					SwitchPorterUpdate (true);
			}
			if (itemPickUpName.StartsWith (strAmmo_Rail)) {
				weapRailObtained = true;
				ammoPickupType = 2;
				railAmmo += 10;

				if (CompareTag (playerTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), scoringManager.pickup_railStat, 1);
				} else if (CompareTag (enemyTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, gameObject.name, scoringManager.pickup_railStat, 1);
				}

				if (railAmmo >= 50) {
					railAmmo = 50;
				}
				if (cInputManager.switchWeaponDefault)
					SwitchRailUpdate (true);
			}
			if (itemPickUpName.StartsWith (strAmmo_Grenade)) {
				weapGrenadeObtained = true;
				ammoPickupType = 3;
				grenadeAmmo += 25;

				if (CompareTag (playerTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), scoringManager.pickup_empStat, 1);
				} else if (CompareTag (enemyTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, gameObject.name, scoringManager.pickup_empStat, 1);
				}

				if (grenadeAmmo >= 100) {
					grenadeAmmo = 100;
				}
				if (cInputManager.switchWeaponDefault)
					SwitchGrenadeUpdate (true);
			}
			if (itemPickUpName.StartsWith (strAmmo_Rocket)) {
				weapRocketObtained = true;
				ammoPickupType = 4;
				rocketAmmo += 15;

				if (CompareTag (playerTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, teamMember.GetPlayerName (), scoringManager.pickup_rocketStat, 1);
				} else if (CompareTag (enemyTag)) {
					sceneScriptsPhotonView.RPC (changeScoreRPC, PhotonTargets.AllBuffered, gameObject.name, scoringManager.pickup_rocketStat, 1);
				}

				if (rocketAmmo >= 50) {
					rocketAmmo = 50;
				}

				if (cInputManager.switchWeaponDefault)
					SwitchRocketUpdate (true);
			}

			itemPickupPlayerName = string.Empty;
			itemPickUpName = string.Empty;
		}
	}

	string PickupItemRPC = "PickupItem";
	void OnTriggerEnter(Collider other){

		if (playerPhotonView == null)
			return;
		if (!playerPhotonView.isMine)
			return;

		string itemName = other.gameObject.name;

		if (itemName.StartsWith (strAmmo)) {
			setItemPickupPlayerNamePlayerShooting (teamMember.GetPlayerName (), itemName);
			if (itemPickupPlayerName != string.Empty && itemPickUpName != string.Empty) {
//				Debug.Log ("Initial ammo picked by: " + itemPickupPlayerName + " and itemName is: " + itemPickUpName);
				other.GetComponentInParent<PhotonView> ().RPC (PickupItemRPC, PhotonTargets.AllBuffered, itemName);
			}
		}
	}

	float switchWeaponTime = 0.6f;
	string strMouseScrollWheel = "Mouse ScrollWheel";
	void switchNextWeap(){

		if (switchWeaponTime > 0.6f) {
			switchWeaponTime = 0f;

			if (kMgr.GetKeyUpPublic (keyType.switchNextWeap) || Input.GetAxis (strMouseScrollWheel) > 0) {
				if (cooldown > 0f)
					return;
				GetNext (selectedAmmoType);
			}
		} else {
			switchWeaponTime += Time.deltaTime;
		}
	}

	public void switchPreviousWeap(){
		if (switchWeaponTime > 0.6f) {
			if (kMgr.GetKeyUpPublic (keyType.switchPreviousWeap) || Input.GetAxis (strMouseScrollWheel) < 0) {
				if (cooldown > 0f)
					return;
				GetPrev (selectedAmmoType);
			}
		} else {
			switchWeaponTime += Time.deltaTime;
		}
	}

	void getAmmoSel(float desiredWeapon){

		selectedAmmoType = desiredWeapon;

		regAmmoSel = desiredWeapon == 0 ? true : false;
		quadAmmoSel = desiredWeapon == 1 ? true : false;
		rocketAmmoSel = desiredWeapon == 2 ? true : false;
		railAmmoSel = desiredWeapon == 3 ? true : false;
		grenadeAmmoSel = desiredWeapon == 4 ? true : false;
		axeAmmoSel = desiredWeapon == 5 ? true : false;

		if (desiredWeapon == 0)
			gunMG_LocalModel.SetActive (true);		
		if (desiredWeapon == 1)
			gunHMG_LocalModel.SetActive (true);		
		if (desiredWeapon == 2)
			gunRocket_LocalModel.SetActive (true);		
		if (desiredWeapon == 3)
			gunRail_LocalModel.SetActive (true);		
		if (desiredWeapon == 4)
			gunGrenade_LocalModel.SetActive (true);		
		if (desiredWeapon == 5)
			meleeAxe_LocalModel.SetActive (true);
	}

	string WeaponSwitchFxRPC = "WeaponSwitchFx";

	void GetNext(float currAmmoType){
		gunMG_LocalModel.SetActive (false);
		gunHMG_LocalModel.SetActive (false);
		gunRocket_LocalModel.SetActive (false);
		gunRail_LocalModel.SetActive (false);
		gunGrenade_LocalModel.SetActive (false);
		meleeAxe_LocalModel.SetActive (false);

		playerPhotonView.RPC (WeaponSwitchFxRPC, PhotonTargets.All);

		if (currAmmoType == 0) {
			//FROM MG
			if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapRailObtained)
				getAmmoSel (3);
			else if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapMGObtained)
				getAmmoSel (0);
		} else if (currAmmoType == 1) {
			//FROM HMG
			if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapRailObtained)
				getAmmoSel (3);
			else if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapMGObtained)
				getAmmoSel (0);
			else if (weapHMGObtained)
				getAmmoSel (1);			
		} else if (currAmmoType == 2) {
			//FROM ROCKET
			if (weapRailObtained)
				getAmmoSel (3);
			else if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapMGObtained)
				getAmmoSel (0);
			else if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapRocketObtained)
				getAmmoSel (2);
		} else if (currAmmoType == 3) {
			//FROM RAIL
			if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapMGObtained)
				getAmmoSel (0);
			else if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapRailObtained)
				getAmmoSel (3);
		} else if (currAmmoType == 4) {
			//FROM GRENADE
			if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapMGObtained)
				getAmmoSel (0);
			else if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapRailObtained)
				getAmmoSel (3);
			else if (weapGrenadeObtained)
				getAmmoSel (4);			
		} else if (currAmmoType == 5) {
			//FROM PORTER
			if (weapMGObtained)
				getAmmoSel (0);
			else if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapRailObtained)
				getAmmoSel (3);
			else if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapAxeObtained)
				getAmmoSel (5);			
		}

	}

	void GetPrev(float currAmmoType){
		gunMG_LocalModel.SetActive (false);
		gunHMG_LocalModel.SetActive (false);
		gunRocket_LocalModel.SetActive (false);
		gunRail_LocalModel.SetActive (false);
		gunGrenade_LocalModel.SetActive (false);
		meleeAxe_LocalModel.SetActive (false);

		playerPhotonView.RPC (WeaponSwitchFxRPC, PhotonTargets.All);

		if (currAmmoType == 5) {
			//FROM AXE
			if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapRailObtained)
				getAmmoSel (3);
			else if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapMGObtained)
				getAmmoSel (0);
			else if (weapAxeObtained)
				getAmmoSel (5);			
		} else if (currAmmoType == 4) {
			//FROM GRENADE
			if (weapRailObtained)
				getAmmoSel (3);
			else if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapMGObtained)
				getAmmoSel (0);
			else if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapGrenadeObtained)
				getAmmoSel (4);			
		} else if (currAmmoType == 3) {
			//FROM RAIL
			if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapMGObtained)
				getAmmoSel (0);
			else if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapRailObtained)
				getAmmoSel (3);			
		} else if (currAmmoType == 2) {
			//FROM ROCKET
			if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapMGObtained)
				getAmmoSel (0);
			else if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapRailObtained)
				getAmmoSel (3);
			else if (weapRocketObtained)
				getAmmoSel (2);			
		} else if (currAmmoType == 1) {
			//FROM HMG
			if (weapMGObtained)
				getAmmoSel (0);
			else if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapRailObtained)
				getAmmoSel (3);
			else if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapHMGObtained)
				getAmmoSel (1);			
		} else if (currAmmoType == 0) {
			//FROM MG
			if (weapAxeObtained)
				getAmmoSel (5);
			else if (weapGrenadeObtained)
				getAmmoSel (4);
			else if (weapRailObtained)
				getAmmoSel (3);
			else if (weapRocketObtained)
				getAmmoSel (2);
			else if (weapHMGObtained)
				getAmmoSel (1);
			else if (weapMGObtained)
				getAmmoSel (9);			
		}
	}


	////////////////FX RPC CALLS///////////////////
	//Miss Shot FX

	string MissShotFxRPC = "MissShotFx";
	string MissAxeFxRPC = "MissAxeFx";
	string HitAxeFxRPC = "HitAxeFx";
	string MGLightFxRPC = "MGLightFx";

	void DoMissShotFX(Vector3 hitPoint){
		sceneScriptsPhotonView.RPC(MissShotFxRPC,PhotonTargets.All, hitPoint);
	}

	void DoMissAxeFX(Vector3 hitPoint){
		sceneScriptsPhotonView.RPC(MissAxeFxRPC,PhotonTargets.All, hitPoint);
	}

	void DoHitAxeFX(Vector3 hitPoint){
		sceneScriptsPhotonView.RPC(HitAxeFxRPC,PhotonTargets.All, hitPoint);
	}

	void DoRegBulletFX(Vector3 hitPoint){
		sceneScriptsPhotonView.RPC(MGLightFxRPC, PhotonTargets.All, gunPositionDataTransform.position, hitPoint);
	}

	void DoBulletHitFX(Vector3 hitPoint){
		//This doesn't really need a hit point, since the sound is a 'local non-doppler sound' 2D sound
		fxManager.BulletHitSoundFx (hitPoint);
	}
	void DoProjectileHitFX(Vector3 hitPoint){
		fxManager.ProjectileHitSoundFx (hitPoint);
	}


	string MGHeavyFx = "MGHeavyFx";
	Vector3 adjustedHMGBulletVector = Vector3.zero;
	void DoQuadBulletFX(Vector3 hitPoint){
		if (!kMgr.GetKeyPublic (keyType.zoom)) {
			adjustedHMGBulletVector = gunPositionDataTransform.right;
			adjustedHMGBulletVector = Vector3.Scale (adjustedHMGBulletVector, new Vector3 (0.25f, 0.25f, 0.25f));
		} else {
			adjustedHMGBulletVector = Vector3.zero;
		}

		sceneScriptsPhotonView.RPC (MGHeavyFx, PhotonTargets.All, gunPositionDataTransform.position + gunPositionDataTransform.forward + adjustedHMGBulletVector, hitPoint);
	}

	string RocketFx = "RocketFx";
	void DoRocketBulletFx(Vector3 hitPoint, int shooterTeamID, int photonID, string tagName){
		sceneScriptsPhotonView.RPC (RocketFx, PhotonTargets.All, gunPositionDataTransform.position + gunPositionDataTransform.forward, hitPoint, PhotonNetwork.player.name, this.teamID, photonID, tagName);
	}

	string GrenadeFx = "GrenadeFx";
	void DoGrenadeBulletFx(Vector3 hitPoint, int shooteTeamID, int photonID, string tagName){
		sceneScriptsPhotonView.RPC (GrenadeFx, PhotonTargets.All, gunPositionDataTransform.position + gunPositionDataTransform.forward, hitPoint, PhotonNetwork.player.name, this.teamID, photonID, tagName);
	}

	string TeleportFx = "TeleportFx";
	void DoTeleportFx(Vector3 hitPoint, int shooteTeamID, int photonID, string tagName){
		sceneScriptsPhotonView.RPC (TeleportFx, PhotonTargets.All, gunPositionDataTransform.position, hitPoint, PhotonNetwork.player.name, this.teamID, playerPhotonView.viewID, tagName);
	}

	string RailReloadSoundFx = "RailReloadSoundFx";
	void DoRailBulletFX(Vector3 hitPoint){
		if (!kMgr.GetKeyPublic (keyType.zoom)) {
			adjustedHMGBulletVector = gunPositionDataTransform.right;
			adjustedHMGBulletVector = Vector3.Scale (adjustedHMGBulletVector, new Vector3 (0.25f, 0.25f, 0.25f));
		} else {
			adjustedHMGBulletVector = Vector3.zero;
		}

		//Same Bullet Prefab as HMG
		sceneScriptsPhotonView.RPC(MGHeavyFx, PhotonTargets.All, gunPositionDataTransform.position + gunPositionDataTransform.forward + adjustedHMGBulletVector, hitPoint);
		sceneScriptsPhotonView.RPC(RailReloadSoundFx, PhotonTargets.All, gunPositionDataTransform.position + gunPositionDataTransform.forward + adjustedHMGBulletVector);

		//RailReloadSoundFx
	}

	//Ammo Pickup FX Stuff
	[PunRPC]
	void AmmoPickFx(){
		if(ammoPickupType == 0){
			PlayerAudioSource.PlayOneShot(Ammo_pickup[0]);
		}
		else if(ammoPickupType == 1){
			PlayerAudioSource.PlayOneShot(Ammo_pickup[1]);
		}
		else if(ammoPickupType == 2){
			PlayerAudioSource.PlayOneShot(Ammo_pickup[2]);
		}
		else if(ammoPickupType == 4){
			PlayerAudioSource.PlayOneShot(Ammo_pickup[4]);
		}
		else if(ammoPickupType == 3){
			PlayerAudioSource.PlayOneShot(Ammo_pickup[3]);
		}
		else if(ammoPickupType == 5){
			PlayerAudioSource.PlayOneShot(Ammo_pickup[3]);
		}
	}

	//Switch Weapon FX Stuff
	[PunRPC]
	void WeaponSwitchFx(){

		if(ammoPickupType == 0){
			PlayerAudioSource.PlayOneShot(switchWeaponSounds[0]);
		}
		else if(ammoPickupType == 1){
			PlayerAudioSource.PlayOneShot(switchWeaponSounds[0]);
		}
		else if(ammoPickupType == 2){
			PlayerAudioSource.PlayOneShot(switchWeaponSounds[0]);
		}
		else if(ammoPickupType == 4){
			PlayerAudioSource.PlayOneShot(switchWeaponSounds[0]);
		}
		else if(ammoPickupType == 3){
			PlayerAudioSource.PlayOneShot(switchWeaponSounds[0]);
		}
		else if(ammoPickupType == 5){
			PlayerAudioSource.PlayOneShot(switchWeaponSounds[0]);
		}
	}

	[PunRPC]
	void SetIsPlayerShooting(bool val){
		isPlayerShooting = val;
	}

	[PunRPC]
	void BulletFired(float ammoType, bool shotFired){
		//Use ShotFired = TRUE when incrementing total shots fired
		//Use ShotFired = FALSE when incrementing total shots hit
		if (playerPhotonView == null)
			return;
		if (!playerPhotonView.isMine)
			return;

		if (ammoType == 1) {

			if (shotFired) {
				HMGFiredCount += 1;
				scoringManager.SetScore(PhotonNetwork.playerName, scoringManager.shotfired_hmgStat, HMGFiredCount);
			} else {
				HMGHitCount += 1;
				scoringManager.SetScore(PhotonNetwork.playerName, scoringManager.shothit_hmgStat, HMGHitCount);
			}
		} else if (ammoType == 2) {
			if (shotFired) {
				RocketFiredCount += 1;
				scoringManager.SetScore(PhotonNetwork.playerName, scoringManager.shotfired_rocketStat, RocketFiredCount);
			} else {
				RocketHitCount += 1;
				scoringManager.SetScore(PhotonNetwork.playerName, scoringManager.shothit_rocketStat, RocketHitCount);
			}
		} else if (ammoType == 3) {
			if (shotFired) {
				RailFiredCount += 1;
				scoringManager.SetScore(PhotonNetwork.playerName, scoringManager.shotfired_railStat, RailFiredCount);
			} else {
				RailHitCount += 1;
				scoringManager.SetScore(PhotonNetwork.playerName, scoringManager.shothit_railStat, RailHitCount);
			}
		} else if (ammoType == 4) {
			if (shotFired) {
				EMPFiredCount += 1;
				scoringManager.SetScore(PhotonNetwork.playerName, scoringManager.shotfired_empStat, EMPFiredCount);
			} else {
				EMPHitCount += 1;
				scoringManager.SetScore(PhotonNetwork.playerName, scoringManager.shothit_empStat, EMPHitCount);
			}
		}

	}


//	ExitGames.Client.Photon.Hashtable playerProps;
//	PhotonPlayer photonPlayer;
//	PhotonPlayer tempPlayer;
//	void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) {
//		PhotonNetwork.SendMonoMessageTargetType = typeof(PlayerShooting);
//		PhotonNetwork.CacheSendMonoMessageTargets (typeof(PlayerShooting));
//
//		tempPlayer = playerAndUpdatedProps[0] as PhotonPlayer;
//		playerProps = playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;
//
//		if (playerProps.ContainsKey (DamageDealtProp)) {
//			DamageDealtCount = (int)playerProps [DamageDealtProp];
////			Debug.Log (tempPlayer.NickName + " has dealt: " + DamageDealtCount);
//		}
//	}

}
