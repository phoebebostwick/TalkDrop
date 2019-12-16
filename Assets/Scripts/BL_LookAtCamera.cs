using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	public Transform target;
    public float speed;

	void Update()
	{
		// Rotate the camera every frame so it keeps looking at the target
		Quaternion OriginalRot = transform.rotation;
		transform.LookAt(target);
		Quaternion NewRot = transform.rotation;
		transform.rotation = OriginalRot;
		transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, speed * Time.deltaTime);
	}
}