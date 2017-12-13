using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NUnit.Framework;
using UnityEngine.WSA;

public class TerrainManager : MonoBehaviour {

	//This game object will control the terrain of the map and exist in the game only if the option for terrain is chosen over boulders.

	public int tileBuffer;

	public float tileScale;

	public string chosenTerrain; //passed to this at the start of the game during the map drawing process

	public Vector2 origin;
	public Vector2 mapDimensions;

	public Vector2[] neighborTileAdditives = {
		new Vector2(-1.0f, 1.0f), 
		new Vector2(0.0f, 1.0f), 
		new Vector2(1.0f, 1.0f),
		new Vector2(-1.0f, 0.0f), 
		new Vector2(1.0f, 0.0f), 
		new Vector2(-1.0f, -1.0f), 
		new Vector2(0.0f, -1.0f), 
		new Vector2(1.0f, -1.0f)
	};

	//dimensions of these determined by options menu
	public bool[,] terrainBoolMap; //the boolean array of the map
	public GameObject[,] terrainMapArray; //the map array of all the terrain tiles
	public GameObject[,] mapArrayWithBuffer;

	public Queue<Vector2> notifications; //the queue of terrain tiles (by their coordinates in the map) that have notified the manager of an update

	public GameObject[] terrainTiles;

	// Use this for initialization
	void Start () {

		//there needs to be an initial map draw, done in MapController

		notifications = new Queue<Vector2> ();
		terrainTiles = LoadTerrainTiles (chosenTerrain);

		//LoadGameParameterData ();

		terrainMapArray = new GameObject[(int)mapDimensions.x, (int)mapDimensions.y]; //instantiate the array for the terrain map
		mapArrayWithBuffer = new GameObject[(int)mapDimensions.x + tileBuffer, (int)mapDimensions.y + tileBuffer];

		CreateMap ();
		DrawMap (tileScale, origin);
	}
	
	// Update is called once per frame
	void Update () {


		
	}

	void FixedUpdate() {
		//print ("notification count: " + notifications.Count);

		//if there are notifications in the list, take care of them
		if(notifications.Count > 0){
			HandleNotifications ();
		}
	}

	public void CreateMap(){
		
		byte tileByte = 0;
		string tileType = "";
		GameObject chosenTile = null;

		//check the bool map
		for(int y = 0; y < (int)mapDimensions.y; y++){
			for(int x = 0; x < (int)mapDimensions.x; x++){
				//look at each tile, get it's tile byte and tile type
				if(terrainBoolMap[x, y]){
					tileByte = DetermineTileByte(x, y);
					print ("tileByte: " + tileByte);
					tileType = DetermineTileType (tileByte);
					print ("tileType: " + tileType);

					//fetch the appropriate tile
					chosenTile = FindTerrainTile(tileType);
				}
				else{
					chosenTile = FindTerrainTile ("excavatedTile");
				}

				TerrainTile tempTile = chosenTile.GetComponent<TerrainTile> ();
				tempTile.tileByte = tileByte;
				tempTile.tileType = tileType;
				tempTile.tileScale = tileScale;
				tempTile.origin = origin;

				//put the appropriate tile in the map array
				terrainMapArray[x, y] = chosenTile;
			}
		}
	}
		
	public void DrawMap(float tileScale, Vector2 origin){
		Vector2 tileCoords;
		int mapSizeX = (int)mapDimensions.x;
		int mapSizeY = (int)mapDimensions.y;

		for (int y = 0; y < mapSizeY; y++) {
			for (int x = 0; x < mapSizeX; x++) {
				//move the position based on tile scale
				var transformX = x * tileScale;
				var transformY = y * tileScale;

				//change depending on origin
				transformX += origin.x;
				transformY += origin.y;

				//set tile coords to send to tile
				tileCoords.x = transformX;
				tileCoords.y = transformY;

				var temp = Instantiate (terrainMapArray [x, y]) as GameObject;
				temp.transform.position = new Vector3 (transformX, transformY, 0.0f);

				//give the tile information about it's position on the screen
				TerrainTile currentTile = temp.GetComponent<TerrainTile> ();
				currentTile.tileCoords = tileCoords;
			}
		}
	}

