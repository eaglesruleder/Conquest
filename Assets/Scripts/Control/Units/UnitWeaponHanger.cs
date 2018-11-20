using UnityEngine;
using System.Collections;

public class UnitWeaponHanger : UnitWeapon {

	public int noSquadrons = 1;
	ProjectileFighter[] squadrons;

	float pause = 0f;

	override
	//Used for any additional requirements
	public void BuildExtend()
		{
		squadrons = new ProjectileFighter[noSquadrons];
		}
	
	override
	public void Fire()
		{
		foreach(ProjectileFighter squad in squadrons)
			{
			if(squad)
				squad.SetTarget(targetObj);
			}
		}
	
	override
	public void SpecialFire()
		{
		for(int i = 0; i < squadrons.Length; i++)
			{
			if(!squadrons[i] && pause < 0)
				{
				ProjectileFighter squad = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation) as ProjectileFighter;
				squad.Build(null, unitData, gameObject.transform.forward, returnWeaponOut());
				squadrons[i] = squad;
				pause = 10f;
				}
			}

		pause -= 0.1f;
		}

    override
    public void Kill()
    {
        foreach(ProjectileFighter squad in squadrons)
        {
            Destroy(squad.gameObject);
        }
    }
}
