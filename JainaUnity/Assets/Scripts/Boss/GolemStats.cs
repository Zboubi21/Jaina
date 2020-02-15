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
    [Range(0, 100)] public float m_phase2LifeTrigger = 66f;
    [Range(0, 100)] public float m_phase3LifeTrigger = 33f;
    [Space]
    public float m_timeBeforeTriggerPhase2 = 120f;
    public float m_timeBeforeTriggerPhase3 = 180f;

    GolemController m_golemController;
    int m_actualPhase = 1;
    bool loadPhase2;
    bool loadPhase3;


    public override void Start()
    {
        base.Start();
        m_golemController = GetComponent<GolemController>();
        m_actualPhase = m_golemController.PhaseNbr;
        StartCoroutine(PhaseTimer1());
    }


    protected override void CheckPhaseChanges()
    {
        if(m_actualPhase == 1 && (GetLifePercentage() <= m_phase2LifeTrigger || loadPhase2))
        {
            m_actualPhase = 2;
            m_golemController.On_GolemChangePhase();
            StopCoroutine(PhaseTimer1());
            StartCoroutine(PhaseTimer2());

        }
        else if(m_actualPhase == 2 && (GetLifePercentage() <= m_phase3LifeTrigger || loadPhase3))
        {
            m_actualPhase = 3;
            m_golemController.On_GolemChangePhase();
            StopCoroutine(PhaseTimer2());

        }
    }

    float GetLifePercentage()
    {
        return CurrentHealth / maxHealth * 100;
    }



    IEnumerator PhaseTimer1()
    {
        yield return new WaitForSeconds(m_timeBeforeTriggerPhase2);
        loadPhase2 = true;
        CheckPhaseChanges();
    }
    IEnumerator PhaseTimer2()
    {
        yield return new WaitForSeconds(m_timeBeforeTriggerPhase3);
        loadPhase3 = true;
        CheckPhaseChanges();
    }
}
