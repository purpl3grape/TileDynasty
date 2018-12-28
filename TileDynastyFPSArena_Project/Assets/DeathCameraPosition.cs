using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCameraPosition : MonoBehaviour {

	Transform tr;
	Skybox skybox;
	public Material skyboxMaterial1;
	public Material skyboxMaterial2;

	void Start () {
		tr = GetComponent<Transform> ();
		skybox = GetComponent<Skybox> ();
		if (SceneManager.GetActiveScene ().buildIndex == 2) {
			//SEAMS AND BOLTS
			skybox.material = skyboxMaterial1;
		} else if (SceneManager.GetActiveScene ().buildIndex == 3) {
			//CAMPGROUNDS
			skybox.material = skyboxMaterial1;
		} else if (SceneManager.GetActiveScene ().buildIndex == 4) {
			//WINTER_LAND
			skybox.material = skyboxMaterial2;
		} else if (SceneManager.GetActiveScene ().buildIndex == 5) {
			//WINTER_LAND
			skybox.material = skyboxMaterial2;
		}
	}
}