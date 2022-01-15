using System.Collections.Generic;
using System.Linq;

using UnityEngine; 

namespace OdWyer.RTS
{
	public class TargetingBehaviour : MonoBehaviour
	{
		public string FactionID = null;


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


		public IEnumerable<Weapon> Weapons => GetComponentsInChildren<Weapon>();
		public float DamPerSec => Weapons.Sum(w => w.fireRate * w.volley * w.weaponDamage);
		public float SupPerSec => Weapons.Sum(w => w.fireRate * w.volley * w.supplyDrain);

		public float EngageDistance => Weapons.Max(w => w.engageDistance);


		public int currentKills = 0;
		public void KilledTarget() => currentKills++;


		public void SetTarget(PlayerControlled target)
		{
			targetObj = target;
			foreach (Weapon weapon in Weapons)
				weapon.SetTarget(targetObj);
		}

		private void FindTarget()
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, EngageDistance);
			foreach (Collider c in colliders)
			{
				PlayerControlled unit = c.GetComponent<PlayerControlled>();
				TargetingBehaviour targeting = c.GetComponent<TargetingBehaviour>();
				if (unit && targeting && targeting.FactionID != FactionID)
				{
					SetTarget(unit);
					return;
				}
			}
		}

		public Vector3? GetTargetPosition()
		{
			if (!targetObj)
				return null;

			if (Vector3.Distance(targetObj.transform.position, transform.position) > EngageDistance)
				return targetObj.transform.position;

			return null;
		}

		public void Update()
		{
			if(!targetObj)
				FindTarget();

			Vector3 targetAim = transform.forward;
			if (targetObj)
				targetAim = (targetObj.transform.position - transform.position).normalized;

			bool aimLock = Vector3.Angle(AimTransform.forward, targetAim) < 5;
			if(!aimLock)
			{
				AimTransform.rotation = Quaternion.Slerp
					(AimTransform.rotation
					,Quaternion.LookRotation(targetAim)
					,Time.deltaTime * 2
					);

				foreach (Weapon weapon in Weapons)
					weapon.AimTarget(AimTransform.forward);
			}

			foreach (Weapon weapons in Weapons)
				weapons.Fire(targetObj && aimLock);
		}
	}
}