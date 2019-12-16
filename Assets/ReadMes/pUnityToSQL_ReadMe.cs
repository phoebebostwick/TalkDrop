using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pUnityToSQL_ReadMe : MonoBehaviour {

	//Left - text box that data from the server should display in
	//Middle Button - post to database an entry with name and random 'score'
	//Right Button - pull data from the server and display in

	//This is based on a highscores tutorial 

	//hash and md5 are apparently not very secure but I'm unsure we need to worry about that

	//this works on iphones 

	//Many of the functions in P_shapeFromSQLController don't need to be nested the way they are,
	// but when reusing parameters its just easier, and that way I don't have a bunch of buttons

	//Currently the code doesn't post anything, as I was working on data conversions and checking
	//I'm not sure it will post anything if you uncomment that code

	//Conversions are generally the same, except for the original float array / new float array
	//That returns both 'they are the same' and 'they are not the same' and I don't know why
	//the for loop that checks the arrays for identical values freezes your computer for a couple of minutes
	//so I've been opting to check if they lengths are the same as a faster way
	//although honestly we were never going to be able to do that in the app anyway


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

}
