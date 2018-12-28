using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum enumBottomMenuPage{
	Menu1,
	Menu2,
};

public enum enumTopMenuPage{
	MenuObjective,
	MenuAudio,
	MenuBots,
	MenuInterpolation,
	MenuExtrapolation,
};

public enum enumDisplayBotMenu{
	DisplayBotMenuRed,
	DisplayBotMenuBlue,
	DisplayBotSpawnDistance,
	DisplayBotSpeed,
}

public enum enumDisplayInterpolationMenu{
	DisplayClientInterpolation,
	DisplayEnemyInterpolation,
}

public enum enumDisplayExtrapolationMenu{
	DisplayExtrapolation,
	DisplaySyncDistance,
}

public enum enumMissionObjectives{
	ObjectiveCaptureEnemyBase,
	ObjectiveEliminateEnemy,
	ObjectiveRescueSurvivors,
}

public class SettingsPanelInput : MonoBehaviour {

	NetworkManager nManager;
	GuiManager guiManager;

	//OBJECTIVE
	public GameObject MissionLabel;
	public GameObject ObjectiveMenu;
	public GameObject Button_ObjectivePrev;
	public GameObject Button_ObjectiveNext;
	public GameObject ObjectiveCaptureEnemyBases;
	public GameObject ObjectiveEliminateEnemy;
	public GameObject ObjectiveRescueSurvivors;
	public GameObject ObjectiveCaptureEnemyBases_Status1;
	public GameObject ObjectiveCaptureEnemyBases_Status2;
	public GameObject ObjectiveEliminateEnemy_Status1;
	public GameObject ObjectiveEliminateEnemy_Status2;
	public GameObject ObjectiveRescueSurvivors_Status1;
	public GameObject ObjectiveRescueSurvivors_Status2;

	//AUDIO
	public GameObject AudioMenu;
	public GameObject Value_MusicVolume;
	public GameObject Button_MusicVolumeDown;
	public GameObject Button_MusicVolumeUp;

	//INTERPOLATION
	public GameObject InterPolationMenu;
	public GameObject ButtonInterpolationPrev;
	public GameObject ButtonInterpolationNext;

	public GameObject Label_ClientPrediction;
	public GameObject Value_ClientPrediction;
	public GameObject ButtonClientPredDown;
	public GameObject ButtonClientPredUp;

	public GameObject Label_EnemyPrediction;
	public GameObject Value_EnemyPrediction;
	public GameObject ButtonEnemyPredDown;
	public GameObject ButtonEnemyPredUp;

	//EXTRAPOLATION
	public GameObject ExtrapolationMenu;
	public GameObject ButtonExtrapolationPrev;
	public GameObject ButtonExtrapolationNext;

	public GameObject Label_VelocityPrediction;
	public GameObject Value_VelocityPrediction;
	public GameObject ButtonVelocityPredDown;
	public GameObject ButtonVelocityPredUp;

	public GameObject Label_SyncDistance;
	public GameObject Value_SyncDistance;
	public GameObject ButtonSyncDistanceDown;
	public GameObject ButtonSyncDistanceUp;


	//BOTS
	public GameObject BotsMenu;
	public GameObject ButtonBotOptionPrev;
	public GameObject ButtonBotOptionNext;
	public GameObject Value_BlueBotCount;
	public GameObject ButtonBlueBotDown;
	public GameObject ButtonBlueBotUp;
	public GameObject BotImageBlue;
	public GameObject Value_RedBotCount;
	public GameObject ButtonRedBotDown;
	public GameObject ButtonRedBotUp;
	public GameObject BotImageRed;
	public GameObject ButtonBotSpawnDistanceIncrease;
	public GameObject ButtonBotSpawnDistanceDecrease;
	public GameObject LabelBotSpawnDistance;
	public GameObject ValueBotSpawnDistance;
	public GameObject ButtonBotSpeedIncrease;
	public GameObject ButtonBotSpeedDecrease;
	public GameObject LabelBotSpeed;
	public GameObject ValueBotSpeed;

