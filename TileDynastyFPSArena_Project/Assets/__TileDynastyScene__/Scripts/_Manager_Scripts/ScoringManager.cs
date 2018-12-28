using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoringManager : MonoBehaviour {

	Dictionary< string, Dictionary< string, int>> playerScores;
	Dictionary< string, Dictionary< string, string>> playerKilledLast;
	GuiManager gManager;

	int changeCounter = 0;

	public string teamStat = "team";
	public string hostagescapturedStat = "hostagescaptured";

	public string shotfired_hmgStat = "shotfired_hmg";
	public string shotfired_rocketStat = "shotfired_rocket";
	public string shotfired_empStat = "shotfired_emp";
	public string shotfired_railStat = "shotfired_rail";
	public string shothit_hmgStat = "shothit_hmg";
	public string shothit_rocketStat = "shothit_rocket";
	public string shothit_empStat = "shothit_emp";
	public string shothit_railStat = "shothit_rail";
	public string shothit_Stat = "shothit_melee";
	public string shothit_bullseye = "shothit_bullseye";
	public string shothit_meleeStat = "shothit_melee";
	public string shothit_headshotStat = "shothit_headshot";

	public string damagereceivedStat = "damager";
	public string damagedealtStat = "damaged";
	public string killsStat = "kills";
	public string deathsStat = "deaths";
	public string killstreakStat = "killstreak";
	public string killstreak_highestStat = "killstreak_highest";
	public string name_lastplayerkilledStat = "name_lastplayerkilled";
	public string name_killedbyplayerStat = "name_killedbyplayer";
	public string pingStat = "ping";

	public string pickup_healthStat = "pickup_health";
	public string pickup_armorStat = "pickup_armor";
	public string pickup_hmgStat = "pickup_hmg";
	public string pickup_rocketStat = "pickup_rocket";
	public string pickup_empStat = "pickup_emp";
	public string pickup_railStat = "pickup_rail";
	public string pickup_porterStat = "pickup_porter";


	public string acc_overallStat = "acc_overall";
	public string acc_RocketStat = "acc_rocket";
	public string acc_GrenadeStat = "acc_emp";
	public string acc_railStat = "acc_rail";
	public string acc_HMGStat = "acc_hmg";


	void Start(){
		gManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<GuiManager> ();
		StartCoroutine (UpdatePlayerLeaderBoard_Coroutine ());
	}


	void Init(){
		if(playerScores != null){
			return;
		}
		playerScores = new Dictionary< string, Dictionary< string, int>>();

		if(playerKilledLast != null){
			return;
		}
		playerKilledLast = new Dictionary< string, Dictionary< string, string>>();

	}
		
	void OnPhotonPlayerDisconnected(PhotonPlayer player){
		RemovePlayer (player.name);
	}

	public void RemovePlayer(string playerKey){
		Debug.Log ("Player: " + playerKey + " disconnected.");
		if (playerScores != null) {
			playerScores.Remove (playerKey);
		}
	}

	public int GetScore(string username, string scoreType){
		Init ();
		if (playerScores.ContainsKey (username) == false) {
			//We have no score record at all for this username
			return 0;
		}

		if(playerScores[username].ContainsKey (scoreType) == false){
			return 0;
		}

		return playerScores [username] [scoreType];
	}

	public string GetKilledLast(string username, string scoreType){
		Init ();
		if (playerKilledLast.ContainsKey (username) == false) {
			//We have no score record at all for this username
			return string.Empty;
		}
		if(playerKilledLast[username].ContainsKey (scoreType) == false){
			return string.Empty;
		}
		return playerKilledLast [username] [scoreType];
	}


	/// <summary>
	/// Sets the score.
	/// </summary>
	/// <param name="username">Username.</param>
	/// <param name="scoreType">Score type.</param>
	/// <param name="value">Value.</param>


	[PunRPC]
	public void PlayerStatsResetScore(string username){
		Init ();
		changeCounter++;
		//KILLS
		if(playerScores.ContainsKey(username) == false){
			playerScores[username] = new Dictionary<string, int>();
		}
		playerScores [username] ["kills"] = 0;

		//DEATHS
		if(playerScores.ContainsKey(username) == false){
			playerScores[username] = new Dictionary<string, int>();
		}
		playerScores [username] ["deaths"] = 0;

		//KILLSTREAKS
		if(playerScores.ContainsKey(username) == false){
			playerScores[username] = new Dictionary<string, int>();
		}
		playerScores [username] ["killstreak"] = 0;

		//GARBAGE COLLECTED
		if(playerScores.ContainsKey(username) == false){
			playerScores[username] = new Dictionary<string, int>();
		}
		playerScores [username] ["garbagecollected"] = 0;

	}


	[PunRPC]
	public void SetScore(string username, string scoreType, int value){
		Init ();
		changeCounter++;
		if(playerScores.ContainsKey(username) == false){
			playerScores[username] = new Dictionary<string, int>();
		}
		playerScores [username] [scoreType] = value;
	}

	[PunRPC]
	public void SetScore(string username, string scoreType, string value){
		Init ();
		changeCounter++;
		if(playerKilledLast.ContainsKey(username) == false){
			playerKilledLast[username] = new Dictionary<string, string>();
		}
		playerKilledLast [username] [scoreType] = value;
	}


	[PunRPC]
	public void UpdateStatsForKiller(string killerName, int killAmount, string deadPlayerName){
		Init ();
		if(playerKilledLast.ContainsKey(killerName) == false){
			playerKilledLast[killerName] = new Dictionary<string, string>();
		}
		playerKilledLast [killerName] ["name_lastplayerkilled"] = deadPlayerName;

		int currScore = GetScore (killerName, "kills");
		//KILL
		if(playerScores.ContainsKey(killerName) == false){
			playerScores[killerName] = new Dictionary<string, int>();
		}
		playerScores [killerName] ["kills"] = (currScore + killAmount);

		currScore = GetScore (killerName, "killstreak");
		//KILLSTREAK
		if(playerScores.ContainsKey(killerName) == false){
			playerScores[killerName] = new Dictionary<string, int>();
		}
		playerScores [killerName] ["killstreak"] = (currScore + killAmount);
	}

	[PunRPC]
	public void SetLastKilled(string username, string scoreType, string value){
		Init ();
		if(playerKilledLast.ContainsKey(username) == false){
			playerKilledLast[username] = new Dictionary<string, string>();
		}
		playerKilledLast [username] [scoreType] = value;
	}

	[PunRPC]
	public void ChangeScore(string username, string scoreType, int amount){
		Init ();

		int currScore = GetScore (username, scoreType);
		GetComponent<PhotonView>().RPC("SetScore",PhotonTargets.AllBuffered, username, scoreType, currScore + amount);
		//SetScore (username, scoreType, currScore + amount);
	}

	[PunRPC]
	public void ResetScore(string username, string scoreType){
		Init ();
		GetComponent<PhotonView>().RPC("SetScore",PhotonTargets.AllBuffered, username, scoreType, 0);
		//SetScore (username, scoreType, currScore + amount);
	}
	[PunRPC]
	public void ResetLastKilled(string username, string scoreType){
		Init ();
		GetComponent<PhotonView>().RPC("SetLastKilled",PhotonTargets.AllBuffered, username, scoreType, "");
	}


	public string[] GetPlayerNames(){
		Init ();
		return playerScores.Keys.ToArray();
	}

	IEnumerator UpdatePlayerLeaderBoard_Coroutine(){
		while (true) {
			if (gManager.currentGameMode == 1 || gManager.currentGameMode == 2) {
				GetPlayerNames ("kills");
			}
			if (gManager.currentGameMode == 3) {
				GetPlayerNames ("garbagetrashed");
			}
			if (gManager.currentGameMode == 0) {
				GetPlayerNames ("tilescaptured");
			}
			yield return new WaitForSeconds (5f);
		}
	}

	public string[] GetPlayerNames(string sortingScoreType){
		Init ();

		return playerScores.Keys.OrderByDescending(n => GetScore (n, sortingScoreType)).ToArray();
	}

	public int GetChangeCounter(){
		return changeCounter;
	}

}