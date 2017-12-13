using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMessage {

	//This class represents a message to be displayed on screen about an in game event that the player may need info about

	public int messageNumber; //numbers assigned to messages indicating when they were received, not sure if it'd be needed
	public string messageText;
	public string messageOwner;
	public bool unreadMessage;

	public ActionMessage(int number, string text, string owner){
		this.messageNumber = number;
		this.messageText = text;
		this.messageOwner = owner;
		this.unreadMessage = true;
	}
		
}