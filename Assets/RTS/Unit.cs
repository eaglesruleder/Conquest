using System.Collections.Generic;
using System.Linq;

using UnityEngine; 

namespace OdWyer.RTS
{
	public interface IUnitValues
	{
		float Speed { get; }
		float TurnSpeed { get; }

		float AvoidDistance { get; }
		float StopDistance { get; }
	}

	[RequireComponent(typeof(HullBehaviour)), RequireComponent(typeof(MovementBehaviour)), RequireComponent(typeof(TargetingBehaviour))]
	public class Unit : PlayerControlled
		,IUnitValues
	{
		public bool upgWeaponActivatable = false;

		private Loadout_Unit loadout;

		public Loadable_Hull loadable;
		public List<Weapon> weapons = new List<Weapon>();
    
		public float DamPerSec => weapons.Sum(w => w.fireRate * w.volley * w.weaponDamage);
		public float SupPerSec => weapons.Sum(w => w.fireRate * w.volley * w.supplyDrain);

		// Ratio for actual to game is 1:100 as Vector3.MoveTowards()
		// Ratio for actual to game is 1:50,000? as Rigidbody.velocity
		public float Speed => loadable.engine * Time.deltaTime / 100f;
		public float TurnSpeed => 2;
		public float AvoidDistance => GetComponent<CapsuleCollider>().radius * 1.1f;
		public float StopDistance => loadable.stopDist;


		public Unit SetHull(Loadable_Hull loading)
		{
			loadable = loading;

			gameObject.name = loadable.Loadable_Name;

			if (!SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.selectionObj))
				throw new UnityException("MeshHandler " + loadable.selectionObj + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);

			if (!SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.Loadable_Mesh))
				throw new UnityException("MeshHandler " + loadable.Loadable_Mesh + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);

			if (!SelectableLoadout.ForgeAvailable<ParticleSystem>(loadable.shieldHit))
				throw new UnityException("ParticleSystem " + loadable.shieldHit + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);

			if (!SelectableLoadout.ForgeAvailable<ParticleSystem>(loadable.deathEffect))
				throw new UnityException("ParticleSystem " + loadable.deathEffect + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);

			if(!SelectionObj)
			{
				SelectionObj = ((MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.selectionObj)).gameObject;
				SelectionObj.transform.parent = transform;
				SelectionObj.transform.localPosition = Vector3.zero;
				SelectionObj.transform.localRotation = Quaternion.identity;
				Selected(false);
			}

			MeshHandler hullobj = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.Loadable_Mesh);
			hullobj.transform.parent = transform;
			hullobj.transform.localPosition = Vector3.zero;
			hullobj.transform.localRotation = Quaternion.identity;

			loadable.Loadable_Collider.AddComponent(gameObject);

			Config_HullBehaviour(loadable);
			Config_TargetingBehaviour(loadable);

			return this;
		}

		public Unit SetUnitLoadout(Loadout_Unit loading)
		{
			if (!loading.Loadout_Hull.Equals(loadable.Loadable_ID))
				throw new UnityException("hullID " + loadable.Loadable_ID + " does not match PlayerLoadout.UnitLoadout " + loading.Loadout_Hull);

			loadout = loading;
			gameObject.name = loadout.Loadout_Name;

			foreach (Loadout.WeaponPos wp in loadout.weapons)
			{
				if (!SelectableLoadout.ForgeAvailable<Weapon>(wp.weaponID))
					throw new UnityException("weaponID " + wp.weaponID + " declared but not found on PlayerLoadout.UnitLoadout " + loadout.Loadout_ID);
			}
		
			int points = loadable.points;
		
			foreach (Loadout.WeaponPos wp in loadout.weapons)
			{
				Weapon temp = (Weapon)SelectableLoadout.Forge<Weapon>(wp.weaponID);
			
				points -= temp.points;
				if(points < 0)
				{
					foreach(Weapon w in weapons)
						Destroy(w.gameObject);
					throw new UnityException("Points overdraw on " + loadout.Loadout_ID);
				}
			
				temp.transform.parent = transform;
				temp.transform.localPosition = wp.position.Convert;
				temp.SetUnit(this);
			
				weapons.Add(temp);
			}

			Config_HullBehaviour(loading);
			Config_TargetingBehaviour(loading);

			return this;
		}


		private HullBehaviour _hull = null;
		public HullBehaviour Hull => _hull ? _hull : (_hull = GetComponent<HullBehaviour>());

		private void Config_HullBehaviour(Loadable_Hull config)
		{
			Hull.BaseShield = config.shield;
			Hull.ShieldFromArmour = config.shieldFromArmour;

			Hull.BaseHealth = config.health;
			Hull.HealthFromArmour = config.healthFromArmour;

			Hull.BaseSupply = config.supply;
			Hull.SupplyFromSupply = config.supplyFromSupply;

			if (!Hull.ShieldEffect)
				Hull.ShieldEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(config.shieldHit);

			if (!Hull.DeathEffect)
				Hull.DeathEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(config.deathEffect);
		}

		private void Config_HullBehaviour(Loadout_Unit config)
		{
			Hull.ArmourLevel = loadout.armourLevel;
			Hull.SupplyLevel = loadout.supplyLevel;
		}


		private TargetingBehaviour _targeting = null;
		public TargetingBehaviour Targeting => _targeting ? _targeting : (_targeting = GetComponent<TargetingBehaviour>());

		private void Config_TargetingBehaviour(Loadable_Hull config)
		{
			Targeting.FactionID = PlayerManager.ThisPlayerID;
		}

		private void Config_TargetingBehaviour(Loadout_Unit config)
		{
			Targeting.EngageDistance = weapons.Max(w => w.engageDistance);
		}
	}
}