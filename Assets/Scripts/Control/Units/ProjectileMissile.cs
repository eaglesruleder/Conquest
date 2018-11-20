using UnityEngine;
using System.Collections;

public class ProjectileMissile : Projectile {

	//To be overwritten when required in projectile subclasses
	override
	public void BuildExtend()
		{
		}
	
	override
	public void StepUpdate()
		{
		Vector3 targetAim = target.transform.position - transform.position;
		fireDirection = Vector3.RotateTowards(fireDirection, targetAim, Time.deltaTime, 0.0F);
		
		GetComponent<Rigidbody>().velocity = fireDirection * 10;
		}
}
