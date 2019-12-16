using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderController: MonoBehaviour {


	GameObject ShapeInstance;
	Button saveButton;

	// Use this for initialization
	void Start () {
		
	}

	private void OnTriggerEnter(Collider other){
		ShapeInstance = GameObject.FindWithTag ("createdPrefab");
		Debug.Log ("a collision happened");
		if (other.gameObject.tag == "createdPrefab") {
			Debug.Log ("entered");

			//change emission
			Color cantPlaceRed = new Color(0.2426f, 0.0035f, 0.0035f);
			ShapeInstance.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", cantPlaceRed);

			//vibrate baby oh yea
			Handheld.Vibrate();

			//set place button inactive
			saveButton = GameObject.Find("SaveRecordingButton").GetComponent<UnityEngine.UI.Button>();
			saveButton.interactable = false;
		}
	}

	private void OnTriggerExit(Collider other){
		ShapeInstance = GameObject.FindWithTag ("createdPrefab");
		Debug.Log ("a collision stopped happening");
		if (other.gameObject.tag == "createdPrefab") {
			Debug.Log ("exited");

			//reset emission
			ShapeInstance.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", Color.black);

			//set place button back to active
			saveButton = GameObject.Find("SaveRecordingButton").GetComponent<UnityEngine.UI.Button>();
			saveButton.interactable = true;
		}
	}
		
    // Update is called once per frame
    void Update () {
		
	}
}
