using UnityEngine;

using OdWyer.RTS;

public class Spawn : MonoBehaviour {

	public bool isUnit = true;
	public bool isPlanet = false;
	public string PlayerLoadoutID;

	// Use this for initialization
	void Start () 
	{
		if(isUnit)
		{
			Unit result = (Unit)SelectableLoadout.Forge<Unit> (PlayerLoadoutID);
			result.PlayerID = PlayerManager.ThisPlayerID;
			result.transform.position = transform.position;
			result.transform.rotation = transform.rotation;
		}
		else if (isPlanet)
		{
			Planet result = (Planet)SelectableLoadout.Forge<Planet> (PlayerLoadoutID);
			result.transform.position = transform.position;
			result.transform.rotation = transform.rotation;
		}
		Destroy (gameObject);
	}
}
