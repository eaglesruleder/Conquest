using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Native_Terran
{
	public static class Projectiles
	{
		public static List<Loadable_Projectile> All
		{get{return new List<Loadable_Projectile>(){
					 Ion
					,Plasma
					,Arc
					,Laser
					,Fighter_Laser
					,Missile
					,Fighter
		};}}

		private static Loadable_Projectile Ion
		{get{	Loadable_Projectile.LP_Projectile res = new Loadable_Projectile.LP_Projectile();
				res.Loadable_ID			= "TER_PRO_Ion";
				res.Loadable_Name		= "Ion Round";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "TER_PRO_Ion_PAR";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,-0.5f)
					,2
					,0.1f
					,1f);
				
				res.life				= 10;
				
				res.velocity			= 1;
				
		return res;}}

		private static Loadable_Projectile Plasma
		{get{	Loadable_Projectile.LP_Projectile res = new Loadable_Projectile.LP_Projectile();
				res.Loadable_ID			= "TER_PRO_Plasma";
				res.Loadable_Name		= "Plasma Round";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "TER_PRO_Plasma_PAR";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,-0.5f)
					,2
					,0.1f
					,1f);
				
				res.life				= 10;
				
				res.velocity			= 1;
				
		return res;}}

		private static Loadable_Projectile Laser
		{get{	Loadable_Projectile.LP_Laser res = new Loadable_Projectile.LP_Laser();
				res.Loadable_ID			= "TER_PRO_Laser";
				res.Loadable_Name		= "Laser Blast";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				res.life				= 0.2f;
				
				res.line				= new Serializable_LineRenderer(
					0.2f
					,new float[]{1, 1, 1});
				res.verticalSlide		= false;
				res.slideDist			= 0;
				
		return res;}}

		private static Loadable_Projectile Arc
		{get{	Loadable_Projectile.LP_Arc res = new Loadable_Projectile.LP_Arc();
				res.Loadable_ID			= "TER_PRO_Arc";
				res.Loadable_Name		= "Lancer Arc";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				res.life				= 0.2f;
				
				res.line				= new Serializable_LineRenderer(
					0.2f
					,new float[]{1, 1, 1});
				res.verticalSlide		= false;
				res.slideDist			= 2.5f;

				res.bounce				= 4;
				res.range				= 10f;
				
		return res;}}

		private static Loadable_Projectile Fighter_Laser
		{get{	Loadable_Projectile.LP_Laser res = new Loadable_Projectile.LP_Laser();
				res.Loadable_ID			= "TER_PRO_FighterLaser";
				res.Loadable_Name		= "Fighter Laser Burst";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				res.life				= 0.2f;
				
				res.line				= new Serializable_LineRenderer(
					0.05f
					,new float[]{1, 1, 1});
				res.verticalSlide		= false;
				res.slideDist			= 0;
				
		return res;}}

		private static Loadable_Projectile Missile
		{get{	Loadable_Projectile.LP_Missile res = new Loadable_Projectile.LP_Missile();
				res.Loadable_ID			= "TER_PRO_Missile";
				res.Loadable_Name		= "Missile";
				res.Loadable_Mesh		= "TER_PRO_Missile_MESH";
				res.Loadable_Particle	= "TER_PRO_Missile_PAR";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,0.02f
					,0.1f);
				
				res.life				= 10;
				
				res.velocity			= 1;
				res.turnMagnitude		= 0.5f;
				
		return res;}}

		private static Loadable_Projectile Fighter
		{get{	Loadable_Projectile.LP_Fighter res = new Loadable_Projectile.LP_Fighter();
				res.Loadable_ID			= "TER_PRO_Fighter";
				res.Loadable_Name		= "Fighter";
				res.Loadable_Mesh		= "TER_PRO_Fighter_MESH";
				res.Loadable_Particle	= "TER_PRO_Fighter_PAR";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,0.1f
					,0.1f);
				
				res.life				= 30;
				
				res.travelRadius		= 90f;
				res.engageDistance		= 20f;
				res.travelSpeed			= 2500f;
				
				res.fighterProjectileID = "TER_PRO_FighterLaser";
				res.firerate			= 1;
				
		return res;}}
	}

	public static class Weapons
	{
		public static List<Loadable_Weapon> All
		{get{return new List<Loadable_Weapon>(){
					 Ion_Cannon
					,Plasma_Cannon
					,Laser_Turret
					,Missile_Pod
					,Hanger
		};}}

		private static Loadable_Weapon Ion_Cannon
		{get{	Loadable_Weapon res = new Loadable_Weapon();
				res.Loadable_ID			= "TER_WP_Ion";
				res.Loadable_Name		= "Ion Cannon";
				res.Loadable_Mesh		= "TER_WP_Ion_MESH_Base";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				res.projectileID		= "TER_PRO_Ion";
				res.weaponDamage		= 20;
				res.armorBonus = new float[]{30f, 20f, 10f, -10f};
				res.engageDistance		= 30;
				res.fireRate			= 2;
				res.volley				= -1;
				res.supplyDrain			= 4;
				res.spin				= 0;
				res.yawObj				= "TER_WP_Ion_MESH_Yaw";
				res.yawTransHeight		= 0.15f;
				res.pitchObj			= "TER_WP_Ion_MESH_Pitch";
				res.pitchTransHeight	= 0.15f;
				
		return res;}}

		private static Loadable_Weapon Plasma_Cannon
		{get{	Loadable_Weapon res = new Loadable_Weapon();
				res.Loadable_ID			= "TER_WP_Plasma";
				res.Loadable_Name		= "Plasma Cannon";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				
				res.projectileID		= "TER_PRO_Plasma";
				res.weaponDamage		= 40;
				res.armorBonus = new float[]{60f, 40f, 25f, 20f};
				res.engageDistance		= 45f;
				res.fireRate			= 3;
				res.volley				= -1;
				res.supplyDrain			= 4;
				res.spin				= 0;
				res.yawObj				= "TER_WP_Plasma_MESH_Yaw";
				res.yawTransHeight		= 0;
				res.pitchObj			= "TER_WP_Plasma_MESH_Pitch";
				res.pitchTransHeight	= 0;
				
		return res;}}

		private static Loadable_Weapon Laser_Turret
		{get{	Loadable_Weapon res = new Loadable_Weapon();
				res.Loadable_ID			= "TER_WP_Laser";
				res.Loadable_Name		= "Laser Turret";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				
				res.projectileID		= "TER_PRO_Laser";
				res.weaponDamage		= 3;
				res.armorBonus = new float[]{0f, -10f, -40f, -60f};
				res.engageDistance		= 1.75f;
				res.fireRate			= 0.75f;
				res.volley				= -1;
				res.supplyDrain			= 1;
				res.spin				= 0;
				res.yawObj				= "TER_WP_Laser_MESH_Yaw";
				res.yawTransHeight		= 0;
				res.pitchObj			= "TER_WP_Laser_MESH_Pitch";
				res.pitchTransHeight	= 0.075f;
				
		return res;}}

		private static Loadable_Weapon Arc
		{get{	Loadable_Weapon res = new Loadable_Weapon();
				res.Loadable_ID			= "TER_WP_Arc";
				res.Loadable_Name		= "Lancer Arc";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				
				res.projectileID		= "TER_PRO_Arc";
				res.weaponDamage		= 35;
				res.armorBonus = new float[]{13000f, 12000f, -15f, -30f};
				res.engageDistance		= 3.5f;
				res.fireRate			= 3.5f;
				res.volley				= -1;
				res.supplyDrain			= 25;
				res.spin				= 0;
				res.yawObj				= "";
				res.yawTransHeight		= 0;
				res.pitchObj			= "";
				res.pitchTransHeight	= 0f;
				
				return res;}}

		private static Loadable_Weapon Missile_Pod
		{get{	Loadable_Weapon res = new Loadable_Weapon();
				res.Loadable_ID			= "TER_WP_Missile";
				res.Loadable_Name		= "Missile Pod";
				res.Loadable_Mesh		= "";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				
				res.projectileID		= "TER_PRO_Missile";
				res.weaponDamage		= 14;
				res.armorBonus = new float[]{0f, -20f, 30f, 50f};
				res.engageDistance		= 45f;
				res.fireRate			= 3f;
				res.volley				= 2;
				res.supplyDrain			= 20;
				res.spin				= 0;
				res.yawObj				= "TER_WP_Missile_MESH_Yaw";
				res.yawTransHeight		= 0;
				res.pitchObj			= "TER_WP_Missile_MESH_Pitch";
				res.pitchTransHeight	= 0;
				
		return res;}}

		private static Loadable_Weapon Hanger
		{get{	Loadable_Weapon.LW_Hanger res = new Loadable_Weapon.LW_Hanger();
				res.Loadable_ID			= "TER_WP_Hanger";
				res.Loadable_Name		= "Hanger Bay";
				res.Loadable_Mesh		= "TER_WP_Hanger_MESH_Base";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider();
				
				
				res.projectileID		= "TER_PRO_Fighter";
				res.weaponDamage		= 6;
				res.armorBonus = new float[]{0, 0, 0, 0};
				res.engageDistance		= 90f;
				res.fireRate			= 5f;
				res.volley				= -1;
				res.supplyDrain			= 5;
				res.spin				= 0;
				res.yawObj				= "";
				res.yawTransHeight		= 0;
				res.pitchObj			= "";
				res.pitchTransHeight	= 0;
				
				res.noSquadrons			= 2;
				
		return res;}}
	}

	public static class Hulls
	{
		public static List<Loadable_Hull> All
		{get{return new List<Loadable_Hull>(){
					 Corvette
					,Cruiser
					,Battleship
		};}}

		private static Loadable_Hull Corvette
		{get{	Loadable_Hull res = new Loadable_Hull();
				res.Loadable_ID			= "TER_HULL_Corvette";
				res.Loadable_Name		= "Corvette";
				res.Loadable_Mesh		= "TER_HULL_Corvette_MESH";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,1f
					,1f);
				
				res.selectionObj		= "TER_HULL_Corvette_MESH_Select";
				res.deathEffect			= "TER_HULL_Corvette_PAR_Die";
				res.health				= 75;
				res.healthFromArmour	= 25;
				res.shieldHit			= "HULL_PAR_Hit";
				res.shield				= 0;
				res.shieldFromArmour	= 15;
				res.stopDist			= 2.5f;
				res.engine				= 2000;
				res.supply				= 40;
				res.supplyFromSupply	= 25;
				res.sensor				= 2;
				res.points				= 0;
				
		return res;}}

		private static Loadable_Hull Cruiser
		{get{	Loadable_Hull res = new Loadable_Hull();
				res.Loadable_ID			= "TER_HULL_Cruiser";
				res.Loadable_Name		= "Cruiser";
				res.Loadable_Mesh		= "TER_HULL_Cruiser_MESH";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,1.5f
					,5f);
				
				res.selectionObj		= "TER_HULL_Cruiser_MESH_Select";
				res.deathEffect			= "TER_HULL_Cruiser_PAR_Die";
				res.health				= 150;
				res.healthFromArmour	= 150;
				res.shieldHit			= "HULL_PAR_Hit";
				res.shield				= 0;
				res.shieldFromArmour	= 10;
				res.stopDist			= 2.5f;
				res.engine				= 1200;
				res.supply				= 250;
				res.supplyFromSupply	= 250;
				res.sensor				= 3;
				res.points				= 0;
				
		return res;}}

		private static Loadable_Hull Battleship
		{get{	Loadable_Hull res = new Loadable_Hull();
				res.Loadable_ID			= "TER_HULL_Battleship";
				res.Loadable_Name		= "Battleship";
				res.Loadable_Mesh		= "TER_HULL_Battleship_MESH";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,2f
					,8.5f);
				
				res.selectionObj		= "TER_HULL_Battleship_MESH_Select";
				res.deathEffect			= "TER_HULL_Battleship_PAR_Die";
				res.health				= 200;
				res.healthFromArmour	= 150;
				res.shieldHit			= "HULL_PAR_Hit";
				res.shield				= 5;
				res.shieldFromArmour	= 5;
				res.stopDist			= 2.5f;
				res.engine				= 1000;
				res.supply				= 300;
				res.supplyFromSupply	= 0;
				res.sensor				= 3;
				res.points				= 0;
				
		return res;}}

		private static Loadable_Hull Dreadonaught
		{get{	Loadable_Hull res = new Loadable_Hull();
				res.Loadable_ID			= "TER_HULL_Dreadonaught";
				res.Loadable_Name		= "Dreadonaught";
				res.Loadable_Mesh		= "TER_HULL_Dreadonaught_MESH";
				res.Loadable_Particle	= "";
				res.Loadable_Collider	= new Serializable_CapsuleCollider(
					true
					,new Serializable_Vector3(0,0,0)
					,2
					,0.1f
					,0.1f);
				
				res.selectionObj		= "TER_HULL_Dreadonaught_MESH_Select";
				res.deathEffect			= "TER_HULL_Dreadonaught_PAR_Die";
				res.health				= 400;
				res.healthFromArmour	= 300;
				res.shieldHit			= "HULL_PAR_Hit";
				res.shield				= 10;
				res.shieldFromArmour	= 5;
				res.stopDist			= 2.5f;
				res.engine				= 800;
				res.supply				= 450;
				res.supplyFromSupply	= 0;
				res.sensor				= 4;
				res.points				= 0;
				
		return res;}}
	}

	public static class Loadouts
	{
		public static List<Loadout_Unit> All
		{get{return new List<Loadout_Unit>(){
					 Corvette
					,MissileCruiser
					,Battleship
					,FleetCarrier
		};}}

		public static Loadout_Unit Corvette
		{get{	Loadout_Unit res = new Loadout_Unit();
				res.Loadout_ID			= "TER_UNIT_Corvette";
				res.Loadout_Name		= "Corvette";
				
				res.Loadout_Hull		= "TER_HULL_Corvette";
				res.armourLevel			= 1;
				res.supplyLevel			= 0;
				res.weapons				= new Loadout.WeaponPos[]
				{
					new Loadout.WeaponPos("TER_WP_Laser", new Serializable_Vector3(0, 0, -0.5f))
				};
				
		return res;}}

		public static Loadout_Unit MissileCruiser
		{get{	Loadout_Unit res = new Loadout_Unit();
				res.Loadout_ID			= "TER_UNIT_MissileCruiser";
				res.Loadout_Name		= "Missile Cruiser";
				
				res.Loadout_Hull		= "TER_HULL_Cruiser";
				res.armourLevel			= 1;
				res.supplyLevel			= 0;
				res.weapons				= new Loadout.WeaponPos[]
				{
					new Loadout.WeaponPos("TER_WP_Missile", new Serializable_Vector3(0, 0.5f, -0.75f))
				};
				
		return res;}}

		public static Loadout_Unit Battleship
		{get{	Loadout_Unit res = new Loadout_Unit();
				res.Loadout_ID			= "TER_UNIT_Battleship";
				res.Loadout_Name		= "Battleship";
				
				res.Loadout_Hull		= "TER_HULL_Battleship";
				res.armourLevel			= 3;
				res.supplyLevel			= 0;
				res.weapons				= new Loadout.WeaponPos[]
				{
					 new Loadout.WeaponPos("TER_WP_Ion", new Serializable_Vector3(0, 0.6f, 1))
					,new Loadout.WeaponPos("TER_WP_Ion", new Serializable_Vector3(0, 0.6f, -1))
				};
				
		return res;}}

		public static Loadout_Unit FleetCarrier
		{get{	Loadout_Unit res = new Loadout_Unit();
				res.Loadout_ID			= "TER_UNIT_FleetCarrier";
				res.Loadout_Name		= "Fleet Carrier";
				
				res.Loadout_Hull		= "TER_HULL_Battleship";
				res.armourLevel			= 1;
				res.supplyLevel			= 0;
				res.weapons				= new Loadout.WeaponPos[]
				{
					 new Loadout.WeaponPos("TER_WP_Hanger", new Serializable_Vector3(0, 0.6f, 1f))
					,new Loadout.WeaponPos("TER_WP_Hanger", new Serializable_Vector3(0, 0.6f, -1f))
				};
				
		return res;}}
	}
}
