using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class UnitWeapon : MonoBehaviour 
{
	//All of the presets to be edited
	public string weaponName;
	public PlayerControlled unitData;

	public float weaponTechInfluence;
	Technology playerTech;
	public Technology.TechLocation weaponTech;
	
	public int weaponDamage = 0;
	public float fireRate = 0;
	public Projectile projectile;

	public float spin;
	
	public GameObject rollObjLow;
	public GameObject yawObjLow;
	public float yawTransHeight;
	public GameObject pitchObjLow;
	public float pitchTransHeight;
	
	//Dynamic Fields
	public GameObject rollObj;
	public GameObject yawObj;
	public GameObject pitchObj;
	public Vector3 pitchEuler;
	
	public PlayerControlled targetObj;
	
	public void Build(Technology PlayerTech, PlayerControlled UnitData)
		{
		playerTech = PlayerTech;
		unitData = UnitData;

		if(rollObjLow)
			{
			rollObj = Instantiate(rollObjLow) as GameObject;
			rollObj.transform.SetParent(gameObject.transform);
			rollObj.transform.localPosition = Vector3.zero;
			}

		if(yawObjLow)
			{
			yawObj = Instantiate(yawObjLow) as GameObject;
			yawObj.transform.SetParent(rollObj.transform);
			yawObj.transform.localPosition = new Vector3(0, yawTransHeight, 0);
			}

		if(pitchObjLow)
			{
			pitchObj = Instantiate(pitchObjLow) as GameObject;
			pitchObj.transform.SetParent(rollObj.transform);
			pitchObj.transform.localPosition = new Vector3(0, pitchTransHeight, 0);
			}

		BuildExtend();
		}

	//Used for any additional requirements
	public abstract void BuildExtend();

	//Apply Targeting
	public void Target(Vector3 YawObjRot, Vector3 PitchObjRot, bool Fire, bool Spin, PlayerControlled TargetObj)
		{
		targetObj = TargetObj;

		if(yawObj)
			yawObj.transform.eulerAngles = YawObjRot;

		pitchEuler = PitchObjRot;
		if(pitchObj)
			{
			if(spin!=0)
				pitchObj.transform.rotation = Quaternion.LookRotation(PitchObjRot, rollObj.transform.up);
			else
				pitchObj.transform.rotation = Quaternion.LookRotation(PitchObjRot, Vector3.up);
	
			float spinDelta = 0;
			if(spin != 0 && Spin)
				spinDelta = spin * 100 * Time.deltaTime;
			pitchObj.transform.Rotate(0, 0, spinDelta);
			}

		if(Fire)
			{
			if(!IsInvoking("Fire"))
				{
				Invoke("Fire", fireRate);
				Invoke("Fire", 0f);
				}
			}
		else
			CancelInvoke("Fire");
		}

	//Used for fire mechanism
	public abstract void Fire();

	//Run SpecialFire on Update for extra requirements
	void Update()
		{
		SpecialFire();
		}
	public abstract void SpecialFire();

    public abstract void Kill();

	public int returnWeaponOut()
		{
		Technology.TechNode tempWeapon = playerTech.getNode(weaponTech);
		
		int techLevel = tempWeapon.ReturnLevels(weaponTech);
		float techModifier = 1 + (techLevel * weaponTechInfluence);
		techModifier *= (1 + tempWeapon.ReturnBonus(weaponTech));
		
		int tempReturn = Mathf.CeilToInt(techModifier * weaponDamage);
		return tempReturn;
		}
}