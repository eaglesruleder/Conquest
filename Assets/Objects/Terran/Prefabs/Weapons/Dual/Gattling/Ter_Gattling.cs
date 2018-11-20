using UnityEngine;
using System.Collections;

public class Ter_Gattling : Ability {

    public Ter_GattlingProjectile bullet;
    public float abilityRange = 0f;
    ProjectileFighter abilityTarget = null;

    float pause = 0f;

    public override void Build(Technology PlayerTech, Unit UnitData)
    {
        base.Build(PlayerTech, UnitData);

        onActivateEvent = false;
    }

    public override void AimTarget(Vector3 aimRot, bool fire) 
    {
        if(!abilityTarget)
        {
            base.AimTarget(aimRot, fire);
            if (fire)
            {
                if (!IsInvoking("Fire"))
                {
                    InvokeRepeating("Fire", 0f, fireRate);
                }
            }
            else
                CancelInvoke("Fire");
        }
    }

    void Fire()
    {
        Ter_GattlingProjectile tempBullet = Instantiate(bullet) as Ter_GattlingProjectile;
        tempBullet.transform.position = transform.position;
        tempBullet.transform.rotation = Quaternion.LookRotation(pitchEuler);
        tempBullet.Build(targetObj, unitData, pitchEuler, weaponDamage, armorBonus);

        unitData.SupplyBurn(supplyDrain);
    }

    void Update()
    {
        if(!abilityTarget && pause < Time.time)
        {
            foreach(Collider c in Physics.OverlapSphere(transform.position, abilityRange))
            {
                ProjectileFighter temp = c.GetComponent<ProjectileFighter>();
                if(temp)
                {
                    if (!temp.launcherPlayerID.Equals(unitData.playerID))
                    {
                        abilityTarget = temp;
                        Vector3 aimNorm = (abilityTarget.transform.position - transform.position).normalized;
                        base.AimTarget(aimNorm, false);

                        Ter_GattlingProjectile tempBullet = Instantiate(bullet) as Ter_GattlingProjectile;
                        tempBullet.transform.position = transform.position;
                        tempBullet.transform.rotation = Quaternion.LookRotation(aimNorm);

                        tempBullet.Build(temp, unitData, aimNorm, weaponDamage, armorBonus);
                        pause = Time.time + fireRate;
                        break;
                    }
                }
            }
        }
    }
}
