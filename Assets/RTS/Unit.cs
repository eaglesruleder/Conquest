using System.Collections.Generic;
using System.Linq;

using UnityEngine; 

namespace OdWyer.RTS
{
	[RequireComponent(typeof(Rigidbody))]
	public class Unit : PlayerControlled
	{
		public bool upgWeaponActivatable = false;

		private Rigidbody _rigidBody = null;
		private Rigidbody Rigidbody => _rigidBody ? _rigidBody : (_rigidBody = GetComponent<Rigidbody>());


		private Loadout_Unit loadout;

		Loadable_Hull loadable;
		private List<Weapon> weapons = new List<Weapon>();

		public int currentKills = 0;

		public Queue<Vector3> destPositions = new Queue<Vector3>();
		Vector3? targetPosition = null;


		public override string Name => loadout.Loadout_Name;

		public override int Health => Mathf.CeilToInt(loadable.health + (loadable.healthFromArmour * loadout.armourLevel));
		public float Shield => loadable.shield + (loadable.shieldFromArmour * loadout.armourLevel);
		public override int Supply => Mathf.CeilToInt(loadable.supply + (loadable.supplyFromSupply * loadout.supplyLevel));


		public int armourLevel => loadout.armourLevel;
		public int supplyLevel => loadout.supplyLevel;


		// Ratio for actual to game is 1:100 as Vector3.MoveTowards()
		// Ratio for actual to game is 1:50,000? as Rigidbody.velocity
		public float Engine => loadable.engine * Time.deltaTime / 100f;
		public float StopDist => loadable.stopDist;


		// Multiply by 10 to make relative to gameworld
		public float EngageDistance => weapons.Max(w => w.engageDistance) * 10;
		public int Sensor => -1;

    
		public float DamPerSec => weapons.Sum(w => w.fireRate * w.volley * w.weaponDamage);
		public float SupPerSec => weapons.Sum(w => w.fireRate * w.volley * w.supplyDrain);
	
	
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

			Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			Rigidbody.drag = 1f;
			Rigidbody.angularDrag = 1f;
			Rigidbody.useGravity = false;
		
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

			if (CurrentHealth == 0)
				return false;

			if (CurrentHealth > damage)
			{
				damageTaken += damage;
				return false;
			}

			damageTaken = Mathf.Infinity;

			OnBecameInvisible();
			Selected(false);

			Vector3 torque = (transform.position - hitPoint).normalized * damage;
			torque.x = torque.z;
			Rigidbody.AddTorque(torque);

			EndSelf();
			return true;
		}

		public void SupplyBurn(int supplies) => supplyDrained += supplies;

		void OnCollisionStay(Collision hit)
		{
			Vector3 targetDir = hit.transform.position - transform.position;

			Vector3 targetSide = transform.right;
			if (Vector3.Angle(transform.right, targetDir) > 180)
				targetSide *= -1;

			Vector3 newPos = transform.position - transform.forward;
			transform.position = Vector3.MoveTowards(transform.position, newPos + targetSide, Engine);
		}

		public override void EndSelf()
		{
			ParticleSystem dieEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(loadable.deathEffect);
			dieEffect.transform.position = transform.position;
			dieEffect.transform.rotation = transform.rotation;
			dieEffect.Emit(150);
			Destroy (dieEffect.gameObject, 2f);

			foreach (Weapon wp in weapons)
				wp.EndNow();

			base.EndSelf();
		}



		public void Update()
		{
			UpdateAiming();
			UpdatePosition();
		}

		//---	TARGETING	---//
		public PlayerControlled targetObj = null;

		private Transform _aimTransform = null;
		private Transform AimTransform
		{ get {
			if (!_aimTransform)
			{
				_aimTransform = new GameObject("Aim").transform;
				_aimTransform.SetParent(transform);
				_aimTransform.localPosition = Vector3.zero;
				_aimTransform.localRotation = Quaternion.identity;
			}

			return _aimTransform;
		}	}

