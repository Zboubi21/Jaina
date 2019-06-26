using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public IdleState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        m_enemyController.Anim.SetTrigger("Idle");
    }

    public void FixedUpdate()
    {
        
    }

    public void Update()
    {
        LookingForTarget();
    }

    public void Exit()
    {

    }

    public virtual void LookingForTarget()
    {
        if (m_enemyController.PlayerInLookRange())
        {
            m_enemyController.ChangeState(3);
        }
    }

}
