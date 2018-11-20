using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

	public int life = 1;

	// Use this for initialization
	void Start ()
		{
		Invoke("Kill", life);
		}
	
	public void Kill()
		{
		Destroy(gameObject);
		}
}
