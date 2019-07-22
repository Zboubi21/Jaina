using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrail : Spell {

	[SerializeField] int m_damage = 10;
	[SerializeField] float m_timeToLive = 1;

	void OnEnable(){
		StartCoroutine(DestroyTrail());
	}

	void OnTriggerEnter(Collider col){

		// Le tir du player touche un enemy
		if(col.CompareTag("Enemy")){
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();
            CharacterStats.FireMark(MarksTime1.Fire);
            CharacterStats.TakeDamage(m_damage);
            CharacterStats.StartHitFxCorout();
        }

	}
    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();
            CharacterStats.FireTrail();
            CharacterStats.StartHitFxCorout();
        }
    }

    IEnumerator DestroyTrail(){
		yield return new WaitForSeconds(m_timeToLive);
		ObjectPoolerInstance.ReturnSpellToPool(m_spellType, gameObject);
	}

}
