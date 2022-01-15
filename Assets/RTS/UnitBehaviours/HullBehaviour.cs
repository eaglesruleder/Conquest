using UnityEngine; 

namespace OdWyer.RTS
{
	[RequireComponent(typeof(Rigidbody))]
	public class HullBehaviour : MonoBehaviour
	{
		private Rigidbody _rigidBody = null;
		private Rigidbody Rigidbody => _rigidBody ? _rigidBody : (_rigidBody = GetComponent<Rigidbody>());

		public int BaseHealth = 0;
		public int HealthFromArmour = 0;
		public int ArmourLevel = 0;
		public int MaxHealth => Mathf.CeilToInt(BaseHealth + (HealthFromArmour * ArmourLevel));

		internal float damageTaken = 0;
		public int CurrentHealth => (damageTaken < MaxHealth) ? MaxHealth - (int)damageTaken : 0;


		public int BaseSupply = 0;
		public int SupplyFromSupply = 0;
		public int SupplyLevel = 0;
		public int MaxSupply => Mathf.CeilToInt(BaseSupply + (SupplyFromSupply * SupplyLevel));

		internal float supplyDrained = 0;
		public int CurrentSupply => (supplyDrained < MaxSupply) ? MaxSupply - (int)supplyDrained : 0;

		public bool Damage(int damage, float[] armorBonus, Vector3 hitPoint)
		{
			ParticleSystem hitEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(Values.ShieldEffectID);
			hitEffect.transform.position = hitPoint;
			hitEffect.transform.rotation = Quaternion.LookRotation(transform.position - hitPoint);
			Destroy (hitEffect.gameObject, 1f);
		
			ParticleSystem.MainModule main = hitEffect.main;
			main.startSpeed = damage / 10;

			hitEffect.Emit(damage * damage);
		
			damage = Mathf.CeilToInt(damage * Values.Shield * armorBonus[ArmourLevel]);

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