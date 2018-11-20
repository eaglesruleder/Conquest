using UnityEngine;
using System.Collections;

public class LaserAbilityProjectile : ProjectileLaser {

    ProjectileFighter squadronTarget = null;

    public void Build(ProjectileFighter Target)
    {
        // Replaces only relevant component of base.Build
        Invoke("EndNow", life);

        squadronTarget = Target;
        width /= 2;

        drawEffect = gameObject.AddComponent<LineRenderer>();
        drawEffect.useWorldSpace = true;
        drawEffect.material = new Material(Shader.Find("Unlit/Texture"));
        drawEffect.material.color = hue;
        drawEffect.SetWidth(width, width);
        drawEffect.SetVertexCount(2);
        drawEffect.SetPosition(0, transform.position);
        drawEffect.SetPosition(1, squadronTarget.transform.position);

        Destroy(squadronTarget.gameObject);
    }
}
