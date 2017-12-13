using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour {

	public int elementVolumeCapacity; //the carrying capacity of elements based on volume, perhaps no limit set at 0 or -1?
	public int currentTotalElementVolume;

	public bool elementsFull;

	public List<string> elementNames;
	public List<int> elementVols;
	public int elementsCount;

	public Inventory playerInventory; //the actual inventory object of the player

	// Use this for initialization
	void Start () {
		elementsFull = false;

		string playerName = gameObject.GetComponentInParent<PlayerController> ().playerIdentifier;
		playerInventory = new Inventory (true, elementVolumeCapacity, playerName); //trying out a seperate inventory class to contain all the actions

		elementNames = new List<string> ();
		elementVols = new List<int> ();
	}
	
	// Update is called once per frame
	void Update () {

		if(currentTotalElementVolume >= elementVolumeCapacity){
			elementsFull = true;
		}
		else{
			elementsFull = false;
		}

		if(playerInventory.elementsUpdated){

			elementNames = playerInventory.GetElementNames ();
			elementVols = playerInventory.GetElementVolumes ();

			currentTotalElementVolume = playerInventory.currentTotalElementVolume;

			playerInventory.elementsUpdated = false;
		}
	}
}