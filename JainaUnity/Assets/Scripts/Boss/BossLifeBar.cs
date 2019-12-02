using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLifeBar : MonoBehaviour
{
    public BigEnemyLifeBarManager lifeBar;
    public EnemyStats bossStats;
    bool FightOn;
    public ParticleSystem[] flammingDoor;
    public GameObject cantBlinkAgent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !FightOn)
        {
            FightOn = true;

            for (int i = 0, l = flammingDoor.Length; i < l; ++i)
            {
                if (flammingDoor[i] != null)
                { 
                    flammingDoor[i].Play(true); 
                }
            }
            if (!cantBlinkAgent.activeSelf)
            {
                cantBlinkAgent.SetActive(true);
            }

            
        }
    }

    private void Update()
    {
        if (FightOn)
        {
            lifeBar.OnLoadBossGameObject(bossStats);
            lifeBar.OnFightBoss(true);
        }
    }
}
