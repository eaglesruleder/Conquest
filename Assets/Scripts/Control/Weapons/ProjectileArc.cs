using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileArc : ProjectileLaser {

    //	ProjectileLaser Values
	Loadable_Projectile.LP_Arc loadableArc
		{get{return (Loadable_Projectile.LP_Arc)loadable;}}
	internal int bounce {get{return loadableArc.bounce;}}
	internal float range { get { return loadableArc.range; } }

	internal bool isSource;

	//	Local variables
    LineRenderer drawEffect;
    float currentWidth;
	Vector3 slide = Vector3.zero;
	Vector3 slideShift = Vector3.zero;
	
	public override Projectile SetProjectile(Loadable_Projectile loading)
	{
		base.SetProjectile(loading);

		isSource = true;

		return this;
	}

    public override void Initialise(PlayerControlled Target, PlayerControlled Launcher, int Damage, float[] ArmourBonus)
    {
        base.Initialise(Target, Launcher, Damage, ArmourBonus);

		if(isSource)
		{
			PlayerControlled last = Target;
			List<PlayerControlled> hit = new List<PlayerControlled>(){last, Launcher};
			for(int i = 0; i < bounce; i++)
			{
				//	Test we have not 'exited'
				if(last)
				{
					//	Get nearby last hit
					Collider[] near = Physics.OverlapSphere(last.transform.position, range * Mathf.Pow(0.9f, i));
					
					// If any
					if(near.Length > 0)
					{
						//	Get the closest
						PlayerControlled closest = null;
						float dist = Mathf.Infinity;
						foreach(Collider c in near)
						{
							PlayerControlled current = c.GetComponent<PlayerControlled>();
							if(current)
							{
								if(current.playerID == Target.playerID && !hit.Contains(current))
								{
									if(!closest)
									{
										closest = current;
										dist = Vector3.Distance(closest.transform.position, c.transform.position);
									}
									else if(Vector3.Distance(last.transform.position, c.transform.position) < dist)
									{
										closest = current;
										dist = Vector3.Distance(last.transform.position, c.transform.position);
									}
								}
							}
						}

						if(!closest)
						{
							last = null;
						}
						else
						{
							ProjectileArc temp = (ProjectileArc)SelectableLoadout.Forge<Projectile>(loadableArc.Loadable_ID);
							temp.isSource = false;
							temp.transform.position = last.transform.position;
							temp.transform.rotation = last.transform.rotation;
							
							temp.Initialise(closest, Launcher, Damage, ArmourBonus);
							temp.transform.parent = last.transform;

							last = closest;
							hit.Add(last);
						}
					}
					else
					{
						print ("hit11");
						last = null;
					}
				}
			}
		}
    }
}
