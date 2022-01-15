using UnityEngine; 

namespace OdWyer.RTS
{
	[RequireComponent(typeof(Rigidbody))]
	public class HullBehaviour : MonoBehaviour
	{
		private IUnitValues _values = null;
		private IUnitValues Values => _values is null ? (_values = GetComponent<IUnitValues>()) : _values;


		private Rigidbody _rigidBody = null;
		private Rigidbody Rigidbody => _rigidBody ? _rigidBody : (_rigidBody = GetComponent<Rigidbody>());


		internal float damageTaken = 0;
		public int CurrentHealth => (damageTaken < Values.MaxHealth) ? Values.MaxHealth - (int)damageTaken : 0;


		internal float supplyDrained = 0;
		public int CurrentSupply => (supplyDrained < Values.MaxSupply) ? Values.MaxSupply - (int)supplyDrained : 0;

		public bool Damage(int damage, float[] armorBonus, Vector3 hitPoint)
		{
			ParticleSystem hitEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(Values.ShieldEffectID);
			hitEffect.transform.position = hitPoint;
			hitEffect.transform.rotation = Quaternion.LookRotation(transform.position - hitPoint);
			Destroy (hitEffect.gameObject, 1f);
		
			ParticleSystem.MainModule main = hitEffect.main;
			main.startSpeed = damage / 10;

			hitEffect.Emit(damage * damage);
		
			damage = Mathf.CeilToInt(damage * Values.Shield * armorBonus[Values.Armour]);

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

		public void EndSelf()
		{
			ParticleSystem dieEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(Values.DeathEffectID);
			dieEffect.transform.position = transform.position;
			dieEffect.transform.rotation = transform.rotation;
			dieEffect.Emit(150);
			Destroy (dieEffect.gameObject, 2f);

			foreach (IUnitLifecycle lifecycle in GetComponentsInChildren<IUnitLifecycle>())
				lifecycle.BeforeDestroy();
		}
	}
}