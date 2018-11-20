using UnityEngine;
using System.Collections.Generic;  

[System.Serializable]
public class UnitHull : MonoBehaviour
{
    public static Vector3 posNull = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

    //All of the presets to be edited
    public GameObject selectionObj;
	public GameObject die;
	public GameObject shieldHit;

	int solarID = 0;
	public PlayerControlled unitData;

	public int minTechHeight;
	Technology shipTech;

	public float hullCapacity;
	public float hullTechInfluence;
	public Technology.TechLocation hullTech;

	public float shieldMagnitude;
	public float shieldTechInfluence;
	public Technology.TechLocation shieldTech;

	public float engineCapacity;
	public float engineTechInfluence;
	public Technology.TechLocation engineTech;

	public float supplyCapacity;
	public float supplyTechInfluence;
	public Technology.TechLocation supplyTech;

	public float sensorDistance;
	public float sensorTechInfluence;
	public Technology.TechLocation sensorTech;
	public float engageDistance;

	//Dynamic Fields
	public float currentHull;
	public float currentSupply;
	
	public void Build(Technology ShipTech, PlayerControlled UnitData)
		{
        unitData = UnitData;
		shipTech = ShipTech;
		
		selectionObj = Instantiate(selectionObj, transform.position, transform.rotation) as GameObject;
		selectionObj.transform.parent = gameObject.transform;
        unitData.selectionObj = selectionObj;
		selectionObj.SetActive(false);
		
		currentHull = ReturnHullMax();
		}

	void OnBecameInvisible()
	{
        unitData.BecameInvisible();
	}
	void OnBecameVisible()
	{
        unitData.BecameVisible();
	}

    public void OnTriggerEnter(Collider hit)
    {
        if (hit.GetComponent<Projectile>() && !hit.GetComponent<ProjectileFighter>())
        {
            Projectile bullet = hit.GetComponent<Projectile>();
            PlayerControlled target = bullet.target;

            bool pass = false;
            if(target != null)
            {
                pass = (target.Equals(unitData)) ? true : (bullet.targetPlayerID.Equals(unitData.playerID) && !bullet.launcherPlayerID.Equals(unitData.playerID));
            }
            if (pass)
            {
                float damage = 0;
                damage = bullet.damage;
                Destroy(hit.gameObject);

                //Setting up particle system
                Vector3 relativePos = transform.position - hit.transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePos);

                //Creating particle
                GameObject hitObj = Instantiate(shieldHit, hit.transform.position, rotation) as GameObject;
                ParticleSystem hitEffect = hitObj.GetComponent<ParticleSystem>();

                //Setting particle speed and particle-count as per projectile damage
                hitEffect.startSpeed = damage / 10;
                int emit = 0;
                if (currentHull > 0)
                    emit = Mathf.CeilToInt(damage);
                hitEffect.Emit(emit);

                //Damage Unit
                if (DamageUnit(damage))
                {
                    bullet.launcher.KilledTarget();
                }
            }
        }
        if (hit.GetComponent<UnitHull>())
        {

        }
    }

	public void OnCollisionStay(Collision hit)
		{
		Vector3 targetDir = hit.transform.position - transform.position;
		float hitAngle = Vector3.Angle(transform.right, targetDir);

		Vector3 targetSide;
		if(hitAngle <= 180)
			targetSide = transform.right;
		else
			targetSide = transform.right * -1;

		Vector3 newPos = transform.position - transform.forward;
		newPos += targetSide;
		//transform.position = newPos;

		transform.position = Vector3.MoveTowards(transform.position, newPos, ReturnEngineDist());
		}

    // Apply damage to unit
    public bool DamageUnit(float Damage)
    {
        //Modify the damage as per shield for unit damaging
        float shieldModifier = 1f - ReturnShieldMax();
        Damage *= shieldModifier;

        if (currentHull > Damage)
        {
            currentHull -= Damage;
        }
        else if (currentHull != 0)
        {
            currentHull = 0;
            Instantiate(die, gameObject.transform.position, gameObject.transform.rotation);
            unitData.KillSelf();
            return true;
        }
        return false;
    }

    public void moveUnit(Vector3 TargetPosition)
    {
        if(!TargetPosition.Equals(posNull))
        {
            Vector3 targetDir = TargetPosition - transform.position;

            float engAngle = Vector3.Angle(transform.forward, targetDir);
            if (engAngle >= 90)
                engAngle = 90;
            engAngle *= Mathf.Deg2Rad;
            float engOut = Mathf.Cos(engAngle) * ReturnEngineDist();

            Vector3 movePos = Vector3.MoveTowards(transform.position, TargetPosition, engOut);
            GetComponent<Rigidbody>().MovePosition(movePos);

            TargetPosition.z = transform.position.z;
            Vector3 newRot = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * 2, 0.0F);
            transform.rotation = Quaternion.LookRotation(newRot, Vector3.up);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }
    }

    public int ReturnHullMax()
    {
        Technology.TechNode tempHull = shipTech.getNode(hullTech);

        int techLevel = tempHull.ReturnLevels(hullTech);
        float techModifier = 1 + (techLevel * hullTechInfluence);
        techModifier *= (1 + tempHull.ReturnBonus(hullTech));

        int tempReturn = Mathf.CeilToInt(techModifier * hullCapacity);
        return tempReturn;
    }

	public float ReturnShieldMax()
	{
		Technology.TechNode tempShield = shipTech.getNode(shieldTech);
		
		int techLevel = tempShield.ReturnLevels(shieldTech);
		float techModifier = techLevel * shieldTechInfluence;

		float shieldReturn = shieldMagnitude + techModifier;
		shieldReturn *= (1 + tempShield.ReturnBonus(shieldTech));
		
		return shieldReturn;
	}

	public float ReturnEngineDist()
	{
		// Ratio for actual to game is 1:10,000 as Vector3.MoveTowards()
		// Ratio for actual to game is 1:50,000 as Rigidbody.velocity
		Technology.TechNode tempEngine = shipTech.getNode(engineTech);
		
		int techLevel = tempEngine.ReturnLevels(engineTech);
		float techModifier = 1 + (techLevel * engineTechInfluence);
		techModifier *= (1 + tempEngine.ReturnBonus(engineTech));

		float tempReturn = techModifier * engineCapacity / 10000;

		return tempReturn;
	}
}
