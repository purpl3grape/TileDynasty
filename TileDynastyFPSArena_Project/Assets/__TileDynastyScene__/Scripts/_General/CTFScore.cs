using UnityEngine;
using System.Collections;

public class CTFScore : MonoBehaviour {

	GameObject flagRed;
	GameObject flagBlue;

	ScoringManager scoringManager;
	GuiManager gManager;
	FXManager fxManager;
	NetworkFlagRed networkFlagRed;
	NetworkFlagBlue networkFlagBlue;
	MatchTimer mTimer;

	Transform playerTransform = null;

	public AudioClip RedFlagCapture_confirm;
	public AudioClip BlueFlagCapture_confirm;

	private string blueCaptureMessage;
	private string redCaptureMessage;

	public int CTFScore_TeamID;
	int redScoreMultiplier=1;
	int blueScoreMultiplier=1;

	private int _CTFPointsBlue=0;
	public int CTFPointsBlue{
		get{ return this._CTFPointsBlue; }
		set{this._CTFPointsBlue = value;}
	}
	private int _CTFPointsRed=0;
	public int CTFPointsRed{
		get{ return this._CTFPointsRed; }
		set{this._CTFPointsRed = value;}
	}

	[PunRPC]
	public void IncreaseRedScore(){
		_CTFPointsRed += 1;
		scoringManager.GetComponent<PhotonView>().RPC("ChangeScore",PhotonTargets.AllBufferedViaServer, PhotonNetwork.player.name, "flagscaptured", (int)1);
		GetComponent<AudioSource> ().PlayOneShot (RedFlagCapture_confirm);
	}
	[PunRPC]
	public void IncreaseBlueScore(){
		_CTFPointsBlue += 1;
		scoringManager.GetComponent<PhotonView>().RPC("ChangeScore",PhotonTargets.AllBufferedViaServer, PhotonNetwork.player.name, "flagscaptured", (int)1);
		GetComponent<AudioSource> ().PlayOneShot (BlueFlagCapture_confirm);
	}

	[PunRPC]
	public void SetDestroyBlueFlagBool(bool boolval){
		destroyBlueFlag = boolval;
	}
	[PunRPC]
	public void SetDestroyRedFlagBool(bool boolval){
		destroyRedFlag = boolval;
	}

	bool destroyBlueFlag=false;
	bool destroyRedFlag=false;


	// Use this for initialization
	void Start () {
		scoringManager = GameObject.FindObjectOfType<ScoringManager> ();
		gManager = GameObject.FindObjectOfType<GuiManager> ();
		fxManager = GameObject.FindObjectOfType<FXManager> ();
		mTimer = GameObject.FindObjectOfType<MatchTimer> ();

	}
	
	// Update is called once per frame
	void Update () {
		networkFlagRed = GameObject.FindObjectOfType<NetworkFlagRed> ();
		networkFlagBlue = GameObject.FindObjectOfType<NetworkFlagBlue> ();

		if (!mTimer.isReady) {
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_RedFlagStatus.text = "";
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_BlueFlagStatus.text = "";
			return;
		}


		//Store Capture message for when capture occurs
		if (networkFlagBlue) {
			blueCaptureMessage = "Captured";// + networkFlagBlue.GetComponent<FlagAI> ().blueFlagBearerName;
		}
		if (networkFlagRed) {
			redCaptureMessage = "Captured";// + networkFlagRed.GetComponent<FlagAI> ().redFlagBearerName;
		}

		if (!networkFlagBlue) {
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_BlueFlagStatus.color = gManager.flagCapturedColor;
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_BlueFlagStatus.text = blueCaptureMessage;//"BLUE FLAG CAPTURED";
		}
		if (!networkFlagRed) {
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_RedFlagStatus.color = gManager.flagStolenColor;
			gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_RedFlagStatus.text = redCaptureMessage;// "RED FLAG CAPTURED";
		}

		if (destroyBlueFlag && networkFlagBlue.GetComponent<FlagAI>().isTakenBlue) {
			if (PhotonNetwork.isMasterClient) {
				GetComponent<PhotonView> ().RPC ("SetDestroyBlueFlagBool", PhotonTargets.AllBuffered, false);
				gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_BlueFlagStatus.color = gManager.flagCapturedColor;
//				ScoreMultiplier = playerTransform.gameObject.GetComponent<PlayerMovement>().CurrentScoringMultiplier;
				//UpdateScoringMultiplierRed (playerTransform.gameObject.GetComponent<PlayerMovement>().CurrentScoringMultiplier);
				GetComponent<PhotonView> ().RPC ("IncreaseRedScore", PhotonTargets.AllBuffered);
				PhotonNetwork.Destroy (networkFlagBlue.gameObject);
			}
		}

		if (destroyRedFlag && networkFlagRed.GetComponent<FlagAI>().isTakenRed) {
			if (PhotonNetwork.isMasterClient) {
				GetComponent<PhotonView> ().RPC ("SetDestroyRedFlagBool", PhotonTargets.AllBuffered, false);
				gManager.InGamePanel.GetComponent<InGamePanel> ().Container_CTF.GetComponent<ContainerCTFInput> ().Message_RedFlagStatus.color = gManager.flagCapturedColor;
				GetComponent<PhotonView> ().RPC ("IncreaseBlueScore", PhotonTargets.AllBuffered);
				PhotonNetwork.Destroy (networkFlagRed.gameObject);
			}
		}


	}

	void OnTriggerEnter(Collider other){
		//Trying to score for Blue
		if (other.CompareTag ("Player")) {
			playerTransform = other.transform;
			//TO OBTAIN THE SCORING MULTIPLIER FOR THIS PLAYER
//		ScoreMultiplier = playerTransform.gameObject.GetComponent<PlayerMovement>().CurrentScoringMultiplier;

			if (CTFScore_TeamID == 1 && networkFlagRed != null) {
				if (networkFlagRed.GetComponent<FlagAI> ().isTakenRed && !networkFlagBlue.GetComponent<FlagAI> ().isTakenBlue) {
					if (PhotonNetwork.isMasterClient) {
						GetComponent<PhotonView> ().RPC ("SetDestroyRedFlagBool", PhotonTargets.AllBuffered, true);
					}
				}
			}
			//Scoring for Red Team
			if (CTFScore_TeamID == 2 && networkFlagBlue != null) {
				if (networkFlagBlue.GetComponent<FlagAI> ().isTakenBlue && !networkFlagRed.GetComponent<FlagAI> ().isTakenRed) {
					if (PhotonNetwork.isMasterClient) {
						GetComponent<PhotonView> ().RPC ("SetDestroyBlueFlagBool", PhotonTargets.AllBuffered, true);
					}
				}
			}
		}
	}

}
