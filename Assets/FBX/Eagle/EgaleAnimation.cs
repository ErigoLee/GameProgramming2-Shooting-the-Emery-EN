using UnityEngine;
using System.Collections;

public class EgaleAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Animation>().Play("Armature|Fly");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
