using UnityEngine;
using System.Collections;

public class ProjectileFighter : Projectile {
    
    //ProjectileFighter Values
	Loadable_Projectile.LP_Fighter loadableFighter
		{get{return (Loadable_Projectile.LP_Fighter)loadable;}}
	internal float	travelRadius		{get{return loadableFighter.travelRadius;}}
	internal float	engageDistance		{get{return loadableFighter.engageDistance;}}
	internal float	travelSpeed			{get{return loadableFighter.travelSpeed * Time.deltaTime / 100;}}
	internal string	fighterProjectileID	{get{return loadableFighter.fighterProjectileID;}}
	internal float	firerate			{get{return loadableFighter.firerate;}}


    internal Vector3 targetLocation = Vector3.zero;
    internal float travelStart = 0f;
    internal float pause = 0f;
	bool travelReturn = false;
	
	public override Projectile SetProjectile(Loadable_Projectile loading)
	{
		base.SetProjectile(loading);

		if (!SelectableLoadout.ForgeAvailable<Projectile>(loadableFighter.fighterProjectileID))
		{
			throw new UnityEngine.UnityException("projectileID " + loadableFighter.fighterProjectileID + " declared but not found on Loadable_Projectile.LP_Fighter " + loadableFighter.Loadable_ID);
		}

		Rigidbody fighterRB = gameObject.AddComponent<Rigidbody>();
		fighterRB.useGravity = false;

        return this;
	}

    public override void Initialise(PlayerControlled Target, PlayerControlled Launcher, int Damage, float[] ArmourBonus)
    {
        base.Initialise(Target, Launcher, Damage, ArmourBonus);

        targetLocation = launcher.transform.forward * 5;
		travelStart = Time.time;
    }

    public void SetTarget(PlayerControlled Target)
    {
        target = Target;
        targetPlayerID = (target) ? target.playerID : null;

        targetLocation = RandomLocation(engageDistance);
    }

    public override void EndNow()
    {
		targetLocation = Vector3.zero;
		travelStart = 0f;
		pause = 0f;
		travelReturn = false;
        base.EndNow();
    }

    public override void OnTriggerEnter(Collider hit)
    {
		Console.Function_Instance f = Console.Start ("ProjectileFighter", "OnTriggerEnter");

        if(travelReturn && hit.GetComponent<PlayerControlled>())
        {
            if(hit.GetComponent<PlayerControlled>().Equals(launcher))
            {
                EndNow();
            }
        }

		f.End ();
    }

    void Update()
    {
		Console.Function_Instance f = Console.Start ("ProjectileFighter", "Update");

		Vector3 dest = Vector3.right * ((Time.time % 2 == 0) ? 1 : -1);
		if(launcher)
		{
			//	dest is a world position, 
			//	Meanwhile targetLocation is a 'localposition' to dest

			//	If there is a target, position around the target else the launcher	ADD	If there is no target and we are returning add nothing, else add the targetLocation
			dest = ((target) ? target.transform.position : launcher.transform.position) + ((!target && travelReturn) ? Vector3.zero : targetLocation);

	        // If nearby the target dest find new location
	        if (Vector3.Distance(dest, transform.position) < travelSpeed)
	        {
				targetLocation = (target) ? RandomLocation(engageDistance) : RandomLocation(travelRadius);

				/*BUGGED //	If time to return to Launcher is less than time remaining
				if((Vector3.Distance(launcher.transform.position, transform.position) / (loadableFighter.travelSpeed / 100)) >= (travelStart + loadableFighter.life - Time.time))
				{
					//	Set to return
					travelReturn = true;

					//	And if there is not a target
					if(!target)
					{
						//	Overwrite dest parameter
						dest = launcher.transform.position;
					}
				}*/
	        }

	        // If target and fire ready
	        if(target && pause < Time.time)
	        {
	            // If in firing range
	            if(Vector3.Distance(target.transform.position, transform.position) < engageDistance)
	            {
	                // Fire
	                Projectile tempBullet = (Projectile) SelectableLoadout.Forge<Projectile>(fighterProjectileID);
	                tempBullet.transform.position = transform.position;
	                tempBullet.transform.rotation = transform.rotation;
	                tempBullet.Initialise(target, launcher, damage, armourBonus);

					//	Set the Lasers parent to this
					tempBullet.transform.parent = transform;
	                pause = Time.time + firerate;
	            }
	        }
		}

        // Apply move
		if((dest - transform.position).normalized != transform.forward)
		{
			transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, dest - transform.position, Time.deltaTime * 2, 0.0F), Vector3.up);
		}
		transform.position = transform.position + (transform.forward * travelSpeed);
		
		f.End ();
    }

    Vector3 RandomLocation(float Radius)
    {
		//	Makes a random angle between 0 and 360, and trigs it to the Radius
        float distFromTarget = Random.Range(Radius / 2, Radius);
        float randAngle = Random.Range(0, 360f) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(randAngle) * distFromTarget, 0, Mathf.Cos(randAngle) * distFromTarget);
    }
}
