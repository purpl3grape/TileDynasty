using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour {

	public GameObject sceneScripts;
	GuiManager guiManager;
	PhotonView scenePV;
	MatchTimer mTimer;

	void Start(){
		guiManager = sceneScripts.GetComponent<GuiManager> ();
		mTimer = sceneScripts.GetComponent<MatchTimer> ();
		scenePV = sceneScripts.GetComponent<PhotonView> ();
	}

	string strPlayer = "Player";
	string SetGameEndRPC = "SetGameEnd";
	void OnTriggerEnter(Collider other){

		if (!PhotonNetwork.isMasterClient)
			return;
		if (!mTimer.isReady)
			return;

		if (other.CompareTag (strPlayer)) {
//			guiManager.GAMEEND = true;

			if (guiManager.currentGameMode == 2) {

				if (guiManager.HostagesRescued > 0) {
					if (!guiManager.GAMEEND)
						scenePV.RPC (SetGameEndRPC, PhotonTargets.AllBuffered, true);
				}
			}
//			enemyMovement = other.GetComponent<EnemyMovement> ();
//			if (enemyMovement) {
//				enemyMovement.isFollowingRecruiter = false;
//				enemyMovement.isHostageRescued = true;
//				if (enemyMovement.lastRescueTarget) {
//					enemyMovement.lastRescueTarget.GetComponent<PhotonView> ().RPC ("SetRescuedHostage", PhotonTargets.AllBuffered);
//					scenePV.RPC ("ChangeScore", PhotonTargets.AllBuffered, enemyMovement.lastRescueTarget.GetComponent<TeamMember> ().GetPlayerName (), "hostagescaptured", 1);
//					scenePV.RPC ("DecreaseHostagesRemaining", PhotonTargets.AllBuffered);
//					hostageList [hostageCount] = other.gameObject;
//					hostageCount += 1;
//				}
//				enemyMovement.lastRescueTarget = null;
//			}
		}

	}

}
