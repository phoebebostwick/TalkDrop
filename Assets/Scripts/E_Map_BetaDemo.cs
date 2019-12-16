using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;
using UnityEngine.XR.iOS; 

public class E_Map_BetaDemo : MonoBehaviour {

	// Unity ARKit Session handler
	private UnityARSessionNativeInterface mSession;
	public string mapId;
	//set global boolean
	public bool runLoadMap = false;
	public GameObject onboardingBubble;
	public GameObject onboardingPanel;

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

	private string ReadMapIDFromFile() {
		string path = Application.persistentDataPath + "/mapID.txt";
		Debug.Log(path);

		if (System.IO.File.Exists(path))
		{
			StreamReader reader = new StreamReader(path);
			string returnValue = reader.ReadLine();

			Debug.Log(returnValue);
			reader.Close();

			return returnValue;
		}
		else
		{
			return null;
		}


	}

	public void LoadMap() {
		//this should be called ONCE, should output false
		Debug.Log ("load map called!" + runLoadMap);

		LibPlacenote.Instance.LoadMap(mapId, 
			(completed, faulted, percentage) =>   
			{
				if (completed) {

					LibPlacenote.Instance.StartSession();
					Debug.Log("Localized Map: " + mapId);
					//show onboarding bubble, hide onboarding ui
					onboardingBubble.SetActive(true);
					onboardingPanel.SetActive(false);

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

}

