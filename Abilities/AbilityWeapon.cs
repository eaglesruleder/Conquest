using UnityEngine;
using System.Collections;

public class AbilityWeapon : Ability {

    public Weapon coreWeapon = null;

    public virtual void Build(Technology PlayerTech, Unit UnitData)
    {
        coreWeapon = GetComponent<Weapon>();
        base.Build(PlayerTech, UnitData);
    }

    public virtual void SetTarget(PlayerControlled TargetObj) {}

    //Apply Targeting
    public virtual void AimTarget(Vector3 aimRot, bool fire) {}

    public virtual void EndNow() {}

    public virtual int returnWeaponOut()
    {
        return coreWeapon.weaponDamage;
    }
}
