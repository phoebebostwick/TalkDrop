using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;

using UnityEngine.XR.iOS; // Import ARKit Library

public class LoadMap_BetaDemo : MonoBehaviour {
	// Unity ARKit Session handler
	private UnityARSessionNativeInterface mSession;

//	public Text notifications;
	public string mapId;
	//set global boolean
	public bool runLoadMap = false;

	// Use this for initialization
	void Start () {
		// Start ARKit using the Unity ARKit Plugin
		mSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();
		StartARKit();

		Debug.Log ("ARkit Init");
		Debug.Log ("Map ID is: " + mapId);

		//set load map to true, but also needs pn sdk to be initialized
		runLoadMap = true;

		FeaturesVisualizer.EnablePointcloud(); // Optional - to see the point features

//		notifications.text = "Press and hold to make a recording!";

	}
	// Initialize ARKit. This will be standard in all AR apps
	private void StartARKit()
	{
		Application.targetFrameRate = 60;
		ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();
		config.planeDetection = UnityARPlaneDetection.Horizontal;
		config.alignment = UnityARAlignment.UnityARAlignmentGravity;
		config.getPointCloudData = true;
		config.enableLightEstimation = true;
		mSession.RunWithConfig(config);

		Debug.Log ("ARkit Started");
	}
	
	// Update is called once per frame
	void Update () {
		// should be true first time, false every other time?
		//		Debug.Log ("update called " + runLoadMap );

		if (!LibPlacenote.Instance.Initialized () ) {
//			notifications.text = "SDK not yet initialized";
			return;
		}

		if ( (runLoadMap == true) && (LibPlacenote.Instance.Initialized ()) ) {
			//this should be called ONCE, should output true
			//			Debug.Log ("sdk initialized, running load map" + runLoadMap);
			LoadMap ();
			// set run Load map to false so when update is called it wont run this conditional again
			runLoadMap = false;
		} 
	}

	public void LoadMap() {
		//this should be called ONCE, should output false
		//		Debug.Log ("load map called!" + runLoadMap);
		LibPlacenote.Instance.LoadMap(mapId, 
			(completed, faulted, percentage) =>   
			{
				if (completed) {

					LibPlacenote.Instance.StartSession();
//					notifications.text = "Trying to Localize Map: " + mapId;

				}
				else if (faulted) {
//					notifications.text = "Failed to load ID: " + mapId;
				}
				else {
//					notifications.text = "Download Progress: " + percentage.ToString("F2") + "/1.0)";
				}
			}
		);

	}
}
	
