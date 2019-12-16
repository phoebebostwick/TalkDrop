using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;


[System.Serializable]
public struct AudioClipDataJSON 
{
	public int frequency;
	public int channels;
	public float[] samples;
}

[System.Serializable]
public class ShapeInfoJSON
{
//	public float px;
//	public float py;
//	public float pz;
//	public float qx;
//	public float qy;
//	public float qz;
//	public float qw;
//	public int shapeType;
//	public int colorType;
	public AudioClipDataJSON clipData; // TODO: replace with simple string/int ID for the audio data
}

public class E_shapeFromSQLController_JSON : MonoBehaviour {

	public string addScoreURL = "http://phoebebostwick.com/seniorproject/addShapetoTable.php";
	public string highscoreURL = "http://phoebebostwick.com/seniorproject/getShapeFromTable.php";
	public Text fetchResults;

	//this is the input field in the scene
	public Text nameText;
	//a placeholder clip
	public AudioClip testClip;
	public float[] samples;

	void Start() {
		ShapeInfoJSON shapeInfo = new ShapeInfoJSON();
//		shapeInfo.clipData = testClip; 
	}

	//this will need to be called when the check/submit recording button is clicked
	//right now this is just called when you click the post button
	public void PostAudio() {

		float[] samples = new float[testClip.samples * testClip.channels];
		testClip.GetData (samples, 0);
		Debug.Log ( "samples.length is " + samples.Length);
		FloatToByte (samples);
	}


	public void FloatToByte(float[] samples) {
		Debug.Log("FloatToByte is being passed samples with length: " + samples.Length);

		// float array -> byte array. create a byte array and copy the floats into it...
		byte[] floatToByteArray = new byte[samples.Length * 4];
		Buffer.BlockCopy(samples, 0, floatToByteArray, 0, floatToByteArray.Length);

//		StartCoroutine(PostScores(nameText.text, floatToByteArray));
	}


	public void ByteToFloat() {
		// byte array -> float array. create a second float array and copy the bytes into it...
		//		float[] byteToFloatArray = new float[floatToByteArray.Length / 4];
		//		Buffer.BlockCopy(floatToByteArray, 0, byteToFloatArray, 0, floatToByteArray.Length);

		// do we have the same sequence of floats that we started with?
		//		Debug.Log(samples == byteToFloatArray);    // True
	}


	//for debugging
	public void GetTheScores()
	{
		StartCoroutine(GetScores());
	}

	//posting
	IEnumerator PostScores(ShapeInfoJSON shapeInfo)
	{
//		Debug.Log ("in PostScores bytes length: " + bytes.Length);

//		WWWForm form = new WWWForm();
//		form.AddField("namePost", name);
//		form.AddBinaryData("audioPost", bytes);

		string jsonData = JsonUtility.ToJson(shapeInfo);
		string method = "CreateAudio";

		using (UnityWebRequest www = UnityWebRequest.Put (addScoreURL + method, jsonData)) {
			www.method = "POST";
			www.SetRequestHeader("Content-Type", "application/json");
			www.SetRequestHeader("Accept", "application/json");




			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				//This was logged
				Debug.Log("Form upload complete!");
			}
		}
	}


	//This co-rutine gets the score, and print it to a text UI element.
	IEnumerator GetScores()
	{
		UnityWebRequest www = UnityWebRequest.Get(highscoreURL);
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
		}
		else {
			// Show results as text
			Debug.Log(www.downloadHandler.text);
			fetchResults.text = www.downloadHandler.text;

			// Or retrieve results as binary data
			byte[] results = www.downloadHandler.data;
			print (results);
		}
	}
}