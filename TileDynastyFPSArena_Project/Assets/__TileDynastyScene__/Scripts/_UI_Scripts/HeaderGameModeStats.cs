using UnityEngine;
using System.Collections;

public class HeaderGameModeStats : MonoBehaviour {

	public GameObject Header_Points;
	public GameObject Header_Capture;

	GuiManager guiManager;

	bool isHeaderDisabledInitially = false;

	// Use this for initialization
	void Start () {
		guiManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<GuiManager> ();
		Header_Capture.SetActive(false);
	}

	bool EnableHeaderInit(bool someAwesomeBooleanValue){
		if (!someAwesomeBooleanValue) {
			//TILE STATS ONLY
			if (guiManager.currentGameMode == 2) {
				Header_Capture.SetActive (true);
			}

			return true;
		} else
			return true;
	}

	// Update is called once per frame
	void Update () {
		EnableHeaderInit (isHeaderDisabledInitially);
	}
}
