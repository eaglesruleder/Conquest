using UnityEngine;
using System.Collections;

public class HangerAbility : AbilityWeapon {

    public HangerAbilityProjectile ProbeProjectile;

    void Update()
    {
        if(activateAbility)
        {
            activateAbility = false;
            Activate();
        }
    }

    public override void Activate()
    {
        if(unitData.currentSupply > abilityDrain)
        {
            HangerAbilityProjectile temp = Instantiate(ProbeProjectile) as HangerAbilityProjectile;
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
            temp.Build(coreWeapon.unitData, transform.forward);
        }
    }
}
