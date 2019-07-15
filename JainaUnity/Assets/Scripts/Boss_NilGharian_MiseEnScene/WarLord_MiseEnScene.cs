using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WarLord_MiseEnScene : MonoBehaviour
{
    public UnityEvent OnStartFight;
    public UnityEvent OnEndFight;

    EnemyController controller;
    EnemyStats stats;

    bool fightHasStart;
    bool fightHasEnded;
    PlayerManager m_playerManager;

    void Start()
    {
        controller = GetComponent<EnemyController>();
        stats = GetComponent<EnemyStats>();
        m_playerManager = PlayerManager.Instance;
    }

    void Update()
    {
        if (controller.PlayerInLookRange() && !fightHasStart)
        {
            fightHasStart = true;
            OnStartFight.Invoke();
        }

        if (stats.CurrentHealth <= (stats.maxHealth/2) && !fightHasEnded)
        {
            fightHasEnded = true;
            // m_playerManager.SwitchPlayerToCinematicState(FautMettreUnTemps);
            OnEndFight.Invoke();
        }
    }
}
