using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPlayerDisplay : MonoBehaviour {

	public bool playerListAltered; //if this is true, update the teamPlayers list

	public string currentPlayerListString;

	public List<PlayerData> teamPlayers; //the list that will be outputed in the text box, there may not even be any reason for this

	public Stack<PlayerData> cpuPlayers;
	public List<PlayerData> humanPlayers;

	public TeamPanelController teamPanelReference;

	// Use this for initialization
	void Start () {
		playerListAltered = false;

		currentPlayerListString = "";

		teamPlayers = new List<PlayerData> ();
		cpuPlayers = new Stack<PlayerData> ();
		humanPlayers = new List<PlayerData> ();

	}
	
	// Update is called once per frame
	void Update () {

		if(playerListAltered){
			//make a new list string
			currentPlayerListString = MakePlayerListString();
			//update the onscreen list
			gameObject.GetComponent<Text> ().text = currentPlayerListString; //maybe?
			playerListAltered = false;
		}
		
	}

	//construct a string with line breaks to display in the onscreen text box
	public string MakePlayerListString(){
		string playerList = "";

		//put the CPU players first
		foreach(PlayerData cpuPlayer in cpuPlayers){
			playerList += cpuPlayer.playerName + "\n";
		}

		//then the human players
		foreach(PlayerData humanPlayer in humanPlayers){
			playerList += humanPlayer.playerName + "\n";
		}

		return playerList;
	}

	//how to determine a CPU player from a human player
	//human player difficulty value is 0
	public void AddPlayerToTeam(PlayerData playerToAdd){
		//check for player already present?
		//adding a human player
		if(playerToAdd.cpuPlayerDifficulty == 0){
			humanPlayers.Add (playerToAdd);
		}
		//adding a CPU player
		else{
			cpuPlayers.Push (playerToAdd);
		}

		playerListAltered = true;
	}

	//want to be able to remove the last CPU player added to the team
	//and want to be able to remove a certain human player from the team
	//if player not there return null
	public PlayerData RemovePlayerFromTeam(string playerName){
		//check for player not there? yes, with the network stuff who knows
		int playerIndex = CheckForHumanPlayerPresent(playerName);
		if(playerIndex == -1){
			return null;
		}
		playerListAltered = true;
		return humanPlayers.RemoveAt(playerIndex);
	}

	public PlayerData RemoveCPUPlayerFromTeam(){
		PlayerData cpuToRemove = cpuPlayers.Pop ();
		playerListAltered = true;
		return cpuToRemove;
	}

	//returns index of human player in human player array
	//return -1 if player not found
	public int CheckForHumanPlayerPresent(string playerName){
		int returnVal = -1;
		for(int i = 0; i < humanPlayers.Count; i++){
			if(humanPlayers[i].playerName.Equals(playerName)){
				returnVal = i;
			}
		}
		return returnVal;
	}
		
}
