//------------------------------------------------------------
//RECORDING BUTTON CONTROLLER SCRIPT FOR BETA DEMO (INCLUDES SOUND ON/OFF, DELETE RECORDING, AND SAVE RECORDING)
//------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;

public class M_RecordingController_RecordingDemo : MonoBehaviour {

	//Set sprites
	public GameObject deleteRecordingButton;
	public GameObject saveRecordingButton;
	public GameObject soundOffButton;
	public GameObject soundOnButton;
	public GameObject recordButton;


//	Stack<GameObject> deleteLastStack = new Stack<GameObject>();

	public GameObject customizationPanel;

	public GameObject audioSpherePrefab;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = audioSpherePrefab.GetComponent<AudioSource> ();
	}

	//called after 3 bubbles are tapped along with recordPrompt()
	public void showRecording() {
		recordButton.SetActive (true);
	}
	//clear audio source on delete recording clicked
	public void deleteRecording() {
		Debug.Log ("delete recording called");
		//destroy prefab instance
//		deleteRecordingButton.SetActive (true);
		Destroy (GameObject.FindWithTag ("createdPrefab"));
//		var objToUndo = deleteLastStack.Pop();
//		Destroy(objToUn/do );
		hideRecordingControls ();
	}


	//save audio source on save recording clicked
	public void saveRecording() {
		Debug.Log ("save recording called");
		hideRecordingControls ();

//		if(hit.collider.tag == "createdPrefab"){
//			transform.gameObject.tag = "placedPrefab";
//		}

//		RaycastHit hit;

//		if(GameObject.CompareTag("createdPrefab")){
//
//		}
//		gameObject.tag = "Untagged";

//		if (gameObject.tag == ("createdPrefab")){
//			Debug.Log ("removetag");
//			gameObject.tag = null;
//
//		}
	}

	//show correct sound icon on click
	public void soundControl() {
//		Debug.Log ("sound control called");
		// if sound on button is active, hide it, show sound off
//		if ( (soundOnButton.activeInHierarchy == true) && (audioSource.isPlaying == true) ) {
		if ( soundOnButton.activeInHierarchy == true  ) {

			Debug.Log ("sound on button tapped, stop audio");
			soundOnButton.SetActive (false);
			soundOffButton.SetActive (true);
			audioSource.Stop ();

		} else {
		//if sound on button isnt active, show it, hide sound off
			Debug.Log ("sound off button tapped, play audio");

			soundOnButton.SetActive (true);
			soundOffButton.SetActive (false);
			audioSource.Play ();

		}
	}

	//called when stop recording is called (on microphone pointer up)
	public void showRecordingControls() {
//		Debug.Log ("called showRecordingControls");
		deleteRecordingButton.SetActive (true);
		saveRecordingButton.SetActive (true);
		soundOnButton.SetActive (true);
		soundOffButton.SetActive (false);
		recordButton.SetActive (false);
		customizationPanel.SetActive (true);
	}

	//hide delete, save, and sound icons, show recording
	public void hideRecordingControls() {
		deleteRecordingButton.SetActive (false);
		saveRecordingButton.SetActive (false);
		if (soundOnButton.activeInHierarchy == true) {
			soundOnButton.SetActive (false);
		} else {
			soundOffButton.SetActive (false);
		}
		recordButton.SetActive (true);
		customizationPanel.SetActive(false);

	}


	void OnTriggerEnter (Collider collision) {
			audioSource.Play ();
			Debug.Log ("collided");
	}

	void OnTriggerExit (Collider collision) {
		audioSource.Stop ();
		Debug.Log ("stopping audio");
	}


	// Update is called once per frame
	void Update () {

	}
}
