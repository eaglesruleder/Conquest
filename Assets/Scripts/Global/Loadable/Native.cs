using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Native
{
	public static string Version = "0.0.1";

	public static class Projectiles
	{
		public static List<Loadable_Projectile> All
		{get{	List<Loadable_Projectile> res = new List<Loadable_Projectile>();
				res.AddRange(Native_Terran.Projectiles.All);
		return res;}}
		
		private static Loadable_Projectile TEMPLATE_Projectile
		{get{	Loadable_Projectile.LP_Projectile res = new Loadable_Projectile.LP_Projectile();
				res.Loadable_ID			= "";
				res.Loadable_Name		= "";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,0f
					,0f);
				
				res.life				= 0;
				
				res.velocity			= 0;

		return res;}}
		
		private static Loadable_Projectile TEMPLATE_Laser
		{get{	Loadable_Projectile.LP_Laser res = new Loadable_Projectile.LP_Laser();
				res.Loadable_ID			= "";
				res.Loadable_Name		= "";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,0f
					,0f);
				
				res.life				= 0;
				
				res.line				= new Serializable_LineRenderer(
					 0f
					,new float[]{0, 0, 0, 0});
				res.verticalSlide		= false;
				res.slideDist			= 0;

		return res;}}
		
		private static Loadable_Projectile TEMPLATE_Missile
		{get{	Loadable_Projectile.LP_Missile res = new Loadable_Projectile.LP_Missile();
				res.Loadable_ID			= "";
				res.Loadable_Name		= "";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,0f
					,0f);
				
				res.life				= 0;
				
				res.velocity			= 0;
				res.turnMagnitude		= 0;

		return res;}}
		
		private static Loadable_Projectile TEMPLATE_Fighter
		{get{	Loadable_Projectile.LP_Fighter res = new Loadable_Projectile.LP_Fighter();
				res.Loadable_ID			= "";
				res.Loadable_Name		= "";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,0f
					,0f);
				
				res.life				= 0;
				
				res.travelRadius		= 0;
				res.engageDistance		= 0;
				res.travelSpeed			= 0;
				
				res.fighterProjectileID = "";
				res.firerate			= 0;
		
		return res;}}
	}

	public static class Weapons
	{
		public static List<Loadable_Weapon> All
		{get{	List<Loadable_Weapon> res = new List<Loadable_Weapon>();
				res.AddRange(Native_Terran.Weapons.All);
		return res;}}
		
		private static Loadable_Weapon TEMPLATE
		{get{	Loadable_Weapon res = new Loadable_Weapon();
				res.Loadable_ID			= "";
				res.Loadable_Name		= "";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				
				res.projectileID		= "";
				res.weaponDamage		= 0;
				res.armorBonus = new float[]{0, 0, 0, 0};
				res.engageDistance		= 0;
				res.fireRate			= 0;
				res.volley				= 0;
				res.supplyDrain			= 0;
				res.spin				= 0;
				res.yawObj				= "";
				res.yawTransHeight		= 0;
				res.pitchObj			= "";
				res.pitchTransHeight	= 0;
				
		return res;}}

		private static Loadable_Weapon TEMPLATE_Hanger
		{get{	Loadable_Weapon.LW_Hanger res = new Loadable_Weapon.LW_Hanger();
				res.Loadable_ID			= "";
				res.Loadable_Name		= "";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				
				res.projectileID		= "";
				res.weaponDamage		= 0;
				res.armorBonus = new float[]{0, 0, 0, 0};
				res.engageDistance		= 0;
				res.fireRate			= 0;
				res.volley				= 0;
				res.supplyDrain			= 0;
				res.spin				= 0;
				res.yawObj				= "";
				res.yawTransHeight		= 0;
				res.pitchObj			= "";
				res.pitchTransHeight	= 0;

				res.noSquadrons			= 0;
				
		return res;}}
	}

	public static class Hulls
	{
		public static List<Loadable_Hull> All
		{get{	List<Loadable_Hull> res = new List<Loadable_Hull>();
				res.AddRange(Native_Terran.Hulls.All);
		return res;}}
		
		private static Loadable_Hull TEMPLATE
		{get{	Loadable_Hull res = new Loadable_Hull();
				res.Loadable_ID			= "";
				res.Loadable_Name		= "";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,0f
					,0f);
				
				res.selectionObj		= "";
				res.deathEffect			= "";
				res.health				= 0;
				res.healthFromArmour	= 0;
				res.shieldHit			= "HULL_PAR_Hit";
				res.shield				= 0;
				res.shieldFromArmour	= 0;
				res.stopDist			= 2.5f;
				res.engine				= 0;
				res.supply				= 0;
				res.supplyFromSupply	= 0;
				res.sensor				= 0;
				res.points				= 0;
				
		return res;}}
	}

	public static class Environment
	{
		public static List<Loadable_Environment> All
		{get{	List<Loadable_Environment> res = new List<Loadable_Environment>();
				res.AddRange(Native_Environment.All);
		return res;}}
		
		private static Loadable_Environment TEMPLATE
		{get{	Loadable_Environment.LE_Planet res = new Loadable_Environment.LE_Planet();
				res.Loadable_ID			= "";
				res.Loadable_Name		= "";
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
				res.crewCapacity		= 0;
				
				res.metalGrowth			= 0;
				res.gasGrowth			= 0;
				res.crewGrowth			= 0;
				res.slotModelID			= "";
				res.slotCapacity		= 0;
				res.radius				= 0;
				
		return res;}}
	}

	public static class Loadouts
	{
		public static List<Loadout_Unit> All
		{get{	List<Loadout_Unit> res = new List<Loadout_Unit>();
				res.AddRange(Native_Terran.Loadouts.All);
		return res;}}

		private static Loadout_Unit TEMPLATE
		{get{	Loadout_Unit res = new Loadout_Unit();
				res.Loadout_ID			= "";
				res.Loadout_Name		= "";

				res.Loadout_Hull		= "";
				res.armourLevel			= 0;
				res.supplyLevel			= 0;
				res.weapons				= new Loadout.WeaponPos[]
					{
						new Loadout.WeaponPos("", new Serializable_Vector3(0, 0, 0))
					};

		return res;}}
	}
}
