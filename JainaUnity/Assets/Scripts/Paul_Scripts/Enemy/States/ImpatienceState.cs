using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ImpatienceState : IState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
    ZglorgController m_zglorgController;
    public ImpatienceState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
        m_zglorgController = enemyController.GetComponent<ZglorgController>();
    }



    public virtual void Enter()
    {
        StateAnimation(m_enemyController.Anim);
        ImpatienceEffect(m_zglorgController.speedSprint);
        m_enemyController.IsImpatient = true;
    }

    public virtual void FixedUpdate()
    {
        Destination();
    }

    public virtual void Update()
    {
        FaceTarget();
        GetOutOfState();
    }

    public virtual void Exit()
    {
        m_enemyController.IsImpatient = false;

        ImpatienceEffect(m_enemyController.AgentSpeed);
        DestroySign();
    }


    #region Animation

    public virtual void StateAnimation(Animator anim)
    {
        if (!m_enemyController.IsRootByIceNova)
        {
            anim.SetTrigger("Impatience");
        }
        else
        {
            // anim.SetTrigger("Idle");
            anim.SetTrigger("Freeze");
        }
    }

    #endregion

    #region AgentMotion

    public virtual void Destination()
    {
        m_enemyController.SetDestination(m_enemyController.Target.transform.position);
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
        // if (m_enemyController._signImpatience != null)
        // {
        //     m_enemyController.DestroyGameObject(m_enemyController._signImpatience);
        // }
        m_enemyController.ImpatienceSign.gameObject.SetActive(false);
    }

    #endregion

    #region Leaving State

    public virtual void GetOutOfState()
    {
        if (/*m_enemyController.InAttackRange()*/m_enemyController.PlayerInAttackBox())
        {
            if (m_enemyController.CanAttackWhenImpatience() || m_enemyController.PlayerInAttackBox())
            {
                m_enemyController.ChangeState((int)EnemyState.AttackState); //Attack
            }
        }
        //else
        //{
        //    m_enemyController.ChangeState(3); //Chase
        //}
    }

    #endregion
}
