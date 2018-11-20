using UnityEngine;
using System.Collections;

public class ProjectileProjectile : Projectile {

    public Vector3 fireDirection = Vector3.zero;
    public float velocity = 1f;

    public float colliderHeight = 0.5f;

    public override void Build(PlayerControlled Target, Unit Launcher, Vector3 FireDirection, int Damage, float[] ArmourBonus)
    {
        base.Build(Target, Launcher, FireDirection, Damage, ArmourBonus);
        fireDirection = FireDirection;

        SphereCollider tempCollider = gameObject.AddComponent<SphereCollider>();
        tempCollider.isTrigger = true;
        tempCollider.radius = colliderHeight;
    }

    public virtual void Update()
    {
        if (target)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            Vector3 perpVec = Vector3.Cross(fireDirection, targetDir);
            Vector3 moveVec = Vector3.Cross(perpVec, fireDirection).normalized;

            float angle = Vector3.Angle(targetDir, moveVec) * Mathf.Deg2Rad;
            float distToTarget = Vector3.Distance(Vector3.zero, targetDir);
            float modifier = Mathf.Pow(0.95f, Mathf.Sin(angle) * distToTarget);
            float sideDist = Mathf.Cos(angle) * distToTarget;
            transform.position += moveVec * sideDist * modifier;
        }

        transform.position += fireDirection * velocity;
    }
	
}
