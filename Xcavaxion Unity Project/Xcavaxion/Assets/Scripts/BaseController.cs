using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseController : TileController {

	//A subclass of TileController to handle all of the specifics related to base operations
	//May want to make a generic "move element containers around" class since this is the third time it'll be repeated DONE

	public Team baseOwner;

	public string baseColor;

	public int baseNumber; //1 to 4 bases
	public int totalElementVolume; 

	public bool playersSpawned;

	public List<PlayerController> teamPlayers;
	public List<string> nameOfStoredElements;
	public List<int> volumeOfStoredElements;
	public List<string> linkedStructures; //references to strucutes that may be linked to this base, no idea yet.

	public Inventory baseInventory;

	// Use this for initialization
	public override void Start () {
		base.Start (); //should be okay for now a lot of the TileController classes Start() functionality isn't needed

		nameOfStoredElements = new List<string> ();
		volumeOfStoredElements = new List<int> ();

		baseInventory = new Inventory (false, 0, baseColor + " base");
		baseInventory.elementsUpdated = false;

		playersSpawned = false;

	}

	public void SpawnTeamPlayers(){

		foreach(PlayerController player in teamPlayers){
			player.transform.position = transform.position; //move the player to the staring base
			player.basePosition = transform.position; 		//give the players the position of their base
		}
	}
	
	// Update is called once per frame
	public override void Update () {
		//don't think we need any of the TileController update functionality

		if(teamPlayers.Count != 0 && !playersSpawned){
			SpawnTeamPlayers ();
			playersSpawned = true;
		}

		bool updated = baseInventory.elementsUpdated;

		if(updated){
			Debug.Log ("base inventory updated!");
			nameOfStoredElements = baseInventory.GetElementNames ();
			volumeOfStoredElements = baseInventory.GetElementVolumes ();

			totalElementVolume = baseInventory.currentTotalElementVolume;
			baseInventory.elementsUpdated = false;
		}
	}
		
	//How to handle players moving on to the base
	//Temp: just want to have the player dump all elements in to the base for the time being
	void OnTriggerEnter2D(Collider2D other){
		string triggeesName = other.gameObject.name;
		Debug.Log (FirstCharToUpper(triggeesName) + " triggered " + baseOwner + "'s base.");
		if(triggeesName == "Player"){
		}
	}
		
	public static string FirstCharToUpper(string input){
		return input.Substring (0, 1).ToUpper () + input.Substring (1);
	}

	void OnMouseOver(){
		if(base.highlightOn){
			rend.material.color = new Color(0.95f, 0, 0, 0.8f);
		}
	}

	void OnMouseExit(){
		if(base.highlightOn){
			rend.material.color = Color.white; //restores tile color
		}
	}
}