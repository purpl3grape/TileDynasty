using UnityEngine;
using System.Collections;

public class ImpactReceiver : MonoBehaviour {
	/*
	float mass = 5.0F; // defines the character mass
	public Vector3 impact = Vector3.zero;
	private Rigidbody rbody;
	PlayerMovement playerMovement3;

	float lerpVal = 1f;
	float currLerpVal = 0f;

	// Use this for initialization
	void Start () {
		rbody = GetComponent<Rigidbody>();
		playerMovement3 = GetComponent<PlayerMovement> ();
	}
	
	// Update is called once per frame
	void Update () {
		// apply the impact force:
		if (impact.magnitude > 0.2F){
			rbody.MovePosition(rbody.position + impact * Time.fixedDeltaTime);
		}


//		currLerpVal += Time.deltaTime * Time.deltaTime * Time.deltaTime * (Time.deltaTime * (6f * Time.deltaTime - 15f) + 10f);
//		if(currLerpVal > lerpVal){
//			currLerpVal = lerpVal;
//		}
//		float perc = currLerpVal / lerpVal;
//
//		// consumes the impact energy each cycle: (This is using the smooth Lerp Formula)
//		//impact = Vector3.Lerp(impact, Vector3.zero, perc);
		impact = Vector3.Lerp(impact, Vector3.zero, 0.2f);


//		if(currLerpVal == lerpVal)
//			currLerpVal = 0f;

	}
	// call this function to add an impact force:
	[PunRPC]
	public void AddImpact(Vector3 dir, float force){
		dir.Normalize();
		if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
		impact += dir.normalized * force / mass;
	}
*/
}
