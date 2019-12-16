﻿//------------------------------------------------------------
//PLAY AND STOP AUDIO ON BUBBLE TAP SCRIPT FOR BETA DEMO (INCLUDES START AND STOP AUDIO)
//------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;

public class AudioOnTapController_Master : MonoBehaviour {
	AudioSource audioSource;

	public Material[] playPauseMaterials;
	public Material playCore;
	public Material pauseCore;
	public Material replayCore;
	public GameObject core;
	float inactiveScale = .54f;
	public float playingScale;

	private AudioSource[] allAudioSources;

	//for moderation app
	public Boolean isModerationApp;

	public Boolean isOnboardingBubble;
	GameObject OnboardingController;
	Boolean audioStopped = false;


	void Start() {
		OnboardingController = GameObject.FindWithTag("OnboardingController");
		audioSource = GetComponent<AudioSource> ();
	}

	public void moderationBubbleIsPlaying() {
		this.gameObject.tag = "isPlaying";
		
	}

	public void moderationBubbleIsStopped() {
		this.gameObject.tag = "placedPrefab"; 
	}


	// Update is called once per frame
	void Update () {

		int fingerCount = 0;
		foreach (Touch touch in Input.touches) {
			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				fingerCount++;
		}
		//touch input
		if (fingerCount > 0) {
			//			print ("User has " + fingerCount + " finger(s) touching the screen");

			if (Input.touchCount > 0) {

				Touch touch = Input.GetTouch (0);
				if (touch.phase == TouchPhase.Began) {

					Ray ray = Camera.main.ScreenPointToRay (touch.position);
					RaycastHit hit;
					Debug.Log ("eventstart");

					if (Physics.Raycast (ray, out hit, 100)) {
						if (hit.collider.transform == transform && EventSystem.current.currentSelectedGameObject == null) {
//							Debug.Log("current selected is: " + EventSystem.current.currentSelectedGameObject);


							if (audioSource.isPlaying == false) {

								//stop any audio that may be playing before playing the new audio
								StopAllAudio();

								Debug.Log ("audio file: " + audioSource);
								Debug.Log ("hit detected, play audio");
								audioSource.Play ();
								audioStopped = false;
								//set element 0 on ColorBubble in Bubble_BetaDemo prefab to transparent play button in inspector for this
								//to work properly. Also assign pauseCore and playCore to their respective transparent materials

								//get materials array of object
								playPauseMaterials = hit.transform.Find("ColorBubble").GetComponent<MeshRenderer>().materials;
								//change overlay material
								playPauseMaterials [0] = pauseCore;
								print ("play audio: " + playPauseMaterials);

								//re set materials array of object
								hit.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;

								if (isModerationApp) {
									moderationBubbleIsPlaying();
								}


								if (isOnboardingBubble) {
									OnboardingController.GetComponent<OnboardingController_Master> ().CallOnboardingDone ();
								} else {
									StartCoroutine (ClipEnded ());
								}
							
							} 
							else {
								Debug.Log ("hit detected, stop audio"); // TODO: is there anything debug-logged when the audio is stopped?
								audioSource.Stop ();
								//get materials array of object
								playPauseMaterials = hit.transform.Find("ColorBubble").GetComponent<MeshRenderer>().materials;
								//change overlay material
								//we would set this to the replay icon instead of play

								playPauseMaterials [0] = replayCore;
								print ("stop audio: " + playPauseMaterials);
								ResetCoreScale ();

								//re set materials array of object
								hit.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;

								if (isModerationApp) {
									moderationBubbleIsStopped();
								}

								if (isOnboardingBubble) {
									OnboardingController.GetComponent<OnboardingController_Master>().StopAudio();
								}

								audioStopped = true;
							}

						}

					}
				}

			}
		}
			
	}

	#if UNITY_EDITOR

	//mouse input
	public void OnMouseOver() {

		if (Input.GetMouseButtonDown (0) && EventSystem.current.currentSelectedGameObject == null) {
//			Debug.Log("current selected is: " + EventSystem.current.currentSelectedGameObject.tag);

			if (audioSource.isPlaying == false) {

				//stop any audio that may be playing before playing the new audio
				StopAllAudio ();

				Debug.Log ("audio file: " + audioSource);
				Debug.Log ("hit detected, play audio");
				audioSource.Play ();
				audioStopped = false;

				//set element 0 on ColorBubble in Bubble_BetaDemo prefab to transparent play button in inspector for this
				//to work properly. Also assign pauseCore and playCore to their respective transparent materials

				//get materials array of object
				playPauseMaterials = this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials;
				//change overlay material
				playPauseMaterials [0] = pauseCore;
				print ("play audio: " + playPauseMaterials);

				//re set materials array of object
				this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;

				if (isModerationApp) {
					moderationBubbleIsPlaying ();
				}

				if (isOnboardingBubble) {
					OnboardingController.GetComponent<OnboardingController_Master> ().CallOnboardingDone ();
				} else {
					StartCoroutine (ClipEnded ());
				}

			} else {
				Debug.Log ("hit detected, stop audio"); // TODO: is there anything debug-logged when the audio is stopped?
				audioSource.Stop ();
				ResetCoreScale ();
				audioStopped = true;

				//get materials array of object
				playPauseMaterials = this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials;
				//change overlay material
				//we would set this to the replay icon instead of play
				playPauseMaterials [0] = replayCore;
				print ("stop audio: " + playPauseMaterials);

				//re set materials array of object
				this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;

				if (isModerationApp) {
					moderationBubbleIsStopped ();
				}

				if (isOnboardingBubble) {
					OnboardingController.GetComponent<OnboardingController_Master>().StopAudio();
				}
			}
		}
	}
	#endif


		
	IEnumerator ClipEnded() {
		yield return new WaitForSeconds(audioSource.clip.length);
		if (audioStopped == false) {
			//get materials array of object
			playPauseMaterials = this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials;
			//change overlay material
			playPauseMaterials [0] = replayCore;
			print ("clip ended stop audio: " + playPauseMaterials);
			//re set materials array of object
			this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;
			ResetCoreScale ();


			if (isModerationApp) {
				//change tag back
				moderationBubbleIsStopped ();
			}
		}
	}

	void StopAllAudio() {
		allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		foreach( AudioSource audioS in allAudioSources) {
			if (audioS.isPlaying) {
				audioS.Stop ();
				ResetCoreScale ();
				Debug.Log ("yo stop all audio" + audioS.gameObject.name);
				playPauseMaterials = audioS.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials;
				playPauseMaterials [0] = replayCore;
				audioS.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;
			}
		} 
	}

	public void ResetCoreScale() {
		//reduce scale of core to normal
		core.transform.localScale = new Vector3(inactiveScale, inactiveScale, inactiveScale);
	}

}
