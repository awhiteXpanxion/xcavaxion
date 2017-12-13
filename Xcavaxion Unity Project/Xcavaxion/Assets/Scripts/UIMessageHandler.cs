using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageHandler : MonoBehaviour {

	//This script is attached to the UIMessageHandler GameObject and works with all the messages that need to be displayed on screen, behind the scenes

	public Queue<ActionMessage> actionMessages; //starting with message 0

	public int totalMessageCount; //number of messages for this session, used on the message as an identifier
	public int currentMessageCount; //the number of messages being stored in the queue
	public int messageLimit; //max number of message to keep in the queue just so it doesn't bog down, maintain message number though

	public bool DebugMessagesOn;

	// Use this for initialization
	void Start () {
		totalMessageCount = 0;
		actionMessages = new Queue<ActionMessage> ();

		DebugMessagesOn = false;
	}
	
	// Update is called once per frame
	void Update () {

		currentMessageCount = actionMessages.Count;

		if(currentMessageCount >= messageLimit){ //if we reach the message limit clear the messages out
			actionMessages.Clear ();
		}

	}

	public void AddMessage(ActionMessage toAdd){
		actionMessages.Enqueue (toAdd);
		totalMessageCount++;
	}

	public void CreateMessage(string messageText, string composer){
		ActionMessage newMessage = new ActionMessage (totalMessageCount, messageText, composer);
		AddMessage (newMessage);
	}

	public bool CheckForNewMessages(){
		bool newFound = false;
		foreach(ActionMessage message in actionMessages){
			if(message.unreadMessage){
				newFound = true;
			}
		}
		return newFound;
	}
}