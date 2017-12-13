using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectionScreenManager : MonoBehaviour {

	public bool activateStartGameButton;

	public int totalNumberOfPlayers; //the number of players human and cpu, max 4 per team, max of 4 teams
	public int numberOfHumanPlayers;
	public int numberOfCPUPlayers;

	private int CPUPlayerMax = 16;

	public int numberOfTeams; //max of 4 teams

	public Text numTeamsText;
	public Text numPlayersText;

	public Button startGameButton;

	public List<PlayerData> allPlayers;
	public List<string> allTeamNames;

	public Stack<string> availableCPUPlayers;

	//used to determine what panels are where
	public GameObject[] teamPanels; //there should only every be four of these
	public GameObject[] addTeamButtons;

	public GameObject teamPanelPrefab;
	//public GameObject addTeamButtonReference;

	//references to the positions on screen where the selection screen team panels go
	public GameObject[] panelPositions;

	public Canvas canvasReference;

	public TeamDataManager teamDataReference;

	public GameObject playerNameInput;
	public string currentPlayerName;
	public string playerDefaultName = "PLAYER";

	public PlayerData thePlayer;

	//team panels are created by the "AddTeam" button. Only one button is on screen at a time, always to right of the last teamPanel, except for the last one.
	//team panel defaults to the first remaining team color? Right now it defaults to nothing.
	//how to close team panels?
	//which ones can be open? middle two, only first two? last two? last three? does it matter?

	// Use this for initialization
	void Start () {
		totalNumberOfPlayers = 0;

		numberOfHumanPlayers = 0;
		numberOfCPUPlayers = 0;

		allPlayers = new List<PlayerData> ();
		allTeamNames = new List<string> ();

		numberOfTeams = 0;

		var tempData = GameObject.FindGameObjectWithTag ("TeamData");
		teamDataReference = tempData.GetComponent<TeamDataManager> ();

		availableCPUPlayers = new Stack<string> ();

		LoadCPUPlayers ();

		activateStartGameButton = false;
		startGameButton.gameObject.SetActive (activateStartGameButton); //set the start button inactive until there are players chosen

		thePlayer = new PlayerData ("", null, 1000, 0);//empty string name, do assign until on a team, null team color when not on a team, 0 difficulty for non cpu player
	}
	
	// Update is called once per frame
	void Update () {
		totalNumberOfPlayers = numberOfCPUPlayers + numberOfHumanPlayers;
		numTeamsText.text = numberOfTeams.ToString();
		numPlayersText.text = totalNumberOfPlayers.ToString();

		if(totalNumberOfPlayers > 1 && numberOfTeams > 1){
			activateStartGameButton = true;
		}
		else{
			activateStartGameButton = false;
		}
		startGameButton.gameObject.SetActive (activateStartGameButton); //hopefully continually activating and deactivating doesn't cause issues
	}

	void LoadCPUPlayers(){
		int length = teamDataReference.CPUPlayerNames.Length;
		for(int i = length - 1; i > -1; i--){
			availableCPUPlayers.Push (teamDataReference.CPUPlayerNames [i]);
		}
	}

	void FillTeamPanelList(){
		teamPanels = new GameObject[4];
		for(int i = 0; i < 4; i++){
			teamPanels [i] = null;
		}
	}

	public int GetButtonIndex(GameObject addTeamButton){
		int index = 0;
		foreach(GameObject addButton in addTeamButtons){
			if(addButton.Equals(addTeamButton)){
				return index;
			}
			index++;
		}
		return -1; //no match found
	}

	void RemoveExtraneousAddButtons(){
		int index = 0;
		foreach(GameObject addButton in addTeamButtons){
			if(index != numberOfTeams){
				addTeamButtons [index].SetActive (false);
			}
			index++;
		}
	}

	//load scene here and store all game setup information
	public void StartGame(){
		GameObject setupData = GameObject.FindGameObjectWithTag("SetupData");
		GameSetupParameters parameters = setupData.GetComponent<GameSetupParameters> ();
		GatherTeamPlayerData ();

		parameters.numberOfTeams = numberOfTeams;
		parameters.numberOfPlayers = totalNumberOfPlayers;
		parameters.playerInformation = allPlayers;
		parameters.listOfTeams = allTeamNames;

		SceneManager.LoadScene (2);
	}

	public void GatherTeamPlayerData(){
		GameObject[] currentTeamPanels = teamPanels;
		string currentTeamName = "";
		//grab all players from each team panel to put in game param data
		foreach(GameObject teamPanel in currentTeamPanels){
			//if the panel in question is active, get information from it, otherwise ignore it
			if (teamPanel.activeSelf == true){
				TeamPanelController teamPanelReference = teamPanel.GetComponent<TeamPanelController> ();
				print ("team player count: " + teamPanelReference.teamPlayers);
				allPlayers.AddRange (teamPanelReference.teamPlayers);
				currentTeamName = teamPanelReference.teamName.text;
				//if the team name has the placeholder text "COMPANY NAME" ignore it
				if(!currentTeamName.Equals("COMPANY NAME")){
					allTeamNames.Add (currentTeamName);
				}
			}
		}
	}

	//default player name will be PLAYER #, # being the number of human players in the game, only determined after added to a team
	public void PlayerNameEntry(string enteredPlayerName){
		currentPlayerName = enteredPlayerName;
	}

}
