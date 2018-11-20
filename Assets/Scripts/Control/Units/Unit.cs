using UnityEngine; 
using System.Collections.Generic;

public class Unit : PlayerControlled {

    public int currentKills = 0;
    public int currentSupply = 0;

    static Vector3 posNull = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

    public int hullLevel = 0;
    public int hullFaction = 0;

    GameObject shieldHit = null;

    Technology shipTech;

    int healthFromArmour;
    float hullTechInfluence;
    Technology.TechLocation hullLoc;
    Technology.TechNode hullTech;

    float shieldFromArmour;
    float shieldTechInfluence;
    Technology.TechLocation shieldLoc;
    Technology.TechNode shieldTech;

    float engineCapacity;
    float engineTechInfluence;
    Technology.TechLocation engineLoc;
    Technology.TechNode engineTech;

    int supplyCapacity;
    float supplyTechInfluence;
    Technology.TechLocation supplyLoc;
    Technology.TechNode supplyTech;

    /*
    float sensorDistance;
    float sensorTechInfluence;
    Technology.TechLocation sensorLoc;
    Technology.TechNode sensorTech;
    */

    float engageDistance = 0f;

	//Build Objects
    public int armour = 0;
    public int supply = 0;
    //List<Ability> abilities = null;
    List<UnitWeapon> weapons = new List<UnitWeapon>();

    public List<Vector3> destPositions = new List<Vector3>();
    Vector3 targetPosition = posNull;
    float stopDist = 2.5f;
    float colliderRadius = 0f;

	bool aimForward = false;
	public PlayerControlled targetObj = null;
	GameObject pitchObj = null;

    public void Initialise(GUIPlayer PlayerData, UnitBuilder unitBuilder)
    {
        base.Initialise(PlayerData);

        pcName = unitBuilder.UnitName;

        armour = unitBuilder.ArmourRating;
        supply = unitBuilder.SupplyExpand;

        UnitHull tempHull = unitBuilder.hull;
        hullLevel = tempHull.hullLevel;
        hullFaction = tempHull.hullFaction;
        shipTech = PlayerData.playerTechs[hullFaction];

        healthFromArmour = tempHull.healthFromArmour[armour];
        hullTechInfluence = tempHull.hullTechInfluence;
        hullLoc = tempHull.hullLoc;
        hullTech = shipTech.getNode(tempHull.hullLoc);

        shieldFromArmour = tempHull.shieldFromArmour[armour];
        shieldTechInfluence = tempHull.shieldTechInfluence;
        shieldLoc = tempHull.shieldLoc;
        shieldTech = shipTech.getNode(tempHull.shieldLoc);

        engineCapacity = tempHull.engineCapacity;
        engineTechInfluence = tempHull.engineTechInfluence;
        engineLoc = tempHull.engineLoc;
        engineTech = shipTech.getNode(tempHull.engineLoc);

        supplyCapacity = tempHull.supplyCapacity * (1 + supply / 4);
        supplyTechInfluence = tempHull.supplyTechInfluence;
        supplyLoc = tempHull.supplyLoc;
        supplyTech = shipTech.getNode(tempHull.supplyLoc);

        /*
        sensorDistance = tempHull.sensorDistance;
        sensorTechInfluence = tempHull.sensorTechInfluence;
        sensorLoc = tempHull.supplyLoc;
        sensorTech = shipTech.getNode(tempHull.sensorLoc);
        */

        shieldHit = tempHull.shieldHit;

        selectionObj = Instantiate(tempHull.selectionObj, transform.position, transform.rotation) as GameObject;
        selectionObj.transform.parent = transform;
        selectionObj.SetActive(false);

        pitchObj = new GameObject();
        pitchObj.transform.parent = transform;
        pitchObj.transform.localPosition = Vector3.zero;
        pitchObj.transform.localRotation = Quaternion.identity;

        colliderRadius = GetComponent<CapsuleCollider>().radius;
        colliderHeight = GetComponent<CapsuleCollider>().height;

        foreach (UnitBuilder.WeaponLocation wpLoc in unitBuilder.weaponLocations)
        {
            UnitWeapon temp = Instantiate(wpLoc.weapon) as UnitWeapon;
            temp.transform.parent = transform;
            temp.transform.localPosition = wpLoc.location;
            temp.Build(shipTech, this);
            if(temp.engageDistance > engageDistance)
            {
                engageDistance = temp.engageDistance;
            }
            weapons.Add(temp);
        }

        currentHull = ReturnMaxHealth();
        currentSupply = ReturnSupplyMax();
    }

