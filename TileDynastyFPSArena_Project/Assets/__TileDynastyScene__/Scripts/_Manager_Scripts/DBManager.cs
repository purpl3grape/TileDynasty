using UnityEngine;
using System.Collections;
//using System;
//using System.Data;
//using Mono.Data.Sqlite;

public class DBManager : MonoBehaviour {

//	private string connectionString;
//	private bool _userNameFound = false;
//
//
//
//	 //Use this for initialization
//	void Start () {
//		connectionString = "URI=file:" + Application.dataPath + "/HighScoreDB.sqlite";
//
//		CreateTable ();
//	}
//	
//	 //Update is called once per frame
//	void Update () {
//	
//	}
//
//	private void CreateTable(){
//		using (IDbConnection dbConnection = new SqliteConnection(connectionString)) {
//			dbConnection.Open();
//			
//			using(IDbCommand dbCmd = dbConnection.CreateCommand()){
//				string sqlQuery = String.Format("CREATE TABLE if not exists PlayerInfo (PlayerID STRING PRIMARY KEY  NOT NULL  DEFAULT (null) ,Name STRING NOT NULL ,Score INTEGER NOT NULL ,Date DATETIME NOT NULL  DEFAULT (CURRENT_DATE))");
//				dbCmd.CommandText = sqlQuery;
//				dbCmd.ExecuteScalar();	
//				dbConnection.Close();
//
//				
//			}
//		}
//
//		AddDefaultPlayerNames ("Pete");
//		AddDefaultPlayerNames ("Red");
//		AddDefaultPlayerNames ("Blue");
//	}
//
//	private void AddDefaultPlayerNames(string name){
//		using (IDbConnection dbConnection = new SqliteConnection(connectionString)) {
//			dbConnection.Open();
//			
//			using(IDbCommand dbCmd = dbConnection.CreateCommand()){
//				string sqlQuery = String.Format("INSERT INTO PlayerInfo Name Values (\"{0}\")", name);
//				dbCmd.CommandText = sqlQuery;
//				dbCmd.ExecuteScalar();	
//				dbConnection.Close();
//			}
//		}
//	}
//
//
//	public bool CheckUserName(string attemptUserName){
//
//		bool isUserNameFound = false;
//
//		using (IDbConnection dbConnection = new SqliteConnection(connectionString)) {
//			dbConnection.Open();
//			
//			using(IDbCommand dbCmd = dbConnection.CreateCommand()){
//				string sqlQuery = "SELECT * FROM PlayerInfo";
//				
//				dbCmd.CommandText = sqlQuery;
//
//				String userName = "";
//
//				using (IDataReader reader = dbCmd.ExecuteReader()){
//					while(reader.Read()){
//						userName = reader.GetString(1);
//						if(attemptUserName == userName){
//							_userNameFound = true;
//						}
//					}
//					
//					dbConnection.Close();
//					reader.Close();
//				}
//			}
//		}
//		return _userNameFound;
//	}
//
//	private void GetScores(){
//		using (IDbConnection dbConnection = new SqliteConnection(connectionString)) {
//			dbConnection.Open();
//
//			using(IDbCommand dbCmd = dbConnection.CreateCommand()){
//				string sqlQuery = "SELECT * FROM PlayerInfo";
//
//				dbCmd.CommandText = sqlQuery;
//
//				using (IDataReader reader = dbCmd.ExecuteReader()){
//					while(reader.Read()){
//						Debug.Log(reader.GetString(1));
//					}
//
//					dbConnection.Close();
//					reader.Close();
//				}
//			}
//		}
//	}
}
