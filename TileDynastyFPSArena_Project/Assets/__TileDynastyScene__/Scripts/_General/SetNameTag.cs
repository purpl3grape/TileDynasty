using UnityEngine;
using System.Collections;

public class SetNameTag : MonoBehaviour {

	KeyBindingManager kMgr;
	ScoringManager scoringManager;
	GuiManager guiManager;
	MatchTimer mTimer;

	private bool _ready = false;

	public bool ready{
		get{
			return _ready;
		}
		set{
			_ready = value;
		}
	}

	private string currName = string.Empty;
	public string playerName{
		get{ return currName; }
	}
	private string oldName = string.Empty;

	void Start(){

		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();
		scoringManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<ScoringManager> ();
		guiManager = scoringManager.GetComponent<GuiManager> ();
		mTimer = scoringManager.GetComponent<MatchTimer> ();

		if(transform.GetComponent<PhotonView>().isMine && transform.CompareTag("Player")){
			transform.localPosition = new Vector3 (0f, 1.6f, -3f);
		}
	}




	[PunRPC] 
	void PlayerReady () {
		_ready = !_ready;

		if(_ready && GetComponentInParent<PlayerMovement>())
			gameObject.tag = "Ready";
		else if(!_ready && GetComponentInParent<PlayerMovement>())
			gameObject.tag = "Not_Ready";

	}

	//Called upon initial spawn
	[PunRPC] 
	void TagPlayerName (string name) {
		currName = name;
		GetComponent<TextMesh>().text = currName;
	}

	[PunRPC] 
	void TagTeam (string name, int teamID) {
		currName = name;
		GetComponent<TextMesh>().text = currName;
		if(teamID == 0){
			gameObject.tag = "Coop";
		}
		if (teamID == 1) {
			gameObject.tag = "BlueTeam";
		}
		if (teamID == 2) {
			gameObject.tag = "RedTeam";
		}
	}



}