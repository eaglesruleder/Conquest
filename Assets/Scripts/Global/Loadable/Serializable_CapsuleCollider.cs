using UnityEngine;
using System.Collections;

[System.Serializable]
public class Serializable_CapsuleCollider {

	public bool isTrigger;
	public Serializable_Vector3 center;
    public int direction;
	public float radius;
    public float height;

	public Serializable_CapsuleCollider()
	{
		isTrigger = false;
		center = new Serializable_Vector3 (0, 0, 0);
		direction = 0;
		radius = 0;
		height = 0;
	}

	public Serializable_CapsuleCollider(bool isTrigger, Serializable_Vector3 center, int direction, float radius, float height)
	{
		this.isTrigger = isTrigger;
		this.center = center;
		this.direction = direction;
		this.radius = radius;
		this.height = height;
	}

	//	Add a capsult collider to an existing object
	public CapsuleCollider AddComponent(GameObject source)
    {
		//	Radius of 0 is a null collider
		if(radius > 0)
		{
			CapsuleCollider collider = source.AddComponent<CapsuleCollider>();
			collider.isTrigger = isTrigger;
			collider.center = center.Convert;
			collider.direction = direction;
			collider.height = height;
			collider.radius = radius;
			return collider;
		}
		return null;
    }
}
