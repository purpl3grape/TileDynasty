using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageResueZoneTrigger : MonoBehaviour {

	EnemyMovement enemyMovement;
	public GameObject sceneScripts;
	GuiManager guiManager;
	ScoringManager scoringManager;
	PhotonView scenePV;
	MatchTimer mTimer;

	public int hostageCount = 0;
	public GameObject[] hostageList = new GameObject[5];

	void Start(){
		sceneScripts = GameObject.FindGameObjectWithTag ("SceneScripts");
		guiManager = sceneScripts.GetComponent<GuiManager> ();
		scoringManager = sceneScripts.GetComponent<ScoringManager> ();
		mTimer = sceneScripts.GetComponent<MatchTimer> ();
		scenePV = sceneScripts.GetComponent<PhotonView> ();
	}

	string msgRescued = "RESCUED ";
	string ChangeScoreRPC = "ChangeScore";
	string SetRescuedHostageRPC = "SetRescuedHostage";
	string DecreaseHostagesRemainingRPC = "DecreaseHostagesRemaining";

	void OnTriggerEnter(Collider other){

		if (!mTimer.isReady)
			return;

		if (other.GetComponent<EnemyMovement> () != null) {
			enemyMovement = other.GetComponent<EnemyMovement> ();
			if (other.GetComponent<TeamMember> ().teamID == 3 && !enemyMovement.isHostageRescued) {
				guiManager.ObjMsgCO = guiManager.ObjMsg_CO (msgRescued + other.gameObject.name);
				StartCoroutine (guiManager.ObjMsgCO);
			}
		}

		if (!PhotonNetwork.isMasterClient)
			return;

		if (other.GetComponent<EnemyMovement> () != null) {
			if (other.GetComponent<TeamMember> ().teamID == 3 && !enemyMovement.isHostageRescued) {
				enemyMovement.isFollowingRecruiter = false;
				enemyMovement.isHostageRescued = true;
				if (enemyMovement.lastRescueTarget) {
					enemyMovement.lastRescueTarget.GetComponent<PhotonView> ().RPC (SetRescuedHostageRPC, PhotonTargets.AllBuffered);
					scenePV.RPC (ChangeScoreRPC, PhotonTargets.AllBuffered, enemyMovement.lastRescueTarget.GetComponent<TeamMember> ().GetPlayerName (), scoringManager.hostagescapturedStat, 1);
					scenePV.RPC (DecreaseHostagesRemainingRPC, PhotonTargets.AllBuffered);
					hostageList [hostageCount] = other.gameObject;
					hostageCount += 1;

				}
				enemyMovement.lastRescueTarget = null;
			}
		}

	}

}
