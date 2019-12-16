using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;


public class CustomizationController_BetaDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
		
	public void SetShapeColor() {

		//  // Check if finger is over a UI element
        // if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))

		GameObject ShapeInstance = GameObject.FindWithTag ("createdPrefab");
		//set selected color to prefab color
		//ShapeInstance.GetComponent<MeshRenderer> ().material.color = this.GetComponent<Image>().material.color;
		Debug.Log("called set shape color");
		ShapeInstance.transform.Find("ColorBubble").GetComponent<MeshRenderer> ().material.color = this.GetComponent<Image>().material.color;
	}

}
