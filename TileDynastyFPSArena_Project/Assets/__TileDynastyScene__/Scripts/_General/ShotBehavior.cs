using UnityEngine;
using System.Collections;

public enum MachineGunBulletType{
	MG,
	HMG,
}

public class ShotBehavior : MonoBehaviour {

	Vector3 lastPosition;
	public float rayCastDistance = 5f;
	public float bulletSpeed = 200f;

	Transform tr;
	FXManager fxManager;
	GameObject sceneScripts;
	public string BulletType="";
	public MachineGunBulletType bulletType;
	Rigidbody rbody;

	// Use this for initialization
	void Start () {
		tr = GetComponent<Transform> ();
		rbody = GetComponent<Rigidbody> ();
		lastPosition = tr.position;

		fxManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<FXManager> ();
	}


	int nonItemLayerMask = ~(1 << 10);	//Do not hit itemLayer
	Vector3 direction;
	Ray ray;
	RaycastHit hit;
	// Update is called once per frame
	void FixedUpdate () {

//		direction = rbody.position - lastPosition;
		ray = new Ray(lastPosition, direction);


		if (Physics.Raycast (ray, out hit, rayCastDistance, nonItemLayerMask)) {
			if (!hit.collider.CompareTag ("A1")) {
				if (bulletType == MachineGunBulletType.MG) {
					//					fxManager.DestroyMG (gameObject);
					fxManager.DestroyFXPrefab(gameObject, fxManager.mgList);
				} else if (bulletType == MachineGunBulletType.HMG) {
					//					fxManager.DestroyHMG (gameObject);
					fxManager.DestroyFXPrefab(gameObject, fxManager.hmgList);
				}
			} else {
				rbody.MovePosition (rbody.position + direction * bulletSpeed * Time.fixedDeltaTime);
//				tr.position += tr.forward * Time.fixedDeltaTime * bulletSpeed;
			}
		} else {
			rbody.MovePosition (rbody.position + direction * bulletSpeed * Time.fixedDeltaTime);
//			tr.position += tr.forward * Time.fixedDeltaTime * bulletSpeed;
		}
		lastPosition = rbody.position;
	}

	public void SetBulletDirection(Vector3 dir){
		direction = dir;
	}
}
