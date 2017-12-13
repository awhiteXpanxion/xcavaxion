using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;

public class TeamPanelController : MonoBehaviour {

	public int panelNum;

	public Canvas canvasReference;

	public string RED = "red";
	public string BLUE = "blue";
	public string GREEN = "green";
	public string ORANGE = "orange";

	public string teamColor;
	public string playerToRemove; //if given a name by the SelectionScreenController, remove said player from player list

	public bool colorChanged;
	public bool firstActivated;
	public bool firstAwakened;

	public bool humanPlayerAddedToTeam;

	public Text teamName;
	public Text playerInfoText;

	public Button colorPickButton;

	public TeamDataManager teamData;

	public GameObject teamLogoSpot;
	public GameObject currentTeamLogo;
	public GameObject teamRobotSpot;
	public GameObject currentTeamRobot;
	public GameObject offScreenPosition;

	public Stack<PlayerData> teamPlayers;

	public SelectionScreenManager managerReference;

	public PanelPlayerDisplay playerDisplay;

	// Use this for initialization
	void Start () {
		//start is called on the first run of the game object, and when the script is enabled, once and never again

	}

	void Awake(){
		gameObject.SetActive (false);

		//awake is called immediately when the object is created, once and never again
		print(gameObject.name + "has awakened!");
		firstAwakened = true;
		firstActivated = false;
		teamData = null;
		colorChanged = false;
		teamColor = null;
		playerToRemove = null;
		currentTeamLogo = null;
		currentTeamRobot = null;

		humanPlayerAddedToTeam = false;

		var tempTeamData = GameObject.FindGameObjectWithTag ("TeamData");
		teamData = tempTeamData.GetComponent<TeamDataManager> ();

		playerDisplay = playerInfoText.GetComponent<PanelPlayerDisplay> ();

		//clear player info text
		playerInfoText.text = "";
		teamPlayers = new Stack<PlayerData> ();
	}

	//when the panel is set active
	void OnEnable(){
		//on enable is called when an object is set active
		print (gameObject.name + " enabled!");

		firstActivated = true;
		CyclePanelColor (); //grab the first color by default

	}

	void OnDisable(){
		//on diable is called when an object is set inactive
		print (gameObject.name + " disabled!");
			
	}

	//when the player clicks a team panel, they are added to that team, when they click it again they are removed.
	//If the player clicks on another team panel they are moved to that one and removed from the other
	public void OnTeamPanelClick(){
		print ("you clicked on " + gameObject.name);

		//click
		//if present here, remove
		//if not present, move here
		//if present elsewhere, move here



	}


	//this is called when the "Add Team" button is clicked in the team panel's place
	public void OnAddTeamClick(){
		//set the team panel in the position of the addteam button active
		managerReference.teamPanels [panelNum - 1].SetActive (true);

		//set the pushed add team button inactive
		managerReference.addTeamButtons[panelNum - 1].SetActive(false);

		managerReference.numberOfTeams++;
	}

	//this is called when the close 'x' button is clicked on the team panel
	public void OnCloseClick(){
		//give back the selected color
		teamData.currentColors.Add(teamColor);
		teamColor = null;

		//give back the logo
		print(gameObject.name + "currentTeamLogo: " + currentTeamLogo.name);
		MoveToOffScreenPosition (currentTeamLogo); 
		currentTeamLogo = null; 

		//give back the robot
		MoveToOffScreenPosition (currentTeamRobot);
		currentTeamRobot = null;

		int numberOfCPUPlayers = teamPlayers.Count;
		//give back any cpu players
		for(int i = 0; i < numberOfCPUPlayers; i++){
			RemoveCPUPlayerFromTeam ();
		}

		gameObject.SetActive (false);
		managerReference.addTeamButtons [panelNum - 1].SetActive (true);
		managerReference.numberOfTeams--;
	}
	
