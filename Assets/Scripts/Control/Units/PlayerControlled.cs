using UnityEngine;
using System.Collections;

public abstract class PlayerControlled : MonoBehaviour {

    public GUIPlayer playerData;
    public string playerID;

    public string pcName;

    public int currentHull = 0;

    public GameObject selectionObj;
    public float colliderHeight = 0f;

    public virtual void Initialise(GUIPlayer PlayerData)
    {
        playerData = PlayerData;
        playerID = playerData.playerID;
    }

    public abstract bool Damage(int damage, float[] armorBonus, Vector3 hitPoint);

    public void Selected(bool Select)
    {
        if (selectionObj)
        {
            selectionObj.SetActive(Select);
        }
    }

    public virtual void EndSelf()
    {
        if(!IsInvoking("EndNow"))
        {
            Invoke("EndNow", 1f);
        }
    }
    public virtual void EndNow()
    {
        Destroy(gameObject);
    }

    public void OnBecameInvisible()
    {
        playerData.onScreen.Remove(this);
    }

    public void OnBecameVisible()
    {
        if(currentHull > 0)
        {
            playerData.onScreen.Add(this);
        }
    }

    public abstract int ReturnMaxHealth();
    public abstract void SetTarget(PlayerControlled Target);
    public abstract void SetMove(Vector3 Destination, bool Increment);
}