	//BOTTOM MENU ITEMS
	public GameObject Button_Resume_1;
	public GameObject Button_Objectives_1;
	public GameObject SpacingMainA_1;
	public GameObject SpacingA_2;
	public GameObject Button_KeyInput_2;
	public GameObject Button_Audio_2;
	public GameObject Button_ChatDisplay_2;
	public GameObject SpacingB_2;
	public GameObject Button_Bots_3;
	public GameObject Button_Interpolation_3;
	public GameObject Button_Extrapolation_3;
	public GameObject Button_SupportMe_3;
	public GameObject SpacingMainA_4;
	public GameObject Button_BackToLobby_4;
	public GameObject Button_Quit_4;

	[HideInInspector]public int MenuPage = -9;
	[HideInInspector]public int TopMenuPage = -9;
	[HideInInspector]public int DisplayObjectivesPage = -9;
	[HideInInspector]public int BotMenuPage = -9;
	[HideInInspector]public int InterpolationMenuPage = -9;
	[HideInInspector]public int ExtrapolationMenuPage = -9;

	void OnEnable(){
		MenuPage = -9;
		TopMenuPage = -9;
		DisplayObjectivesPage = -9;
		BotMenuPage = -9;
		InterpolationMenuPage = -9;
		ExtrapolationMenuPage = -9;

		//DEFAULT TOP MENU TO OBJECTIVE
		ToggleTopMenuObjective ();

		//BOTTOM MENU REFRESH
		RefreshBottomMenu ();

		//SUB TOP MENU ITEMS REFRESH
		RefreshObjectiveOptions ();
		RefreshBotOptions ();
		RefreshInterpolationOptions ();
		RefreshExtrapolationOptions ();
	}

	void Awake(){
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		guiManager = nManager.GetComponent<GuiManager> ();
	}


	public void ToggleObjectivePrev(){
		DisplayObjectivesPage = (int)enumMissionObjectives.ObjectiveRescueSurvivors;
		ObjectiveCaptureEnemyBases.SetActive (false);
		ObjectiveEliminateEnemy.SetActive (false);
		ObjectiveRescueSurvivors.SetActive (true);	
	}

	public void ToggleObjectiveNext(){
		DisplayObjectivesPage = (int)enumMissionObjectives.ObjectiveRescueSurvivors;
		ObjectiveCaptureEnemyBases.SetActive (false);
		ObjectiveEliminateEnemy.SetActive (false);
		ObjectiveRescueSurvivors.SetActive (true);
	}

//	public void ToggleObjectivePrev(){
//		if (DisplayObjectivesPage == (int)enumMissionObjectives.ObjectiveEliminateEnemy) {
//			DisplayObjectivesPage = (int)enumMissionObjectives.ObjectiveRescueSurvivors;
//			ObjectiveCaptureEnemyBases.SetActive (false);
//			ObjectiveEliminateEnemy.SetActive (false);
//			ObjectiveRescueSurvivors.SetActive (true);
//		} else if (DisplayObjectivesPage == (int)enumMissionObjectives.ObjectiveRescueSurvivors) {
//			DisplayObjectivesPage = (int)enumMissionObjectives.ObjectiveCaptureEnemyBase;
//			ObjectiveCaptureEnemyBases.SetActive (true);
//			ObjectiveEliminateEnemy.SetActive (false);
//			ObjectiveRescueSurvivors.SetActive (false);
//		} else if (DisplayObjectivesPage == (int)enumMissionObjectives.ObjectiveCaptureEnemyBase) {
//			DisplayObjectivesPage = (int)enumMissionObjectives.ObjectiveEliminateEnemy;
//			ObjectiveCaptureEnemyBases.SetActive (false);
//			ObjectiveEliminateEnemy.SetActive (true);
//			ObjectiveRescueSurvivors.SetActive (false);
//		}
//	}
//
//	public void ToggleObjectiveNext(){
//		if (DisplayObjectivesPage == (int)enumMissionObjectives.ObjectiveCaptureEnemyBase) {
//			DisplayObjectivesPage = (int)enumMissionObjectives.ObjectiveRescueSurvivors;
//			ObjectiveCaptureEnemyBases.SetActive (false);
//			ObjectiveEliminateEnemy.SetActive (false);
//			ObjectiveRescueSurvivors.SetActive (true);
//		} else if (DisplayObjectivesPage == (int)enumMissionObjectives.ObjectiveRescueSurvivors) {
//			DisplayObjectivesPage = (int)enumMissionObjectives.ObjectiveEliminateEnemy;
//			ObjectiveCaptureEnemyBases.SetActive (false);
//			ObjectiveEliminateEnemy.SetActive (true);
//			ObjectiveRescueSurvivors.SetActive (false);
//		} else if (DisplayObjectivesPage == (int)enumMissionObjectives.ObjectiveEliminateEnemy){
//			DisplayObjectivesPage = (int)enumMissionObjectives.ObjectiveCaptureEnemyBase;
//			ObjectiveCaptureEnemyBases.SetActive (true);
//			ObjectiveEliminateEnemy.SetActive (false);
//			ObjectiveRescueSurvivors.SetActive (false);
//		}
//	}

