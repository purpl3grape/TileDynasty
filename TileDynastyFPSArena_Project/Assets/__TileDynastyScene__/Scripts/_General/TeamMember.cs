using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TeamMember : MonoBehaviour {

	WaitForSeconds waitFor0_2 = new WaitForSeconds (0.2f);
	public Color bluePlayerColor;
	public Color redPlayerColor;

	public Texture BlueTeamTexture;
	public Texture RedTeamTexture;

	public Material blueTeamTrail;
	public Material redTeamTrail;

	public Material localPlayerSkinMaterial;
	public GameObject EnemyModel;

//	CapsuleCollider capsuleCollider;

	public int _teamID = 0;
	public int _classID = 0;

	public bool isBot = false;

	public string playerName = string.Empty;
	private string enemyName = string.Empty;

	public int teamID{
		get{
			return _teamID;
		}
		set{
			_teamID = value;
		}
	}

	public int classID{
		get{
			return _classID;
		}
		set{
			_classID = value;
		}
	}
		
	PhotonView pv;
	SkinnedMeshRenderer smr;
	TrailRenderer tRenderer;

	NavMeshAgent agent;

	void Start(){

		pv = GetComponent<PhotonView> ();
		//Local SkinnedMeshRenderer
		smr = GetComponentInChildren<SkinnedMeshRenderer> ();
		tRenderer = GetComponent<TrailRenderer> ();

		if (CompareTag ("Enemy")) {
			agent = GetComponent<NavMeshAgent> ();
//			capsuleCollider = GetComponent<CapsuleCollider> ();
		}

		StartCoroutine (UpdatePlayerSkinColor_Coroutine ());
	}

	IEnumerator UpdatePlayerSkinColor_Coroutine(){
		while (true) {
			if (CompareTag ("Player") && pv.isMine) {
				if ((teamID == 1 || teamID == 0) && smr != null && localPlayerSkinMaterial.mainTexture != BlueTeamTexture) {
					localPlayerSkinMaterial.mainTexture = BlueTeamTexture;
				} else if (teamID == 2 && smr != null && localPlayerSkinMaterial.mainTexture != RedTeamTexture) {
					localPlayerSkinMaterial.mainTexture = RedTeamTexture;
				}
			}
			yield return waitFor0_2;
		}
	}

	[PunRPC]
	void SetTeamID(int id){
		_teamID = id;
	}

	[PunRPC]
	void SetEnemyModelActive(int teamID, bool val){
		if (CompareTag ("Enemy")) {
			if (val == false) {
				tRenderer.endWidth = 0;
				tRenderer.startWidth = 0;
				agent.speed = 0f;
				agent.velocity = Vector3.zero;
			} else {
				tRenderer.endWidth = 1;
				tRenderer.startWidth = 1;
//				agent.speed = 15f;
			}
//			capsuleCollider.enabled = val;
		}
	}

	[PunRPC]
	void SetClassID(int id){
		_classID = id;
		if (classID == 1) {
			GetComponent<PlayerMovement> ().moveScale = 10f;
		}
		if (classID == 2) { 
			GetComponent<PlayerMovement> ().moveScale = 10f;
		}



		//1=defender
		//2=scout

		SkinnedMeshRenderer myskin = GetComponentInChildren<SkinnedMeshRenderer> ();
		tRenderer = GetComponent<TrailRenderer> ();
		if (myskin == null) {
			Debug.Log("Couldn't find a skinnedMeshRenderer!");
		}


		if(teamID == 0){
			if (classID == 1) {
				myskin.material.mainTexture = BlueTeamTexture;
			}
			if (classID == 2) {
				myskin.material.mainTexture = BlueTeamTexture;
			}
		}
		if(teamID == 1){
			if (classID == 1) {
				myskin.material.mainTexture = BlueTeamTexture;
			}
			if (classID == 2) {
				myskin.material.mainTexture = BlueTeamTexture;
			}
		}
		if (teamID == 2) {
			if (classID == 1) {
				myskin.material.mainTexture = RedTeamTexture;
			}
			if (classID == 2) {
				myskin.material.mainTexture = RedTeamTexture;
			}
		}
	}

		
	[PunRPC]
	public void synchronizeRenamedEnemyName(string renamedEnemyName){
		GetComponent<PhotonView> ().name = renamedEnemyName;
	}

	[PunRPC]
	void SetPlayerName(string Name){
		this.playerName = Name;
	}

	public string GetPlayerName(){
		return this.playerName;
	}

}