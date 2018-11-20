using UnityEngine;
using System.Collections;

public class ProjectileFighter : Projectile {

	Vector3 targetLocation = Vector3.zero;
	public float travelDistance = 0;
	public float travelRadius = 0;
	public float engageDistance = 2.5f;
	
	ParticleSystem trail;
//	Rigidbody rigidbody;
	ProjectileProjectile bullet;

	//To be overwritten when required in projectile subclasses
	override
	public void BuildExtend()
		{
		targetLocation = gameObject.transform.position;
		}

	public void SetTarget(PlayerControlled Target)
		{
		target = Target;
        targetPlayerID = target.playerID;
		}

	override
	public void StepUpdate()
		{
		float distToDest = Vector3.Distance(targetLocation, gameObject.transform.position);
		float distToSrc = Vector3.Distance(gameObject.transform.position, launcher.transform.position);
		//If running out of fuel return to launcher
		if(travelDistance <= distToSrc)
			{
			targetLocation = launcher.transform.position;
			}
		//Else aquire target if there is one
		else if(target)
			{
			float distToTarget = Vector3.Distance(targetLocation, target.transform.position);
			targetLocation = target.transform.position;
			//If close enough to fire
			if(distToTarget < engageDistance)
				{
				//FIRE WEAPON THEN
				}
			}
		else if (distToDest < engageDistance)
			{
			float randX = Random.Range(travelRadius*-1, travelRadius);
			float randZ = Random.Range(travelRadius*-1, travelRadius);
			Vector3 randDest = launcher.transform.position;
			randDest.x += randX;
			randDest.z += randZ;

			targetLocation = randDest;
			}
		Vector3 relativeLocation = targetLocation - transform.position;
		Vector3 moveDir = Vector3.RotateTowards(gameObject.transform.forward, relativeLocation, Time.deltaTime, 0.0F);
		gameObject.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);

		//Value found through trial and error
		travelDistance -= 0.03f;
		
		GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 2;
		}
}
