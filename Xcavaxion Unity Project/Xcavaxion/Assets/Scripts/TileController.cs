using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using NUnit.Framework;

public class TileController : MonoBehaviour {

	public Renderer rend;

	public bool hasBoulder;
	public BoulderController tileBoulder; //reference to the potential boulder on this tile

	public bool minedBoulder;
	public bool hasBuriedItems;
	public bool hasBuriedElements;
	public bool hasBuriedCodeNuggets;

	public bool mouseClicked;

	public bool highlightOn;

	public int elementFrequency; //scale of 1 to 100, 100 being alot of elements?
	private int groundTilePrefabCount = 5; //the number of different ground tile prefabs available
	private int boulderPrefabCount = 3;

	public int boulderPrefabChoice;
	public int randomFactor;

	private string tileName;
	private string boulder = "BoulderTile";
    private string buffer = "BufferTile";
    private string border = "BorderTile";
    private string ground = "GroundTile";
	private string baseTile = "Base";

	private string boulderPrefab = "BoulderPrefab";
	private string prefabsPath = "Prefabs/";

	private List<Vector2> itemSpawnPoints;

	public TileItemManager tileItems;

	public Vector2 tileCoordinates; //this is the position data of the game screen
	public Vector3 mouseClickCoords;

	public UIMessageHandler messages;

	System.Random rand = new System.Random ();

	// Use this for initialization
	public virtual void Start () {
       
		rend = GetComponent<SpriteRenderer> ();
		tileName = gameObject.name;
		minedBoulder = false;
		hasBuriedItems = false;
		hasBuriedElements = false;
		hasBuriedCodeNuggets = false;

		mouseClicked = false;
		highlightOn = false;

		if(hasBuriedItems || hasBuriedElements || hasBuriedCodeNuggets){

			//because this tile has items, elements, or code nuggets, create the tileItem object
			tileItems = new TileItemManager();
			//and get the item spawn point list
			itemSpawnPoints = tileItems.tileItemSpawnPoints;

			if(hasBuriedItems){
				Debug.Log ("Added items to tile");
			}
			if (hasBuriedElements) {
				AddElementsToTile ();
			}
			if(hasBuriedCodeNuggets){
				Debug.Log ("Added code nuggets to tile");
			}
		}

		if(hasBoulder){
			CreateBoulder ();
		}
	}

	public void CreateBoulder(){
		//create a new boulder prefab on top of this tile
		string toLoad = boulderPrefab + boulderPrefabChoice;

		//select random prefab
		GameObject newBoulder = (GameObject)Instantiate (Resources.Load (prefabsPath + toLoad), 
			new Vector3(transform.position.x, transform.position.y, 0.0f), transform.rotation); //instantiate new boulder

		//establish the two way references
		tileBoulder = newBoulder.GetComponent<BoulderController>();
		tileBoulder.tileBoulderIsOn = this.gameObject.GetComponent<TileController>();

		newBoulder.GetComponent<BoulderController> ().randomElementFactor = randomFactor;

		//boulder handles it's own destruction
	}

	public virtual void Update() {

		//this is no longer used
		if(tileName.Contains(boulder) && minedBoulder){ //this handles the self destruction of a boulder tile and creation of a tile to replace it
			List<ElementContainer> temp = new List<ElementContainer> (); //buffer to copy the tile element contents into
			temp = tileItems.buriedElements;

			Destroy (gameObject); //destroy the boulder tile
			UpdateBoulderCount(-1); //accesses the map controller and removes one from the boulder count
			Debug.Log ("Destroyed the boulder tile!");

			CreateRandomGroundTile();

			if(hasBuriedElements){
				ReleaseBoulderElements (temp, itemSpawnPoints);
			}
		}

//		if(Input.GetMouseButtonDown(0)){
//			mouseClickCoords = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//			mouseClicked = true;
//			GameObject playerWhoClicked = GameObject.Find("Player");
//			playerWhoClicked.GetComponent<PlayerController> ().mouseClickCoords = this.mouseClickCoords;
//			playerWhoClicked.GetComponent<PlayerController> ().mouseClicked = true;
//			mouseClicked = false;
//		}

		//if boulder is destroyed, check hasBoulder, I don't think the tile should have to do anything
			//boulder unchecks hasBoulder
			//boulder destroys itself
			//boulder releases items
			//boulder nulls reference
	}

