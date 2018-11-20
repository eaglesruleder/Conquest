using UnityEngine;
using System.Collections;

public class ProjectileLaser : Projectile {

    LineRenderer drawEffect;
    public Color hue = new Color(0.1f, 0.75f, 1f, 0.5f);
    public float width = 0.2f;

    public override void Build(PlayerControlled Target, Unit Launcher, Vector3 FireDirection, int Damage, float[] ArmourBonus)
    {
        base.Build(Target, Launcher, FireDirection, Damage, ArmourBonus);

        drawEffect = gameObject.AddComponent<LineRenderer>();
        drawEffect.useWorldSpace = true;
        drawEffect.material = new Material(Shader.Find("Unlit/Texture"));
        drawEffect.material.color = hue;
        drawEffect.SetWidth(width, width);
        drawEffect.SetVertexCount(2);
        drawEffect.SetPosition(0, transform.position);
        drawEffect.SetPosition(1, target.transform.position);

        Vector3 collisionPosition = target.transform.position;
        Collider targetCollider = target.GetComponent<Collider>();
        Ray ray = new Ray(transform.position, target.transform.position - transform.position);
        float dist = Vector3.Distance(target.transform.position, transform.position);
        RaycastHit rayOut;
        if(targetCollider.Raycast(ray, out rayOut, dist))
        {
            collisionPosition = rayOut.point;
        }
        target.Damage(damage, ArmourBonus, collisionPosition);
    }

    void Update()
    {
        width /= 2;
        drawEffect.SetWidth(width, width);
    }

    public override void OnTriggerEnter(Collider hit)
    {}
}
