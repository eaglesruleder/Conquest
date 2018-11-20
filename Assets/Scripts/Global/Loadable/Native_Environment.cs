using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Native_Environment {

	public static List<Loadable_Environment> All
	{get{return new List<Loadable_Environment>(){
				 Earth
				,Moon
				,GasGiant
				,Swamp
	};}}

	private static Loadable_Environment Earth
	{get{	Loadable_Environment.LE_Planet res = new Loadable_Environment.LE_Planet();
			res.Loadable_ID			= "ENV_PLA_Earth";
			res.Loadable_Name		= "Earth-Type Planet";
			res.Loadable_Mesh		= "";
			res.Loadable_Particle	= "";
			res.Loadable_Collider	= new Serializable_CapsuleCollider(
				true
				,new Serializable_Vector3(0,0,0)
				,2
				,0f
				,0f);
			
			res.metalCapacity		= 5000;
			res.gasCapacity			= 5000;
			res.crewCapacity		= 3500;
			
			res.metalGrowth			= 0;
			res.gasGrowth			= 0;
			res.crewGrowth			= 0;
			res.slotModelID			= "";
			res.slotCapacity		= 0;
			res.radius				= 0;
			
			return res;}}

	private static Loadable_Environment Moon
	{get{	Loadable_Environment.LE_Planet res = new Loadable_Environment.LE_Planet();
			res.Loadable_ID			= "ENV_PLA_Moon";
			res.Loadable_Name		= "Moon";
			res.Loadable_Mesh		= "ENV_PLANET_Moon_MESH";
			res.Loadable_Particle	= "";
			res.Loadable_Collider	= new Serializable_CapsuleCollider(
				true
				,new Serializable_Vector3(0,0,0)
				,2
				,5f
				,5f);
			
			res.metalCapacity		= 10000;
			res.gasCapacity			= 0;
			res.crewCapacity		= 0;
			
			res.metalGrowth			= 0;
			res.gasGrowth			= 0;
			res.crewGrowth			= 0;
			res.slotModelID			= "ENV_PLANET_Slot_MESH_Small";
			res.slotCapacity		= 0;
			res.radius				= 0;
			
			return res;}}

	private static Loadable_Environment GasGiant
	{get{	Loadable_Environment.LE_Planet res = new Loadable_Environment.LE_Planet();
			res.Loadable_ID			= "ENV_PLA_Gas";
			res.Loadable_Name		= "Gas Giant";
			res.Loadable_Mesh		= "";
			res.Loadable_Particle	= "";
			res.Loadable_Collider	= new Serializable_CapsuleCollider(
				true
				,new Serializable_Vector3(0,0,0)
				,2
				,0f
				,0f);
			
			res.metalCapacity		= 0;
			res.gasCapacity			= 10000;
			res.crewCapacity		= 0;
			
			res.metalGrowth			= 0;
			res.gasGrowth			= 0;
			res.crewGrowth			= 0;
			res.slotModelID			= "";
			res.slotCapacity		= 0;
			res.radius				= 0;
			
			return res;}}

	private static Loadable_Environment Swamp
	{get{	Loadable_Environment.LE_Planet res = new Loadable_Environment.LE_Planet();
			res.Loadable_ID			= "ENV_PLA_Swamp";
			res.Loadable_Name		= "Swamp Planet";
			res.Loadable_Mesh		= "";
			res.Loadable_Particle	= "";
			res.Loadable_Collider	= new Serializable_CapsuleCollider(
				true
				,new Serializable_Vector3(0,0,0)
				,2
				,0f
				,0f);
			
			res.metalCapacity		= 0;
			res.gasCapacity			= 0;
			res.crewCapacity		= 5000;
			
			res.metalGrowth			= 0;
			res.gasGrowth			= 0;
			res.crewGrowth			= 0;
			res.slotModelID			= "";
			res.slotCapacity		= 0;
			res.radius				= 0;
			
			return res;}}
}