	public void HandleNotifications(){

		string tileType = "";

		//take the first notification from the queue and address that tile
		Vector2 currentTileCoords = notifications.Dequeue();
		print ("Dequeued: " + currentTileCoords.ToString ());
		GameObject currentTile = terrainMapArray [(int)currentTileCoords.x, (int)currentTileCoords.y];
		tileType = "excavatedTile";

		//take care of the tile itself
		UpdateTile(0, tileType, currentTileCoords); //tileByte doesn't matter here for excavated tile
			
		//reevaluate the tileByte of the tile's 8 neighbors
		//if necesary keep track of the what new tiles the neighbors need to be replaced with
		List<Vector2> neighborTiles = GatherCoordsOfTileNeighbors(currentTileCoords);
		print ("currentTileCoords: " + currentTileCoords);
		foreach(Vector2 coord in neighborTiles){
			print ("neigbor tile coord: " + coord.ToString ());
		}

		int neighborNum = 0;
		//take care of all the tile's neighbors
		foreach(Vector2 tileCoord in neighborTiles){
			print ("neighbor " + neighborNum + ":");
			//if the tile coordinates of the neighbors are within the bounds of the map, do stuff
			if(CheckIfTileInMapBounds((int)tileCoord.x, (int)tileCoord.y)){

				//if the tile in question is unexcavated, we examine it
				if(terrainBoolMap[(int)tileCoord.x, (int)tileCoord.y] == true){
					//find out the neighbor's tileByte
					byte tileByte = 0;
					tileByte = DetermineTileByte((int)tileCoord.x, (int)tileCoord.y);
					print ("neighbor tile Byte: " + tileByte);
					//find out the neighbor's tileType
					tileType = DetermineTileType(tileByte);
					print ("neighbor tile Type: " + tileType);
					UpdateTile(tileByte, tileType, tileCoord);
				}
				//otherwise leave it alone
				else{
					print ("neighbor is already excavated");
				}

			}
			else{
				print ("out of bounds neighbor");
			}
			neighborNum++;
			//otherwise dont do anything
		}
			
	}

	//a tile has been excavated and needs to be updated to a new tile prefab
	//this also draws the new tile, so that each tile can be updated individually and the whole map doesn't have to be redrawn
	public void UpdateTile(byte tileByte, string tileType, Vector2 updatedTileCoords){

		int xCoord = (int)updatedTileCoords.x;
		int yCoord = (int)updatedTileCoords.y;

		//if chosen tileType already equals existing tileType, just update tile's tileByte. tile tile tile tile
		TerrainTile existingTile = terrainMapArray[xCoord, yCoord].GetComponent<TerrainTile>();
		string existingTileType = existingTile.tileType;
		if(existingTileType.Equals(tileType)){
			existingTile.tileByte = tileByte;
			return;
		}
		existingTile.needUpdatingAndRemoval = true;

		//grab the old tile's screen position information
		//Transform tileScreenTransform = terrainMapArray [xCoord, yCoord].transform;
		//print (tileScreenTransform.position.ToString ());

		//add the appropriate terrain tile (decided elsewhere) to the place in the terrain map array
		GameObject tileToUse = FindTerrainTile(tileType);
		TerrainTile tileParameters = tileToUse.GetComponent<TerrainTile> ();
		tileParameters.tileCoords = updatedTileCoords; //give the new tile the old tile's coords
		tileParameters.tileByte = tileByte; //give the new tile it's tileByte
		tileParameters.tileType = tileType; //give the new tile it's tileType

		//destroy the old tile
		//terrainMapArray [xCoord, yCoord].GetComponent<TerrainTile> ().pendingDestruct = true; //tell the old tile to destroy itself, doesn't work?
		GameObject tileToDestroy = terrainMapArray[xCoord, yCoord];
		TerrainTile destroyTile = tileToDestroy.GetComponent<TerrainTile> ();
		destroyTile.PrepForPendingDestruct ();

		//add the new tile to the terrainMapArray
		terrainMapArray [xCoord, yCoord] = tileToUse;

		//change depending on the tileScale
		var transformX = xCoord * tileScale;
		var transformY = yCoord * tileScale;

		//change depending on origin
		transformX += origin.x;
		transformY += origin.y;

		//draw the tile
		var temp = Instantiate (terrainMapArray [xCoord, yCoord]) as GameObject;
		temp.transform.position = new Vector3(transformX, transformY, 0.0f); //ehh might work?

		//update the terrain bool map
		if(tileType.Equals("excavatedTile")){
			terrainBoolMap[xCoord, yCoord] = false;
		}
		else{
			terrainBoolMap[xCoord, yCoord] = true;
		}

		//the tile itself should take care of it's parameters for being excavated and such
	}

