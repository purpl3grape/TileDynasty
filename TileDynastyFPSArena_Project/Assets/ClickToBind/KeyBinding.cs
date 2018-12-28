using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class KeyBinding : MonoBehaviour {

	public delegate void remap(KeyBinding key);
	public static event remap keyRemap;

	//Name comes from enum in KeyBindingsManager scrip
	public keyType keyName;
	//keycode set in inspector and by player
	public KeyCode thisKey = KeyCode.W;
	//Text to display keycode for user feedback
	public Text keyDisplay;

	//Traditional Mouse(0) left click Shooting Bool Setting

	//Used for color changing during key binding
	public GameObject button;
	public Color toggleColor = new Color();
	Image buttonImage;
	Color originalColor;

	//Internal variables
	bool reassignKey = false;
	Event curEvent;


	void Update(){
		//ONLY AFFECTS JOYSTICK LABELS
		keyDisplay.text = JoyStickLabeler (keyDisplay.text);
	}

	//Changes in button behavior should be made here
	void OnGUI()
	{
		curEvent = Event.current;
		//Checks if key is pressed and if button has been pressed indicating wanting to re-assign
		if(curEvent.isKey && curEvent.keyCode != KeyCode.None && reassignKey)
		{
			thisKey = curEvent.keyCode;
			ChangeKeyCode(false);
			UpdateKeyCode();
			SaveKeyCode();
		}
	}

	//Initializes
	void Awake()
	{
		buttonImage = button.GetComponent<Image>();
		originalColor = buttonImage.color;
		button.GetComponent<Button>().onClick.AddListener(() => ChangeKeyCode(true));
		OnEnable_Function ();
	}

	void OnEnable(){
		OnEnable_Function ();
	}

	//Loads keycodes from player preferences
	void OnEnable_Function()
	{
		//Comment out this line it you want to allow multiple simultaneous assignments
		KeyBinding.keyRemap += PreventDoubleAssign;

		KeyCode tempKey;
		tempKey = (KeyCode) PlayerPrefs.GetInt(keyName.ToString());


		if(tempKey.ToString() == "None")
		{
			Debug.Log(thisKey.ToString());
			keyDisplay.text = thisKey.ToString();	
			UpdateKeyCode();
			SaveKeyCode();
		}
		else
		{	
			thisKey = tempKey;
			keyDisplay.text = thisKey.ToString();	
			UpdateKeyCode();
		}
	}

	void OnDisable()
	{
		KeyBinding.keyRemap -= PreventDoubleAssign;
	}

	//Called by button on GUI
	public void ChangeKeyCode(bool toggle)
	{
		reassignKey = toggle;

		if(toggle)
		{
			buttonImage.color = toggleColor;

			if(keyRemap != null)
				keyRemap(this);
		}
		else
			buttonImage.color = originalColor;
	}
		
	//saves keycode to player prefs
	public void SaveKeyCode()
	{
		keyDisplay.text = thisKey.ToString();
		PlayerPrefs.SetInt(keyName.ToString(),(int)thisKey);
		PlayerPrefs.Save();
	}


	//Prevents user from remapping two keys at the same time
	void PreventDoubleAssign(KeyBinding kb)
	{
		if(kb != this)
		{
			reassignKey = false;
			buttonImage.color = originalColor;
		}
	}

	//updates dictionary on key bindings manager
	public void UpdateKeyCode()
	{		
		KeyBindingManager.singleton.UpdateDictionary(keyName,thisKey);
	}

	/*
	 * DEFAULT BUTTON SETTINGS
	 */
	public void defaultNextWeapon(){
		if (keyName == keyType.switchNextWeap) {
			thisKey = KeyCode.G;
			SaveKeyCode ();
		}
	}
	public void defaultPreviousWeapon(){
		if (keyName == keyType.switchPreviousWeap) {
			thisKey = KeyCode.F;
			SaveKeyCode ();
		}
	}
	public void defaultLeftMouseShoot(){
		if (keyName == keyType.shoot) {
			thisKey = KeyCode.Mouse0;
			SaveKeyCode ();
		}
	}

	public void defaultJump(){
		if (keyName == keyType.jump) {
			thisKey = KeyCode.Space;
			SaveKeyCode ();
		}
	}

	public void defaultChat(){
		if (keyName == keyType.talk) {
			thisKey = KeyCode.T;
			SaveKeyCode ();
		}
	}

	public void defaultSelfDestruct(){
		if (keyName == keyType.SelfDestruct) {
			thisKey = KeyCode.X;
			SaveKeyCode ();
		}
	}

	public void defaultHostageDisengage(){
		if (keyName == keyType.stealth) {
			thisKey = KeyCode.H;
			SaveKeyCode ();
		}
	}

	public void defaultStealth(){
		if (keyName == keyType.stealth) {
			thisKey = KeyCode.LeftControl;
			SaveKeyCode ();
		}
	}

	public void defaultAxe(){
		if (keyName == keyType.switchAxeAmmo) {
			thisKey = KeyCode.Alpha1;
			SaveKeyCode ();
		}
	}
	

	public void defaultHMG(){
		if (keyName == keyType.switchHMGAmmo) {
			thisKey = KeyCode.Alpha2;
			SaveKeyCode ();
		}
	}

	public void defaultRail(){
		if (keyName == keyType.switchRailAmmo) {
			thisKey = KeyCode.Alpha3;
			SaveKeyCode ();
		}
	}

	public void defaultRocket(){
		if (keyName == keyType.switchRocketAmmo) {
			thisKey = KeyCode.Alpha4;
			SaveKeyCode ();
		}
	}

	public void defaultGrenade(){
		if (keyName == keyType.switchGrenadeAmmo) {
			thisKey = KeyCode.Alpha5;
			SaveKeyCode ();
		}
	}


	public void defaultMG(){
		if (keyName == keyType.switchMGAmmo) {
			thisKey = KeyCode.Alpha6;
			SaveKeyCode ();
		}
	}

	public void defaultForward(){
		if (keyName == keyType.forward) {
			thisKey = KeyCode.W;
			SaveKeyCode ();
		}
	}

	public void defaultBack(){
		if (keyName == keyType.back) {
			thisKey = KeyCode.S;
			SaveKeyCode ();
		}
	}

	public void defaultRight(){
		if (keyName == keyType.right) {
			thisKey = KeyCode.D;
			SaveKeyCode ();
		}
	}

	public void defaultLeft(){
		if (keyName == keyType.left) {
			thisKey = KeyCode.A;
			SaveKeyCode ();
		}
	}

	public void defaultScoreboard(){
		if (keyName == keyType.scoreboard) {
			thisKey = KeyCode.Tab;
			SaveKeyCode ();
		}
	}

	public void defaultReadyUp(){
		if (keyName == keyType.readyUp) {
			thisKey = KeyCode.M;
			SaveKeyCode ();
		}
	}

	public void defaultMiniMapFovDecrease(){
		if (keyName == keyType.MiniMapFOVDecrease) {
			thisKey = KeyCode.K;
			SaveKeyCode ();
		}
	}

	public void defaultMiniMapFovIncrease(){
		if (keyName == keyType.MiniMapFOVIncrease) {
			thisKey = KeyCode.L;
			SaveKeyCode ();
		}
	}


	public void defaultPS3Controls(){
		if (keyName == keyType.switchNextWeap) {
			thisKey = KeyCode.JoystickButton11;
			SaveKeyCode ();
		}
		if (keyName == keyType.switchPreviousWeap) {
			thisKey = KeyCode.JoystickButton10;
			SaveKeyCode ();
		}
		if (keyName == keyType.shoot) {
			thisKey = KeyCode.JoystickButton9;
			SaveKeyCode ();
		}
		if (keyName == keyType.jump) {
			thisKey = KeyCode.JoystickButton8;
			SaveKeyCode ();
		}
		if (keyName == keyType.talk) {
			thisKey = KeyCode.T;
			SaveKeyCode ();
		}
		if (keyName == keyType.SelfDestruct) {
			thisKey = KeyCode.JoystickButton4;
			SaveKeyCode ();
		}
		if (keyName == keyType.switchMGAmmo) {
			thisKey = KeyCode.JoystickButton12;
			SaveKeyCode ();
		}
		if (keyName == keyType.switchHMGAmmo) {
			thisKey = KeyCode.JoystickButton15;
			SaveKeyCode ();
		}
		if (keyName == keyType.switchRocketAmmo) {
			thisKey = KeyCode.JoystickButton13;
			SaveKeyCode ();
		}
		if (keyName == keyType.switchGrenadeAmmo) {
			thisKey = KeyCode.JoystickButton14;
			SaveKeyCode ();
		}
		if (keyName == keyType.switchRailAmmo) {
			thisKey = KeyCode.JoystickButton6;
			SaveKeyCode ();
		}
		if (keyName == keyType.switchPreviousWeap) {
			thisKey = KeyCode.JoystickButton1;
			SaveKeyCode ();
		}		
		if (keyName == keyType.switchNextWeap) {
			thisKey = KeyCode.JoystickButton2;
			SaveKeyCode ();
		}		
		if (keyName == keyType.HostageDisengage) {
			thisKey = KeyCode.JoystickButton16;
			SaveKeyCode ();
		}
		if (keyName == keyType.stealth) {
			thisKey = KeyCode.JoystickButton0;
			SaveKeyCode ();
		}
		if (keyName == keyType.readyUp) {
			thisKey = KeyCode.JoystickButton3;
			SaveKeyCode ();
		}
		if (keyName == keyType.settings) {
			thisKey = KeyCode.JoystickButton7;
			SaveKeyCode ();
		}
	}

	public string JoyStickLabeler(string keyCodeLabel){
		if (keyCodeLabel == "JoystickButton0") {
			return "START";
		} else if (keyCodeLabel == "JoystickButton1") {
			return "L3";
		} else if (keyCodeLabel == "JoystickButton2") {
			return "R3";
		} else if (keyCodeLabel == "JoystickButton3") {
			return "SELECT";
		} else if (keyCodeLabel == "JoystickButton4") {
			return "UP";
		} else if (keyCodeLabel == "JoystickButton5") {
			return "RIGHT";
		} else if (keyCodeLabel == "JoystickButton6") {
			return "DOWN";
		} else if (keyCodeLabel == "JoystickButton7") {
			return "LEFT";
		} else if (keyCodeLabel == "JoystickButton8") {
			return "L2";
		} else if (keyCodeLabel == "JoystickButton9") {
			return "R2";
		} else if (keyCodeLabel == "JoystickButton10") {
			return "L1";
		} else if (keyCodeLabel == "JoystickButton11") {
			return "R1";
		} else if (keyCodeLabel == "JoystickButton12") {
			return "TRIANGLE";
		} else if (keyCodeLabel == "JoystickButton13") {
			return "CIRCLE";
		} else if (keyCodeLabel == "JoystickButton14") {
			return "X";
		} else if (keyCodeLabel == "JoystickButton15") {
			return "SQUARE";
		} else if (keyCodeLabel == "JoystickButton16") {
			return "SOME PS3 BUTTON";
		} else if (keyCodeLabel == "None") {
			return "ANALOG STICKS";
		} else if (keyCodeLabel == "Mouse0") {
			return "LEFT CLICK";
		} else if (keyCodeLabel == "Mouse1") {
			return "RIGHT CLICK";
		} else if (keyCodeLabel == "Alpha0") {
			return "0";
		} else if (keyCodeLabel == "Alpha1") {
			return "1";
		} else if (keyCodeLabel == "Alpha2") {
			return "2";
		} else if (keyCodeLabel == "Alpha3") {
			return "3";
		} else if (keyCodeLabel == "Alpha4") {
			return "4";
		} else if (keyCodeLabel == "Alpha5") {
			return "5";
		} else if (keyCodeLabel == "Alpha6") {
			return "6";
		} else if (keyCodeLabel == "Alpha7") {
			return "7";
		} else if (keyCodeLabel == "Alpha8") {
			return "8";
		} else if (keyCodeLabel == "Alpha9") {
			return "9";
		} else if (keyCodeLabel == "BackQuote") {
			return keyCodeLabel + " (')";
		}

		else {
			//THIS IS NOT J0YSTICK LABEL
			return keyCodeLabel;
		}

	}




}
