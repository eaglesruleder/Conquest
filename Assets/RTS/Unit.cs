using System.Collections.Generic;
using System.Linq;

using UnityEngine; 

namespace OdWyer.RTS
{
	public class Unit : PlayerControlled
	{
		private static Vector3 posNull = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

		public bool upgWeaponActivatable = false;


		private Loadout_Unit loadout;

		Loadable_Hull loadable;
		private List<Weapon> weapons = new List<Weapon>();

		Technology shipTech;
		public int currentKills = 0;

		public List<Vector3> destPositions = new List<Vector3>();
		Vector3 targetPosition = posNull;


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
		
			if(SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.selectionObj))
			{
				SelectionObj = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.selectionObj);
				SelectionObj.transform.parent = transform;
				SelectionObj.transform.localPosition = Vector3.zero;
				SelectionObj.transform.localRotation = Quaternion.identity;
				SelectionObj.gameObject.SetActive(false);
			}
			else
			{
				throw new UnityEngine.UnityException("MeshHandler " + loadable.selectionObj + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);
			}

			if(SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.Loadable_Mesh))
			{
				MeshHandler hullobj = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.Loadable_Mesh);
				hullobj.transform.parent = transform;
				hullobj.transform.localPosition = Vector3.zero;
				hullobj.transform.localRotation = Quaternion.identity;
			}
			else
			{
				throw new UnityEngine.UnityException("MeshHandler " + loadable.selectionObj + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);
			}
        
			// Check Exists
			if (!SelectableLoadout.ForgeAvailable<ParticleSystem>(loadable.shieldHit))
			{
				throw new UnityEngine.UnityException("ParticleSystem " + loadable.shieldHit + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);
			}

			// Check Exists
			if (!SelectableLoadout.ForgeAvailable<ParticleSystem>(loadable.deathEffect))
			{
				throw new UnityEngine.UnityException("ParticleSystem " + loadable.deathEffect + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);
			}
        
			loadable.Loadable_Collider.AddComponent(gameObject);

			// Add Rigidbody with Y-Axis rotation only
			Rigidbody body = gameObject.AddComponent<Rigidbody>();
			body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			body.drag = 1f;
			body.angularDrag = 1f;
			body.useGravity = false;
		
			return this;
		}

		public Unit SetUnitLoadout(Loadout_Unit loading)
		{
			if (!loading.Loadout_Hull.Equals(loadable.Loadable_ID))
			{
				throw new UnityEngine.UnityException("hullID " + loadable.Loadable_ID + " does not match PlayerLoadout.UnitLoadout " + loading.Loadout_Hull);
			}
		
			loadout = loading;

			gameObject.name = loadout.Loadout_Name;
		
			int points = loadable.points;
		
			foreach (Loadout_Unit.WeaponPos wp in loadout.weapons)
			{
				if (!SelectableLoadout.ForgeAvailable<Weapon>(wp.weaponID))
				{
					throw new UnityEngine.UnityException("weaponID " + wp.weaponID + " declared but not found on PlayerLoadout.UnitLoadout " + loadout.Loadout_ID);
				}
				Weapon temp = (Weapon)SelectableLoadout.Forge<Weapon>(wp.weaponID);
			
				points -= temp.points;
				if(points < 0)
				{
					foreach(Weapon w in weapons)
					{
						Destroy(w.gameObject);
					}
					throw new UnityEngine.UnityException("Points overdraw on " + loadout.Loadout_ID);
				}
			
				temp.transform.parent = transform;
				temp.transform.localPosition = wp.position.Convert;
				temp.SetUnit(this);
			
				weapons.Add(temp);
			}

			return this;
		}
		public void Update()
		{
			UpdateAiming();

			//	//	//	Move to posiiton	//	//	//
			// If near targetPosition, null targetPosition
			if(Vector3.Distance(transform.position, targetPosition) <= StopDist)
			{
				destPositions.Remove(targetPosition);
				targetPosition = posNull;
			}

			// If no target destination exists
			if(targetPosition.Equals(posNull))
			{
				// Begin destination check by checking if there is already an input destination
				if (destPositions.Count != 0)
				{
					// Raycast check that there is no environment in the way of the next position
					// Initialise Raycast variables
					Vector3 relPos = destPositions[0] - transform.position;
					Ray ray = new Ray(transform.position, relPos);
					float relPosDist = Vector3.Distance(Vector3.zero, relPos);
					RaycastHit checkHit;
				
					int layer = 1 << LayerMask.NameToLayer("Environment");
				
					// If there is an 'Environment' object get out the way
					if (Physics.SphereCast(ray, GetComponent<CapsuleCollider>().radius * 1.1f, out checkHit, relPosDist, layer))
					{
						// In short, get avoid direction
						Vector3 toHit = checkHit.transform.position - transform.position;
						Vector3 toPoint = checkHit.point - transform.position;
						Vector3 perpNorm = Vector3.Cross(toHit, toPoint);
						Vector3 avoidDir = Vector3.Cross(perpNorm, toHit).normalized;
					
						// Get the ship size
						float avoidDist = checkHit.collider.gameObject.GetComponent<CapsuleCollider>().radius + GetComponent<CapsuleCollider>().height;
					
						// Get apply the avoid
						Vector3 avoidVec = checkHit.transform.position + avoidDir * avoidDist;
						destPositions.Insert(0, avoidVec);
					}
				
					// Move towards first pos
					targetPosition = destPositions[0];
				}
				// Else follow target
				else if (targetObj)
				{
					if(Vector3.Distance(targetObj.transform.position, transform.position) > (EngageDistance * 4f / 5f))
					{
						targetPosition = transform.position + (Vector3.Normalize(targetObj.transform.position - transform.position) * Engine);
					}
				}
			}
		
			// If there is target dest
			if (!targetPosition.Equals(posNull))
			{
				//	Get rel position
				Vector3 targetDir = targetPosition - transform.position;
			
				//	If the angle is greater then left/right (aka behind) limit to left/right
				//	This stops reverse thrust
				float engAngle = Vector3.Angle(transform.forward, targetDir);
				if (engAngle >= 90)
					engAngle = 90;
				engAngle *= Mathf.Deg2Rad;
			
				//	Move to this + (directipn * angle of thrust * engine)
				GetComponent<Rigidbody>().MovePosition(transform.position + ((targetPosition - transform.position).normalized * Mathf.Cos(engAngle) * Engine));
			
				//	Rotate to (increment towards target direction) with 'up' of world 'up'
				transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * 2, 0.0F), Vector3.up);
			}
			// Else align upwards
			else if(transform.forward.z != 0)
			{
				transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, new Vector3(transform.forward.x, 0, transform.forward.z), Time.deltaTime, 0.0F), Vector3.up);
			}
		}

		// Add a new position, or make a single new position to the list
		public override void SetMove(Vector3 Position, bool Increment)
		{
			if(Position.y > 20)
			{
				Position.y = 20;
			}
			else if(Position.y < -20)
			{
				Position.y = -20;
			}

			int layer = 1 << LayerMask.NameToLayer("Environment");
			Collider[] col = Physics.OverlapSphere (Position, loadable.Loadable_Collider.height, layer);
			foreach(Collider c in col)
			{
				Planet p = c.GetComponent<Planet>();
				if(p)
				{
					float dist = c.GetComponent<CapsuleCollider>().radius + loadable.Loadable_Collider.height;
					Vector3 dir = Position - c.transform.position;
					dir.Normalize();
					Position = c.transform.position + (dir * dist);
				}
			}

			if (Increment)
			{
				destPositions.Add(Position);
			}
			else
			{
				destPositions = new List<Vector3> { Position };
				// Due to a new list, reset targetPosition
				targetPosition = posNull;
			}
		}

		public void KilledTarget()
		{
			currentKills++;
		}

		// Apply damage to unit
		public override bool Damage(int damage, float[] armorBonus, Vector3 hitPoint)
		{
			//Setting up particle system
			Vector3 relativePos = transform.position - hitPoint;
			Quaternion rotation = Quaternion.LookRotation(relativePos);
		
			//Creating particle
			ParticleSystem hitEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(loadable.shieldHit);
			hitEffect.transform.position = hitPoint;
			hitEffect.transform.rotation = rotation;
			Destroy (hitEffect.gameObject, 1f);
		
			//Setting particle speed and particle-count as per projectile damage
			ParticleSystem.MainModule main = hitEffect.main;
			main.startSpeed = damage / 10;

			hitEffect.Emit(damage * damage);
		
			//Modify the damage as per shield for unit damaging
			float modifier = 1f - (Shield / 100) + (armorBonus[armourLevel] / 100);
			damage = Mathf.CeilToInt(damage * modifier);
		
			if (CurrentHealth > damage)
			{
				damageTaken += damage;
			}
			else if (CurrentHealth != 0)
			{
				damageTaken = Mathf.Infinity;
				OnBecameInvisible();
				Selected(false);
				Vector3 torque = (transform.position - hitPoint).normalized * damage;
				torque = new Vector3(torque.z, torque.y, torque.z);
				GetComponent<Rigidbody>().AddTorque(torque);
				EndSelf();
				return true;
			}
			return false;
		}
	
		public void SupplyBurn(int supplies)
		{
			supplyDrained += supplies;
		}

		void OnCollisionStay(Collision hit)
		{
			//	Find position of collision
			Vector3 targetDir = hit.transform.position - transform.position;
			float hitAngle = Vector3.Angle(transform.right, targetDir);

			// Determine if on Left or Right of ship
			Vector3 targetSide;
			if (hitAngle <= 180)
				targetSide = transform.right;
			else
				targetSide = transform.right * -1;

			//	Determine avoidance
			Vector3 newPos = transform.position - transform.forward;
			newPos += targetSide;

			//	Apply
			transform.position = Vector3.MoveTowards(transform.position, newPos, Engine);
		}

		public override void EndSelf()
		{
			//	Create kasplosion at position
			ParticleSystem dieEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(loadable.deathEffect);
			dieEffect.transform.position = transform.position;
			dieEffect.transform.rotation = transform.rotation;
			dieEffect.Emit(150);
			Destroy (dieEffect.gameObject, 2f);

			//	Apply EndNow to Weapons Immediately
			foreach (Weapon wp in weapons)
			{
				wp.EndNow();
			}

			base.EndSelf();
		}

	//	public class Command_Unit
	//	{
	//		public const int NULL_MOVE = 0;
	//		public const int MOVE = 1;
	//
	//		public const int NULL_ATTACK = 2;
	//		public const int ATTACK = 3;
	//
	//		public const int ATTACK_MOVE = 4;
	//		public const int PATROL = 5;
	//		public const int DEFEND_UNIT = 6;
	//		public const int DEFEND_LOC = 7;
	//
	//		//	AKA Has specific destination
	//		public bool isMove
	//			{ get { return
	//					commandID == NULL_MOVE
	//				||	commandID == MOVE
	//				||	commandID == ATTACK_MOVE
	//				||	commandID == PATROL;
	//			}}
	//
	//		//	AKA Attack AND follow
	//		public bool isAttack
	//		{ get { return
	//					commandID == NULL_ATTACK
	//				||	commandID == ATTACK
	//				||	commandID == PATROL
	//				||	commandID == ATTACK_MOVE;
	//			}}
	//
	//		//	AKA Attack in range of defend
	//		public bool isDefend
	//		{ get { return
	//					commandID == DEFEND_UNIT
	//				||	commandID == DEFEND_LOC;
	//			}}
	//
	//		public int commandID;
	//		public Vector3 position;
	//		public PlayerControlled target;
	//
	//		public Vector3 to;
	//		public Vector3 from;
	//
	//		private Command_Unit()
	//		{
	//			commandID = -1;
	//		}
	//
	//		public static Command_Unit Null_Move(Vector3 To)
	//		{
	//			Command_Unit res = new Command_Unit ();
	//			res.commandID = NULL_MOVE;
	//			res.position = To;
	//			res.to = To;
	//			return res;
	//		}
	//
	//		public static Command_Unit Move(Vector3 To)
	//		{
	//			Command_Unit res = new Command_Unit ();
	//			res.commandID = MOVE;
	//			res.position = To;
	//			res.to = To;
	//			return res;
	//		}
	//	}


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
	}
}