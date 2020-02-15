using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaWave : Spell
{
    
    [Header("Damages")]
    [SerializeField] float m_lavaWaveTick = 0.25f;
    
    // 100 dégâts en 5s toute les 0.25s = 5 damage/s
    [SerializeField] int m_lavaWaveTickDamage = 5;

    void OnTriggerEnter(Collider col)
    {
		if(col.CompareTag("Player")){
            // Debug.Log("OnTriggerEnter");
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();

            if(CharacterStats.LavaWaveTick != m_lavaWaveTick)
            {
                CharacterStats.LavaWaveTick = m_lavaWaveTick;
            }

            if(CharacterStats.LavaWaveTickDamage != m_lavaWaveTickDamage)
            {
                CharacterStats.LavaWaveTickDamage = m_lavaWaveTickDamage;
            }

            CharacterStats.OnCharacterEnterInLavaWave();
        }

	}
    void OnTriggerExit(Collider col)
    {
		if(col.CompareTag("Player"))
        {
            // Debug.Log("OnTriggerExit");
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();
            CharacterStats.OnCharacterExitInLavaWave();
        }
    }
    
}
