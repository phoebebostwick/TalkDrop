using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderController: MonoBehaviour {


	GameObject ShapeInstance;
	Button saveButton;
	//public Material DropOuter;

	// Use this for initialization
	void Start () {
		
	}

	private void OnTriggerEnter(Collider other){
		ShapeInstance = GameObject.FindWithTag ("createdPrefab");
		Debug.Log ("a collision happened");
		if (other.gameObject.tag == "createdPrefab") {
			//turn on emission on shader
			ShapeInstance.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
			//vibrate baby oh yea
			Handheld.Vibrate();
			//set place button inactive or greyed out or whatever is decided
			saveButton = GameObject.Find("SaveRecordingButton").GetComponent<UnityEngine.UI.Button>();
			saveButton.interactable = false;
		}
	}

	private void OnTriggerExit(Collider other){
		ShapeInstance = GameObject.FindWithTag ("createdPrefab");
		Debug.Log ("a collision stopped happening");
		if (other.gameObject.tag == "createdPrefab") {
			//reset emission
			ShapeInstance.GetComponent<MeshRenderer> ().material.DisableKeyword("_EMISSION");

			//set place button back to active or full color or whatever
			saveButton = GameObject.Find("SaveRecordingButton").GetComponent<UnityEngine.UI.Button>();
			saveButton.interactable = true;
		}
	}
		
    // Update is called once per frame
    void Update () {
		
	}
}
