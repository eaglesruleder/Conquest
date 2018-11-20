using UnityEngine;
using System.Collections;

public class ProjectileMissile : Projectile {

    public float velocity = 1f;
    public float turnMagnitude = 1f;

    public float colliderHeight = 0.5f;

    public override void Build(PlayerControlled Target, Unit Launcher, Vector3 FireDirection, int Damage, float[] ArmourBonus)
    {
        base.Build(Target, Launcher, FireDirection, Damage, ArmourBonus);

        SphereCollider tempCollider = gameObject.AddComponent<SphereCollider>();
        tempCollider.isTrigger = true;
        tempCollider.radius = colliderHeight;
    }

	void Update()
		{
		Vector3 targetAim = target.transform.position - transform.position;
		Vector3 thrustDirection = Vector3.RotateTowards(transform.forward, targetAim, Time.deltaTime * turnMagnitude, 0.0F);
        transform.rotation = Quaternion.LookRotation(thrustDirection);

        transform.position += transform.forward * velocity;
		}
}
