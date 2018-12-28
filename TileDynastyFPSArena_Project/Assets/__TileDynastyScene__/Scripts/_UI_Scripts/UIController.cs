

using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

    public GameObject pauseButton;
    public GameObject TeamClassPanel;
	public GameObject SettingsPanel;
    public ControllerCanvasManager controllerCanvas;

	public void PauseGame() {
      pauseButton.SetActive(false);
      TeamClassPanel.SetActive(true);
	  controllerCanvas.backgroundImage.SetActive (true);
    }

    public void UnpauseGame() {
      pauseButton.SetActive(true);
      TeamClassPanel.SetActive(false);
	  controllerCanvas.backgroundImage.SetActive (false);
    }

}

