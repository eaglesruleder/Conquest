using UnityEngine; 

namespace OdWyer.RTS
{
	[RequireComponent(typeof(HullBehaviour)), RequireComponent(typeof(MovementBehaviour)), RequireComponent(typeof(TargetingBehaviour))]
	public class Unit : PlayerControlled
	{
		public Unit SetHull(Loadable_Hull loadable)
		{
			gameObject.name = loadable.Loadable_Name;

			if(!SelectionObj)
			{
				if (!SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.selectionObj))
					throw new UnityException("MeshHandler " + loadable.selectionObj + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);

				SelectionObj = ((MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.selectionObj)).gameObject;
				SelectionObj.transform.parent = transform;
				SelectionObj.transform.localPosition = Vector3.zero;
				SelectionObj.transform.localRotation = Quaternion.identity;
				Selected(false);
			}

			if (!SelectableLoadout.ForgeAvailable<MeshHandler>(loadable.Loadable_Mesh))
				throw new UnityException("MeshHandler " + loadable.Loadable_Mesh + " declared but not found on Loadable_Hull " + loadable.Loadable_ID);

			if (!GetComponent<Renderer>())
			{
				MeshHandler hullobj = (MeshHandler)SelectableLoadout.Forge<MeshHandler>(loadable.Loadable_Mesh);
				hullobj.transform.parent = transform;
				hullobj.transform.localPosition = Vector3.zero;
				hullobj.transform.localRotation = Quaternion.identity;
			}

			loadable.Loadable_Collider.AddComponent(gameObject);

			Config_HullBehaviour(loadable);
			Config_TargetingBehaviour(loadable);
			Config_MovementBehaviour(loadable);

			return this;
		}

		public Unit SetUnitLoadout(Loadout_Unit loadout)
		{
			gameObject.name = loadout.Loadout_Name;

			foreach (Loadout.WeaponPos wp in loadout.weapons)
			{
				if (!SelectableLoadout.ForgeAvailable<Weapon>(wp.weaponID))
				{
					Debug.LogWarning("weaponID " + wp.weaponID + " declared but not found on PlayerLoadout.UnitLoadout " + loadout.Loadout_ID);
					continue;
				}

				Weapon temp = (Weapon)SelectableLoadout.Forge<Weapon>(wp.weaponID);

				temp.transform.parent = transform;
				temp.transform.localPosition = wp.position.Convert;
				temp.SetUnit(this);
			}

			Config_HullBehaviour(loadout);

			return this;
		}


		private HullBehaviour _hull = null;
		public HullBehaviour Hull => _hull ? _hull : (_hull = GetComponent<HullBehaviour>());

		private void Config_HullBehaviour(Loadable_Hull config)
		{
			Hull.BaseShield = config.shield;
			Hull.ShieldFromArmour = config.shieldFromArmour;

			Hull.BaseHealth = config.health;
			Hull.HealthFromArmour = config.healthFromArmour;

			Hull.BaseSupply = config.supply;
			Hull.SupplyFromSupply = config.supplyFromSupply;

			if (!Hull.ShieldEffect)
				Hull.ShieldEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(config.shieldHit);

			if (!Hull.DeathEffect)
				Hull.DeathEffect = (ParticleSystem)SelectableLoadout.Forge<ParticleSystem>(config.deathEffect);
		}

		private void Config_HullBehaviour(Loadout_Unit config)
		{
			Hull.ArmourLevel = config.armourLevel;
			Hull.SupplyLevel = config.supplyLevel;
		}


		private TargetingBehaviour _targeting = null;
		public TargetingBehaviour Targeting => _targeting ? _targeting : (_targeting = GetComponent<TargetingBehaviour>());

		private void Config_TargetingBehaviour(Loadable_Hull config)
		{
			Targeting.FactionID = PlayerManager.ThisPlayerID;
		}


		private MovementBehaviour _movement = null;
		public MovementBehaviour Movement => _movement ? _movement : (_movement = GetComponent<MovementBehaviour>());

		private void Config_MovementBehaviour(Loadable_Hull config)
		{
			Movement.Speed = config.engine / 10f;
			Movement.StopDistance = config.stopDist;
		}
	}
}