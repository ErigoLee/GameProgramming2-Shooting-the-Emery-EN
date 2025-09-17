using UnityEngine;
using System.Collections;

public class AIController4 : MonoBehaviour {

	public enum FSMState
	{
		None,
		Patrol,
		Chase,
		Flee, // runaway
		Attack,
		Dead
	}
	
	public FSMState curState;
	
	//chase or flee probability variable
	public int chaseProbability = 50;
	public int fleeProbability = 50;
	
	//speed AI
	private float curSpeed;
	private float maxSpeed = 30.0f;
	private Vector3 velocity;
	//flee speed
	private float fleeSpeed = 50.0f;

	//roataion speed AI
	private float curRotSpeed;
	
	//Bullet & BulletSpawnPoint & animation duration Time
	public GameObject bullet;
	private Transform bulletSpawnPoint;
	private float shootTime = 2.0f;
	private float shootTimer = 0.0f;
	
	//AI head & Death check
	private bool bDead;
	private int health;
	
	//PointList
	private GameObject[] pointList;
	
	//patrol destination
	private Vector3 patrolDestination;
	
	
	//player transform variable
	private Transform playerTransform;
	
	//animation controller
	CharacterController controller;
	private bool bulletWork = false;
	bool bulletWorking = false;
	
	//obstacle
	private float minumDistToAvoid = 15.0f;
	//obstacle timer
	private float obstacleTime = 0.0f;
	private float obstacleTimer = 10.0f;
	private bool obSwtich = false;
	
	//Explosion
	public GameObject Explosion;

	//obstacles collider
	private bool obstacleCollider = false;
	private float obstacleColliderTime = 0.0f;
	private float obstacleColliderTimer = 10.0f;

	//Audio
	public AudioClip shootBefore;
	public AudioClip shoot;

	void Initialize(){
		curSpeed = 0.0f;
		curState = FSMState.Patrol;
		curRotSpeed = 10.0f;
		bDead = false;
		health = 100;
		controller = GetComponent <CharacterController>();
		pointList = GameObject.FindGameObjectsWithTag ("WanderPoint4");
		FindNextPoint ();
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		playerTransform = playerObj.transform;
		
		if (!playerTransform) {
			print ("Is not Player!");
		}
		
		bulletSpawnPoint = gameObject.transform.Find ("BulletSpawnPoint").transform;
	}
	
	void FindNextPoint(){
		int rndIndex = Random.Range (0, pointList.Length);
		patrolDestination = pointList [rndIndex].transform.position;
		float xPos = Mathf.Abs (patrolDestination.x-transform.position.x);
		float zPos = Mathf.Abs (patrolDestination.z-transform.position.z);
		
		if (xPos < 30 && zPos < 30) {
			Vector3 add = new Vector3(Random.Range(-10.0f,10.0f),0.0f,Random.Range(-10.0f,10.0f));
			patrolDestination = patrolDestination + add;
		}
		
	}
	
	void FSMUpdate(){
		
		switch (curState) {
		case FSMState.Patrol: UpdatePatrolState(); break;
		case FSMState.Flee: UpdateFleeState(); break;
		case FSMState.Chase: UpdateChaseState(); break;
		case FSMState.Attack: UpdateAttackState(); break;
		case FSMState.Dead: UpdateDeadState(); break;
		}
		
		
		
	}
	
	float Distance(Vector3 a, Vector3 b){
		float x_distance = Mathf.Abs (a.x - b.x);
		float z_distance = Mathf.Abs (a.z - b.z);
		float distance = (float)(Mathf.Sqrt ((x_distance*x_distance)+(z_distance*z_distance)));
		
		return distance;
	}
	
