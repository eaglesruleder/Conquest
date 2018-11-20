using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class UnitWeapon : MonoBehaviour 
{
	//All of the presets to be edited
	public string weaponName;
	public Unit unitData;

	public float weaponTechInfluence;
	public Technology.TechLocation techLoc;
    Technology.TechNode techNode;
	
	public int weaponDamage = 0;
    public float[] armorBonus = new float[] { 0, 0, 0, 0 };

    public float engageDistance = 0f;

	public float fireRate = 0;
    public int supplyDrain = 0;
    public float spin;
	
	public PlayerControlled targetObj;

    public GameObject rollObjLow;
    public GameObject yawObjLow;
    public float yawTransHeight;
    public GameObject pitchObjLow;
    public float pitchTransHeight;

    //Dynamic Fields
    GameObject rollObj;
    GameObject yawObj;
    GameObject pitchObj;

	public Vector3 pitchEuler;

    public virtual void Build(Technology PlayerTech, Unit UnitData)
    {
        techNode = PlayerTech.getNode(techLoc);
        unitData = UnitData;

        if (rollObjLow)
        {
            rollObj = Instantiate(rollObjLow) as GameObject;
            rollObj.transform.SetParent(transform);
            rollObj.transform.localPosition = Vector3.zero;
        }

        if (yawObjLow)
        {
            yawObj = Instantiate(yawObjLow) as GameObject;
            yawObj.transform.SetParent(transform);
            yawObj.transform.localPosition = new Vector3(0, yawTransHeight, 0);
        }

        if (pitchObjLow)
        {
            pitchObj = Instantiate(pitchObjLow) as GameObject;
            pitchObj.transform.SetParent(transform);
            pitchObj.transform.localPosition = new Vector3(0, pitchTransHeight, 0);
        }
    }

    public virtual void SetTarget(PlayerControlled TargetObj)
    {
        targetObj = TargetObj;
    }

	//Apply Targeting
    public virtual void AimTarget(Vector3 aimRot, bool fire)
    {
		pitchEuler = aimRot;
        if(pitchObj)
        {
            if(spin == 0)
            {
                pitchObj.transform.rotation = Quaternion.LookRotation(aimRot, Vector3.up);
            }
            else
            {
                pitchObj.transform.rotation = Quaternion.LookRotation(aimRot, pitchObj.transform.up);
                if(fire && spin != 0)
                {
                    pitchObj.transform.Rotate(0, 0, spin * 100 * Time.deltaTime);
                }
            }
        }
        if(yawObj)
        {
            yawObj.transform.rotation = Quaternion.LookRotation(aimRot, transform.up);
        }
    }

    public virtual void EndNow() { }

	public int returnWeaponOut()
		{
		int techLevel = techNode.ReturnLevels(techLoc);
		float techModifier = 1 + (techLevel * weaponTechInfluence);
        techModifier *= (1 + techNode.ReturnBonus(techLoc));
		
		return Mathf.CeilToInt(techModifier * weaponDamage);;
		}
}