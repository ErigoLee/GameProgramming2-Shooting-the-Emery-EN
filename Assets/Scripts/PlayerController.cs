using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//bullet variable
	public GameObject bullet;
	private Transform bulletSpawnPoint;

	//bulletTimer variable
	private bool bulletTimer=false;
	private float timer = 0.0f;
	private float endTime = 0.5f;



	private Vector3 velocity;
	private float curSpeed, targetSpeed, rotSpeed;
	private float maxForwardSpeed = 100.0f;
	private float maxBackwardSpeed = -90.0f;
	private float maxRightAngle = 100.0f;
	private float maxLeftAngle = -100.0f;

	//총을 쏠 때 사용하는 변수들
	protected float shootRate;
	protected float elapsedTime;
	private bool shootingAni;
	//animation 지속시간
	private float shootTime = 1.0f;
	private float shootTimer = 0.0f;

	//총효과음
	public AudioClip shootBefore;

	void Start () {
		rotSpeed = 120.0f;
		shootingAni = false;
		bulletSpawnPoint = gameObject.transform.Find ("BulletSpawnPoint").transform;

	}
	

	void Update () {
		UpdateControl ();
		if (bulletTimer) {
			if(timer<endTime){
				timer = timer+Time.deltaTime;
			}
			else{
				timer = 0.0f;
				bulletTimer = false;
				Instantiate(bullet, bulletSpawnPoint.position,bulletSpawnPoint.rotation);
			}
		
		}
	}

	void OnEndGame(){
		this.enabled = false;
	}


	void UpdateControl(){


		CharacterController controller = GetComponent <CharacterController>();
		//float v = Input.GetAxis ("vertical");
		//float h = Input.GetAxis ("Horizontal");
		//velocity = v * transform.forward;
		if (Input.GetMouseButtonUp (0)) {
			GetComponent<AudioSource>().PlayOneShot(shootBefore);
			GetComponent<Animation>().Play ("Shooting");
			shootingAni = true;
			targetSpeed = 0.0f;


			Plane playerPlane = new Plane (Vector3.up, transform.position);
			Ray RayCast = Camera.main.ScreenPointToRay (Input.mousePosition);
			float HitDist = 0.0f;
			
			if(playerPlane.Raycast (RayCast, out HitDist)){
				Vector3 targetPoint = RayCast.GetPoint (HitDist);
				Quaternion playerLookAt = Quaternion.LookRotation(targetPoint-transform.position);
				transform.rotation = playerLookAt;
				bulletTimer = true;
			}


			
		}
		else if(shootingAni){
			shootTimer = shootTimer + Time.deltaTime;
			if(shootTimer>=shootTime){
				shootingAni = false;
				shootTimer = 0.0f;
			}
		}

		if (!shootingAni) {
				if (Input.GetKey (KeyCode.W)) {
					gameObject.GetComponent<Animation>().CrossFade ("walking", 0.1f);
					targetSpeed = maxForwardSpeed;
				} else if (Input.GetKey (KeyCode.S)) {
					gameObject.GetComponent<Animation>().CrossFade ("walking", 0.1f);
					targetSpeed = maxBackwardSpeed;
				}
				else {
					gameObject.GetComponent<Animation>().CrossFade ("Idle", 0.1f);
					targetSpeed = 0.0f;
				}
		}

	    if(Input.GetKey(KeyCode.A)){
			rotSpeed = maxLeftAngle;
		}
		else if(Input.GetKey(KeyCode.D)){
			rotSpeed = maxRightAngle;
		}
		else{
			rotSpeed = 0.0f;
		}
		curSpeed = Mathf.Lerp (curSpeed,targetSpeed,7.0f*Time.deltaTime);
		velocity = transform.forward*curSpeed;
		controller.Move (velocity*Time.deltaTime);
		//transform.Translate(Vector3.forward * Time.deltaTime*curSpeed);
		transform.Rotate (0,rotSpeed*Time.deltaTime,0.0f);


	}
}
