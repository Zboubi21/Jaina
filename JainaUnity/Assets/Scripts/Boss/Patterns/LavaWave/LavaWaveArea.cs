using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaWaveArea : Spell
{
    
    [Header("Damages")]
    [SerializeField] float m_lavaWaveAreaTick = 0.25f;
    
    // 100 dégâts en 5s toute les 0.25s = 5 damage/s
    [SerializeField] int m_lavaWaveAreaTickDamage = 5;

    void OnTriggerEnter(Collider col)
    {
		if(col.CompareTag("Player")){
            // Debug.Log("OnTriggerEnter");
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();

            if(CharacterStats.LavaWaveAreaTick != m_lavaWaveAreaTick)
            {
                CharacterStats.LavaWaveAreaTick = m_lavaWaveAreaTick;
            }

            if(CharacterStats.LavaWaveAreaTickDamage != m_lavaWaveAreaTickDamage)
            {
                CharacterStats.LavaWaveAreaTickDamage = m_lavaWaveAreaTickDamage;
            }

            CharacterStats.OnCharacterEnterInLavaWaveArea();
        }

	}
    void OnTriggerExit(Collider col)
    {
		if(col.CompareTag("Player"))
        {
            // Debug.Log("OnTriggerExit");
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();
            CharacterStats.OnCharacterExitInLavaWaveArea();
        }
    }
    
}
