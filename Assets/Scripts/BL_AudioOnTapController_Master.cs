//------------------------------------------------------------
//PLAY AND STOP AUDIO ON BUBBLE TAP SCRIPT FOR BETA DEMO (INCLUDES START AND STOP AUDIO)
//------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;

public class BL_AudioOnTapController_Master : MonoBehaviour {
	AudioSource audioSource;

	public Material[] playPauseMaterials;
//	public Material[] pauseMaterial;
	public Material playCore;
	public Material pauseCore;
    //luu
    public Material replayCore;
    public GameObject core;
    public Renderer outer;
    public float inactiveScale;
    public float playingScale;

	private AudioSource[] allAudioSources;

	//for moderation app
	public Boolean isModerationApp;

	void Start() {
		audioSource = GetComponent<AudioSource> ();
		outer = GetComponent<Renderer>();
		outer.enabled = true;
	}


//	void OnTriggerEnter (Collider collision) {
//		if (isModerationApp) {
//			audioSource.Play ();
//			//set element 0 on ColorBubble in Bubble_BetaDemo prefab to transparent play button in inspector for this
//			//to work properly. Also assign pauseCore and playCore to their respective transparent materials
//
//			//get materials array of object
//			playPauseMaterials = this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials;
//			//change overlay material
//			playPauseMaterials [0] = pauseCore;
//			print ("play audio: " + playPauseMaterials);
//
//			//re set materials array of object
//			this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;
//
//			//change tag
//			moderationBubbleIsPlaying();
//
//			StartCoroutine (ClipEnded ());
//		}
//	}


//	void OnTriggerExit (Collider collision) {
//		if (isModerationApp) {
//			audioSource.Stop ();
//			//get materials array of object
//			playPauseMaterials = this.transform.Find("ColorBubble").GetComponent<MeshRenderer>().materials;
//			//change overlay material
//			playPauseMaterials [0] = playCore;
//			print ("stop audio: " + playPauseMaterials);
//
//			//re set materials array of object
//			this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;
//
//			//change tag back
//			moderationBubbleIsStopped();
//		
//		}
//	}

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
						if (hit.collider.transform == transform) {
							if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
							{
								Debug.Log("Touched the UI");
							}


//  if(!EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId))
//  {
//  //Button script here
//  }

							if (audioSource.isPlaying == false) {

								//stop any audio that may be playing before playing the new audio
								StopAllAudio();

								Debug.Log ("audio file: " + audioSource);
								Debug.Log ("hit detected, play audio");
								audioSource.Play ();

								//increase scale of core luu 
								core.transform.localScale = new Vector3(playingScale, playingScale, playingScale);

								//disable outer luu 
								outer.enabled = false;


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

								StartCoroutine (ClipEnded ());

							
							} 
							else {
								Debug.Log ("hit detected, stop audio"); // TODO: is there anything debug-logged when the audio is stopped?
								audioSource.Stop ();
								//get materials array of object
								playPauseMaterials = hit.transform.Find("ColorBubble").GetComponent<MeshRenderer>().materials;
								//change overlay material
								//we would set this to the replay icon instead of play

                                //luu
								playPauseMaterials[0] = replayCore;

								//reduce scale of core to normal luu
								core.transform.localScale = new Vector3(inactiveScale, inactiveScale, inactiveScale);

								//enable outer luu
								outer.enabled = true;

								print ("stop audio: " + playPauseMaterials);

								//re set materials array of object
								hit.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;



								if (isModerationApp) {
									moderationBubbleIsStopped();
								}
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

		if (Input.GetMouseButtonDown (0)) {
			if (audioSource.isPlaying == false) {

				//stop any audio that may be playing before playing the new audio
				StopAllAudio ();

				Debug.Log ("audio file: " + audioSource);
				Debug.Log ("hit detected, play audio");
				audioSource.Play ();

				//increase scale of core luu 
				core.transform.localScale = new Vector3(playingScale, playingScale, playingScale);

				//disable outer luu 
				outer.enabled = false;


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

				StartCoroutine (ClipEnded ());

			} else {
				Debug.Log ("hit detected, stop audio"); // TODO: is there anything debug-logged when the audio is stopped?
				audioSource.Stop ();
				//get materials array of object
				playPauseMaterials = this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials;
				//change overlay material
				//we would set this to the replay icon instead of play

                //luu
				playPauseMaterials [0] = replayCore;

				//reduce scale of core to normal luu 
				core.transform.localScale = new Vector3(inactiveScale, inactiveScale, inactiveScale);

				//enable outer luu
				outer.enabled = true;


				print ("stop audio: " + playPauseMaterials);

				//re set materials array of object
				this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;

				if (isModerationApp) {
					moderationBubbleIsStopped ();
				}
			}
		}
	}
	#endif

		
	IEnumerator ClipEnded() {
		yield return new WaitForSeconds(audioSource.clip.length);
		//get materials array of object
		playPauseMaterials = this.transform.Find("ColorBubble").GetComponent<MeshRenderer>().materials;
		//change overlay material

        //luu
		playPauseMaterials [0] = replayCore;

		//reduce scale of core to normal luu
		core.transform.localScale = new Vector3(inactiveScale, inactiveScale, inactiveScale);

		//enable outer luu
		outer.enabled = true;

		print ("stop audio: " + playPauseMaterials);
		//re set materials array of object
		this.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = playPauseMaterials;

	

		if (isModerationApp) {
			//change tag back
			moderationBubbleIsStopped();
		}
	}

	void StopAllAudio() {
		allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		foreach( AudioSource audioS in allAudioSources) {
			audioS.Stop();
		}
	}
}
