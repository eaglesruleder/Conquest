using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Projectile : MonoBehaviour {

	//All of the presets to be edited
	public float life;
	public int damage;
    public float[] armourBonus;

    //All values to be manipulated internally, or by script-input
    public PlayerControlled target;
    public string targetPlayerID;
    public Unit launcher;
    public string launcherPlayerID;

	public virtual void Build(PlayerControlled Target, Unit Launcher, Vector3 FireDirection, int Damage, float[] ArmourBonus)
		{
		target = Target;
        
        targetPlayerID = (target) ? target.playerID : null;
		launcher = Launcher;
        launcherPlayerID = launcher.playerID;

		damage = Damage;
        armourBonus = ArmourBonus;

        Invoke("EndNow", life);
		}

	public virtual void EndNow()
		{
		Destroy(gameObject);
		}

    public virtual void OnTriggerEnter(Collider hit)
    {
        PlayerControlled hitObj = hit.GetComponent<PlayerControlled>();
        if (hitObj)
        {
            if ((hitObj.Equals(target)) ? true : (targetPlayerID.Equals(hitObj.playerID) && !launcherPlayerID.Equals(hitObj.playerID)))
            {
                if (hitObj.Damage(damage, armourBonus, transform.position))
                {
                    launcher.KilledTarget();
                }
                EndNow();
            }
        }
    }
}