	// Update is called once per frame
	void Update () {
		if(colorChanged){
			SetButtonColor();
			SetPanelColor ();
			SetTeamText ();
			SetTeamLogo ();
			SetTeamRobot ();
			colorChanged = false;
		}

		//this is to take care of a bug where the logos are in the wrong place when the screen is resized.
		if(currentTeamLogo != null){
			MoveTeamLogoToPosition (currentTeamLogo);
		}

		//ditto, with the robot image
		if(currentTeamRobot != null){
			MoveRobotImageToPosition (currentTeamRobot);
		}

		if(playerToRemove != null){
			
		}

		SetButtonColor (); //to keep the button up to date

	}

	void SetObjectColor(GameObject toSet, string chosenColor){
		if(chosenColor != null){
			if(chosenColor.Equals(RED)){
				toSet.GetComponent<Image> ().color = Color.red;
			}
			if(chosenColor.Equals(BLUE)){
				toSet.GetComponent<Image> ().color = Color.blue;
			}
			if(chosenColor.Equals(GREEN)){
				toSet.GetComponent<Image> ().color = Color.green;
			}
			if (chosenColor.Equals (ORANGE)) {
				toSet.GetComponent<Image> ().color = teamData.orangeTeamColor;
			}
		}

	}

	//set the color of the panel according to the chosen team
	void SetPanelColor(){

		SetObjectColor (gameObject, teamColor);
		if(currentTeamLogo != null){
			MoveToOffScreenPosition (currentTeamLogo); //move the logo back to off screen position so it's up for grabs
		}
		currentTeamLogo = null; //make currentTeamLogo null to indicate change in team

		if(currentTeamRobot != null){
			MoveToOffScreenPosition (currentTeamRobot);
		}
		currentTeamRobot = null;
	}

	//set the color pick button to the next color in the sequence

	//TODO: does not update when a color has been chosen on another panel, and when there are not more options (grey it out?)
	void SetButtonColor(){

		if(teamData.currentColors.Count > 0){
			string buttonColor = teamData.currentColors [0];

			SetObjectColor (colorPickButton.gameObject, buttonColor);
		}
		else{
			//if we are out of color choices
			colorPickButton.gameObject.GetComponent<Image>().color = Color.grey;
			return;
		}

	}

	//when pressing the pick color button on the panel, cycle through the available colors
	public void CyclePanelColor(){

		//if the team data is not loaded yet, do nothing.
		if(teamData == null){
			return;
		}
		print ("teamColor count:" + teamData.currentColors.Count);
		print ("teamColor: " + teamColor);
		//if the list of availble colors is empty, do nothing
		if(teamData.currentColors.Count == 0){
			return;
		}

		string toPutBack = null;

		//buffer the color to put back, if there is one to put back
		if(teamColor != null){ 
			toPutBack = teamColor;
		}

		//get the new color, first one in the list
		teamColor = teamData.currentColors [0];
		teamData.currentColors.RemoveAt (0);
		colorChanged = true;
		print ("teamColor, after grabbing it: " + teamColor);

		//put the old color back, order?
		if(toPutBack != null){
			teamData.currentColors.Add(toPutBack);
		}

	}

	void SetTeamText(){
		if(teamColor != null){
			if(teamColor.Equals(RED)){
				teamName.text = teamData.redText;
			}
			if(teamColor.Equals(GREEN)){
				teamName.text = teamData.greenText;
			}
			if(teamColor.Equals(BLUE)){
				teamName.text = teamData.blueText;
			}
			if(teamColor.Equals(ORANGE)){
				teamName.text = teamData.orangeText;
			}
		}
	}

	void SetTeamLogo(){

		if(currentTeamLogo == null){

			if(teamColor != null){ //if the team color has been chosen load the logo
				if(teamColor.Equals(RED)){
					currentTeamLogo = teamData.redLogo;
				}
				if(teamColor.Equals(GREEN)){
					currentTeamLogo = teamData.greenLogo;
				}
				if(teamColor.Equals(BLUE)){
					currentTeamLogo = teamData.blueLogo;
				}
				if(teamColor.Equals(ORANGE)){
					currentTeamLogo = teamData.orangeLogo;
				}

				MoveTeamLogoToPosition (currentTeamLogo); //move the team logo in to position on this team panel
			}
		}	
	}

