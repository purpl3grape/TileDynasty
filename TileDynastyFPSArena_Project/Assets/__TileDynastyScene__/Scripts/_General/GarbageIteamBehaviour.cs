using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageIteamBehaviour : MonoBehaviour {

	private Rigidbody rbody;
	private Transform tr;
	private bool grounded = false;
	private Ray ray0;
	public Vector3 gravity = new Vector3 (0, 1, 0);

	void Start () {
		rbody = GetComponent<Rigidbody> ();
		tr = GetComponent<Transform> ();
	}
	
	void FixedUpdate () {
		GroundCheck ();
		if (!grounded) {
			rbody.MovePosition (rbody.position - gravity * Time.fixedDeltaTime);
		}
	}

	void GroundCheck(){
		ray0 = new Ray (tr.position, - tr.up);
		RaycastHit hit;
		Transform hitTransform;
		if (Physics.Raycast (ray0, out hit, 1f + .1f)) {
			hitTransform = hit.transform;
			if (!hitTransform.CompareTag ("A1") && !hitTransform.name.StartsWith ("Health_") && !hitTransform.name.StartsWith ("Armor_") && !hitTransform.name.StartsWith ("Ammo_")) {
				grounded = true;
			}
		} else {
			grounded = false;
		}
	}


}