	public void UpdateBoulderCount (int amount){
		GameObject mapCont = GameObject.Find ("MapController");
		mapCont.GetComponent<MapController> ().onScreenBoulderCount += amount;
	}

	public void UpdateElementBoxCount(int amount){
		GameObject mapCont = GameObject.Find ("MapController");
		mapCont.GetComponent<MapController> ().onScreenElementBoxCount += amount;
	}
		
	public void SetCoordinates(Vector2 newCoords){
		tileCoordinates = newCoords;
	}

	void OnDestroy(){
		//this leaves things in the game scene and is not cleaned upon end, don't use
	}

	public virtual void OnCollisionEnter2D(Collision2D collision){
		//nothing at the moment, used to handle boulder tile collsion
	}

	void OnMouseEnter(){

	}

	void OnMouseOver(){
		if (highlightOn) {
			if (tileName.Contains (ground)) { //only highlight ground or boulder tiles
				rend.material.color = new Color (0.95f, 0, 0, 0.8f);
			}
		}
	}

	void OnMouseExit(){
		if(highlightOn){
			rend.material.color = Color.white; //restores tile color
		}
	}

	//This adds a random number of random elements of random quantities to the tile... randomly.
	public void AddElementsToTile(){
		//find the gameobject containing all the in game items
		GameObject inGameItems = GameObject.FindWithTag ("All Game Items");
		int i;
		for(i = 0; i < elementFrequency; i++){
			tileItems.AddElementContainerToTile (inGameItems.GetComponent<GameItems>().allGameItems.GetRandomVolumeOfDispersedElement());
		}
	}
		
	//Randomly chooses one of the ground tile prefabs to create, for when the boulder is destroyed, not needed anymore
	void CreateRandomGroundTile(){
		int nextRandom = rand.Next (groundTilePrefabCount) + 1; //add one because rand.Next is from 0 to (x) exclusive
		string toLoad = ground + nextRandom;
		GameObject newTile = (GameObject)Instantiate (Resources.Load (prefabsPath + toLoad), 
			new Vector3(transform.position.x, transform.position.y, 0.0f), transform.rotation);//instantiate new ground tile
	}

	//Remove all elements from boulder tile and into ElementBoxes created on the ground.
	//Does not duplicate items anymore
	void ReleaseBoulderElements(List<ElementContainer> boulderContents, List<Vector2> spawnPoints){
		int contentsCount = boulderContents.Count;
		List<Vector2> chosenPoints = tileItems.ItemDispersement (contentsCount, spawnPoints); //list of randomly chosen spawn points

		foreach(ElementContainer ele in boulderContents){
			string nameOfBoxPrefab = prefabsPath + ele.contents.name + "Box";
			GameObject newBox = (GameObject)Instantiate(Resources.Load(nameOfBoxPrefab), transform.parent);
			UpdateElementBoxCount (1);
			Transform tilePos = gameObject.transform; //the position in coordinate space of the current tile

			int removeIndex = rand.Next(chosenPoints.Count); //randomly choose spawn point
			Vector2 newItemPos = chosenPoints[removeIndex];
			chosenPoints.RemoveAt (removeIndex); //remove used point so no overlap

			float newX = tilePos.position.x + newItemPos.x;
			float newY = tilePos.position.y + newItemPos.y;

			//if the element box prefab has a position entered it will be spawned in the wrong place
			newBox.transform.Translate (new Vector3 (newX, newY, 0.0f)); //move the box to the item spawn point
			newBox.transform.localScale = new Vector3(0.3f, 0.3f, 0.0f); //scale down the element boxes by 0.3

			newBox.GetComponent<ElementBox>().container = ele; //transfer tile single tile element container to new box
		}
	}

	public void SetHasBoulder(){
		hasBoulder = true;
	}

	public void UnsetHasBoulder(){
		hasBoulder = false;
	}
}