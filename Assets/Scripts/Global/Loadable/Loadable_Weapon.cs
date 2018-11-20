using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Loadable_Weapon : Loadable {

    public string projectileID;

    public int weaponDamage = 0;
    public float[] armorBonus = new float[] { 0, 0, 0, 0 };

    public float engageDistance = 0f;

    public float fireRate = 0f;
	public int volley = 0;
    public int supplyDrain = 0;
    public float spin = 0f;

    public string yawObj;
    public float yawTransHeight;
    public string pitchObj;
    public float pitchTransHeight;

    public int points;

	public override System.Type Loadable_Component { get{ return typeof(Weapon); } }

    [System.Serializable]
    public class LW_Hanger : Loadable_Weapon
    {
		public override System.Type Loadable_Component { get { return typeof(WeaponHanger); } }

        public int noSquadrons = 1;
	}
}