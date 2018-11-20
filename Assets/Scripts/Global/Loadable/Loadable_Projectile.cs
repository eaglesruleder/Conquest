using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class Loadable_Projectile : Loadable {

    public float life;

    [System.Serializable]
    public class LP_Projectile : Loadable_Projectile
    {
		public override System.Type Loadable_Component {get{return typeof(ProjectileProjectile);}}

        public float velocity;
    }

    [System.Serializable]
    public class LP_Laser : Loadable_Projectile
    {
		public override System.Type Loadable_Component {get{return typeof(ProjectileLaser);}}

		public Serializable_LineRenderer line;
		public bool verticalSlide;
		public float slideDist;
    }

	[System.Serializable]
	public class LP_Arc : LP_Laser
	{
		public override System.Type Loadable_Component {get{return typeof(ProjectileArc);}}

		public int bounce;
		public float range;
	}

    [System.Serializable]
    public class LP_Missile : Loadable_Projectile
    {
		public override System.Type Loadable_Component {get{return typeof(ProjectileMissile);}}

        public float velocity;
        public float turnMagnitude;
    }

    [System.Serializable]
    public class LP_Fighter : Loadable_Projectile
    {
		public override System.Type Loadable_Component {get{return typeof(ProjectileFighter);}}

        public float travelRadius;
        public float engageDistance;
        public float travelSpeed;

        public string fighterProjectileID;
        public float firerate;
    }
}
