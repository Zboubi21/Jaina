using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
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
            if (!m_enemyController.InAttackRange())
            {
                m_enemyController.ChangeState(3); // ChaseState
            }
            else if (m_enemyController.InAttackRange() && !m_enemyController.PlayerInAttackBox())
            {
                m_enemyController.ChangeState(4); // Impatience
            }
            else
            {
                m_enemyController.ChangeState(5); // Attack
            }
        }
    }

    #endregion
}
