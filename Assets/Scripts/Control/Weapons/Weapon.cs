using UnityEngine;

using OdWyer.RTS;

[System.Serializable]
public class Weapon : MonoBehaviour
{
	private HullBehaviour _hull = null;
	private HullBehaviour Hull => _hull ? _hull : (_hull = GetComponentInParent<HullBehaviour>());

	//Weapon Values
	internal Loadable_Weapon loadable;
	
	public	 string		Name			{get{return loadable.Loadable_Name;}}
	internal string		projectileID	{get{return loadable.projectileID;}}
	public	 int		weaponDamage
    {
        get
        {
			float result = loadable.weaponDamage;// + (techDamage.Level * techDamage.influence);
            //result *= (1 + techDamage.Bonus);
            return Mathf.CeilToInt(result);
        }
    }
	internal float[]	armorBonus		{get{return loadable.armorBonus;}}
	public	 float		engageDistance	{get{return loadable.engageDistance;}}
	public	 float		fireRate		{get{return loadable.fireRate;}}
	public	 int		volley			{get{return (loadable.volley < 1) ? 1 : loadable.volley;}}// + techVolley.Level;}}
	public	 int		supplyDrain		{get{return loadable.supplyDrain;}}
	internal float		spin			{get{return loadable.spin;}}
	internal MeshHandler rollObj;
    internal MeshHandler yawObj;
    internal MeshHandler pitchObj;
	public	 int points					{get{return loadable.points;}}

	
	public PlayerControlled targetObj;
    public Unit unitData;
	public Vector3 pitchEuler;

	public virtual Weapon SetWeapon(Loadable_Weapon loading)
	{
		loadable = loading;
		
		gameObject.name = Name;

		if (!SelectableLoadout.ForgeAvailable<Projectile>(loadable.projectileID))
        {
			throw new UnityEngine.UnityException("projectileID " + loadable.projectileID + " declared but not found on Loadable_UnitWeapon " + loadable.Loadable_ID);
        }

		if (SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.Loadable_Mesh))
        {
			rollObj = (MeshHandler) SelectableLoadout.Forge<MeshHandler>(loadable.Loadable_Mesh);
            rollObj.transform.parent = transform;
            rollObj.transform.localPosition = new Vector3(0, 0, 0);
        }

		if (SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.yawObj))
        {
			yawObj = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.yawObj);
            yawObj.transform.parent = transform;
			yawObj.transform.localPosition = new Vector3(0, loadable.yawTransHeight, 0);
        }

		if (SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.pitchObj))
        {
			pitchObj = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.pitchObj);
            pitchObj.transform.parent = transform;
			pitchObj.transform.localPosition = new Vector3(0, loadable.pitchTransHeight, 0);
        }

        return this;
	}

    public virtual void SetUnit(Unit UnitData)
    {
        unitData = UnitData;
    }

    public virtual void SetTarget(PlayerControlled TargetObj)
    {
        targetObj = TargetObj;
    }

	//Apply Targeting
    public virtual void AimTarget(Vector3 aimRot)
    {
		// Set targeting vec3 to given rotation
		pitchEuler = aimRot;

		// If pitch object exists set to rotation
        if(pitchObj)
        {
			// If no spin then align with 'up' else keep current rotation
            pitchObj.transform.rotation = Quaternion.LookRotation(aimRot, (spin == 0) ? transform.up : pitchObj.transform.up);
			if(IsInvoking("FireVolley") && spin != 0)
            {
				// rotate when firing
                pitchObj.transform.Rotate(0, 0, spin * Time.deltaTime);
            }
        }

		//	If yawObj exists
        if(yawObj)
        {
			// Set with y of 0 to keep straight vertcally
			yawObj.transform.rotation = Quaternion.LookRotation(new Vector3(aimRot.x, 0, aimRot.z), transform.up);
        }
    }

	public virtual void Fire(bool firing)
	{
		//	If firing
		if(firing)
		{
			//	And not already firing
			if (!IsInvoking("FireVolley"))
			{
				//	Start the volley
				InvokeRepeating("FireVolley", 0f, fireRate);// * (1 - techVolley.Bonus));
			}
		}
		//	Else canel the firing
		else
		{
			CancelInvoke("FireVolley");
		}
	}

	internal virtual void FireVolley()
	{
		//	For each bullet in a volley
		for (int i = 0; i < volley; i++)
		{
			//	Individually set the fire time of each bullet (rather than InvokeRepeating)
			Invoke("Fire", i * (fireRate / 3f / (float)volley));// * (1 - techVolley.Bonus));
		}
	}

    internal virtual void Fire()
    {
		if(targetObj)
		{
			//	Get bullet then send it off
	        Projectile tempBullet = (Projectile)SelectableLoadout.Forge<Projectile>(projectileID);
	        tempBullet.transform.position = transform.position;
	        tempBullet.transform.rotation = Quaternion.LookRotation(pitchEuler);
	        tempBullet.Initialise(targetObj, unitData, weaponDamage, armorBonus);

			//	Burn supplies
			if(Hull)
				Hull.SupplyBurn(supplyDrain);
		}
    }
}