using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : IState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public DieState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        m_enemyController.Anim.SetTrigger("Die");
        m_enemyController.StopMoving(true);
        m_enemyController.Die();
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        
    }

    public void Exit()
    {

    }

}
