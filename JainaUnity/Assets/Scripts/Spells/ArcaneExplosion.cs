using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneExplosion : Projectile {

	public override void OnArcanEnter(Collider col)
    {
		//base.OnArcaneProjectilesEnter(col);
		col.gameObject.GetComponent<CharacterStats>().ArcaneExplosion(Damage);
	}
	
}
