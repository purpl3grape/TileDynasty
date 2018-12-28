using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerBehavior : MonoBehaviour {

	public Health health;
	public EnemyMovement movement;
	public GameObject parentObject;
	public PhotonView pv;
	public GameObject triggerObject;
	public bool isTriggerEnterObject = false;

	NetworkManager nManager;
	GuiManager guiManager;
	MatchTimer mTimer;

	void Start(){
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		guiManager = nManager.GetComponent<GuiManager> ();
		mTimer = nManager.GetComponent<MatchTimer> ();
	}

	string SetIsFollowingPlayerRPC = "SetIsFollowingPlayer";
	string SetIsRecruitingNPCRPC = "SetIsRecruitingNPC";
	void FixedUpdate(){
		if (!PhotonNetwork.isMasterClient)
			return;
		if (mTimer == null)
			return;
		if (!mTimer.isReady)
			return;

		if (triggerObject) {
			if (triggerObject.GetComponent<Health> ().isPlayerDead) {
				movement.enemyPhotonView.RPC (SetIsFollowingPlayerRPC, PhotonTargets.AllBuffered, false, -9);
				triggerObject = null;
			}
		}
		if (health.enemyHitPoints <= 0) {
			if (movement.isFollowingRecruiter) {
				movement.enemyPhotonView.RPC (SetIsFollowingPlayerRPC, PhotonTargets.AllBuffered, false, -9);
			}
			if (triggerObject) {
				triggerObject.GetComponent<PhotonView> ().RPC (SetIsRecruitingNPCRPC, PhotonTargets.AllBuffered, string.Empty, false, false);
				triggerObject = null;
			}
		}

		if (triggerObject) {
			HostageTriggerEnter (triggerObject);
		}


	}

	void OnTriggerEnter(Collider other){

		if (!PhotonNetwork.isMasterClient)
			return;
		if (mTimer == null)
			return;
		if (!mTimer.isReady)
			return;
		
		if (other.CompareTag (strPlayer)) {			
			if ((int)other.GetComponent<Health> ().currentHitPoints > 0) {
				triggerObject = other.gameObject;
			}
		}


	}

	void OnTriggerStay(Collider other){		
		if (other.CompareTag (strPlayer)) {	
			if ((int)other.GetComponent<Health> ().currentHitPoints > 0) {
				triggerObject = other.gameObject;
			}
		}
	}

	string strHostage = "Hostage_";
	string strPlayer = "Player";
	void HostageTriggerEnter(GameObject other){
		if (movement.isHostageRescued)
			return;

		if (!parentObject.name.StartsWith (strHostage))
			return;
		
		if (other.CompareTag (strPlayer)) {
			//TRUE and Give Player's PhotonID
			//So the ParentObject (Hostage) will be looking for players with that ViewID to follow

			if (!movement.isFollowingRecruiter) {
				if ((int)health.enemyHitPoints > 0) {
					if (!other.GetComponent<Health> ().isPlayerDead) {
						pv.RPC (SetIsFollowingPlayerRPC, PhotonTargets.AllBuffered, true, other.GetComponent<PhotonView> ().viewID);
						if (movement.subscribedEnemySpawnTrigger) {
							movement.subscribedEnemySpawnTrigger.hasAbandonedSurvivor = false;
							if (movement.subscribedEnemySpawnTrigger.killThresholdMet) {
								other.GetComponent<PhotonView> ().RPC (SetIsRecruitingNPCRPC, PhotonTargets.AllBuffered, parentObject.name, true, true);
							} else {
								other.GetComponent<PhotonView> ().RPC (SetIsRecruitingNPCRPC, PhotonTargets.AllBuffered, parentObject.name, true, false);
							}
						}
					}
				}
			}
		}

	}
		
	void OnTriggerExit(Collider other){

		if (!PhotonNetwork.isMasterClient)
			return;
		if (!mTimer.isReady)
			return;
		if (movement.isHostageRescued)
			return;

	}

}
