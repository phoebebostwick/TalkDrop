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
public struct ClipData_Master_LO 
{
	public int frequency;
	public int channels;
	public float[] samples;
}

public class GameController_Master_ListenOnly : MonoBehaviour {
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

	public GameObject audioSpherePrefab;
	public AudioClip tempClip; // used when recordings
	public GameObject MainCamera;
	public AudioSource populatingSound;

	public Material coreColor;
	public Material YellowCore;
	public Material RedCore;
	public Material GreenCore;
	public Material OrangeCore;
	public Material PurpleCore;
	public Material BlueCore;
	public Material[] bubbleMaterials;

	void Start() {

		timeSinceLastLoaded = new Stopwatch();

		//		CallGetAllIds ();
	}
		

	public void CallGetAllIds() {
		StartCoroutine(GetAllIds());
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
