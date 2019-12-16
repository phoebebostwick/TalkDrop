using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_LookAt : MonoBehaviour
{
	Transform target;
	public float speed;

	void Start() {
		target = GameObject.FindWithTag ("MainCamera").transform;
		Debug.Log ("target transform rotation " + target.rotation);
	}
	void Update()
	{
		// Rotate the camera every frame so it keeps looking at the target
		Quaternion OriginalRot = transform.rotation;
		transform.LookAt(target);
		Quaternion NewRot = transform.rotation;
		transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, speed * Time.deltaTime);
	}
}