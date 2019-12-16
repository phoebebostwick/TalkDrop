using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.XR.iOS; 

public class LoadMap_Master : MonoBehaviour, PlacenoteListener {

	// Unity ARKit Session handler
	private UnityARSessionNativeInterface mSession;
	public string mapId;

	//set global boolean
	bool runLoadMap = false;
	bool onboardingDone = false;
	public GameObject onboardingBubble;
	public GameObject onboardingPanel;

	// Use this for initialization
	void Start () {
		Input.location.Start ();

		// Start ARKit using the Unity ARKit Plugin
		mSession = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
		StartARKit ();
		LibPlacenote.Instance.RegisterListener (this);

		Debug.Log ("lm master ARkit Init");
		Debug.Log ("Map ID is: " + mapId);


		//set load map to true, but also needs pn sdk to be initialized
		runLoadMap = true;
	}

	// Initialize ARKit. This will be standard in all AR apps
	private void StartARKit()
	{
		Application.targetFrameRate = 60;
		ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();
		config.planeDetection = UnityARPlaneDetection.Horizontal;
		config.alignment = UnityARAlignment.UnityARAlignmentGravity;
		config.enableLightEstimation = true;
		mSession.RunWithConfig(config);
	}

	// Update is called once per frame
	void Update () {
		// should be true first time, false every other time?
		//		Debug.Log ("update called " + runLoadMap );

		if (!LibPlacenote.Instance.Initialized () ) {
			Debug.Log("SDK not yet initialized");
			return;
		}

		if ( (runLoadMap == true) && (LibPlacenote.Instance.Initialized ()) ) {
			//this should be called ONCE, should output true
			Debug.Log ("sdk initialized, running load map" + runLoadMap);
			LoadMap ();
			// set run Load map to false so when update is called it wont run this conditional again
			runLoadMap = false;
		} 
	}


	public void LoadMap() {
		//this should be called ONCE, should output false
		Debug.Log ("load map called!" + runLoadMap);

		ConfigureSession ();

		if (!LibPlacenote.Instance.Initialized()) {
			Debug.Log ("SDK not yet initialized");
			return;
		}

		LibPlacenote.Instance.LoadMap(mapId, 
			(completed, faulted, percentage) =>   
			{
				if (completed) {

					// Now try to localize the map
					LibPlacenote.Instance.StartSession ();

//					Uncomment these lines to test off location
					onboardingBubble.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 0.5f);
					onboardingBubble.SetActive(true);
					onboardingPanel.SetActive(false);

					Debug.Log("Loaded Map: " + mapId);

				}
				else if (faulted) {
					Debug.Log("Failed to load ID: " + mapId);
				}
				else {
					Debug.Log("Download Progress: " + percentage.ToString("F2") + "/1.0)");
				}
			}
		);

	}

	private void ConfigureSession() {
		#if !UNITY_EDITOR
		ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration ();
		config.alignment = UnityARAlignment.UnityARAlignmentGravity;
		config.enableLightEstimation = true;
		config.planeDetection = UnityARPlaneDetection.Horizontal;
		mSession.RunWithConfig (config);
		#endif
	}
		

	public void OnPose (Matrix4x4 outputPose, Matrix4x4 arkitPose) {}

	public void OnStatusChange (LibPlacenote.MappingStatus prevStatus, LibPlacenote.MappingStatus currStatus)
	{
		if (onboardingDone == false) {
			Debug.Log ("prevStatus: " + prevStatus.ToString () + " currStatus: " + currStatus.ToString ());
			if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.LOST) {
				Debug.Log ("Localized");
//				onboardingBubble.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 0.5f);
//				onboardingBubble.SetActive (true);
//				onboardingPanel.SetActive (false);
//				onboardingDone = true;
			} else if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.WAITING) {
				Debug.Log ("Mapping: Tap to add Shapes");
			} else if (currStatus == LibPlacenote.MappingStatus.LOST) {
				Debug.Log ("Searching for position lock");
			} else if (currStatus == LibPlacenote.MappingStatus.WAITING) {
				Debug.Log ("waiting");
	
			}
		}
	}

}


