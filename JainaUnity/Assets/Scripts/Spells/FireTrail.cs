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
            col.gameObject.GetComponent<CharacterStats>().FireMark(MarksTime1.Fire);
            col.gameObject.GetComponent<CharacterStats>().TakeDamage(m_damage);
        }

	}
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            // print("feu");
            other.gameObject.GetComponent<CharacterStats>().FireTrail();
        }
    }

    IEnumerator DestroyTrail(){
		yield return new WaitForSeconds(m_timeToLive);
		ObjectPoolerInstance.ReturnSpellToPool(m_spellType, gameObject);
	}

}