	public bool CheckIfTileInMapBounds(int xCoord, int yCoord){
		return (xCoord >= 0 && yCoord >= 0) && (xCoord <= (int)mapDimensions.x && yCoord <= (int)mapDimensions.y);
	}

	//From the chosen options for the map in the menu. Load them into this gameObject to reference.
	public void LoadGameParameterData(){
		
		GameObject setupObj = GameObject.FindGameObjectWithTag ("SetupData"); //unsure about the name of the object
		GameSetupParameters gameParameters = setupObj.GetComponent<GameSetupParameters>();
		chosenTerrain = gameParameters.terrainType;
		mapDimensions = new Vector2 (gameParameters.mapSizeX, gameParameters.mapSizeY);

	}

	//load the appropriate terrain tiles for the chosen map type
	public GameObject[] LoadTerrainTiles(string terrainType){

		string directoryName = "TerrainTiles_" + terrainType;

		GameObject[] tilesToLoad = Resources.LoadAll<GameObject> ("Prefabs/" + directoryName); //might work?, mac OS vs. Windows with slash?

		return tilesToLoad;
	}

	//provide the coordinates of the neighbor tiles to a tile, if the coords are negative it is off the play field
	public List<Vector2> GatherCoordsOfTileNeighbors(Vector2 mainTileCoords){
		
		List<Vector2> coordsOfTileNeighbors = new List<Vector2> (); //there should be 8 neighbors

		//TODO finish function
		foreach(Vector2 coord in neighborTileAdditives){
			Vector2 neighborCoord = mainTileCoords + coord;
			coordsOfTileNeighbors.Add (neighborCoord);
		}

		return coordsOfTileNeighbors;
	}

	//get the appropriate tile from the terrainTile list, if not found null is returned
	public GameObject FindTerrainTile(string tileName){
		GameObject tileToReturn = null;
		foreach(GameObject tile in terrainTiles){
			if(tile.name.Equals(tileName)){
				tileToReturn = tile;
			}
		}
		return tileToReturn;
	}

	//determine the tile byte, based on the surrounding tiles
	public byte DetermineTileByte(int x, int y){
		byte startByte = 0x00000000; //0b_0000_0000

		byte outOfBoundsBit = 1;

		int sizeX = terrainBoolMap.GetLength (0); //get first array dimension length X
		int sizeY = terrainBoolMap.GetLength (1); //get second array dimension length Y

		//cases (coords from this tile)
		//from upper left to lower right
		// x-1, y+1  NW
		try{
			if(terrainBoolMap[x-1, y+1]){
				//bit 2^0 gets flipped
				startByte ^= (1 << 0);
			}
		}
		catch(Exception e){ //if the tile under consideration is out of bounds of the array then that byte place is 0
			startByte ^= (1 << 0);
		}

		// x, y+1 N
		try{
			if(terrainBoolMap[x, y+1]){
				//bit 2^1 gets flipped
				startByte ^= (1 << 1);
			}
		}
		catch(Exception e){
			startByte ^= (1 << 1);
		}

		// x+1, y+1 NE
		try{
			if(terrainBoolMap[x+1, y+1]){
				//bit 2^2 gets flipped
				startByte ^= (1 << 2);
			}
		}
		catch(Exception e){
			startByte ^= (1 << 2);
		}

		// x-1, y W
		try{
			if(terrainBoolMap[x-1, y]){
				//bit 2^3 gets flipped
				startByte ^= (1 << 3);
			}
		}
		catch(Exception e){
			startByte ^= (1 << 3);
		}

		// self

		// x+1, y E
		try{
			if(terrainBoolMap[x+1, y]){
				//bit 2^4 gets flipped
				startByte ^= (1 << 4);
			}
		}
		catch(Exception e){
			startByte ^= (1 << 4);
		}

		// x-1, y-1 SW
		try{
			if(terrainBoolMap[x-1, y-1]){
				//bit 2^5 gets flipped
				startByte ^= (1 << 5);
			}
		}
		catch(Exception e){
			startByte ^= (1 << 5);
		}

		// x, y-1 S
		try{
			if(terrainBoolMap[x, y-1]){
				//bit 2^6 gets flipped
				startByte ^= (1 << 6);
			}
		}
		catch(Exception e){
			startByte ^= (1 << 6);
		}

		// x+1, y-1 SE
		try{
			if(terrainBoolMap[x+1, y-1]){
				//bit 2^7 gets flipped
				startByte ^= (1 << 7);
			}
		}
		catch(Exception e){
			startByte ^= (1 << 7);
		}

		return startByte;
	}

