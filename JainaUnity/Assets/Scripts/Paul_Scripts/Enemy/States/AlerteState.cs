using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class AlerteState : IState
{
    bool yell;
    // CONSTRUCTOR
    EnemyController m_enemyController;
    public AlerteState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        m_enemyController.Anim.SetTrigger("Alerte");
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        if (!yell)
        {
            m_enemyController.Yell(m_enemyController.m_sM.CurrentStateIndex);
            yell = true;
        }
        Alert();
        LookingForTarget();
    }

    public void Exit()
    {

    }

    public virtual void Alert()
    {
        m_enemyController.IsAlert();
    }

    public virtual void LookingForTarget()
    {
        if (m_enemyController.PlayerInLookRange())
        {
            m_enemyController.ChangeState(EnemyState.ChaseState); // Chase
        }
    }
}
