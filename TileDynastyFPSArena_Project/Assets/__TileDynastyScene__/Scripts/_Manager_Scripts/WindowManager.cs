using UnityEngine;
using System.Collections;

public class WindowManager : MonoBehaviour {

	KeyBindingManager kMgr;

	MatchTimer matchTimer;
	GuiManager guiManager;

	WaitForSeconds waitFor0_25 = new WaitForSeconds (.25f * 0.6f);

	public EndGamePanelInputs endGameInputs;
	public PlayerInGamePanel playerInGamePanelInputs;
	public DisplayChatPanelInputs displayChatPanelInputs;
	public GameObject scoreBoard;

	bool dispScore = false;
	public bool respawnDisplayScoreBoard = false;

	// Use this for initialization
	void Start () {
		kMgr = GameObject.FindGameObjectWithTag ("KeyBindingManagerTag").GetComponent<KeyBindingManager> ();
		matchTimer = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<MatchTimer> ();
		guiManager = matchTimer.GetComponent<GuiManager> ();
		scoreBoard.SetActive (false);
		dispScore = false;
	}

	// Update is called once per frame
	void Update () {

		if (ViewScoreBoardCO == null) {
			ViewScoreBoardCO = ViewScoreBoard_Coroutine ();
			StartCoroutine (ViewScoreBoardCO);
		}

		if (!respawnDisplayScoreBoard) {
//			StartCoroutine(ViewScoreBoard_Coroutine());

		} else {
			SetSpawnDisplayScoreBoard (true);
			if (Input.anyKey) {
				respawnDisplayScoreBoard = false;
				SetSpawnDisplayScoreBoard (false);
			}
		}


	}

	bool isViewScoreBoardCoroutineStarted = false;
	IEnumerator ViewScoreBoardCO;
	IEnumerator ViewScoreBoard_Coroutine(){
		while (true) {

			if (!respawnDisplayScoreBoard) {
				if (kMgr.GetKeyPublic (keyType.scoreboard)) {
					if (!isViewScoreBoardCoroutineStarted) {
						isViewScoreBoardCoroutineStarted = true;
						if (matchTimer.GameEndedWindowDisplay) {
							scoreBoard.SetActive (true);
							endGameInputs.gameObject.SetActive (false);
							guiManager.MessagePanel.SetActive (false);
							playerInGamePanelInputs.Image_CrossHair.enabled = false;
							displayChatPanelInputs.Text_ChatMessage.enabled = false;
							yield return waitFor0_25;
						} else {
							scoreBoard.SetActive (true);
							playerInGamePanelInputs.Image_CrossHair.enabled = false;
							displayChatPanelInputs.Text_ChatMessage.enabled = false;
							guiManager.MessagePanel.SetActive (false);
							yield return waitFor0_25;
						}
					} else {
						isViewScoreBoardCoroutineStarted = false;
						if (matchTimer.GameEndedWindowDisplay) {
							scoreBoard.SetActive (false);
							endGameInputs.gameObject.SetActive (true);
							guiManager.MessagePanel.SetActive (true);
							yield return waitFor0_25;
						} else {
							scoreBoard.SetActive (false);
							playerInGamePanelInputs.Image_CrossHair.enabled = true;
							displayChatPanelInputs.Text_ChatMessage.enabled = true;
							yield return waitFor0_25;
						}
					}
				}
				if (Input.GetKeyDown (KeyCode.Escape)) {
					if (isViewScoreBoardCoroutineStarted) {
						isViewScoreBoardCoroutineStarted = false;
						if (matchTimer.GameEndedWindowDisplay) {
							scoreBoard.SetActive (false);
							endGameInputs.gameObject.SetActive (true);
							guiManager.MessagePanel.SetActive (true);
						} else {
							scoreBoard.SetActive (false);
							playerInGamePanelInputs.Image_CrossHair.enabled = true;
							displayChatPanelInputs.Text_ChatMessage.enabled = true;
						}
					}
				}
			}
			yield return true;
		}
	}

	void SetSpawnDisplayScoreBoard(bool val){
		if (val) {
			scoreBoard.SetActive (true);
			playerInGamePanelInputs.Image_CrossHair.enabled = false;
			displayChatPanelInputs.Text_ChatMessage.enabled = false;
		} else {
			scoreBoard.SetActive (false);
			playerInGamePanelInputs.Image_CrossHair.enabled = true;
			displayChatPanelInputs.Text_ChatMessage.enabled = true;		
		}
	}

	public bool getDispScore(){
		return this.dispScore;
	}

	public void setDispScore(bool val){
		this.dispScore = val;
	}
}
