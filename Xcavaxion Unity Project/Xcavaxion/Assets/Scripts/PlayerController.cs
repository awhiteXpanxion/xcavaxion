using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;
using UnityEngine.UI;
using System.CodeDom.Compiler;

public class PlayerController : MonoBehaviour {

	public string playerIdentifier;

	public Team playerTeam;
	public string teamColor;

	public float playerSpeed;

	public SpriteRenderer rend;

	private Rigidbody2D playerRigidBody;
	private BoxCollider2D playerCollider2d;

	public InventoryManager inventory; //inventory cannot be InventoryManager class because it is a monobehaviour

	public bool playerMoving;
	public bool droppedOffAtBase;

	public Vector3 mouseClickCoords;
	public bool mouseClicked;

	public Vector3 basePosition;

	public GameObject floatingMessage;
	public List<GameObject> floatingMessages;
	public int floatingMessageTotal;
	public float floatingMessagePositionCount; //a gap between new messages to avoid the colliding text

	public Canvas canvasReference;

	public ControlPanelController playerControlPanel;

	public UIMessageHandler messages;

	public bool CPUPlayer = false;

	public bool cpMousedOver;

	// Use this for initialization
	public virtual void Start () {
		basePosition = Vector3.zero;
		rend = GetComponent<SpriteRenderer> ();
		playerRigidBody = GetComponent<Rigidbody2D> ();
		playerCollider2d = GetComponent<BoxCollider2D> ();
		droppedOffAtBase = false;

		mouseClicked = false;

		floatingMessageTotal = 0;
		floatingMessagePositionCount = 160.0f;

		playerControlPanel = GameObject.FindGameObjectWithTag ("ControlPanel").GetComponent<ControlPanelController>(); //there should only be one
		cpMousedOver = false;
	}

	// Update is called once per frame
	public virtual void Update () {

		//TODO move this to its own function 
		//TODO restrict player movement, to tiles? to perpedicular axis? to movement inside tiles? to a set increment of distance?
		playerMoving = false;

		if(Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f){
			playerRigidBody.velocity = new Vector2 (Input.GetAxisRaw ("Horizontal") * playerSpeed, playerRigidBody.velocity.y);
			playerMoving = true;
		}
		if(Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -0.5f){
			playerRigidBody.velocity = new Vector2 (playerRigidBody.velocity.x, Input.GetAxisRaw ("Vertical") * playerSpeed);
			playerMoving = true;
		}
		if(Input.GetAxisRaw("Horizontal") < 0.5f && Input.GetAxisRaw("Horizontal") > -0.5f){
			playerRigidBody.velocity = new Vector2 (0f, playerRigidBody.velocity.y);
		}
		if(Input.GetAxisRaw("Vertical") < 0.5f && Input.GetAxisRaw("Vertical") > -0.5f){
			playerRigidBody.velocity = new Vector2 (playerRigidBody.velocity.x, 0f);
		}
		HorzontalSpriteFlip (playerRigidBody, rend);
		cpMousedOver = playerControlPanel.mousedOver; //this bool is flipped on floating alerts too for some reason

//		if(Input.GetMouseButtonDown(0) && !cpMousedOver){ //this doesn't really work
//			mouseClickCoords = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//			mouseClicked = true;
//
//		}

		if(Input.GetMouseButtonDown(0)){ //this doesn't really work
			mouseClickCoords = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mouseClicked = true;

		}

//		if(mouseClicked && !cpMousedOver){ //this doesn't really work
//			MoveToPoint (mouseClickCoords);
//			if(transform.position.Equals(mouseClickCoords)){
//				mouseClicked = false;
//			}
//		}

		if(mouseClicked){ //this doesn't really work
			MoveToPoint (mouseClickCoords);
			if(transform.position.Equals(mouseClickCoords)){
				mouseClicked = false;
			}
		}
			
	}

	public void MoveToPoint(Vector3 pointToMoveTo){
		transform.position = Vector2.MoveTowards (new Vector2 (transform.position.x, transform.position.y), 
			new Vector2 (pointToMoveTo.x, pointToMoveTo.y), playerSpeed * Time.deltaTime);
		//moveFinished = false;
	}

	public void HorzontalSpriteFlip(Rigidbody2D body2D, SpriteRenderer renderer){
		if(body2D.velocity.x < 0){
			renderer.flipX = true;
		}
		if(body2D.velocity.x > 0){
			renderer.flipX = false;
		}
	}

	public virtual void OnCollisionEnter2D(Collision2D collision2d){
		Debug.Log ("player collided with: " + collision2d.gameObject.name);

		//this is here in order to ignore collisions with the box colliders of the ground tiles
		if(collision2d.gameObject.layer == 8){
			Physics2D.IgnoreLayerCollision (8, 22, true);
		}
		//ignore element box collisions
		if(collision2d.gameObject.layer == 25){
			Physics2D.IgnoreLayerCollision (25, 22, true);
		}

		//ignore collisions with other players
		//ignore collisions only with other players on team?
		if(collision2d.gameObject.layer == 22){
			Physics2D.IgnoreLayerCollision (22, 22, true);
		}
	}

