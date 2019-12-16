using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelController_BetaDemo : MonoBehaviour {

	//Set label
	public Text labelText;
	public GameObject labelPanel;


	// Use this for initialization
	void Start () {
		
	}

	//called after map load is complete
	public void mapInitialized() {
		labelText.text = "Turn up your sound and tap the bubble!";
	}

	//called after 3 bubbles are tapped along with showRecordingControl()
	public void recordPrompt() {
		labelText.text = "Record your own thought!";
	}


	//called when start recording is called (on mic button click)	
	public void hideLabelPanel() {
//		Debug.Log ("hide label panel called");
		labelPanel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
