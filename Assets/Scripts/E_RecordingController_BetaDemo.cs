//------------------------------------------------------------
//RECORDING BUTTON CONTROLLER SCRIPT FOR BETA DEMO (INCLUDES SOUND ON/OFF, DELETE RECORDING, AND SAVE RECORDING)
//------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;

public class E_RecordingController_BetaDemo : MonoBehaviour {

	//Set sprites
	public GameObject deleteRecordingButton;
	public GameObject saveRecordingButton;
	public GameObject soundOffButton;
	public GameObject soundOnButton;
	public GameObject recordButton;
	public GameObject customizationPanel;

	GameObject GameController;
	GameObject ShapeInstance; 

	void Start () {
		GameController = GameObject.FindWithTag("GameController");
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
		ShapeInstance = GameObject.FindWithTag ("createdPrefab");
		Debug.Log ("save recording called");
		Debug.Log (ShapeInstance);
		hideRecordingControls ();
		//pass instance
		GameController.GetComponent<E_GameController_BetaDemo_Audio>().SaveShapeInfo(ShapeInstance);
		//switch tags
		if(ShapeInstance) {
			ShapeInstance.tag="placedPrefab";
			Debug.Log ("tags switched!");
			ShapeInstance.transform.parent = null;
		}


	}

//	show correct sound icon on click
//	public void soundControl() {
//		ShapeInstance = GameObject.FindWithTag ("createdPrefab");
//		AudioSource ShapeInstanceAS = ShapeInstance.GetComponent<AudioSource>();
//		//		Debug.Log ("sound control called");
//		if ( soundOnButton.activeInHierarchy == true  ) {
//			Debug.Log ("sound on button tapped, stop audio");
//			soundOnButton.SetActive (false);
//			soundOffButton.SetActive (true);
//			ShapeInstanceAS.Stop ();
//		} else {
//			Debug.Log ("sound off button tapped, play audio");
//			soundOnButton.SetActive (true);
//			soundOffButton.SetActive (false);
//			ShapeInstanceAS.Play ();
//		}
//	}

	//called when stop recording is called (on microphone pointer up)
	public void showRecordingControls() {
		//		Debug.Log ("called showRecordingControls");
		deleteRecordingButton.SetActive (true);
		saveRecordingButton.SetActive (true);
//		soundOnButton.SetActive (true);
//		soundOffButton.SetActive (false);
		recordButton.SetActive (false);
		customizationPanel.SetActive (true);
	}

	//hide delete, save, and sound icons, show recording
	public void hideRecordingControls() {
				Debug.Log ("called hideRecordingControls");

		deleteRecordingButton.SetActive (false);
		saveRecordingButton.SetActive (false);
//		if (soundOnButton.activeInHierarchy == true) {
//			soundOnButton.SetActive (false);
//		} else {
//			soundOffButton.SetActive (false);
//		}
		recordButton.SetActive (true);
		customizationPanel.SetActive(false);

	}


	// Update is called once per frame
	void Update () {

	}
}
