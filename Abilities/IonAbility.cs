using UnityEngine;
using System.Collections;

public class IonAbility : AbilityWeapon {

    bool aiming = false;
    CircleRenderer aimCircle;
    CircleRenderer targetCircle;
    LineRenderer aimLine;

    public int abilityDamage = 0;
    public float abilityRange = 0f;
    public float abilitySplashRange = 0f;
    public IonAbilityProjectile abilityProjectile;

    public override void Build(Technology PlayerTech, Unit UnitData)
    {
        base.Build(PlayerTech, UnitData);

        GameObject temp;
        temp = (Instantiate(new GameObject()) as GameObject);
        aimCircle = temp.AddComponent<CircleRenderer>();
        aimCircle.Initialise();
        aimCircle.gameObject.SetActive(false);

        temp = Instantiate(new GameObject()) as GameObject;
        targetCircle = temp.AddComponent<CircleRenderer>();
        targetCircle.Initialise();
        targetCircle.SetRadius(abilitySplashRange);
        targetCircle.gameObject.SetActive(false);

        aimLine = gameObject.AddComponent<LineRenderer>();
        aimLine.useWorldSpace = true;
        aimLine.material = new Material(Shader.Find("Unlit/Texture"));
        aimLine.material.color = new Color(1f, 0.92f, 0.016f, 0.5f);
        aimLine.SetWidth(0.2f, 0.2f);
        aimLine.SetVertexCount(2);
        aimLine.enabled = false;
    }

    public override void Activate()
    {
        aiming = true;
        aimCircle.gameObject.SetActive(true);
        targetCircle.gameObject.SetActive(true);
        aimLine.enabled = true;
    }

    void OnGUI()
    {
        if(aiming)
        {
            Vector3 aimPos = transform.position;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane collPlane = new Plane(Vector3.up, transform.position);
            float distance = 0f;
            if (collPlane.Raycast(ray, out distance))
            {
                aimPos = ray.GetPoint(distance);
            }

            if (Input.GetMouseButton(0))
            {
                aiming = false;
                if(unitData.currentSupply > abilityDrain)
                {
                    IonAbilityProjectile tempBullet = Instantiate(abilityProjectile) as IonAbilityProjectile;
                    tempBullet.transform.position = transform.position;
                    tempBullet.transform.rotation = Quaternion.LookRotation(aimPos - transform.position);

                    tempBullet.Build(aimPos, coreWeapon.unitData, abilityDamage, coreWeapon.armorBonus, abilitySplashRange);

                    unitData.SupplyBurn(abilityDrain);
                }
            }
            else if (Input.GetMouseButton(1))
            {
                aiming = false;
            }
            else
            {
                Vector3 shipTrans = unitData.transform.position;
                aimCircle.transform.position = shipTrans;
                aimCircle.SetRadius(Vector3.Distance(shipTrans, aimPos));
                targetCircle.transform.position = aimPos;
                aimLine.SetPosition(0, transform.position);
                aimLine.SetPosition(1, aimPos);
            }
        }
        else
        {
            aimCircle.gameObject.SetActive(false);
            targetCircle.gameObject.SetActive(false);
            aimLine.enabled = false;
        }
    }
}
