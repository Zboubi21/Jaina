using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneProjectile : Projectile {

	[SerializeField] int m_arcaneMarkNb = 1;

	public override void OnArcanEnter(Collider col)
    {
		//base.OnArcaneProjectilesEnter(col);
		col.gameObject.GetComponent<CharacterStats>().ArcanMark(Damage, MarksTime1.Arcane, m_arcaneMarkNb);
	}
	
}
