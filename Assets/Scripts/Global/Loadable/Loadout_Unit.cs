using UnityEngine;
using System.Collections;

[System.Serializable]
public class Loadout_Unit : Loadout
{
	public string Loadout_Hull;

	public int armourLevel;
	public int supplyLevel;
	public WeaponPos[] weapons;
}
