using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class AttackState : IState
{

    // CONSTRUCTOR

    EnemyController m_enemyController;

    #region Get Set
    public EnemyController EnemyController
    {
        get
        {
            return m_enemyController;
        }

        set
        {
            m_enemyController = value;
        }
    }
    #endregion

    public AttackState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

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


    #region AttackMethods

    public virtual void IfInAttackRange()
    {

    }

    public virtual void Attack()
    {
        if (m_enemyController.CanAttack && !m_enemyController.CheckAnimEnd1) //attack CoolDown
        {
            m_enemyController.StartAttackCoolDown();
            m_enemyController.Attack();
            m_enemyController.AnimFinished();
        }
    }

    #endregion

    

    #region Leaving State

    public virtual void OnOutState()
    {
        if (m_enemyController.CheckAnimEnd1)
        {
            if (/*!m_enemyController.InAttackRange() &&*/ !m_enemyController.PlayerInAttackBox())
            {

                m_enemyController.ChangeState((int)EnemyState.ChaseState); // ChaseState
            }
            /*else if (m_enemyController.InAttackRange() && !m_enemyController.PlayerInAttackBox() && m_enemyController.IsChasing())
            {
                m_enemyController.ChangeState(EnemyState.ImpatienceState); // Impatience
            }*/
            else
            {
                m_enemyController.ChangeState((int)EnemyState.AttackState); // Attack
            }

        }

    }

    #endregion
}
