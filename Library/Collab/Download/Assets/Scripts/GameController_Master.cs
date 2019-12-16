//------------------------------------------------------------
//MAIN SCRIPT CONTROLLER FOR BETA DEMO (INCLUDES ADD SHAPE, START AND STOP RECORDING)
//------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Collections.Generic;
using SimpleJSON;
using System.Diagnostics;
using Debug=UnityEngine.Debug;
using System.Linq;



// Classes to hold shape information

[System.Serializable]
public struct ClipData_Master 
{
	public int frequency;
	public int channels;
	public float[] samples;
}

public class GameController_Master : MonoBehaviour {
	string addURL = "http://phoebebostwick.com/seniorproject/add.php";
	string getIdsURL = "http://phoebebostwick.com/seniorproject/getIDs.php?t=";
	string getInstanceURL = "http://phoebebostwick.com/seniorproject/getInstance.php";
	string getAudioURL = "http://phoebebostwick.com/seniorproject/getAudio.php";

//	string refreshURL = "http://phoebebostwick.com/seniorproject/refreshIDs.php?t=";


	//a placeholder clip
	//	public AudioClip Clip;

	string ShapeXPosition;
	string ShapeYPosition;
	string ShapeZPosition;
	string ShapeColor;
	string TimeStamp;
	string TimeFirstLoaded;

	string timeSinceLastCall;
	long timeSinceLastCallLong;

	public Stopwatch timeSinceLastLoaded;

	public int recordingLength;
	public GameObject audioSpherePrefab;
	public AudioClip tempClip; // used when recordings
	public GameObject MainCamera;
	public Stopwatch recordingLengthCheck;
	public GameObject RecordingControlPanel;
	public AudioSource populatingSound;
	public GameObject refreshButton;
	public GameObject customizationPanel;

	public Image recordingStroke;
	public Image enlargedRecordingStroke;
	private float recordingCurrentLength;
	public GameObject enlargedRecordingGraphic;
	public GameObject recordingFill;
	public GameObject refreshAnim;



	public Material coreColor;
	public Material YellowCore;
	public Material RedCore;
	public Material GreenCore;
	public Material OrangeCore;
	public Material PurpleCore;
	public Material BlueCore;
	public Material[] bubbleMaterials;


	//for moderation app
	public Boolean isModerationApp;


	void Start() {

		recordingLengthCheck = new Stopwatch();
		timeSinceLastLoaded = new Stopwatch();

		if (isModerationApp) {
			CallGetAllIds ();
		}

//		CallGetAllIds ();
	}

	void Update() {
		//fill % red stroke on recording icon over 10 seconds
		if (Microphone.IsRecording (null)) {
			
			enlargedRecordingGraphic.SetActive (true);
			recordingFill.SetActive (true);
			recordingCurrentLength += Time.deltaTime * 0.1f;
			recordingStroke.fillAmount = enlargedRecordingStroke.fillAmount = recordingCurrentLength;

			recordingLength = Microphone.GetPosition(null);
			if (Microphone.GetPosition (null) >= 436000) {
				//Prevent recordings from being over 10 seconds by automatically stopping them if the exceed that length
				StopRecording ();
			}
		}
	}
		
	public void CallGetAllIds() {
		StartCoroutine(GetAllIds());
	}


	public void StartRecording () {
		Debug.Log ("start recording called");
		Debug.Log (null);
		tempClip = Microphone.Start ("Built-in Microphone", true, 10, 44100);
		recordingLengthCheck.Start();
	}

