using UnityEngine;

using OdWyer.RTS;

[System.Serializable]
public abstract class Projectile : MonoBehaviour {

    //Projectile Values
	internal Loadable_Projectile loadable;
	
    internal string Name	{get{return loadable.Loadable_Name;}}
    internal MeshHandler model;
    internal ParticleSystem particle;
    internal float life				{get{return loadable.life;}}

	public int damage;
    public float[] armourBonus;
    public PlayerControlled target;
    public string targetPlayerID;
    public PlayerControlled launcher;
    public string launcherPlayerID;

	public virtual Projectile SetProjectile(Loadable_Projectile loading)
	{
		loadable = loading;

		gameObject.name = Name;

		if(SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.Loadable_Mesh))
        {
			model = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.Loadable_Mesh);
            model.transform.parent = transform;
        }

		if (SelectableLoadout.ForgeAvailable<ParticleSystem>(loadable.Loadable_Particle))
        {
			particle = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(loadable.Loadable_Particle);
            particle.transform.parent = transform;
        }

		loadable.Loadable_Collider.AddComponent(gameObject);

        return this;
	}
	
	public virtual void Initialise(PlayerControlled Target, PlayerControlled Launcher, int Damage, float[] ArmourBonus)
	{
		target = Target;
		if(target)
		{
			TargetingBehaviour targetTargeting = target.GetComponent<TargetingBehaviour>();
			if (targetTargeting)
				targetPlayerID = targetTargeting.FactionID;
		}

		launcher = Launcher;
		if(launcher)
		{
			TargetingBehaviour launcherTargeting = launcher.GetComponent<TargetingBehaviour>();
			if (launcherTargeting)
				launcherPlayerID = launcherTargeting.FactionID;
		}

		damage = Damage;
        armourBonus = ArmourBonus;

        Invoke("EndNow", life);
	}

    public virtual void EndNow()
    {
		//	Essentailly empty self
        target = null;
        targetPlayerID = null;

        launcher = null;
        launcherPlayerID = null;

		//	Just in  case, if there is a parent empty it
		transform.parent = null;

		//	Disable object instead of destroy it to put it in the pool
        gameObject.SetActive(false);
    }

    public virtual void OnTriggerEnter(Collider hit)
    {
        PlayerControlled hitObj = hit.GetComponent<PlayerControlled>();
        if (hitObj)
        {
			TargetingBehaviour targeting = hitObj.GetComponent<TargetingBehaviour>();
			//	(If there is a target, is the object the target), else is the targeted team when that team is not the current team
			if (((target) ? hitObj.Equals(target) : false) ? true : (targetPlayerID.Equals(targeting.FactionID) && !launcherPlayerID.Equals(targeting.FactionID)))
            {
				HullBehaviour targetHull = target.GetComponent<HullBehaviour>();
				if (targetHull && targetHull.Damage(damage, armourBonus, transform.position))
                {
					TargetingBehaviour targetTargeting = target.GetComponent<TargetingBehaviour>();
					if (targetTargeting)
						targetTargeting.KilledTarget();
                }
				
                EndNow();
            }
        }
    }
}
