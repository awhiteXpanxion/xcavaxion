using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlPanelController : MonoBehaviour {

	public bool controlPanelOnScreen;

	public string teamColor; //control panel knows it's own team color, built in prefab
	public string teamName; //control panel knows it's own team name, built in prefab
	public string playerName; //grabbed from player reference

	public int playerCurrentElementVolume; //grabbed from player reference
	public int playerMaxElementVolume; //grabbed from player reference

	public float playerCurrentEnergyLevel; //concept not yet implemented

	public PlayerController playerReference; //reference loaded at runtime

	public List<Button> controlPanelButtons; //references built in to prefab

	//references built in to prefab
	public Text nameText;
	public Text teamText;
	public Text eleVolText;

	private Animator anim;

	public string eleVolString = "ELEMENTS: ";

	public bool mousedOver;

	// Use this for initialization
	void Start () {

		teamText.text = teamName.ToUpper();
		LoadPlayerReference ();

		anim = gameObject.GetComponent<Animator> ();
		anim.enabled = false;

		controlPanelOnScreen = false;
		mousedOver = false;
	}
	
	// Update is called once per frame
	void Update () {

		if(EventSystem.current.IsPointerOverGameObject()){
			mousedOver = true;
			print ("moused over control panel!");
		}
		else{
			mousedOver = false;
		}

		UpdatePlayerState ();
		//ButtonListener ();
		UpdateCPText();

		if(Input.GetKeyUp(KeyCode.Space) && !controlPanelOnScreen){
			ControlPanelIn ();
		}
		else if(Input.GetKeyUp(KeyCode.Space) && controlPanelOnScreen){
			ControlPanelOut ();
		}
	}

	void ControlPanelIn(){
		anim.enabled = true;
		anim.Play ("ControlPanelSlideIn");
		controlPanelOnScreen = true;
	}

	void ControlPanelOut(){
		anim.Play ("ControlPanelSlideOut");
		controlPanelOnScreen = false;
	}

	void LoadPlayerReference(){
		var player = GameObject.Find ("Player");
		playerReference = player.GetComponent<PlayerController> ();

		playerName = playerReference.playerIdentifier;
		nameText.text = playerName.ToUpper (); //because the custom font has no lower case letters
		playerCurrentElementVolume = playerReference.inventory.currentTotalElementVolume;
		playerMaxElementVolume = playerReference.inventory.elementVolumeCapacity;

		//playerCurrentEnergyLevel = playerReference.energyLevel;
	}

	void UpdatePlayerState(){
		playerCurrentElementVolume = playerReference.inventory.currentTotalElementVolume;
		playerMaxElementVolume = playerReference.inventory.elementVolumeCapacity; //just in case the capacity may change, like with an item or something

		//playerCurrentEnergyLevel = playerReference.energyLevel;
	}

	void UpdateCPText(){
		eleVolText.text = eleVolString + playerCurrentElementVolume.ToString () + "/" + playerMaxElementVolume.ToString ();
	}

	void ButtonListener(){
		//not sure if I need to do this, or the functions below, each button could handle their own thing
	}

	void BringUpMenu(){
		
	}

	void DigAction(){
		
	}

	void StopAction(){
		
	}

	void BoxMoveTo(){
		
	}

	void BoulderMoveTo(){
		
	}

	void BaseMoveTo(){
		
	}
		
}