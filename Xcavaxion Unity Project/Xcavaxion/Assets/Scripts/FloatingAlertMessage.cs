using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingAlertMessage : MonoBehaviour {

	public int messageNumber;

	public string messageText;

	public Text alertText;

	public float moveSpeed;
	public float timeToDestroy;
	public float amountToMoveUp;

	public Vector2 characterPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		alertText.text = messageText;
		alertText.transform.position = new Vector3 (transform.position.x, transform.position.y + (moveSpeed * Time.deltaTime), transform.position.z);

		timeToDestroy -= Time.deltaTime;

		if(timeToDestroy <= 0){
			Destroy(gameObject);
		}
	}

	public void NewMessage(string newText){
		messageText = newText;
	}

	public void MoveMessageUp(){
		alertText.transform.position = new Vector3 (transform.position.x, transform.position.y + amountToMoveUp, transform.position.z);
	}
}