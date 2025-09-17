using UnityEngine;
using System.Collections;


public class Failed : MonoBehaviour {
	

	public GUISkin skin;
	private static int min;
	private static int sec;

	// Use this for initialization
	void Start () {
		min = MainGuI.min;
		sec = MainGuI.sec;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Jump"))
			Application.LoadLevel("Start");
	
	}

	void OnGUI()
	{
		GUI.skin = skin;
		int ScreenWidth = Screen.width;
		int ScreenHeight = Screen.height;
		
		if (min >= 10) {
			if(sec>=10){
				GUI.Label (new Rect (0,ScreenHeight/10,ScreenWidth,ScreenHeight),"Time: "+min.ToString()+" : "+sec.ToString(),"Manuual");
			}
			else{
				GUI.Label (new Rect (0,ScreenHeight/10,ScreenWidth,ScreenHeight),"Time: "+min.ToString()+" : 0"+sec.ToString(),"Manuual");
			}
		}
		else{
			if(sec>=10){
				GUI.Label (new Rect (0,ScreenHeight/10,ScreenWidth,ScreenHeight),"Time: 0"+min.ToString()+" : "+sec.ToString(),"Manuual");
			}
			else{
				GUI.Label (new Rect (0,ScreenHeight/10,ScreenWidth,ScreenHeight),"Time: 0"+min.ToString()+" : 0"+sec.ToString(),"Manuual");
			}
		}
		
		GUI.Label (new Rect (0,ScreenHeight/5,ScreenWidth,ScreenHeight),"Game Over","Manuual");
	}
}
