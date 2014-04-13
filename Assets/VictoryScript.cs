using UnityEngine;
using System.Collections;

public class VictoryScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(100, 200, 100, 100), "Back to Title"))
		{
			Application.LoadLevel("Title Screen");	
		}
	}
}
