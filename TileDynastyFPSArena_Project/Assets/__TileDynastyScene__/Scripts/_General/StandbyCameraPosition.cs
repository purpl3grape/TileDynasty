using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StandbyCameraPosition : MonoBehaviour {

	Transform tr;
	Skybox skybox;
	public Material skyboxMaterial1;
	public Material skyboxMaterial2;
	public Color ambientLight1;
	public Color ambientLight2;

	void Start () {
		tr = GetComponent<Transform> ();
		skybox = GetComponent<Skybox> ();
		if (SceneManager.GetActiveScene ().buildIndex == 2) {
			//SECTOR 1
			skybox.material = skyboxMaterial2;
			tr.position = new Vector3 (175, 75, 175);
			tr.rotation = Quaternion.Euler (25, 45, 0);
			RenderSettings.ambientLight = ambientLight1;
			GetComponent<Camera> ().fieldOfView = 90f;
		} else if (SceneManager.GetActiveScene ().buildIndex == 3) {
			//CAMPGROUNDS
			skybox.material = skyboxMaterial1;
			tr.position = new Vector3 (-1960, 25, -130);
			tr.rotation = Quaternion.Euler (45, 225, 0);
			RenderSettings.ambientLight = ambientLight1;
			GetComponent<Camera> ().fieldOfView = 120f;
		} else if (SceneManager.GetActiveScene ().buildIndex == 4) {
			//WINTER_LAND
			skybox.material = skyboxMaterial2;
			tr.position = new Vector3 (-2010, 30, 0);
			tr.rotation = Quaternion.Euler (30, 0, 0);
			RenderSettings.ambientLight = ambientLight1;
			GetComponent<Camera> ().fieldOfView = 120f;
		} else if (SceneManager.GetActiveScene ().buildIndex == 5) {
			//HOME BASE
			skybox.material = skyboxMaterial2;
			tr.position = new Vector3 (-2040, 20, 60);
			tr.rotation = Quaternion.Euler (30, 135, 0);
			RenderSettings.ambientLight = ambientLight2;
			GetComponent<Camera> ().fieldOfView = 90f;
		}
	}
}