//------------------------------------------------------------
//RECORDING BUTTON CONTROLLER SCRIPT FOR BETA DEMO (INCLUDES SOUND ON/OFF, DELETE RECORDING, AND SAVE RECORDING)
//------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;


public class RecordingController_Master : MonoBehaviour {

	//Set sprites
	public GameObject deleteRecordingButton;
	public GameObject saveRecordingButton;
	public GameObject recordButton;
	public GameObject customizationPanel;
	public GameObject toolTip;
	public GameObject Refresh;

	GameObject ShapeInstance; 
	GameObject GameController;

	public AudioSource PlaceBubbleSound;

	void Start () {
		GameController = GameObject.FindWithTag("GameController");
	}

	public void ShortRecordingNotice() {
		Debug.Log ("called short recording notice");
		toolTip.SetActive (true);
		deleteRecording ();
	}

	//called after 3 bubbles are tapped along with recordPrompt()
	public void showRecording() {
		recordButton.SetActive (true);
	}

	//clear audio source on delete recording clicked
	public void deleteRecording() {
		ShapeInstance = GameObject.FindWithTag ("createdPrefab");
		Debug.Log ("delete recording called");
		//destroy prefab instance
		Destroy (ShapeInstance);
		hideRecordingControls ();
	}


	//save audio source on save recording clicked
	public void saveRecording() {
		PlaceBubbleSound.Play ();
		ShapeInstance = GameObject.FindWithTag ("createdPrefab");
		Debug.Log ("save recording called");
		hideRecordingControls ();
		//pass instance
		GameController.GetComponent<GameController_Master>().SaveShapeInfo(ShapeInstance);
		//switch tags
		if(ShapeInstance) {
			ShapeInstance.tag="placedPrefab";
			Debug.Log ("tags switched!");
			ShapeInstance.transform.parent = null;
		}
	}


	//called when stop recording is called (on microphone pointer up)
	public void showRecordingControls() {
		if (toolTip.activeInHierarchy == true) {
			toolTip.SetActive (false);
		}
		deleteRecordingButton.SetActive (true);
		saveRecordingButton.SetActive (true);
		customizationPanel.SetActive (true);
		recordButton.SetActive (false);
		Refresh.SetActive (false);
	}

	//hide delete, save, and sound icons, show recording
	public void hideRecordingControls() {
		deleteRecordingButton.SetActive (false);
		saveRecordingButton.SetActive (false);
		customizationPanel.SetActive(false);
		recordButton.SetActive (true);
		Refresh.SetActive(true);
	}
		
	// Update is called once per frame
	void Update () {

	}
}
