using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileItemManager {

	// the contents item contents of the tile
	public List<string> buriedItems; //temporarily a sting until the class is made
	public List<ElementContainer> buriedElements;
	public List<string> buriedCodeNuggets; //temporarily a string until the class is made

	//the list of 8 spawn points for items within a tile
	public List<Vector2> tileItemSpawnPoints = new List<Vector2> {
			new Vector2(0.0f, 0.24f),
			new Vector2(0.16f, 0.16f), 
			new Vector2(0.24f, 0.0f), 
			new Vector2(0.16f, -0.16f), 
			new Vector2(0.0f, -0.24f), 
			new Vector2(-0.16f, 0.16f),
			new Vector2(-0.24f, 0.0f), 
			new Vector2(-0.16f, -0.16f) };

	public TileItemManager(){
		buriedElements = new List<ElementContainer> ();
	}

	public void AddItemToTile(string itemToAdd){
		buriedItems.Add (itemToAdd);
	}

	//handles combining element containters, only want element containers of individual elements
	//return element extraction difficulty?
	public int AddElementContainerToTile(ElementContainer elementContainerToAdd){
		string toAddName = elementContainerToAdd.contents.name;
		if (CheckIfElementAlreadyContained (toAddName)) {
			AddElementContentsTo (toAddName, elementContainerToAdd.volume);
		} else {
			buriedElements.Add (elementContainerToAdd);
		}
		return elementContainerToAdd.contents.extractionDifficulty;
	}

	public void AddCodeNuggetsToTile(string codeNuggetsToAdd){
		buriedCodeNuggets.Add (codeNuggetsToAdd);
	}

	//checks if the element container is already present within the tile in order to avoid duplication
	public bool CheckIfElementAlreadyContained(string elementName){
		bool elementContained = false;
		foreach(ElementContainer cont in buriedElements){
			if(elementName.Equals(cont.contents.name)){
				elementContained = true;
			} 
		}
		return elementContained;
	}

	//assumes element container already exists in list of buried elements
	public void AddElementContentsTo(string elementName, int volumeToAdd){
		foreach(ElementContainer cont in buriedElements){
			if(elementName.Equals(cont.contents.name)){
				cont.volume += volumeToAdd; //add the amount to preexisting element container
			}
		}
	}

	/* Item Spawn Points
	  
	  There are currently 8 predetermined possible spawn points for items on a tile, which are selected from randomly
	  Items contents of a tile will have to be limited to 8 max if these are the points
	  64 x 64 pixel tiles
	   -----------------------------
	  |              |              |
	  |              *0,24          |
	  |              |              |
	  |     *-16,16  |   16,16*     |
	  |              |              |
	  |              |              |
	  |              |      24,0    |
	  |---*----------o----------*---|
	  |    -24,0     |              |
	  |              |              |
	  |              |              |
	  |     *-16,-16 |  16,-16*     |
	  |              |              |
	  |         0,-24*              |
	  |              |              |
	   -----------------------------
	 
	*/

	//returns a single tile spawn point randomly from the list of points, will duplicate points
	public Vector2 SingleItemPoint(){
		int pointCount = tileItemSpawnPoints.Count;
		System.Random rand = new System.Random ();
		return tileItemSpawnPoints [rand.Next (pointCount)];
	}

	//returns a list of spawn points of specified size from the list of spawn points, does not duplicate points
	//passing in spawn points in case I want to keep a list of availble points at a higher level
	public List<Vector2> ItemDispersement(int numberOfPoints, List<Vector2> spawnPoints){
		List<Vector2> pointList = spawnPoints; //copy points list to be able to modify list
		List<Vector2> returnPoints = new List<Vector2>();
		System.Random rand = new System.Random ();
		int pointCount = spawnPoints.Count;

		if(numberOfPoints > pointCount){
			Debug.Log ("Number of item spawn points requested exceeds number of existing spawn points in list.");
			return null;
		}

		for(int i = 0; i < numberOfPoints; i++){ //remove randomly chosen point from list of points
			int choice = rand.Next (pointCount);
			returnPoints.Add (pointList [choice]);
			pointList.RemoveAt (choice);
			pointCount = pointList.Count;
		}
		return returnPoints;
	}

	public void PrintList(List<Vector2> toPrint){
		int i = 0;
		foreach(Vector2 vec in toPrint){
			Debug.Log ("list: " + i + ": " + vec.ToString ());
			i++;
		}
	}
}