using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class StatsManager : MonoBehaviour {

	private int statMissionAttempts;
	private int statMissionSuccess;
	private float statMissionSuccessPercent;
	private int statMissionFailure;
	private float statMissionFailurePercent;

	private int statWins;
	private float statWinPercent;
	private int statLosses;
	private float statLossPercent;
	private int statDraws;
	private float statDrawPercent;
	private int statGamesPlayed;

	private int statTotalFired;
	private int statTotalHit;
	private float statTotalAcc;
	private int statHMGFired;
	private int statHMGHit;
	private float statHMGAcc;
	private int statRocketFired;
	private int statRocketHit;
	private float statRocketAcc;
	private int statRailFired;
	private int statRailHit;
	private float statRailAcc;
	private int statEMPFired;
	private int statEMPHit;
	private float statEMPAcc;

	private int statHeadShotHits;
	private int statBullsEyeHits;
	private int statMeleeHits;
	private int statKills;
	private int statDeaths;
	private int statHostagesRescued;
	private int statDamageD;
	private int statDamageR;
	private int statBestKillStreak;

	public GameObject MissionAttempts;
	public GameObject MissionSuccess;
	public GameObject MissionSuccessPercent;
	public GameObject MissionFailure;
	public GameObject MissionFailurePercent;
	public GameObject Wins;
	public GameObject WinPercent;
	public GameObject Losses;
	public GameObject LossesPercent;
	public GameObject Draws;
	public GameObject DrawsPercent;
	public GameObject GamesPlayed;
	public GameObject TotalFired;
	public GameObject TotalHit;
	public GameObject TotalAcc;
	public GameObject HMGFired;
	public GameObject HMGHit;
	public GameObject HMGAcc;
	public GameObject RocketFired;
	public GameObject RocketHit;
	public GameObject RocketAcc;
	public GameObject RailFired;
	public GameObject RailHit;
	public GameObject RailAcc;
	public GameObject EMPFired;
	public GameObject EMPHit;
	public GameObject EMPAcc;
	public GameObject HeadShotHits;
	public GameObject BullsEyeHits;
	public GameObject MeleeHits;
	public GameObject Kills;
	public GameObject Deaths;
	public GameObject Rescues;
	public GameObject DamageD;
	public GameObject DamageR;
	public GameObject BestKillStreak;
	public GameObject ResetRegion;
	public GameObject RegionPreviousArrow;
	public GameObject RegionNextArrow;
	public GameObject Region;

	public GameObject MultiplayerMap;
	public GameObject MultiplayerMap_Text;
	public GameObject MultiplayerMap_Image;
	public GameObject MultiplayerMode;
	public GameObject MultiplayerMode_Text;
	public GameObject MultiplayerPlayers;
	public GameObject MultiplayerPlayers_Text;
	public GameObject MultiplayerTime;
	public GameObject MultiplayerTime_Text;
	public GameObject OptionNext;
	public GameObject OptionBack;
	public GameObject ChoiceNext;
	public GameObject ChoiceBack;
	public GameObject CheckMark;
	public GameObject Selection;
	public GameObject CurrentOption;
	public GameObject SelectedOption;
	public GameObject CheckMark2;

	public GameObject Menu_Text;
	public GameObject CampaignMode_Text;
	public GameObject CampaignMode_Next;
	public GameObject CampaignMode_Back;

	void Start () {

		if (Steamworks.SteamUserStats.GetStat ("ResuceMissionSuccess", out statMissionSuccess))
			MissionSuccess.GetComponent<Text> ().text = statMissionSuccess.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("ResuceMissionFailure", out statMissionFailure))
			MissionFailure.GetComponent<Text> ().text = statMissionFailure.ToString ();
		statMissionAttempts = statMissionFailure + statMissionSuccess;
//		Mathf.RoundToInt (((float)statDraws / (float)statGamesPlayed) * 100);
		if (statMissionAttempts > 0) {
			statMissionFailurePercent = Mathf.RoundToInt (((float)statMissionFailure / (float)statMissionAttempts) * 100);
			statMissionSuccessPercent = Mathf.RoundToInt (((float)statMissionSuccess / (float)statMissionAttempts) * 100);
		}
		MissionSuccess.GetComponent<Text> ().text = statMissionSuccess.ToString ();
		MissionFailure.GetComponent<Text> ().text = statMissionFailure.ToString ();
		MissionAttempts.GetComponent<Text> ().text = (statMissionSuccess + statMissionFailure).ToString ();
		MissionSuccessPercent.GetComponent<Text> ().text = statMissionSuccessPercent.ToString ();
		MissionFailurePercent.GetComponent<Text> ().text = statMissionFailurePercent.ToString ();

		if (Steamworks.SteamUserStats.GetStat ("Wins", out statWins))
			Wins.GetComponent<Text> ().text = statWins.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("Losses", out statLosses))
			Losses.GetComponent<Text> ().text = statLosses.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("Draws", out statDraws))
			Draws.GetComponent<Text> ().text = statDraws.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("GamesPlayed", out statGamesPlayed))
			GamesPlayed.GetComponent<Text> ().text = statGamesPlayed.ToString ();

		if (Steamworks.SteamUserStats.GetStat ("Wins", out statWins) && Steamworks.SteamUserStats.GetStat ("GamesPlayed", out statGamesPlayed)) {
			statWinPercent = statGamesPlayed == 0 ? 0 : Mathf.RoundToInt (((float)statWins / (float)statGamesPlayed) * 100);
			WinPercent.GetComponent<Text> ().text = statWinPercent.ToString ();
//			Debug.Log ("Working1");
		}
		if (Steamworks.SteamUserStats.GetStat ("Losses", out statLosses) && Steamworks.SteamUserStats.GetStat ("GamesPlayed", out statGamesPlayed)) {
			statLossPercent = statGamesPlayed == 0 ? 0 : Mathf.RoundToInt (((float)statLosses / (float)statGamesPlayed) * 100);
			LossesPercent.GetComponent<Text> ().text = statLossPercent.ToString ();
//			Debug.Log ("Working2");
		}
		if (Steamworks.SteamUserStats.GetStat ("Draws", out statDraws) && Steamworks.SteamUserStats.GetStat ("GamesPlayed", out statGamesPlayed)) {
			statDrawPercent = statGamesPlayed == 0 ? 0 : Mathf.RoundToInt (((float)statDraws / (float)statGamesPlayed) * 100);
			DrawsPercent.GetComponent<Text> ().text = statDrawPercent.ToString ();
//			Debug.Log ("Working3");
		}
			

		//ACCURACY STATS
		if (Steamworks.SteamUserStats.GetStat ("ShotsFired", out statTotalFired))
			TotalFired.GetComponent<Text> ().text = statTotalFired.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("ShotsFiredHMG", out statHMGFired))
			HMGFired.GetComponent<Text> ().text = statHMGFired.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("ShotsFiredRocket", out statRocketFired))
			RocketFired.GetComponent<Text> ().text = statRocketFired.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("ShotsFiredRail", out statRailFired))
			RailFired.GetComponent<Text> ().text = statRailFired.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("ShotsFiredEMP", out statEMPFired))
			EMPFired.GetComponent<Text> ().text = statEMPFired.ToString ();

		if (Steamworks.SteamUserStats.GetStat ("ShotsHit", out statTotalHit))
			TotalHit.GetComponent<Text> ().text = statTotalHit.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("ShotsHitHMG", out statHMGHit))
			HMGHit.GetComponent<Text> ().text = statHMGHit.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("ShotsHitRocket", out statRocketHit))
			RocketHit.GetComponent<Text> ().text = statRocketHit.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("ShotsHitRail", out statRailHit))
			RailHit.GetComponent<Text> ().text = statRailHit.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("ShotsHitEMP", out statEMPHit))
			EMPHit.GetComponent<Text> ().text = statEMPHit.ToString ();

		if (Steamworks.SteamUserStats.GetStat ("Accuracy", out statTotalAcc))
			TotalAcc.GetComponent<Text> ().text = ((int)(statTotalAcc * 100)).ToString () + " %";		
		if (Steamworks.SteamUserStats.GetStat ("AccuracyHMG", out statHMGAcc))
			HMGAcc.GetComponent<Text> ().text = ((int)(statHMGAcc * 100)).ToString () + " %";
		if (Steamworks.SteamUserStats.GetStat ("AccuracyRocket", out statRocketAcc))
			RocketAcc.GetComponent<Text> ().text = ((int)(statRocketAcc * 100)).ToString () + " %";
		if (Steamworks.SteamUserStats.GetStat ("AccuracyRail", out statRailAcc))
			RailAcc.GetComponent<Text> ().text = ((int)(statRailAcc * 100)).ToString () + " %";
		if (Steamworks.SteamUserStats.GetStat ("AccuracyEMP", out statEMPAcc))
			EMPAcc.GetComponent<Text> ().text = ((int)(statEMPAcc * 100)).ToString () + " %";
		
		//Player Statistics
		if (Steamworks.SteamUserStats.GetStat ("Kills", out statKills))
			Kills.GetComponent<Text> ().text = statKills.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("Deaths", out statDeaths))
			Deaths.GetComponent<Text> ().text = statDeaths.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("HostagesRescued", out statHostagesRescued))
			Rescues.GetComponent<Text> ().text = statHostagesRescued.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("DamageDealt", out statDamageD))
			DamageD.GetComponent<Text> ().text = statDamageD.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("DamageReceived", out statDamageR))
			DamageR.GetComponent<Text> ().text = statDamageR.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("BestKillStreak", out statBestKillStreak))
			BestKillStreak.GetComponent<Text> ().text = statBestKillStreak.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("MeleeHits", out statMeleeHits))
			MeleeHits.GetComponent<Text> ().text = statMeleeHits.ToString ();
		if (Steamworks.SteamUserStats.GetStat ("BullsEyeHits", out statBullsEyeHits))
			BullsEyeHits.GetComponent<Text> ().text = statBullsEyeHits.ToString ();		
		if (Steamworks.SteamUserStats.GetStat ("HeadShotHits", out statHeadShotHits))
			HeadShotHits.GetComponent<Text> ().text = statHeadShotHits.ToString ();


		if (PhotonNetwork.room != null) { 
			Region.GetComponent<Text> ().text = PhotonNetwork.room.CustomProperties [RoomProperties.Region].ToString ();
		}
	}

}
