using UnityEngine;
using System.Collections;

public class ProjectileProjectile : Projectile {

	//To be overwritten when required in projectile subclasses
	override
	public void BuildExtend()
		{
		}

	override
	public void StepUpdate()
		{
        if(target)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            Vector3 perpVec = Vector3.Cross(fireDirection, targetDir);
            Vector3 moveVec = Vector3.Cross(perpVec, fireDirection).normalized;

            float moveDist = Mathf.Cos(Vector3.Angle(targetDir, moveVec) * Mathf.Deg2Rad) * Vector3.Distance(Vector3.zero, targetDir);
            transform.position = transform.position + moveVec * moveDist;
        }

		GetComponent<Rigidbody>().velocity = fireDirection * 20;
		}
	
}
