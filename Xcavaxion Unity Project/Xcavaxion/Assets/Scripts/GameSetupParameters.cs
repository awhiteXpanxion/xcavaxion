using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameSetupParameters : MonoBehaviour {

	public string terrainType;

	public bool boulders; //whether or not to use boulders in the game or the terrain tiles

	public int mapSizeX;
	public int mapSizeY;
	public int elementFrequency; //how common it is to find elements
	public int numberOfTeams;
	public int numberOfPlayers; //try to keep an equal number of players per team

	public List<PlayerData> playerInformation; //each "PlayerData" represents a player that will be in the game
	public List<string> listOfTeams; //at most 4

	public string playerName;
	public string playerTeamChoice;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);

		playerInformation = new List<PlayerData> ();
		listOfTeams = new List<string> ();

		//listOfTeams = GetListOfTeams ();
		//numberOfTeams = GetTeamCount ();

//		GameObject lengthInput;
//		lengthInput = GameObject.Find ("LengthInput");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetNumberOfPlayers(){
		return playerInformation.Count;
	}

	public List<string> GetListOfTeams(){
		List<string> foundTeams = new List<string>();

		foreach(PlayerData playerInfo in playerInformation){
			string currentTeam = playerInfo.playerTeamColor;
			if(foundTeams.Count == 0){
				foundTeams.Add (currentTeam);
			}
			else{
				bool found = false;
				foreach(string teamColor in foundTeams){
					if(currentTeam.Equals(teamColor)){
						found = true;
					}
				}
				if(!found){
					foundTeams.Add (currentTeam);
				}
			}
		}
		return foundTeams;
	}

	public int GetTeamCount(){
		return listOfTeams.Count;
	}		
}