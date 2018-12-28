using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
using System.Collections;
using Photon;

public enum playerKillStatus{
	None,
	KillConfirm,
	MultiKillConfirm
}

public enum playerAssistStatus{
	None,
	Assist
}

public enum playerKillStreaks{
	None,
	KillStreak3,
	KillStreak5,
	KillStreak10
}

public class PlayerGuiManager : PunBehaviour {

	public ExitGames.Client.Photon.Hashtable PlayerScoreTable;
	public string DamageDealtProp = "damagedealtprop";

	public int DamageDealtStat = 0;

	GuiManager guiManagerMain;
	PlayerMovement playerMovement;
	PlayerShooting pShooting;
	Health pHealth;
	ScoringManager scoringManager;
	FXManager fxManager;

	PlayerInGamePanel playerInGamePanel;
	InGamePanel inGamePanel;

	WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame ();
	WaitForSeconds waitFor0_1 = new WaitForSeconds (0.1f);
	WaitForSeconds waitFor0_2 = new WaitForSeconds (0.2f);
	WaitForSeconds waitFor0_25 = new WaitForSeconds (0.25f);
	WaitForSeconds waitFor0_5 = new WaitForSeconds (0.5f);
	WaitForSeconds waitFor1 = new WaitForSeconds (1f);
	WaitForSeconds waitFor3 = new WaitForSeconds (3f);

	public Camera playerCamera;
	public Camera miniMapCamera;
	public PostProcessingBehaviour filters;
	PostProcessingProfile profile;
	public Color[] VignetteColors;

	int playerVel = 0;

	//KILLSTREAK ICONS
	bool enableKillStreakIcon = false;
	int killStreak = 0;
	public Color[] killStreakColors;

	//TILE CAPTURED ICON VARS
	bool enableTilesCapturedIcon = false;
	int tilesCaptured = 0;
	int currTilesCaptured = 0;

	//ASSIST CONFIRM VARS
	bool enableAssistConfirmIcon = false;
	int assists = 0;
	int currAssists = 9999;

	//KILL CONFIRM ICON VARS
	bool enableKillConfirmIcon = false;
	int kills = 0;
	int currKills=9999; //initialize it so that first call does not call the killconfirm icon

	//MULTI KILL CONFIRM ICON VARS "EXCELLENT" ACHIEVEMENT
	bool enableMultiKillConfirmIcon = false;

	//5 WEAPON ICONS OF DIFFERENT COLOR STUFF
	public Texture2D rocketIcon;
	public Texture2D grenadeIcon;
	public Texture2D railIcon;
	public Texture2D mgIcon;
	public Texture2D hmgIcon;

	//AUDIO ANNOUNCER CLIPS
	public AudioClip[] playerSkillAudioList;

	int _announceValue=-1;
	public int announceValue {
		get{ return this._announceValue; }
		set{ this._announceValue = value; }
	}

	int _announceStreakValue=-1;
	public int announceStreakValue {
		get{ return this._announceStreakValue; }
		set{ this._announceStreakValue = value; }
	}

	int _announceAssistValue=-1;
	public int announceAssistValue {
		get{ return this._announceAssistValue; }
		set{ this._announceAssistValue = value; }
	}

	public bool isHit = false;

	void Awake () {

		guiManagerMain = GameObject.FindGameObjectWithTag("SceneScripts").GetComponent<GuiManager> ();
		scoringManager = guiManagerMain.GetComponent<ScoringManager> ();
		fxManager = guiManagerMain.GetComponent<FXManager> ();
		if (GameObject.FindGameObjectWithTag ("PlayerInGamePanelTag") != null) {
			playerInGamePanel = GameObject.FindGameObjectWithTag ("PlayerInGamePanelTag").GetComponent<PlayerInGamePanel> ();
		}
		if (GameObject.FindGameObjectWithTag ("InGamePanelTag") != null) {
			inGamePanel = GameObject.FindGameObjectWithTag ("InGamePanelTag").GetComponent<InGamePanel> ();
		}
		pShooting = GetComponent<PlayerShooting> ();
		pHealth = GetComponent<Health> ();
		playerMovement = GetComponent<PlayerMovement> ();

		profile = filters.profile;

		if (GetComponent<PhotonView> ().isMine) {
			//HUD Coroutines
			StartCoroutine (KillConfirmDetection_Coroutine ());

			StartCoroutine (HealthArmorDisplay_Coroutine ());
			StartCoroutine (DamageStunDisplay_Coroutine ());
//			StartCoroutine (PlayerVelocityCalc_Coroutine ());
			StartCoroutine (FPSCalc_Coroutine ());
			StartCoroutine (WeaponSelectedDisplay_Coroutine ());
		}

	}

