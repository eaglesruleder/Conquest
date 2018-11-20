using UnityEngine;
using System.Collections;

public abstract class Ability : MonoBehaviour {

    public string abilityID;
    public bool activateAbility = false;
    public int abilityDrain = 0;

    public Unit unitData = null;

    public virtual void Build(Technology PlayerTech, PlayerControlled UnitData)
    {
        if(UnitData is Unit)
        {
            unitData = UnitData as Unit;
        }
    }

    public virtual void Activate() {}
}