		private void UpdateAiming()
		{
			if(!targetObj)
				FindTarget();

			Vector3 targetAim = transform.forward;
			if (targetObj)
				targetAim = (targetObj.transform.position - transform.position).normalized;

			bool aimLock = Vector3.Angle(AimTransform.forward, targetAim) < 5;
			foreach (Weapon w in weapons)
				w.Fire(targetObj && aimLock);

			if (aimLock)
				return;

			AimTransform.rotation = Quaternion.Slerp
				(AimTransform.rotation
				,Quaternion.LookRotation(targetAim)
				,Time.deltaTime * 2
				);

			foreach (Weapon w in weapons)
				w.AimTarget(AimTransform.forward);
		}

		private void FindTarget()
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, EngageDistance);
			foreach (Collider c in colliders)
			{
				PlayerControlled pc = c.GetComponent<PlayerControlled>();
				if (pc && pc.playerID != playerID)
				{
					SetTarget(pc);
					return;
				}
			}
		}

		public override void SetTarget(PlayerControlled target)
		{
			targetObj = target;
			foreach (Weapon weapon in weapons)
				weapon.SetTarget(targetObj);
		}


		//---	MOVEMENT	---//
		private void UpdatePosition()
		{
			if (targetPosition.HasValue
			&&	Vector3.Distance(transform.position, targetPosition.Value) <= StopDist
				)
				targetPosition = null;

			if (!targetPosition.HasValue)
				FindNextPosition();

			if(!targetPosition.HasValue)
			{
				if (transform.forward.z != 0)
					transform.rotation = Quaternion.Slerp
						(transform.rotation
						,Quaternion.LookRotation(new Vector3(transform.forward.x, 0, transform.forward.z))
						,Time.deltaTime * 2
						);

				return;
			}

			Vector3 targetDir = (targetPosition.Value - transform.position).normalized;

			float engAngle = Vector3.Angle(transform.forward, targetDir);
			if (engAngle >= 90)
				engAngle = 90;
			engAngle *= Mathf.Deg2Rad;

			Rigidbody.MovePosition(transform.position + (targetDir * Mathf.Cos(engAngle) * Engine));
			transform.rotation = Quaternion.Slerp
				(transform.rotation
				,Quaternion.LookRotation(targetDir)
				,Time.deltaTime * 2
				);
		}

		private void FindNextPosition()
		{
			if (destPositions.Any())
			{
				Vector3 relPos = destPositions.Peek() - transform.position;
				if (Physics.SphereCast
					(new Ray(transform.position, relPos)
					,GetComponent<CapsuleCollider>().radius * 1.1f
					,out RaycastHit checkHit
					,Vector3.Distance(Vector3.zero, relPos)
					,1 << LayerMask.NameToLayer("Environment")
					))
				{
					Vector3 objectPos = checkHit.transform.position - transform.position;
					Vector3 pointPos = checkHit.point - transform.position;
					Vector3 perpNorm = Vector3.Cross(objectPos, pointPos);
					Vector3 avoidDir = Vector3.Cross(perpNorm, objectPos).normalized;

					float avoidDist = checkHit.collider.gameObject.GetComponent<CapsuleCollider>().radius + GetComponent<CapsuleCollider>().height;
					targetPosition = checkHit.transform.position + avoidDir * avoidDist;
					return;
				}

				targetPosition = destPositions.Dequeue();
				return;
			}
			
			if (targetObj
			&&	Vector3.Distance(targetObj.transform.position, transform.position) > (EngageDistance * 0.8f)
				)
				targetPosition = targetObj.transform.position;
		}

		public override void SetMove(Vector3 position, bool force = false)
		{
			position.y = Mathf.Clamp(position.y, -20, 20);
			Collider[] col = Physics.OverlapSphere (position, loadable.Loadable_Collider.height, 1 << LayerMask.NameToLayer("Environment"));
			foreach(Collider c in col)
			{
				Planet p = c.GetComponent<Planet>();
				if (!p)
					continue;
				
				float dist = c.GetComponent<CapsuleCollider>().radius + loadable.Loadable_Collider.height;
				Vector3 dir = (position - c.transform.position).normalized;
				position = c.transform.position + (dir * dist);
			}

			if(force)
			{
				destPositions.Clear();
				targetPosition = null;
			}

			destPositions.Enqueue(position);
		}
	}
}