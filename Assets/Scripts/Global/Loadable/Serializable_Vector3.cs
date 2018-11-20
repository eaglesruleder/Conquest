using UnityEngine;
using System.Collections;

[System.Serializable]
public class Serializable_Vector3 {

	public Serializable_Vector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

    public float x;
    public float y;
    public float z;

    public Vector3 Convert { get { return new Vector3(x, y, z); } }
}
