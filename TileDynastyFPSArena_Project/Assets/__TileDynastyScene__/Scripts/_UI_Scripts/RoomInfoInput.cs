using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomInfoInput : MonoBehaviour {

	public Text RoomNamePrefab;
	public Text MapName;
	public Text GameMode;
	public Text MasterClient;
	public Text RoomCountPrefab;
	public Text RoomRegion;

	public void JoinGameFromRoomListButtonBehavior(){
		Debug.Log ("Trying to join: " + RoomNamePrefab.GetComponent<Text> ().text);
		PlayerPrefs.SetInt ("ViewRoomList", 0);
		PlayerPrefs.Save ();
		PhotonNetwork.JoinRoom (RoomNamePrefab.GetComponent<Text>().text);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
