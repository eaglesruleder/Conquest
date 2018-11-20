using UnityEngine;
using System.Collections;

public class UnitWeaponHanger : UnitWeapon {

	public int noSquadrons = 1;
    public ProjectileFighter projectile;
	ProjectileFighter[] squadrons;

	float pause = 0f;

	//Used for any additional requirements
    public override void Build(Technology PlayerTech, Unit UnitData)
		{
        base.Build(PlayerTech, UnitData);

		squadrons = new ProjectileFighter[noSquadrons];
		}

    public override void SetTarget(PlayerControlled TargetObj)
    {
        base.SetTarget(TargetObj);
        foreach (ProjectileFighter squad in squadrons)
        {
            if (squad)
                squad.SetTarget(targetObj);
        }
    }
	
	void Update()
		{
		for(int i = 0; i < squadrons.Length; i++)
			{
			if(!squadrons[i] && pause < Time.time)
				{
				ProjectileFighter squad = Instantiate(projectile, transform.position, transform.rotation) as ProjectileFighter;
				squad.Build(null, unitData, transform.forward, returnWeaponOut(), armorBonus);
                if(targetObj)
                {
                    squad.SetTarget(targetObj);
                }
				squadrons[i] = squad;
				pause = Time.time + fireRate;

                unitData.SupplyBurn(supplyDrain);
				}
			}
		}

    public override void EndNow()
    {
        foreach(ProjectileFighter squad in squadrons)
        {
            squad.EndNow();
        }
    }
}
