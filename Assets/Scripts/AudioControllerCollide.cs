using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControllerCollide : MonoBehaviour {


    public AudioSource AS;
    public GameObject ColliderCube;


	// Use this for initialization
	void Start () {
        AS = ColliderCube.GetComponent<AudioSource>();	
	}

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.name == "CameraCollider"){
            AS.Play ();
            Debug.Log("collided");
        }

        Debug.Log("public audio");
    }

    void OnTriggerExit(Collider collision)
    {
        AS.Stop ();
        Debug.Log("stopping audio");  

    }

    // Update is called once per frame
    void Update () {
		
	}
}
