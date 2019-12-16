using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class OnboardingController_Master2D_ListenOnly : MonoBehaviour {

	public GameObject SkipButton;
	public GameObject StartButton;


	public void ModeSelect() {
		StartCoroutine ("Wait");
		Initiate.Fade("Master25_ListenOnly",Color.black,1.0f);

	}

	IEnumerator Wait(){
		yield return new WaitForSeconds (1.3f);
		SceneManager.LoadScene ("Master25_ListenOnly");

	}


	//	public void LoadScene() {
	//		SceneManager.LoadScene("Master25");
	//	}

}
