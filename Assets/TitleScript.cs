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
		if(GUI.Button(new Rect((int)(Screen.width / 2), (int)(Screen.height * .65), 100, 100), "Start"))
		{
			Application.LoadLevel("Level 1");	
		}
	}
}
