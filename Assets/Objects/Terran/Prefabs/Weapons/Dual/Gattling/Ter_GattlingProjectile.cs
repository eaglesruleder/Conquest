using UnityEngine;
using System.Collections;

public class Ter_GattlingProjectile : ProjectileProjectile {

    ProjectileFighter squadronTarget = null;

    public void Build(ProjectileFighter Target, Unit Launcher, Vector3 FireDirection, int Damage, float[] ArmourBonus)
    {
        squadronTarget = Target;
        base.Build(Target.launcher, Launcher, FireDirection, Damage, ArmourBonus);
    }

    public override void Update()
    {
        if (squadronTarget)
        {
            Vector3 targetDir = squadronTarget.transform.position - transform.position;
            Vector3 perpVec = Vector3.Cross(fireDirection, targetDir);
            Vector3 moveVec = Vector3.Cross(perpVec, fireDirection).normalized;

            float angle = Vector3.Angle(targetDir, moveVec) * Mathf.Deg2Rad;
            float distToTarget = Vector3.Distance(Vector3.zero, targetDir);
            float modifier = Mathf.Pow(0.95f, Mathf.Sin(angle) * distToTarget);
            float sideDist = Mathf.Cos(angle) * distToTarget;
            transform.position += moveVec * sideDist * modifier;
            
            transform.position += fireDirection * velocity;
        }
        else
        {
            base.Update();
        }
    }

    public override void OnTriggerEnter(Collider hit)
    {
        if(squadronTarget)
        {
            ProjectileFighter hitFighter = hit.GetComponent<ProjectileFighter>();
            if (hitFighter)
            {
                if (hitFighter.launcherPlayerID == squadronTarget.launcherPlayerID)
                {
                    squadronTarget.EndNow();
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            base.OnTriggerEnter(hit);
        }
    }
}
