using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Projectile : MonoBehaviour {

	//All of the presets to be edited
	public float life;
	public float damage;

    //All values to be manipulated internally, or by script-input
    public PlayerControlled target;
    public string targetPlayerID;
    public PlayerControlled launcher;
    public string launcherPlayerID;
	public Vector3 fireDirection;

	public void Build(PlayerControlled Target, PlayerControlled Launcher, Vector3 FireDirection, int Damage)
		{
		target = Target;
        
        targetPlayerID = (target) ? target.playerID : null;
		launcher = Launcher;
        launcherPlayerID = launcher.playerID;

		fireDirection = FireDirection;
		damage = Damage;

		BuildExtend();

		Invoke("Kill", life);
		}

	//To be overwritten when required in projectile subclasses
	public abstract void BuildExtend();

	void Update() {StepUpdate();}
	public abstract void StepUpdate();

	public void Kill()
		{
		Destroy(gameObject);
		}
}
