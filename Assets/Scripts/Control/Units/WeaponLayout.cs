using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponLayout : MonoBehaviour {

	public WeaponLocation[] weaponLocations;

	public WeaponLayout (WeaponLocation[] WeaponLocations)
		{
		weaponLocations = WeaponLocations;
		}

	[System.Serializable]
	public class WeaponLocation
		{
		public UnitWeapon weapon;
		public Vector3 location;
			public WeaponLocation(UnitWeapon Weapon, Vector3 Location)
				{
				weapon = Weapon;
				location = Location;
				}
		}
}
