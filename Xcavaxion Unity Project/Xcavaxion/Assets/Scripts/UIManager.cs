using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	//This script is attached to the canvas and handles the actual display of the UI elements

	public Queue<ActionMessage> currentActionMessages; //the current undisplayed messages
	public Queue<GameObject> onScreenActionMessages; //the current messages on the screen

	public int numberOfMessagesToKeep; //The number of messages that should be displayed on screen at the same time
	public float amountToMoveUpMessages;

	public GameObject messagePrefab;

	public UIMessageHandler handler;

	// Use this for initialization
	void Start () {
		currentActionMessages = new Queue<ActionMessage> ();
		onScreenActionMessages = new Queue<GameObject> ();

		//anything that's the UI is supposed to display right at the start
	}

	// Update is called once per frame
	void Update () {

		UpdateMessages ();

		if(!handler.DebugMessagesOn && onScreenActionMessages.Count > 0){ //if the messages have been turned of, remove the ones still on the screen
			foreach(GameObject message in onScreenActionMessages){
				RemoveMessageFromScreen ();
			}
		}
	}

	void UpdateMessages(){
		GatherUnreadMessages ();
		if(currentActionMessages.Count > 0){
			CycleThroughUnreadMessages ();
		}
	}

	void GatherUnreadMessages(){
		foreach(ActionMessage message in handler.actionMessages){
			if(message.unreadMessage){
				currentActionMessages.Enqueue (message);
			}
			message.unreadMessage = false;
		}
	}

	void CycleThroughUnreadMessages(){
		while(currentActionMessages.Count != 0){
			StackMessagesOnScreen ();
			DisplayMessageOnScreen (currentActionMessages.Dequeue());
		}
		foreach(ActionMessage message in currentActionMessages){
			StackMessagesOnScreen ();
			DisplayMessageOnScreen (message);
		}
	}

	void DisplayMessageOnScreen(ActionMessage message){
//		Debug.Log ("current message text: " + message.messageText);
		messagePrefab.GetComponent<Text> ().text = message.messageOwner + ": " + message.messageText;

		//Create a new on screen message object... on the screen
		var clone = (GameObject) Instantiate(messagePrefab, messagePrefab.transform.position, messagePrefab.transform.rotation);
		clone.transform.SetParent (this.transform, false);

		//Put a reference to it in the OnScreen queue
		onScreenActionMessages.Enqueue(clone);
	}

	void StackMessagesOnScreen(){
		//move all the preexisting messages up one to accomidate the new message
		foreach(GameObject actionMessage in onScreenActionMessages){
			Vector3 currentPosition = actionMessage.transform.position;
			currentPosition.y += amountToMoveUpMessages;
			actionMessage.transform.position = currentPosition;
		}

		//check to make sure the message stack is the right size, if not remove the oldest message
		if(onScreenActionMessages.Count >= numberOfMessagesToKeep){
			RemoveMessageFromScreen();
		}
	}

	void RemoveMessageFromScreen(){
		//remove from screen and from the queue
		Destroy(onScreenActionMessages.Dequeue());
	}
}