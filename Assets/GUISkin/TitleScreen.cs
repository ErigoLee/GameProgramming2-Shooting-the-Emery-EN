using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour {


	public GUISkin skin;

	private int page;
	private int min = 1;
	private int max = 3;

	// Use this for initialization
	void Start () {
		page = 1;
	}
	
	// Update is called once per frame
	void Update () {
		PageUpDown ();
		if (Input.GetButtonDown ("Jump")) {
			switch(page){
			case 1:
				Application.LoadLevel("Manual");
				break;
			case 2:
				Application.LoadLevel("main");
				break;
			case 3:
				Application.Quit();
				break;
			}
		}
	}
	void OnGUI(){
		GUI.skin = skin;
		//font size regulate
		GUIStyle customStyle = new GUIStyle(GUI.skin.label);
		customStyle.fontSize = 100;

		int ScreenWidth = Screen.width;
		int ScreenHeight = Screen.height;
		GUI.Label(new Rect(0,  ScreenHeight/5, ScreenWidth, ScreenHeight/4), "Shooting the Emery","Title");


		switch (page) {
			case 1:
				GUI.Label(new Rect(0,  3 * ScreenHeight/10, ScreenWidth, ScreenHeight/4), "Manual","Selected");
				GUI.Label(new Rect(0,  4 * ScreenHeight/10, ScreenWidth, ScreenHeight/4), "Game Start","Unselected");
				GUI.Label(new Rect(0,  5 * ScreenHeight/10, ScreenWidth, ScreenHeight/4), "Quit","Unselected");
				break;
			case 2:
				GUI.Label(new Rect(0,  3 * ScreenHeight/10, ScreenWidth, ScreenHeight/4), "Manual","Unselected");
				GUI.Label(new Rect(0,  4 * ScreenHeight/10, ScreenWidth, ScreenHeight/4), "Game Start","Selected");
				GUI.Label(new Rect(0,  5 * ScreenHeight/10, ScreenWidth, ScreenHeight/4), "Quit","Unselected");
				break;
			case 3:
				GUI.Label(new Rect(0,  3 * ScreenHeight/10, ScreenWidth, ScreenHeight/4), "Manual","Unselected");
				GUI.Label(new Rect(0,  4 * ScreenHeight/10, ScreenWidth, ScreenHeight/4), "Game Start","Unselected");
				GUI.Label(new Rect(0,  5 * ScreenHeight/10, ScreenWidth, ScreenHeight/4), "Quit","Selected");
				break;
			default:
				return;
		}
		
	}
	void PageUpDown(){	
		if (Input.GetKeyUp ("up")) {
			page--;
			if(page<min){
				page = max;
			}
		} else if (Input.GetKeyUp ("down")) {
			page++;
			if(page>max){
				page = min;
			}
		}
	}

}
