using UnityEngine;
using System.Collections;

public class PlayerStartPoint : MonoBehaviour {

	//Player start points for each team? Snap to team point? Even CPU opponents?

	private PlayerController thePlayer;
	private CameraController theCamera;

	private AIController theComputer;

	public Vector2 startDirection;

	// Use this for initialization
	void Start () {

		thePlayer = FindObjectOfType<PlayerController> (); //move the player to the start point
		thePlayer.transform.position = transform.position;

		theComputer = FindObjectOfType<AIController> (); //move the computer player to the same start point
		theComputer.transform.position = transform.position;

		theCamera = FindObjectOfType<CameraController> ();
		theCamera.transform.position = new Vector3 (transform.position.x, transform.position.y, theCamera.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}