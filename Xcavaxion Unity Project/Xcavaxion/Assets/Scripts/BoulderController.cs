using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class BoulderController : MonoBehaviour {

	//This is a Boulder GameObject class that represents boulders as individual objects in the game, as oppossed to just being different tiles, like they used to be.
	//The boulder may contain items, elements, and or code nuggets, all of which are managed in the TileItemManager boulderItems.

	public Renderer rend;

	public int miningDifficulty; //0, nothing in the boulder, crumbles, 1 easy to 5 hard.

	public bool minedBoulder; //whether or not the boulder has been mined
	public TileController tileBoulderIsOn;

	public bool hasItems;
	public bool hasElements;
	public bool hasCodeNuggets;

	public TileItemManager boulderItems;

	private List<Vector2> itemSpawnPoints;

	//this random is created for each new boulder, it is based on system time, so elements contained are not very distriubted
	System.Random rand = new System.Random (); //only use this if the boulder itself needs to do something multiple times

	public int randomElementFactor;

	private string prefabsPath = "Prefabs/";

	// Use this for initialization
	void Start () {
		rend = GetComponent<SpriteRenderer> ();

		hasItems = false;
		hasElements = true;
		hasCodeNuggets = false;

		minedBoulder = false;

		//upon creation add appropriate amounts of items, elements and code nuggets to the boulder
		if(hasItems || hasElements || hasCodeNuggets){

			//because this boulder has items, elements, or code nuggets, create the boulderItem object
			boulderItems = new TileItemManager();
			//and get the item spawn point list
			itemSpawnPoints = boulderItems.tileItemSpawnPoints;

			if(hasItems){
				Debug.Log ("Added items to boulder");
			}
			if (hasElements) {
				AddElementsToBoulder ();
			}
			if(hasCodeNuggets){
				Debug.Log ("Added code nuggets to boulder");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(minedBoulder){
			tileBoulderIsOn.hasBoulder = false; //set the tile this boulder is on, hasBoulder condition to false
			tileBoulderIsOn.tileBoulder = null; //make the tile's boulder reference null

			//release items here
			if(hasElements){
				ReleaseBoulderElements (boulderItems.buriedElements, itemSpawnPoints);
			}
			//release code nuggets here

			Destroy (gameObject); //destroy the boulder
		}
	}

	public void OnCollisionEnter2D(Collision2D collision){
		//Debug.Log ("boulder collision occurred");

		//TODO this needs to be changed, mining a boulder will be more of a process than just running in to it
		//checks if the player collided with a boulder tile and if so sets the mined boulder conidtion to true, which will destroy the boulder
		if(!minedBoulder && collision.gameObject.layer == 22){
			//Debug.Log("boulder mined by: " + collision.gameObject.name);
			minedBoulder = true;
		}
	}

	//This adds a random number of random elements of random quantities to the boulder... randomly.
	public void AddElementsToBoulder(){
		//find the gameobject containing all the in game items
		GameObject inGameItems = GameObject.FindWithTag ("All Game Items");
		int i;
		int extractionDifficulty = 0;
		//int randomAmount = rand.Next (tileBoulderIsOn.elementFrequency + 1);
		int randomAmount = randomElementFactor;
		for(i = 0; i < randomAmount; i++){
			extractionDifficulty += boulderItems.AddElementContainerToTile (inGameItems.GetComponent<GameItems>().allGameItems.GetRandomVolumeOfDispersedElement());
		}
		if(randomAmount != 0){ 	//avoid a divide by zero error
			miningDifficulty = extractionDifficulty / randomAmount;
		}
		else{
			miningDifficulty = 0;
		}
	}

	//Remove all elements from boulder tile and into ElementBoxes created on the ground.
	//Does not duplicate items anymore
	void ReleaseBoulderElements(List<ElementContainer> boulderContents, List<Vector2> spawnPoints){
		int contentsCount = boulderContents.Count;
//		System.Random rand = new System.Random ();
		List<Vector2> chosenPoints = boulderItems.ItemDispersement (contentsCount, spawnPoints); //list of randomly chosen spawn points

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

	public void UpdateElementBoxCount(int amount){
		GameObject mapCont = GameObject.Find ("MapController");
		mapCont.GetComponent<MapController> ().onScreenElementBoxCount += amount;
	}

	public void UpdateBoulderCount (int amount){
		GameObject mapCont = GameObject.Find ("MapController");
		mapCont.GetComponent<MapController> ().onScreenBoulderCount += amount;
	}

	void OnMouseOver(){
		rend.material.color = new Color(0.95f, 0, 0, 1.0f);
	}

	void OnMouseExit(){
		rend.material.color = Color.white; //restores tile color
	}
}