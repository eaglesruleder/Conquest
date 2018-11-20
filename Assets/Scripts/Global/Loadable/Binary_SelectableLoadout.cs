using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Binary_SelectableLoadout
{
	public string version;
	public List<Loadable_Hull> Loadable_Hulls
	{get 
		{
			List<Loadable_Hull> result = new List<Loadable_Hull>();

			result.AddRange(L_Hull);

			return result;
		}
	}

	public List<Loadable_Projectile> Loadable_Projectiles
	{get 
		{
			List<Loadable_Projectile> result = new List<Loadable_Projectile>();

			result.AddRange(LP_Projectile);
			result.AddRange(LP_Missile);
			result.AddRange(LP_Laser);
			result.AddRange(LP_Fighter);

			return result;
		}
	}

	public List<Loadable_Weapon> Loadable_Weapons
	{get 
		{
			List<Loadable_Weapon> result = new List<Loadable_Weapon>();

			result.AddRange(LW);
			result.AddRange(LW_Hanger);
			
			return result;
		}
	}


	public Loadable_Hull[] L_Hull;
	
	public Loadable_Projectile.LP_Projectile[] LP_Projectile;
	public Loadable_Projectile.LP_Missile[] LP_Missile;
	public Loadable_Projectile.LP_Laser[] LP_Laser;
	public Loadable_Projectile.LP_Fighter[] LP_Fighter;
	
	public Loadable_Weapon[] LW;
	public Loadable_Weapon.LW_Hanger[] LW_Hanger;
	
	
	public void Save(string fileName)
	{
		BinaryFormatter format = new BinaryFormatter();
		FileStream file = File.Open(Application.dataPath + "/data/" + fileName + ".conqsel", FileMode.Create);
		
		format.Serialize(file, this);
		file.Close();
	}
	
	public static Binary_SelectableLoadout Load(string fileName)
	{
		BinaryFormatter format = new BinaryFormatter ();
		FileStream file = File.Open (Application.dataPath + "/data/" + fileName + ".conqsel", FileMode.Open);
		
		Binary_SelectableLoadout result = (Binary_SelectableLoadout)format.Deserialize (file);
		file.Close();
		
		return result;
	}

	public static bool Exists(string fileName)
	{
		return File.Exists (Application.dataPath + "/data/" + fileName + ".conqsel");
	}
}
