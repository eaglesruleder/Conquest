using UnityEngine;
using System.Collections;

using OdWyer.Control;

public abstract class PlayerControlled : MonoBehaviour {

    // PlayerControlled values
	public abstract string Name { get; }
    public MeshHandler selectionObj;

	public abstract int health { get; }
	internal float damageTaken = 0;
	public int currentHealth{ get{return (damageTaken < health) ? health - (int)damageTaken : 0;}}

	public abstract int supply { get; }
	internal float supplyDrained = 0;
	public int currentSupply{ get{return (supplyDrained < supply) ? supply - (int)supplyDrained : 0;}}

	// Instantiated Vairables
    public string playerID;

    public virtual void SetPlayer(string PlayerID)
    {
		playerID = PlayerID;
    }

    public abstract bool Damage(int damage, float[] armorBonus, Vector3 hitPoint);

    public void Selected(bool Select)
    {
        if (selectionObj)
        {
            selectionObj.gameObject.SetActive(Select);
        }
    }

    public virtual void EndSelf()
    {
		OnBecameInvisible ();
		Selected (false);
        if(!IsInvoking("EndNow"))
        {
            Invoke("EndNow", 1f);
        }
    }
    public virtual void EndNow()
    {
        Destroy(gameObject);
    }

	// Add pc to list of OnScreen selectables via Renderer
    public void OnBecameInvisible()
    {
		PCSelector.OffScreen(this);
    }

	// Remove pc from list of OnScreen selectables via Renderer
    public void OnBecameVisible()
    {
		if(currentHealth > 0)
        {
			PCSelector.OnScreen(this);
        }
    }

    public abstract void SetTarget(PlayerControlled Target);
    public abstract void SetMove(Vector3 Destination, bool Increment);
}
