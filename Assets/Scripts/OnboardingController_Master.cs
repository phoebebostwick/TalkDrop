using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;

public class OnboardingController_Master : MonoBehaviour {

		GameObject GameController;
		public Material[] playPauseMaterials;
		public Material playCore;
		public Material pauseCore;
		public Material replayCore;
		public GameObject core;
		public float inactiveScale;
		public float playingScale;
		AudioSource audioSource;
		public GameObject toolTip;
		public GameObject RecordingControlPanel;
		public GameObject OnboardingBubble;
		public GameObject Refresh;

		// Use this for initialization
		void Start () {
			GameController = GameObject.FindWithTag("GameController");
			audioSource = GetComponent<AudioSource> ();
		}


		public void StopAudio() {
			RecordingControlPanel.SetActive (true);
			Refresh.SetActive (true);
			toolTip.SetActive (false);
			GameController.GetComponent<GameController_Master>().CallGetAllIds();
			OnboardingBubble.SetActive (false);
		}

	public void CallOnboardingDone() {
		StartCoroutine (OnboardingDone ());
	}
		
		

	IEnumerator OnboardingDone() {
			Debug.Log ("onboarding done");
		yield return new WaitForSeconds(audioSource.clip.length);
		//get materials array of object
		playPauseMaterials = this.transform.Find("ColorBubble").GetComponent<MeshRenderer>().materials;
		//change overlay material
		playPauseMaterials [0] = replayCore;
		print ("stop audio: " + playPauseMaterials);
		//re set materials array of object
		this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;
		ReduceCoreScale ();
			RecordingControlPanel.SetActive (true);
			Refresh.SetActive (true);
			toolTip.SetActive (false);
			GameController.GetComponent<GameController_Master>().CallGetAllIds();
			OnboardingBubble.SetActive (false);
			
		}

	public void ReduceCoreScale() {
		//reduce scale of core to normal
		core.transform.localScale = new Vector3(inactiveScale, inactiveScale, inactiveScale);
	}

	public void IncreaseCoreScale() {
		//increase scale of core 
		core.transform.localScale = new Vector3(playingScale, playingScale, playingScale);
	}
	}
		
