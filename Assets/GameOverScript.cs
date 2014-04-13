using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(100, 200, 100, 100), "Retry"))
		{
			Application.LoadLevel("Level 1");	
		}
		if(GUI.Button(new Rect(600, 200, 100, 100), "Title Screen"))
		{
			Application.LoadLevel("Title Screen");
		}
	}
}
