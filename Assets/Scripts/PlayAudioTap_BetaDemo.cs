﻿//------------------------------------------------------------
//PLAY AND STOP AUDIO ON BUBBLE TAP SCRIPT FOR BETA DEMO (INCLUDES START AND STOP AUDIO)
//------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;

public class PlayAudioTap_BetaDemo : MonoBehaviour {
	AudioSource audioSource;

	void Start() {

		audioSource = GetComponent<AudioSource> ();
//		Debug.Log ("start " + audioSource);
	}

	// Update is called once per frame
	void Update () {

		int fingerCount = 0;
		foreach (Touch touch in Input.touches) {
			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				fingerCount++;
		}

		if (fingerCount > 0) {
//			print ("User has " + fingerCount + " finger(s) touching the screen");

		if (Input.touchCount > 0) {

			Touch touch = Input.GetTouch (0);
			if (touch.phase == TouchPhase.Began) {

				Ray ray = Camera.main.ScreenPointToRay (touch.position);
				RaycastHit hit;

				Debug.Log ("eventstart");

				if (Physics.Raycast (ray, out hit, 100)) {
					if (hit.collider.transform == transform) {

						if (audioSource.isPlaying == false) {
							Debug.Log ("audio file: " + audioSource);
							Debug.Log ("hit detected, play audio");
							audioSource.Play ();
						} 
						else {
							Debug.Log ("hit detected, stop audio");
							audioSource.Stop ();
						}

					}

				}
			}



		}
	}
}
}
