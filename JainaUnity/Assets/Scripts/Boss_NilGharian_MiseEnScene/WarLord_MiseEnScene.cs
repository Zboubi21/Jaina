﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WarLord_MiseEnScene : MonoBehaviour
{
    [Header ("Event Trigger")]
    [Range(0,100)]
    public float _percentHpBeforeEvent;
    [Space]
    [Header("Event Animations")]
    public Animator[] anims;
    public string triggerOn;
    public string triggerOff;
    [Space]
    [Header("Boss Life Bar")]
    public BigEnemyLifeBarManager lifeBar;
    [Space]
    [Header("Cinématique var")]
    public float m_timeToBeInCinematicState = 5;
    [Space]
    [Header("Event Triggering")]
    public UnityEvent OnStartFight;
    public UnityEvent OnEndFight;

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
            for (int i = 0, l = anims.Length; i < l; ++i)
            {
                anims[i].SetTrigger(triggerOn);
            }
            lifeBar.OnLoadBossGameObject(GetComponent<EnemyStats>());
            lifeBar.OnFightBoss(true);
        }

        if (stats.CurrentHealth <= (stats.maxHealth * (_percentHpBeforeEvent/100f)) && !fightHasEnded)
        {
            fightHasEnded = true;
            fightHasStart = false;
            for (int i = 0, l = anims.Length; i < l; ++i)
            {
                anims[i].SetTrigger(triggerOff);
            }
            lifeBar.OnLoadBossGameObject(GetComponent<EnemyStats>());
            lifeBar.OnFightBoss(false);
            m_playerManager.SwitchPlayerToCinematicState(m_timeToBeInCinematicState);
            OnEndFight.Invoke();
        }
    }
}
