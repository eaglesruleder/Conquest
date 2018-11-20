using UnityEngine;
using System.Collections;

public class IonAbilityProjectile : Projectile {

    Vector3 targetPos = Vector3.zero;
    Vector3 fireDirection = Vector3.zero;
    public float velocity = 0.2f;

    public ParticleSystem explosionEffect = null;
    ParticleSystem explodingEffect = null;
    float explosionDist = 0f;
    float explosionStep = 0f;

    float damagePerSec = 0f;

	public void Build(Vector3 TargetPos, Unit Launcher, int Damage, float[] ArmourBonus, float ExplosionRadius)
    {
        targetPos = TargetPos;
        fireDirection = (targetPos - transform.position).normalized;

        launcher = Launcher;
        launcherPlayerID = launcher.playerID;

        damage = Damage;
        damagePerSec = damage / life;
        armourBonus = ArmourBonus;

        explosionStep = ExplosionRadius / life;

        if(explosionEffect)
        {
            explosionEffect.startSpeed = explosionStep;
            explosionEffect.startLifetime = life;
        }
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, targetPos) < velocity)
        {
            Invoke("EndNow", life);
            if(!explodingEffect && explosionEffect)
            {
                explodingEffect = Instantiate(explosionEffect) as ParticleSystem;
                explodingEffect.transform.position = transform.position;
            }

            explosionDist += explosionStep * Time.deltaTime;
            int damageOut = Mathf.CeilToInt(damagePerSec * Time.deltaTime);
            foreach(Collider c in Physics.OverlapSphere(transform.position, explosionDist))
            {
                PlayerControlled hitObj = c.GetComponent<PlayerControlled>();
                if (hitObj)
                {
                    Vector3 randUp = Random.Range(1f, -1f) * hitObj.transform.up;
                    Vector3 randRight = Random.Range(1f, -1f) * Vector3.Cross(hitObj.transform.position - transform.position, Vector3.up).normalized * hitObj.gameObject.GetComponent<CapsuleCollider>().height / 2;
                    Vector3 hitPos = transform.position + randUp + randRight + (hitObj.transform.position - transform.position).normalized * explosionDist;
                    hitObj.Damage(damageOut, armourBonus, hitPos);
                }
            }
        }
        else
        {
            transform.position += fireDirection * velocity;
        }
    }

    public override void EndNow()
    {
        if (explodingEffect)
        {
            Destroy(explodingEffect.gameObject);
        }
        base.EndNow();
    }
}
