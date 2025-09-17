using UnityEngine;
using System.Collections;

public class MainGuI : MonoBehaviour {
	//AI number
	private GameObject[] AI;
	//health
	private int health = 100;
	private float nextTime = 0.0f;
	private float nextTimer = 0.3f;
	private bool playerDead = false;
	private bool playerSuccess = false;

	public static int sec;
	public static int min;
	static float timer;

	//GUISkin
	public GUISkin skin;

	// Use this for initialization
	void Start () {
		AI = GameObject.FindGameObjectsWithTag ("AI");
		timer = 0.0f;
		sec = 0;
		min = 0;
	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;
		sec = (int)timer;
		if (timer >= 60.0f) {
			timer -= 60.0f;
			min += 1;
			sec = 0;
		}
		if(min>=10)
			Application.LoadLevel("Failed");

		AI = GameObject.FindGameObjectsWithTag ("AI");
		if (AI.Length == 0) {
			if(!playerSuccess){
				nextTime = nextTime + Time.deltaTime;
				if(nextTime>nextTimer){
					playerSuccess = true;	
				}
			}
			else
				Application.LoadLevel("Success");
		} else if (health == 0) {
			if(!playerDead){
				nextTime = nextTime + Time.deltaTime;
				if(nextTime>nextTimer){
					playerDead = true;	
				}
			}
			else
				Application.LoadLevel("Failed");
		}
	}

	void OnGUI(){
		GUI.skin = skin;
		int ScreenWidth = Screen.width;
		int ScreenHeight = Screen.height;
		GUI.Label(new Rect(0,  0, ScreenWidth, ScreenHeight), "Emery: "+AI.Length.ToString(),"Enemy");
		GUI.Label(new Rect(0,  ScreenHeight/10, ScreenWidth, ScreenHeight), "Health: "+health.ToString(),"Health");

		string timeOut = "TIME : ";
		if (min < 10) {
			timeOut += "0" + min.ToString()+" : ";
			if(sec < 10)
				timeOut += "0" + sec.ToString();
			else 
				timeOut +=sec.ToString();


		} else {
			timeOut += min.ToString() + " : ";
			if(sec < 10)
				timeOut += "0" + sec.ToString();
			else 
				timeOut +=sec.ToString();
		}
		GUI.Label (new Rect(0,0,ScreenWidth,ScreenHeight),timeOut,"Timer");
	}

	void AIAttack(int attack){
		health -= attack;
		if (health <= 0) {
			health=0;
		}
	}
}
