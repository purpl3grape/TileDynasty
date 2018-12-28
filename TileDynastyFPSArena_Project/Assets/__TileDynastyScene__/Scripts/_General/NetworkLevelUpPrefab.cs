using UnityEngine;
using System.Collections;

public class NetworkLevelUpPrefab : Photon.MonoBehaviour {
	
	
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	
	
	float lerpTime = 1f;
	float currLerpTime = 0f;
	
	bool gotFirstUpdate = false;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if ( photonView.isMine ) {
			//Do nothing -- the character motor/input/etc.. is moving us
		} 
		else {
			transform.position = Vector3.Slerp(transform.position, realPosition, (float)PhotonNetwork.GetPing()/1000);
			transform.rotation = Quaternion.Slerp(transform.rotation, realRotation, (float)PhotonNetwork.GetPing()/1000);
		}
	}
	
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		
		if (stream.isWriting) {
			//This is OUR player. We need to send our actual position to the network.
			
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
		}
		else{
			//This is someone else's player. We need to receive their position (as of a few
			//milliseconds ago, and update our version of THAT player.
			
			// Right now, "RealPosition" holds the other's position at the LAST frame.
			// Instead of simply updating "realPosition" and continuing to lerp,
			// we MAY want to set our transform.position immediately to this old "realPosition"
			// and then update realPosition
			
			
			
			realPosition = (Vector3)stream.ReceiveNext ();
			realRotation = (Quaternion)stream.ReceiveNext ();
			
			if(gotFirstUpdate == false){
				transform.position = realPosition;
				transform.rotation = realRotation;
				gotFirstUpdate = true;
			}
		}
	}
}