	public void ToggleTopMenuAudio(){
		if (TopMenuPage != (int)enumTopMenuPage.MenuAudio) {
			TopMenuPage = (int)enumTopMenuPage.MenuAudio;
			AudioMenu.SetActive (true);
			ObjectiveMenu.SetActive (false);
			Button_ObjectivePrev.SetActive (false);
			Button_ObjectiveNext.SetActive (false);
			BotsMenu.SetActive (false);
			InterPolationMenu.SetActive (false);
			ExtrapolationMenu.SetActive (false);
		}
	}

	public void ToggleTopMenuObjective(){
		if (TopMenuPage != (int)enumTopMenuPage.MenuObjective) {
			TopMenuPage = (int)enumTopMenuPage.MenuObjective;
			ObjectiveMenu.SetActive (true);
			Button_ObjectivePrev.SetActive (true);
			Button_ObjectiveNext.SetActive (true);
			AudioMenu.SetActive (false);
			BotsMenu.SetActive (false);
			InterPolationMenu.SetActive (false);
			ExtrapolationMenu.SetActive (false);
		}
	}

	//KEY INPUT (WILL BE AN OVERLAYING PANEL DUE TO THE NUMBER OF KEYS TO INPUT)

	public void ToggleTopMenuBots(){
		if (TopMenuPage != (int)enumTopMenuPage.MenuBots) {
			TopMenuPage = (int)enumTopMenuPage.MenuBots;
			BotsMenu.SetActive (true);
			AudioMenu.SetActive (false);
			ObjectiveMenu.SetActive (false);
			Button_ObjectivePrev.SetActive (false);
			Button_ObjectiveNext.SetActive (false);
			InterPolationMenu.SetActive (false);
			ExtrapolationMenu.SetActive (false);
		}
	}

	public void ToggleTopMenuInterpolation(){
		if (TopMenuPage != (int)enumTopMenuPage.MenuInterpolation) {
			TopMenuPage = (int)enumTopMenuPage.MenuInterpolation;
			InterPolationMenu.SetActive (true);
			AudioMenu.SetActive (false);
			ObjectiveMenu.SetActive (false);
			Button_ObjectivePrev.SetActive (false);
			Button_ObjectiveNext.SetActive (false);
			BotsMenu.SetActive (false);
			ExtrapolationMenu.SetActive (false);
		}
	}

	public void ToggleTopMenuExtrapolation(){
		if (TopMenuPage != (int)enumTopMenuPage.MenuExtrapolation) {
			TopMenuPage = (int)enumTopMenuPage.MenuExtrapolation;
			ExtrapolationMenu.SetActive (true);
			AudioMenu.SetActive (false);
			ObjectiveMenu.SetActive (false);
			Button_ObjectivePrev.SetActive (false);
			Button_ObjectiveNext.SetActive (false);
			BotsMenu.SetActive (false);
			InterPolationMenu.SetActive (false);
		}
	}

	public void RefreshBottomMenu(){	
		if (MenuPage != (int)enumBottomMenuPage.Menu1) {
			MenuPage = (int)enumBottomMenuPage.Menu1;

			SpacingA_2.SetActive (true);
			Button_KeyInput_2.SetActive (true);
			Button_Audio_2.SetActive (true);
			Button_ChatDisplay_2.SetActive (true);
			SpacingB_2.SetActive (true);

			Button_Bots_3.SetActive (false);
			Button_Interpolation_3.SetActive (false);
			Button_Extrapolation_3.SetActive (false);
			Button_SupportMe_3.SetActive (false);
		}
	}

