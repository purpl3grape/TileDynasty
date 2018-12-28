using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyInput : MonoBehaviour {


	//NEW STUFF
	public GameObject Text_LobbyOption;
	public GameObject Image_LobbyOption;
	public Sprite[] LobbyOptionImages;
	public GameObject Image_LeftArrow;
	public GameObject Image_RightArrow;
	public Color[] LobbySelectColor;

	//FOR DISPLAY CURRENT ROOM
	public GameObject CurrentRoomName;
	public GameObject CurrentRoomMapText;
	public GameObject CurrentRoomMapImage;
	public GameObject CurrentRoomMode;
	public GameObject CurrentRoomMasterClient;
	public GameObject CurrentRoomPlayerCount;
	public GameObject CurrentRoomRegion;
	public GameObject CurrentRoomButtonNext;
	public GameObject CurrentRoomButtonPrev;
	public GameObject CurrentRoomViewIndex;
	public GameObject TotalRoomCount;

	//FOR CREATE ROOM FUNCTION
	public GameObject Create_Div;
	public Text Create_RoomSizeCount;
	public Text Create_GameTimeCount;
	public GameObject Create_RoomsizeSlider;
	public GameObject Create_TimeSlider;
	public GameObject Create_GameModeDropDownInput;
	public GameObject Create_MapNameDropDownInput;
	public GameObject Create_GameRoomInput;
	public GameObject Create_GameRoomLabel;
	public GameObject Create_ButtonCancel;
	public GameObject Create_ButtonCreateRoom;
	public GameObject Create_ButtonCreateLocalRoom;	//LOCAL CONNECTION ONLY
	public GameObject Create_ImageMapPreview;

	//Gameobjects for HIDDEN Rooms
	public GameObject Hidden_Div;
	public GameObject Hidden_LabelHiddenRoomName;
	public GameObject Hidden_InputRoomName;
	public GameObject Hidden_ButtonJoin;

	public Sprite[] mapSpriteList;

	//GameObjects for Main Statistics
	public GameObject Stat_Kills;
	public GameObject Stat_Deaths;
	public GameObject Stat_DamageDealt;
	public GameObject Stat_DamageReceived;
	public GameObject Stat_Killstreak;
	public GameObject Stat_Assists;
	public GameObject Stat_GamesPlayed;

	//DISPLAY NUMBER PLAYERS ONLINE
	public Text TotalPlayerCount;

	//Lobby Room Listing Header
	public GameObject Lobby_Div;
	public GameObject Lobby_RoomName;
	public GameObject Lobby_Map;
	public GameObject Lobby_GameMode;
	public GameObject Lobby_MasterClient;
	public GameObject Lobby_Region;
	public GameObject Lobby_RoomCapCount;
	public GameObject Lobby_NoRoomsMessage;

	public GameObject Menu_ButtonPlayerStats;
	public GameObject Menu_ButtonHomeBase;

	//CreateMenu
	public GameObject Menu_ButtonCreateNew;
	public GameObject Menu_ButtonCampaign;

	//QuitFromLobby
	public GameObject Button_QuitFromLobby;

	//Region Info
	public GameObject RegionText;

	//Title
	public GameObject Lobby_VersionText;

}
