using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WarLord_MiseEnScene : MonoBehaviour
{
    [Range(0,100)]
    public float _percentHpBeforeEvent;
    [Space]
    public UnityEvent OnStartFight;
    public UnityEvent OnEndFight;
    [Space]
    public float m_timeToBeInCinematicState = 5;

    EnemyController controller;
    EnemyStats stats;

    bool fightHasStart;
    bool fightHasEnded;
    PlayerManager m_playerManager;


    #region Get Set
    public bool FightHasStart
    {
        get
        {
            return fightHasStart;
        }

        set
        {
            fightHasStart = value;
        }
    }

    public bool FightHasEnded
    {
        get
        {
            return fightHasEnded;
        }

        set
        {
            fightHasEnded = value;
        }
    }

    #endregion

    void Start()
    {
        controller = GetComponent<EnemyController>();
        stats = GetComponent<EnemyStats>();
        m_playerManager = PlayerManager.Instance;
        fightHasEnded = false;
    }

    void Update()
    {
        if (controller.PlayerInLookRange() && !fightHasStart && !fightHasEnded)
        {
            fightHasStart = true;
            OnStartFight.Invoke();
        }

        if (stats.CurrentHealth <= (stats.maxHealth * (_percentHpBeforeEvent/100)) && !fightHasEnded)
        {
            fightHasEnded = true;
            fightHasStart = false;
            m_playerManager.SwitchPlayerToCinematicState(m_timeToBeInCinematicState);
            OnEndFight.Invoke();
        }
    }
}
