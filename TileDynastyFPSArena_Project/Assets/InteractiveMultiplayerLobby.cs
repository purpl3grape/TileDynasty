using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveMultiplayerLobby : MonoBehaviour {

	MatchTimer mTimer;

	void Start () {
		mTimer = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<MatchTimer> ();
	}
	
	void Update () {
		if (mTimer.isReady)
			return;

	}

}
