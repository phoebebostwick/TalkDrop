using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master_ReadMe : MonoBehaviour {

	//hi babies this is the master scene :)
	 
	//////////////////////////////////
	//Last edited: 4.25.19 - 7:12pm, by wig
	//////////////////////////////////


	//when you build:
	//SET ACTIVE/TURN ON VISIBILITY FOR:
		//Canvas - only the individual canvas element, not everything inside
		//Main Panel - only the individual main panel element, not everything inside
		//Onboarding Panel
		//All gameobjects EXCEPT gif inside Onboarding Panel
		//EventSystem
		//PlacenoteCameraManager
		//PlacenoteMap
	//EVERYTHING ELSE SHOULD BE HIDDEN/SET INACTIVE

	//*****NOTES:*****
	//to build offsite, check "Is Offsite App" in the inspector on the Canvas. 
	//if this is unchecked, localization will be used
	//If you need to update the map:
		//1. build the "random shapes" scene
		//2. create and save the map
		//3. enter the map id in the "map id" area in the inspector on the Canvas in this scene

	//SCRIPTS BEING USED:
	//GameController_Master
	//RecordingController_Master
	//CustomizationController_Master
	//OnboardingController_Master
	//AudioOnTapController_Master
	//LoadMap_Master
	//ColliderController

	//PREFABS BEING USED:
	//Bubble_Master
}
