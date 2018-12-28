using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapControls : MonoBehaviour {

	KeyBindingManager kMgr;
	public Transform playerTr;
	Vector3 newPosition = Vector3.zero;
	Vector3 localPos = Vector3.zero;
	Quaternion newRotation = Quaternion.identity;
	public Camera cam;
	public Transform camTr;
	public PhotonView pv;

	public GameObject[] BlipImages;
	Vector3 screenPos = Vector3.zero;
	Vector2 onScreenPos = Vector2.zero;
	Vector3 newBlipPos = Vector3.zero;
	float distanceToBlip = 0f;
	float max = 0f;

	public GameObject[] NPCBlips;
	string BlipTag = "Blip";

	keyType MiniMapFovDecrease = keyType.MiniMapFOVDecrease;
	keyType MiniMapFovIncrease = keyType.MiniMapFOVIncrease;

	WaitForSeconds waitFor1 = new WaitForSeconds (1f * 0.6f);
	WaitForSeconds waitFor0_5 = new WaitForSeconds (0.5f * 0.6f);
	WaitForSeconds waitFor0_1 = new WaitForSeconds (0.1f * 0.6f);

	void Start(){

		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();
//		BlipImages = GameObject.FindGameObjectsWithTag ("BlipIndicator");
		if (PlayerPrefs.HasKey ("MiniMapSize")) {
			cam.orthographicSize = PlayerPrefs.GetFloat ("MiniMapSize", cam.orthographicSize);
		} else {
			cam.orthographicSize = 50;
		}

		if (PlayerPrefs.GetInt ("IsMiniMapOrthographic") == 1) {
			cam.orthographic = true;	
		} else {
			cam.orthographic = false;
			if (PlayerPrefs.HasKey ("MiniMapHeight")) {
				perspectiveHeight = PlayerPrefs.GetFloat ("MiniMapHeight", perspectiveHeight);
				newPosition = playerTr.position;
				newPosition.y = playerTr.position.y + perspectiveHeight;
				camTr.position = newPosition;
				camTr.localPosition = localPos;
			} else {
				perspectiveHeight = 40;
				newPosition = playerTr.position;
				newPosition.y = playerTr.position.y + perspectiveHeight;
				camTr.position = newPosition;
				localPos.x = camTr.localPosition.x;
				localPos.y = camTr.localPosition.y;
				localPos.z = -perspectiveHeight/3;
				camTr.localPosition = localPos;
			}
		}

		if (PerspectiveChangeCO == null) {
			PerspectiveChangeCO = PerspectiveChange_CO ();
			StartCoroutine (PerspectiveChangeCO);
		}

		if (MiniMapSettingCO == null) {
			MiniMapSettingCO = MiniMapSetting_CO ();
			StartCoroutine (MiniMapSettingCO);
		}

	}

	IEnumerator PerspectiveChangeCO;
	IEnumerator PerspectiveChange_CO(){
		while (true) {
			if (cam.orthographic) {
				if (Input.GetKey (KeyCode.X)) {
					cam.orthographic = false;
					PlayerPrefs.SetInt ("IsMiniMapOrthographic", 0);
					PlayerPrefs.Save ();
					newPosition = playerTr.position;
					newPosition.y = perspectiveHeight;
					LerPositionCO = LerpPosition_CO (camTr.position, newPosition);
					localPos.x = camTr.localPosition.x;
					localPos.y = camTr.localPosition.y;
					localPos.z = -perspectiveHeight/3;
					camTr.localPosition = localPos;

					yield return waitFor0_5;
				}
			} else {
				if (Input.GetKey (KeyCode.X)) {
					cam.orthographic = true;
					PlayerPrefs.SetInt ("IsMiniMapOrthographic", 1);
					PlayerPrefs.Save ();
					newPosition = playerTr.position;
					newPosition.y = playerTr.position.y + 5.2f;
					LerPositionCO = LerpPosition_CO (camTr.position, newPosition);

					yield return waitFor0_5;
				}
			}
			yield return true;
		}
	}

	IEnumerator LerPositionCO;
	IEnumerator LerpPosition_CO(Vector3 initialHeight, Vector3 finalHeight){
		float progress = 0f;
		float increment = 0f;
		float smoothness = 0.02f;
		float duration = 1f;
		increment =	smoothness / duration;

		while (progress < 1) {
			camTr.position = Vector3.Lerp (initialHeight, finalHeight, progress);
			progress += increment;
			yield return new WaitForSeconds (smoothness);
		}
			
	}

	IEnumerator MiniMapSettingCO;
	IEnumerator MiniMapSetting_CO(){
		while (true) {
			if (kMgr.GetKeyPublic (MiniMapFovDecrease)) {
				if (cam.orthographic) {
					if (cam.orthographicSize >= 10) {
						cam.orthographicSize -= 5;
						PlayerPrefs.SetFloat ("MiniMapSize", cam.orthographicSize);
						PlayerPrefs.Save ();
					}
				} else {
					if (perspectiveHeight > 20) {
						perspectiveHeight -= 5;
					} else {
						perspectiveHeight = 20;
					}
					PlayerPrefs.SetFloat ("MiniMapHeight", perspectiveHeight);
					PlayerPrefs.Save ();

					newPosition = playerTr.position;
					newPosition.y = camTr.position.y + perspectiveHeight;
					LerPositionCO = LerpPosition_CO (camTr.position, newPosition);
				}
				yield return waitFor0_1;
			} else if (kMgr.GetKeyPublic (MiniMapFovIncrease)) {
				if (cam.orthographic) {
					if (cam.orthographicSize <= 195) {
						cam.orthographicSize += 5;
					}
					PlayerPrefs.SetFloat ("MiniMapSize", cam.orthographicSize);
					PlayerPrefs.Save ();
				} else {
					if (perspectiveHeight < 200) {
						perspectiveHeight += 5;
					} else {
						perspectiveHeight = 200;
					}
					PlayerPrefs.SetFloat ("MiniMapHeight", perspectiveHeight);
					PlayerPrefs.Save ();
					newPosition = playerTr.position;
					newPosition.y = camTr.position.y + perspectiveHeight;
					LerPositionCO = LerpPosition_CO (camTr.position, newPosition);
				}
				yield return waitFor0_1;
			}

			if (cam.orthographicSize < 5) {
				cam.orthographicSize = 5;
				PlayerPrefs.SetFloat ("MiniMapSize", cam.orthographicSize);
				PlayerPrefs.Save ();
			} else if (cam.orthographicSize > 200) {
				cam.orthographicSize = 200;
				PlayerPrefs.SetFloat ("MiniMapSize", cam.orthographicSize);
				PlayerPrefs.Save ();
			}
			yield return true;
		}
	}

	float perspectiveHeight = 40;
	void LateUpdate () {

		if (!pv.isMine)
			return;


//		if (PerspectiveChangeCO == null) {
//			PerspectiveChangeCO = PerspectiveChange_CO ();
//			StartCoroutine (PerspectiveChangeCO);
//		}
//
//		if (MiniMapSettingCO == null) {
//			MiniMapSettingCO = MiniMapSetting_CO ();
//			StartCoroutine (MiniMapSettingCO);
//		}

		if (cam.orthographic) {
			newPosition = playerTr.position;
			newPosition.y = playerTr.position.y + 5.2f;
			camTr.position = newPosition;
		} else {
			newPosition = playerTr.position;
			newPosition.y = playerTr.position.y + perspectiveHeight;
			camTr.position = newPosition;
			localPos.x = camTr.localPosition.x;
			localPos.y = camTr.localPosition.y;
			localPos.z = -perspectiveHeight/3;
			camTr.localPosition = localPos;
		}



		if (cam.orthographic) {
			camTr.rotation = Quaternion.Euler (90f, playerTr.eulerAngles.y, 0f);
		} else {
			camTr.rotation = Quaternion.Euler (45f, playerTr.eulerAngles.y, 0f);
		}
			
//		if (UpdateNPCBlipOnScreenCO == null) {
//			UpdateNPCBlipOnScreenCO = UpdateNPCBlipOnScreen_CO ();
//			StartCoroutine (UpdateNPCBlipOnScreenCO);
//		}

	}

	IEnumerator UpdateNPCBlipOnScreenCO;
	IEnumerator UpdateNPCBlipOnScreen_CO(){
		NPCBlips = GameObject.FindGameObjectsWithTag ("Enemy");
		GetNPCIndicators (NPCBlips);
		yield return waitFor1;
		UpdateNPCBlipOnScreenCO = null;
	}

	float xDist=0f;
	float yDist=0f;
	float yPos;
	float xPos;
	float a;
	int blipIndex = 0;
	void GetNPCIndicators(GameObject[] Blips){
		foreach (GameObject blip in Blips) {
			blipIndex = 0;
//			screenPos = cam.WorldToViewportPoint (blip.transform.position); //get viewport positions

			screenPos = camTr.InverseTransformPoint (blip.transform.position);
			a = Mathf.Atan2 (screenPos.x, screenPos.z) * Mathf.Rad2Deg;
			a += 180;
			if (BlipImages [blipIndex].GetActive () == false) {
				BlipImages [blipIndex].SetActive (true);
			}
			BlipImages [blipIndex].GetComponent<Transform> ().localEulerAngles = new Vector3 (0, 0, a);
			blipIndex++;
			if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1) {

				Debug.Log ("already on screen, don't bother with the rest!");
				continue;
			}				



			onScreenPos = new Vector2 (screenPos.x - 25f, screenPos.y - 25f) * 2; //2D version, new mapping
			max = Mathf.Max (Mathf.Abs (onScreenPos.x), Mathf.Abs (onScreenPos.y)); //get largest offset
			onScreenPos = (onScreenPos / (max * 2)) + new Vector2 (0.5f, 25f); //undo mapping

			Debug.Log (onScreenPos);
		}
	}


}
