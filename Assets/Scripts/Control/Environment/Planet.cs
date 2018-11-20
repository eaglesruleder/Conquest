using UnityEngine;  
using System.Collections;

[System.Serializable]
public class Planet : MonoBehaviour{

	public string title;

	public int metalCapacity = 0;
	public int gasCapacity = 0;
	public int crewCapacity = 0;

	public PlanetSlot slotTemplate;
	public int slotCapacity = 0;
	public float ringJump = 7.5f;
	public PlanetSlot[] planetSlots;

	void Start()
	{
	//Make all the slots
	planetSlots = new PlanetSlot[slotCapacity];
	
	if(slotCapacity > 0)
		{
		float angleSeperation = 360 / slotCapacity;
		float currentAngle = 0;

		for(int i = 0; i < planetSlots.Length; i++)
			{
			PlanetSlot temp = Instantiate(slotTemplate) as PlanetSlot;
			
			temp.transform.parent = gameObject.transform;
			temp.transform.localPosition = Vector3.zero;
			temp.transform.Rotate(Vector3.up * currentAngle, Space.World);
			currentAngle += angleSeperation;
			temp.transform.localPosition = temp.transform.localPosition + temp.transform.forward * ringJump;
			
			temp.host = this;
			
			planetSlots[i] = temp;
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
	}

	void Update()
		{
		foreach(PlanetSlot slot in planetSlots)
			{
			slot.enabled = slot.empty;
			}
		}
}
