using UnityEngine;
using System.Collections;

public enum TileStatus{
	TileCapturedBlue,
	TileCapturedRed,
	TileCapturedNeutral
}
	
public class ColorChange : Photon.MonoBehaviour {

	Vector3 tempcolor;
	Color synccolor;
	
	ScoringManager scoringManager;
	NetworkManager nManager;
	MatchTimer mTimer;
	
	//Color Informtion
	public Color blueColor = new Color (55,130,255,255);
	public Color redColor = new Color (255,30,150,255);
	public Color defaultColor;

	public Material BlueMaterial;
	public Material RedMaterial;
	public Material DefaultMaterial;

	public Material BlueLightMaterial;
	public Material RedLightMaterial;
	public Material DefaultLightMaterial;

	InGamePanel inGamePanel;
	GameObject tileArrow;
	GameObject tileLightBeam;
	GameObject tileImage;
	int _tileStatusValue=-1;
	float captureTimeRemaining=60f;
	public AudioClip[] tileStatusAudio;

	//Tile Information
	private int numTilesOwned = 0;
	private string _lastOwner = "";
	public string lastOwner{
		get{ return this._lastOwner; }
		set{ this._lastOwner = value; }
	}
	private string prevOwner = "";
	private int _currTeamID = 0;

	public int currTeamID {
		get{ return this._currTeamID; }
		set{ this._currTeamID = value; }
	}

	void Start () {
		scoringManager = GameObject.FindGameObjectWithTag("SceneScripts").GetComponent<ScoringManager> ();
		nManager = scoringManager.GetComponent<NetworkManager> ();
		mTimer = scoringManager.GetComponent<MatchTimer> ();
		inGamePanel = GameObject.FindObjectOfType<InGamePanel> ();

		tileArrow = transform.Find ("TileIndicator").gameObject;
		tileLightBeam = transform.Find ("LightBeam").gameObject;
		tileImage = transform.Find ("TileImage").gameObject;

		tileArrow.GetComponent<MeshRenderer> ().material = DefaultLightMaterial;
		tileLightBeam.GetComponent<MeshRenderer> ().material = DefaultLightMaterial;
		_tileStatusValue = (int)TileStatus.TileCapturedNeutral;
	}
	
	void FixedUpdate () {

		if (!mTimer.isReady) {
			return;
		}

		if (inGamePanel == null) {
			inGamePanel = GameObject.FindObjectOfType<InGamePanel> ();
		}


		if (_currTeamID == 0) {
			setColor (DefaultMaterial);
			tileArrow.GetComponent<MeshRenderer> ().material = DefaultLightMaterial;
			tileLightBeam.GetComponent<MeshRenderer> ().material = DefaultLightMaterial;

			if (_tileStatusValue != (int)TileStatus.TileCapturedNeutral) {
//				inGamePanel.Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (tileStatusAudio [2]);
				GetComponent<AudioSource>().PlayOneShot(tileStatusAudio[2]);
				_tileStatusValue = (int)TileStatus.TileCapturedNeutral;
			}

		} else if (_currTeamID == 1) {
			setColor (BlueMaterial);
			tileArrow.GetComponent<MeshRenderer> ().material = BlueLightMaterial;
			tileLightBeam.GetComponent<MeshRenderer> ().material = BlueLightMaterial;

			if (PhotonNetwork.isMasterClient) {
				if (captureTimeRemaining > 0) {
					captureTimeRemaining -= Time.deltaTime;
				} else {
					GetComponent<PhotonView> ().RPC ("setLastTeamOwner", PhotonTargets.AllBuffered, 0);
					transform.Find ("NameTag").GetComponent<PhotonView> ().RPC ("TagTeam", PhotonTargets.AllBuffered, _lastOwner, 0);
					captureTimeRemaining = 60f;
				}
			}

			if (_tileStatusValue != (int)TileStatus.TileCapturedBlue) {
//				inGamePanel.Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (tileStatusAudio[0]);
				GetComponent<AudioSource>().PlayOneShot(tileStatusAudio[0]);
				_tileStatusValue = (int)TileStatus.TileCapturedBlue;
			}

		} else if (_currTeamID == 2) {
			setColor(RedMaterial);
			tileArrow.GetComponent<MeshRenderer> ().material = RedLightMaterial;
			tileLightBeam.GetComponent<MeshRenderer> ().material = RedLightMaterial;

			if (PhotonNetwork.isMasterClient) {
				if (captureTimeRemaining > 0) {
					captureTimeRemaining -= Time.deltaTime;
				} else {
					GetComponent<PhotonView> ().RPC ("setLastTeamOwner", PhotonTargets.AllBuffered, 0);
					transform.Find ("NameTag").GetComponent<PhotonView> ().RPC ("TagTeam", PhotonTargets.AllBuffered, _lastOwner, 0);
					captureTimeRemaining = 60f;
				}
			}

			if (_tileStatusValue != (int)TileStatus.TileCapturedRed) {
//				inGamePanel.Announcer_TeamLead.GetComponent<AudioSource> ().PlayOneShot (tileStatusAudio[1]);
				GetComponent<AudioSource>().PlayOneShot(tileStatusAudio[1]);
				_tileStatusValue = (int)TileStatus.TileCapturedRed;
			}

		}


	}
		
