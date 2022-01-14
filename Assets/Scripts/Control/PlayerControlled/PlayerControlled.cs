using UnityEngine;

using OdWyer.Control;

public abstract class PlayerControlled : MonoBehaviour {

	public abstract string Name { get; }
    public MeshHandler SelectionObj;

	public abstract int Health { get; }
	internal float damageTaken = 0;
	public int CurrentHealth => (damageTaken < Health) ? Health - (int)damageTaken : 0;

	public abstract int Supply { get; }
	internal float supplyDrained = 0;
	public int CurrentSupply{ get{return (supplyDrained < Supply) ? Supply - (int)supplyDrained : 0;}}

	// Instantiated Vairables
    public string playerID;

    public virtual void SetPlayer(string PlayerID)
    {
		playerID = PlayerID;
    }

    public abstract bool Damage(int damage, float[] armorBonus, Vector3 hitPoint);

    public void Selected(bool Select)
    {
        if (SelectionObj)
        {
            SelectionObj.gameObject.SetActive(Select);
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
		if(CurrentHealth > 0)
        {
			PCSelector.OnScreen(this);
        }
    }

    public abstract void SetTarget(PlayerControlled Target);
    public abstract void SetMove(Vector3 Destination, bool Increment);
}
