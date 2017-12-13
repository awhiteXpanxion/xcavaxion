using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainTile : MonoBehaviour {

	/*  ---------------------
	 *  |     |      |      |
	 *  | NW  |  N   |  NE  |
	 *  |     |      |      |
	 *  ---------------------
	 *  |     |      |      |
	 *  |  W  |      |   E  |
	 *  |     |      |      |
	 *  ---------------------
	 *  |     |      |      |
	 *  | SW  |  S   |  SE  |
	 *  |     |      |      |
	 *  ---------------------
	 * 
	 * byte readout of tile surroundings, start at NW, left to right, top to bottom
	 * 
	 * 
	 * byte: 0b | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
	 *          |SE | S |SW | E | W |NE | N |NW |
	 *          |2^7|2^6|2^5|2^4|2^3|2^2|2^1|2^0|
	 * 
	 */

	public Renderer rend;

	public bool excavated; // if this is true there is no raised terrain on this tile
	public bool newlyExcavated; //if this is true, terrain manager needs updating, this may be redundant, we'll see
	public bool pendingDestruct;
	public bool notifiedManager;

	public bool needUpdatingAndRemoval;

	public byte tileByte; //the byte that shows that state surrounding the tile, null if it is an excavated tile

	public float excavationPercent;
	public float excavationAmount = 50.0f; //will probably be based on the player doing the excavating
	public float tileScale;

	public string tileType; //the current name of the type of tile for this tile

	public Vector2 tileCoords;
	public Vector2 origin;

	public GameObject terrainManagerReference;


	// Use this for initialization
	void Start () {
		
		pendingDestruct = false;
		notifiedManager = false;

		rend = GetComponent<SpriteRenderer> ();

		if(excavated){
			ExcavatedTileAcclimation ();
		}

		terrainManagerReference = GameObject.FindGameObjectWithTag ("TerrainManager");

		needUpdatingAndRemoval = false;

	}
	
	// Update is called once per frame
	void Update () {

		if(excavationPercent >= 100.0f && !notifiedManager){
			newlyExcavated = true;
		}


		//if newly excavated inform terrain manager
		if(newlyExcavated && !notifiedManager){
			
			terrainManagerReference.GetComponent<TerrainManager>().notifications.Enqueue(tileCoords);
			notifiedManager = true;
			print ("Enqueued: " + tileCoords.ToString ());
			newlyExcavated = false; //no longer newly excavated

			//then Destroy self GASP! maybe not
			//Destroy(this.gameObject);
			pendingDestruct = true;
			//tile will be replaced by the terrain manager

		}

//		if(gameObject.name.Equals("excavatedTile(Clone)")){
//			//ignore collisions on polygon collider
//			Physics2D.IgnoreLayerCollision(22, gameObject.layer); //<-------- not sure about this
//		}

//		if(pendingDestruct){
//			print ("self destruct!");
//			//doesn't seem to work
//			Destroy (gameObject); //flag for it this tile needs to be changed because of a neighbor
//		}

	}

	void FixedUpdate(){
		if(pendingDestruct){
			print ("self destruct!");
			//doesn't seem to work
			Destroy (gameObject); //flag for it this tile needs to be changed because of a neighbor
		}
	}

	public void PrepForPendingDestruct(){
		print ("pending destruct PREPARED!!! for: " + tileType);
		Destroy (gameObject);
		//pendingDestruct = true;
		print("I guess we didn't destroy the tile eh?");
	}

	void ExcavatedTileAcclimation(){
		excavated = true;
		newlyExcavated = false;
		tileByte = 0; //can't be null, I guess it doesn't really matter what the value of this is as it is meaningless when a tile is excavated
		tileType = "excavatedTile";
	}
		
	public void OnCollisionEnter2D(Collision2D collision){

		Debug.Log ("terrain collision occurred");

		//checks if the player collided with a terrain tile and if so increases the excavation percent
		if(!excavated && collision.gameObject.layer == 22){
			excavationPercent += excavationAmount; //each time the terrain is touched add a quarter of excavation percent
		}
	}


}
