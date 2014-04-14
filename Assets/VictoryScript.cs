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
		if(GUI.Button(new Rect((int)(Screen.width * (.6)), Screen.height / 2, 100, 100), "Back to Title"))
		{
			Application.LoadLevel("Title Screen");	
		}
	}
}