	public void RefreshObjectiveOptions(){	
		
		if (DisplayObjectivesPage != (int)enumMissionObjectives.ObjectiveRescueSurvivors) {
			DisplayObjectivesPage = (int)enumMissionObjectives.ObjectiveRescueSurvivors;

			ObjectiveCaptureEnemyBases.SetActive (false);
			ObjectiveEliminateEnemy.SetActive (false);
			ObjectiveRescueSurvivors.SetActive (true);

		}			
	}

	public void RefreshBotOptions(){	

		if (BotMenuPage != (int)enumDisplayBotMenu.DisplayBotMenuBlue) {
			BotMenuPage = (int)enumDisplayBotMenu.DisplayBotMenuBlue;

			Value_BlueBotCount.SetActive (true);
			ButtonBlueBotDown.SetActive (true);
			ButtonBlueBotUp.SetActive (true);
			BotImageBlue.SetActive (true);

			Value_RedBotCount.SetActive (false);
			ButtonRedBotDown.SetActive (false);
			ButtonRedBotUp.SetActive (false);
			BotImageRed.SetActive (false);

			ValueBotSpawnDistance.SetActive (false);
			LabelBotSpawnDistance.SetActive (false);
			ButtonBotSpawnDistanceIncrease.SetActive (false);
			ButtonBotSpawnDistanceDecrease.SetActive (false);

			ValueBotSpeed.SetActive (false);
			LabelBotSpeed.SetActive (false);
			ButtonBotSpeedIncrease.SetActive (false);
			ButtonBotSpeedDecrease.SetActive (false);

			if (PlayerPrefs.HasKey ("BotSpawnDistance")) {
				ValueBotSpawnDistance.GetComponent<Text> ().text = PlayerPrefs.GetInt ("BotSpawnDistance").ToString ();
			} else {
				PlayerPrefs.SetInt ("BotSpawnDistance", 125);
				PlayerPrefs.Save ();
				ValueBotSpawnDistance.GetComponent<Text> ().text = PlayerPrefs.GetInt ("BotSpawnDistance").ToString ();
			}
			if (PlayerPrefs.HasKey ("BotSpeed")) {
				ValueBotSpeed.GetComponent<Text> ().text = PlayerPrefs.GetInt ("BotSpeed").ToString ();
			} else {
				PlayerPrefs.SetInt ("BotSpeed", 15);
				PlayerPrefs.Save ();
				ValueBotSpeed.GetComponent<Text> ().text = PlayerPrefs.GetInt ("BotSpeed").ToString ();
			}
		}			
	}


	public void RefreshExtrapolationOptions(){	

		if (ExtrapolationMenuPage != (int)enumDisplayExtrapolationMenu.DisplayExtrapolation) {
			ExtrapolationMenuPage = (int)enumDisplayExtrapolationMenu.DisplayExtrapolation;

			Label_VelocityPrediction.SetActive (true);
			Value_VelocityPrediction.SetActive (true);
			ButtonVelocityPredDown.SetActive (true);
			ButtonVelocityPredUp.SetActive (true);

			Label_SyncDistance.SetActive (false);
			Value_SyncDistance.SetActive (false);
			ButtonSyncDistanceDown.SetActive (false);
			ButtonSyncDistanceUp.SetActive (false);
		}			
	}

	public void ToggleExtrapolationOptionsNext(){	

		if (ExtrapolationMenuPage != (int)enumDisplayExtrapolationMenu.DisplayExtrapolation) {
			ExtrapolationMenuPage = (int)enumDisplayExtrapolationMenu.DisplayExtrapolation;

			Label_VelocityPrediction.SetActive (true);
			Value_VelocityPrediction.SetActive (true);
			ButtonVelocityPredDown.SetActive (true);
			ButtonVelocityPredUp.SetActive (true);

			Label_SyncDistance.SetActive (false);
			Value_SyncDistance.SetActive (false);
			ButtonSyncDistanceDown.SetActive (false);
			ButtonSyncDistanceUp.SetActive (false);
		} else if (ExtrapolationMenuPage != (int)enumDisplayExtrapolationMenu.DisplaySyncDistance) {
			ExtrapolationMenuPage = (int)enumDisplayExtrapolationMenu.DisplaySyncDistance;

			Label_VelocityPrediction.SetActive (false);
			Value_VelocityPrediction.SetActive (false);
			ButtonVelocityPredDown.SetActive (false);
			ButtonVelocityPredUp.SetActive (false);

			Label_SyncDistance.SetActive (true);
			Value_SyncDistance.SetActive (true);
			ButtonSyncDistanceDown.SetActive (true);
			ButtonSyncDistanceUp.SetActive (true);
		}
	}

