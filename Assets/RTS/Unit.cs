using System.Collections.Generic;
using System.Linq;

using UnityEngine; 

namespace OdWyer.RTS
{
	public interface IUnitValues
	{
		string FactionID { get; }

		float Speed { get; }
		float TurnSpeed { get; }

		float EngageDistance { get; }

		float AvoidDistance { get; }
		float StopDistance { get; }

		float Shield { get; }
		int Armour { get; }
		int MaxHealth { get; }
		int MaxSupply { get; }

		string ShieldEffectID { get; }
		string DeathEffectID { get; }
	}

	[RequireComponent(typeof(MovementBehaviour)), RequireComponent(typeof(TargetingBehaviour))]
	public class Unit : PlayerControlled
		,IUnitValues
	{
		public bool upgWeaponActivatable = false;

		private Loadout_Unit loadout;

		public Loadable_Hull loadable;
		public List<Weapon> weapons = new List<Weapon>();
    
		public float DamPerSec => weapons.Sum(w => w.fireRate * w.volley * w.weaponDamage);
		public float SupPerSec => weapons.Sum(w => w.fireRate * w.volley * w.supplyDrain);

		public string FactionID => PlayerID;

		// Ratio for actual to game is 1:100 as Vector3.MoveTowards()
		// Ratio for actual to game is 1:50,000? as Rigidbody.velocity
		public float Speed => loadable.engine * Time.deltaTime / 100f;
		public float TurnSpeed => 2;
		public float EngageDistance => weapons.Max(w => w.engageDistance);
		public float AvoidDistance => GetComponent<CapsuleCollider>().radius * 1.1f;
		public float StopDistance => loadable.stopDist;

		public float Shield => loadable.shield + (loadable.shieldFromArmour * loadout.armourLevel);
		public int Armour => loadout.armourLevel;

		public string ShieldEffectID => loadable.shieldHit;
		public string DeathEffectID => loadable.deathEffect;


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
			
			HullBehaviour hull = hullobj.GetComponent<HullBehaviour>();
			hull.BaseHealth = loadable.health;
			hull.HealthFromArmour = loadable.healthFromArmour;
			hull.ArmourLevel = loadout.armourLevel;

			hull.BaseSupply = loadable.supply;
			hull.SupplyFromSupply = loadable.supplyFromSupply;
			hull.SupplyLevel = loadout.supplyLevel;
        
			loadable.Loadable_Collider.AddComponent(gameObject);
		
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

			return this;
		}
	}
}