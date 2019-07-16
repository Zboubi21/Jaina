using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Butcher;

public class Butcher_AlerteState : AlerteState
{
    bool yell;
    // CONSTRUCTOR
    EnemyController m_enemyController;
    public Butcher_AlerteState(EnemyController enemyController) : base(enemyController)
    {
        m_enemyController = enemyController;
    }
    /*
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
    */
    public override void Alert()
    {
        base.Alert();
    }

    public override void LookingForTarget()
    {
        if (m_enemyController.PlayerInLookRange())
        {
            m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ChaseState); // Chase
        }
    }

}
