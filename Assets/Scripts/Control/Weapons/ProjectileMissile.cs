using UnityEngine;
using System.Collections;

public class ProjectileMissile : Projectile {

    //ProjectileMissile Values
	Loadable_Projectile.LP_Missile loadableMissile
		{get{return (Loadable_Projectile.LP_Missile)loadable;}}
	internal float velocity 		{get{return loadableMissile.velocity;}}
	internal float turnMagnitude {get{return loadableMissile.turnMagnitude;}}

    void Update()
    {
		Console.Function_Instance f = Console.Start ("ProjectileMissile", "Update");

		//	Determine thrust direction
        Vector3 targetAim = (target) ? target.transform.position - transform.position : transform.forward;
        Vector3 thrustDirection = Vector3.RotateTowards(transform.forward, targetAim, Time.deltaTime * turnMagnitude, 0.0F);

		//	Apply and push forward
        transform.rotation = Quaternion.LookRotation(thrustDirection);
        transform.position += transform.forward * velocity;

		f.End ();
    }
}
