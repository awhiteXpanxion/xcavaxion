using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public GameSetupParameters parameters; 

	public MapController currentGameMap;

	public List<PlayerController> players; //the players are currently added to this list from the Unity inspector, they will have to added at game start
	public List<GameObject> playerControlPanels;

	public bool gameWon;
	public bool basesPresent;
	public bool playersDistributed;

	public int boulderCount;
	public int elementBoxCount;

	public GameObject UIOverlayCanvas;

	public Vector2 controlPanelOutPosition;

	public bool[,] terrainCoverMap;
	public GameObject[,] playableMapReference;

	// Use this for initialization
	void Start () {

		gameWon = false;
		basesPresent = false;
		playersDistributed = false;
		boulderCount = 0;
		elementBoxCount = 0;

		UIOverlayCanvas = GameObject.Find ("UIOverlayCanvas"); //load the UI canvas

		GameObject setupData = GameObject.FindGameObjectWithTag ("SetupData");
		parameters = setupData.GetComponent<GameSetupParameters> ();

		LoadControlPanel();

		GatherPlayers ();

		//Load menu screen parameters in to mapcontroller?
		//Load player selection from menu screen in to "players" list
	}
	
	// Update is called once per frame
	void Update () {

		GetBoulderState ();
		GetElementBoxState ();

		if(boulderCount == 0 && elementBoxCount == 0){

			if(CheckPlayerInventories()){
				CheckWhoWon ();
				CheckTimePassed (30.0f); //wait 30 seconds until the game closes
//				EndGame ();
			}
		}
		if(!basesPresent){
			if(CheckForBases()){
				basesPresent = true;
				Debug.Log ("Found bases!");
			}
		}
		if(basesPresent && !playersDistributed){
			DistributePlayers ();
			playersDistributed = true;
			Debug.Log ("Distributed players!");
		}
	}

	public void GetBoulderState(){
		boulderCount = currentGameMap.onScreenBoulderCount;
	}

	public void GetElementBoxState(){
		elementBoxCount = currentGameMap.onScreenElementBoxCount;
	}

	public void EndGame(){
		SceneManager.LoadScene(0);
	}

	//checks all the in game player inventories to make sure they are empty to end the game
	public bool CheckPlayerInventories(){
		int totalVolume = 0;
		bool emptyInventories = false;
		foreach(PlayerController cont in players){
			totalVolume += cont.inventory.currentTotalElementVolume;
		}
		if(totalVolume == 0){
			emptyInventories = true;
		}
		return emptyInventories;
	}

	//checks to make sure all 4 different bases are present
	public bool CheckForBases(){
		Debug.Log ("checking for bases!");
		GameObject blueBase = GameObject.Find ("BlueBase(Clone)");
		GameObject greenBase = GameObject.Find ("GreenBase(Clone)");
		GameObject orangeBase = GameObject.Find ("OrangeBase(Clone)");
		GameObject redBase = GameObject.Find ("RedBase(Clone)");

		bool blueCheck = false;
		bool greenCheck = false;
		bool orangeCheck = false;
		bool redCheck = false;

		if(blueBase != null){
			blueCheck = true;
		}
		if(greenBase != null){
			greenCheck = true;
		}
		if(orangeBase != null){
			orangeCheck = true;
		}
		if(redBase != null){
			redCheck = true;
		}
		return blueCheck && greenCheck && orangeCheck && redCheck;
	}

	//Checks for a base of a specific color, expects the the teamColor to be capitialized
	public bool CheckForSpecificBase(string teamColor){
		GameObject baseToCheck = GameObject.Find (teamColor + "Base(Clone)");
		return baseToCheck != null;
	}

	//this is a dumb way to do this
	public void CheckWhoWon(){
		GameObject blueBase = GameObject.Find ("BlueBase(Clone)");
		GameObject greenBase = GameObject.Find ("GreenBase(Clone)");
		GameObject orangeBase = GameObject.Find ("OrangeBase(Clone)");
		GameObject redBase = GameObject.Find ("RedBase(Clone)");

		int blueAmount = blueBase.GetComponent<BaseController>().totalElementVolume;
		int greenAmount = greenBase.GetComponent<BaseController>().totalElementVolume;
		int orangeAmount = orangeBase.GetComponent<BaseController>().totalElementVolume;
		int redAmount = redBase.GetComponent<BaseController>().totalElementVolume;

		int highest = 0;
		string winStatement = "";

		print ("Blue Total: " + blueAmount);
		print ("Green Total: " + greenAmount);
		print ("Orange Total: " + orangeAmount);
		print ("Red Total: " + redAmount);

		if (blueAmount > highest){
			highest = blueAmount;
			winStatement = "BLUE TEAM WINS!";
		}
		if (greenAmount > highest){
			highest = greenAmount;
			winStatement = "GREEN TEAM WINS!";
		}
		if (orangeAmount > highest){
			highest = orangeAmount;
			winStatement = "ORANGE TEAM WINS!";
		}
		if (redAmount > highest){
			highest = redAmount;
			winStatement = "RED TEAM WINS!";
		}
		print (winStatement);	
	}

	public void GatherPlayers(){
		//get player information from the game parameters
		List<PlayerData> gamePlayers = new List<PlayerData>();
		gamePlayers.AddRange(parameters.playerInformation);

		string playerName = "";
		bool cpuPlayer = false;
		int cpuDifficulty = 0;
		//create each player game object
		foreach(PlayerData gamePlayer in gamePlayers){
			playerName = gamePlayer.playerName;
			print ("gameplayer team color: " + gamePlayer.playerTeamColor);
			GameObject playerPrefab = GrabPlayerPrefab (gamePlayer.playerTeamColor);
			var playerTemp = Instantiate (playerPrefab) as GameObject;
			PlayerController tempCont = playerTemp.GetComponent<PlayerController> ();
			tempCont.playerIdentifier = playerName;
			cpuDifficulty = gamePlayer.cpuPlayerDifficulty;

			if(cpuDifficulty == 0){
				cpuPlayer = false;
			}
			else{
				cpuPlayer = true;
				tempCont.GetComponent<AIController> ().difficultyLevel = cpuDifficulty;
			}

			players.Add (tempCont); //put reference to it in players list
		}

		//movement of players to bases handled elsewhere

	}
		
	public void DistributePlayers(){

		foreach(PlayerController player in players){

			if(player.teamColor.Equals("blue")){
				GameObject blueBase = GameObject.Find ("BlueBase(Clone)");
				blueBase.GetComponent<BaseController> ().teamPlayers.Add (player);	
			}
			if(player.teamColor.Equals("green")){
				GameObject greenBase = GameObject.Find ("GreenBase(Clone)");
				greenBase.GetComponent<BaseController> ().teamPlayers.Add (player);
			}
			if(player.teamColor.Equals("orange")){
				GameObject orangeBase = GameObject.Find ("OrangeBase(Clone)");
				orangeBase.GetComponent<BaseController> ().teamPlayers.Add (player);
			}
			if(player.teamColor.Equals("red")){
				GameObject redBase = GameObject.Find ("RedBase(Clone)");
				redBase.GetComponent<BaseController> ().teamPlayers.Add (player);
			}
		}
	}

	public GameObject GrabPlayerPrefab(string teamColor){
		string loadString = "";
		switch(teamColor){
		case "red":
			loadString = "RedTeamPlayerCPU";
			break;
		case "green":
			loadString = "GreenTeamPlayerCPU";
			break;
		case "blue":
			loadString = "BlueTeamPlayerCPU";
			break;
		case "orange":
			loadString = "OrangeTeamPlayerCPU";
			break;
		}
		GameObject temp = Resources.Load<GameObject> ("Prefabs/" + loadString); //this is not loading anything
		print ("temp game object name: " + temp.name);
		return temp;
	}


	public void LoadControlPanel(){
		string teamColor = "";
		//Find the main player
		foreach(PlayerController player in players){
			if(player.playerIdentifier.Equals("player")){
				teamColor = player.teamColor;
			}
		}

		//find the appropriate control panel for player color
		GameObject controlPanel = null;
		foreach(GameObject cp in playerControlPanels){
			if(cp.name.Contains(teamColor)){
				controlPanel = cp;
			}
		}
		//put it in to place on the UI overlay canvas, instantiate
	}

	public bool CheckTimePassed(float timeToWait){
		float delta = 0.0f;
		while(timeToWait >= delta){
			delta += Time.deltaTime;
		}
		return true;
	} 
}