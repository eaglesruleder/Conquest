using UnityEngine;  
using System.Collections;

public class UnitWeaponProjectile : UnitWeapon {

	override
	//Used for any additional requirements
	public void BuildExtend()
	{}

	override
	//Used for fire mechanism
	public void Fire()
	{
	ProjectileProjectile bullet = Instantiate(projectile) as ProjectileProjectile;
	bullet.transform.position = gameObject.transform.position;

	bullet.transform.rotation = Quaternion.LookRotation(pitchEuler, Vector3.up);

	
	Vector3 fireDir = pitchEuler;
	if(pitchObj)
		fireDir = pitchObj.transform.forward;
	int damage = returnWeaponOut();
	bullet.Build(targetObj, unitData, fireDir, damage);
	}

	override
	public void SpecialFire()
	{}

    override
    public void Kill()
    {}
}
