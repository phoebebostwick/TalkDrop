using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;


public class P_CustomizationController_BetaDemo : MonoBehaviour {

	public Material YellowCore;
	public Material PurpleCore;
	public Material OrangeCore;
	public Material RedCore;
	public Material GreenCore;

	public Material coreColor;

	public Material[] newMaterial;

	// Use this for initialization
	void Start () {

	}

	public void SetShapeColor() {
		GameObject ShapeInstance = GameObject.FindWithTag ("createdPrefab");
		//set selected color to prefab color
		//		ShapeInstance.GetComponent<MeshRenderer> ().material.color = this.GetComponent<Image>().material.color;
		Debug.Log("option selected " + this.name);
		switch (this.name) {
		case "BubbleColor_Opt1":
			coreColor = YellowCore;
			break;
		case "BubbleColor_Opt2":
			coreColor = RedCore;
			break;
		case "BubbleColor_Opt3":
			coreColor = GreenCore;
			break;
		case "BubbleColor_Opt4":
			coreColor = OrangeCore;
			break;
		case "BubbleColor_Opt5":
			coreColor = PurpleCore;
			break;
		}
		//set element 0 on ColorBubble in Bubble_BetaDemo prefab to transparent play button in inspector for this
		//to work properly
		newMaterial = ShapeInstance.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials;
		newMaterial [1] = coreColor;
		ShapeInstance.transform.Find ("ColorBubble").GetComponent<MeshRenderer> ().materials = newMaterial;

	}

}
