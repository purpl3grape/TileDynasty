using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour {

	public GameObject ClosedDoor;
	public GameObject OpenedDoor;
	public bool isDoorOpen = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (isDoorOpen) {
			StartCoroutine (DoorCloseEvent (4f));
		}
	}

	IEnumerator DoorCloseEvent(float timeToClose){
		float elapsedTime = 0f;
		while (elapsedTime < timeToClose) {
			timeToClose	+= Time.fixedDeltaTime;
			yield return null;
		}
		CloseDoor (true);
	}

	void CloseDoor(bool DoorClose){

		if (DoorClose) {
			ClosedDoor.SetActive (true);
			OpenedDoor.SetActive (false);
			isDoorOpen = false;
		} else {
			ClosedDoor.SetActive (false);
			OpenedDoor.SetActive (true);
			isDoorOpen = true;
		}
	}

	void OnTriggerEnter(Collider other){

		if (!other.CompareTag ("Player") || other.CompareTag ("Enemy"))
			return;
		
		if (isDoorOpen) {
			CloseDoor (true);
		}
	}

}
