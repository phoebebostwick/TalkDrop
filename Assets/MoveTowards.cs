using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour {

    public Transform target;
	public float speed = 1.0f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float step = speed * Time.deltaTime; // calculate distance to move
		transform.position = Vector3.MoveTowards(transform.position, target.position, step);
	}
}
