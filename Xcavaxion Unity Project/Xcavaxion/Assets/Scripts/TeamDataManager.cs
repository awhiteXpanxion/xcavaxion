using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;

public class TeamDataManager : MonoBehaviour {

	public int numberOfPlayers;
	public int numberOfTeams;

	private string[] possibleColors = { "red", "orange", "green", "blue" };
	public List<string> currentColors; 

	public string[] CPUPlayerNames = 
	{
		"CPU 1", "CPU 2", "CPU 3", "CPU 4", "CPU 5", "CPU 6", "CPU 7",
		"CPU 8", "CPU 9", "CPU 10", "CPU 11", "CPU 12", "CPU 13", "CPU 14", "CPU 15", "CPU 16"
	};

	public string blueText = "SPACE MINING ADMINISTRATION";
	public string greenText = "COLONIAL MINING FEDERATION";
	public string redText = "STAR STUFF INC.";
	public string orangeText = "GENERAL RESOURCES LTD.";

	public GameObject blueLogo;
	public GameObject greenLogo;
	public GameObject redLogo;
	public GameObject orangeLogo;

	public GameObject blueRobotImage;
	public GameObject greenRobotImage;
	public GameObject redRobotImage;
	public GameObject orangeRobotImage;

	public Color orangeTeamColor = new Color(0.98f, 0.59f, 0.04f, 1.0f);

	// Use this for initialization
	void Start () {
		currentColors = new List<string> ();
		LoadColors ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LoadColors(){
		foreach(string teamColor in possibleColors){
			currentColors.Add (teamColor);
		}
	}
}
