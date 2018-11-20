using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class Loadable_Environment : Loadable
{
	public int metalCapacity;
	public int gasCapacity;
	public int crewCapacity;

	[System.Serializable]
	public class LE_Planet : Loadable_Environment
	{
		public override System.Type Loadable_Component { get { return typeof(Planet); } }

		public float metalGrowth;
		public float gasGrowth;
		public float crewGrowth;

		public string slotModelID;
		public int slotCapacity;
		public float radius;
	}
}
