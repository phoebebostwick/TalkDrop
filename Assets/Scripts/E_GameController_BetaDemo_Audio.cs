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


// Classes to hold shape information

[System.Serializable]
public struct E_ClipData_Audio 
{
	public int frequency;
	public int channels;
	public float[] samples;
}

public class E_GameController_BetaDemo_Audio : MonoBehaviour {
	string addURL = "http://phoebebostwick.com/seniorproject/add.php";
	string getIdsURL = "http://phoebebostwick.com/seniorproject/getIDs.php";
	string getInstanceURL = "http://phoebebostwick.com/seniorproject/getInstance.php";
	string getAudioURL = "http://phoebebostwick.com/seniorproject/getAudio.php";


	//a placeholder clip
	//	public AudioClip Clip;

	string ShapeXPosition;
	string ShapeYPosition;
	string ShapeZPosition;
	string ShapeColor;
	string TimeStamp;

	public Material mShapeMaterial;
	public GameObject audioSpherePrefab;
	public AudioClip tempClip; // used when recordings
	public GameObject MainCamera;


	void Start() {
		//Commented out for E_BetaDemo_Onboarding scene
//		StartCoroutine(GetAllIds());
	}

	//for debugging
	//	public void CallGetAllIds() {
	//		StartCoroutine(GetAllIds());
	//	}

	public void StartRecording () {
		Debug.Log ("start recording called");
		tempClip = Microphone.Start ("Built-in Microphone", true, 10, 11025);
	}

	public void StopRecording () {
		Debug.Log ("stop recording called");

		if (Microphone.IsRecording (null)) {
			Microphone.End (null);
		}
		E_ClipData_Audio tempClipData;
		tempClipData.frequency = tempClip.frequency;
		tempClipData.channels = tempClip.channels;
		float[] samples = new float[tempClip.samples * tempClip.channels];
		tempClip.GetData (samples, 0);
		tempClipData.samples = samples;
		Debug.Log("New Clip with freq " + tempClipData.frequency + ", " + tempClipData.channels + " channels, samples: " + tempClipData.samples.Length);

		// shape position
		Vector3 dropPosition = (Camera.main.transform.position + Camera.main.transform.forward * .5f);
		Quaternion dropRotation = Quaternion.identity;

		AddShape(dropPosition, dropRotation, tempClipData);
	}


	public GameObject AddShape(Vector3 ShapePosition, Quaternion ShapeRotation, E_ClipData_Audio clip) {
		//create prefab and get position and rotation
		GameObject newInstance = Instantiate (audioSpherePrefab, ShapePosition, ShapeRotation);
		newInstance.gameObject.tag = "createdPrefab"; //set prefab tag
		AudioSource newAS = newInstance.GetComponent<AudioSource> (); //get audio source component on new instance

		//create clip
		AudioClip ZaddyClip = AudioClip.Create ("clipname", clip.samples.Length / clip.channels, clip.channels, clip.frequency, false);
		ZaddyClip.SetData (clip.samples, 0);
		newAS.clip = ZaddyClip;
		Debug.Log("Newest Clip with freq " + clip.frequency + ", " + clip.channels + " channels, samples: " + clip.samples.Length); //110250 

		//default material
		newInstance.transform.Find("ColorBubble").GetComponent<MeshRenderer>().material = mShapeMaterial;

		//		newInstance.GetComponent<MeshRenderer>().material = mShapeMaterial;
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
		Debug.Log ("save shape info " + floatToByteArray.Length); //441000

		//Get shape position and color data
		ShapeXPosition = ShapeInstance.transform.position.x.ToString ();
		ShapeYPosition = ShapeInstance.transform.position.y.ToString ();
		ShapeZPosition = ShapeInstance.transform.position.z.ToString ();
		ShapeColor = ShapeInstance.GetComponent<MeshRenderer>().material.color.ToString ();

		StartCoroutine(AddValues(floatToByteArray, TimeStamp, ShapeXPosition, ShapeYPosition, ShapeZPosition, ShapeColor));
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

	//called on start and refresh - to be added
	IEnumerator GetAllIds() {

		Action<String> ParseJsonCallback = (jsonArrayStr) => {
			StartCoroutine(ParseJSON(jsonArrayStr));
		};

		using (UnityWebRequest www = UnityWebRequest.Get (getIdsURL)) {
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
			//convert string values to floats
			float XPosition = float.Parse(ShapeXPosition);
			float YPosition = float.Parse(ShapeYPosition);
			float ZPosition = float.Parse(ShapeZPosition);
			Vector3 ShapePosition = new Vector3(XPosition, YPosition, ZPosition);

			ShowShapes (ShapePosition, Quaternion.identity, id);
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
	public GameObject ShowShapes(Vector3 ShapePosition, Quaternion ShapeRotation, string id) {

		//create prefab and get position and rotation
		GameObject newInstance = Instantiate (audioSpherePrefab, ShapePosition, ShapeRotation);
		newInstance.gameObject.tag = "placedPrefab"; //set prefab tag

		//default color and material (no customization)
		newInstance.transform.Find("ColorBubble").GetComponent<MeshRenderer>().material = mShapeMaterial;

		StartCoroutine(GetAudio(id, newInstance));

		return newInstance;
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
				Buffer.BlockCopy(bytes, 0, byteToFloatArray, 0, byteToFloatArray.Length);
				Debug.Log ("parse json " + byteToFloatArray.Length);

				AudioSource newAS = ShapeInstance.GetComponent<AudioSource> (); //get audio source component on new instance

				//create clip
				tempClip = AudioClip.Create ("clipname", byteToFloatArray.Length / 1, 1, 11025, false);
				tempClip.SetData (byteToFloatArray, 0);
				newAS.clip = tempClip;
				Debug.Log("Show shapes Clip with samples: " + byteToFloatArray.Length);
			}		
		}

	}

	//for debugging
	public void PlayAudio() {
		AudioSource newAS = GameObject.FindWithTag("placedPrefab").GetComponent<AudioSource> (); //get audio source component on new instance
		newAS.Play();
	}

}
