using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe_Master2D : MonoBehaviour {

	public bool tap, swipeLeft, swipeRight;
	public bool isDragging = false;
	public Vector2 startTouch, swipeDelta;
	public Transform screens;
	private Vector3 desiredPosition;
	
	void Update () {
		tap = swipeLeft = swipeRight = false;
		swipeDelta = Vector2.zero;

		int fingerCount = 0;
		foreach (Touch touch in Input.touches) {
			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				fingerCount++;
		}

		if (fingerCount > 0) {

			if (Input.touchCount > 0) {

				Touch touch = Input.GetTouch (0);
				if (touch.phase == TouchPhase.Began) {
					isDragging = true;
					tap = true;
					startTouch = touch.position;
					Debug.Log ("start touch is " + startTouch);
				} else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
					isDragging = false;
					Reset ();
				}

				if (isDragging) {
					Debug.Log ("is dragging is true");

					if (Input.touchCount > 0) {
						swipeDelta = Input.GetTouch (0).position - startTouch;
					}
				}

			}
		}


		if (swipeDelta.magnitude > 125) {

			Debug.Log ("swipe delta magnitude is " + swipeDelta.magnitude);

			//which direction are they swiping
			float x = swipeDelta.x;

			Debug.Log ("swipe delta x is " + x);

			if (x < 0) {
				Debug.Log ("swipe left is true");

				swipeLeft = true;
				desiredPosition += Vector3.left;
			} else {
				Debug.Log ("swipe right is true");

				swipeRight = true;
				desiredPosition += Vector3.right;
			}
			screens.transform.position = Vector3.MoveTowards (screens.transform.position, desiredPosition, 3f * Time.deltaTime);

			Reset ();
		}
			
	}

	public void Reset() {
		startTouch = swipeDelta = Vector2.zero;
		isDragging = false;
	}
}
