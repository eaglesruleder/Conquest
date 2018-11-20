using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Binary_Environment
{
	public string version;
	public List<Loadable_Environment> Loadable_Enviroments
		{get
			{
			List<Loadable_Environment> result = new List<Loadable_Environment>();

			result.AddRange(planets);

			return result;
			}
		}

	public Loadable_Environment.LE_Planet[] planets;
	
	public void Save(string fileName)
	{
		BinaryFormatter format = new BinaryFormatter();
		FileStream file = File.Open(Application.dataPath + "/data/" + fileName + ".conqenv", FileMode.Create);
		
		format.Serialize(file, this);
		file.Close();
	}

	public static Binary_Environment Load(string fileName)
	{
		BinaryFormatter format = new BinaryFormatter ();
		FileStream file = File.Open (Application.dataPath + "/data/" + fileName + ".conqenv", FileMode.Open);
		
		Binary_Environment result = (Binary_Environment)format.Deserialize (file);
		file.Close();

		return result;
	}

	public static bool Exists(string fileName)
	{
		return File.Exists (Application.dataPath + "/data/" + fileName + ".conqenv");
	}
}
