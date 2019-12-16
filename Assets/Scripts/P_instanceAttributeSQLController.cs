using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;


public class P_instanceAttributeSQLController : MonoBehaviour
{
	public string addShapeURL = "http://phoebebostwick.com/seniorproject/addToTable.php";
	public string getShapeURL = "http://phoebebostwick.com/seniorproject/getFromTable.php";
	public Text fetchShapes;

	//this is the input field in the scene
	public string ShapeXPosition;
	public string ShapeYPosition;
	public string ShapeZPosition;
	public string ShapeColor;

	void Start() {
		//	StartCoroutine(GetScores());
	}

	public void GetShapeInfo() {

		//set prefabs color for test purposes
		GameObject.Find("testPrefab").GetComponent<MeshRenderer>().material.color = Color.blue;

		//get positions/color of prefab and convert to string for passing to PostShape()
		ShapeXPosition = GameObject.Find("testPrefab").transform.position.x.ToString();
		ShapeYPosition = GameObject.Find("testPrefab").transform.position.y.ToString ();
		ShapeZPosition = GameObject.Find("testPrefab").transform.position.z.ToString ();
		ShapeColor = GameObject.Find("testPrefab").GetComponent<MeshRenderer>().material.color.ToString ();

		Debug.Log ("shape position: " + ShapeXPosition + ", " + ShapeYPosition + ", " + ShapeZPosition
			+ ", shape color: " + ShapeColor);

		StartCoroutine(PostShape(ShapeXPosition, ShapeYPosition, ShapeZPosition, ShapeColor));
	}

	//posting
	IEnumerator PostShape(string xPosition, string yPosition, string zPosition, string color)
	{

		WWWForm form = new WWWForm();
		form.AddField ("xPositionPost", xPosition);
		form.AddField ("yPositionPost", yPosition);
		form.AddField ("zPositionPost", zPosition);
		form.AddField("colorPost", color);

		using (UnityWebRequest www = UnityWebRequest.Post (addShapeURL, form)) {
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


	public void GetShapes(){
		StartCoroutine (GetScores ());
	}

	//This co-rutine gets the score, and print it to a text UI element.
	IEnumerator GetScores()
	{
		UnityWebRequest www = UnityWebRequest.Get(getShapeURL);
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
		}
		else {
			// Show results as text
			Debug.Log(www.downloadHandler.text);

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


		}
	}
}