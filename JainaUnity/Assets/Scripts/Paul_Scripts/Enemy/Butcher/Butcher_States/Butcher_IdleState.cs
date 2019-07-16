using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Butcher;

public class Butcher_IdleState : IdleState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public Butcher_IdleState(EnemyController enemyController) : base(enemyController)
    {
        m_enemyController = enemyController;
    }
    /*
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
    */
    public override void LookingForTarget()
    {
        if (m_enemyController.PlayerInLookRange())
        {
            m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ChaseState);
        }
    }

}
