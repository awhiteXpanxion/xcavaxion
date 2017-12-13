using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Specialized;

public class AIController : PlayerController {

	//increasing AI mass on rigid body 2d to 1000 keeps them from being jittery

	public int difficultyLevel; // 1 to 5? easiest to hardest? what does it mean?

	private Rigidbody2D cpuRigidBody;
	private BoxCollider2D cpuCollider2d;

	public float timeBetweenMove;
	private float timeBetweenMoveCounter;
	public float timeToMove;
	private float timeToMoveCounter;

	private Vector3 moveDirection;

	public List<GameObject> allGameItems;
	public GameObject[] _availableBoulders;
	public string[] _availableItems;
	public GameObject[] _availableElementBoxes;
	public string _availableCodeNuggets;

	public GameObject teamBaseReference;
	public Inventory teamBaseInventoryReference;

	public string[] _aiPriorities = {
		"value",	// 0
		"variety",	// 1
		"rarity",	// 2
		"amount",	// 3
		"carbon",	// 4
		"copper",	// 5
		"gold",		// 6
		"helium",	// 7
		"iron",		// 8
		"lead",		// 9
		"silver",	// 10
		"uranium",	// 11
		"water"		// 12
	};

	public GameItems allPossibleItems;
	public List<string> elementNames;
		
	public GameObject[,] _gameMap;

	System.Random rand = new System.Random();

	public bool moveFinished;
	public Vector3 moveToPosition;
	public Transform placeToMove;

	public Vector2 position1;
	public Vector2 position2;

	public Vector2 upRight = new Vector2 (1f, 1f);
	public Vector2 downRight = new Vector2 (1f, -1f);
	public Vector2 downLeft = new Vector2 (-1f, -1f);
	public Vector2 upLeft = new Vector2 (-1f, 1f);

	public bool movingLeft;
	public bool movingRight;
	public bool movingUp;
	public bool movingDown;

	public int priorityChoice;
	public bool chosen; //so the priority is chosen only once;

	// Use this for initialization
	public override void Start () {

		base.Start (); //run the PlayerController start function
		cpuRigidBody = GetComponent<Rigidbody2D> ();
		cpuCollider2d = GetComponent<BoxCollider2D> ();

		timeBetweenMoveCounter = timeBetweenMove;
		timeToMoveCounter = timeToMove;

		_gameMap = GameObject.Find ("MapController").GetComponent<MapController> ()._gameMap;

		moveFinished = true;
		playerMoving = false;
		moveToPosition = Vector3.zero;

		position1 = transform.position;

		movingLeft = false;
		movingRight = false;
		movingUp = false;
		movingDown = false;

		//teamBaseReference = PeekAtBase ();
		//teamBaseInventoryReference = teamBaseReference.GetComponent<BaseController>().baseInventory;

		allPossibleItems = GameObject.FindWithTag ("All Game Items").GetComponent<GameItems>();
		elementNames = allPossibleItems.allGameItems.GetListOfElementNames ();

		priorityChoice = rand.Next(_aiPriorities.Length);
		chosen = true;

		CPUPlayer = true;

		//start the AI player on one of the not occupied bases, handled in the BaseController
	}

	// Update is called once per frame
	public override void Update () {
		CPUPlayer = true;

		GetItemState();

//		bool check = LookAheadWithRaycast (); //check for obstacles to avoid
//		if(!check){
//			AIMovementSelector(); //if there's nothing to hit, do the standard move
//		}
		AIMovementSelector();
		MoveToPoint (moveToPosition);

		CPUSpriteFlip();

		teamBaseReference = PeekAtBase ();

		teamBaseInventoryReference = teamBaseReference.GetComponent<BaseController>().baseInventory;

		if(Time.frameCount % 2 == 0){
			position2 = transform.position;
			position1 = CheckMovementDirection ();
		}

		if(droppedOffAtBase){
			chosen = false;
		}						
	}

	public void FixedUpdate (){

	}

	public bool LookAheadWithRaycast(){
		bool objectFound = false;

		//raycast the way the character is facing to look for potential collisions
		Vector2 facingDirection = GetFacingDirection();
		RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection);

