//------------------------------------------------------------
//MAIN SCRIPT CONTROLLER FOR BETA DEMO (INCLUDES ADD SHAPE, START AND STOP RECORDING)
//------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

// Classes to hold shape information

[System.Serializable]
public struct ClipData 
{
	public int frequency;
	public int channels;
	public float[] samples;
}

[System.Serializable]
public class ShapeInfoBeta
{
	public float px;
	public float py;
	public float pz;
	public float qx;
	public float qy;
	public float qz;
	public float qw;
	public int shapeType;
	public int colorType;
	public ClipData clipData; // TODO: replace with simple string/int ID for the audio data
	//add id using timestamp
}


[System.Serializable]
public class ShapeListBeta
{
	public ShapeInfoBeta[] shapes;
}


public class GameController_BetaDemo : MonoBehaviour {

		public List<ShapeInfoBeta> shapeInfoList = new List<ShapeInfoBeta>();
		public List<GameObject> shapeObjList = new List<GameObject>();
		public Material mShapeMaterial;
		public GameObject audioSpherePrefab;
		public GameObject StartRecordingButton;
		public GameObject recordingColor;

		public AudioClip tempClip; // used when recordings
		private Color[] colorTypeOptions = {Color.magenta, Color.yellow, Color.red};

		public void StartRecording () {
			Debug.Log ("start recording called");
			tempClip = Microphone.Start ("Built-in Microphone", true, 30, 11025);
			recordingColor.SetActive (true);
			
		}

		public void StopRecording () {
			Debug.Log ("stop recording called");
			recordingColor.SetActive (false);

			if (Microphone.IsRecording (null)) {
				Microphone.End (null);
			}
			
			ClipData tempClipData;
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
		
		// All shape management functions (add shapes, save shapes to metadata etc.
		public void AddShape(Vector3 shapePosition, Quaternion shapeRotation, ClipData clip)
		{
			Debug.Log ("add shape called");
			System.Random rnd = new System.Random();
			int colorType =  rnd.Next(0, 3);

			ShapeInfoBeta shapeInfo = new ShapeInfoBeta();
			shapeInfo.px = shapePosition.x;
			shapeInfo.py = shapePosition.y;
			shapeInfo.pz = shapePosition.z;
			shapeInfo.qx = shapeRotation.x;
			shapeInfo.qy = shapeRotation.y;
			shapeInfo.qz = shapeRotation.z;
			shapeInfo.qw = shapeRotation.w;
			shapeInfo.colorType = colorType;
			shapeInfo.clipData = clip; 
			shapeInfoList.Add(shapeInfo);

			GameObject shape = ShapeFromInfo(shapeInfo);

			shapeObjList.Add(shape);
		}

		public GameObject ShapeFromInfo(ShapeInfoBeta info)
		{
		//would do the web request here (call ienumerator func)? post x,y,z coordinates, tempclip, color
		GameObject newInstance = Instantiate (audioSpherePrefab, new Vector3(info.px, info.py, info.pz), new Quaternion(info.qx, info.qy, info.qz, info.qw));
		newInstance.gameObject.tag = "createdPrefab";
		AudioSource newAS = newInstance.GetComponent<AudioSource> ();
		tempClip = AudioClip.Create ("clipname", info.clipData.samples.Length / info.clipData.channels, info.clipData.channels, info.clipData.frequency, false);
		tempClip.SetData (info.clipData.samples, 0);
		newAS.clip = tempClip;
		Debug.Log("Newest Clip with freq " + info.clipData.frequency + ", " + info.clipData.channels + " channels, samples: " + info.clipData.samples.Length);
//		Debug.Log("audio source is " + newAS);

		newInstance.GetComponent<MeshRenderer>().material = mShapeMaterial;
		newInstance.GetComponent<MeshRenderer>().material.color = colorTypeOptions[info.colorType];
		return newInstance;

		}
		

		public void ClearShapes()
		{
			foreach (var obj in shapeObjList)
			{
				Destroy(obj);
			}
			shapeObjList.Clear();
			shapeInfoList.Clear();

			Debug.Log ("clearing shapes");
		}

		//refactor to only push id
		public JObject Shapes2JSON()
		{
			ShapeListBeta shapeList = new ShapeListBeta();
			shapeList.shapes = new ShapeInfoBeta[shapeInfoList.Count];
			for (int i = 0; i < shapeInfoList.Count; i++)
			{
				shapeList.shapes[i] = shapeInfoList[i];
			}

			return JObject.FromObject(shapeList);
		}
		
		//refactor to only load id	
		public void LoadShapesJSON(JToken mapMetadata)
		{
			//ClearShapes();
			if (mapMetadata is JObject && mapMetadata["shapeList"] is JObject)
			{
				ShapeListBeta shapeList = mapMetadata["shapeList"].ToObject<ShapeListBeta>();
				if (shapeList.shapes == null)
				{
					Debug.Log("no shapes dropped");
					return;
				}

				foreach (var shapeInfo in shapeList.shapes)
				{
					shapeInfoList.Add(shapeInfo);
					GameObject shape = ShapeFromInfo(shapeInfo);
					shapeObjList.Add(shape);
				}
			}
		}
}
	