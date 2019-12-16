using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class m25MasterProject_ReadMe : MonoBehaviour {
	//2.23.19 
		//M_BETADEMO IS THE MOST UP TO DATE BUILD
		//E_UNITYTOSQL_ALLDATA IS THE MOST UP TO DATE BACKEND


	//How to call a function that's inside a different script:

	//Create a tag and assign it to the gameobject with the script you need to access
	//outside of Start(); write `GameObject MyGameObject;`
	//inside Start() write `MyGameObject = GameObject.FindWithTag("MyTag");`
	//where you need to call the function, write `MyGameObject.GetComponent<ScriptName>().FunctionName();`

}