	string strPlayerInGamePanelTag = "PlayerInGamePanelTag";
	string strInGamePanelTag = "InGamePanelTag";
	void Update () {
		if (playerInGamePanel == null) {
			if (GameObject.FindGameObjectWithTag (strPlayerInGamePanelTag))
				playerInGamePanel = GameObject.FindGameObjectWithTag (strPlayerInGamePanelTag).GetComponent<PlayerInGamePanel> ();
		}
		if (inGamePanel == null) {
			if (GameObject.FindGameObjectWithTag (strInGamePanelTag))
				inGamePanel = GameObject.FindGameObjectWithTag (strInGamePanelTag).GetComponent<InGamePanel> ();
		}
	}


	//cache string info
	string strHeadShot = "HEADSHOT ";
	string strdemolished = "DEMOLISHED ";
	string strFragged = "FRAGGED ";
	string strRevenged = "FRAGGED ";
	string strSpeed = "SPEED";
	string strFPS = "FPS";
	IEnumerator KillConfirmDetection_Coroutine(){
		while (true) {
			kills = scoringManager.GetScore (PhotonNetwork.player.NickName, scoringManager.killsStat);

			if (kills - currKills > 0) {

				enableKillConfirmIcon = true;

				if (headShotFlag) {
					guiManagerMain.KillMsgCO = guiManagerMain.KillMsg_CO (strHeadShot + scoringManager.GetKilledLast (GetComponent<TeamMember> ().GetPlayerName (), scoringManager.name_lastplayerkilledStat));
				} else if (fxManager.isProjectileDirectHit) {
					guiManagerMain.KillMsgCO = guiManagerMain.KillMsg_CO (strdemolished + scoringManager.GetKilledLast (GetComponent<TeamMember> ().GetPlayerName (), scoringManager.name_lastplayerkilledStat));
				} else {
					guiManagerMain.KillMsgCO = guiManagerMain.KillMsg_CO (strFragged + scoringManager.GetKilledLast (GetComponent<TeamMember> ().GetPlayerName (), scoringManager.name_lastplayerkilledStat));
				}

				if (!guiManagerMain.Msg_NPC.gameObject.GetActive () && !guiManagerMain.Msg_Death.gameObject.GetActive () && !guiManagerMain.Msg_Obj.gameObject.GetActive ()) {
					StartCoroutine (guiManagerMain.KillMsgCO);
				}

				KillConfirmCO = KillConfirm_CO ();
				StartCoroutine (KillConfirmCO);

			} else {
				enableKillConfirmIcon = false;
			}
			//Scan again after DisplayTime runs out
			currKills = scoringManager.GetScore (PhotonNetwork.player.NickName, scoringManager.killsStat);
			yield return waitForEndOfFrame;
		}
	}