	public void UpdateElementBoxCount(int amount){
		GameObject mapCont = GameObject.Find ("MapController");
		mapCont.GetComponent<MapController> ().onScreenElementBoxCount += amount;
	}

	void OnTriggerEnter2D(Collider2D other){

		//this is for element boxes being traversible even when you can't grab them
		//if player triggers ElementBox, attempt to add ElementContainer to player's element inventory
		if(other.gameObject.layer == 25){ 

			//trying the separate inventory class here
			ElementContainer temp = other.gameObject.GetComponent<ElementBox> ().container;
			bool elementsAdded = inventory.GetComponent<InventoryManager> ().playerInventory.AddElementContainerToInventory (temp);

			if(elementsAdded){
				Destroy (other.gameObject); //remove the ElementBox after contents absorbed
				UpdateElementBoxCount(-1);
				droppedOffAtBase = false; //we are adding more so we haven't dropped off yet.
//				if(!CPUPlayer){
//					CreateFloatingTextMessage("+ " + temp.volume + " " + temp.contents.name.ToUpper()); //not sure if this works
//				}
				CreateFloatingTextMessage("+ " + temp.volume + " " + temp.contents.name.ToUpper(), 0.0f); //not sure if this works
			}
			else if(!elementsAdded){
				//if we can't add all try adding part of the container
				int partialAmount = inventory.GetComponent<InventoryManager> ().playerInventory.PartialElementAdd (temp);;
				//bool partialSuccess = inventory.GetComponent<InventoryManager> ().playerInventory.PartialElementAdd (temp, partialAmount);
				//don't destroy the container
				//if(partialSuccess){
				if(partialAmount != 0){
					droppedOffAtBase = false;
					CreateFloatingTextMessage("+ " + partialAmount + " " + temp.contents.name.ToUpper(), 0.0f); //not sure if this works
					print ("Addded a partial amount of an element!"); //checking for partial amount bug
				}
			}				
		}

		//if players enters base collision box, give all elements to the base
		if(other.gameObject.layer == 9) { //layer 9 is the layer for bases
			
			//If it's the base, bump the character up a couple of pixels to simulate going over a bump
			Vector3 currentPos = transform.position;
			currentPos.y += 0.03f;
			transform.position = currentPos;

			string touchedBaseTeam = other.gameObject.GetComponent<BaseController> ().baseColor;

			if(touchedBaseTeam.Equals(teamColor)){
				//Handle the depositing of items, giving all of them automatically right now
				//This is the base adding all of a copy of the players elements to it's inventory
				//throws and exception here upon placement of the character at start on base, presumably because everything is empty
				if(inventory.currentTotalElementVolume != 0 && !CPUPlayer){
					CreateFloatingTextMessage ("DROPPED OFF AT BASE!", 1.5f);
					floatingMessagePositionCount = 160.0f;
				}
				other.gameObject.GetComponent<BaseController> ().baseInventory.AddListOfElementContainer (
					inventory.playerInventory.elementsInventory);
				other.gameObject.GetComponent<BaseController> ().baseInventory.elementsUpdated = true;
				inventory.playerInventory.ClearInventory ();
				droppedOffAtBase = true;
			}
			else{
				messages.CreateMessage ("Not your team's base.", playerIdentifier);
				CreateFloatingTextMessage ("NOT YOUR TEAM'S BASE!", 1.5f);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if(other.gameObject.layer == 9){
			//Move the player back down from the bump
			Vector3 currentPos = transform.position;
			currentPos.y -= 0.03f;
			transform.position = currentPos;
		}
	}

	void OnMouseOver(){
		rend.material.color = new Color(0.95f, 0, 0, 1.0f);
	}

	void OnMouseExit(){
		rend.material.color = Color.white; //restores tile color
	}

	public virtual void CreateFloatingTextMessage(string messageText, float customDestroyTime){
		floatingMessage.GetComponent<FloatingAlertMessage> ().NewMessage (messageText);

		Vector3 curPos = transform.position;
		Vector3 messagePos = new Vector3 (curPos.x, curPos.y + 1.0f + floatingMessagePositionCount, curPos.z);
		var clone = (GameObject) Instantiate(floatingMessage, messagePos, transform.rotation);
		clone.transform.SetParent (canvasReference.transform, false);
		clone.GetComponent<FloatingAlertMessage> ().messageNumber = floatingMessageTotal;
		if(customDestroyTime != 0.0f){
			clone.GetComponent<FloatingAlertMessage> ().timeToDestroy = customDestroyTime;
		}
		floatingMessages.Add (clone); //add reference to created message in the list
		floatingMessageTotal++;
		floatingMessagePositionCount-= 20.0f;
		if(floatingMessagePositionCount < 0){ //for safety in case the number gets too low
			floatingMessagePositionCount = 160.0f;
		}
	}
}