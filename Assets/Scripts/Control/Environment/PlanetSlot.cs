using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlanetSlot : MonoBehaviour{

	PlanetSlot toLeft;
	PlanetSlot toRight;
	Planet host;
	bool empty = true;
	
	public void SetSides(PlanetSlot ToLeft, PlanetSlot ToRight)
	{
		toLeft = ToLeft;
		toRight = ToRight;
	}

	public void SetHost(Planet Host)
	{
		host = Host;
	}

	public bool Check(bool EmptyState, int Radius)
	{
		List<PlanetSlot> temp = GetSlots(Radius);
		foreach(PlanetSlot s in temp)
		{
			if(s.empty != EmptyState)
			{
				return false;
			}
		}
		return true;
	}

	List<PlanetSlot> GetSlots(int Radius)
	{
		List<PlanetSlot> result = new List<PlanetSlot>(){this};
		if(Radius != 0)
			{
			result.AddRange(toLeft.GetSlots(Radius, 0));
			result.AddRange(toRight.GetSlots(0, Radius));
			}
		return result;
	}

	List<PlanetSlot> GetSlots(int Left, int Right)
	{
		List<PlanetSlot> result = new List<PlanetSlot>(){this};
		if(Left != 0)
		{
			result.AddRange(toLeft.GetSlots(Left - 1, 0));
		}
		if(Right != 0)
		{
			result.AddRange(toRight.GetSlots(0, Right - 1));
		}
		return result;
	}
}
