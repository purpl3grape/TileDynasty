using UnityEngine;
using System.Collections;

public class FlagAI : MonoBehaviour {

	FXManager fxManager;
	NetworkManager nManager;
	ScoringManager scoringManager;
	GuiManager gManager;
	Health health;
	MatchTimer mTimer;

	private string _blueFlagBearerName;
	public string blueFlagBearerName {
		get{ return this._blueFlagBearerName; }
	}
	private string _redFlagBearerName;
	public string redFlagBearerName {
		get{ return this._redFlagBearerName; }
	}

	private bool _isTakenBlue = false;
	public bool isTakenBlue{
		get{ return this._isTakenBlue; }
		set{ this._isTakenBlue = value; }
	}
	private bool _isTakenRed = false;
	public bool isTakenRed{
		get{ return this._isTakenRed; }
		set{ this._isTakenRed = value; }
	}
	private bool _flagStolen = false;
	public bool flagStolen {
		get{ return this._flagStolen; }
		set{ this._flagStolen = value; }
	}

	int flagStolenValue=-1;
		
	//Enemy TeamID for capturing our Flag in CTF
	//teamID=1 is Blue
	//teamID=2 is Red
	public int enemyTeamID;

	EnemyMove enemyMove;

	Transform currentPlayerTransform = null;

	// Use this for initialization
	void Start () {

		fxManager = GameObject.FindObjectOfType<FXManager> ();
		nManager = GameObject.FindObjectOfType<NetworkManager> ();
		scoringManager = GameObject.FindObjectOfType<ScoringManager> ();
		gManager = GameObject.FindObjectOfType<GuiManager> ();
		mTimer = GameObject.FindObjectOfType<MatchTimer> ();

		GetComponent<PhotonView> ().RPC ("BlueFlagTaken", PhotonTargets.AllBuffered, false);
		GetComponent<PhotonView> ().RPC ("RedFlagTaken", PhotonTargets.AllBuffered, false);

		if (enemyTeamID == 1) {
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_RedFlagStatus.color = gManager.flagReturnedColor;
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_RedFlagStatus.text = "Returned";
			flagStolenValue = (int)enumTeamStatus.FlagReturned;
		}
		if (enemyTeamID == 2) {
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_BlueFlagStatus.color = gManager.flagReturnedColor;
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_BlueFlagStatus.text = "Returned";
			flagStolenValue = (int)enumTeamStatus.FlagReturned;
		}

	}


	[PunRPC]
	public void BlueFlagTaken(bool boolean_Val){
		_isTakenBlue = boolean_Val;
	}
	[PunRPC]
	public void RedFlagTaken(bool boolean_Val){
		_isTakenRed = boolean_Val;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player") {
			if (!_isTakenBlue && (other.GetComponent<TeamMember> ().teamID == enemyTeamID)) {
				this.transform.SetParent (other.transform);
//				Debug.Log (other.gameObject.tag + ", " + other.GetComponent<TeamMember>().teamID + ", flagTeamID " + enemyTeamID);
				_redFlagBearerName = other.GetComponentInChildren<SetNameTag> ().playerName;
				currentPlayerTransform = other.transform;
				this.transform.position = other.transform.position + new Vector3 (0, 15, 0);
				gManager.networkFlagBlue = this.gameObject.GetComponent<NetworkFlagBlue>();
				if (PhotonNetwork.isMasterClient) {
					GetComponent<PhotonView> ().RPC ("BlueFlagTaken", PhotonTargets.AllBuffered, true);
				}
			}
			if (!_isTakenRed && (other.GetComponent<TeamMember> ().teamID == enemyTeamID)) {
				this.transform.SetParent (other.transform);
//				Debug.Log (other.gameObject.tag + ", " + other.GetComponent<TeamMember>().teamID + ", flagTeamID " + enemyTeamID);
				_blueFlagBearerName = other.GetComponentInChildren<SetNameTag> ().playerName;
				currentPlayerTransform = other.transform;
				this.transform.position = other.transform.position + new Vector3 (0, 15, 0);
				gManager.networkFlagRed = this.gameObject.GetComponent<NetworkFlagRed> ();
				if (PhotonNetwork.isMasterClient) {
					GetComponent<PhotonView> ().RPC ("RedFlagTaken", PhotonTargets.AllBuffered, true);
				}
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (gManager.networkFlagBlue == null) {
			//nManager.hasSpawnedFlag = false;
			return;
		}

		//UPDATE FLAG POSITION BASED ON WHO STOLE FLAG
		if(currentPlayerTransform != null && (isTakenRed || isTakenBlue)){

			//FIRST UPDATE WHEN FLAG IS STOLEN. INCREASE PLAYER STATS FOR: FLAGS STOLEN
			if (!_flagStolen) {
				if (PhotonNetwork.isMasterClient) {
					scoringManager.GetComponent<PhotonView> ().RPC ("ChangeScore", PhotonTargets.AllBufferedViaServer, currentPlayerTransform.GetComponentInChildren<SetNameTag> ().playerName, "flagsstolen", (int)1);
				}
				_flagStolen = true;
			}
			this.transform.position = currentPlayerTransform.position + new Vector3 (0, 4, 0);
			this.transform.rotation = currentPlayerTransform.rotation;
		}

		//THE INSTANT WE CANNOT FIND A PLAYER, MEANS HE DIED. SO WE DESTROY THE FLAG HE WAS CARRYING AS WELL
		//EVEN IF CLIENT'S COPY DOESN'T HAVE IT, IT CANNOT DESTROY SOMETHING THAT DOESN NOT EXIST (THE CHILD OBJECT: FLAG)
		if (currentPlayerTransform == null && (isTakenRed || isTakenBlue)) {

			if (PhotonNetwork.isMasterClient) {
				PhotonNetwork.Destroy (this.gameObject);
			}
		}
			
		if (enemyTeamID==1 && isTakenRed) {
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_RedFlagStatus.color = gManager.flagStolenColor;
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_RedFlagStatus.text = "Stolen";// + _blueFlagBearerName;

			if (flagStolenValue != (int)enumTeamStatus.CaptureTheFlagCommand) {
				gManager.InGamePanel.GetComponent<InGamePanel> ().Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (gManager.Audio_TeamStatus [7]);
				flagStolenValue = (int)enumTeamStatus.CaptureTheFlagCommand;
			}

		}
		if (enemyTeamID==2 && isTakenBlue) {
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_BlueFlagStatus.color = gManager.flagStolenColor;
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_BlueFlagStatus.text = "Stolen";// + _redFlagBearerName;

			if (flagStolenValue != (int)enumTeamStatus.CaptureTheFlagCommand) {
				gManager.InGamePanel.GetComponent<InGamePanel> ().Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (gManager.Audio_TeamStatus [7]);
				flagStolenValue = (int)enumTeamStatus.CaptureTheFlagCommand;
			}
		}


	}
		

}
