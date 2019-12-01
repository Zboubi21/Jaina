using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLifeBar : MonoBehaviour
{
    public BigEnemyLifeBarManager lifeBar;

    void Start()
    {
        
    }

    bool ison;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) || ison)
        {
            ison = true;
            lifeBar.OnLoadBossGameObject(GetComponent<EnemyStats>());
            lifeBar.OnFightBoss(true);
        }
    }
}
