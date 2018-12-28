using UnityEngine;
using System.Collections;

public class _DEV_CLEAR_Steam : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(SteamManager.Initialized) {
			Steamworks.SteamAPI.RunCallbacks ();
			Steamworks.SteamUserStats.ResetAllStats(true);
			Steamworks.SteamUserStats.StoreStats();
		}
	}
}
