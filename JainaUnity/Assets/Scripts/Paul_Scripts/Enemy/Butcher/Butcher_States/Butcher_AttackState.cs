using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Butcher;


public class Butcher_AttackState : AttackState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public Butcher_AttackState(EnemyController enemyController) : base (enemyController)
    {
        m_enemyController = enemyController;
    }
    /*
    public void Enter()
    {
        m_enemyController.CheckAnimEnd1 = false;
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        OnOutState();
        Attack();
    }

    public void Exit()
    {
    }
    */
    #region AttackMethods

    public override void IfInAttackRange()
    {

    }

    public override void Attack()
    {
        base.Attack();
        /*if (m_enemyController.CanAttack && !m_enemyController.CheckAnimEnd1) //attack CoolDown
        {
            m_enemyController.StartAttackCoolDown();
            m_enemyController.Attack();
            m_enemyController.AnimFinished();
        }*/
    }

    #endregion

    

    #region Leaving State

    public override void OnOutState()
    {
        if (m_enemyController.CheckAnimEnd1)
        {
            if (!m_enemyController.PlayerInAttackBox())
            {

                m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ChaseState); // ChaseState
            }
            else
            {
                m_enemyController.ChangeState((int)EnemyButcherState.Butcher_AttackState); // Attack
            }
        }
    }

    #endregion
}
