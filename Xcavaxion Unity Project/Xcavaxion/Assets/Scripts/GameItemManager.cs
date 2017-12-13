using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class GameItemManager {

	//the parent class for all in game items, elements, and code nuggets
	//contains methods and variables in common for all those different items

	//element boxes only exist on the map to serve as a container for an amount of a certain element
	//otherwise we only need to know how much of an element we have
	//element still exists as a concept so we can get information from it, like it's name, description, value, density, prevalence, etc.

	//these will just be a list of the possible things in each category <string> temporary
	public List<string> listOfItems;
	public static List<Elements> listOfElements = new List<Elements>();
	public List<string> listOfCodeNuggets;

	public float elementConstant = 0.25f;

	System.Random rand = new System.Random();

	public GameItemManager(){
		//LoadAllItems ();
		LoadAllElements ();
		//LoadAllCodeNuggets ();
	}

	public static void LoadAllItems(){
		
	}

	public static void LoadAllElements(){

		Elements EMPTY 		= new Elements ("empty", 	"no elements", 										"e", 	0, 0, 	0, 0);

		Elements CARBON 	= new Elements ("carbon", 	"Forms strong bonds and is useful in many forms.", 	"C", 	1, 10, 	5, 2000);
		Elements COPPER 	= new Elements ("copper", 	"Conductive, shiny, and easily trarnishes.", 		"Cu", 	3, 35, 	3, 8960);
		Elements GOLD 		= new Elements ("gold", 	"Shiny, rare, conductive, and easily malliable.", 	"Au", 	5, 200, 1, 19230);
		Elements HELIUM 	= new Elements ("helium", 	"Lighter than air and colorless.", 					"He", 	5, 50, 	1, 1);
		Elements IRON 		= new Elements ("iron", 	"Strong, magentic and malliable.", 					"Fe", 	2, 25, 	4, 7870);
		Elements LEAD 		= new Elements ("lead", 	"Toxic and malliable.", 							"Pb", 	3, 30, 	4, 11340);
		Elements SILVER 	= new Elements ("silver", 	"Shiny and less rare than gold.", 					"Ag", 	4, 100, 1, 10490);
		Elements URANIUM 	= new Elements ("uranium", 	"Dense, toxic, and radioactive.", 					"U", 	4, 100, 1, 19100);
		Elements WATER 		= new Elements ("water", 	"You can drink it. Well, not robots.", 				"H20", 	1, 12, 	4, 1000);

		listOfElements.Add (EMPTY);
		listOfElements.Add (CARBON);
		listOfElements.Add (COPPER);
		listOfElements.Add (GOLD);
		listOfElements.Add (HELIUM);
		listOfElements.Add (IRON);
		listOfElements.Add (LEAD);
		listOfElements.Add (SILVER);
		listOfElements.Add (URANIUM);
		listOfElements.Add (WATER);

		Debug.Log("Elements loaded, " + listOfElements.Count + " in total.");

	}

	public static void LoadAllCodeNuggets(){
		
	}

	public List<string> GetListOfElementNames(){
		List<string> names = new List<string> ();
		foreach(Elements ele in listOfElements){
			names.Add (ele.name);
		}
		return names;
	}

	//returns a type of element weighted on their prevalence number
	public Elements GetElementDispersement(){
		//System.Random rand = new System.Random ();
		List<Elements> roulette = new List<Elements> ();

		foreach(Elements ele in listOfElements){
			int prevalence = ele.prevalence;

			for(int i = 0; i < prevalence; i++){
				roulette.Add (ele);
			}
		}
		return roulette [rand.Next (roulette.Count - 1)];
	}

	//random amount based on prevalence... I guess?
	public ElementContainer GetRandomVolumeOfDispersedElement(){

		ElementContainer toReturn = new ElementContainer ();
		//System.Random rand = new System.Random ();
		Elements temp = GetElementDispersement ();
		toReturn.contents = temp;
		int currentPrev = toReturn.contents.prevalence;
		int factor = (int)((currentPrev * currentPrev) * elementConstant * 100); //100 just to stop from getting a volume of zero
		int amount = rand.Next (factor);
		toReturn.volume = amount;

		return toReturn;
	}
}