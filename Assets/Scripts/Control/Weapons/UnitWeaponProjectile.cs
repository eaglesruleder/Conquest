using UnityEngine;
using System.Collections;

public class UnitWeaponProjectile : UnitWeapon {

    public Projectile bullet;

    public override void Build(Technology PlayerTech, Unit UnitData)
    {
        base.Build(PlayerTech, UnitData);
    }

    public override void AimTarget(Vector3 aimRot, bool fire)
    {
        base.AimTarget(aimRot, fire);

        pitchEuler = aimRot;
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

    void Fire()
    {
        Projectile tempBullet = Instantiate(bullet) as Projectile;
        tempBullet.transform.position = transform.position;
        tempBullet.transform.rotation = Quaternion.LookRotation(pitchEuler);
        tempBullet.Build(targetObj, unitData, pitchEuler, weaponDamage, armorBonus);

        unitData.SupplyBurn(supplyDrain);
    }
}
