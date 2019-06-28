using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ImpatienceState : IState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public ImpatienceState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }



    public void Enter()
    {
        StateAnimation(m_enemyController.Anim);
        ImpatienceEffect(m_enemyController.speedSprint);
    }

    public void FixedUpdate()
    {
        Destination();
    }

    public void Update()
    {
        FaceTarget();
        GetOutOfState();
    }

    public void Exit()
    {
        ImpatienceEffect(m_enemyController.AgentSpeed);
        DestroySign();
    }


    #region Animation

    public virtual void StateAnimation(Animator anim)
    {
        anim.SetTrigger("Impatience");
    }

    #endregion

    #region AgentMotion

    public virtual void Destination()
    {
        m_enemyController.SetDestination();
    }

    public virtual void FaceTarget()
    {
        m_enemyController.FaceTarget();
    }

    #endregion

    #region Impatience Effect

    public virtual void ImpatienceEffect(float speed)
    {
        m_enemyController.Sprint(speed);
    }

    public virtual void DestroySign()
    {
        if (m_enemyController._signImpatience != null)
        {
            m_enemyController.DestroyGameObject(m_enemyController._signImpatience);
        }
    }

    #endregion

    #region Leaving State

    public virtual void GetOutOfState()
    {
        if (m_enemyController.InAttackRange())
        {
            if (m_enemyController.CanAttackWhenImpatience() || m_enemyController.PlayerInAttackBox())
            {
                m_enemyController.ChangeState(EnemyState.AttackState); //Attack
            }
        }
        //else
        //{
        //    m_enemyController.ChangeState(3); //Chase
        //}
    }

    #endregion
}
