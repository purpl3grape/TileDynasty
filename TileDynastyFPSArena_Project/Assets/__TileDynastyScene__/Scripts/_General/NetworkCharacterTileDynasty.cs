using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkCharacterTileDynasty : Photon.MonoBehaviour, IPunObservable {

	NetworkManager nManager;
	ScoringManager sManager;
	PlayerMovement pMovement;
	PlayerShooting pShooting;
	Health pHealth;
	TeamMember networkedTeamMember;

	Transform tr;
	float currentDistance = 0f;
	float fullDistance = 0f;
	float progress = 0f;
	Vector3 startPosition = Vector3.zero;
	Vector3 networkedPlayerPosition = Vector3.zero;
	Quaternion networkedPlayerRotation = Quaternion.identity;

	float lerpTime = 1f;
	float currLerpTime = 0f;
	Vector3 networkedPlayerVelocity= Vector3.zero;
	float lastPackageReceivedTimeStamp=0f;
	float syncTime=0f;
	float syncDelay=0f;
	float velocityPredictionValue=0f;
	float clientPredictionValue = 0f;
	float syncDistanceValue=0f;

	public GameObject GunModel;
	public float weaponSelected = -1;

	int currHMGBulletFired = 0;
	int currRocketBulletFired = 0;
	int currRailBulletFired = 0;
	int currEMPBulletFired = 0;
	int currHMGBulletHit = 0;
	int currRocketBulletHit = 0;
	int currRailBulletHit = 0;
	int currEMPBulletHit = 0;
	int currBullsEyeCount = 0;
	int currMeleeHitCount = 0;
	int currHeadShotCount = 0;
	int currDamageReceivedCount = 0;
	int currDamageDealtCount = 0;
	int currDeathCount = 0;
	int currKillCount = 0;
	int currKillStreakCount = 0;
	int currKillStreakHighestCount = 0;
	string currLastPlayerKilledName = string.Empty;


	int HMGBulletFired = 0;
	int RocketBulletFired = 0;
	int RailBulletFired = 0;
	int EMPBulletFired = 0;
	int HMGBulletHit = 0;
	int RocketBulletHit = 0;
	int RailBulletHit = 0;
	int EMPBulletHit = 0;
	int BullsEyeCount = 0;
	int MeleeHitCount = 0;
	int HeadShotCount = 0;
	int DamageReceivedCount = 0;
	int DamageDealtCount = 0;

	int DeathCount = 0;
	int KillCount = 0;
	int KillStreakCount = 0;
	int KillStreakHighestCount = 0;
	string LastPlayerKilledName = string.Empty;


	public GameObject weaponHMG;
	public GameObject weaponRocket;
	public GameObject weaponGrenade;
	public GameObject weaponRail;
	public GameObject weaponPorter;
	public GameObject weaponAxe;

	public Material[] hmgMaterial;
	public Material[] rocketMaterial;
	public Material[] empMaterial;
	public Material[] railMaterial;
	public Material[] porterMaterial;
	public Material[] axeMaterial;
	public Texture hmgTexture;
	public Texture rocketTexture;
	public Texture empTexture;
	public Texture railTexture;
	public Texture porterTexture;

	public AudioSource audioSource;
	public AudioClip[] audioList;

	float pingInSeconds = 0f;
	float timeSinceLastUpdate = 0f;
	float totalTimePassedSinceLastUpdate = 0f;


	Animator anim;

	bool gotFirstUpdate = false;

	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator> ();
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		pMovement = GetComponent<PlayerMovement> ();
		pShooting = GetComponent<PlayerShooting> ();
		pHealth = GetComponent<Health> ();
		tr = GetComponent<Transform> ();

		if (anim == null) {
			Debug.LogError ("Forgot to put an animator Component on this character prefab");
		}
		//		PhotonNetwork.sendRate = 33;
		//		PhotonNetwork.sendRateOnSerialize = 20;
		//		PhotonNetwork.sendRate = 60;
		//		PhotonNetwork.sendRateOnSerialize = 40;
		if (PhotonNetwork.room.MaxPlayers <= 2) {
			PhotonNetwork.sendRate = 40;
			PhotonNetwork.sendRateOnSerialize = 20;
		} else if (PhotonNetwork.room.MaxPlayers > 2) {
			PhotonNetwork.sendRate = 20;
			PhotonNetwork.sendRateOnSerialize = 10;
		}

	}

	[PunRPC]
	public void DelayedPlayerPacketSyncPosition(){
		networkedPlayerPosition = tr.position;
	}


	bool hasJumped = false;

	// Update is called once per frame
	void Update () {
		clientPredictionValue = nManager.clientPredVal;
		velocityPredictionValue = nManager.velocityPredVal;
		syncDistanceValue = nManager.syncDisnceVal;



		if ( photonView.isMine ) {
			//Do nothing -- the character motor/input/etc.. is moving us
			//			if(syncTime > (syncDelay * 1f)){
			if(syncTime > 0.15f){
				//				GetComponent<PhotonView> ().RPC ("DelayedPlayerPacketSyncPosition", PhotonTargets.AllBuffered);
			}				

			anim.SetFloat (animSpeed, pMovement.playerVelocity.magnitude);
			if (pMovement.FootStepAudioSource) {
				if (anim.GetBool (animJump) == false) {
					if (!pMovement.FootStepAudioSource.isPlaying) {
						if (pMovement.playerVelocity.magnitude > 15f) {
							pMovement.FootStepAudioSource.PlayOneShot (audioList [1]);
						} else if (pMovement.playerVelocity.magnitude > 10f) {
							pMovement.FootStepAudioSource.PlayOneShot (audioList [0]);
						}
						if (hasJumped) {
							pMovement.FootStepAudioSource.Stop ();
							pMovement.FootStepAudioSource.PlayOneShot (audioList [3]);
							hasJumped = false;
						}
					}
				} else {
					if (!hasJumped) {
						pMovement.FootStepAudioSource.Stop ();
						pMovement.FootStepAudioSource.PlayOneShot (audioList [2]);
						hasJumped = true;
					}
				}
			}	


		} 
		else {

			pingInSeconds = (float)PhotonNetwork.GetPing () * 0.001f;
			timeSinceLastUpdate = (float)(PhotonNetwork.time - lastPackageReceivedTimeStamp);
			totalTimePassedSinceLastUpdate = pingInSeconds + timeSinceLastUpdate;

			currentDistance = Vector3.Distance (tr.position, networkedPlayerPosition);
			fullDistance = Vector3.Distance (startPosition, networkedPlayerPosition);
			if (fullDistance != 0) {
				progress = currentDistance / fullDistance;
			}

			if (clientPredictionValue != 0) {
				progress /= clientPredictionValue;
			}

			syncTime += Time.deltaTime;
			//			if (currentDistance > (syncDistanceValue / clientPredictionValue) || networkedPlayerVelocity.magnitude == 0f) {				
			if (networkedPlayerVelocity.magnitude == 0f) {
				tr.rotation = networkedPlayerRotation;
				tr.position = networkedPlayerPosition;
			} else {
				tr.rotation = Quaternion.Lerp (tr.rotation, networkedPlayerRotation, syncTime / syncDelay);
				tr.position = Vector3.Lerp (tr.position, networkedPlayerPosition, progress);
			}


			anim.SetFloat (animSpeed, networkedPlayerVelocity.magnitude);
			if (pMovement.FootStepAudioSource) {
				if (anim.GetBool (animJump) == false) {
					if (!pMovement.FootStepAudioSource.isPlaying) {
						if (networkedPlayerVelocity.magnitude > 15f) {
							pMovement.FootStepAudioSource.PlayOneShot (audioList [1]);
						} else if (networkedPlayerVelocity.magnitude > 10f) {
							pMovement.FootStepAudioSource.PlayOneShot (audioList [0]);
						}
					}
					if (hasJumped) {
						pMovement.FootStepAudioSource.Stop ();
						pMovement.FootStepAudioSource.PlayOneShot (audioList [3]);
						hasJumped = false;
					}
				} else {
					if (!hasJumped) {
						pMovement.FootStepAudioSource.Stop ();
						pMovement.FootStepAudioSource.PlayOneShot (audioList [2]);
						hasJumped = true;
					}
				}
			}	


		}
	}

	string animSpeed = "Speed";
	string animAimAngle = "AimAngle";
	string animHorizontalMovement = "HorizontalMovement";
	string animVerticalMovement = "VerticalMovement";
	string animJump = "Jump";
	string animGunStatus = "GunStatus";
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){

		if (stream.isWriting) {
			//This is OUR player. We need to send our actual position to the network.

			stream.SendNext (tr.position);
			stream.SendNext (tr.rotation);
			stream.SendNext (pMovement.playerVelocity);
			stream.SendNext (anim.GetFloat (animAimAngle));
			stream.SendNext (anim.GetFloat (animHorizontalMovement));
			stream.SendNext (anim.GetFloat (animVerticalMovement));
			stream.SendNext (anim.GetBool (animJump));
			stream.SendNext (anim.GetInteger (animGunStatus));

			stream.SendNext (pShooting.selectedAmmoType);

			//Weapon Stats
			stream.SendNext (pShooting.HMGFiredCount);
			stream.SendNext (pShooting.RocketFiredCount);
			stream.SendNext (pShooting.RailFiredCount);
			stream.SendNext (pShooting.EMPFiredCount);

			stream.SendNext (pShooting.HMGHitCount);
			stream.SendNext (pShooting.RocketHitCount);
			stream.SendNext (pShooting.RailHitCount);
			stream.SendNext (pShooting.EMPHitCount);

			stream.SendNext (pShooting.BullsEyeCount);
			stream.SendNext (pShooting.MeleeHitCount);
			stream.SendNext (pShooting.HeadShotCount);

			stream.SendNext (pHealth.DamageReceivedCount);
			stream.SendNext (pShooting.DamageDealtCount);
//
//			stream.SendNext (pHealth.DeathCount);
//			stream.SendNext (pHealth.KillCount);
//			stream.SendNext (pHealth.KillStreakCount);
//			stream.SendNext (pHealth.KillStreakHighestCount);
//			stream.SendNext (pHealth.LastPlayerKilledName);
		} else {

			if (networkedTeamMember == null) {
				sManager = nManager.GetComponent<ScoringManager> ();
				networkedTeamMember = GetComponent<TeamMember> ();
			}


			//Get the start position before the networkedPlayerPosition is updated again
			startPosition = networkedPlayerPosition;

			//The networkedPosition is now updated along with other networked variables
			networkedPlayerPosition = (Vector3)stream.ReceiveNext ();
			networkedPlayerRotation = (Quaternion)stream.ReceiveNext ();
			networkedPlayerVelocity = (Vector3)stream.ReceiveNext ();
			anim.SetFloat (animAimAngle, (float)stream.ReceiveNext ());
			anim.SetFloat (animHorizontalMovement, (float)stream.ReceiveNext ());
			anim.SetFloat (animVerticalMovement, (float)stream.ReceiveNext ());
			anim.SetBool (animJump, (bool)stream.ReceiveNext ());
			anim.SetInteger (animGunStatus, (int)stream.ReceiveNext ());

			weaponSelected = (float)stream.ReceiveNext ();


			HMGBulletFired = (int)stream.ReceiveNext ();
			RocketBulletFired = (int)stream.ReceiveNext ();
			RailBulletFired = (int)stream.ReceiveNext ();
			EMPBulletFired = (int)stream.ReceiveNext ();

			HMGBulletHit = (int)stream.ReceiveNext ();
			RocketBulletHit = (int)stream.ReceiveNext ();
			RailBulletHit = (int)stream.ReceiveNext ();
			EMPBulletHit = (int)stream.ReceiveNext ();

			BullsEyeCount = (int)stream.ReceiveNext ();
			MeleeHitCount = (int)stream.ReceiveNext ();
			HeadShotCount = (int)stream.ReceiveNext ();

			DamageReceivedCount = (int)stream.ReceiveNext ();
			DamageDealtCount = (int)stream.ReceiveNext ();
//
//			DeathCount = (int)stream.ReceiveNext ();
//			KillCount = (int)stream.ReceiveNext ();
//			KillStreakCount = (int)stream.ReceiveNext ();
//			KillStreakHighestCount = (int)stream.ReceiveNext ();
//			LastPlayerKilledName = (string)stream.ReceiveNext ();


//			if (currHMGBulletFired != HMGBulletFired)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shotfired_hmgStat, HMGBulletFired);
//			if (currRocketBulletFired != RocketBulletFired)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shotfired_rocketStat, RocketBulletFired);
//			if (currRailBulletFired != RailBulletFired)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shotfired_railStat, RailBulletFired);
//			if (currEMPBulletFired != EMPBulletFired)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shotfired_empStat, EMPBulletFired);

//			if (currHMGBulletHit != HMGBulletHit)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shothit_hmgStat, HMGBulletHit);
//			if (currRocketBulletHit != RocketBulletHit)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shothit_rocketStat, RocketBulletHit);
//			if (currRailBulletHit != RailBulletHit)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shothit_railStat, RailBulletHit);
//			if (currEMPBulletHit != EMPBulletHit)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shothit_empStat, EMPBulletHit);

//			if (currBullsEyeCount != BullsEyeCount)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shothit_bullseye, BullsEyeCount);
//			if (currMeleeHitCount != MeleeHitCount)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shothit_meleeStat, MeleeHitCount);
//			if (currHeadShotCount != HeadShotCount)
				sManager.SetScore (networkedTeamMember.playerName, sManager.shothit_headshotStat, HeadShotCount);


//			if (currDamageDealtCount != DamageDealtCount)
				sManager.SetScore (networkedTeamMember.playerName, sManager.damagedealtStat, DamageDealtCount);
//			if (currDamageReceivedCount != DamageReceivedCount)
				sManager.SetScore (networkedTeamMember.playerName, sManager.damagereceivedStat, DamageReceivedCount);


//			currHMGBulletFired = HMGBulletFired;
//			currRocketBulletFired = RocketBulletFired;
//			currRailBulletFired = RailBulletFired;
//			currEMPBulletFired = EMPBulletFired;
//
//			currHMGBulletHit = HMGBulletHit;
//			currRocketBulletHit = RocketBulletHit;
//			currRailBulletHit = RailBulletHit;
//			currEMPBulletHit = EMPBulletHit;
//
//			currBullsEyeCount = BullsEyeCount;
//			currMeleeHitCount = MeleeHitCount;
//			currHeadShotCount = HeadShotCount;
//
//			currDamageReceivedCount = DamageReceivedCount;
//			currDamageDealtCount = DamageDealtCount;
//			currDeathCount = DeathCount;
//			currKillCount = KillCount;
//			currKillStreakCount = KillStreakCount;
//			currKillStreakHighestCount = KillStreakHighestCount;
//			currLastPlayerKilledName = LastPlayerKilledName;

			if (weaponSelected == 0) {

				weaponHMG.SetActive (false);
				weaponRocket.SetActive (false);
				weaponGrenade.SetActive (false);
				weaponRail.SetActive (false);
				weaponPorter.SetActive (true);
				weaponAxe.SetActive (false);
				//				GunModel.GetComponent<SkinnedMeshRenderer> ().materials = porterMaterial;
			} else if (weaponSelected == 1) {

				weaponHMG.SetActive (true);
				weaponRocket.SetActive (false);
				weaponGrenade.SetActive (false);
				weaponRail.SetActive (false);
				weaponPorter.SetActive (false);
				weaponAxe.SetActive (false);
				//				GunModel.GetComponent<SkinnedMeshRenderer> ().materials = hmgMaterial;			
			} else if (weaponSelected == 2) {

				weaponHMG.SetActive (false);
				weaponRocket.SetActive (true);
				weaponGrenade.SetActive (false);
				weaponRail.SetActive (false);
				weaponPorter.SetActive (false);
				weaponAxe.SetActive (false);
				//				GunModel.GetComponent<SkinnedMeshRenderer> ().materials = rocketMaterial;			
			} else if (weaponSelected == 3) {

				weaponHMG.SetActive (false);
				weaponRocket.SetActive (false);
				weaponGrenade.SetActive (false);
				weaponRail.SetActive (true);
				weaponPorter.SetActive (false);
				weaponAxe.SetActive (false);
				//				GunModel.GetComponent<SkinnedMeshRenderer> ().materials = railMaterial;			
			} else if (weaponSelected == 4) {

				weaponHMG.SetActive (false);
				weaponRocket.SetActive (false);
				weaponGrenade.SetActive (true);
				weaponRail.SetActive (false);
				weaponPorter.SetActive (false);
				weaponAxe.SetActive (false);
				//				GunModel.GetComponent<SkinnedMeshRenderer> ().materials = empMaterial;			
			} else if (weaponSelected == 5) {

				weaponHMG.SetActive (false);
				weaponRocket.SetActive (false);
				weaponGrenade.SetActive (false);
				weaponRail.SetActive (false);
				weaponPorter.SetActive (false);
				weaponAxe.SetActive (true);
			}


			//When we receive an update here, our syncTime gets re-initialized to 0
			syncTime = 0f;
			syncDelay = Time.time - lastPackageReceivedTimeStamp;
			lastPackageReceivedTimeStamp = Time.time;

			//Velocity Prediction
			networkedPlayerPosition = networkedPlayerPosition + (networkedPlayerVelocity * syncDelay) * velocityPredictionValue;

			if (gotFirstUpdate == false) {
				tr.position = networkedPlayerPosition;
				tr.rotation = networkedPlayerRotation;
				gotFirstUpdate = true;
			}
		}
	}



}
