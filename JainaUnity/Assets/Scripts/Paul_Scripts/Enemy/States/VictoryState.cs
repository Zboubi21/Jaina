using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class VictoryState : IState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public VictoryState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        m_enemyController.StopMoving(true);
        m_enemyController.Anim.SetTrigger("Victory");
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
