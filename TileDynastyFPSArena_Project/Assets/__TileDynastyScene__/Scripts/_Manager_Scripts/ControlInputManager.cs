using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlInputManager : MonoBehaviour {

	[HideInInspector] public bool usePS3Controls=false;
	[HideInInspector] public bool invertView = false;
	[HideInInspector] public int controlPresetID = 0;	//1 - Bumper Jumper, 2 - Halo, 3 - Other (none defined yet)
	[HideInInspector] public float m_sens = 30f;
	[HideInInspector] public float zoom_sens = 30f;
	[HideInInspector] public float fov = 100f;
	[HideInInspector] public float zoom_fov = 60f;
	[HideInInspector] public float smoothingframes=2f;
	[HideInInspector] public bool switchWeaponDefault = true;

	public GameObject PS3MovementLabel;
	public GameObject Button_MiniMapFovDecrease;
	public GameObject Button_MiniMapFovIncrease;
	public GameObject Button_NextWeap;
	public GameObject Button_PrevWeap;
	public GameObject Button_Jump;
	public GameObject Button_Shoot;
	public GameObject Button_HostageDisengage;
	public GameObject Button_Stealth;
	public GameObject Button_Axe;
	public GameObject Button_MG;
	public GameObject Button_HMG;
	public GameObject Button_Rocket;
	public GameObject Button_Grenade;
	public GameObject Button_Rail;
	public GameObject Button_DestroySelf;
	public GameObject Button_Talk;
	public GameObject Button_LeaderBoard;
	public GameObject Button_ReadyUp;
	public GameObject Button_Zoom;
	public GameObject Button_Left;
	public GameObject Button_Right;
	public GameObject Button_Forward;
	public GameObject Button_Back;
	public GameObject Button_Sprint;
	public GameObject Button_Settings;
	public GameObject Image_PC;
	public GameObject Image_PS3;
	public GameObject Toggle_InvertView;
	public GameObject DropDown_PS3Preset;
	public GameObject Label_PS3Preset;
	public GameObject Value_MouseSensitivity;
	public GameObject Value_ZoomSensitivity;
	public GameObject Slider_MouseSensitivity;
	public GameObject Slider_ZoomSensitivity;
	public GameObject Value_Fov;
	public GameObject Value_ZoomFov;
	public GameObject Slider_Fov;
	public GameObject Slider_ZoomFov;
	public GameObject Value_SmoothingFrames;
	public GameObject Toggle_SwitchWeaponDefault;

	public GameObject KeyInputMenu_1;
	public GameObject KeyInputMenu_2;
	public GameObject KeyInputMenu_3;
	public GameObject InputTypeMenu;
	public GameObject SensitivityMenu;

	public GameObject Interpolation_Menu;
	public GameObject Prediction_Menu;
	public GameObject Bot_Menu;


	public int KeyInputPageNumber = 0;
	public int MainMenuPageNumber = 0;

	void Start () {
		Image_PC.SetActive (true);
		Image_PS3.SetActive (false);
		Toggle_InvertView.SetActive (false);
		DropDown_PS3Preset.SetActive (false);
		Label_PS3Preset.SetActive (false);

		Set_MouseSensitivity();
		Set_ZoomSensitivity ();
		Set_SmoothingFrames ();
		Set_FOV ();
		Set_ZoomFOV ();
		initializeInvertFlag ();
		initializeSwitchWeaponFlag ();
		KeyCodeUpdate ();


		m_sens = PlayerPrefs.HasKey ("MouseSensitivity") == true ? PlayerPrefs.GetFloat ("MouseSensitivity") : 100;

//		m_sens = (PlayerPrefs.GetFloat ("MouseSensitivity") < 1 || PlayerPrefs.GetFloat ("MouseSensitivity") > 300) ? 1 : PlayerPrefs.GetFloat ("MouseSensitivity");

		if (m_sens < 1)
			m_sens = 1;		
		if (m_sens > 300)
			m_sens = 300;
		PlayerPrefs.SetFloat ("MouseSensitivity", m_sens);
		zoom_sens = PlayerPrefs.HasKey ("ZoomSensitivity") == true ? PlayerPrefs.GetFloat ("ZoomSensitivity") : 50;
		if (zoom_sens < 1)
			zoom_sens = 1;
		if (zoom_sens > 300)
			zoom_sens = 300;
		PlayerPrefs.SetFloat ("ZoomSensitivity", zoom_sens);
		fov = PlayerPrefs.HasKey ("FOV") == true ? PlayerPrefs.GetFloat ("FOV") : 90;
		if (fov < 50)
			fov = 50;
		if (fov > 110)
			fov = 110;
		PlayerPrefs.SetFloat ("FOV", fov);
		zoom_fov = PlayerPrefs.HasKey ("ZoomFOV") == true ? PlayerPrefs.GetFloat ("ZoomFOV") : 75;
		if (zoom_fov < 50)
			zoom_fov = 50;
		if (zoom_fov > 110)
			zoom_fov = 110;
		PlayerPrefs.SetFloat ("ZoomFOV", zoom_fov);

	}

	//SWITCH WEAPON DEFAULT
	public void Toggle_SwitchWeaonMethod(){

		if (!KeyInputMenu_2.GetActive ())
			return;

		switchWeaponDefault = !switchWeaponDefault;
		if (switchWeaponDefault) {
			Toggle_SwitchWeaponDefault.GetComponent<Toggle> ().isOn = true;
			PlayerPrefs.SetInt ("SwitchWeaponDefault", 1);
		} else {
			Toggle_SwitchWeaponDefault.GetComponent<Toggle> ().isOn = false;
			PlayerPrefs.SetInt ("SwitchWeaponDefault", 0);
		}

		PlayerPrefs.Save ();
	}

	//MOUSE SENSITIVITY
	public void Button_MouseIncreaseMethod(){
		if (m_sens < 300) {
			m_sens += 1;
		}
		Value_MouseSensitivity.GetComponent<Text> ().text = m_sens.ToString ();
		PlayerPrefs.SetFloat ("MouseSensitivity", m_sens);
		PlayerPrefs.Save ();
	}	
	public void Button_MouseDecreaseMethod(){
		if (m_sens > 1) {
			m_sens -= 1;
		}
		Value_MouseSensitivity.GetComponent<Text> ().text = m_sens.ToString ();
		PlayerPrefs.SetFloat ("MouseSensitivity", m_sens);
		PlayerPrefs.Save ();
	}	
	public void Set_MouseSensitivity(){
		m_sens = PlayerPrefs.GetFloat ("MouseSensitivity");
		Value_MouseSensitivity.GetComponent<Text> ().text = m_sens.ToString ();
	}	

	//ZOOM SENSITIVITY
	public void Button_ZoomIncreaseMethod(){
		if (zoom_sens < 300) {
			zoom_sens += 1;
		}
		Value_ZoomSensitivity.GetComponent<Text> ().text = zoom_sens.ToString ();
		PlayerPrefs.SetFloat ("ZoomSensitivity", zoom_sens);
		PlayerPrefs.Save ();
	}	
	public void Button_ZoomDecreaseMethod(){
		if (zoom_sens > 1) {
			zoom_sens -= 1;
		}
		Value_ZoomSensitivity.GetComponent<Text> ().text = zoom_sens.ToString ();
		PlayerPrefs.SetFloat ("ZoomSensitivity", zoom_sens);
		PlayerPrefs.Save ();
	}	
	public void Set_ZoomSensitivity(){
		zoom_sens = PlayerPrefs.GetFloat ("ZoomSensitivity");
		Value_ZoomSensitivity.GetComponent<Text> ().text = zoom_sens.ToString ();
	}
	public void Set_SmoothingFrames(){
		if (!PlayerPrefs.HasKey ("SmoothingFrames")) {
			PlayerPrefs.SetFloat ("SmoothingFrames", 5);
		}
		if (PlayerPrefs.GetFloat ("SmoothingFrames") < 3) {
			PlayerPrefs.SetFloat ("SmoothingFrames", 3);
		}
		smoothingframes = PlayerPrefs.GetFloat ("SmoothingFrames");
		Value_SmoothingFrames.GetComponent<Text> ().text = smoothingframes.ToString ();
	}

	//FOV
	public void Button_FOVIncreaseMethod(){
		if (fov < 110) {
			fov += 1;
		}
		Value_Fov.GetComponent<Text> ().text = fov.ToString ();
		PlayerPrefs.SetFloat ("FOV", fov);
		PlayerPrefs.Save ();
	}	
	public void Button_FOVDecreaseMethod(){
		if (fov > 50) {
			fov -= 1;
		}
		Value_Fov.GetComponent<Text> ().text = fov.ToString ();
		PlayerPrefs.SetFloat ("FOV", fov);
		PlayerPrefs.Save ();
	}	
	public void Set_FOV(){
		fov = PlayerPrefs.GetFloat ("FOV");
		Value_Fov.GetComponent<Text> ().text = fov.ToString ();
	}

	//ZOOM FOV
	public void Button_ZoomFOVIncreaseMethod(){
		if (zoom_fov < 110) {
			zoom_fov += 1;
		}
		Value_ZoomFov.GetComponent<Text> ().text = zoom_fov.ToString ();
		PlayerPrefs.SetFloat ("ZoomFOV", zoom_fov);
		PlayerPrefs.Save ();
	}	
	public void Button_ZoomFOVDecreaseMethod(){
		if (zoom_fov > 50) {
			zoom_fov -= 1;
		}
		Value_ZoomFov.GetComponent<Text> ().text = zoom_fov.ToString ();
		PlayerPrefs.SetFloat ("ZoomFOV", zoom_fov);
		PlayerPrefs.Save ();
	}	
	public void Set_ZoomFOV(){
		zoom_fov = PlayerPrefs.GetFloat ("ZoomFOV");
		Value_ZoomFov.GetComponent<Text> ().text = zoom_fov.ToString ();
	}	

	public void Button_SmoothingFramesIncrease(){
		if (smoothingframes < 20) {
			smoothingframes += 1;
		}
		Value_SmoothingFrames.GetComponent<Text> ().text = smoothingframes.ToString ();
		PlayerPrefs.SetFloat ("SmoothingFrames", smoothingframes);
		PlayerPrefs.Save ();
	}
	public void Button_SmoothingFramesDecrease(){
		if (smoothingframes > 3) {
			smoothingframes -= 1;
		}
		Value_SmoothingFrames.GetComponent<Text> ().text = smoothingframes.ToString ();
		PlayerPrefs.SetFloat ("SmoothingFrames", smoothingframes);
		PlayerPrefs.Save ();
	}


	public void initializeSwitchWeaponFlag(){		
		switchWeaponDefault = PlayerPrefs.GetInt ("SwitchWeaponDefault") == 1 ? true : false;
		Toggle_SwitchWeaponDefault.GetComponent<Toggle> ().isOn = switchWeaponDefault;
	}

	public void initializeInvertFlag(){
		if (!PlayerPrefs.HasKey ("InvertFlag")) {
			PlayerPrefs.SetInt ("InvertFlag", 0);
			Toggle_InvertView.GetComponent<Toggle> ().isOn = false;
		} else {
			if (PlayerPrefs.GetInt ("InvertFlag") == 1) {
				Toggle_InvertView.GetComponent<Toggle> ().isOn = true;
			} else if (PlayerPrefs.GetInt ("InvertFlag") == 2) {
				Toggle_InvertView.GetComponent<Toggle> ().isOn = false;
			}
		}
		PlayerPrefs.Save ();
	}

	public void setInvertFlag(){
		//ButtonSwitch
		if (PlayerPrefs.GetInt ("InvertFlag") == 1) {
			Toggle_InvertView.GetComponent<Toggle> ().isOn = false;
		} else {
			Toggle_InvertView.GetComponent<Toggle> ().isOn = true;
		}
		if (Toggle_InvertView.GetComponent<Toggle> ().isOn) {
			invertView = true;
			PlayerPrefs.SetInt ("InvertFlag", 1);
		} else {
			invertView = false;
			PlayerPrefs.SetInt ("InvertFlag", 0);
		}
		PlayerPrefs.Save ();
	}

	public void KeyCodeUpdate (){
		if (!usePS3Controls) {
			PS3MovementLabel.GetComponent<Text> ().text = "P.C.";

			//P.C KEY BIND DEFAULTS WHEN REVERTING TO DISABLE PS3 CONTROLS
			Button_MiniMapFovDecrease.GetComponent<KeyBinding>().thisKey = KeyCode.K;
			Button_MiniMapFovDecrease.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_MiniMapFovDecrease.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_MiniMapFovIncrease.GetComponent<KeyBinding>().thisKey = KeyCode.L;
			Button_MiniMapFovIncrease.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_MiniMapFovIncrease.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_NextWeap.GetComponent<KeyBinding>().thisKey = KeyCode.G;
			Button_NextWeap.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_NextWeap.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_PrevWeap.GetComponent<KeyBinding>().thisKey = KeyCode.F;
			Button_PrevWeap.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_PrevWeap.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Shoot.GetComponent<KeyBinding>().thisKey = KeyCode.Mouse0;
			Button_Shoot.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Shoot.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Jump.GetComponent<KeyBinding>().thisKey = KeyCode.Space;
			Button_Jump.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Jump.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Talk.GetComponent<KeyBinding>().thisKey = KeyCode.T;
			Button_Talk.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Talk.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_DestroySelf.GetComponent<KeyBinding>().thisKey = KeyCode.O;
			Button_DestroySelf.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_DestroySelf.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_HostageDisengage.GetComponent<KeyBinding> ().thisKey = KeyCode.H;
			Button_HostageDisengage.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_HostageDisengage.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Stealth.GetComponent<KeyBinding> ().thisKey = KeyCode.LeftControl;
			Button_Stealth.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Stealth.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Axe.GetComponent<KeyBinding> ().thisKey = KeyCode.Alpha1;
			Button_Axe.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Axe.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_MG.GetComponent<KeyBinding>().thisKey = KeyCode.Q;
			Button_MG.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_MG.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_HMG.GetComponent<KeyBinding>().thisKey = KeyCode.E;
			Button_HMG.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_HMG.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Rocket.GetComponent<KeyBinding>().thisKey = KeyCode.Alpha4;
			Button_Rocket.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Rocket.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Grenade.GetComponent<KeyBinding>().thisKey = KeyCode.Alpha2;
			Button_Grenade.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Grenade.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Rail.GetComponent<KeyBinding>().thisKey = KeyCode.Alpha3;
			Button_Rail.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Rail.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_LeaderBoard.GetComponent<KeyBinding>().thisKey = KeyCode.Tab;
			Button_LeaderBoard.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_LeaderBoard.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_ReadyUp.GetComponent<KeyBinding>().thisKey = KeyCode.R;
			Button_ReadyUp.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_ReadyUp.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Zoom.GetComponent<KeyBinding>().thisKey = KeyCode.Mouse1;
			Button_Zoom.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Zoom.GetComponent<KeyBinding> ().SaveKeyCode ();

			Button_Left.GetComponent<KeyBinding>().thisKey = KeyCode.A;
			Button_Left.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Left.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Right.GetComponent<KeyBinding>().thisKey = KeyCode.D;
			Button_Right.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Right.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Forward.GetComponent<KeyBinding>().thisKey = KeyCode.W;
			Button_Forward.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Forward.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Back.GetComponent<KeyBinding>().thisKey = KeyCode.S;
			Button_Back.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Back.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Sprint.GetComponent<KeyBinding>().thisKey = KeyCode.LeftShift;
			Button_Sprint.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Sprint.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Settings.GetComponent<KeyBinding> ().thisKey = KeyCode.BackQuote;
			Button_Settings.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Settings.GetComponent<KeyBinding> ().SaveKeyCode ();

			Image_PC.SetActive (true);
			Image_PS3.SetActive (false);
			Toggle_InvertView.SetActive (false);
			DropDown_PS3Preset.SetActive (false);
			Label_PS3Preset.SetActive (false);

		} else {
			PS3MovementLabel.GetComponent<Text> ().text = "PS3";
			Image_PC.SetActive (false);
			Image_PS3.SetActive (true);
			Toggle_InvertView.SetActive (true);
			DropDown_PS3Preset.SetActive (true);
			Label_PS3Preset.SetActive (true);

			setPS3ControlPreset ();
		}
	}

	public void setPS3Controls(){
		usePS3Controls = !usePS3Controls;
		KeyCodeUpdate ();
	}


	public void setPS3ControlPreset(){

		controlPresetID = DropDown_PS3Preset.GetComponent<Dropdown> ().value;

		if (controlPresetID == 0) {
			//SET KEY TYPE OF EACH BUTTON INSTEAD OF KEYCODE. I.E. GIVEN A BUTTON 'L2', WE DEFINE THE FUNCTION THAT THIS BUTTON PERFORMS
			Button_NextWeap.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton2;
			Button_NextWeap.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_NextWeap.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_PrevWeap.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton1;
			Button_PrevWeap.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_PrevWeap.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Shoot.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton9;
			Button_Shoot.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Shoot.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Jump.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton8;
			Button_Jump.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Jump.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Talk.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton5;
			Button_Talk.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Talk.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_DestroySelf.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton4;
			Button_DestroySelf.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_DestroySelf.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_MG.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton12;
			Button_MG.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_MG.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_HMG.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton15;
			Button_HMG.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_HMG.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Rocket.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton13;
			Button_Rocket.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Rocket.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Grenade.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton14;
			Button_Grenade.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Grenade.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Rail.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton6;
			Button_Rail.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Rail.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_HostageDisengage.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton16;
			Button_HostageDisengage.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_HostageDisengage.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Stealth.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton0;
			Button_Stealth.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Stealth.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_ReadyUp.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton3;
			Button_ReadyUp.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_ReadyUp.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Zoom.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton10;
			Button_Zoom.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Zoom.GetComponent<KeyBinding> ().SaveKeyCode ();

			Button_Left.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Left.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Left.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Right.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Right.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Right.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Forward.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Forward.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Forward.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Back.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Back.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Back.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Sprint.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton11;
			Button_Sprint.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Sprint.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Settings.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton7;
			Button_Settings.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Settings.GetComponent<KeyBinding> ().SaveKeyCode ();
		} else if (controlPresetID == 1) {
			//JUMP=X, L1=SPRINT, L2=ZOOM, R1=GRENADE
			Button_NextWeap.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton2;
			Button_NextWeap.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_NextWeap.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_PrevWeap.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton1;
			Button_PrevWeap.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_PrevWeap.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Shoot.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton9;
			Button_Shoot.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Shoot.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Jump.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton14;
			Button_Jump.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Jump.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Talk.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton5;
			Button_Talk.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Talk.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_DestroySelf.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton4;
			Button_DestroySelf.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_DestroySelf.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_MG.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton12;
			Button_MG.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_MG.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_HMG.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton15;
			Button_HMG.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_HMG.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Rocket.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton13;
			Button_Rocket.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Rocket.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Grenade.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton11;
			Button_Grenade.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Grenade.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Rail.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton6;
			Button_Rail.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Rail.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_HostageDisengage.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton16;
			Button_HostageDisengage.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_HostageDisengage.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Stealth.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton0;
			Button_Stealth.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Stealth.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_ReadyUp.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton3;
			Button_ReadyUp.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_ReadyUp.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Zoom.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton8;
			Button_Zoom.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Zoom.GetComponent<KeyBinding> ().SaveKeyCode ();
		
			Button_Left.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Left.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Left.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Right.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Right.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Right.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Forward.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Forward.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Forward.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Back.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Back.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Back.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Sprint.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton10;
			Button_Sprint.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Sprint.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Settings.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton7;
			Button_Settings.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Settings.GetComponent<KeyBinding> ().SaveKeyCode ();
		} else {
			//SET KEY TYPE OF EACH BUTTON INSTEAD OF KEYCODE. I.E. GIVEN A BUTTON 'L2', WE DEFINE THE FUNCTION THAT THIS BUTTON PERFORMS
			Button_NextWeap.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton2;
			Button_NextWeap.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_NextWeap.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_PrevWeap.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton1;
			Button_PrevWeap.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_PrevWeap.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Shoot.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton9;
			Button_Shoot.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Shoot.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Jump.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton8;
			Button_Jump.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Jump.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Talk.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton5;
			Button_Talk.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Talk.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_DestroySelf.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton4;
			Button_DestroySelf.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_DestroySelf.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_MG.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton12;
			Button_MG.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_MG.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_HMG.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton15;
			Button_HMG.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_HMG.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Rocket.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton13;
			Button_Rocket.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Rocket.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Grenade.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton14;
			Button_Grenade.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Grenade.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Rail.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton6;
			Button_Rail.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Rail.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_HostageDisengage.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton16;
			Button_HostageDisengage.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_HostageDisengage.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Stealth.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton0;
			Button_Stealth.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Stealth.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_ReadyUp.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton3;
			Button_ReadyUp.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_ReadyUp.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Zoom.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton10;
			Button_Zoom.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Zoom.GetComponent<KeyBinding> ().SaveKeyCode ();

			Button_Left.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Left.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Left.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Right.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Right.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Right.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Forward.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Forward.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Forward.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Back.GetComponent<KeyBinding> ().thisKey = KeyCode.None;
			Button_Back.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Back.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Sprint.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton11;
			Button_Sprint.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Sprint.GetComponent<KeyBinding> ().SaveKeyCode ();
			Button_Settings.GetComponent<KeyBinding> ().thisKey = KeyCode.JoystickButton7;
			Button_Settings.GetComponent<KeyBinding> ().UpdateKeyCode ();
			Button_Settings.GetComponent<KeyBinding> ().SaveKeyCode ();
		}
	}


	public void NextPage_SettingsPanel(){
		if (KeyInputPageNumber == 4) {
			KeyInputPageNumber = 0;
			KeyInputMenu_1.SetActive (true);
			KeyInputMenu_2.SetActive (false);
			KeyInputMenu_3.SetActive (false);
			InputTypeMenu.SetActive (false);
			SensitivityMenu.SetActive (false);
		} else if (KeyInputPageNumber == 0) {
			KeyInputPageNumber = 1;
			KeyInputMenu_1.SetActive (false);
			KeyInputMenu_2.SetActive (true);
			KeyInputMenu_3.SetActive (false);
			InputTypeMenu.SetActive (false);
			SensitivityMenu.SetActive (false);
		} else if (KeyInputPageNumber == 1) {
			KeyInputPageNumber = 2;
			KeyInputMenu_1.SetActive (false);
			KeyInputMenu_2.SetActive (false);
			KeyInputMenu_3.SetActive (true);
			InputTypeMenu.SetActive (false);
			SensitivityMenu.SetActive (false);
		} else if (KeyInputPageNumber == 2) {
			KeyInputPageNumber = 3;
			KeyInputMenu_1.SetActive (false);
			KeyInputMenu_2.SetActive (false);
			KeyInputMenu_3.SetActive (false);
			InputTypeMenu.SetActive (true);
			SensitivityMenu.SetActive (false);
		} else if (KeyInputPageNumber == 3) {
			KeyInputPageNumber = 4;
			KeyInputMenu_1.SetActive (false);
			KeyInputMenu_2.SetActive (false);
			KeyInputMenu_3.SetActive (false);
			InputTypeMenu.SetActive (false);
			SensitivityMenu.SetActive (true);
		}
	}

	public void PreviousPage_SettingsPanel(){
		if (KeyInputPageNumber == 4) {
			KeyInputPageNumber = 3;
			KeyInputMenu_1.SetActive (false);
			KeyInputMenu_2.SetActive (false);
			KeyInputMenu_3.SetActive (false);
			InputTypeMenu.SetActive (true);
			SensitivityMenu.SetActive (false);
		} else if (KeyInputPageNumber == 3) {
			KeyInputPageNumber = 2;
			KeyInputMenu_1.SetActive (false);
			KeyInputMenu_2.SetActive (false);
			KeyInputMenu_3.SetActive (true);
			InputTypeMenu.SetActive (false);
			SensitivityMenu.SetActive (false);
		} else if (KeyInputPageNumber == 2) {
			KeyInputPageNumber = 1;
			KeyInputMenu_1.SetActive (false);
			KeyInputMenu_2.SetActive (true);
			KeyInputMenu_3.SetActive (false);
			InputTypeMenu.SetActive (false);
			SensitivityMenu.SetActive (false);
		} else if (KeyInputPageNumber == 1) {
			KeyInputPageNumber = 0;
			KeyInputMenu_1.SetActive (true);
			KeyInputMenu_2.SetActive (false);
			KeyInputMenu_3.SetActive (false);
			InputTypeMenu.SetActive (false);
			SensitivityMenu.SetActive (false);
		} else if (KeyInputPageNumber == 0) {
			KeyInputPageNumber = 4;
			KeyInputMenu_1.SetActive (false);
			KeyInputMenu_2.SetActive (false);
			KeyInputMenu_3.SetActive (false);
			InputTypeMenu.SetActive (false);
			SensitivityMenu.SetActive (true);
		}

	}

	public void Initialize_KeyInputPanel(){
		KeyInputMenu_1.SetActive (true);
		KeyInputMenu_2.SetActive (false);
		KeyInputMenu_3.SetActive (false);
		InputTypeMenu.SetActive (false);
		SensitivityMenu.SetActive (false);
	}

}