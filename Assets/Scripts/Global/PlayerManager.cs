using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	static PlayerManager thisPlayer;
	
	string playerID = "player";
	public static string ThisPlayerID { get { return (thisPlayer) ? thisPlayer.playerID : ""; } }

	void Awake()
	{
		if(thisPlayer == null)
		{
			DontDestroyOnLoad(this);
			thisPlayer = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
	}
}
