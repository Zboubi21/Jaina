﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceNova : Spell {

	[SerializeField] int m_damage = 10;

	[SerializeField] float m_timeToLive = 1;

	void OnEnable(){
		StartCoroutine(DestroyNova());
	}

	void OnTriggerEnter(Collider col){

		// Le tir du player touche un enemy
		if(col.CompareTag("Enemy")){
			CharacterStats = col.gameObject.GetComponent<CharacterStats>();
            CharacterStats.IceMark(MarksTime1.Ice);
            CharacterStats.TakeDamage(m_damage);
            CharacterStats.StartHitFxCorout();
            col.gameObject.GetComponent<EnemyController>().Freeze();
        }

		if(col.CompareTag("Stalactite")){
			col.GetComponent<StalactiteController>().RemoveStalactiteState();
		}		
	}

	IEnumerator DestroyNova(){
		yield return new WaitForSeconds(m_timeToLive);
		ObjectPoolerInstance.ReturnSpellToPool(m_spellType, gameObject);
	}

}