	public void ToggleExtrapolationOptionsPrev(){	

		if (ExtrapolationMenuPage != (int)enumDisplayExtrapolationMenu.DisplaySyncDistance) {
			ExtrapolationMenuPage = (int)enumDisplayExtrapolationMenu.DisplaySyncDistance;

			Label_VelocityPrediction.SetActive (false);
			Value_VelocityPrediction.SetActive (false);
			ButtonVelocityPredDown.SetActive (false);
			ButtonVelocityPredUp.SetActive (false);

			Label_SyncDistance.SetActive (true);
			Value_SyncDistance.SetActive (true);
			ButtonSyncDistanceDown.SetActive (true);
			ButtonSyncDistanceUp.SetActive (true);
		} else if (ExtrapolationMenuPage != (int)enumDisplayExtrapolationMenu.DisplayExtrapolation) {
			ExtrapolationMenuPage = (int)enumDisplayExtrapolationMenu.DisplayExtrapolation;

			Label_VelocityPrediction.SetActive (true);
			Value_VelocityPrediction.SetActive (true);
			ButtonVelocityPredDown.SetActive (true);
			ButtonVelocityPredUp.SetActive (true);

			Label_SyncDistance.SetActive (false);
			Value_SyncDistance.SetActive (false);
			ButtonSyncDistanceDown.SetActive (false);
			ButtonSyncDistanceUp.SetActive (false);
		}
	}

	public void RefreshInterpolationOptions(){	

		if (InterpolationMenuPage != (int)enumDisplayInterpolationMenu.DisplayClientInterpolation) {
			InterpolationMenuPage = (int)enumDisplayInterpolationMenu.DisplayClientInterpolation;

			Label_ClientPrediction.SetActive (true);
			Value_ClientPrediction.SetActive (true);
			ButtonClientPredDown.SetActive (true);
			ButtonClientPredUp.SetActive (true);

			Label_EnemyPrediction.SetActive (false);
			Value_EnemyPrediction.SetActive (false);
			ButtonEnemyPredDown.SetActive (false);
			ButtonEnemyPredUp.SetActive (false);
		}			
	}



	public void ToggleInterpolationNext(){	

		if (InterpolationMenuPage != (int)enumDisplayInterpolationMenu.DisplayClientInterpolation) {
			InterpolationMenuPage = (int)enumDisplayInterpolationMenu.DisplayClientInterpolation;

			Label_ClientPrediction.SetActive (true);
			Value_ClientPrediction.SetActive (true);
			ButtonClientPredDown.SetActive (true);
			ButtonClientPredUp.SetActive (true);

			Label_EnemyPrediction.SetActive (false);
			Value_EnemyPrediction.SetActive (false);
			ButtonEnemyPredDown.SetActive (false);
			ButtonEnemyPredUp.SetActive (false);
		} else if (InterpolationMenuPage != (int)enumDisplayInterpolationMenu.DisplayEnemyInterpolation) {
			InterpolationMenuPage = (int)enumDisplayInterpolationMenu.DisplayEnemyInterpolation;

			Label_ClientPrediction.SetActive (false);
			Value_ClientPrediction.SetActive (false);
			ButtonClientPredDown.SetActive (false);
			ButtonClientPredUp.SetActive (false);

			Label_EnemyPrediction.SetActive (true);
			Value_EnemyPrediction.SetActive (true);
			ButtonEnemyPredDown.SetActive (true);
			ButtonEnemyPredUp.SetActive (true);
		}
	}

