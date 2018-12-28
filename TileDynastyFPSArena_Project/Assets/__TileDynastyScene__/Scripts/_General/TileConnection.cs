using UnityEngine;
using System.Collections;

public enum FullConnectionStatus{
	None,
	BlueConstructionComplete,
	RedConstructionComplete
}

public class TileConnection : MonoBehaviour {

	public GameObject TileA;
	public GameObject TileB;
	public GameObject TileC;
	public GameObject TileD;

	[HideInInspector] public bool connectionABlue = false;
	[HideInInspector] public bool connectionBBlue = false;
	[HideInInspector] public bool connectionCBlue = false;
	[HideInInspector] public bool connectionDBlue = false;
	[HideInInspector] public bool connectionARed = false;
	[HideInInspector] public bool connectionBRed = false;
	[HideInInspector] public bool connectionCRed = false;
	[HideInInspector] public bool connectionDRed = false;
	[HideInInspector] public bool connectionANeutral = false;
	[HideInInspector] public bool connectionBNeutral = false;
	[HideInInspector] public bool connectionCNeutral = false;
	[HideInInspector] public bool connectionDNeutral = false;

	void Start () {
	}
		
	void Update () {
		if (TileA.GetComponent<ColorChange> ().currTeamID == 0) {
			connectionANeutral = true;
			connectionARed = false;
			connectionABlue = false;
		} else if (TileA.GetComponent<ColorChange> ().currTeamID == 1) {
			connectionANeutral = false;
			connectionARed = false;
			connectionABlue = true;
		} else if (TileA.GetComponent<ColorChange> ().currTeamID == 2) {
			connectionANeutral = false;
			connectionARed = true;
			connectionABlue = false;		
		}

		if (TileB.GetComponent<ColorChange> ().currTeamID == 0) {
			connectionBNeutral = true;
			connectionBRed = false;
			connectionBBlue = false;
		} else if (TileB.GetComponent<ColorChange> ().currTeamID == 1) {
			connectionBNeutral = false;
			connectionBRed = false;
			connectionBBlue = true;
		} else if (TileB.GetComponent<ColorChange> ().currTeamID == 2) {
			connectionBNeutral = false;
			connectionBRed = true;
			connectionBBlue = false;		
		}

		if (TileC.GetComponent<ColorChange> ().currTeamID == 0) {
			connectionCNeutral = true;
			connectionCRed = false;
			connectionCBlue = false;
		} else if (TileC.GetComponent<ColorChange> ().currTeamID == 1) {
			connectionCNeutral = false;
			connectionCRed = false;
			connectionCBlue = true;
		} else if (TileC.GetComponent<ColorChange> ().currTeamID == 2) {
			connectionCNeutral = false;
			connectionCRed = true;
			connectionCBlue = false;		
		}

		if (TileD.GetComponent<ColorChange> ().currTeamID == 0) {
			connectionDNeutral = true;
			connectionDRed = false;
			connectionDBlue = false;
		} else if (TileD.GetComponent<ColorChange> ().currTeamID == 1) {
			connectionDNeutral = false;
			connectionDRed = false;
			connectionDBlue = true;
		} else if (TileD.GetComponent<ColorChange> ().currTeamID == 2) {
			connectionDNeutral = false;
			connectionDRed = true;
			connectionDBlue = false;		
		}
	}
}