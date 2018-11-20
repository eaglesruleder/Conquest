using UnityEngine;
using System.Collections;

public class HangerAbilityProjectile : Projectile {

    Vector3 fireDirection;
    float inertia = 15;

	public virtual void Build(Unit Launcher, Vector3 FireDirection)
    {
        fireDirection = FireDirection;
        Invoke("EndNow", life);
    }

    void Update()
    {
        Vector3 moveDist = Vector3.MoveTowards(Vector3.zero, fireDirection, inertia);
        inertia /= 1.25f;

        transform.position += moveDist;
    }
}