	public void ToggleInterpolationPrev(){	

		if (InterpolationMenuPage != (int)enumDisplayInterpolationMenu.DisplayEnemyInterpolation) {
			InterpolationMenuPage = (int)enumDisplayInterpolationMenu.DisplayEnemyInterpolation;

			Label_ClientPrediction.SetActive (false);
			Value_ClientPrediction.SetActive (false);
			ButtonClientPredDown.SetActive (false);
			ButtonClientPredUp.SetActive (false);

			Label_EnemyPrediction.SetActive (true);
			Value_EnemyPrediction.SetActive (true);
			ButtonEnemyPredDown.SetActive (true);
			ButtonEnemyPredUp.SetActive (true);
		} else if (InterpolationMenuPage != (int)enumDisplayInterpolationMenu.DisplayClientInterpolation) {
			InterpolationMenuPage = (int)enumDisplayInterpolationMenu.DisplayClientInterpolation;

			Label_ClientPrediction.SetActive (true);
			Value_ClientPrediction.SetActive (true);
			ButtonClientPredDown.SetActive (true);
			ButtonClientPredUp.SetActive (true);

			Label_EnemyPrediction.SetActive (false);
			Value_EnemyPrediction.SetActive (false);
			ButtonEnemyPredDown.SetActive (false);
			ButtonEnemyPredUp.SetActive (false);
		}
	}

	public void ToggleBotMenuNext(){
		if (BotMenuPage == (int)enumDisplayBotMenu.DisplayBotSpeed) {
			BotMenuPage = (int)enumDisplayBotMenu.DisplayBotMenuBlue;

			Value_BlueBotCount.SetActive (true);
			ButtonBlueBotDown.SetActive (true);
			ButtonBlueBotUp.SetActive (true);
			BotImageBlue.SetActive (true);

			Value_RedBotCount.SetActive (false);
			ButtonRedBotDown.SetActive (false);
			ButtonRedBotUp.SetActive (false);
			BotImageRed.SetActive (false);

			ValueBotSpawnDistance.SetActive (false);
			LabelBotSpawnDistance.SetActive (false);
			ButtonBotSpawnDistanceIncrease.SetActive (false);
			ButtonBotSpawnDistanceDecrease.SetActive (false);

			ValueBotSpeed.SetActive (false);
			LabelBotSpeed.SetActive (false);
			ButtonBotSpeedIncrease.SetActive (false);
			ButtonBotSpeedDecrease.SetActive (false);

		} else if (BotMenuPage == (int)enumDisplayBotMenu.DisplayBotMenuBlue) {
			BotMenuPage = (int)enumDisplayBotMenu.DisplayBotMenuRed;

			Value_BlueBotCount.SetActive (false);
			ButtonBlueBotDown.SetActive (false);
			ButtonBlueBotUp.SetActive (false);
			BotImageBlue.SetActive (false);

			Value_RedBotCount.SetActive (true);
			ButtonRedBotDown.SetActive (true);
			ButtonRedBotUp.SetActive (true);
			BotImageRed.SetActive (true);

			ValueBotSpawnDistance.SetActive (false);
			LabelBotSpawnDistance.SetActive (false);
			ButtonBotSpawnDistanceIncrease.SetActive (false);
			ButtonBotSpawnDistanceDecrease.SetActive (false);

			ValueBotSpeed.SetActive (false);
			LabelBotSpeed.SetActive (false);
			ButtonBotSpeedIncrease.SetActive (false);
			ButtonBotSpeedDecrease.SetActive (false);

		} else if (BotMenuPage == (int)enumDisplayBotMenu.DisplayBotMenuRed) {
			BotMenuPage = (int)enumDisplayBotMenu.DisplayBotSpawnDistance;

			Value_BlueBotCount.SetActive (false);
			ButtonBlueBotDown.SetActive (false);
			ButtonBlueBotUp.SetActive (false);
			BotImageBlue.SetActive (false);

			Value_RedBotCount.SetActive (false);
			ButtonRedBotDown.SetActive (false);
			ButtonRedBotUp.SetActive (false);
			BotImageRed.SetActive (false);

			ValueBotSpawnDistance.SetActive (true);
			LabelBotSpawnDistance.SetActive (true);
			ButtonBotSpawnDistanceIncrease.SetActive (true);
			ButtonBotSpawnDistanceDecrease.SetActive (true);

			ValueBotSpeed.SetActive (false);
			LabelBotSpeed.SetActive (false);
			ButtonBotSpeedIncrease.SetActive (false);
			ButtonBotSpeedDecrease.SetActive (false);

		} else if (BotMenuPage == (int)enumDisplayBotMenu.DisplayBotSpawnDistance) {
			BotMenuPage = (int)enumDisplayBotMenu.DisplayBotSpeed;

			Value_BlueBotCount.SetActive (false);
			ButtonBlueBotDown.SetActive (false);
			ButtonBlueBotUp.SetActive (false);
			BotImageBlue.SetActive (false);

			Value_RedBotCount.SetActive (false);
			ButtonRedBotDown.SetActive (false);
			ButtonRedBotUp.SetActive (false);
			BotImageRed.SetActive (false);

			ValueBotSpawnDistance.SetActive (false);
			LabelBotSpawnDistance.SetActive (false);
			ButtonBotSpawnDistanceIncrease.SetActive (false);
			ButtonBotSpawnDistanceDecrease.SetActive (false);

			ValueBotSpeed.SetActive (true);
			LabelBotSpeed.SetActive (true);
			ButtonBotSpeedIncrease.SetActive (true);
			ButtonBotSpeedDecrease.SetActive (true);
		}
	}

