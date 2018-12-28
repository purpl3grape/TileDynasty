using UnityEngine;
using System.Collections;

public class CannonDisable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void disableCannon(){
		if (GetComponent<MeshRenderer> () != null) {
			GetComponent<MeshRenderer> ().enabled = false;
		} else {
			gameObject.SetActive (false);
		}
	}
		
	public void enableCannon(){
		if (GetComponent<MeshRenderer> () != null) {
			GetComponent<MeshRenderer> ().enabled = true;
		} else {
			gameObject.SetActive (true);
		}
	}
}
