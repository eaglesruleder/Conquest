using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Loadable
{
	public string Loadable_ID;
	public string Loadable_Name;
	public string Loadable_Mesh;
	public string Loadable_Particle;
	public Serializable_CapsuleCollider Loadable_Collider;

	public abstract System.Type Loadable_Component { get; }
}