	public void ToggleBotMenuPrev(){
		if (BotMenuPage == (int)enumDisplayBotMenu.DisplayBotSpeed) {
			BotMenuPage = (int)enumDisplayBotMenu.DisplayBotSpawnDistance;

			Value_BlueBotCount.SetActive (false);
			ButtonBlueBotDown.SetActive (false);
			ButtonBlueBotUp.SetActive (false);
			BotImageBlue.SetActive (false);

			Value_RedBotCount.SetActive (false);
			ButtonRedBotDown.SetActive (false);
			ButtonRedBotUp.SetActive (false);
			BotImageRed.SetActive (false);

			ValueBotSpawnDistance.SetActive (true);
			LabelBotSpawnDistance.SetActive (true);
			ButtonBotSpawnDistanceIncrease.SetActive (true);
			ButtonBotSpawnDistanceDecrease.SetActive (true);

			ValueBotSpeed.SetActive (false);
			LabelBotSpeed.SetActive (false);
			ButtonBotSpeedIncrease.SetActive (false);
			ButtonBotSpeedDecrease.SetActive (false);
		}
		else if (BotMenuPage == (int)enumDisplayBotMenu.DisplayBotSpawnDistance) {
			BotMenuPage = (int)enumDisplayBotMenu.DisplayBotMenuRed;

			Value_BlueBotCount.SetActive (false);
			ButtonBlueBotDown.SetActive (false);
			ButtonBlueBotUp.SetActive (false);
			BotImageBlue.SetActive (false);

			Value_RedBotCount.SetActive (true);
			ButtonRedBotDown.SetActive (true);
			ButtonRedBotUp.SetActive (true);
			BotImageRed.SetActive (true);

			ValueBotSpawnDistance.SetActive (false);
			LabelBotSpawnDistance.SetActive (false);
			ButtonBotSpawnDistanceIncrease.SetActive (false);
			ButtonBotSpawnDistanceDecrease.SetActive (false);

			ValueBotSpeed.SetActive (false);
			LabelBotSpeed.SetActive (false);
			ButtonBotSpeedIncrease.SetActive (false);
			ButtonBotSpeedDecrease.SetActive (false);

		} else if (BotMenuPage == (int)enumDisplayBotMenu.DisplayBotMenuRed) {
			BotMenuPage = (int)enumDisplayBotMenu.DisplayBotMenuBlue;

			Value_BlueBotCount.SetActive (true);
			ButtonBlueBotDown.SetActive (true);
			ButtonBlueBotUp.SetActive (true);
			BotImageBlue.SetActive (true);

			Value_RedBotCount.SetActive (false);
			ButtonRedBotDown.SetActive (false);
			ButtonRedBotUp.SetActive (false);
			BotImageRed.SetActive (false);

			ValueBotSpawnDistance.SetActive (false);
			LabelBotSpawnDistance.SetActive (false);
			ButtonBotSpawnDistanceIncrease.SetActive (false);
			ButtonBotSpawnDistanceDecrease.SetActive (false);

			ValueBotSpeed.SetActive (false);
			LabelBotSpeed.SetActive (false);
			ButtonBotSpeedIncrease.SetActive (false);
			ButtonBotSpeedDecrease.SetActive (false);

		} else if (BotMenuPage == (int)enumDisplayBotMenu.DisplayBotMenuBlue) {
			BotMenuPage = (int)enumDisplayBotMenu.DisplayBotSpeed;

			Value_BlueBotCount.SetActive (false);
			ButtonBlueBotDown.SetActive (false);
			ButtonBlueBotUp.SetActive (false);
			BotImageBlue.SetActive (false);

			Value_RedBotCount.SetActive (false);
			ButtonRedBotDown.SetActive (false);
			ButtonRedBotUp.SetActive (false);
			BotImageRed.SetActive (false);

			ValueBotSpawnDistance.SetActive (false);
			LabelBotSpawnDistance.SetActive (false);
			ButtonBotSpawnDistanceIncrease.SetActive (false);
			ButtonBotSpawnDistanceDecrease.SetActive (false);

			ValueBotSpeed.SetActive (true);
			LabelBotSpeed.SetActive (true);
			ButtonBotSpeedIncrease.SetActive (true);
			ButtonBotSpeedDecrease.SetActive (true);

		}
	}

