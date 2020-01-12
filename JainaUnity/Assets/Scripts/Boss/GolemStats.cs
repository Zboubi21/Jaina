using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using EnemyStateEnum;

public class GolemStats : EnemyStats
{

    [Space]
    [Header("Boss Phases")]
    [SerializeField, Range(0, 100)] float m_phase2LifeTrigger = 66;
    [SerializeField, Range(0, 100)] float m_phase3LifeTrigger = 33;

    GolemController m_golemController;
    int m_actualPhase = 1;

    public override void Start()
    {
        base.Start();
        m_golemController = GetComponent<GolemController>();
        m_actualPhase = m_golemController.PhaseNbr;
    }

    protected override void CheckPhaseChanges()
    {
        if(m_actualPhase == 1 && GetLifePercentage() <= m_phase2LifeTrigger)
        {
            m_actualPhase = 2;
            m_golemController.On_GolemChangePhase();
        }

        if(m_actualPhase == 2 && GetLifePercentage() <= m_phase3LifeTrigger)
        {
            m_actualPhase = 3;
            m_golemController.On_GolemChangePhase();
        }
    }

    float GetLifePercentage()
    {
        return CurrentHealth / maxHealth * 100;
    }

}
