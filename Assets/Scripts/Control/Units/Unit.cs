using UnityEngine; 
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public string shipclassTitle;

	public static Vector3 posNull = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
	public List<Vector3> destPositions = new List<Vector3>();
		   Vector3 targetPosition = posNull;
		   float stopDist = 2.5f;
		   float colliderRadius = 0f;
           float colliderHeight = 0f;
	
	//Build Objects
	public UnitWeapon[] Weapons = null;
	public UnitHull hull = null;

    public PlayerControlled unitData = null;
	public GUIPlayer tempGUIPlayer = null;
	public int shipFaction;
	public Technology playerTech = null;

	bool aimForward = false;
	public PlayerControlled targetObj = null;
	GameObject pitchObj = null;

    public int Kills = 0;

    void Start()
    {
        if (!GetComponent<UnitHull>())
        {
            UnitHull temphull = Instantiate(hull) as UnitHull;
            temphull.transform.position = transform.position;
            temphull.transform.rotation = transform.rotation;
            temphull.gameObject.AddComponent<Unit>();
            temphull.GetComponent<Unit>().Initialise(this, tempGUIPlayer);
            Destroy(this.gameObject);
        }
    }

    public void Initialise(Unit Parent, GUIPlayer PlayerData)
    {
        shipFaction = Parent.shipFaction;
        shipclassTitle = Parent.shipclassTitle;        

        pitchObj = new GameObject();
        pitchObj.transform.parent = transform;
        pitchObj.transform.localPosition = Vector3.zero;
        pitchObj.transform.localRotation = Quaternion.identity;

        colliderRadius = GetComponent<CapsuleCollider>().radius;
        colliderHeight = GetComponent<CapsuleCollider>().height;

        unitData = gameObject.AddComponent<PlayerControlled>();
        unitData.Initialise(this, PlayerData);
        unitData.selectionHeight = GetComponent<CapsuleCollider>().height;
        playerTech = unitData.PlayerTech(shipFaction);

        hull = GetComponent<UnitHull>();
        hull.Build(playerTech, unitData);

        WeaponLayout.WeaponLocation[] tempArray = Parent.GetComponent<WeaponLayout>().weaponLocations;
        Weapons = new UnitWeapon[tempArray.Length];
        for (int i = 0; i < tempArray.Length; i++)
        {
            UnitWeapon temp = Instantiate(tempArray[i].weapon) as UnitWeapon;
            temp.transform.parent = gameObject.transform;
            temp.transform.localPosition = tempArray[i].location;
            Weapons[i] = temp;
        }
        foreach (UnitWeapon weapon in Weapons)
        {
            weapon.Build(playerTech, unitData);
        }
    }

	void Update()
		{
		AimTurrets();

		MoveToPosition();
		}

    public void SetTarget(PlayerControlled Target)
    {
        targetObj = Target;
    }

	void AimTurrets()
		{
		if(!aimForward || targetObj)
			{
            aimForward = Vector3.Angle(pitchObj.transform.forward, transform.forward) < 2.5f;

            Vector3 targetAim = Vector3.zero;
			bool spin = false;
			bool fire = false;
			
            targetAim = (targetObj) ? targetObj.transform.position - transform.position : transform.forward;
			
			Vector3 pitchDir = Vector3.RotateTowards(pitchObj.transform.forward, targetAim, Time.deltaTime * 2, 0.0F);
			pitchObj.transform.rotation = Quaternion.LookRotation(pitchDir, pitchObj.transform.up);
			Vector3 yawDir = new Vector3(0, pitchObj.transform.localEulerAngles.y, 0);
			
			if(Vector3.Angle(pitchObj.transform.forward, targetAim) < 5)
				{
				if(targetObj)
				{
					spin = true;
					fire = true;
				}
				else
					aimForward = true;
				}
			foreach(UnitWeapon weapon in Weapons)
				{
				weapon.Target(yawDir, pitchDir, fire, spin, targetObj);
				}
			}
		}
	
	// Add a new position, or make a sinlge new position to the list
	public void SetMove(Vector3 Position, bool Increment)
		{
		if(Increment)
			{
			destPositions.Add(Position);
			}
		else
			{
			destPositions = new List<Vector3>{Position};
			// Due to a new list, reset targetPosition
			targetPosition = posNull;
			}
		}

	void MoveToPosition()
	{
		// Initialise the move-to dest
		Vector3 tempDest = posNull;

		// If near targetPosition, null targetPosition
		if(Vector3.Distance(hull.transform.position, targetPosition) <= stopDist)
		{
			destPositions.Remove(targetPosition);
			targetPosition = posNull;
		}
		
		// Begin destination check by checking if there is already an input destination
		if(targetPosition.Equals(posNull) && destPositions.Count != 0)
		{
			// Raycast check that there is no environment in the way of the next position
			// Initialise Raycast variables
			Vector3 relPos = destPositions[0] - transform.position;
			Ray ray = new Ray(transform.position, relPos);
			float relPosDist = Vector3.Distance(Vector3.zero, relPos);
			RaycastHit checkHit;

            int layer = 1 << LayerMask.NameToLayer("Environment");
			
			// If there is a Planet object
            if(Physics.SphereCast(ray, colliderRadius * 1.1f, out checkHit, relPosDist, layer))
			{
                Vector3 toHit = checkHit.transform.position - transform.position;
                Vector3 toPoint = checkHit.point - transform.position;
                Vector3 perpNorm = Vector3.Cross(toHit, toPoint);
                Vector3 avoidDir = Vector3.Cross(perpNorm, toHit).normalized;
                float avoidDist = checkHit.collider.gameObject.GetComponent<SphereCollider>().radius + colliderHeight;
                Vector3 avoidVec = checkHit.transform.position + avoidDir * avoidDist;
				destPositions.Insert(0, avoidVec);
			}
			
			targetPosition = destPositions[0];
			tempDest = destPositions[0];
		}
		// Else if a target position exists
		else if (!targetPosition.Equals(posNull))
		{
			tempDest = targetPosition;
		}
		// Else find if there is a target nearby
		else
		{
			// PLACEHOLDER
			// Find nearby targets
		}

        hull.moveUnit(targetPosition);
	}

    public void KillSelf()
    {
        foreach(UnitWeapon wp in Weapons)
        {
            wp.Kill();
        }
    }
}