	void SetTeamRobot(){

		if(currentTeamRobot == null){

			if(teamColor != null){
				if(teamColor.Equals(RED)){
					currentTeamRobot = teamData.redRobotImage;
				}
				if(teamColor.Equals(GREEN)){
					currentTeamRobot = teamData.greenRobotImage;
				}
				if(teamColor.Equals(BLUE)){
					currentTeamRobot = teamData.blueRobotImage;
				}
				if(teamColor.Equals(ORANGE)){
					currentTeamRobot = teamData.orangeRobotImage;
				}

				MoveRobotImageToPosition (currentTeamRobot);
			}
		}
	}

	void MoveRobotImageToPosition(GameObject robotToMove){
		robotToMove.transform.position = teamRobotSpot.transform.position;
	}

	void MoveTeamLogoToPosition(GameObject logoToMove){
		logoToMove.transform.position = teamLogoSpot.transform.position;
	}

	void MoveToOffScreenPosition(GameObject toMove){
		toMove.transform.position = offScreenPosition.transform.position;
	}

	public void AddCPUPlayerToTeam(){
		string playerName = managerReference.availableCPUPlayers.Pop ();
		PlayerData newPlayer = new PlayerData (playerName, teamColor, 1000, 3);

		playerDisplay.AddPlayerToTeam (newPlayer);

		managerReference.numberOfCPUPlayers++;
	}

	//right now, just automatically adds a level 3 CPU player to the team
	//with a max of 4 players on a team
//	public void AddCPUPlayerToTeam(){
//		//when pressed
//
//		//get reference to selection screen manager to find out how many CPU players there currently are, and to change it
//		//GameObject screenManager = GameObject.Find ("SelectionScreenManager");
//		//int CPUcount = ++screenManager.GetComponent<SelectionScreenManager> ().numberOfCPUPlayers;//increment 1 and store in CPUcount?
//		managerReference.numberOfCPUPlayers++;
//		string playerName = managerReference.availableCPUPlayers.Pop ();
//
//		//string playerName = "CPU " + CPUcount;
//		//add a new playerdata object to the player stack for this team panel
//		//when the start game button is pressed all the data from each panel will be transfered to the TeamData object to carry over to the game
//		PlayerData newPlayer = new PlayerData(playerName, teamColor, 1000, 3); //default max element volume of 1000 and difficulty of 3 for now
//		teamPlayers.Push(newPlayer);
//
//		//add a new text line to the playerInfoText box, with CPU number taken from the current number of CPU players
//		playerInfoText.text += "\n" + playerName;
//	}

	public void RemoveCPUPlayerFromTeam(){
		//first check to make sure that there are actually players in out stack to remove
		if(teamPlayers.Count > 0){
			PlayerData removedPlayer = playerDisplay.RemoveCPUPlayerFromTeam ();
			managerReference.availableCPUPlayers.Push (removedPlayer.playerName); //why not just keep array of player data? ehh idk
			managerReference.numberOfCPUPlayers--;
		}
		//if not don't do a damn thing
	}

//	//remove last one added, a stack
//	public void RemoveCPUPlayerFromTeam(){
//		//first check to make sure that there are actually players in our stack to remove
//		if(teamPlayers.Count > 0){
//			//removing a player, so subtract from the total count
//			managerReference.numberOfCPUPlayers--;
//
//			//remove the last player added to the stack for this team
//			string playerToRemove = teamPlayers.Pop().playerName;
//
//			//remove player from the text
//			playerInfoText.text = RemoveLastLine(playerInfoText.text); //this is going to break when adding human players, can't just remove last one. Separate box for human players?
//
//			//put the CPU player back in the availble CPU Player stack
//			managerReference.availableCPUPlayers.Push(playerToRemove);
//		}
//		//if not don't do a damn thing
//
//	}

	public string RemoveLastLine(string toChange){
		int index = toChange.LastIndexOf ("\n");
		string returnString = toChange.Substring (0, index);
		return returnString;
	}
}
