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


public class GameController_Moderation : MonoBehaviour {

	public GameObject destroyBubbleButton;
	string deleteBubbleURL = "http://phoebebostwick.com/seniorproject/deleteBubble.php";

	//destroy thought bubble, remove from database
	public void destroyBubble() {
		GameObject SelectedBubble = GameObject.FindWithTag ("isPlaying");
		StartCoroutine (DeleteBubble (SelectedBubble.name));
		Destroy (SelectedBubble);
	}

	IEnumerator DeleteBubble(string id) {

		WWWForm form = new WWWForm();
		form.AddField("IDPost", id);

		using (UnityWebRequest www = UnityWebRequest.Post(deleteBubbleURL, form)) {
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				Debug.Log ("delete instance " + www.downloadHandler.text);
			}		
		}

	}

}