	public void BottomMenuNext(){

		//SET TO MENU1

		//WRAP AROUND: MENU2 BACK TO MENU1
		if (MenuPage == (int)enumBottomMenuPage.Menu2) {
			MenuPage = (int)enumBottomMenuPage.Menu1;
			SpacingA_2.SetActive (true);
			Button_KeyInput_2.SetActive (true);
			Button_Audio_2.SetActive (true);
			Button_ChatDisplay_2.SetActive (true);
			SpacingB_2.SetActive (true);

			Button_Bots_3.SetActive (false);
			Button_Interpolation_3.SetActive (false);
			Button_Extrapolation_3.SetActive (false);
			Button_SupportMe_3.SetActive (false);

		}

		//SET TO MENU2 (MENU 1 -> MENU 2)
		else if (MenuPage == (int)enumBottomMenuPage.Menu1) {
			MenuPage = (int)enumBottomMenuPage.Menu2;
			SpacingA_2.SetActive (false);
			Button_KeyInput_2.SetActive (false);
			Button_Audio_2.SetActive (false);
			Button_ChatDisplay_2.SetActive (false);
			SpacingB_2.SetActive (false);

			Button_Bots_3.SetActive (true);
			Button_Interpolation_3.SetActive (true);
			Button_Extrapolation_3.SetActive (true);
			Button_SupportMe_3.SetActive (true);
		}

	}

	public void BottomMenuPrev(){

		//SET TO MENU2

		//WRAP AROUND: MENU1 BACK TO MENU2
		if (MenuPage == (int)enumBottomMenuPage.Menu1) {
			MenuPage = (int)enumBottomMenuPage.Menu2;
			SpacingA_2.SetActive (false);
			Button_KeyInput_2.SetActive (false);
			Button_Audio_2.SetActive (false);
			Button_ChatDisplay_2.SetActive (false);
			SpacingB_2.SetActive (false);

			Button_Bots_3.SetActive (true);
			Button_Interpolation_3.SetActive (true);
			Button_Extrapolation_3.SetActive (true);
			Button_SupportMe_3.SetActive (true);
		}

		//SET TO MENU1 (MENU 2 -> MENU 1)
		else if (MenuPage == (int)enumBottomMenuPage.Menu2) {
			MenuPage = (int)enumBottomMenuPage.Menu1;
			SpacingA_2.SetActive (true);
			Button_KeyInput_2.SetActive (true);
			Button_Audio_2.SetActive (true);
			Button_ChatDisplay_2.SetActive (true);
			SpacingB_2.SetActive (true);

			Button_Bots_3.SetActive (false);
			Button_Interpolation_3.SetActive (false);
			Button_Extrapolation_3.SetActive (false);
			Button_SupportMe_3.SetActive (false);

		}
			
	}

}