	//choose what the actual tile will be in the terrain tile array
	public string DetermineTileType(byte tileByte){

		string chosenTile = "";

		//isolated tile
		if(	tileByte == 0 ||
			tileByte == 1 ||
			tileByte == 4 ||
			tileByte == 5 ||
			tileByte == 32 ||
			tileByte == 33 ||
			tileByte == 36 ||
			tileByte == 37 ||
			tileByte == 128 ||
			tileByte == 129 ||
			tileByte == 132 ||
			tileByte == 133 ||
			tileByte == 160 ||
			tileByte == 161 ||
			tileByte == 164 ||
			tileByte == 165){
			chosenTile = "isolatedPiece";
		}

		//perpendicular join with N
		if(	tileByte == 2 ||
			tileByte == 3 ||
			tileByte == 6 ||
			tileByte == 7 ||
			tileByte == 34 ||
			tileByte == 35 ||
			tileByte == 38 ||
			tileByte == 39 ||
			tileByte == 130 ||
			tileByte == 131 ||
			tileByte == 134 ||
			tileByte == 135 ||
			tileByte == 162 ||
			tileByte == 163 ||
			tileByte == 166 ||
			tileByte == 167){
			chosenTile = "perpendicularJoinWithN";
		}

		//perpendicular join with E
		if(	tileByte == 16 ||
			tileByte == 17 ||
			tileByte == 20 ||
			tileByte == 21 ||
			tileByte == 48 ||
			tileByte == 49 ||
			tileByte == 52 ||
			tileByte == 53 ||
			tileByte == 144 ||
			tileByte == 145 ||
			tileByte == 148 ||
			tileByte == 149 ||
			tileByte == 176 ||
			tileByte == 177 ||
			tileByte == 180 ||
			tileByte == 181){
			chosenTile = "perpendicularJoinWithE";
		}

		//perpendicular join with S
		if(	tileByte == 64 ||
			tileByte == 65 ||
			tileByte == 68 ||
			tileByte == 69 ||
			tileByte == 96 ||
			tileByte == 97 ||
			tileByte == 100 ||
			tileByte == 101 ||
			tileByte == 192 ||
			tileByte == 193 ||
			tileByte == 196 ||
			tileByte == 197 ||
			tileByte == 224 ||
			tileByte == 225 ||
			tileByte == 228 ||
			tileByte == 229){
			chosenTile = "perpendicularJoinWithS";
		}

		//perpendicular join with W
		if( tileByte == 8 ||
			tileByte == 9 ||
			tileByte == 12 ||
			tileByte == 13 ||
			tileByte == 40 ||
			tileByte == 41 ||
			tileByte == 44 ||
			tileByte == 45 ||
			tileByte == 136 ||
			tileByte == 137 ||
			tileByte == 140 ||
			tileByte == 141 ||
			tileByte == 168 ||
			tileByte == 169 ||
			tileByte == 172 ||
			tileByte == 173){
			chosenTile = "perpendicularJoinWithW";
		}

		//perpendicular corner with N & E
		if( tileByte == 18 ||
			tileByte == 19 ||
			tileByte == 50 ||
			tileByte == 51 ||
			tileByte == 146 ||
			tileByte == 147 ||
			tileByte == 178 ||
			tileByte == 179){
			chosenTile = "perpendicularCornerWithN&E";
		}

		//perpendicular corner with N & W
		if(	tileByte == 10 ||
			tileByte == 14 ||
			tileByte == 42 ||
			tileByte == 46 ||
			tileByte == 138 ||
			tileByte == 142 ||
			tileByte == 170 ||
			tileByte == 174){
			chosenTile = "perpendicularCornerWithN&W";
		}

		//perpendicular corner with S & E
		if(	tileByte == 80 ||
			tileByte == 81 ||
			tileByte == 84 ||
			tileByte == 85 ||
			tileByte == 112 ||
			tileByte == 113 ||
			tileByte == 116 ||
			tileByte == 117){
			chosenTile = "perpendicularCornerWithS&E";
		}

		//perpendicular corner with S & W
		if(	tileByte == 72 ||
			tileByte == 73 ||
			tileByte == 76 ||
			tileByte == 77 ||
			tileByte == 200 ||
			tileByte == 201 ||
			tileByte == 204 ||
			tileByte == 205){
			chosenTile = "perpendicularCornerWithS&W";
		}

		//straight through N to S
		if(	tileByte == 66 ||
			tileByte == 67 ||
			tileByte == 70 ||
			tileByte == 71 ||
			tileByte == 98 ||
			tileByte == 99 ||
			tileByte == 102 ||
			tileByte == 103 ||
			tileByte == 194 ||
			tileByte == 195 ||
			tileByte == 198 ||
			tileByte == 199 ||
			tileByte == 226 ||
			tileByte == 227 ||
			tileByte == 230 ||
			tileByte == 231){
			chosenTile = "straightThroughNtoS";
		}

		//straight through W to E
		if(	tileByte == 24 ||
			tileByte == 25 ||
			tileByte == 28 ||
			tileByte == 29 ||
			tileByte == 56 ||
			tileByte == 57 ||
			tileByte == 60 ||
			tileByte == 61 ||
			tileByte == 152 ||
			tileByte == 153 ||
			tileByte == 156 ||
			tileByte == 157 ||
			tileByte == 184 ||
			tileByte == 185 ||
			tileByte == 188 ||
			tileByte == 189){
			chosenTile = "straightThroughWtoE";
		}

		//corner with N & E
		if(	tileByte == 22 ||
			tileByte == 23 ||
			tileByte == 54 ||
			tileByte == 55 ||
			tileByte == 150 ||
			tileByte == 151 ||
			tileByte == 182 ||
			tileByte == 183){
			chosenTile = "cornerWithN&E";
		}

		//corner with N & W
		if(	tileByte == 11 ||
			tileByte == 15 ||
			tileByte == 43 ||
			tileByte == 47 ||
			tileByte == 139 ||
			tileByte == 143 ||
			tileByte == 171 ||
			tileByte == 175){
			chosenTile = "cornerWithN&W";
		}

		//corner with S & E
		if( tileByte == 208 ||
			tileByte == 209 ||
			tileByte == 212 ||
			tileByte == 213 ||
			tileByte == 240 ||
			tileByte == 241 ||
			tileByte == 244 ||
			tileByte == 245){
			chosenTile = "cornerWithS&E";
		}

		//corner with S & W
		if( tileByte == 104 ||
			tileByte == 105 ||
			tileByte == 108 ||
			tileByte == 109 ||
			tileByte == 232 ||
			tileByte == 233 ||
			tileByte == 236 ||
			tileByte == 237){
			chosenTile = "cornerWithS&W";
		}

		//three perpendicular join with N, W, E
		if(	tileByte == 26 ||
			tileByte == 58 ||
			tileByte == 154 ||
			tileByte == 186){
			chosenTile = "threePerpendicularJoinWithNWE";
		}

		//three perpendicular join with S, W, E
		if(	tileByte == 88 ||
			tileByte == 89 ||
			tileByte == 92 ||
			tileByte == 93){
			chosenTile = "threePerpendicularJoinWithSWE";
		}

		//three perpendicular join with W, N, S
		if(	tileByte == 74 ||
			tileByte == 78 ||
			tileByte == 202 ||
			tileByte == 206){
			chosenTile = "threePerpendicularJoinWithWNS";
		}

		//three perpendicular join with E, N, S
		if(	tileByte == 82 ||
			tileByte == 83 ||
			tileByte == 114 ||
			tileByte == 115){
			chosenTile = "threePerpendicularJoinWithENS";
		}

		//N, W corner with perpendicular join on W, N, S
		if(	tileByte == 75 ||
			tileByte == 79 ||
			tileByte == 203 ||
			tileByte == 207){
			chosenTile = "NWcornerWithPerpendicularJoinOnWNS";
		}

		//S, W, corner with perpendicular join on W, N, S
		if(	tileByte == 106 ||
			tileByte == 110 ||
			tileByte == 234 ||
			tileByte == 238){
			chosenTile = "SWcornerWithPerpendicularJoinOnWNS";
		}

		//N, E corner with perpendicular join on E, N, S
		if(	tileByte == 86 ||
			tileByte == 87 ||
			tileByte == 118 ||
			tileByte == 119){
			chosenTile = "NEcornerWithPerpendicularJoinOnENS";
		}

		//S, E corner with perpendicular join on E, N, S
		if(	tileByte == 210 ||
			tileByte == 211 ||
			tileByte == 242 ||
			tileByte == 243){
			chosenTile = "SEcornerWithPerpendicularJoinOnENS";
		}

		//N, E corner with perpendicular join on N, W, E
		if(	tileByte == 27 ||
			tileByte == 59 ||
			tileByte == 155 ||
			tileByte == 187){
			chosenTile = "NEcornerWithPerpendicularJoinOnNWE";
		}

		//N, W corner with perpendicular join on N, W, E
		if(	tileByte == 30 ||
			tileByte == 62 ||
			tileByte == 158 ||
			tileByte == 190){
			chosenTile = "NWcornerWithPerpendicularJoinOnNWE";
		}

		//S, E corner with perpendicular join on S, W, E
		if(	tileByte == 216 ||
			tileByte == 217 ||
			tileByte == 220 ||
			tileByte == 221){
			chosenTile = "SEcornerWithPerpendicularJoinOnSWE";
		}

		//S, W corner with perpendicular join on S, W, E
		if(	tileByte == 120 ||
			tileByte == 121 ||
			tileByte == 124 ||
			tileByte == 125){
			chosenTile = "SWcornerWithPerpendicularJoinOnSWE";
		}

		//three way join N, W, E
		if(	tileByte == 31 ||
			tileByte == 63 ||
			tileByte == 159 ||
			tileByte == 191){
			chosenTile = "threeWayJoinNWE";
		}

		//three way join N, S, E
		if(	tileByte == 214 ||
			tileByte == 215 ||
			tileByte == 246 ||
			tileByte == 247){
			chosenTile = "threeWayJoinNSE";
		}

		//three way join S, W, E
		if(	tileByte == 248 ||
			tileByte == 249 ||
			tileByte == 252 ||
			tileByte == 253){
			chosenTile = "threeWayJoinSWE";
		}

		//three way join N, W, S
		if(	tileByte == 107 ||
			tileByte == 111 ||
			tileByte == 235 ||
			tileByte == 239){
			chosenTile = "threeWayJoinNWS";
		}

		//four way perpendicular join with and without corner

		if(tileByte == 90){
			chosenTile = "fourWayPerpendicularJoin";
		}

		if(tileByte == 91){
			chosenTile = "fourWayJoinWithNWcorner";
		}

		if(tileByte == 94){
			chosenTile = "fourWayJoinWithNEcorner";
		}

		if(tileByte == 95){
			chosenTile = "fourWayJoinWithNW&NEcorner";
		}

		if(tileByte == 122){
			chosenTile = "fourWayJoinWithSWcorner";
		}

		if(tileByte == 123){
			chosenTile = "fourWayJoinWithNW&SWcorner";
		}

		if(tileByte == 126){
			chosenTile = "fourWayJoinWithNE&SWcorner";
		}

		if(tileByte == 127){
			chosenTile = "fourWayJoinWithNW&NE&SWcorner";
		}

		if(tileByte == 218){
			chosenTile = "fourWayJoinWithSEcorner";
		}

		if(tileByte == 219){
			chosenTile = "fourWayJoinWithNW&SEcorner";
		}

		if(tileByte == 222){
			chosenTile = "fourWayJoinWithNE&SEcorner";
		}

		if(tileByte == 223){
			chosenTile = "fourWayJoinWithNW&NE&SEcorner";
		}

		if(tileByte == 250){
			chosenTile = "fourWayJoinWithSW&SEcorner";
		}

		if(tileByte == 251){
			chosenTile = "fourWayJoinWithNW&SW&SWcorner";
		}

		if(tileByte == 254){
			chosenTile = "fourWayJoinWithNE&SW&SEcorner";
		}

		if(tileByte == 255){
			chosenTile = "covered";
		}

		return chosenTile;
	}
}
