using UnityEngine;
using System.Collections;

public abstract class Ability : UnitWeapon {

    public bool onActivateEvent = false;

    public virtual void Activate() { }
}