		if(hit != null){
//			if(hit.collider.gameObject.name.Equals(this.gameObject.name)){
//				objectFound = false;
//				return objectFound;
//			}
			if(hit.collider.gameObject.name.Contains("Player")){ //raycast is hiting the player's own collision box.
				AvoidanceManuever (facingDirection);
				objectFound = true;
			}
		}
		return objectFound;
	}

	public void AvoidanceManuever(Vector2 facingDirection){
		int choice = CoinFlip (); //random left or right
		Vector2 avoidanceVector = Vector2.zero;

		//look at facing direction and choose a left or right direction relative to that	
		if (facingDirection.Equals (Vector2.up)) {
			if (choice == 1) {
				avoidanceVector = Vector2.left;
			} else {
				avoidanceVector = Vector2.right;
			}
		} else if (facingDirection.Equals (upRight)) {
			if (choice == 1) {
				avoidanceVector = upLeft;
			} else {
				avoidanceVector = downRight;
			}
		} else if (facingDirection.Equals (Vector2.right)) {
			if (choice == 1) {
				avoidanceVector = Vector2.up;
			} else {
				avoidanceVector = Vector2.down;
			}
		} else if (facingDirection.Equals (downRight)) {
			if (choice == 1) {
				avoidanceVector = upRight;
			} else {
				avoidanceVector = downLeft;
			}
		} else if (facingDirection.Equals (Vector2.down)) {
			if (choice == 1) {
				avoidanceVector = Vector2.right;
			} else {
				avoidanceVector = Vector2.left;
			}
		} else if (facingDirection.Equals (downLeft)) {
			if (choice == 1) {
				avoidanceVector = downRight;
			} else {
				avoidanceVector = upLeft;
			}
		} else if (facingDirection.Equals (Vector2.left)) {
			if (choice == 1) {
				avoidanceVector = Vector2.down;
			} else {
				avoidanceVector = Vector2.up;
			}
		} else if (facingDirection.Equals (upLeft)) {
			if (choice == 1) {
				avoidanceVector = downLeft;
			} else {
				avoidanceVector = upRight;
			}
		}
		Vector3 currentPosition = transform.position; 

		//make that the move position
		moveToPosition = new Vector3(currentPosition.x + avoidanceVector.x, currentPosition.y + avoidanceVector.y, currentPosition.z);
	}

	public GameObject PeekAtBase(){
		string teamColor = this.teamColor;
		GameObject teamBase = GameObject.Find (Capitalize(teamColor) + "Base(Clone)");
		return teamBase;
	}

	public Vector2 CheckMovementDirection(){

		float diffX = 0f;
		float diffY = 0f;

		//take care of screen positions in the negative here
		if(position1.x < 0 && position2.x < 0){
			diffX = Math.Abs (position1.x) - Math.Abs (position2.x);
		}
		else{
			diffX = Math.Abs (position2.x) - Math.Abs (position1.x);
		}

		if(position1.y < 0 && position2.y < 0){
			diffY = Math.Abs (position1.y) - Math.Abs (position2.y);
		}
		else{
			diffY = Math.Abs (position2.y) - Math.Abs (position1.y);
		}
				
		Vector2 difference = new Vector2 (diffX, diffY);

		if(difference.y > 0){
			movingUp = true;
			movingDown = false;
		}
		if(difference.y < 0){
			movingDown = true;
			movingUp = false;
		}

		if(difference.x > 0){
			movingRight = true;
			movingLeft = false;
		}
		if(difference.x < 0){
			movingLeft = true;
			movingRight = false;
		}			
		return transform.position; //pass on the current position to the next check
	}

	public Vector2 GetFacingDirection(){
		Vector2 toReturn = Vector2.zero; //will this cause problems?
		if(movingUp){
			toReturn = Vector2.up;
		}
		if(movingRight){
			toReturn = Vector2.right;
		}
		if(movingDown){
			toReturn = Vector2.down;
		}
		if(movingLeft){
			toReturn = Vector2.left;
		}

		if(movingUp && movingLeft){
			toReturn = upLeft;
		}
		if(movingDown && movingLeft){
			toReturn = downLeft;
		}
		if(movingDown && movingRight){
			toReturn = downRight;
		}
		if(movingUp && movingRight){
			toReturn = upLeft;
		}
		return toReturn;
	}

	public void CPUSpriteFlip(){
		if (movingLeft){
			rend.flipX = true;;
		}
		if(movingRight){
			rend.flipX = false;
		}
	}

	public override void OnCollisionEnter2D(Collision2D other){
		base.OnCollisionEnter2D (other);
	}

	public void AIMovementSelector(){
		switch (difficultyLevel) 
		{
		case 0: //crazy AI
			CrazyRandomMovement ();
			break;
		case 1: //dumb AI
			//simple random movement, returns to base when elements are full
			SensibleRandomMovement ();
			break;
		case 2: //easy AI
			//seeks nearest boulder, and nearest elementbox, returns to base when full
			EasyAIMovement();
			break;
		case 3: //average AI
			//seeks nearest boulder, randombly prioritizes certain elementboxes, returns to base when full
			if(droppedOffAtBase && !chosen){
				priorityChoice = ChooseRandomPriority ();
			}
			BetterAIMovement(_aiPriorities[priorityChoice]);
			break;
		case 4: //smart AI
			//seeks nearest boulder, searches for specific elementboxes, grabs items, returns to base when element needs are met
			//something
			break;
		case 5: //very smart AI
			//objective based: only chooses boulders that don't let other teams in area, searches for specific elements, and items, grabs code nuggets, returns to base when objectives met
			//something
			break;
		default:
			SensibleRandomMovement ();
			break;
		}
	}

	public int ChooseRandomPriority(){
		int choice = rand.Next (_aiPriorities.Length);
		chosen = true;
		droppedOffAtBase = false;
		return choice;
	}

	public bool CheckTimePassed(float timeToWait){
		float delta = 0.0f;
		while(timeToWait >= delta){
			delta += Time.deltaTime;
		}
		return true;
	} 

	//this moves at too fast a speed to the destination, MoveToPoint is better
	public void MoveDistance(float amountX, float amountY){
		Vector2 newPosition = new Vector2 (transform.position.x + amountX, transform.position.y + amountY);
		cpuRigidBody.MovePosition (newPosition + cpuRigidBody.velocity * Time.fixedDeltaTime);
	}

	public void MoveToPoint(Vector3 pointToMoveTo){
		transform.position = Vector2.MoveTowards (new Vector2 (transform.position.x, transform.position.y), 
			new Vector2 (pointToMoveTo.x, pointToMoveTo.y), playerSpeed * Time.deltaTime);
		moveFinished = false;
	}

	public GameObject ChooseElementPriority(string priority){
		GameObject prioritizedElementBox = null;

		switch(priority)
		{
		case "value":
			//prioritize elements based on their value
			prioritizedElementBox = PrioritizeValue();
			break;
		case "rarity":
			//prioritize elements based on their rarity
			prioritizedElementBox = PrioritizeRarity();
			break;
		case "amount":
			//prioritize elements based on the one of least amount you have
			prioritizedElementBox = PrioritizeAmount();
			break;
		case "variety":
			//prioritize getting a container of each element
			prioritizedElementBox = PrioritizeVariety();
			break;
		default:
			//prioritize getting a single element
			prioritizedElementBox = PrioritizeSingleElement (priority);
			break;
		}
		return prioritizedElementBox;
	}

	public GameObject PrioritizeValue(){
		print ("prioritizing element value");
		int highestValue = 0;
		GameObject mostValuable = null;
		foreach(GameObject box in _availableElementBoxes){
			int value = box.GetComponent<ElementBox> ().container.contents.valueByWeight;
			if(value > highestValue){
				highestValue = value;
				mostValuable = box;
			}
		}
		return mostValuable;
	}

	//rarity --> prevalence in "Elements" scale of 1..5, 1 being the rarest
	public GameObject PrioritizeRarity(){
		print ("prioritizing element rarity");
		int highestRarity = 10;
		GameObject rarest = null;
		foreach(GameObject box in _availableElementBoxes){
			int rarity = box.GetComponent<ElementBox> ().container.contents.prevalence;
			if(rarity < highestRarity){
				highestRarity = rarity;
				rarest = box;
			}
		}
		return rarest;
	}

	public GameObject PrioritizeAmount(){
		print ("prioritizing element amount");
		//find element with least amount in base inventory
		int leastAmount = int.MaxValue;
		string elementToGet = "";
//		Inventory baseInventory = teamBaseReference.GetComponent<BaseController>().baseInventory;
		foreach(ElementContainer cont in teamBaseInventoryReference.elementsInventory){
			int currentAmount = cont.volume;
			if(currentAmount < leastAmount){
				elementToGet = cont.contents.name;
			}
		}
		return PrioritizeSingleElement (elementToGet);
	}

	//TODO this needs to be fixed
	public GameObject PrioritizeVariety(){
		print ("prioritizing element variety");
		string elementToGet = CheckMissingElements();

		if(elementToGet == null){ //if there are no other elements to fill in
			int choice = rand.Next(4, _aiPriorities.Length);
			return ChooseElementPriority (_aiPriorities[choice]); //choose random element to grab
			print("No more variety, choosing random element.");
		}
		return PrioritizeSingleElement (CheckMissingElements());
	}

	public string CheckMissingElements(){
		List<string> inventoryElements = new List<string> ();
//		Inventory baseInventory = teamBaseReference.GetComponent<BaseController> ().baseInventory;
		foreach(ElementContainer cont in teamBaseInventoryReference.elementsInventory){
			inventoryElements.Add (cont.contents.name);
		}
		List<string> difference = inventoryElements.Except (elementNames).ToList ();
		if(difference.Count() == 0){
			return null; //no difference
		}
		return difference [0]; //grab the first one
	}

	public GameObject PrioritizeSingleElement(string elementName){
		print ("prioritizing " + elementName);
//		GameObject matchingName = null;
		List<GameObject> elementsOnScreen = new List<GameObject> ();
		foreach(GameObject box in _availableElementBoxes){
			string name = box.GetComponent<ElementBox> ().container.contents.name;
			if(elementName.Equals(name.ToLower())){
				elementsOnScreen.Add (box);
			}
		}
		return ChooseClosest(elementsOnScreen);
	}

	public Vector2 FindBoulder(){
		return ChooseClosest (_availableBoulders).transform.position;
	}

	public void BetterAIMovement(string elementPriority){
		//get status of screen items
		bool elementBoxesPresent = _availableElementBoxes.Length != 0;
		bool bouldersPresent = _availableBoulders.Length != 0;
		bool elementsFull = inventory.elementsFull;

		//screen states
		bool nothingOnScreen 	= !elementBoxesPresent && !bouldersPresent;
		bool justBoulders 		= bouldersPresent && !elementBoxesPresent;
		bool bouldersAndBoxes 	= elementBoxesPresent && bouldersPresent; 
		bool justBoxes 			= !bouldersPresent && elementBoxesPresent;
			 
		if(justBoulders){
			moveToPosition = FindBoulder();
		}
		if(bouldersAndBoxes){
			GameObject boxPriority = ChooseElementPriority (elementPriority);

			if(boxPriority == null){
				moveToPosition = FindBoulder ();
			}
			else{
				moveToPosition = boxPriority.transform.position;
			}
		}
		if(justBoxes){
			GameObject boxPriority = ChooseElementPriority (elementPriority);

			if(boxPriority == null){
				//boxPriority = ChooseElementPriority (_aiPriorities[ChooseRandomPriority ()]); //change priority if previous one is null
				priorityChoice = ChooseRandomPriority();
			}
			else{
				moveToPosition = boxPriority.transform.position;
			}
		}
		if(elementsFull || nothingOnScreen){
			moveToPosition = basePosition;
		}
		//MoveToPoint (moveToPosition); //execute the move
	}

	public void EasyAIMovement(){	 
		GameObject[] _allStuff = new GameObject[_availableBoulders.Length + _availableElementBoxes.Length];
		Array.Copy (_availableBoulders, _allStuff, _availableBoulders.Length);
		Array.Copy (_availableElementBoxes, 0, _allStuff, _availableBoulders.Length, _availableElementBoxes.Length);

		//if move finished choose new destination
		if(_allStuff.Length != 0 && !inventory.elementsFull){
			//look for the nearest thing
			GameObject nearestThing = ChooseClosest(_allStuff);
			moveToPosition = nearestThing.transform.position;
		}

		//TODO put in > 900 condition to address the situation where the AI stops picking up boxes, may be an issue with inventory system
		if(inventory.elementsFull || _allStuff.Length == 0 || (inventory.currentTotalElementVolume > 900)){
			moveToPosition = basePosition;
		}
		//execute move, has to be done in update continuously
		//MoveToPoint (moveToPosition);
	}

	public void SensibleRandomMovement(){
		if(inventory.elementsFull){
			MoveToPoint (basePosition);
			playerMoving = false;
		}

		if(playerMoving){
			timeToMoveCounter -= Time.deltaTime;
			cpuRigidBody.velocity = moveDirection;

			if(timeToMoveCounter < 0f){
				playerMoving = false;
				timeBetweenMoveCounter = timeBetweenMove;
			}
		}
		else{
			timeBetweenMoveCounter -= Time.deltaTime;
			cpuRigidBody.velocity = Vector2.zero;
			if(timeBetweenMoveCounter < 0.0f){
				playerMoving = true;
				timeToMoveCounter = timeToMove;
				moveDirection = new Vector3 (UnityEngine.Random.Range (-1f, 1f) * playerSpeed, UnityEngine.Random.Range (-1f, 1f) * playerSpeed, 0f);
			}
		}
	}

	public void CrazyRandomMovement(){
		//choose a random direction
		int direction = CoinFlip(); //pick horizontal or vertical movement

		//vertical
		if(direction == 0){
			//move a small random amount in that direction, random positive or negative

			MoveDistance(0.0f, 0.1f * DirectionChange());
			playerMoving = true;
		}

		//horizontal
		if(direction == 1){
			//move a small random amount in that direction, random positive or negative
			MoveDistance(0.1f * DirectionChange(), 0.0f);
			playerMoving = true;
		}
	}

	public void GetCurrentMapState(){
		_gameMap = GameObject.Find ("MapController").GetComponent<MapController> ()._gameMap;
		//FindAllBoulders();
		//FindAllElementBoxes ();
	}

	public void GetItemState(){
		FindAllBoulders();
		FindAllElementBoxes ();
	}
		
	//TODO fix this, as of right now when there are a lot of boulders this checks every boulder for distance, VERY SLOW
	public GameObject ChooseClosest(GameObject[] arrayOfObjects){
		if(arrayOfObjects.Length == 1){
			return arrayOfObjects [0];
		}

		Vector3 currentPosition = transform.position;
		float shortestDistance = float.MaxValue;
		GameObject closest = null;
		foreach(GameObject obj in arrayOfObjects){
			if(obj != null){
				float difference = Vector3.Distance(currentPosition, obj.transform.position);
				if(difference < shortestDistance){
					shortestDistance = difference;
					if(obj != null){
						closest = obj;
					}
				}
			}
		}
		return closest;
	}

	public GameObject ChooseClosest(List<GameObject> listOfObjects){
		if(listOfObjects.Count() == 1){ //if there is just one thing, skip all this other stuff
			return listOfObjects [0];
		}

		Vector3 currentPosition = transform.position;
		float shortestDistance = float.MaxValue;
		GameObject closest = null;

		foreach(GameObject obj in listOfObjects){
			if(obj != null){
				float difference = Vector3.Distance(currentPosition, obj.transform.position);
				if(difference < shortestDistance){
					shortestDistance = difference;
					if(obj != null){
						closest = obj;
					}
				}
			}
		}
		return closest;
	}


	public GameObject CheckAllBouldersForClosest(){
		int mapX = _gameMap.GetLength (1);
		int mapY = _gameMap.GetLength (0);

		Vector3 currentPosition = transform.position;
		float shorestDistance = float.MaxValue;
		GameObject closest = null;
		for(int i = 0; i < mapY; i++){
			for(int j = 0; j < mapX; j++){
				float difference = Vector3.Distance (currentPosition, _gameMap [j, i].transform.position);
				if(difference < shorestDistance){
					shorestDistance = difference;
					closest = _gameMap[j,i];
				}
			}
		}
		return closest;
	}

	public void FindAllGameItems(){
		
	}

	public void FindAllBoulders(){
		_availableBoulders = GameObject.FindGameObjectsWithTag("Boulder");
	}

	public void FindAllItems(){

	}

	public void FindAllElementBoxes(){
		_availableElementBoxes = GameObject.FindGameObjectsWithTag ("ElementBox");
	}

	public void FindAllCodeNuggets(){
		
	}
		
	public int CoinFlip(){
		return rand.Next (2);
	}

	public int DirectionChange(){
		int direction = CoinFlip ();
		if(direction == 0){
			return -1;
		}
		else{
			return 1;
		}
	}

	public string Capitalize(string toCapitalize){
		return char.ToUpper(toCapitalize[0]) + toCapitalize.Substring(1);
	}

	public override void CreateFloatingTextMessage(string messageText, float customDestroyTime){
		//do nothing
		print(playerIdentifier + " AI create floating text message called!");
	}
}