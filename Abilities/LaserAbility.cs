using UnityEngine;
using System.Collections;

public class LaserAbility : AbilityWeapon {


    float pause = 0f;
    public float abilityFireRate = 0f;
    public float abilityRange = 0f;

    public LaserAbilityProjectile bullet;

    public override void AimTarget(Vector3 aimRot, bool fire)
    {
        base.AimTarget(aimRot, fire);
        if(fire)
        {
            pause = Time.time + abilityFireRate;
        }
    }

    void Update()
    {
        if (unitData && pause < Time.time)
        {
            foreach (Collider c in Physics.OverlapSphere(transform.position, abilityRange))
            {
                ProjectileFighter temp = c.GetComponent<ProjectileFighter>();
                if (temp)
                {
                    if (!temp.launcherPlayerID.Equals(coreWeapon.unitData.playerID))
                    {
                        Vector3 aimNorm = (temp.transform.position - transform.position).normalized;
                        base.AimTarget(aimNorm, false);

                        LaserAbilityProjectile tempBullet = Instantiate(bullet) as LaserAbilityProjectile;
                        tempBullet.transform.position = transform.position;
                        tempBullet.transform.rotation = Quaternion.LookRotation(aimNorm);

                        tempBullet.Build(temp);
                        pause = Time.time + abilityFireRate;
                        break;
                    }
                }
            }
        }
    }
}
