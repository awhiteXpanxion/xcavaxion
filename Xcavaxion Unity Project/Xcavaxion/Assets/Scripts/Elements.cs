using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elements {

	//This class represents the concept of an element
	//It is not something that can be aquired in game
	//Elements are given interactivity with ElementContainers, in which they have an amount, the volume.
	//Elements are represented in game by ElementBoxes, which have an ElementContainer within

	/*
	 *      Elements
	 *         |
	 *         |
	 *         v
	 *  ElementContainer
	 *         |
	 *         |
	 *         V
	 *     ElementBox
	 * 
	 */

	public string name;					//the name of the type of element
	public string description;			//a short text description to possibly include in the UI, with possibly useful info?
	public string symbol;				//the chemical symbol for the element, appears on the element boxes
	public int extractionDifficulty;	// 1(easy) ... 5(hard) how hard it is to mine from the ground, ties in to how long a boulder will take to mine
	public int valueByWeight;			//how much the element is worth based on the weight
	public int prevalence;				// 1(rare) ... 5(common) how common to find the element is 
	public int density;					//how dense the element is as far as it molecules, not sure if this used now or how this will be used.

	public Elements(string name, string description, string symbol, int extractionDifficulty, int valueByWeight, int prevalence, int density){
		this.name = name;
		this.description = description;
		this.symbol = symbol;
		this.extractionDifficulty = extractionDifficulty;
		this.valueByWeight = valueByWeight;
		this.prevalence = prevalence;
		this.density = density;
	}
}