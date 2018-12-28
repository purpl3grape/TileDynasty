using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public float rotX = 0f;
	public float rotY = 0f;
	public float rotZ = 0f;
	Transform tr;

	public Vector3 targetAngle = new Vector3(0f, 345f, 0f);
	private Vector3 currentAngle;
	// Use this for initialization
	void Start () {
		tr = GetComponent<Transform> ();
		Rotate ();
	}
	IEnumerator RotateObject_Coroutine(){
		while (true) {
			tr.Rotate (new Vector3 (rotX, rotY, rotZ) * .1f);
			yield return new WaitForSeconds (.1f);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		tr.Rotate (new Vector3 (rotX, rotY, rotZ) * Time.fixedDeltaTime);
	}

	void Rotate(){
		tr.Rotate (new Vector3 (rotX, rotY, rotZ) * Time.fixedDeltaTime);
	}
}
