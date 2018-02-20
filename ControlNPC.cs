using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlNPC : MonoBehaviour {
	
	bool isWandering = true;
	bool isFollowingWayPoints;
	float timer;
	GameObject wanderingTarget;
	public GameObject target;
	public GameObject WP1, WP2, WP3, WP4;
	int WPcount;
	GameObject[] WayPoints;
	void Start () {
		//check if the NPC is in wandering state
		if(isWandering){
			wanderingTarget = new GameObject();		//Create new gameobject to be used as a new target
			wanderingTarget.transform.position = new Vector3(20,0,20); 		//Set new position of target
			target = wanderingTarget;
			GetComponent<NavMeshAgent>().SetDestination(target.transform.position);

		}
		WP1 = GameObject.Find("WP1");
		WP2 = GameObject.Find("WP2");
		WP3 = GameObject.Find("WP3");
		WP4 = GameObject.Find("WP4");
		WayPoints = new GameObject[]{WP1, WP2, WP3, WP4};
		WPcount = 0;
		
	}
	
	void Update () {
		CheckAhead();
		timer += Time.deltaTime;	//timer will increase by 1 every second
		if(timer > 4){
			timer = 0;	//after 4 seconds timer is reset
			Wander();
		}
		if(isFollowingWayPoints){
		target = WayPoints[WPcount];
		if(Vector3.Distance(transform.position, target.transform.position) < 1.0){
			//MoveToNextWP();
			MoveToRandomWP();
		}
	}
		GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
}

	void Wander(){
		Ray ray = new Ray();	//create new Ray
		RaycastHit hit;		//used to obtain information about the object in front of the NPC
		ray.origin = transform.position + Vector3.up *0.7f;		//ray will start at the same position as the player, at a height of 0.7
		float distanceToObstacle = 0; 	// used to determine how far away NPC is from the object
		float castingDistance = 20;		//maximum distance for ray cast

		do{
			//used to define a random vector
			float randomDirectionX = Random.Range(-1.0f, 1.0f);
			float randomDirectionZ = Random.Range(-1.0f, 1.0f);
			ray.direction = transform.forward*randomDirectionZ + transform.right*randomDirectionX; //variables used to define the direction of the ray
			Debug.DrawRay(ray.origin, ray.direction, Color.red);	//draw ray only visible in scene mode

			if(Physics.Raycast(ray.origin, ray.direction, out hit, castingDistance)){	//detect objects ahead of NPC
				distanceToObstacle = hit.distance;	//in case of hit, save distance to distanceToObstacle
			}else{ distanceToObstacle = castingDistance;} //in case of no hit, if no object detected the NPC will walk forward for 20 meters

			wanderingTarget.transform.position = ray.origin + ray.direction*(distanceToObstacle - 1);	//wanderingTarget is set to 1 meter before destination
			target = wanderingTarget;	//set target
		}while(distanceToObstacle < 1.0f);		//stay in the loop as long as distance is less than 1

	}

	void CheckAhead(){
		Ray ray = new Ray();
		RaycastHit hit;
		float castingDistance = 2;
		ray.origin = transform.position + Vector3.up*0.7f;
		ray.direction = transform.forward * castingDistance;
		Debug.DrawRay(ray.origin, ray.direction, Color.red);
		if(Physics.Raycast(ray.origin, ray.direction, out hit, castingDistance)){
			print("object in sight at " + hit.distance);
			Wander();
		}
	}

	void MoveToNextWP(){
		WPcount++;
		if (WPcount > WayPoints.Length-1)
		{
			WPcount = 0;
		}
	}

	void MoveToRandomWP(){
		int previous = WPcount;
		int random = 0;
		do{
			random = Random.Range(0, WayPoints.Length);
		}while(random == previous);
		WPcount = random;
	}
}
