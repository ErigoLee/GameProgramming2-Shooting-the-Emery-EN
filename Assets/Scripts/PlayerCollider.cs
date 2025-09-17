using UnityEngine;
using System.Collections;

public class PlayerCollider : MonoBehaviour {
	
	private GameObject sendObject;
	public AudioClip shoot;
	// Use this for initialization
	void Start () {
		sendObject = GameObject.FindGameObjectWithTag ("SendMessage");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision collision)
	{
		
		if(collision.gameObject.tag == "Bullet2"){
			GetComponent<AudioSource>().PlayOneShot(shoot);
			sendObject.SendMessage("AIAttack",10);
		}
	}
}

