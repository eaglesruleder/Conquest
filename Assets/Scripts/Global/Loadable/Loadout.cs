using UnityEngine;
using System.Collections;

public class Loadout
{
	public string Loadout_ID;
	public string Loadout_Name;

	[System.Serializable]
	public class WeaponPos
	{
		public string weaponID;
		public Serializable_Vector3 position;

		public WeaponPos(string weaponID, Serializable_Vector3 position)
		{
			this.weaponID = weaponID;
			this.position = position;
		}
	}
}
