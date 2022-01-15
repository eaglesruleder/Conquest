﻿using System.Collections.Generic;
using System.Linq;

using UnityEngine; 

namespace OdWyer.RTS
{
	[RequireComponent(typeof(Rigidbody))]
	public class MovementBehaviour : MonoBehaviour
	{
		private Unit _unit = null;
		public Unit Unit => _unit ? _unit : (_unit = GetComponent<Unit>());

		private TargetingBehaviour _targeting = null;
		private TargetingBehaviour Targeting => _targeting ? _targeting : (_targeting = GetComponent<TargetingBehaviour>());

		private Rigidbody _rigidBody = null;
		private Rigidbody Rigidbody => _rigidBody ? _rigidBody : (_rigidBody = GetComponent<Rigidbody>());


		public Queue<Vector3> destPositions = new Queue<Vector3>();
		Vector3? targetPosition = null;

		public void Awake()
		{
			Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			Rigidbody.drag = 1f;
			Rigidbody.angularDrag = 1f;
			Rigidbody.useGravity = false;
		}


		public void AddCollisionTorque(Vector3 hitPoint, float force)
		{
			Vector3 torque = (transform.position - hitPoint).normalized * force;
			torque.x = torque.z;
			Rigidbody.AddTorque(torque);
		}


		public void SetMove(Vector3 position, bool force = false)
		{
			position.y = Mathf.Clamp(position.y, -20, 20);
			Collider[] col = Physics.OverlapSphere (position, Unit.loadable.Loadable_Collider.height, 1 << LayerMask.NameToLayer("Environment"));
			foreach(Collider c in col)
			{
				Planet p = c.GetComponent<Planet>();
				if (!p)
					continue;
				
				float dist = c.GetComponent<CapsuleCollider>().radius + Unit.loadable.Loadable_Collider.height;
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

			if (Targeting && Targeting.targetObj
			&&	Vector3.Distance(Targeting.targetObj.transform.position, transform.position) > (Unit.EngageDistance * 0.8f)
				)
				targetPosition = Targeting.targetObj.transform.position;
		}


		public void Update()
		{
			if (targetPosition.HasValue
			&&	Vector3.Distance(transform.position, targetPosition.Value) <= Unit.StopDist
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

			Rigidbody.MovePosition(transform.position + (targetDir * Mathf.Cos(engAngle) * Unit.Engine));
			transform.rotation = Quaternion.Slerp
				(transform.rotation
				,Quaternion.LookRotation(targetDir)
				,Time.deltaTime * 2
				);
		}

		void OnCollisionStay(Collision hit)
		{
			Vector3 targetDir = hit.transform.position - transform.position;

			Vector3 targetSide = transform.right;
			if (Vector3.Angle(transform.right, targetDir) > 180)
				targetSide *= -1;

			Vector3 newPos = transform.position - transform.forward;
			transform.position = Vector3.MoveTowards(transform.position, newPos + targetSide, Unit.Engine);
		}
	}
}