	void UpdatePatrolState(){
		
		gameObject.GetComponent<Animation>().CrossFade ("walking", 0.1f);
		if (Distance (patrolDestination, transform.position) < 50.0f) {
			FindNextPoint ();
		} else if (Distance (playerTransform.position,transform.position)<200.0f) {
			print ("Switch to Chase or flee Position");
			
			int probability = Random.Range(0,chaseProbability+fleeProbability);
			if(probability<chaseProbability){
				curState = FSMState.Chase;
			}
			else{
				curState = FSMState.Flee;
			}
		}
		
		Quaternion targetRotation = Quaternion.LookRotation (patrolDestination-transform.position);
		
		
		RaycastHit hit;
		
		int layerMask = 1 << 9;
		if (Physics.Raycast (transform.position, transform.forward, out hit, minumDistToAvoid, layerMask)) {
			//Vector3 hitNormal = hit.normal;
			//hitNormal.y = 0;
			//targetRotation = Quaternion.LookRotation(transform.forward+hitNormal*50.0f);

			int a = Random.Range (0,2);
			if(a==0)
				transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y-60.0f,transform.rotation.z));
			else
				transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y+60.0f,transform.rotation.z));

			transform.Rotate (new Vector3 (transform.rotation.x, transform.rotation.y - 60.0f, transform.rotation.z));
			//transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
			obSwtich = true;
		} else if (obSwtich) {
			obstacleTime = obstacleTime + Time.deltaTime;
			//transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
			if(obstacleTime>obstacleTimer){
				obstacleTime = 0.0f;
				obSwtich = false;
			}
		}
		else if(obstacleCollider){
			obstacleColliderTime = obstacleColliderTime + Time.deltaTime;
			if(obstacleColliderTime>obstacleColliderTimer){
				obstacleTime = 0.0f;
				obstacleCollider = false;
			}
		}
		else
			transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
		
		curSpeed = Mathf.Lerp (curSpeed,maxSpeed,7.0f*Time.deltaTime);
		velocity = transform.forward*curSpeed;
		//transform.Translate (velocity);
		controller.Move (velocity*Time.deltaTime);
		
		
	}
	
	void UpdateChaseState(){
		gameObject.GetComponent<Animation>().CrossFade ("walking", 0.1f);
		if (Distance (playerTransform.position, transform.position) > 220.0f) {
			curState = FSMState.Patrol;
		} else if (Distance (playerTransform.position,transform.position)<60.0f) {
			curState = FSMState.Attack;
		}
		
		Quaternion targetRotation = Quaternion.LookRotation (playerTransform.position-transform.position);
		RaycastHit hit;
		
		int layerMask = 1 << 9;
		if(Physics.Raycast(transform.position,transform.forward,out hit,minumDistToAvoid,layerMask)){
			//Vector3 hitNormal = hit.normal;
			//hitNormal.y = 0;
			//targetRotation = Quaternion.LookRotation(transform.forward+hitNormal*50.0f);

			int a = Random.Range (0,2);
			if(a==0)
				transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y-60.0f,transform.rotation.z));
			else
				transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y+60.0f,transform.rotation.z));

			transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y-60.0f,transform.rotation.z));
			//transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
			obSwtich = true;
		}
		else if (obSwtich) {
			obstacleTime = obstacleTime + Time.deltaTime;
			//transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
			if(obstacleTime>obstacleTimer){
				obstacleTime = 0.0f;
				obSwtich = false;
			}
		}
		else if(obstacleCollider){
			obstacleColliderTime = obstacleColliderTime + Time.deltaTime;
			if(obstacleColliderTime>obstacleColliderTimer){
				obstacleTime = 0.0f;
				obstacleCollider = false;
			}
		}
		else
			transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
		
		curSpeed = Mathf.Lerp (curSpeed,maxSpeed,7.0f*Time.deltaTime);
		velocity = transform.forward*curSpeed;
		controller.Move (velocity*Time.deltaTime);
		//transform.Translate (velocity);
		
	}
	
	void UpdateFleeState(){
		gameObject.GetComponent<Animation>().CrossFade ("walking", 0.1f);
		if (Distance (playerTransform.position, transform.position) > 200.0f)
			curState = FSMState.Patrol;
		
		Quaternion targetRotation = Quaternion.LookRotation (transform.position-playerTransform.position);
		RaycastHit hit;
		
		int layerMask = 1 << 9;
		if(Physics.Raycast(transform.position,transform.forward,out hit,minumDistToAvoid,layerMask)){
			//Vector3 hitNormal = hit.normal;
			//hitNormal.y = 0;
			//targetRotation = Quaternion.LookRotation(transform.forward+hitNormal*50.0f);

			int a = Random.Range (0,2);
			if(a==0)
				transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y-60.0f,transform.rotation.z));
			else
				transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y+60.0f,transform.rotation.z));
			

			transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y-60.0f,transform.rotation.z));
			//transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
			obSwtich = true;
		}
		else if (obSwtich) {
			obstacleTime = obstacleTime + Time.deltaTime;
			//transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
			if(obstacleTime>obstacleTimer){
				obstacleTime = 0.0f;
				obSwtich = false;
			}
		}
		else if(obstacleCollider){
			obstacleColliderTime = obstacleColliderTime + Time.deltaTime;
			if(obstacleColliderTime>obstacleColliderTimer){
				obstacleTime = 0.0f;
				obstacleCollider = false;
			}
		}
		else
			transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
		
		curSpeed = Mathf.Lerp (curSpeed,fleeSpeed,7.0f*Time.deltaTime);
		velocity = transform.forward*curSpeed;
		controller.Move (velocity*Time.deltaTime);
		//transform.Translate (velocity);
	}
	
	void UpdateAttackState(){
		
		bulletWork = true;
		
		if (Distance (playerTransform.position, transform.position) > 200.0f) {
			curState = FSMState.Patrol;
			bulletWork = false;
		}
		else if (Distance (playerTransform.position, transform.position) > 80.0f) {
			int probability = Random.Range(0,chaseProbability+fleeProbability);
			if(probability<chaseProbability){
				curState = FSMState.Chase;
				bulletWork = false;
			}
			else{
				curState = FSMState.Flee;
				bulletWork = false;
			}
		}
		
		
		
		Quaternion targetRotation = Quaternion.LookRotation (playerTransform.position-transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation,targetRotation,Time.deltaTime*curRotSpeed);
	}
	
	void UpdateDeadState(){
		
		if (!bDead) {
			bDead = true;
			Instantiate(Explosion, gameObject.transform.position, Quaternion.identity);
			Destroy(gameObject,1.0f);
		}
		
		
	}
	void OnCollisionEnter(Collision collision)
	{
		print ("collider working");
		if (collision.gameObject.tag == "Obstacle") {
			print ("AI4 collider!!");
			int a = Random.Range (0,2);
			if(a==0)
				transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y-110.0f,transform.rotation.z));
			else
				transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y+110.0f,transform.rotation.z));
			velocity = transform.forward*curSpeed;
			transform.Translate(velocity);
			obstacleCollider=true;
		}
		else if(collision.gameObject.tag == "Bullet"){
			GetComponent<AudioSource>().PlayOneShot(shoot);
			health -= 40;
			if(health<=0){
				
				curState = FSMState.Dead;
				print ("Dead");
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
		FSMUpdate ();
	}
	void FixedUpdate(){
		if (bulletWork) {
			
			if(shootTimer<shootTime){
				if(shootTimer == 0.0f){
					GetComponent<AudioSource>().PlayOneShot(shootBefore);
					gameObject.GetComponent<Animation>().Play ("Shooting");
					bulletWorking = true;
				}
				if((bulletWorking==true)&&(shootTimer>=0.5f)){
					Instantiate(bullet, bulletSpawnPoint.position,bulletSpawnPoint.rotation);
					bulletWorking = false;
				}
				shootTimer = shootTimer+Time.deltaTime;
			}
			else{
				shootTimer = 0.0f;
				
			}
			
		}
	}
}
