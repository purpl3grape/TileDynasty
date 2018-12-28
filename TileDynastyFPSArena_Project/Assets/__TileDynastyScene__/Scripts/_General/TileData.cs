using UnityEngine;
using System.Collections;

public class TileData : MonoBehaviour {

	ScoringManager scoringManager;
	NetworkManager networkManager;

	public Texture2D tex;
	public GUIStyle style;

	float bluePoints = 0f;
	float redPoints = 0f;
	float enemyPoints = 0f;

	float blueTiles;
	float redTiles;
	float enemyTiles;
	float scoreFixedUpdateTimer = 1f;


	// Use this for initialization
	void Start () {
		scoringManager = GameObject.FindObjectOfType<ScoringManager> ();
		networkManager = GameObject.FindObjectOfType<NetworkManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		blueTiles = GameObject.FindGameObjectsWithTag ("BlueTeam").Length;
		redTiles = GameObject.FindGameObjectsWithTag ("RedTeam").Length;
		enemyTiles = GameObject.FindGameObjectsWithTag ("EnemyTeam").Length;

		if (scoreFixedUpdateTimer >= 0) {
			scoreFixedUpdateTimer -= Time.deltaTime;
		} else {
				bluePoints += blueTiles;
				redPoints += redTiles;

			
			//scoringManager.GetComponent<PhotonView> ().RPC ("SetScore", PhotonTargets.AllBuffered, networkManager.GetPlayerName(), "tilepoints", (int)currPoints);
			scoreFixedUpdateTimer = 1f;
			
		}
	}

//	void OnGUI(){
//			
//			style.normal.background = tex;
//
//			style.fontSize = 16;
//
//			//TILES
//			style.normal.textColor = Color.grey;
//			style.alignment = TextAnchor.MiddleCenter;
//			GUI.Label(new Rect(Screen.width/2 - 6 * (Screen.width/15), 0, (Screen.width/15), (Screen.height/30)), "TEAM SCORE", style);
//			style.normal.textColor = Color.blue;
//			GUI.Label(new Rect(Screen.width/2 - 5 * (Screen.width/15), 0, (Screen.width/15), (Screen.height/30)), "BLUE", style);
//			GUI.Label(new Rect(Screen.width/2 - 4 * (Screen.width/15), 0, (Screen.width/15), (Screen.height/30)), ((int)bluePoints).ToString(), style);
//			style.normal.textColor = Color.red;
//			GUI.Label(new Rect(Screen.width/2 - 3 * (Screen.width/15), 0, (Screen.width/15), (Screen.height/30)), "RED", style);
//			GUI.Label(new Rect(Screen.width/2 - 2 * (Screen.width/15), 0, (Screen.width/15), (Screen.height/30)), ((int)redPoints).ToString(), style);
//
//
//
//			//SCORE
//			style.normal.textColor = Color.grey;
//			style.alignment = TextAnchor.MiddleCenter;
//			//GUI.Label(new Rect(Screen.width/2 + 1 * (Screen.width/15), 0, (Screen.width/15), (Screen.height/30)), "YOUR SCORE", style);
//			style.alignment = TextAnchor.MiddleCenter;
//			//GUI.Label(new Rect(Screen.width/2 + 2 * (Screen.width/15), 0, (Screen.width/15), (Screen.height/30)), ((int)currPoints).ToString(), style);
//	}

}
