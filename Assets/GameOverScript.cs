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
		if(GUI.Button(new Rect((int)(Screen.width * (1f/3f)) - 50, (int)(Screen.height * .4f) - 50, 100, 100), "Retry"))
		{
			Application.LoadLevel("Level 1");	
		}
		if(GUI.Button(new Rect((int)(Screen.width * (2f/3f)) - 50, (int)(Screen.height * .4f) - 50, 100, 100), "Title Screen"))
		{
			Application.LoadLevel("Title Screen");
		}
	}
}
