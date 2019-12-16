using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ColorSelect : MonoBehaviour {

	public GameObject spherePrefab;

	public void changeColorOfSphere(Material mat) {
		
		spherePrefab.GetComponent<Renderer>().material = mat;

	}
}
