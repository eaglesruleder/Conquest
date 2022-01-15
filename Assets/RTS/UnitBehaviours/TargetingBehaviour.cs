using UnityEngine; 

namespace OdWyer.RTS
{
	public class TargetingBehaviour : MonoBehaviour
	{
		private IUnitValues _values = null;
		private IUnitValues Values => _values is null ? (_values = GetComponent<IUnitValues>()) : _values;

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


		public int currentKills = 0;
		public void KilledTarget() => currentKills++;


		public void SetTarget(PlayerControlled target)
		{
			targetObj = target;
			foreach (Weapon weapon in GetComponentsInChildren<Weapon>())
				weapon.SetTarget(targetObj);
		}

		private void FindTarget()
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, Values.EngageDistance);
			foreach (Collider c in colliders)
			{
				PlayerControlled pc = c.GetComponent<PlayerControlled>();
				if (pc && pc.playerID != Values.FactionID)
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
			if(!aimLock)
			{
				AimTransform.rotation = Quaternion.Slerp
					(AimTransform.rotation
					,Quaternion.LookRotation(targetAim)
					,Time.deltaTime * 2
					);

				foreach (Weapon w in GetComponentsInChildren<Weapon>())
					w.AimTarget(AimTransform.forward);
			}

			foreach (Weapon w in GetComponentsInChildren<Weapon>())
				w.Fire(targetObj && aimLock);
		}
	}
}