    public void KilledTarget()
    {
        currentKills++;
    }

    // Apply damage to unit
    public override bool Damage(int damage, float[] armorBonus, Vector3 hitPoint)
    {
        //Setting up particle system
        Vector3 relativePos = transform.position - hitPoint;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        //Creating particle
        GameObject hitObj = Instantiate(shieldHit, hitPoint, rotation) as GameObject;
        ParticleSystem hitEffect = hitObj.GetComponent<ParticleSystem>();

        //Setting particle speed and particle-count as per projectile damage
        hitEffect.startSpeed = damage / 10;
        hitEffect.Emit(damage * damage);

        //Modify the damage as per shield for unit damaging
        float modifier = 1f - ReturnShieldMax() + armorBonus[armour];
        damage = Mathf.CeilToInt(damage * modifier);

        if (currentHull > damage)
        {
            currentHull -= damage;
        }
        else if (currentHull != 0)
        {
            currentHull = 0;
            OnBecameInvisible();
            Vector3 torque = (transform.position - hitPoint).normalized * damage;
            torque = new Vector3(torque.z, torque.y, torque.z);
            GetComponent<Rigidbody>().AddTorque(torque);
            EndSelf();
            return true;
        }
        return false;
    }

    public void SupplyBurn(int supplies)
    {
        if (currentSupply > supplies)
        {
            currentSupply -= supplies;
        }
        else
        {
            currentSupply = 0;
        }
    }

	void Update()
		{
		AimTurrets();

		MoveToPosition();
		}

    public override void SetTarget(PlayerControlled Target)
    {
        targetObj = Target;
        foreach (UnitWeapon weapon in weapons)
        {
            weapon.SetTarget(targetObj);
        }
    }

    void AimTurrets()
    {
        if (!aimForward || targetObj)
        {
            aimForward = Vector3.Angle(pitchObj.transform.forward, transform.forward) < 2.5f;

            Vector3 targetAim = Vector3.zero;
            bool fire = false;

            targetAim = (targetObj) ? targetObj.transform.position - transform.position : transform.forward;

            Vector3 pitchDir = Vector3.RotateTowards(pitchObj.transform.forward, targetAim, Time.deltaTime * 2, 0.0F);
            pitchObj.transform.rotation = Quaternion.LookRotation(pitchDir, pitchObj.transform.up);

            if (Vector3.Angle(pitchObj.transform.forward, targetAim) < 5)
            {
                if (targetObj)
                {
                    fire = true;
                }
                else
                    aimForward = true;
            }
            foreach (UnitWeapon weapon in weapons)
            {
                weapon.AimTarget(pitchDir, (currentSupply > 0 && fire));
            }
        }
    }
	
	// Add a new position, or make a sinlge new position to the list
    public override void SetMove(Vector3 Position, bool Increment)
    {
        if (Increment)
        {
            destPositions.Add(Position);
        }
        else
        {
            destPositions = new List<Vector3> { Position };
            // Due to a new list, reset targetPosition
            targetPosition = posNull;
        }
    }

