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
public struct P_AudioClipData 
{
	public int frequency;
	public int channels;
	public float[] samples;
}

public class P_shapeFromSQLController : MonoBehaviour
{
	public string addScoreURL = "http://phoebebostwick.com/seniorproject/addShapetoTable.php";
	public string highscoreURL = "http://phoebebostwick.com/seniorproject/getShapeFromTable.php";
	public Text fetchResults;

	//this is the input field in the scene
	public Text nameText;
	//a placeholder clip
	public AudioClip testClip;
	public float[] samples;
	public string byteString;
	public byte[] stringByte;

	void Start() {
		//	StartCoroutine(GetScores());
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

		//byte array -> string
		byteString = Convert.ToBase64String(floatToByteArray);
		Debug.Log ("floatToByteArray as string: " + byteString);

		//string -> byte array
		byte[] stringByte = Convert.FromBase64String (byteString);

		//are the original byte array and the new one the same?
		//this is the fast but less accurate way to verify they are the same
		//the commented out block below actually goes through and checks each value against each other
		//but it takes a long time
		if (stringByte.Length == floatToByteArray.Length) {
			Debug.Log ("byte arrays match");
		} else {
			Debug.Log ("byte arrays do not match");
		}

		//The second parameter, samples, I'm only passing so that the new float can be checked
		//against the original float.
		ByteToFloat(stringByte, samples);


		//This confirms the arrays match, but it takes for fucking ever
//		for(var i = 0; i < stringByte.Length; i++)
//		{
//			//If we find values at any point that don't match, return false
//			if (stringByte [i] != floatToByteArray [i]) {
//				Debug.Log ("it doesn't match?");
//			} else {
//				Debug.Log ("it does match?");
//			}
//				
//		}


		//StartCoroutine(PostScores(nameText.text, floatToByteArray));
	}
		

	public void ByteToFloat(byte[] bytes, float[] samplescheck) {
		//byte array -> float array. create a second float array and copy the bytes into it...
		float[] byteToFloatArray = new float[bytes.Length / 4];
		Buffer.BlockCopy(bytes, 0, byteToFloatArray, 0, byteToFloatArray.Length);

		 //Checks if the original float array is the same length as the new float array
		//again just the quick and dirty way to check if they're the same
		if (samplescheck.Length == byteToFloatArray.Length) {
			Debug.Log ("float arrays match");
		} else {
			Debug.Log ("float arrays do not match");
		}

		//checks if the original float arry and the new have the same values
		//takes a long time
		//also this one returns both answers to me, despite being the same length, which
		//rules out that they're same to a point, and one of them has extra values at the end
//		for(var i = 0; i < byteToFloatArray.Length; i++) {
//						//If we find values at any point that don't match, return false
//			if (byteToFloatArray [i] == samplescheck [i]) {
//				Debug.Log ("it does match");
//			} else {
//				Debug.Log ("it does not match");
//			}
//							
//		}


	}


	//for debugging
//	public void GetTheScores()
//	{
//		StartCoroutine(GetScores());
//	}

	//posting
	IEnumerator PostScores(string name, byte[] bytes)
	{
		Debug.Log ("in PostScores bytes length: " + bytes.Length);

		WWWForm form = new WWWForm();
		form.AddField("namePost", name);
		form.AddBinaryData("audioPost", bytes);

		using (UnityWebRequest www = UnityWebRequest.Post (addScoreURL, form)) {
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				//This was logged
				Debug.Log("Form upload complete!");
				Debug.Log ("download handler text is: " + www.downloadHandler.text);
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