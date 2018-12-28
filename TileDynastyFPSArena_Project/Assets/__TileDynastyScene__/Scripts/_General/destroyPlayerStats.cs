using UnityEngine;
using System.Collections;

public class destroyPlayerStats : MonoBehaviour {

	void OnDestroy () {

		PhotonView pv = GetComponent<PhotonView>();

		if(pv != null && pv.instantiationId!=0){
			PhotonNetwork.Destroy(gameObject);
		}
		else{
			Destroy(gameObject);
		}
			
	}
}

