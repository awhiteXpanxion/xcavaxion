using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

	//This class is used to represent chosen player data to load in to a game from the main menu

	public string playerName;
	public string playerTeamColor;

	public int playerMaxElementVol;
	public int cpuPlayerDifficulty; //set to zero for player characters

	public PlayerData(string name, string teamColor, int maxElementVol, int cpuDifficulty){
		playerName = name;
		playerTeamColor = teamColor;
		playerMaxElementVol = maxElementVol;
		cpuPlayerDifficulty = cpuDifficulty;
	}

}