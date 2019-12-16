using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRayCast : MonoBehaviour {
    AudioSource AS;

	// Use this for initialization
	void Start () {
        AS = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        int fingerCount = 0;
        foreach (Touch touch in Input.touches){
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;
        }
        if (fingerCount > 0)
            print("user has" + fingerCount + "finger(s) touching the screen");
        
        if (Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                Debug.Log("touchme");
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
        
        if(Physics.Raycast (ray, out hit, 100)) {
            if (hit.collider.transform == transform) {
                if (AS.isPlaying == false) {
                    Debug.Log("hit deteched! play audio");
                    AS.Play();
                } else {
                    Debug.Log("hit deteched! stop audio");
                    AS.Stop();
                }

            }
        }
	}
        }
    }
}
