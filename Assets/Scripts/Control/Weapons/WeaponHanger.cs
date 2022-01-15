using UnityEngine;

using OdWyer.RTS;

public class WeaponHanger : Weapon
{
    //	WeaponHanger Values
	Loadable_Weapon.LW_Hanger loadableHanger
		{get{return (Loadable_Weapon.LW_Hanger)loadable;}}
	internal int	noSquadrons		{get{return loadableHanger.noSquadrons;}}
	

	//	Local variables
	internal ProjectileFighter[] squadrons;
	internal float pause = 0f;
	
	//Used for any additional requirements
	public override Weapon SetWeapon(Loadable_Weapon loading)
	{
		base.SetWeapon(loading);

		squadrons = new ProjectileFighter[noSquadrons];

		return this;
	}

    public override void SetTarget(PlayerControlled TargetObj)
    {
        base.SetTarget(TargetObj);

		//	Set the target into each fighter
        foreach (ProjectileFighter squad in squadrons)
        {
            if (squad)
                squad.SetTarget(targetObj);
        }
    }
	
	internal virtual void Update()
	{
		//	Check each fighter slot
		for(int i = 0; i < squadrons.Length; i++)
		{
			//	If the fighter isnt spawned (or is dead), and the refire is ready
			if((!squadrons[i] || !squadrons[i].gameObject.activeSelf) && pause < Time.time)
			{
				//	Instantiate bullet with double check its a fighter
                Projectile bullet = (Projectile)SelectableLoadout.Forge<Projectile>(projectileID);
				bullet.transform.position = transform.position;
				bullet.transform.rotation = transform.rotation;
                if(bullet is ProjectileFighter)
                {
					//	Prepare fighter
                    ProjectileFighter squad = (ProjectileFighter) bullet;
                    squad.Initialise((targetObj) ? targetObj : null, unitData, weaponDamage, armorBonus);

					//	Assign to slot
                    squadrons[i] = squad;

					//	Update fire time
                    pause = Time.time + fireRate;

					//	Burn supplies
                    unitData.SupplyBurn(supplyDrain);

					break;
                }
			}
		}
	}

	public override void Fire(bool firing)
	{}

    public override void EndNow()
    {
		//	Destroy all projectiles
        foreach(ProjectileFighter squad in squadrons)
        {
            squad.EndNow();
        }
		base.EndNow ();
    }
}