	public void StopRecording () {
		Debug.Log ("stop recording called");

		if (Microphone.IsRecording (null)) {
			Microphone.End (null);
		}
			
		recordingLengthCheck.Stop();
		//reset red stroke on recording icon
		recordingStroke.fillAmount = enlargedRecordingStroke.fillAmount = recordingCurrentLength = 0;
		enlargedRecordingGraphic.SetActive (false);
		recordingFill.SetActive (false);

		if (recordingLengthCheck.ElapsedMilliseconds < 1000) {
			Debug.Log ("tap not long enough");
			//If the recording length is too short, delete recording and show tool tip to prevent accidents/spamming
			RecordingControlPanel.GetComponent<RecordingController_Master> ().ShortRecordingNotice ();
		} else {
			recordingLengthCheck.Reset();
			RecordingControlPanel.GetComponent<RecordingController_Master> ().showRecordingControls ();

			ClipData_Master tempClipData;
			tempClipData.frequency = tempClip.frequency;
			tempClipData.channels = tempClip.channels;
			float[] samples = new float[tempClip.samples * tempClip.channels];
			tempClip.GetData (samples, 0);
			tempClipData.samples = samples;
			Debug.Log ("New Clip with freq " + tempClipData.frequency + ", " + tempClipData.channels + " channels, samples: " + tempClipData.samples.Length);

			// shape position
			Vector3 dropPosition = (Camera.main.transform.position + Camera.main.transform.up * -0.01f + Camera.main.transform.forward * 0.5f);
			Quaternion dropRotation = Quaternion.identity;

			//check if thoughtbubble has already been instantiated because of a 15 second long clip
			if (GameObject.FindWithTag("createdPrefab") == null) {
				AddShape (dropPosition, dropRotation, tempClipData);
			} 
		}
	}


	public GameObject AddShape(Vector3 ShapePosition, Quaternion ShapeRotation, ClipData_Master clip) {
		//create prefab and get position and rotation
		GameObject newInstance = Instantiate (audioSpherePrefab, ShapePosition, ShapeRotation);
		newInstance.gameObject.tag = "createdPrefab"; //set prefab tag
		AudioSource newAS = newInstance.GetComponent<AudioSource> (); //get audio source component on new instance

		//create clip
		AudioClip ZaddyClip = AudioClip.Create ("clipname", recordingLength / clip.channels, clip.channels, clip.frequency, false);
		ZaddyClip.SetData (clip.samples, 0);
		newAS.clip = ZaddyClip;
		Debug.Log("Newest Clip with freq " + clip.frequency + ", " + clip.channels + " channels, samples: " + clip.samples.Length); //110250 

		//lock instance to camera
		newInstance.transform.parent = MainCamera.transform;
		return newInstance;
	}


	//called when save recording button is clicked
	public void SaveShapeInfo(GameObject ShapeInstance) {
		//generate unique id
		TimeStamp = DateTime.Now.ToString ("MMddyyyyHHmmffff");
		AudioSource newestAS = ShapeInstance.GetComponent<AudioSource> ();
		float[] samples = new float[newestAS.clip.samples * newestAS.clip.channels];
		newestAS.clip.GetData (samples, 0);

		Debug.Log("Saved Clip with freq " + newestAS.clip.frequency + ", " + newestAS.clip.channels + " channels, samples: " + newestAS.clip.samples);

		// float array -> byte array
		byte[] floatToByteArray = new byte[samples.Length * 4];
		Buffer.BlockCopy(samples, 0, floatToByteArray, 0, floatToByteArray.Length);

		//Get shape position and color data
		ShapeXPosition = ShapeInstance.transform.position.x.ToString ();
		ShapeYPosition = ShapeInstance.transform.position.y.ToString ();
		ShapeZPosition = ShapeInstance.transform.position.z.ToString ();
		ShapeColor = ShapeInstance.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials [1].ToString ();
		String ShapeColorName = ShapeColor.Remove(ShapeColor.IndexOf("_"));

		StartCoroutine(AddValues(floatToByteArray, TimeStamp, ShapeXPosition, ShapeYPosition, ShapeZPosition, ShapeColorName));
	}


