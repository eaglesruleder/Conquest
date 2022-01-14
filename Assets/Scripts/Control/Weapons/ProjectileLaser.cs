using UnityEngine;
using System.Collections;

public class ProjectileLaser : Projectile {

    //	ProjectileLaser Values
	Loadable_Projectile.LP_Laser loadableLaser
		{get{return (Loadable_Projectile.LP_Laser)loadable;}}
	internal bool verticalSlide { get { return loadableLaser.verticalSlide; } }
	internal float slideDist { get { return loadableLaser.slideDist; } }


	//	Local variables
	internal LineRenderer drawEffect;
	internal float currentWidth;
	internal Vector3 slide = Vector3.zero;
	internal Vector3 slideShift = Vector3.zero;
	
	public override Projectile SetProjectile(Loadable_Projectile loading)
	{
		base.SetProjectile(loading);

		//	Add a line renderer
		drawEffect = loadableLaser.line.AddComponent(gameObject);
		drawEffect.useWorldSpace = true;

		drawEffect.positionCount = 2;

		return this;
	}

    public override void Initialise(PlayerControlled Target, PlayerControlled Launcher, int Damage, float[] ArmourBonus)
    {
        base.Initialise(Target, Launcher, Damage, ArmourBonus);

		//	Determine slide direction
		slide = Vector3.Cross(target.transform.position - transform.position, (verticalSlide) ? transform.right : transform.up);
		slide.Normalize();
		slide *= slideDist;

		//	Set the Launcher as the Parent (This can be overridden after the initialise)
		transform.parent = launcher.transform;

		//	Reset and apply starting width
		currentWidth = loadableLaser.line.width;

		drawEffect.startWidth = currentWidth;
		drawEffect.endWidth = currentWidth;

		//	Reset and apply starting slide
		slideShift = (slide / 2) * -1;
		drawEffect.SetPosition(0, transform.position);
		drawEffect.SetPosition(1, target.transform.position + slideShift);

		//	Determine damage position
        Vector3 collisionPosition = target.transform.position;
        Collider targetCollider = target.GetComponent<Collider>();
		Ray ray = new Ray(transform.position, target.transform.position - transform.position);
		float dist = Vector3.Distance(target.transform.position, transform.position);
        RaycastHit rayOut;
        if(targetCollider.Raycast(ray, out rayOut, dist))
        {
            collisionPosition = rayOut.point;
        }

		//	Hit the target (instant)
        target.Damage(damage, ArmourBonus, collisionPosition);
    }

    public override void EndNow()
    {
        currentWidth = 0f;
        base.EndNow();
    }

    void Update()
    {
		//	Increment and apply width
		currentWidth -= currentWidth * Time.deltaTime / life;

		drawEffect.startWidth = currentWidth;
		drawEffect.endWidth = currentWidth;


		//	Increment and apply slide
		slideShift += slide * Time.deltaTime / life;
		drawEffect.SetPosition(0, transform.position);
		if(target)
		{
			drawEffect.SetPosition(1, target.transform.position + slideShift);
		}
    }

	//	Override OnTriggerEnter to do nothing just in case
    public override void OnTriggerEnter(Collider hit)
    {}
}
