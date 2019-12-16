using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;
using SimpleJSON;

public class E_SQLController_AllData : MonoBehaviour {
	string addURL = "http://phoebebostwick.com/seniorproject/add.php";
	string getURL = "http://phoebebostwick.com/seniorproject/getIDs.php";
	string getInstanceURL = "http://phoebebostwick.com/seniorproject/getInstance.php";

	Action<string> _createBubbleCallback;

	//a placeholder clip
	public AudioClip Clip;

	public GameObject BubblePrefab;

	string ShapeXPosition;
	string ShapeYPosition;
	string ShapeZPosition;
	string ShapeColor;

	float BubbleXPosition;
	float BubbleYPosition;
	float BubbleZPosition;

	string TimeStamp = DateTime.Now.ToString ("MMddyyyyHHmmss"); //this needs to be stored in the placenote map
	float[] samples;


	void Start () {
		_createBubbleCallback = (jsonArray) => {
			StartCoroutine(CreateBubblesRoutine(jsonArray));
		};

		CreateBubbles ();
	}


	//this will need to be called when the check/submit recording button is clicked
	public void SaveAudio() {
		float[] samples = new float[Clip.samples * Clip.channels];
		Clip.GetData (samples, 0);
//		Debug.Log ( "samples.length is " + samples.Length); //142848
		FloatToByte (samples);

	}

	// float array -> byte array
	public void FloatToByte(float[] samples) {
//		Debug.Log("FloatToByte is being passed samples with length: " + samples.Length);
		byte[] floatToByteArray = new byte[samples.Length * 4];
		Buffer.BlockCopy(samples, 0, floatToByteArray, 0, floatToByteArray.Length);

		//Get shape position and color data
		//Done in this function because the values need to be passed in AddValues
		ShapeXPosition = GameObject.Find("testPrefab").transform.position.x.ToString();
		ShapeYPosition = GameObject.Find("testPrefab").transform.position.y.ToString ();
		ShapeZPosition = GameObject.Find("testPrefab").transform.position.z.ToString ();
		ShapeColor = GameObject.Find("testPrefab").GetComponent<MeshRenderer>().material.color.ToString ();

		StartCoroutine(AddValues(floatToByteArray, TimeStamp, ShapeXPosition, ShapeYPosition, ShapeZPosition, ShapeColor));
	}

	IEnumerator AddValues(byte[] bytes, string id, string xPosition, string yPosition, string zPosition, string color) {
		//returns 571392
//		Debug.Log ("in PostScores bytes length: " + bytes.Length);
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
				Debug.Log("Form upload complete!");
				Debug.Log ("download handler text is: " + www.downloadHandler.text);
			}
		}
	}

	//for debugging
//	public void CallGetValues() {
//		StartCoroutine(GetValues(TimeStamp));
//	}
//
//	IEnumerator GetValues(string id) {
////		Debug.Log ("calling get scores");
//		WWWForm form = new WWWForm();
//		form.AddField ("indexPost", id);
//
//		using (UnityWebRequest www = UnityWebRequest.Post (getURL, form)) {
//			yield return www.SendWebRequest();
//			if (www.isNetworkError || www.isHttpError) {
//				Debug.Log (www.error);
//			} else {
//				byte[] bytes = www.downloadHandler.data;
//				ByteToFloat (bytes);
//			}
//		}
//
//	}

	// byte array -> float array
	public void ByteToFloat(byte[] bytes) {
		float[] byteToFloatArray = new float[bytes.Length / 4];
		Buffer.BlockCopy(bytes, 0, byteToFloatArray, 0, byteToFloatArray.Length);
		Debug.Log (byteToFloatArray.Length); //142852
	}

	IEnumerator GetBubbleIDs(System.Action<string> callback){
		using (UnityWebRequest www = UnityWebRequest.Get(getURL)) {
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				Debug.Log (www.downloadHandler.text);
				string jsonArray = www.downloadHandler.text;

				callback (jsonArray);
			}		
		}
			
	}

	public void CreateBubbles(){
		StartCoroutine (GetBubbleIDs(_createBubbleCallback));
	}

	IEnumerator CreateBubblesRoutine(string jsonArrayString){
		//Parsing json array string as an array
		//none of the JSON functions work yet, need to import a json file from tutorial, or figure out whats happening with the using json above
		JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;
		print (jsonArray);

		for (int i = 0; i < jsonArray.Count; i++) {
			//Create local variables
			bool isDone = false;
			string bubbleId = jsonArray[i].AsObject["id"];
			JSONObject bubbleInfoJson = new JSONObject();

			Action<string> getBubbleInfoCallback = (bubbleInfo) => {
				isDone = true;
				JSONArray tempArray = JSON.Parse(bubbleInfo) as JSONArray;
				bubbleInfoJson = tempArray[0].AsObject;
			};
			StartCoroutine(GetBubble(bubbleId, getBubbleInfoCallback));

			yield return new WaitUntil (() => isDone == true);

			BubbleXPosition = float.Parse(bubbleInfoJson["x"]);
			BubbleYPosition = float.Parse(bubbleInfoJson["y"]);
			BubbleZPosition = float.Parse(bubbleInfoJson["z"]);
			Vector3 BubblePosition = new Vector3 (BubbleXPosition, BubbleYPosition, BubbleZPosition);
			GameObject bubble = Instantiate(BubblePrefab, BubblePosition, transform.rotation);



			//need to do string -> float 
			//either Float.Parse(bubbleInfoJson["x"]) = xPosition, or just bubble.transform.position.x = Float.Parse(bubbleInfoJson["x"]);
			//FromString is not the real function

//			bubble.GetComponent<MeshRenderer>().material.color = bubbleInfoJson["color"];

		}

		yield return null;
	}

	IEnumerator GetBubble(string bubbleID, System.Action<string> callback){
		WWWForm form = new WWWForm();
		form.AddField("indexPost", bubbleID);
		using (UnityWebRequest www = UnityWebRequest.Post(getInstanceURL, form)) {
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				Debug.Log (www.downloadHandler.text);
				string jsonArray = www.downloadHandler.text;

				callback (jsonArray);
			}		
		}

	}
	
	// Update is called once per frame
	void Update () {}


}
