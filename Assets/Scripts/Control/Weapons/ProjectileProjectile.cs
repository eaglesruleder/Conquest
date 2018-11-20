using UnityEngine;
using System.Collections;

public class ProjectileProjectile : Projectile {

    //ProjectileProjectile Values
	Loadable_Projectile.LP_Projectile loadableProjectile
		{get{return (Loadable_Projectile.LP_Projectile)loadable;}}
	internal float velocity {get{return loadableProjectile.velocity;}}


	// Dynamic Values
    Vector3 fireDirection = Vector3.zero;

    public override void Initialise(PlayerControlled Target, PlayerControlled Launcher, int Damage, float[] ArmourBonus)
    {
        base.Initialise(Target, Launcher, Damage, ArmourBonus);

        fireDirection = transform.forward;
    }

    public virtual void Update()
    {
		Console.Function_Instance f = Console.Start ("ProjectileProjectile", "Update");

		//	While the target still exists
        if (target)
        {
			//	Determine the slide direction to keep target directly ahead
            Vector3 targetDir = target.transform.position - transform.position;
            Vector3 perpVec = Vector3.Cross(fireDirection, targetDir);
            Vector3 moveVec = Vector3.Cross(perpVec, fireDirection).normalized;

			//	Determine magnitude to move based on slide direction dist
            float angle = Vector3.Angle(targetDir, moveVec) * Mathf.Deg2Rad;
            float distToTarget = Vector3.Distance(Vector3.zero, targetDir);
            float modifier = Mathf.Pow(0.95f, Mathf.Sin(angle) * distToTarget);
            float sideDist = Mathf.Cos(angle) * distToTarget;

			//	Apply
            transform.position += moveVec * sideDist * modifier;
        }

		//	Push forward
        transform.position += fireDirection * velocity;

		f.End ();
    }
}
