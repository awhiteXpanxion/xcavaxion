using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class Inventory {

	//A class representing an inventory object that can be used by players, devices, vehicles, bases, tc.
	//TODO and to and from identifiers to screen messages

	public int totalCurrency;

	public string playerName;

	public List<string> itemInventory;
	public List<ElementContainer> elementsInventory;
	public List<string> codeNuggetInventory;

	public bool itemsUpdated;
	public bool elementsUpdated;
	public bool codeNuggetsUpdated;

	public bool elementVolumeLimit;
	public int elementVolumeCapacity;
	public int currentTotalElementVolume;

	public GameObject messages;

	public bool DebugMessagesOn;

	public Inventory(bool elementLimit, int elementCapacity, string playerName){
		this.elementVolumeLimit = elementLimit;
		this.elementVolumeCapacity = elementCapacity;
		this.playerName = playerName;

		itemInventory = new List<string> ();
		elementsInventory = new List<ElementContainer> ();
		codeNuggetInventory = new List<string> ();

		elementsUpdated = false;

		messages = GameObject.FindGameObjectWithTag ("Messages");
	}

	public void SendMessage(string message){
		if(messages.GetComponent<UIMessageHandler>().DebugMessagesOn){
			messages.GetComponent<UIMessageHandler> ().CreateMessage (message, playerName);
		}
		Debug.Log (message);
	}

	public List<string> GetElementNames(){
		List<string> elementNames = new List<string> ();
		foreach(ElementContainer cont in elementsInventory){
			elementNames.Add (cont.contents.name);
		}
		return elementNames;
	}

	public List<int> GetElementVolumes(){
		List<int> elementVolumes = new List<int> ();
		foreach(ElementContainer cont in elementsInventory){
			elementVolumes.Add (cont.volume);
		}
		return elementVolumes;
	}

	public void ClearInventory(){
		elementsInventory.Clear ();
		currentTotalElementVolume = 0;
		elementsUpdated = true;
	}

	public void ToggleElementLimit(){
		elementVolumeLimit = !elementVolumeLimit;
	}

	public void ChangeElementCapacity(int newCapacity){
		elementVolumeCapacity = newCapacity;
	}

	//TODO when items and code nuggets are worked out this will have to be updated
	//This will check to see it a given item, element, or code nugget is already present in the appropriate list
	public bool InventoryContainsByName(string name, string type){
		bool found = false;
		if(type.Contains("item")){
			foreach(string item in itemInventory){
				if(item.Equals(name)){
					found = true;
				}
			}
		}
		if(type.Contains("element")){
			foreach(ElementContainer cont in elementsInventory){
				if(cont.contents.name.Equals(name)){
					found = true;
				}
			}
		}
		if(type.Contains("code") || type.Contains("nugget")){
			foreach(string code in codeNuggetInventory){
				if(code.Equals(name)){
					found = true;
				}
			}
		}
		return found;
	}
		
	/* 		CURRENCY 	*/

	public void AddCurrency(int amountToAdd){
		totalCurrency += amountToAdd;
	}

	public void RemoveCurrency(int amountToRemove){
		totalCurrency -= amountToRemove;
	}
		
	/*		ITEMS		*/

	//TODO once items are sorted out this needs to be updated
	//Counts the number of occurances of an item of the given name in the item inventory
	public int ItemTypeCount(string itemName){
		int count = 0;
		foreach(string item in itemInventory){
			if(itemName.Equals(item)){
				count++;
			}
		}
		return count;
	}

	//TODO once items are sorted out this needs to be updated
	public void AddItemToInventory(string item){
		itemInventory.Add (item);
		itemsUpdated = true;
	}

	//TODO once items are sorted out this needs to be updated
	//Removes the element of given name from the inventory
	//Assumes the element is present in the inventory
	//Removes the first instance of said item in list
	public string RemoveItemFromInventory(string itemName){
		string toReturn = null;
		foreach(string item in itemInventory){
			if(itemName.Equals(item)){
				toReturn = item;
			}
		}
		return toReturn;
	}
		
	/*		ELEMENTS		*/

	//Checks to see if the element with the given name is already present in the element inventory
	public bool CheckIfElementAlreadyContained(string elementName){
		bool elementContained = false;
		foreach(ElementContainer cont in elementsInventory){
			if(elementName.Equals(cont.contents.name)){
				elementContained = true;
			} 
		}
		return elementContained;
	}

	//returns true if there is volume overflow when adding elements to inventory
	public bool CheckForVolumeOverflow(int volumeToAdd){
		bool volumeOverflow = false;
		int newVolumeTotal = currentTotalElementVolume + volumeToAdd;
		if((newVolumeTotal > elementVolumeCapacity) && elementVolumeLimit){
			volumeOverflow = true;
		}
		return volumeOverflow;
	}

	//Adds a given volume of an element to an existing container in the inventory
	public bool AddElementVolume(string elementName, int volumeToAdd){
		bool successfullyAdded = false;
		foreach(ElementContainer cont in elementsInventory){
			if(elementName.Equals(cont.contents.name)){
				if(!CheckForVolumeOverflow(volumeToAdd)){ //if there is no overflow add amount to existing container
					cont.volume += volumeToAdd; //add the amount to preexisting element container
					currentTotalElementVolume += volumeToAdd;
					successfullyAdded = true;
//					Debug.Log ("Added " + volumeToAdd + " of " + elementName + " to preexisting element container in player inventory.");
					SendMessage("Added " + volumeToAdd + " " + elementName.ToUpper() + " to preexisting element container in inventory.");
				}
				else{
//					Debug.Log ("Could not add " + volumeToAdd + " of " + elementName + " to presixitng inventory because of overflow.");
//					SendMessage("Could not add " + volumeToAdd + " of " + elementName + " to presixitng inventory because of overflow.");
				}
			}
		}
		return successfullyAdded;
	}

	//Adds an entire element container, all of it's volume, to the inventory
	public bool AddElementContainerToInventory(ElementContainer elementContainerToAdd){
		bool successfullyAdded = false;
		string toAddName = elementContainerToAdd.contents.name;
		int volumeToAdd = elementContainerToAdd.volume;
		if (CheckIfElementAlreadyContained (toAddName)) { //if element container already in player inventory
			successfullyAdded = AddElementVolume (toAddName, volumeToAdd); //volume overflow check within this function
		} 
		else { //if the element container is not in the player inventory
			if(!CheckForVolumeOverflow(volumeToAdd)){
				elementsInventory.Add (elementContainerToAdd);
				currentTotalElementVolume += volumeToAdd;
//				Debug.Log ("Added element container of: " + volumeToAdd + " " + toAddName + " to the player inventory elements");
				SendMessage("Added element container of: " + volumeToAdd + " " + toAddName.ToUpper() + " to the inventory elements");
				successfullyAdded = true;
			}
		}
		elementsUpdated = successfullyAdded;
		return successfullyAdded;
	}

	//Add part of the volume of an element container to the inventory if there is not enough room for the whole
	public int PartialElementAdd(ElementContainer elementToDivide){
		bool successfullyAdded = false;
		int difference = elementVolumeCapacity - currentTotalElementVolume;
		string elementName = elementToDivide.contents.name;
		ElementContainer whatWillFit = new ElementContainer (elementToDivide.contents, difference);
		if(difference > 0){
			successfullyAdded = AddElementContainerToInventory (whatWillFit); //add what will fit to the inventory
			elementToDivide.volume -= difference; //remove the amount from the element container
//			Debug.Log ("Added " + difference + " of " + elementName + " to player inventory. ElementBox still contains " + elementToDivide.volume + " of " + elementName + ".");
			SendMessage("Element box still contains " + elementToDivide.volume + " of " + elementName.ToUpper() + ".");
		}
		else{
//			Debug.Log("Could not add any partial amount of " + elementName + " to the inventory.");
			SendMessage("Could not add any partial amount of " + elementName.ToUpper() + " to the inventory.");
		}
		//partialAmount = whatWillFit.volume;
		return difference;
	}

	public void AddListOfElementContainer(List<ElementContainer> toAdd){
		foreach(ElementContainer cont in toAdd){
			AddElementContainerToInventory (cont);
		}
		int numberOfContainers = toAdd.Count;
		if(numberOfContainers != 0){
			SendMessage ("Transfered: " + numberOfContainers + " " + Plurality("container", numberOfContainers) + " of elements to other inventory.");
		}
	}

	public string Plurality(string baseWord, int number){
		if(number > 1){
			return baseWord + "s";
		}
		else{
			return baseWord;
		}
	}

	//Use to trade between players? Dump elements at base, or into strucutres?
	//Add the users inventory list of elements to another list of elements
	//Check for prexisiting containers, make if there is not
	//Empty players inventory
	// recipientList: the list of which to give all the elements in this inventory
	public void GiveAllElements(List<ElementContainer> recipientList){
		foreach(ElementContainer cont in elementsInventory){
			if(CheckIfElementAlreadyContained(cont.contents.name)){
				//GiveOneElement(GetElementContainerByName(cont))
				GiveOneElement (cont);
			}
			else{
				recipientList.Add (cont);
				//remove container from inventory?
				elementsInventory.Remove(cont);
			}
		}
		elementsInventory.Clear (); //after giving all clear the elements inventory
	}

	//pass on all of one element container to another
	public void GiveOneElement (ElementContainer recipientContainer){
		string elementName = recipientContainer.contents.name;
		int allVol = GetElementContainerByName (elementsInventory ,elementName).volume;
		GiveOneElementAmount (recipientContainer, allVol);
	}

	//pass on a set amount from one container to another
	//assumes element container exists since it is passed in
	public void GiveOneElementAmount(ElementContainer recipientContainer, int volToGive){
		string nameToGive = recipientContainer.contents.name;
		ElementContainer toGive = GetElementContainerByName (elementsInventory, nameToGive);
		recipientContainer.volume += volToGive;
		toGive.volume -= volToGive;
		elementsUpdated = true;
//		Debug.Log ("Gave " + volToGive + " " + nameToGive + " to another element container.");
		SendMessage("Gave " + volToGive + " " + nameToGive + " to another element container.");
	}

	//Finds element container by name within the list, if not found returns null
	public ElementContainer GetElementContainerByName(List<ElementContainer> listToSearch, string elementName){
		foreach(ElementContainer cont in listToSearch){
			if(cont.contents.name.Equals(elementName)){
				return cont;
			}
		}
		return null;
	}
		
	/*		CODE NUGGETS		*/

	//Counts the number of occurances of a code nugget of the given name in the inventory
	public int CodeNuggetTypeCount(string nuggetName){
		int count = 0;
		foreach(string nugget in codeNuggetInventory){
			if(nuggetName.Equals(nugget)){
				count++;
			}
		}
		return count;
	}
}