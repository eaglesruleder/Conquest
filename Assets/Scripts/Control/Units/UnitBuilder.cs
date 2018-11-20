using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitBuilder : MonoBehaviour {

    public string UnitName;
    public UnitHull hull;

    public int ArmourRating = 0;
    public int SupplyExpand = 0;
    public UnitWeapon[] unspawnedWeapons;
	public WeaponLocation[] weaponLocations;

    //DEBUG VALUE
    public GUIPlayer TEMPPLAYER;

    void Start()
    {
        PlayerControlled tempObj = Initialise();
        tempObj.transform.position = transform.position;
        tempObj.transform.rotation = transform.rotation;

        Destroy(gameObject);
    }

    public PlayerControlled Initialise()
    {
        UnitHull temphull = Instantiate(hull) as UnitHull;
        Unit result = temphull.gameObject.AddComponent<Unit>();
        result.Initialise(TEMPPLAYER, this);
        
        return result;
    }

    [System.Serializable]
    public class WeaponLocation
    {
        public UnitWeapon weapon;
        public Vector3 location;
    }
}