	IEnumerator DamageStunDisplay_Coroutine(){
		//HealthDamageEffect

		VignetteModel.Settings tempVignetteSetting = profile.vignette.settings;

		while (true) {
			if (playerInGamePanel != null) {
				//FREEZE EFFECT DISPLAY TAKES PRECEDENCE OVER DAMAGE FOR NOW
				if (playerMovement.isEmpEffected) {

					tempVignetteSetting = profile.vignette.settings;
					tempVignetteSetting.color = VignetteColors [2];
					profile.vignette.settings = tempVignetteSetting;
					yield return waitFor3;

				} else {
					//THEN CONSIDER DAMAGE EFFECT DISPLAY
					tempVignetteSetting = profile.vignette.settings;
					tempVignetteSetting.color = VignetteColors [0];
					profile.vignette.settings = tempVignetteSetting;
					if (isHit) {
						tempVignetteSetting = profile.vignette.settings;
						tempVignetteSetting.color = VignetteColors [1];
						profile.vignette.settings = tempVignetteSetting;
						yield return waitFor0_25;
						tempVignetteSetting = profile.vignette.settings;
						tempVignetteSetting.color = VignetteColors [0];
						profile.vignette.settings = tempVignetteSetting;
						isHit = false;
					}
				}			
			}
			yield return waitFor0_1;
		}
	}

	public bool headShotFlag = false;
	IEnumerator KillConfirmCO;
	IEnumerator KillConfirm_CO(){

		if (GetComponent<PhotonView> ().isMine && _announceValue != (int)playerKillStatus.KillConfirm) {
			_announceValue = (int)playerKillStatus.KillConfirm;
			if (guiManagerMain.currentGameMode == 1 || guiManagerMain.currentGameMode == 2) {
				//TDM / Survival (no denial) yet
				if (headShotFlag) {
					inGamePanel.Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (playerSkillAudioList [7]);
				} else if (fxManager.isProjectileDirectHit) {
					inGamePanel.Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (playerSkillAudioList [8]);
				} else {
					inGamePanel.Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (playerSkillAudioList [0]);
				}
			}
		}

		headShotFlag = false;
		fxManager.isProjectileDirectHit = false;
		_announceValue = (int)playerKillStatus.None;
		yield return true;
	}
		
	IEnumerator PlayerVelocityCalc_Coroutine(){
		//PLAYER VELOCITY
		while (true) {
			if (playerInGamePanel != null) {
				playerInGamePanel.Label_Speed.text = strSpeed;
				playerVel = (int)playerMovement.playerVelocity.magnitude;
				playerInGamePanel.Value_Speed.text = playerVel.ToString ();
			}
			yield return waitFor0_25;
		}

	}

	IEnumerator FPSCalc_Coroutine(){
		//FRAMES
		while (true) {
			if (playerInGamePanel != null) {
				playerInGamePanel.Label_FPS.text = strFPS;
				playerInGamePanel.Value_FPS.text = ((int)playerMovement.fps).ToString ();
			}
			yield return waitFor0_25;
		}
	}

	string weapRocket = "ROCKET";
	string weapHMG = "H.M.G.";
	string weapPorter = "PORTER";
	string weapAxe = "AXE";
	string weapGrenade = "GRENADE";
	string weapRifle = "RIFLE";

	string percent = " %";
	string zeroPercent = "-- %";


	IEnumerator WeaponSelectedDisplay_Coroutine(){
		//ROCKET AMMO
		while (true) {
			if (playerInGamePanel != null) {

				if (pShooting.selectedAmmoType == 2) {
					playerInGamePanel.Image_AmmoRadial.GetComponent<Image> ().fillAmount = (pShooting.rocketAmmo / 50);
					playerInGamePanel.Value_CurrentAmmo.text = Mathf.Round (pShooting.rocketAmmo).ToString ();
					playerInGamePanel.Label_CurrentAccuracy.text = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.acc_RocketStat).ToString() + percent;

					//Do not need to keep updating these
					if (playerInGamePanel.Label_CurrentWeapon.text != weapRocket) {
						playerInGamePanel.Slider_CurrentAmmo.maxValue = 50;

						playerInGamePanel.Label_CurrentWeapon.text = weapRocket;
					}
				}

				//GRENADE AMMO
				if (pShooting.selectedAmmoType == 4) {
					playerInGamePanel.Image_AmmoRadial.GetComponent<Image> ().fillAmount = (pShooting.grenadeAmmo / 100);
					playerInGamePanel.Value_CurrentAmmo.text = Mathf.Round (pShooting.grenadeAmmo).ToString ();
					playerInGamePanel.Label_CurrentAccuracy.text = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.acc_GrenadeStat).ToString() + percent;

					if (playerInGamePanel.Label_CurrentWeapon.text != weapGrenade) {
						playerInGamePanel.Slider_CurrentAmmo.maxValue = 100;

						playerInGamePanel.Label_CurrentWeapon.text = weapGrenade;
					}
				}

				//RAIL AMMO
				if (pShooting.selectedAmmoType == 3) {
					playerInGamePanel.Image_AmmoRadial.GetComponent<Image> ().fillAmount = (pShooting.railAmmo / 50);
					playerInGamePanel.Value_CurrentAmmo.text = Mathf.Round (pShooting.railAmmo).ToString ();
					playerInGamePanel.Label_CurrentAccuracy.text = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.acc_railStat).ToString() + percent;

					if (playerInGamePanel.Label_CurrentWeapon.text != weapRifle) {
						playerInGamePanel.Slider_CurrentAmmo.maxValue = 50;

						playerInGamePanel.Label_CurrentWeapon.text = weapRifle;
					}
				}

