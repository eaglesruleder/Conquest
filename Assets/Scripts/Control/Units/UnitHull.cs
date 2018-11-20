using UnityEngine;
using System.Collections;

public class UnitHull : MonoBehaviour {

    public GameObject shieldHit;
    public GameObject selectionObj;

    public int hullLevel = 0;
    public int hullFaction = 0;

    public int[] healthFromArmour = new int[] { 0, 0, 0, 0 };
    public float hullTechInfluence;
    public Technology.TechLocation hullLoc;

    public float[] shieldFromArmour = new float[] { 0, 0, 0, 0 };
    public float shieldTechInfluence;
    public Technology.TechLocation shieldLoc;

    public float engineCapacity;
    public float engineTechInfluence;
    public Technology.TechLocation engineLoc;

    public int supplyCapacity;
    public float supplyTechInfluence;
    public Technology.TechLocation supplyLoc;

    public float sensorDistance;
    public float sensorTechInfluence;
    public Technology.TechLocation sensorLoc;
}
