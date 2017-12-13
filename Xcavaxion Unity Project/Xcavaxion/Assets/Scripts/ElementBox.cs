using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElementBox : MonoBehaviour {

	public ElementContainer container;
	public string containerContents; //this is just for the UI or Unity inspector
	public int containerVolume; //ditto
	public Renderer rend;

	void Awake(){
		container = new ElementContainer ();
		rend = GetComponent<SpriteRenderer> ();
	}

	// Use this for initialization
	// aparently start() is not called right away upon instantiation LIES!
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		containerContents = container.contents.name;
		containerVolume = container.volume;
	}

	//TODO want to be able to highlight element boxes to see details in the UI about their contents
	void OnMouseEnter(){
		rend.material.color = Color.red;
	}

	void OnMouseExit(){
		rend.material.color = Color.white; //restores tile color
	}
}