using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameStatusOnError : MonoBehaviour {


	void Start () {
	}
	
	void FixedUpdate () {
		if (Input.GetKeyDown(KeyCode.Backspace)) {
			PlayerPrefs.SetInt ("startMultiplayerFromHomeBaseKey", 0);
			PlayerPrefs.SetInt ("BeginChallengeFromHomeBase", 0);
			PlayerPrefs.SetInt ("ViewRoomList", 0);
			PlayerPrefs.Save ();
		}
	}
		

}
