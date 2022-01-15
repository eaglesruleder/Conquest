using OdWyer.RTS;

[System.Serializable]
public class Loadable_Hull : Loadable
{
	public override System.Type Loadable_Component { get{return typeof(Unit);}}

    public string selectionObj;
    public string deathEffect;
	
    public int health;
    public int healthFromArmour;

    public string shieldHit;
    public float shield;
    public float shieldFromArmour;

    public float stopDist = 2.5f;
    public float engine;

    public int supply;
    public int supplyFromSupply;

    public float sensor;

    public int points;
}
