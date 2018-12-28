using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanelInputs : MonoBehaviour {

	public GameObject MessageObject;


	/// <summary>
	/// </summary>
	public bool isMessageOver = false;
	public bool isDisplayMessagePanel = false;
	public Vector3 initialMessagePanelDimensions = new Vector3 (1, 0, 1);
	public Vector3 finalMessagePanelDimensions = new Vector3 (1, 1, 1);
	public IEnumerator DisplayMessagePanelCoroutine;
	public IEnumerator DisplayMessagePanel_Coroutine(){

		if (CloseMessagePanelCoroutine != null) {
			StopCoroutine (CloseMessagePanelCoroutine);
		}

		float progress = 0f; //This float will serve as the 3rd parameter of the lerp function.
		float smoothness = 0.02f;
		float duration = .1f;
		float increment = 0.2f;
		increment =	smoothness / duration; //The amount of change to apply.
		transform.localScale = new Vector3 (1, 0, 1);
		MessageObject.GetComponent<Text> ().text = "";
		isMessageOver = false;

		gameObject.SetActive (true);
		while (progress < 1 && !isDisplayMessagePanel) {
			transform.localScale = Vector3.Lerp (initialMessagePanelDimensions, finalMessagePanelDimensions, progress);
			progress += increment;
			yield return new WaitForSeconds (smoothness);
			if (progress >= 1)
				isDisplayMessagePanel = true;
		}
		transform.localScale = finalMessagePanelDimensions;

		yield return new WaitForSeconds (0.25f);
		DisplayMessagePanelCoroutine = null;

	}
	public IEnumerator CloseMessagePanelCoroutine;
	public IEnumerator CloseMessagePanel_Coroutine(){

		float progress = 0f; //This float will serve as the 3rd parameter of the lerp function.
		float smoothness = 0.02f;
		float duration = .1f;
		float increment = 0.2f;
		increment =	smoothness / duration; //The amount of change to apply.

		while (progress < 1 && isDisplayMessagePanel) {
			transform.localScale = Vector3.Lerp (finalMessagePanelDimensions, initialMessagePanelDimensions, progress);
			progress += increment;
			yield return new WaitForSeconds (smoothness);
			if (progress >= 1)
				isDisplayMessagePanel = false;
		}
		transform.localScale = initialMessagePanelDimensions;
		MessageObject.GetComponent<Text> ().text = "";
		gameObject.SetActive (false);
		isMessageOver = true;
		yield return true;
		CloseMessagePanelCoroutine = null;
	}

	public string initMessage = "";
	public IEnumerator MessageBuilderCoroutine;
	public IEnumerator MessageBuilder_Coroutine(string msg, string HexColor){

		if (CloseMessagePanelCoroutine != null) {
			StopCoroutine (CloseMessagePanelCoroutine);
		}

		string tempstr = "";
		isMessageOver = false;

		foreach (char c in msg) {
			if (HexColor == "") {				
				MessageObject.GetComponent<Text> ().text += c;
			} else {
				tempstr = "<color=" + HexColor + ">" + c + "</color>";
				MessageObject.GetComponent<Text> ().text += tempstr;
			}
			yield return new WaitForSeconds (0.02f);
		}

		if (DisplayMessagePanelCoroutine != null) {
			StopCoroutine (DisplayMessagePanelCoroutine);
		}
		isMessageOver = true;
		MessageBuilderCoroutine = null;
	}

	public IEnumerator MessageBuilderCoroutine2;
	public IEnumerator MessageBuilder_Coroutine2(string c, string HexColor){

		if (CloseMessagePanelCoroutine != null) {
			StopCoroutine (CloseMessagePanelCoroutine);
		}

		string tempstr = "";
		isMessageOver = false;

		if (HexColor == "") {			
			MessageObject.GetComponent<Text> ().text = c;
		} else {
			tempstr = "<color=" + HexColor + ">" + c + "</color>";
			MessageObject.GetComponent<Text> ().text = tempstr;
		}
		yield return new WaitForSeconds (0.02f);

		if (DisplayMessagePanelCoroutine != null) {
			StopCoroutine (DisplayMessagePanelCoroutine);
		}
		isMessageOver = true;
		MessageBuilderCoroutine2 = null;
	}



}
