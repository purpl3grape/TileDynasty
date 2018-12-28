using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ViewRoomListTriggerBehavior : MonoBehaviour {

	public StatsManager statManager;
	public AudioClip[] ResetRegionSounds;
	public Color OriginalColor;
	public Color ClickColor;
	public Color ConfirmColor;
	private Color tempColor;
	[HideInInspector] public bool isTriggerTrue = false;
	[HideInInspector] public bool isViewRegionList = false;
	[HideInInspector] public int playerPrefRegion = 0;
	CloudRegionCode clientRegion = CloudRegionCode.asia;
	NetworkManager nManager;
	GuiManager guiManager;

	void Start () {
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		guiManager = nManager.GetComponent<GuiManager> ();
		statManager.ResetRegion.SetActive (false);
		statManager.RegionNextArrow.SetActive (false);
		statManager.RegionPreviousArrow.SetActive (false);
	}

	bool hasDisplayedAreaMessage = false;
	void Update () {

		if (!isTriggerTrue) {
			if (RegionMessageCoroutine != null) {
				StopCoroutine (RegionMessageCoroutine);
			}
			if (hasDisplayedAreaMessage) {
				if (guiManager.CloseMessagePanelCoroutine == null)
					guiManager.CloseMessagePanelCoroutine = guiManager.CloseMessagePanel_Coroutine ();
				StartCoroutine (guiManager.CloseMessagePanelCoroutine);
			}
			hasDisplayedAreaMessage = false;
			return;

		} else {
			if (!hasDisplayedAreaMessage) {
				hasDisplayedAreaMessage = true;
				RegionMessageCoroutine = RegionMessage_coroutine ("MULTIPLAYER REGION SETTING");
				StartCoroutine (RegionMessageCoroutine);
			}
		}

		if (!isViewRegionList && Input.GetKeyDown (KeyCode.Return)) {
			statManager.GetComponent<AudioSource> ().PlayOneShot (ResetRegionSounds [3]);
			isViewRegionList = true;
			return;
		}

		if (!isViewRegionList)
			return;

		RegionListUpdate ();
		RegionNavigationArrowUpdate ();
	}

	public void RegionNavigationArrowUpdate(){
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (playerPrefRegion > 0) {
				playerPrefRegion -= 1;
				StartCoroutine (ClickColor_Coroutine (statManager.RegionPreviousArrow.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ResetRegionSounds [1]);
			} else {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ResetRegionSounds [2]);
			}
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (playerPrefRegion < 7) {
				playerPrefRegion += 1;
				StartCoroutine (ClickColor_Coroutine (statManager.RegionNextArrow.GetComponent<Image> ()));
				statManager.GetComponent<AudioSource> ().PlayOneShot (ResetRegionSounds [1]);
			} else {
				statManager.GetComponent<AudioSource> ().PlayOneShot (ResetRegionSounds [2]);
			}
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			Image[] images = new Image[2] {
				statManager.RegionPreviousArrow.GetComponent<Image> (),
				statManager.RegionNextArrow.GetComponent<Image> ()
			};
			StartCoroutine (ConfirmColor_Coroutine (images));
			statManager.GetComponent<AudioSource> ().PlayOneShot (ResetRegionSounds [0]);
			StartCoroutine (ResetRegion_Coroutine ());
		}
	}

	IEnumerator ClickColor_Coroutine(Image img){
		statManager.RegionNextArrow.GetComponent<Image> ().color = OriginalColor;
		statManager.RegionPreviousArrow.GetComponent<Image> ().color = OriginalColor;

		tempColor = img.color;
		img.color = ClickColor;
		yield return new WaitForSeconds (0.1f);
		img.color = tempColor;
	}

	IEnumerator ConfirmColor_Coroutine(Image[] images){
		statManager.RegionNextArrow.GetComponent<Image> ().color = OriginalColor;
		statManager.RegionPreviousArrow.GetComponent<Image> ().color = OriginalColor;

		foreach (Image img in images) {
			tempColor = img.color;
			img.color = ConfirmColor;
		}
		yield return new WaitForSeconds (0.1f);
		foreach (Image img in images) {
			img.color = tempColor;
		}
	}

	IEnumerator ResetRegion_Coroutine(){
		yield return new WaitForSeconds (0.25f);
		PlayerPrefs.SetInt ("Region", playerPrefRegion);
		PlayerPrefs.Save();
		nManager.SceneLoadingPanel.SetActive (true);
		PhotonNetwork.Disconnect ();
		SceneManager.LoadScene (0);
	}

	public void RegionListUpdate(){

		if (playerPrefRegion == 0) {
			clientRegion = CloudRegionCode.asia;
			statManager.ResetRegion.GetComponent<Text> ().text = "ASIA";
		} else if (playerPrefRegion == 1) {
			clientRegion = CloudRegionCode.au;
			statManager.ResetRegion.GetComponent<Text> ().text = "AUSTRALIA";
		} else if (playerPrefRegion == 2) {
			clientRegion = CloudRegionCode.cae;
			statManager.ResetRegion.GetComponent<Text> ().text = "CANADA EAST";
		} else if (playerPrefRegion == 3) {
			clientRegion = CloudRegionCode.eu;
			statManager.ResetRegion.GetComponent<Text> ().text = "EUROPE";
		} else if (playerPrefRegion == 4) {
			clientRegion = CloudRegionCode.jp;
			statManager.ResetRegion.GetComponent<Text> ().text = "JAPAN";
		} else if (playerPrefRegion == 5) {
			clientRegion = CloudRegionCode.sa;
			statManager.ResetRegion.GetComponent<Text> ().text = "SOUTH AMERICA";
		} else if (playerPrefRegion == 6) {
			clientRegion = CloudRegionCode.us;
			statManager.ResetRegion.GetComponent<Text> ().text = "UNITED STATES EAST";
		} else if (playerPrefRegion == 7) {
			clientRegion = CloudRegionCode.usw;
			statManager.ResetRegion.GetComponent<Text> ().text = "UNITED STATES WEST";
		}

	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Player")) {
			isTriggerTrue = true;
			statManager.ResetRegion.SetActive (true);
			statManager.RegionPreviousArrow.SetActive (true);
			statManager.RegionNextArrow.SetActive (true);
			statManager.ResetRegion.GetComponent<Text> ().text = "PRESS ENTER TO SELECT REGION";
			statManager.GetComponent<AudioSource> ().PlayOneShot (ResetRegionSounds [3]);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("Player")) {
			isTriggerTrue = false;
			isViewRegionList = false;
			playerPrefRegion = 0;
			statManager.ResetRegion.SetActive (false);
			statManager.RegionPreviousArrow.SetActive (false);
			statManager.RegionNextArrow.SetActive (false);
			statManager.GetComponent<AudioSource> ().PlayOneShot (ResetRegionSounds [2]);
		}
	}



	public IEnumerator RegionMessageCoroutine;
	public IEnumerator RegionMessage_coroutine(string regionSettingMessage){

		guiManager.Msg_Obj.gameObject.SetActive (false);
		guiManager.Msg_NPC.gameObject.SetActive (false);
		guiManager.Msg_NPC.gameObject.SetActive (false);
		guiManager.Msg_Kill.gameObject.SetActive (false);
		guiManager.Msg_Death.gameObject.SetActive (false);
		guiManager.Msg_Area.gameObject.SetActive (false);
		guiManager.Msg_Ambush.gameObject.SetActive (false);
		guiManager.Msg_Item.gameObject.SetActive (false);
		guiManager.Msg_Spawn.gameObject.SetActive (false);

		if (guiManager.CloseMessagePanelCoroutine != null) {
			StopCoroutine (guiManager.CloseMessagePanelCoroutine);
		}
		if (guiManager.MessageBuilderCoroutine != null) {
			StopCoroutine (guiManager.MessageBuilderCoroutine);
		}
		if (guiManager.MessageBuilderCoroutine2 != null) {
			StopCoroutine (guiManager.MessageBuilderCoroutine2);
		}
		guiManager.DisplayMessagePanelCoroutine = guiManager.DisplayMessagePanel_Coroutine ();
		StartCoroutine (guiManager.DisplayMessagePanelCoroutine);

		guiManager.initMessage = regionSettingMessage;
		guiManager.MessageBuilderCoroutine = guiManager.MessageBuilder_Coroutine (guiManager.initMessage, "#FA3232FF");
		StartCoroutine (guiManager.MessageBuilderCoroutine);
		while (!guiManager.isMessageOver) {
			yield return new WaitForSeconds (0.02f);
		}
		yield return new WaitForSeconds (1f * .6f);			
		guiManager.CloseMessagePanelCoroutine = guiManager.CloseMessagePanel_Coroutine ();
		StartCoroutine (guiManager.CloseMessagePanelCoroutine);
		yield return true;
		RegionMessageCoroutine = null;
	}


}