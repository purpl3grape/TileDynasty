using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScoreList : MonoBehaviour {

	WaitForSeconds waitFor2 = new WaitForSeconds(2f);


	public GameObject playerScoreEntryPrefab_TDM;
	public GameObject playerScoreEntryPrefab_Survival;
	public GameObject playerScoreEntryPrefab_TILES;
	public GameObject playerScoreEntryPrefab_CTF;

	public GameObject TopKills;
	public GameObject TopKills_Name;
	public GameObject TopKillStreak;
	public GameObject TopKillStreak_Name;
	public GameObject TopAccuracy;
	public GameObject TopAccuracy_Name;
	public GameObject TopDamage;
	public GameObject TopDamage_Name;

	public GameObject LabelTopTilePoint;
	public GameObject TopTilePoint;
	public GameObject TopTilePoint_Name;
	public GameObject Image_TopTilePoint;

	public GameObject LabelTopTDMPoints;
	public GameObject TopTDMPoints;
	public GameObject TopTDMPoints_Name;
	public GameObject Image_TopTDMPoints;

	public GameObject LabelTopGarbagePoint;
	public GameObject TopGarbagePoint;
	public GameObject TopGarbagePoint_Name;
	public GameObject Image_TopGarbagePoint;

	[HideInInspector] public float accuracy_hmg=0;
	[HideInInspector] public float accuracy_rocket=0;
	[HideInInspector] public float accuracy_rail=0;
	[HideInInspector] public float accuracy_emp=0;
	[HideInInspector] public float accuracy_overall=0;
	[HideInInspector] public float totalShotsFired=0;
	[HideInInspector] public float totalShotsHit=0;
	[HideInInspector] public float totalMeleeHit=0;
	[HideInInspector] public float totalBullsEyeHit=0;
	[HideInInspector] public float totalHeadShotHit=0;

	GuiManager guiManager;

	bool playerDisconnect = false;

	ScoringManager scoringManager;
	NetworkManager nManager;
	MatchTimer matchTimer;
	WindowManager windowManager;

	Vector3 small = new Vector3 (1f, 1f, 1f);

	RectTransform playerScoreLayoutElement;
	//Vector2 dynamicSize = new Vector2(Screen.width / 2f, Screen.height / 2f);

	// Use this for initialization
	void Start () {

		scoringManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<ScoringManager> ();
		nManager = scoringManager.GetComponent<NetworkManager> ();
		guiManager = scoringManager.GetComponent<GuiManager> ();
		matchTimer = scoringManager.GetComponent<MatchTimer> ();
		windowManager = GameObject.FindObjectOfType<WindowManager> ();

		LabelTopTilePoint.SetActive (false);
		LabelTopGarbagePoint.SetActive (false);
		LabelTopTDMPoints.SetActive (false);
		TopTilePoint.SetActive (false);
		TopTilePoint_Name.SetActive (false);
		TopGarbagePoint.SetActive (false);
		TopGarbagePoint_Name.SetActive (false);
		TopTDMPoints.SetActive (false);
		TopTDMPoints_Name.SetActive (false);
		Image_TopTilePoint.SetActive (false);
		Image_TopGarbagePoint.SetActive (false);
		Image_TopTDMPoints.SetActive (false);
	}

	void OnEnable(){
		StartCoroutine (UpdateListPlayersByKill_Coroutine ());
	}	

	IEnumerator UpdateListPlayersByKill_Coroutine(){
		while (true) {
			listPlayersByKill ();
			yield return waitFor2;
		}
	}

	public void listPlayersByKill(){

		if (scoringManager == null)
			return;

		Transform c;
		while (this.transform.childCount > 0) {
			c = this.transform.GetChild (0);
			c.SetParent (null);
			Destroy (c.gameObject);
		}

		string[] names = scoringManager.GetPlayerNames (scoringManager.killsStat);

		names = scoringManager.GetPlayerNames (scoringManager.acc_overallStat);
		TopAccuracy_Name.GetComponent<Text> ().text = names [0];
		TopAccuracy.GetComponent<Text> ().text = scoringManager.GetScore (names [0], scoringManager.acc_overallStat).ToString() + strPercent;
		names = scoringManager.GetPlayerNames (scoringManager.damagedealtStat);
		TopDamage_Name.GetComponent<Text> ().text = names [0];
		TopDamage.GetComponent<Text> ().text = scoringManager.GetScore (names [0], scoringManager.damagedealtStat).ToString();
		names = scoringManager.GetPlayerNames (scoringManager.killsStat);
		TopKills_Name.GetComponent<Text> ().text = names [0];
		TopKills.GetComponent<Text> ().text = scoringManager.GetScore (names [0], scoringManager.killsStat).ToString();

		//SEARCH AND RESCUE
		if (guiManager.currentGameMode == 1 || guiManager.currentGameMode == 2 || guiManager.currentGameMode == 99) {
			names = scoringManager.GetPlayerNames (scoringManager.killsStat);
		}
		string currName = string.Empty;

		if (guiManager.currentGameMode == 2) {
			InstantiatePlayerScorePrefabs (currName, names, 0);
		} else if (guiManager.currentGameMode == 1) {
			InstantiatePlayerScorePrefabs (currName, names, 1);
			InstantiatePlayerScorePrefabs (currName, names, 2);
		} else if (guiManager.currentGameMode == 99) {
			InstantiatePlayerScorePrefabs (currName, names, 0);
		}

	}

	string strPlayer = "Player";
	string strEnemy = "Enemy";
	string strPercent = " %";
	string strHyphen = "--";

	GameObject [] enemyList;
	GameObject[] playerList;
	GameObject[] PlayerEnemyList;
	void GetPlayerAndEnemyList(){
		enemyList = GameObject.FindGameObjectsWithTag (strEnemy);
		playerList = GameObject.FindGameObjectsWithTag (strPlayer);
	}

	GameObject ReturnCurrentPlayerOrEnemyFromList(GameObject[] list, string currentName, string objTag){
		if (list == null)
			return null;
		foreach (GameObject item in list) {
			if (objTag.Equals (strEnemy)) {
				if (item.name == currentName) {
					return item;
				}
			} else {
				if (item.GetComponent<TeamMember> ().playerName.Equals (currentName)) {
					return item;
				}
			}
		}
		return null;
	}

	GameObject PlayerOrEnemyObject;
	GameObject currentPlayer;
	void InstantiatePlayerScorePrefabs(string currName, string[]names, int teamID){
		foreach (string name in names) {
			playerDisconnect = false;
			currName = name;
			TeamMember playerTeamMember;
			foreach (PhotonPlayer p in PhotonNetwork.playerList) {
				if (p.name == name) {
					playerDisconnect = true;
				}
			}

			currentPlayer = null;

			playerList = null;
			enemyList = null;
			GetPlayerAndEnemyList ();

			PlayerOrEnemyObject = ReturnCurrentPlayerOrEnemyFromList (playerList, currName, strPlayer);
			if (PlayerOrEnemyObject) {
				currentPlayer = PlayerOrEnemyObject;
				if (PlayerOrEnemyObject.GetComponent<TeamMember> ().isBot)
					return;
				if (PlayerOrEnemyObject.GetComponent<TeamMember> ().teamID != teamID)
					continue;
			}
			PlayerOrEnemyObject = ReturnCurrentPlayerOrEnemyFromList (enemyList, currName, strEnemy);
			if (PlayerOrEnemyObject) {
				if (PlayerOrEnemyObject.GetComponent<TeamMember> ().teamID != teamID)
					continue;
			}
			if (teamID != scoringManager.GetScore (name, scoringManager.teamStat)) {
				continue;
			}


			GameObject go;
			StatList statList;
			if (guiManager.currentGameMode == 0) {
				go = (GameObject)Instantiate (playerScoreEntryPrefab_TILES);	
			} else if (guiManager.currentGameMode == 3) {
				go = (GameObject)Instantiate (playerScoreEntryPrefab_CTF);	
			} else if (guiManager.currentGameMode == 1 || guiManager.currentGameMode == 99) {
				go = (GameObject)Instantiate (playerScoreEntryPrefab_TDM);
			} else {
				go = (GameObject)Instantiate (playerScoreEntryPrefab_Survival);
			}
			go.transform.SetParent (this.transform);
			statList = go.GetComponent<StatList> ();

			//Must be same order of PlayerScoreEntry Prefab

			statList.UserName.GetComponent<Text> ().text = name.ToUpper ();

			//TEAM PLAYER IMAGE COLOR
			if (scoringManager.GetScore (name, scoringManager.teamStat) == 1)
				statList.Team.GetComponent<Image> ().color = guiManager.blueTeamScoreColor;
			if (scoringManager.GetScore (name, scoringManager.teamStat) == 2 || scoringManager.GetScore (name, scoringManager.teamStat) == 3 || scoringManager.GetScore (name, scoringManager.teamStat) == 4)
				statList.Team.GetComponent<Image> ().color = guiManager.redTeamScoreColor;				
			if (scoringManager.GetScore (name, scoringManager.teamStat) == 0)
				statList.Team.GetComponent<Image> ().color = guiManager.greenTimeColor;

			//NAME COLOR
			if (PhotonNetwork.player.name == name)
				statList.UserName.GetComponent<Text> ().color = guiManager.greenTimeColor;

			//TDM AND SURVIVAL MODE: HOSTAGE RESCUE POINTS
			if (guiManager.currentGameMode == 2) {
				statList.Capture.GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.hostagescapturedStat).ToString ();
			}
			//KILLS / DEATHS / DAMAGE D / DAMAGE R
			statList.Kills.GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.killsStat).ToString ();
			statList.Deaths.GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.deathsStat).ToString ();


			statList.DamageD.GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.damagedealtStat).ToString ();
			statList.DamageR.GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.damagereceivedStat).ToString ();
			//ACCURACY CALCULATIONS
			totalShotsFired = (float)scoringManager.GetScore (name, scoringManager.shotfired_hmgStat) +
				(float)scoringManager.GetScore (name, scoringManager.shotfired_rocketStat) +
				(float)scoringManager.GetScore (name, scoringManager.shotfired_railStat) +
				(float)scoringManager.GetScore (name, scoringManager.shotfired_empStat);
			totalShotsHit = (float)scoringManager.GetScore (name, scoringManager.shothit_hmgStat) +
				(float)scoringManager.GetScore (name, scoringManager.shothit_rocketStat) +
				(float)scoringManager.GetScore (name, scoringManager.shothit_railStat) +
				(float)scoringManager.GetScore (name, scoringManager.shothit_empStat);
			accuracy_overall = totalShotsHit / totalShotsFired * 100;
			if (totalShotsFired < 1) {
				accuracy_overall = 0f;
			}
			scoringManager.SetScore (name, scoringManager.acc_overallStat, Mathf.RoundToInt (accuracy_overall));
			statList.AccOverall.GetComponent<Text> ().text = accuracy_overall == 0 ? strHyphen : scoringManager.GetScore (name, scoringManager.acc_overallStat).ToString () + strPercent;

			accuracy_hmg = (float)scoringManager.GetScore (name, scoringManager.shotfired_hmgStat) == 0 ? 0 : (float)scoringManager.GetScore (name, scoringManager.shothit_hmgStat) / (float)scoringManager.GetScore (name, scoringManager.shotfired_hmgStat) * 100;	
			scoringManager.SetScore (name, scoringManager.acc_HMGStat, Mathf.RoundToInt (accuracy_hmg));
			statList.AccHMG.GetComponent<Text> ().text = accuracy_hmg == 0 ? strHyphen : scoringManager.GetScore (name, scoringManager.acc_HMGStat).ToString () + strPercent;

			accuracy_rocket = (float)scoringManager.GetScore (name, scoringManager.shotfired_rocketStat) == 0 ? 0 : (float)scoringManager.GetScore (name, scoringManager.shothit_rocketStat) / (float)scoringManager.GetScore (name, scoringManager.shotfired_rocketStat) * 100;
			scoringManager.SetScore (name, scoringManager.acc_RocketStat, Mathf.RoundToInt (accuracy_rocket));
			statList.AccRocket.GetComponent<Text> ().text = accuracy_rocket == 0 ? strHyphen : scoringManager.GetScore (name, scoringManager.acc_RocketStat).ToString () + strPercent;

			accuracy_rail = (float)scoringManager.GetScore (name, scoringManager.shotfired_railStat) == 0 ? 0 : (float)scoringManager.GetScore (name, scoringManager.shothit_railStat) / (float)scoringManager.GetScore (name, scoringManager.shotfired_railStat) * 100;
			scoringManager.SetScore (name, scoringManager.acc_railStat, Mathf.RoundToInt (accuracy_rail));
			statList.AccRail.GetComponent<Text> ().text = accuracy_rail == 0 ? strHyphen : scoringManager.GetScore (name, scoringManager.acc_railStat).ToString () + strPercent;

			accuracy_emp = (float)scoringManager.GetScore (name, scoringManager.shotfired_empStat) == 0 ? 0 : (float)scoringManager.GetScore (name, scoringManager.shothit_empStat) / (float)scoringManager.GetScore (name, scoringManager.shotfired_empStat) * 100;
			scoringManager.SetScore (name, scoringManager.acc_GrenadeStat, Mathf.RoundToInt (accuracy_emp));
			statList.AccEMP.GetComponent<Text> ().text = accuracy_emp == 0 ? strHyphen : scoringManager.GetScore (name, scoringManager.acc_GrenadeStat).ToString () + strPercent;
			//SHOTFIRED / SHOTHIT FOR ACCURACY
			go.transform.Find ("ShotFired_HMG").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shotfired_hmgStat).ToString ();
			go.transform.Find ("ShotFired_Rocket").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shotfired_rocketStat).ToString ();
			go.transform.Find ("ShotFired_Rail").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shotfired_railStat).ToString ();
			go.transform.Find ("ShotFired_EMP").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shotfired_empStat).ToString ();
			go.transform.Find ("ShotHit_HMG").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shothit_hmgStat).ToString ();
			go.transform.Find ("ShotHit_Rocket").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shothit_rocketStat).ToString ();
			go.transform.Find ("ShotHit_Rail").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shothit_railStat).ToString ();
			go.transform.Find ("ShotHit_EMP").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shothit_empStat).ToString ();
			go.transform.Find ("ShotHit_Melee").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shothit_meleeStat).ToString ();
			go.transform.Find ("ShotHit_BullsEye").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shothit_bullseye).ToString ();
			go.transform.Find ("ShotHit_HeadShot").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.shothit_headshotStat).ToString ();
			//REVENGE
			go.transform.Find ("Name_LastPlayerKilled").GetComponent<Text> ().text = scoringManager.GetKilledLast (name, scoringManager.name_lastplayerkilledStat).ToString ();
			go.transform.Find ("Name_KilledByPlayer").GetComponent<Text> ().text = scoringManager.GetKilledLast (name, scoringManager.name_killedbyplayerStat).ToString ();
			//PICKUPS
			go.transform.Find ("Pickup_HMG").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.pickup_hmgStat).ToString ();
			go.transform.Find ("Pickup_Rocket").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.pickup_rocketStat).ToString ();
			go.transform.Find ("Pickup_Rail").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.pickup_railStat).ToString ();
			go.transform.Find ("Pickup_EMP").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.pickup_empStat).ToString ();
			go.transform.Find ("Pickup_Porter").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.pickup_porterStat).ToString ();
			go.transform.Find ("Pickup_Health").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.pickup_healthStat).ToString ();
			go.transform.Find ("Pickup_Armor").GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.pickup_armorStat).ToString ();

			statList.Ping.GetComponent<Text> ().text = scoringManager.GetScore (name, scoringManager.pingStat).ToString ();
		}

	}

}