	[PunRPC]
	void setLastOwner(string name){
		this._lastOwner = name;
	}
	
	[PunRPC]
	void setSecondLastOwner(string name){
		this.prevOwner = name;
	}
	
	[PunRPC]
	void setLastTeamOwner(int teamID){
		this._currTeamID = teamID;
	}

	//Cannot Serialize Color in Photon Unity Networking via RPC method. Was suggested to use OnPhotonSerialize to serialize the R, G, B components
	//The OnPhotonSerialize of R, G, B was inefficient and caused some inconsistent behavior between color synchronization anyway
	//Instead, a unique ID that is also TeamID is used to determine the color displayed.
	//Also the color displayed is no longer sent and received, but rather the local client has it updated based on current tile tag (which can be sent to all clients via RPC method)
	void setColor(Material mat){
		if (gameObject.tag == "A1") {
//			this.gameObject.GetComponent<MeshRenderer> ().material = mat;
			tileImage.GetComponent<MeshRenderer> ().material = mat;
		}
	}
	
	void OnTriggerEnter(Collider other){

		if (!mTimer.isReady)
			return;
		if (!(mTimer.SecondsUntilItsTime > 0))
			return;

		if (other.CompareTag ("Player") || other.CompareTag ("Enemy")) {
			captureTimeRemaining = 60f;

			GetComponent<PhotonView> ().RPC ("setSecondLastOwner", PhotonTargets.All, _lastOwner);
			if (other.CompareTag ("Player")) {
				GetComponent<PhotonView> ().RPC ("setLastOwner", PhotonTargets.All, other.gameObject.GetComponent<TeamMember> ().GetPlayerName ());
			} else if (other.CompareTag ("Enemy")) {
				GetComponent<PhotonView> ().RPC ("setLastOwner", PhotonTargets.All, other.name);
			}
		
			scoringManager.GetComponent<PhotonView> ().RPC ("ChangeScore", PhotonTargets.All, _lastOwner, "tilescaptured", 1);
			transform.Find ("NameTag").GetComponent<PhotonView> ().RPC ("TagTeam", PhotonTargets.All, _lastOwner, other.gameObject.GetComponent<TeamMember> ().teamID);

			//ALWAYS SET TEAMID EVEN IF IT IS SAME TEAM and CHANGE COLOR TOO
			GetComponent<PhotonView> ().RPC ("setLastTeamOwner", PhotonTargets.All, other.GetComponent<TeamMember> ().teamID);
			if (other.GetComponent<TeamMember> ().teamID == 1) {
				setColor (BlueMaterial);
				tileArrow.GetComponent<MeshRenderer> ().material = BlueLightMaterial;
				tileLightBeam.GetComponent<MeshRenderer> ().material = BlueLightMaterial;
			} else if (other.GetComponent<TeamMember> ().teamID == 2) {
				setColor (RedMaterial);
				tileArrow.GetComponent<MeshRenderer> ().material = RedLightMaterial;
				tileLightBeam.GetComponent<MeshRenderer> ().material = RedLightMaterial;
			}
		}
	}

}
