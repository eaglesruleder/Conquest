using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Binary_PlayerLoadout {

	public string version;
	public List<Loadout_Unit> Loadouts
	{get
		{
			List<Loadout_Unit> result = new List<Loadout_Unit>();
			result.AddRange(playerLoadout);

			return result;
		}
	}

	public string playerID;
	public Loadout_Unit[] playerLoadout;
	
	public void Save(string fileName)
	{
		BinaryFormatter format = new BinaryFormatter();
		FileStream file = File.Open(Application.dataPath + "/data/" + fileName + ".conqpla", FileMode.Create);
		
		format.Serialize(file, this);
		file.Close();
	}
	
	public static Binary_PlayerLoadout Load(string fileName)
	{
		BinaryFormatter format = new BinaryFormatter ();
		FileStream file = File.Open (Application.dataPath + "/data/" + fileName + ".conqpla", FileMode.Open);
		
		Binary_PlayerLoadout result = (Binary_PlayerLoadout)format.Deserialize (file);
		file.Close();
		
		return result;
	}

	public static bool Exists(string fileName)
	{
		return File.Exists (Application.dataPath + "/data/" + fileName + ".conqpla");
	}
}
