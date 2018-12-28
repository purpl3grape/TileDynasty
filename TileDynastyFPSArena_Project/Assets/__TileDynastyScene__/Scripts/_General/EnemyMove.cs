using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour {
	
	Animator anim;
	FXManager fxManager;
	NetworkManager nManager;

	public float gravity = -100000.0f;
	public float transformHeight = 10.0f;
	public float moveSpeed = 20.0f;  // Ground move speed
	
	private Vector3 TossMag = Vector3.zero;	
	private bool lookAt = false;
	private bool moveTo = false;
	private Vector3 lookAtPos = Vector3.zero;

	private bool tossed = false;
	private bool grounded= false;
	private Vector3 moveDir;
	private Rigidbody rigidBody;
	
	void  Start (){
		
		anim = GetComponent<Animator>();
		fxManager = GameObject.FindObjectOfType<FXManager> ();
		nManager = GameObject.FindObjectOfType<NetworkManager> ();
		rigidBody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate(){


		//This line was added
//		this.transform.rotation = attractor.Attract (this.transform);

		transformHeight = this.transform.localScale.y;

		if(TossMag.magnitude > 0.00f) {
			applyTossVelocity (TossMag);
			TossMag = Vector3.Slerp (TossMag, Vector3.zero, 0.001f);
		} else {
			TossMag = Vector3.zero;
		}


		if (moveTo == true) {
			rigidBody.MovePosition (rigidBody.position + transform.TransformDirection (transform.forward) * moveSpeed * Time.deltaTime);
			if(rigidBody.velocity.magnitude < 60){
				rigidBody.AddForce (lookAtPos * moveSpeed * 200);
			}
			moveTo = false;
		}
		//rigidBody.AddForce (transform.TransformDirection(moveDir) * moveSpeed * 20);

		applyGravity ();
		GroundCheck ();

		if (lookAt == true) {
//			Quaternion modLookRot = transform.LookAt(lookAtPos);
			Quaternion modLookRot = Quaternion.LookRotation(rigidBody.position - lookAtPos, Vector3.forward);
//			modLookRot.y = 0;
			rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, modLookRot, 0.1f);

//			transform.LookAt(new Vector3(lookAtPos.x, rigidBody.position.y, 0));
			transform.LookAt(lookAtPos, transform.up);

			lookAt = false;
		}

	}
	
	void GroundCheck(){
		Ray ray = new Ray (transform.position, - transform.up);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, (transformHeight) + .2f)) {
			grounded = true;
			if(tossed && Physics.Raycast(ray, out hit, (transformHeight))){
				TossMag = Vector3.Lerp (TossMag, Vector3.zero, 1f);
				tossed=false;
			}
		} else
			grounded = false;
	}
	
	void applyGravity(){
		if (!grounded) {
			rigidBody.AddForce (transform.up * gravity);
		}
	}
	
	public void applyTossVelocity(Vector3 tossMag){
		//TossMag = tossMag;
		//tossed = true;
		//rigidBody.MovePosition(rigidBody.position + TossMag * 0.2f * Time.deltaTime);
	}
	
	public bool getGrounded(){
		return this.grounded;
	}
	
	public void setMoveDirection(Vector3 dir){
		if (tossed) {
			moveDir = Vector3.zero;
		}
		else this.moveDir = dir;
	}

	public void setLookAtPosition(Vector3 lookAtPosition){
		this.lookAtPos = ProjectPointOnPlane(transform.up, transform.position, lookAtPosition);
		lookAt = true;
	}

	public Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point){
		planeNormal.Normalize();
		float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
		return point + planeNormal * distance;
	}

	public void approachTarget(){
		moveTo = true;
	}
}
