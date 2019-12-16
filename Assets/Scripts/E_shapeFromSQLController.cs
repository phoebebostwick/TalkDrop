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
public struct AudioClipData 
{
	public int frequency;
	public int channels;
	public float[] samples;
}

public class E_shapeFromSQLController : MonoBehaviour
{
	public string addURL = "http://phoebebostwick.com/seniorproject/add.php";
	public string getURL = "http://phoebebostwick.com/seniorproject/get.php";
	public Text fetchResults;

	//a placeholder clip
	public AudioClip testClip;
	public float[] samples;
	public GameObject audioSphere;

	//this needs to be stored in the placenote map
	string timeStamp = DateTime.Now.ToString ("MMddyyyyHHmmss");

	void Start() {
		Debug.Log(timeStamp);
		//	StartCoroutine(GetScores());
	}

	//this will need to be called when the check/submit recording button is clicked
	//right now this is just called when you click the post button
	public void PostAudio() {
		float[] samples = new float[testClip.samples * testClip.channels];
		testClip.GetData (samples, 0);
		Debug.Log ( "samples.length is " + samples.Length); //142848
		FloatToByte (samples);
	}


	public void FloatToByte(float[] samples) {
		Debug.Log("FloatToByte is being passed samples with length: " + samples.Length);

		// float array -> byte array. create a byte array and copy the floats into it...
		byte[] floatToByteArray = new byte[samples.Length * 4];
		Buffer.BlockCopy(samples, 0, floatToByteArray, 0, floatToByteArray.Length);
		StartCoroutine(PostScores(floatToByteArray, timeStamp));
	}
		

	public void ByteToFloat(byte[] results) {
		// byte array -> float array. create a second float array and copy the bytes into it...
		float[] byteToFloatArray = new float[results.Length / 4];
		Buffer.BlockCopy(results, 0, byteToFloatArray, 0, byteToFloatArray.Length);
		Debug.Log (byteToFloatArray.Length); //142852
//		CreateAudioClip(byteToFloatArray);
	}

	//idk if this makes sense. audio clip should already be created, instance already exists
	public void CreateAudioClip(float[] audioFloat) {
		GameObject newInstance = Instantiate (audioSphere, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
		AudioSource newAS = newInstance.GetComponent<AudioSource> ();
		testClip = AudioClip.Create ("clipname", audioFloat.Length / testClip.channels, testClip.channels, testClip.frequency, false);
//		testClip.SetData (testClip.samples, 0);
		newAS.clip = testClip;
	}


	//for debugging
	public void GetTheScores()
	{
		
		StartCoroutine(GetScores(timeStamp));
	}

	//posting
	IEnumerator PostScores(byte[] bytes, string id)
	{
		//returns 571392
		Debug.Log ("in PostScores bytes length: " + bytes.Length);

		WWWForm form = new WWWForm();
		form.AddBinaryData("audioPost", bytes);
		form.AddField ("indexPost", id);

		using (UnityWebRequest www = UnityWebRequest.Post (addURL, form)) {
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
		

	IEnumerator GetScores(string id)
	{
		Debug.Log ("calling get scores");

		WWWForm form = new WWWForm();
		form.AddField ("indexPost", id);

		using (UnityWebRequest www = UnityWebRequest.Post (getURL, form)) {
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				// Show results as text
				//fetchResults.text = www.downloadHandler.text;

				byte[] results = www.downloadHandler.data;
				// returns System.byte[]
				Debug.Log (www.downloadHandler.data);
				ByteToFloat (results);
			}
		}
			
	}
}