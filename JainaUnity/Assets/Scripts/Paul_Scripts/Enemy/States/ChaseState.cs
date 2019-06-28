using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ChaseState : IState
{
    bool yell;
    bool hasCalledFunction;
    // CONSTRUCTOR
    EnemyController m_enemyController;
    public ChaseState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        StateAnimation(m_enemyController.Anim);
        MakeThenBeingYelledable(hasCalledFunction);
    }

    public void FixedUpdate()
    {
        Destination();
    }

    public void Update()
    {
        OnYelling();
        GetOutOfState();
    }

    public void Exit()
    {
        m_enemyController.IsChasing();
    }


    #region Animation

    public virtual void StateAnimation(Animator anim)
    {
        anim.SetTrigger("Chase");
    }

    #endregion

    #region AgentMotion

    public virtual void Destination()
    {
        m_enemyController.SetDestination();
    }

    #endregion

    #region Yell Methods

    public virtual void MakeThenBeingYelledable(bool b)
    {
        if (!b)
        {
            for (int i = 0, l = m_enemyController.Enemy.Count; i < l; i++)
            {
                if(m_enemyController.Enemy[i].gameObject != null){  // J'ai rajouté ça sinon il y avait des errurs ! <3
                    m_enemyController.Enemy[i].gameObject.GetComponent<EnemyController>().BeenYelled = false;
                }
                yell = false;
            }
            hasCalledFunction = true;
        }
    }

    public virtual void OnYelling()
    {
        if (!yell)
        {
            m_enemyController.Yell(m_enemyController.m_sM.CurrentStateIndex);
            yell = true;
        }
    }

    #endregion

    #region Leaving State
    public virtual void GetOutOfState()
    {
        if (m_enemyController.InAttackRange() /*Stopping Distance*/ && m_enemyController.PlayerInAttackBox()) //GreenBox
        {
            m_enemyController.ChangeState(EnemyState.AttackState); //Attack
        }
        else if (m_enemyController.IsChasing())
        {
            m_enemyController.ChangeState(EnemyState.ImpatienceState); // Impatience
        }
    }
    #endregion
}
