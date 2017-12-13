using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementContainer {

	public Elements contents; //what element is in the container
	public int volume; //the amount of the element in the container

	public ElementContainer(){
		contents = GameItemManager.listOfElements[0]; //default give it the empty element upon creation
		volume = 0; //and volume of zero
	}

	public ElementContainer(Elements contents, int volume){
		this.contents = contents;
		this.volume = volume;
	}

	//TODO I don't think this is used anywhere, may need to be removed
	public void AddElementToContainer(Elements toAdd, int amountToAdd){
		contents = toAdd;
		volume = amountToAdd;
	}
}