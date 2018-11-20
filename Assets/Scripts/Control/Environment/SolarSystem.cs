using UnityEngine;
using System.Collections;

public class SolarSystem : MonoBehaviour {

	public int solarID;

	public int solarRadius;

	public Planet[] solarPlanets;

	public Planet solarSun;

	void Start()
		{
		solarSun = Instantiate(solarSun) as Planet;
		solarSun.transform.parent = gameObject.transform;
		solarSun.transform.localPosition = Vector3.zero;
		}
}
