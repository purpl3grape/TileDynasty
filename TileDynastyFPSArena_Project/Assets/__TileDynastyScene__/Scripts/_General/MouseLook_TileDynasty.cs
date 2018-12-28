// original by asteins
// adapted by @torahhorse
// http://wiki.unity3d.com/index.php/SmoothMouseLook

// Instructions:
// There should be one MouseLook script on the Player itself, and another on the camera
// player's MouseLook should use MouseX, camera's MouseLook should use MouseY

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class MouseLook_TileDynasty : MonoBehaviour
{
	KeyBindingManager kMgr;

	Skybox skybox;
	NetworkManager nManager;
	GuiManager guiManager;
	MatchTimer mTimer;
	ControlInputManager cInputManager;
	Transform tr;
	Camera cam;
	float wideScreenAspect = 1f;
	float sensitivityXY = 0f;

	public enum RotationAxes { MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseX;
	public Material skyboxMaterial1;
	public Material skyboxMaterial2;
	public Color FogColorTeal;
	public Color FogColorGrey;

	public bool invertY = false;

	public float sensitivityX = 10F;
	public float sensitivityY = 10F;

	[Range(-75f, 75f)] public float minimumX = -60F;
	[Range(-75f, 75f)] public float maximumX = 60F;

	[Range(-75f, 75f)] public float minimumY = -80F;
	[Range(-75f, 75f)] public float maximumY = 80F;

	float rotationX = 0F;
	float rotationY = 0F;

	private List<float> rotArrayX = new List<float>();
	float rotAverageX = 0F;	

	private List<float> rotArrayY = new List<float>();
	float rotAverageY = 0F;

	public float framesOfSmoothing = 2;
	public float mouseSensitivityValue = 0f;

	Quaternion originalRotation;


	void InitAspect () 
	{
		// set the desired aspect ratio (the values in this example are
		// hard-coded for 16:9, but you could make them into public
		// variables instead so you can set them at design time)
		float targetaspect = 16.0f / 9.0f;

		// determine the game window's current aspect ratio
		float windowaspect = (float)Screen.width / (float)Screen.height;

		// current viewport height should be scaled by this amount
		float scaleheight = windowaspect / targetaspect;

		// obtain camera component so we can modify its viewport
		Camera camera = GetComponent<Camera>();

		// if scaled height is less than current height, add letterbox
		if (scaleheight < 1.0f)
		{  
			Rect rect = camera.rect;

			rect.width = 1.0f;
			rect.height = scaleheight;
			rect.x = 0;
			rect.y = (1.0f - scaleheight) / 2.0f;

			camera.rect = rect;
		}
		else // add pillarbox
		{
			float scalewidth = 1.0f / scaleheight;

			Rect rect = camera.rect;

			rect.width = scalewidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scalewidth) / 2.0f;
			rect.y = 0;

			camera.rect = rect;
		}
	}


	void Start ()
	{		

		if (QualitySettings.GetQualityLevel () == 0) {
			
		}

		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();			
		skybox = GetComponent<Skybox> ();
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		guiManager = nManager.GetComponent<GuiManager> ();
		mTimer = nManager.GetComponent< MatchTimer> ();
		cInputManager = nManager.GetComponent<ControlInputManager> ();

		framesOfSmoothing = cInputManager.smoothingframes;

		if (GetComponent<Rigidbody> ()) {
			GetComponent<Rigidbody> ().freezeRotation = true;
		}

		tr = GetComponent<Transform> ();
		originalRotation = tr.localRotation;
		mouseSensitivityValue = cInputManager.m_sens;
		Camera.main.fieldOfView = cInputManager.fov;

		skybox.material.color = Color.black;
		cam = GetComponent<Camera> ();
		wideScreenAspect = 16f / 9f;
		cam.aspect = wideScreenAspect;


		if (SceneManager.GetActiveScene ().buildIndex == 5 || SceneManager.GetActiveScene ().buildIndex == 2) {
			RenderSettings.fogColor = guiManager.fogNight;
			RenderSettings.fogDensity = 0.015f;		
		} else {
			RenderSettings.fogColor = guiManager.fogNight;
			RenderSettings.fogDensity = 0.015f;
		}
	}

	string Mouse_X = "Mouse X";
	string Mouse_Y = "Mouse Y";
	void FixedUpdate ()
	{
		if (cam.aspect != wideScreenAspect) {
			cam.aspect = cam.aspect = wideScreenAspect;
		}
		if (nManager.SettingsPanel.GetActive () || nManager.ChatPanel.GetActive()) {
			return;
		}


		if (kMgr.GetKeyPublic (keyType.zoom)) {
			mouseSensitivityValue = cInputManager.zoom_sens;
		} else {
			mouseSensitivityValue = cInputManager.m_sens;
		}

		if (axes == RotationAxes.MouseX) {			
			rotAverageX = 0f;
			if (cInputManager.usePS3Controls) {
				rotationX += Input.GetAxisRaw (Mouse_X) * (mouseSensitivityValue / 3f) * Time.timeScale;
			} else {
				rotationX += Input.GetAxis (Mouse_X) * (mouseSensitivityValue / 3f) * Time.timeScale;
			}

			rotArrayX.Add (rotationX);

			if (rotArrayX.Count >= framesOfSmoothing) {
				rotArrayX.RemoveAt (0);
			}
			for (int i = 0; i < rotArrayX.Count; i++) {
				rotAverageX += rotArrayX [i];
			}
			rotAverageX /= rotArrayX.Count;
			rotAverageX = ClampAngle (rotAverageX, minimumX, maximumX);

			//			Quaternion xQuaternion = Quaternion.AngleAxis (rotAverageX, Vector3.up);
		} else {			
			rotAverageY = 0f;

			float invertFlag = 1f;
			if (invertY) {
				invertFlag = -1f;
			}

			if (cInputManager.usePS3Controls) {
				if (!cInputManager.invertView) {
					rotationY += Input.GetAxisRaw (Mouse_Y) * (mouseSensitivityValue / 3f) * 0.1f * Time.timeScale;
				} else {
					rotationY -= Input.GetAxisRaw (Mouse_Y) * (mouseSensitivityValue / 3f) * 0.1f * Time.timeScale;
				}
			} else{
				rotationY += Input.GetAxis (Mouse_Y) * (mouseSensitivityValue / 3f) * 0.1f * invertFlag * Time.timeScale;
			}

			rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

			rotArrayY.Add(rotationY);

			if (rotArrayY.Count >= framesOfSmoothing)
			{
				rotArrayY.RemoveAt(0);
			}
			for(int j = 0; j < rotArrayY.Count; j++)
			{
				rotAverageY += rotArrayY[j];
			}
			rotAverageY /= rotArrayY.Count;

			Quaternion yQuaternion = Quaternion.AngleAxis (rotAverageY, Vector3.left);
			tr.localRotation = originalRotation * yQuaternion;

		}
	}

	public void SetSensitivity(float s)
	{
		sensitivityX = s;
		sensitivityY = s;
	}

	public static float ClampAngle (float angle, float min, float max)
	{
		angle = angle % 360;
		if ((angle >= -360F) && (angle <= 360F)) {
			if (angle < -360F) {
				angle += 360F;
			}
			if (angle > 360F) {
				angle -= 360F;
			}			
		}
		return Mathf.Clamp (angle, min, max);
	}

}