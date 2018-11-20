using UnityEngine;
using System.Collections;

public class Binary_Generator : MonoBehaviour
{

	public bool willSave_Environment = false;
	public Binary_Environment toSave_Environment;

	public bool willSave_Selectable = false;
	public Binary_SelectableLoadout toSave_Selectable;

	public bool willSave_Native = false;
	public Binary_PlayerLoadout toSave_Native;

	public void SaveAll()
	{
		if (toSave_Environment != null && willSave_Environment)
		{
			if(!toSave_Environment.version.Equals(""))
			{
				print("Generating " + toSave_Environment.version + ".conqenv");
				toSave_Environment.Save("native");
			}
		}

		if (toSave_Selectable != null && willSave_Selectable)
		{
			if(!toSave_Selectable.version.Equals(""))
			{
				print("Generating " + toSave_Selectable.version + ".conqsel");
				toSave_Selectable.Save(toSave_Selectable.version);
			}
		}
		
		if (toSave_Native != null && willSave_Native)
		{
			if(!toSave_Native.version.Equals("") && toSave_Native.playerID.Equals(""))
			{
				print("ToSave " + toSave_Native.version + ".conqpla");
				toSave_Native.Save("native");

			}
		}
	}
}
