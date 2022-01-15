using System.Collections.Generic;

using UnityEngine; 

namespace OdWyer.RTS
{
	public class TargetingBehaviour : MonoBehaviour
	{
		private Unit _unit = null;
		public Unit Unit => _unit ? _unit : (_unit = GetComponent<Unit>());

		public List<Weapon> weapons => Unit.weapons;


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


		public void SetTarget(PlayerControlled target)
		{
			targetObj = target;
			foreach (Weapon weapon in weapons)
				weapon.SetTarget(targetObj);
		}

		private void FindTarget()
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, Unit.EngageDistance);
			foreach (Collider c in colliders)
			{
				PlayerControlled pc = c.GetComponent<PlayerControlled>();
				if (pc && pc.playerID != Unit.playerID)
				{
					SetTarget(pc);
					return;
				}
			}
		}

		public void Update()
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
	}
}