	void MoveToPosition()
	{
		// If near targetPosition, null targetPosition
		if(Vector3.Distance(transform.position, targetPosition) <= stopDist)
		{
			destPositions.Remove(targetPosition);
			targetPosition = posNull;
		}
		
		// If no destination exists
		if(targetPosition.Equals(posNull))
		{
            // Begin destination check by checking if there is already an input destination
            if (destPositions.Count != 0)
            {
                // Raycast check that there is no environment in the way of the next position
                // Initialise Raycast variables
                Vector3 relPos = destPositions[0] - transform.position;
                Ray ray = new Ray(transform.position, relPos);
                float relPosDist = Vector3.Distance(Vector3.zero, relPos);
                RaycastHit checkHit;

                int layer = 1 << LayerMask.NameToLayer("Environment");

                // If there is a Planet object
                if (Physics.SphereCast(ray, colliderRadius * 1.1f, out checkHit, relPosDist, layer))
                {
                    Vector3 toHit = checkHit.transform.position - transform.position;
                    Vector3 toPoint = checkHit.point - transform.position;
                    Vector3 perpNorm = Vector3.Cross(toHit, toPoint);
                    Vector3 avoidDir = Vector3.Cross(perpNorm, toHit).normalized;
                    float avoidDist = checkHit.collider.gameObject.GetComponent<SphereCollider>().radius + colliderHeight;
                    Vector3 avoidVec = checkHit.transform.position + avoidDir * avoidDist;
                    destPositions.Insert(0, avoidVec);
                }

                targetPosition = destPositions[0];
            }
            // Else find if there is a target nearby
            else
            {
                foreach (Collider c in Physics.OverlapSphere(transform.position, engageDistance))
                {
                    PlayerControlled temp = c.GetComponent<PlayerControlled>();
                    if (temp)
                    {
                        if(temp.playerID != playerID)
                        {
                            targetObj = temp;
                            break;
                        }
                    }
                }
            }
		}

        // If there is target dest
        if (!targetPosition.Equals(posNull))
        {
            Vector3 targetDir = targetPosition - transform.position;

            float engAngle = Vector3.Angle(transform.forward, targetDir);
            if (engAngle >= 90)
                engAngle = 90;
            engAngle *= Mathf.Deg2Rad;
            float engOut = Mathf.Cos(engAngle) * ReturnEngineDist();

            Vector3 movePos = Vector3.MoveTowards(transform.position, targetPosition, engOut);
            GetComponent<Rigidbody>().MovePosition(movePos);

            Vector3 newRot = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * 2, 0.0F);
            transform.rotation = Quaternion.LookRotation(newRot, Vector3.up);
        }
        // Else align upwards
        else
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }
	}

    public void OnCollisionStay(Collision hit)
    {
        Vector3 targetDir = hit.transform.position - transform.position;
        float hitAngle = Vector3.Angle(transform.right, targetDir);

        Vector3 targetSide;
        if (hitAngle <= 180)
            targetSide = transform.right;
        else
            targetSide = transform.right * -1;

        Vector3 newPos = transform.position - transform.forward;
        newPos += targetSide;

        transform.position = Vector3.MoveTowards(transform.position, newPos, ReturnEngineDist());
    }

    public override int ReturnMaxHealth()
    {
        int techLevel = hullTech.ReturnLevels(hullLoc);
        float techModifier = 1 + (techLevel * hullTechInfluence);
        techModifier *= (1 + hullTech.ReturnBonus(hullLoc));

        return Mathf.CeilToInt(techModifier * healthFromArmour);
    }

    public float ReturnShieldMax()
    {
        int techLevel = shieldTech.ReturnLevels(shieldLoc);
        float techModifier = techLevel * shieldTechInfluence;

        float shieldReturn = shieldFromArmour + techModifier;
        shieldReturn *= (1 + shieldTech.ReturnBonus(shieldLoc));

        return shieldReturn;
    }

    public float ReturnEngineDist()
    {
        // Ratio for actual to game is 1:10,000 as Vector3.MoveTowards()
        // Ratio for actual to game is 1:50,000 as Rigidbody.velocity
        int techLevel = engineTech.ReturnLevels(engineLoc);
        float techModifier = 1 + (techLevel * engineTechInfluence);
        techModifier *= (1 + engineTech.ReturnBonus(engineLoc));

        return techModifier * engineCapacity / 10000;
    }

    public int ReturnSupplyMax()
    {
        int techLevel = supplyTech.ReturnLevels(supplyLoc);
        float techModifier = 1 + (techLevel * supplyTechInfluence);
        techModifier *= (1 + supplyTech.ReturnBonus(supplyLoc));

        return Mathf.CeilToInt(techModifier * supplyCapacity);
    }

    public int ReturnSensorMax()
    {
        return -1;
    }

    public override void EndSelf()
    {
        ParticleSystem dieEffect = gameObject.GetComponent<ParticleSystem>();
        dieEffect.Emit(150);
        base.EndSelf();
    }

	public override void EndNow()
	{
		foreach (UnitWeapon wp in weapons)
		{
			wp.EndNow();
		}
        base.EndNow();
	}
}