				//MG
				if (pShooting.selectedAmmoType == 0) {
					playerInGamePanel.Image_AmmoRadial.GetComponent<Image> ().fillAmount = (pShooting.regAmmo / 25);
					playerInGamePanel.Value_CurrentAmmo.text = Mathf.Round (pShooting.regAmmo).ToString ();
					playerInGamePanel.Label_CurrentAccuracy.text = zeroPercent;

					if (playerInGamePanel.Label_CurrentWeapon.text != weapPorter) {
						playerInGamePanel.Slider_CurrentAmmo.maxValue = 5;

						playerInGamePanel.Label_CurrentWeapon.text = weapPorter;
					}
				}

				//HMG
				if (pShooting.selectedAmmoType == 1) {
					playerInGamePanel.Image_AmmoRadial.GetComponent<Image> ().fillAmount = (pShooting.quadAmmo / 400);
					playerInGamePanel.Value_CurrentAmmo.text = Mathf.Round (pShooting.quadAmmo).ToString ();
					playerInGamePanel.Label_CurrentAccuracy.text = scoringManager.GetScore (PhotonNetwork.playerName, scoringManager.acc_HMGStat).ToString() + percent;

					if (playerInGamePanel.Label_CurrentWeapon.text != weapHMG) {
						playerInGamePanel.Slider_CurrentAmmo.maxValue = 400;

						playerInGamePanel.Label_CurrentWeapon.text = weapHMG;
					}
				}

				if (pShooting.selectedAmmoType == 5) {
					playerInGamePanel.Image_AmmoRadial.GetComponent<Image> ().fillAmount = (pShooting.rocketAmmo / 1);
					playerInGamePanel.Value_CurrentAmmo.text = Mathf.Round (pShooting.axeAmmo).ToString ();
					playerInGamePanel.Label_CurrentAccuracy.text = zeroPercent;

					//Do not need to keep updating these
					if (playerInGamePanel.Label_CurrentWeapon.text != weapAxe) {
						playerInGamePanel.Slider_CurrentAmmo.maxValue = 1;

						playerInGamePanel.Label_CurrentWeapon.text = weapAxe;
					}
				}


			}
			yield return waitFor0_25;
		}
	}
		
	IEnumerator HealthArmorDisplay_Coroutine(){
		while (true) {
			if (playerInGamePanel != null) {
				playerInGamePanel.Image_HealthRadial.GetComponent<Image> ().fillAmount = (pHealth.currentHitPoints / 200);
				playerInGamePanel.Image_ArmorRadial.GetComponent<Image> ().fillAmount = (pHealth.currentArmorPoints / 200);
				playerInGamePanel.Image_JetRadial.GetComponent<Image> ().fillAmount = (playerMovement.jetResource / 100);

				playerInGamePanel.Value_Health.text = ((int)pHealth.currentHitPoints).ToString ();
				playerInGamePanel.Value_Armor.text = ((int)pHealth.currentArmorPoints).ToString ();
				playerInGamePanel.Value_JetResource.text = ((int)playerMovement.jetResource).ToString ();
			}
			yield return waitFor0_1;
		}
	}


}