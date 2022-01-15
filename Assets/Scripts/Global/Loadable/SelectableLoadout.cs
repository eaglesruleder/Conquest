using UnityEngine;
using System.Collections.Generic;

using OdWyer.RTS;

public class SelectableLoadout : MonoBehaviour {

    static SelectableLoadout selectableLoadout;

    string version;

    Dictionary<string, MeshHandler> meshes;
    Dictionary<string, ParticleSystem> particleSystems;

	Dictionary<string, Loadable_Environment> environment;
	
    Dictionary<string, Loadable_Hull> hulls;
    Dictionary<string, Loadable_Weapon> weapons;

    Dictionary<string, Loadable_Projectile> projectiles;
    Dictionary<string, List<PoolSlot>> pool;

	Dictionary<string, Loadout_Unit> playerLoadouts;
	
    void Awake()
    {
		if(GetComponent<Binary_Generator>())
		{
			GetComponent<Binary_Generator>().SaveAll();
		}

        if(selectableLoadout == null)
        {
            DontDestroyOnLoad(this);
            selectableLoadout = this;

			LoadRecources();

			LoadEnvironment();

			LoadSelectable();

			LoadPlayer();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static bool ForgeAvailable<T>(string ID)
    {
		if (typeof(T) == typeof(Unit))
		{
			if(selectableLoadout.playerLoadouts.ContainsKey(ID))
			{
				Loadout_Unit search;
				selectableLoadout.playerLoadouts.TryGetValue(ID, out search);
				return selectableLoadout.hulls.ContainsKey(search.Loadout_Hull);
			}
		}
        else if (typeof(T) == typeof(Weapon))
        {
            return selectableLoadout.weapons.ContainsKey(ID);
        }
        else if (typeof(T) == typeof(Projectile))
        {
            return selectableLoadout.projectiles.ContainsKey(ID);
        }
        else if (typeof(T) == typeof(MeshHandler))
        {
            return selectableLoadout.meshes.ContainsKey(ID);
        }
        else if (typeof(T) == typeof(ParticleSystem))
        {
            return selectableLoadout.particleSystems.ContainsKey(ID);
        }
		else if (typeof(T) == typeof(Planet) || typeof(T) == typeof(Recource))
		{
			return selectableLoadout.environment.ContainsKey(ID);
		}

        return false;
    }

    public static Object Forge<T>(string ID)
    {
        if (typeof(T) == typeof(Unit) && selectableLoadout.playerLoadouts.ContainsKey(ID))
        {
			Loadout_Unit search;
			selectableLoadout.playerLoadouts.TryGetValue(ID, out search);

            Loadable_Hull hullSearch;
			selectableLoadout.hulls.TryGetValue(search.Loadout_Hull, out hullSearch);

			return ((Unit) new GameObject().AddComponent(hullSearch.Loadable_Component)).SetHull(hullSearch).SetUnitLoadout(search);
        }
        else if (typeof(T) == typeof(Weapon) && selectableLoadout.weapons.ContainsKey(ID))
        {
            Loadable_Weapon search;
            selectableLoadout.weapons.TryGetValue(ID, out search);
			return ((Weapon)new GameObject().AddComponent(search.Loadable_Component)).SetWeapon(search);
        }
		else if (typeof(T) == typeof(Planet) || typeof(T) == typeof(Recource))
		{
			Loadable_Environment search;
			selectableLoadout.environment.TryGetValue(ID, out search);
			if(typeof(T) == typeof(Planet) && search is Loadable_Environment.LE_Planet)
			{
				return((Planet)new GameObject().AddComponent(search.Loadable_Component)).SetPlanet((Loadable_Environment.LE_Planet)search);
			}
			else
			{
				throw new UnityEngine.UnityException("Attempted to forge environment " + ID);
			}
		}
        else if (typeof(T) == typeof(MeshHandler) && selectableLoadout.meshes.ContainsKey(ID))
        {
            MeshHandler search;
            selectableLoadout.meshes.TryGetValue(ID, out search);
            return Instantiate<MeshHandler>(search) as MeshHandler;
        }
        else if (typeof(T) == typeof(ParticleSystem) && selectableLoadout.particleSystems.ContainsKey(ID))
        {
            ParticleSystem search;
            selectableLoadout.particleSystems.TryGetValue(ID, out search);
            return Instantiate<ParticleSystem>(search) as ParticleSystem;
        }
        else if (typeof(T) == typeof(Projectile) && selectableLoadout.projectiles.ContainsKey(ID))
        {
            List<PoolSlot> pooled;
            if (selectableLoadout.pool.TryGetValue(ID, out pooled))
            {
                foreach (PoolSlot p in pooled)
                {
                    if (!p.stored.gameObject.activeSelf)
                    {
                        p.stored.gameObject.SetActive(true);
                        return p.stored;
                    }
                }
            }
            else
            {
                pooled = new List<PoolSlot>();
                selectableLoadout.pool.Add(ID, pooled);
            }

            Loadable_Projectile search;
            selectableLoadout.projectiles.TryGetValue(ID, out search);
			Projectile result = ((Projectile) new GameObject().AddComponent(search.Loadable_Component)).SetProjectile(search);
            pooled.Add(new PoolSlot(result));
            return result;
        }
        else
        {
            throw new UnityEngine.UnityException("Attempted to forge unreachachable " + ID);
        }
    }

    void LoadRecources()
    {
        meshes = new Dictionary<string, MeshHandler>();
        particleSystems = new Dictionary<string, ParticleSystem>();
        projectiles = new Dictionary<string, Loadable_Projectile>();
        hulls = new Dictionary<string, Loadable_Hull>();
        weapons = new Dictionary<string, Loadable_Weapon>();
        pool = new Dictionary<string, List<PoolSlot>>();
		playerLoadouts = new Dictionary<string, Loadout_Unit> ();
		environment = new Dictionary<string, Loadable_Environment> ();

        ParticleSystem[] res_particles = Resources.LoadAll<ParticleSystem>("");
        foreach(ParticleSystem ps in res_particles)
        {
            particleSystems.Add(ps.name, ps);
        }

        MeshHandler[] res_meshes = Resources.LoadAll<MeshHandler>("");
        foreach (MeshHandler m in res_meshes)
        {
            meshes.Add(m.name, m);
        }
    }

	void LoadEnvironment()
	{
		environment.Clear ();
		foreach(Loadable_Environment le in Native.Environment.All)
		{
			environment.Add(le.Loadable_ID, le);
		}

		if(Binary_Environment.Exists("update"))
		{
			Binary_Environment parse = Binary_Environment.Load ("update");
			
			foreach(Loadable_Environment le in parse.Loadable_Enviroments)
			{
				//	Accept overwrites from update
				if(environment.ContainsKey(le.Loadable_ID))
				{
					environment[le.Loadable_ID] = le;
				}
				else
				{
					environment.Add(le.Loadable_ID, le);
				}
			}
		}
	}
	
	void LoadSelectable()
    {    
		hulls.Clear ();
		foreach(Loadable_Hull LH in Native.Hulls.All)
		{
			hulls.Add(LH.Loadable_ID, LH);
		}

		projectiles.Clear ();
		foreach(Loadable_Projectile LP in Native.Projectiles.All)
		{
			projectiles.Add(LP.Loadable_ID, LP);
		}

		weapons.Clear ();
		foreach(Loadable_Weapon LW in Native.Weapons.All)
		{
			weapons.Add(LW.Loadable_ID, LW);
		}

		if(Binary_SelectableLoadout.Exists("update"))
		{
			Binary_SelectableLoadout parse = Binary_SelectableLoadout.Load ("update");

			foreach(Loadable_Hull LH in parse.Loadable_Hulls)
	        {
				if(hulls.ContainsKey(LH.Loadable_ID))
				{
					hulls[LH.Loadable_ID] = LH;
				}
				else
				{
	            	hulls.Add(LH.Loadable_ID, LH);
				}
	        }

			foreach(Loadable_Projectile LP in parse.Loadable_Projectiles)
	        {
				if(projectiles.ContainsKey(LP.Loadable_ID))
				{
					projectiles[LP.Loadable_ID] = LP;
				}
				else
				{
	            	projectiles.Add(LP.Loadable_ID, LP);
				}
	        }

			foreach (Loadable_Weapon LW in parse.Loadable_Weapons)
	        {
				if(weapons.ContainsKey(LW.Loadable_ID))
				{
					weapons[LW.Loadable_ID] = LW;
				}
				else
				{
					weapons.Add(LW.Loadable_ID, LW);
				}
	        }
		}
    }

	void LoadPlayer()
	{
		foreach(Loadout_Unit ul in Native.Loadouts.All)
		{
			playerLoadouts.Add(ul.Loadout_ID, ul);
		}

		if(Binary_PlayerLoadout.Exists(PlayerManager.ThisPlayerID))
		{
			Binary_PlayerLoadout parse = Binary_PlayerLoadout.Load(PlayerManager.ThisPlayerID);

			if(!parse.version.Equals(version))
			{
				throw new UnityEngine.UnityException("PlayerLoadout Version does not support current SelectableLoadout Version");
			}

			foreach(Loadout_Unit ul in parse.playerLoadout)
			{
				playerLoadouts.Add(PlayerManager.ThisPlayerID + ul.Loadout_ID, ul);
			}
		}
	}

	class PoolSlot
	{
		public PoolSlot(Projectile toStore)
		{
			stored = toStore;
			timeUsed = Time.time;
		}
		
		public Projectile stored;
		public float timeUsed;
	}
}
