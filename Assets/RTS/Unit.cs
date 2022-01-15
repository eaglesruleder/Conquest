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
	}

	[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(MovementBehaviour)), RequireComponent(typeof(TargetingBehaviour))]
	public class Unit : PlayerControlled
		,IUnitValues
	{
		private Rigidbody _rigidBody = null;
		private Rigidbody Rigidbody => _rigidBody ? _rigidBody : (_rigidBody = GetComponent<Rigidbody>());


		public bool upgWeaponActivatable = false;

		private Loadout_Unit loadout;

		public Loadable_Hull loadable;
		public List<Weapon> weapons = new List<Weapon>();

		public int currentKills = 0;

		public override string Name => loadout.Loadout_Name;

		public override int Health => Mathf.CeilToInt(loadable.health + (loadable.healthFromArmour * loadout.armourLevel));
		public float Shield => loadable.shield + (loadable.shieldFromArmour * loadout.armourLevel);
		public override int Supply => Mathf.CeilToInt(loadable.supply + (loadable.supplyFromSupply * loadout.supplyLevel));


		public int armourLevel => loadout.armourLevel;
		public int supplyLevel => loadout.supplyLevel;

		public int Sensor => -1;

    
		public float DamPerSec => weapons.Sum(w => w.fireRate * w.volley * w.weaponDamage);
		public float SupPerSec => weapons.Sum(w => w.fireRate * w.volley * w.supplyDrain);

		public string FactionID => playerID;

		// Ratio for actual to game is 1:100 as Vector3.MoveTowards()
		// Ratio for actual to game is 1:50,000? as Rigidbody.velocity
		public float Speed => loadable.engine * Time.deltaTime / 100f;
		public float TurnSpeed => 2;
		public float EngageDistance => weapons.Max(w => w.engageDistance);
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

			SelectionObj = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.selectionObj);
			SelectionObj.transform.parent = transform;
			SelectionObj.transform.localPosition = Vector3.zero;
			SelectionObj.transform.localRotation = Quaternion.identity;
			SelectionObj.gameObject.SetActive(false);

			MeshHandler hullobj = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.Loadable_Mesh);
			hullobj.transform.parent = transform;
			hullobj.transform.localPosition = Vector3.zero;
			hullobj.transform.localRotation = Quaternion.identity;
        
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
		
		public void KilledTarget() => currentKills++;

		public override bool Damage(int damage, float[] armorBonus, Vector3 hitPoint)
		{
			ParticleSystem hitEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(loadable.shieldHit);
			hitEffect.transform.position = hitPoint;
			hitEffect.transform.rotation = Quaternion.LookRotation(transform.position - hitPoint);
			Destroy (hitEffect.gameObject, 1f);
		
			ParticleSystem.MainModule main = hitEffect.main;
			main.startSpeed = damage / 10;

			hitEffect.Emit(damage * damage);
		
			float modifier = 1f - (Shield / 100) + (armorBonus[armourLevel] / 100);
			damage = Mathf.CeilToInt(damage * modifier);

			AddCollisionTorque(hitPoint, damage);

			if (CurrentHealth == 0)
				return false;

			if (CurrentHealth > damage)
			{
				damageTaken += damage;
				return false;
			}

			damageTaken = Mathf.Infinity;

			EndSelf();
			return true;
		}

		public void AddCollisionTorque(Vector3 hitPoint, float force)
		{
			Vector3 torque = (transform.position - hitPoint).normalized * force;
			torque.x = torque.z;
			Rigidbody.AddTorque(torque);
		}

		public void SupplyBurn(int supplies) => supplyDrained += supplies;

		public override void EndSelf()
		{
			Selected(false);

			ParticleSystem dieEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(loadable.deathEffect);
			dieEffect.transform.position = transform.position;
			dieEffect.transform.rotation = transform.rotation;
			dieEffect.Emit(150);
			Destroy (dieEffect.gameObject, 2f);

			foreach (Weapon wp in weapons)
				wp.EndNow();

			base.EndSelf();
		}
	}
}