using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StalactiteStateEnum;

public class FireTrail : Spell {

	[SerializeField] int m_damage = 10;
	[SerializeField] float m_timeToLive = 1;

	void OnEnable(){
		StartCoroutine(DestroyTrail());
	}

	void OnTriggerEnter(Collider col){

		// Le tir du player touche un enemy
		if(col.CompareTag("Enemy"))
        {
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();
            
            CharacterStats.FireMark(MarksTime1.Fire);
            CharacterStats.TakeDamage(m_damage);

            
            CharacterStats.StartHitFxCorout();
        }
        if (col.CompareTag("Stalactite"))
        {
            StalactiteController controller = col.gameObject.GetComponent<StalactiteController>();
            StalactiteStats stats = col.gameObject.GetComponent<StalactiteStats>();
            stats.FireMark(MarksTime1.Fire);

            if (controller != null && stats != null && controller.StalactiteState == StalactiteState.Fusion)
            {
                stats.TakeDamage(m_damage * stats.fireDamageMutliplicater);
            }
            else
            {
                stats.TakeDamage(m_damage);
            }
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
        if (col.CompareTag("Stalactite"))
        {
            StalactiteController controller = col.gameObject.GetComponent<StalactiteController>();
            StalactiteStats stats = col.gameObject.GetComponent<StalactiteStats>();
            stats.FireTrail();
        }
    }

    IEnumerator DestroyTrail(){
		yield return new WaitForSeconds(m_timeToLive);
		ObjectPoolerInstance.ReturnSpellToPool(m_spellType, gameObject);
	}

}
