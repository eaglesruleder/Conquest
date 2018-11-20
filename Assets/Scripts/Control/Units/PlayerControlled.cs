using UnityEngine;
using System.Collections;

public class PlayerControlled : MonoBehaviour {

    public Unit unitComponent;
    public Structure structureComponent;
    public GUIPlayer playerData;
    public string playerID;
    public GameObject selectionObj;
    public float selectionHeight;

	public void Initialise(Unit UnitComponent, GUIPlayer PlayerData)
    {
        unitComponent = UnitComponent;
        structureComponent = null;
        playerData = PlayerData;
        playerID = playerData.playerID;
    }

    public void Initialise(Structure StructureComponent, GUIPlayer PlayerData)
    {
        unitComponent = null;
        structureComponent = StructureComponent;
        playerData = PlayerData;
    }

    public Technology PlayerTech(int Index)
    {
        return playerData.playerTechs[Index];
    }

    public void Selected(bool Select)
    {
        selectionObj.SetActive(Select);
    }

    public void SetTarget(PlayerControlled Target)
    {
        if(unitComponent)
        {
            unitComponent.targetObj = Target;
        }
    }

    public void SetMove(Vector3 TargetPositon, bool Increment)
    {
        if(unitComponent)
        {
            unitComponent.SetMove(TargetPositon, Increment);
        }
    }

    public void KillSelf()
    {
        playerData.RemovePlayerControlled(this);

        if(unitComponent)
        {
            unitComponent.KillSelf();
        }

        Invoke("EndSelf", 1);
    }

    private void EndSelf()
    {
        Destroy(gameObject);
    }

    public void KilledTarget()
    {
        if(unitComponent)
        {
            unitComponent.Kills++;
        }
    }

    public void BecameInvisible()
    {
        playerData.onScreen.Remove(this);
    }

    public void BecameVisible()
    {
        playerData.onScreen.Add(this);
    }

    public bool Equals(PlayerControlled Compare)
    {
        if (Compare != null)
        {
            if (unitComponent && Compare.unitComponent)
            {
                return unitComponent.gameObject.Equals(Compare.unitComponent.gameObject);
            }
            if (structureComponent && Compare.structureComponent)
            {
                return structureComponent.gameObject.Equals(Compare.structureComponent.gameObject);
            }
        }
        return false;
    }
}
