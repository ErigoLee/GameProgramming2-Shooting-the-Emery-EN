using UnityEngine;
using System.Collections;

public class Manual : MonoBehaviour {
	public GUISkin skin;
	private GameObject camera;
	private int page = 1;
	private int min = 1;
	private int max = 5;
	private string command;

	// Use this for initialization
	void Start () {
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetButtonDown("Jump"))
			Application.LoadLevel("Start");


		if (Input.GetKeyUp ("left")) {
			page --;
			if(page < min){
				page = min;
			}
		}
		else if(Input.GetKeyUp("right")){
			page++;
			if(page > max){
				page = max;
			}
		}
		switch (page) {
			case 1:
				command = "W,A,S,D keys are Arrow keys.";
				camera.transform.position = new Vector3(0.0f,5.0f,0.0f);
			break;
			case 2:
				command = "When you click your left mouse button, you can shoot your gun.";
				camera.transform.position = new Vector3(9.5f,5.0f,0.0f);
			break;
			case 3:
				command = "Shoot your gun toward the enemy!";
				camera.transform.position = new Vector3(0.0f,5.0f,9.8f);
			break;
			case 4:
				command = "If you defeat all enemies within 10 munutes, you will secure victory!";
				camera.transform.position = new Vector3(-9.5f,5.0f,0.0f);
			break;
		}


	}
	void OnGUI(){
		GUI.skin = skin;
		int ScreenWidth = Screen.width;
		int ScreenHeight = Screen.height;
		GUI.Label (new Rect (0,ScreenHeight/10,ScreenWidth,ScreenHeight),command,"Manuual");
		GUI.Label (new Rect(0,0,ScreenWidth,ScreenHeight),"<-","Left");
		GUI.Label (new Rect(0,0,ScreenWidth,ScreenHeight),"->","Right");
	}
}
