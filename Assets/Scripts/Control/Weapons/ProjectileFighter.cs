using UnityEngine;
using System.Collections;

public class ProjectileFighter : Projectile {

	public Vector3 targetLocation = Vector3.zero;
    
	bool travelReturn = false;
	public float travelRadius = 0;
	public float engageDistance = 15f;
    public float travelSpeed = 0.5f;
    public float colliderHeight = 0.1f;
	
	ParticleSystem trail;

	public Projectile fighterBullet;
    public float firerate = 0f;
    float pause = 0f;

    public override void Build(PlayerControlled Target, Unit Launcher, Vector3 FireDirection, int Damage, float[] ArmourBonus)
    {
        base.Build(Target, Launcher, FireDirection, Damage, ArmourBonus);
        targetLocation = transform.position;

        SphereCollider tempCollider = gameObject.AddComponent<SphereCollider>();
        tempCollider.isTrigger = true;
        tempCollider.radius = colliderHeight;

        Rigidbody fighterRB = gameObject.AddComponent<Rigidbody>();
        fighterRB.useGravity = false;
    }

    public void SetTarget(PlayerControlled Target)
    {
        target = Target;
        targetPlayerID = target.playerID;
        targetLocation = RandomLocation(engageDistance);
    }

    public override void EndNow()
    {
        travelReturn = true;
        base.EndNow();
    }

    public override void OnTriggerEnter(Collider hit)
    {
        if(travelReturn && hit.GetComponent<PlayerControlled>())
        {
            if(hit.GetComponent<PlayerControlled>().Equals(launcher))
            {
                Destroy(this);
            }
        }
    }

    void Update()
    {
        Vector3 dest = (target) ? target.transform.position : launcher.transform.position;
        dest += targetLocation;

        //If running out of fuel return to launcher
        if (travelReturn)
        {
            targetLocation = launcher.transform.position;
        }

        // Else if nearby the target dest find new location
        else if (Vector3.Distance(dest, transform.position) < travelSpeed)
        {
            targetLocation = (target) ? RandomLocation(engageDistance) : RandomLocation(travelRadius);
        }

        if(target && pause < Time.time)
        {
            if(Vector3.Distance(target.transform.position, transform.position) < engageDistance)
            {
                Projectile tempBullet = Instantiate(fighterBullet) as Projectile;
                tempBullet.transform.position = transform.position;
                tempBullet.Build(target, launcher, target.transform.position - transform.position, damage, armourBonus);
                pause = Time.time + firerate;
            }
        }

        // Apply move
        transform.rotation = Quaternion.LookRotation(dest - transform.position, Vector3.up);
        Vector3 travelNorm = Vector3.Normalize(dest - transform.position);
        transform.position += travelNorm * travelSpeed;
    }

    Vector3 RandomLocation(float Radius)
    {
        float distFromTarget = Random.Range(Radius / 2, Radius);
        float randAngle = Random.Range(0, 360f) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(randAngle) * distFromTarget, 0, Mathf.Cos(randAngle) * distFromTarget);
    }
}
