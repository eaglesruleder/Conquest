using UnityEngine;  
using System.Collections;

[System.Serializable]
public class Planet : MonoBehaviour{

	internal Loadable_Environment.LE_Planet loadable;

	public string Name { get { return loadable.Loadable_Name; } }

	public int metalCapacity { get { return loadable.metalCapacity; } }
	public float metalGrowth { get { return loadable.metalGrowth; } }
	public int gasCapacity { get { return loadable.gasCapacity; } }
	public float gasGrowth { get { return loadable.gasGrowth; } }
	public int crewCapacity { get { return loadable.crewCapacity; } }
	public float crewGrowth { get { return loadable.crewGrowth; } }

	PlanetSlot[] planetSlots;

	public Planet SetPlanet(Loadable_Environment.LE_Planet loading)
	{
		loadable = loading;

		gameObject.name = Name;

		gameObject.layer = LayerMask.NameToLayer ("Environment");

		if(SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.Loadable_Mesh))
		{
			MeshHandler model = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.Loadable_Mesh);
			model.transform.parent = transform;
			model.transform.localPosition = Vector3.zero;
			model.transform.localRotation = Quaternion.identity;
		}

		loadable.Loadable_Collider.AddComponent (gameObject);

		//Make all the slots
		planetSlots = new PlanetSlot[loadable.slotCapacity];
		
		if(loadable.slotCapacity > 0 && SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.slotModelID))
		{
			float angleSeperation = 360 / loadable.slotCapacity;
			float currentAngle = 0;
			
			for(int i = 0; i < loadable.slotCapacity; i++)
			{
				MeshHandler temp = (MeshHandler) SelectableLoadout.Forge<MeshHandler>(loadable.slotModelID);

				temp.transform.parent = transform;
				temp.transform.localPosition = Vector3.zero;
				temp.transform.Rotate(Vector3.up * currentAngle, Space.World);
				currentAngle += angleSeperation;
				temp.transform.localPosition = temp.transform.localPosition + temp.transform.forward * loadable.radius;

				PlanetSlot slot = temp.gameObject.AddComponent<PlanetSlot>();
				slot.SetHost(this);
				
				planetSlots[i] = slot;
			}
			
			//Set First
			int j = 0;
			planetSlots[j].SetSides(planetSlots[planetSlots.Length - 1], planetSlots[1]);
			j++;
			
			//Set Middle
			for(; j < planetSlots.Length - 1; j++)
			{
				planetSlots[j].SetSides(planetSlots[j-1], planetSlots[j+1]);
			}
			
			//Set Last
			planetSlots[j].SetSides(planetSlots[j-1], planetSlots[0]);
		}

		return this;
	}
}
