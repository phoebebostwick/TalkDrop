using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.XR.iOS; 
using System;
using TMPro;
using UnityEngine.Networking;

public class LoadMap_Master : MonoBehaviour, PlacenoteListener {

	// Unity ARKit Session handler
	private UnityARSessionNativeInterface mSession;
	string getMapIdURL = "http://phoebebostwick.com/seniorproject/mapID.php?t=";
	string mapId;

	//set global boolean
	bool runLoadMap = false;
	bool onboardingDone = false;
	public GameObject onboardingBubble;
	public GameObject onboardingPanel;
	public bool isOffsiteApp;
	string TimeStamp;
	public TextMeshProUGUI ScanningText;
	string[] ScanningTextArray = new string[] {"Point camera at the Drop Zone", "Move camera slowly", "Keep going, almost there!"};

	// Use this for initialization
	void Start () {
		InvokeRepeating("ChangeScanningText", 4.0f, 4.0f);

		Input.location.Start ();

		// Start ARKit using the Unity ARKit Plugin
		mSession = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
		StartARKit ();
		LibPlacenote.Instance.RegisterListener (this);


		StartCoroutine (GetMapId());
		ScanningText.text = ScanningTextArray [0];

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

	IEnumerator GetMapId() {
		Debug.Log (TimeStamp + "calling get map id");
		TimeStamp = DateTime.Now.ToString ("MMddyyyyHHmmffff");


		WWWForm form = new WWWForm();


		using (UnityWebRequest www = UnityWebRequest.Post ((getMapIdURL + TimeStamp), form)) {
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				Debug.Log (TimeStamp + "get map id " + www.downloadHandler.text);

				//get array of ids
				mapId = www.downloadHandler.text;
				//set load map to true, but also needs pn sdk to be initialized
				runLoadMap = true;
			}
		}

	}

	public void ChangeScanningText() {
		int currentIndex = 0;
		for (int i = 0; i < ScanningTextArray.Length; ++i) {
			if (ScanningTextArray[i] == ScanningText.text) {
				currentIndex = i;
			}
		}
		RectTransform rt = ScanningText.GetComponent<RectTransform>();
		currentIndex = (currentIndex + 1) % ScanningTextArray.Length;
		ScanningText.text = ScanningTextArray[currentIndex];
		if (currentIndex == 2) {
			rt.sizeDelta = new Vector2(600, 50);
		}
		if (currentIndex == 0) {
			rt.sizeDelta = new Vector2(500, 50);
		}

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
			Debug.Log (TimeStamp + "sdk initialized, running load map" + runLoadMap);
			LoadMap ();
			// set run Load map to false so when update is called it wont run this conditional again
			runLoadMap = false;
		} 
	}




	public void LoadMap() {
		//this should be called ONCE, should output false
		Debug.Log (TimeStamp + "load map called!" + runLoadMap);

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
					if (isOffsiteApp) {
						onboardingBubble.transform.position = Camera.main.transform.position + Camera.main.transform.forward * .5f;
						onboardingBubble.SetActive(true);
						onboardingPanel.SetActive(false);
					}

				Debug.Log(TimeStamp + "Loaded Map: " + mapId);

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
				if (isOffsiteApp == false) {
					Debug.Log (TimeStamp + "localized app");
					onboardingBubble.transform.position = Camera.main.transform.position + Camera.main.transform.forward * .5f;
					onboardingBubble.SetActive (true);
					onboardingPanel.SetActive (false);
					onboardingDone = true;
				}
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


