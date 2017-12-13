using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	public bool boulders; //whether or not to use boulders in game or terrain tiles

	public string terrainChoice;

	public int mapSizeX;
	public int mapSizeY;
	public int elementFrequency; //how common it is to find elements

	void Start(){
		//set default
		terrainChoice = "Lunar";
		mapSizeX = 20;
		mapSizeY = 20;
		boulders = true;
	}

	//int level is the index of the level in the build settings
	public void LoadScene(){
        //SceneManager.LoadScene (terrainChoice);
		GameObject setupData = GameObject.FindGameObjectWithTag("SetupData");
		GameSetupParameters parameters = setupData.GetComponent<GameSetupParameters> ();

		parameters.terrainType = terrainChoice;
		parameters.mapSizeX = mapSizeX;
		parameters.mapSizeY = mapSizeY;
		parameters.elementFrequency = elementFrequency;
		parameters.boulders = boulders;

        SceneManager.LoadScene(1);
    }

	public void ChooseMapLength(){
        GameObject inputFieldGo = GameObject.Find("LengthInput");
        InputField lengthInput = inputFieldGo.GetComponent<InputField>();

        this.mapSizeX = int.Parse (lengthInput.text);

		Debug.Log ("mapSizeX: " + mapSizeX);
	}

	public void ChooseMapWidth(){
        GameObject inputFieldGo = GameObject.Find("LengthInput");
        InputField widthInput = inputFieldGo.GetComponent<InputField>();

        this.mapSizeY = int.Parse (widthInput.text);

		Debug.Log ("mapSizeY: " + mapSizeY);
	}

	public void ChooseTerrain(int terrain){
		switch (terrain){
		case 0:
			terrainChoice = "Lunar";
			break;
		case 1:
			terrainChoice = "Martian";
			Debug.Log ("terrainChoice: " + terrainChoice);
			break;
		case 2:
			terrainChoice = "Titan";
			Debug.Log ("terrainChoice: " + terrainChoice);
			break;
		case 3:
			terrainChoice = "Omicron Persei 8";
			Debug.Log ("terrainChoice: " + terrainChoice);
			break;
		}
	}

	public void ChooseElementFrequency(){
		//do UI stuff here
		GameObject inputField = GameObject.Find("FrequencyInput");
		InputField frequencyInput = inputField.GetComponent<InputField> ();

		elementFrequency = int.Parse (frequencyInput.text);

	}

	public void ToggleBoulders(){
		boulders = !boulders;
	}
}