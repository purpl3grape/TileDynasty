using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashCanBehaviour : MonoBehaviour {

	ScoringManager scoringManager;
	NetworkManager nManager;
	MatchTimer mTimer;
	InGamePanel inGamePanel;
	public int currentBlueTrash;
	public int currentRedTrash;
	public AudioSource trashCanAudioSource;
	public AudioClip[] trashCanSound;

	// Use this for initialization
	void Start () {
		scoringManager = GameObject.FindGameObjectWithTag("SceneScripts").GetComponent<ScoringManager> ();
		nManager = scoringManager.GetComponent<NetworkManager> ();
		mTimer = scoringManager.GetComponent<MatchTimer> ();
		inGamePanel = GameObject.FindObjectOfType<InGamePanel> ();
	}

	void OnTriggerEnter(Collider other){

		if (!mTimer.isReady)
			return;
		if (!(mTimer.SecondsUntilItsTime > 0))
			return;
		if (!PhotonNetwork.isMasterClient)
			return;
		if (!other.GetComponent<TeamMember> ())
			return;
		if (other.CompareTag ("Player")) {
			if (scoringManager.GetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagecollected") == 0) {
				return;
			}
		} else if (other.CompareTag ("Enemy")) {
			if (scoringManager.GetScore (other.name, "garbagecollected") == 0) {
				return;
			}
		}
		if (other.GetComponent<TeamMember> ().teamID == 1) {
			if (other.CompareTag ("Player")) {
				scoringManager.SetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagetrashed", scoringManager.GetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagetrashed") + scoringManager.GetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagecollected"));
				currentBlueTrash += scoringManager.GetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagecollected");
			} else if (other.CompareTag ("Enemy")) {
				scoringManager.SetScore (other.name, "garbagetrashed", scoringManager.GetScore (other.name, "garbagetrashed") + scoringManager.GetScore (other.name, "garbagecollected"));
				currentBlueTrash += scoringManager.GetScore (other.name, "garbagecollected");
			}
		} else if (other.GetComponent<TeamMember> ().teamID == 2) {
			if (other.CompareTag ("Player")) {
				scoringManager.SetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagetrashed", scoringManager.GetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagetrashed") + scoringManager.GetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagecollected"));
				currentRedTrash += scoringManager.GetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagecollected");
			} else if (other.CompareTag ("Enemy")) {
				scoringManager.SetScore (other.name, "garbagetrashed", scoringManager.GetScore (other.name, "garbagetrashed") + scoringManager.GetScore (other.name, "garbagecollected"));
				currentRedTrash += scoringManager.GetScore (other.name, "garbagecollected");
			}
		}

		if(other.CompareTag("Player")){
			nManager.AddChatMessage (other.GetComponent<TeamMember> ().GetPlayerName () + " Scored " + scoringManager.GetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagecollected") + " points [ " + scoringManager.GetScore (other.GetComponent<TeamMember> ().GetPlayerName (), "garbagetrashed") + " ]");
		}
		else if(other.CompareTag("Enemy")){
			nManager.AddChatMessage (other.name + " Scored " + scoringManager.GetScore (other.name, "garbagecollected") + " points [ " + scoringManager.GetScore (other.name, "garbagetrashed") + " ]");
		}

		if (other.CompareTag ("Enemy")){
			scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, other.name, "garbagecollected");
		}
		else if (other.CompareTag ("Player")){
			scoringManager.GetComponent<PhotonView> ().RPC ("ResetScore", PhotonTargets.AllBuffered, other.GetComponent<TeamMember>().GetPlayerName(), "garbagecollected");
		}

		trashCanAudioSource.PlayOneShot (trashCanSound [0]);

	}
}
