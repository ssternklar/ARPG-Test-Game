using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(200, 200, 100, 100), "Start"))
		{
			Application.LoadLevel("Level 1");	
		}
	}
}
