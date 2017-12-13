using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItems : MonoBehaviour {

	//Ties the conceptual class of GameItemManager to an in game Unity GameObject
	//GameItemManager class has the concept of the raw items that are passed around, ie: items, elements, code nuggets

	public GameItemManager allGameItems;

	public List<GameObject> elementBoxes;

	// Use this for initialization
	void Start () {
		allGameItems = new GameItemManager ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}