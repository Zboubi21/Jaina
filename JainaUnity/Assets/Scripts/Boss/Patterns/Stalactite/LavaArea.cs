using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaArea : Spell
{
    
    [SerializeField] float m_lavaTick = 0.25f;
    
    // 100 dégâts en 5s toute les 0.25s = 5 damage/s
    [SerializeField] int m_lavaTickDamage = 5;


    void OnTriggerEnter(Collider col)
    {
		if(col.CompareTag("Player") || col.CompareTag("Enemy")){
            // Debug.Log("OnTriggerEnter");
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();

            if(CharacterStats.LavaTick != m_lavaTick)
            {
                CharacterStats.LavaTick = m_lavaTick;
            }

            if(CharacterStats.LavaTickDamage != m_lavaTickDamage)
            {
                CharacterStats.LavaTickDamage = m_lavaTickDamage;
            }

            CharacterStats.OnCharacterEnterInLavaArea();

            // CharacterStats.TakeDamage(m_damage);
        }

	}
    void OnTriggerExit(Collider col)
    {
		if(col.CompareTag("Player") || col.CompareTag("Enemy"))
        {
            // Debug.Log("OnTriggerExit");
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();
            CharacterStats.OnCharacterExitInLavaArea();
        }
    }

}