	IEnumerator AddValues(byte[] bytes, string id, string xPosition, string yPosition, string zPosition, string color) {
		WWWForm form = new WWWForm();
		form.AddBinaryData("audioPost", bytes);
		form.AddField ("indexPost", id);
		form.AddField ("xPositionPost", xPosition);
		form.AddField ("yPositionPost", yPosition);
		form.AddField ("zPositionPost", zPosition);
		form.AddField ("colorPost", color);

		using (UnityWebRequest www = UnityWebRequest.Post (addURL, form)) {
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				Debug.Log ("form upload complete! download handler text is: " + www.downloadHandler.text);
			}
		}
	}

	public void CallStopRefreshAnimation() {
		StartCoroutine(StopRefreshAnimation());
	}

	public void RefreshDrops() {

		refreshAnim.SetActive(true);
		refreshButton.SetActive(false); 
		CallStopRefreshAnimation ();


		//check if at least 30 seconds has gone by since the last CallGetAllIds
		if (timeSinceLastLoaded.ElapsedMilliseconds > 30000) { 
			Debug.Log ("refresh called");

			timeSinceLastLoaded.Stop ();
			timeSinceLastLoaded.Reset ();

//			var DropsToDestroy = GameObject.FindGameObjectsWithTag ("placedPrefab");
//
//			for (var i = 0; i < DropsToDestroy.Length; i++) {
//				Destroy (DropsToDestroy [i]);
//			}

			CallGetAllIds ();
		} else {
			//tooltip that says to wait.... .SetActive();
			//may make a small co-routine that waits 5 seconds and sets it inactive again
			//which could also be used for adding too many bubbles tool tip, or look around tool tip
			//just like waitforseconds(5), then passed parameter.setinactive
//			refreshButton.interactable = false;

			Debug.Log("not enough time has gone by to refresh");
		}
	}



	IEnumerator StopRefreshAnimation()
 {
     yield return new WaitForSeconds(30);
     refreshAnim.SetActive(false);
	if (!customizationPanel.activeInHierarchy) {
		refreshButton.SetActive (true); 
	}
	 Debug.Log("refresh anim stopped");
}

	//called on start and refresh 
	IEnumerator GetAllIds() {
		Debug.Log ("calling get all ids");
		populatingSound.Play ();
		timeSinceLastLoaded.Start ();

		WWWForm form = new WWWForm();

		//if time first loaded has a value
		//commented out to get bubbles deleted from database through moderation
		if (timeSinceLastCall != null) {
			form.AddField ("IDPost", timeSinceLastCall);
		}

		Action<String> ParseJsonCallback = (jsonArrayStr) => {
			StartCoroutine(ParseJSON(jsonArrayStr));
		};

		using (UnityWebRequest www = UnityWebRequest.Post ((getIdsURL + TimeStamp), form)) {
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				Debug.Log ("get ids " + www.downloadHandler.text);

				//get array of ids
				string jsonArrayStr = www.downloadHandler.text;

				ParseJsonCallback (jsonArrayStr);
			}
		}

	}
		

	IEnumerator ParseJSON(string jsonArrayStr) {

		//parsing json array string as an array
		JSONArray jsonArray = JSON.Parse(jsonArrayStr) as JSONArray;

		for (int i = 0; i < jsonArray.Count; i++) {
			bool isDone = false;

			//get member of json array as an object
			string id = jsonArray [i].AsObject["id"];
			JSONObject jsonObject = new JSONObject();


			//create callback to download info
			Action<string> GetShapeInfoCallback = (ShapeInfo) => {
				isDone = true;
				JSONArray tempArray = JSON.Parse (ShapeInfo) as JSONArray;
				jsonObject = tempArray [0].AsObject;
				Debug.Log(jsonObject); //{"x":"0.4","y":"0.2","z":"0.1","color":"RGBA(1.000, 0.906, 0.147, 1.000)"}
			};

			//get instance info based on id
			StartCoroutine(GetInstance(id, GetShapeInfoCallback));

			//wait until callback is called
			yield return new WaitUntil (() => isDone == true);

			//must use double quotes!
			ShapeXPosition = jsonObject["x"];
			ShapeYPosition = jsonObject["y"];
			ShapeZPosition = jsonObject["z"];
			ShapeColor = jsonObject ["color"];
			//convert string values to floats
			float XPosition = float.Parse(ShapeXPosition);
			float YPosition = float.Parse(ShapeYPosition);
			float ZPosition = float.Parse(ShapeZPosition);
			Vector3 ShapePosition = new Vector3(XPosition, YPosition, ZPosition);
			var myBubbles = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "MyBubble");
			foreach (GameObject myBubble in myBubbles) {
				Destroy (myBubble);
			}
			ShowShapes (ShapePosition, Quaternion.identity, id, ShapeColor);
		
		}

	}


	IEnumerator GetInstance(string id, Action<string> GetShapeInfoCallback) {

		WWWForm form = new WWWForm();
		form.AddField("indexPost", id);

		using (UnityWebRequest www = UnityWebRequest.Post(getInstanceURL, form)) {
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				Debug.Log ("get instance " + www.downloadHandler.text);
				string jsonArray = www.downloadHandler.text;

				GetShapeInfoCallback (jsonArray);
			}		
		}

	}

	//instantiate shapes based on values in db
	public GameObject ShowShapes(Vector3 ShapePosition, Quaternion ShapeRotation, string id, string ShapeColor) {

		//create prefab and get position and rotation
		GameObject newerInstance = Instantiate (audioSpherePrefab, ShapePosition, ShapeRotation);
		//set name of gameobject to id - access this in order to delete them
		newerInstance.name = id;
		newerInstance.gameObject.tag = "placedPrefab"; //set prefab tag

		//set color
		switch (ShapeColor) {
		case "Yellow":
			coreColor = YellowCore;
			break;
		case "Red":
			coreColor = RedCore;
			break;
		case "Green":
			coreColor = GreenCore;
			break;
		case "Orange":
			coreColor = OrangeCore;
			break;
		case "Purple":
			coreColor = PurpleCore;
			break;
		case "Blue":
			coreColor = BlueCore;
			break;
		}



		bubbleMaterials = newerInstance.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials;
		bubbleMaterials [1] = coreColor;	
		newerInstance.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = bubbleMaterials;

		StartCoroutine (GetAudio (id, newerInstance));

		if (isModerationApp) {
			//save the timestamp from when all current bubbles from db were retrieved
			long idInt = long.Parse (id);
			if (idInt > timeSinceLastCallLong) {
				Debug.Log ("this is a new drop!");
				newerInstance.GetComponent<MeshRenderer> ().material.color = new Color (.75f, .75f, 1f, .5f);
			}
		}

			//if bubble.name > timefirstloaded (may need to do str int conversion) change color of outer bubble. if its less than..set to default color
		timeSinceLastCall = DateTime.Now.ToString ("MMddyyyyHHmmffff");
		timeSinceLastCallLong = long.Parse(timeSinceLastCall);


		return newerInstance;

	}

	IEnumerator GetAudio(string id, GameObject ShapeInstance) {

		WWWForm form = new WWWForm();
		form.AddField("indexPost", id);

		using (UnityWebRequest www = UnityWebRequest.Post(getAudioURL, form)) {
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				Debug.Log ("get audio " + www.downloadHandler.text);
				byte[] bytes = www.downloadHandler.data;
				//byte array -> float array
				Debug.Log(bytes.Length); 
				float[] byteToFloatArray = new float[bytes.Length / 4];
				Buffer.BlockCopy(bytes, 0, byteToFloatArray, 0, bytes.Length);
				Debug.Log ("parse json " + byteToFloatArray.Length);

				AudioSource newAS = ShapeInstance.GetComponent<AudioSource> (); //get audio source component on new instance

				//create clip
				tempClip = AudioClip.Create ("clipname", byteToFloatArray.Length / 1, 1, 44100, false);
				tempClip.SetData (byteToFloatArray, 0);
				newAS.clip = tempClip;
				Debug.Log("Show shapes Clip with samples: " + byteToFloatArray.Length);
			}		
		}

	}
		

}
