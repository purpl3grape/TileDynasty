using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListManager : MonoBehaviour {

	private static RoomListManager roomListManagerInstance;

	public RoomInfo [] managedRoomList;
	public string[] roomNameList;
	public string[] roomModeList;
	public string[] roomMapList;
	public string[] roomCurrentSizeList;
	public string[] roomMaxSizeList;
	public string[] roomTimeList;


	void Awake(){

		if (roomListManagerInstance != null) {
			Destroy(gameObject);
			return;
		}
		roomListManagerInstance = this;

		DontDestroyOnLoad (gameObject);

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
