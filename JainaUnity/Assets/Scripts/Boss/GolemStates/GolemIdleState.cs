using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GolemStateEnum;

public class GolemIdleState : IState
{
    GolemController m_golemController;
    
    // Constructor (CTOR)
    public GolemIdleState (GolemController golemController)
    {
		  m_golemController = golemController;
    }

    public void Enter()
    {

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
