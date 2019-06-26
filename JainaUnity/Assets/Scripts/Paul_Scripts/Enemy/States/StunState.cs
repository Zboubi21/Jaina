using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    float timeBeingStun;
    bool HasBeenOnAlert;
    bool isStunAble = false;

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public StunState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        IsStunable();
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        if (isStunAble)
        {
            //Debug.Log("timeBeingStun = " + timeBeingStun);
            timeBeingStun -= Time.deltaTime;
            if(timeBeingStun <= 0)
            {
                GetOutOfState();
            }
        }
    }

    public void Exit()
    {
        BeStunable(false);
    }

    public virtual void GetOutOfState()
    {
        
        if(!HasBeenOnAlert)
        {
            m_enemyController.ChangeState(2);
            HasBeenOnAlert = true;
        }
        //else if (m_enemyController.GetLastStateIndex() != 6)
        //{
        //    m_enemyController.ChangeState(m_enemyController.GetLastStateIndex());
        //}
        else
        {
            m_enemyController.ChangeState(3);
        }

    }

    public virtual void IsStunable()
    {
        m_enemyController.Anim.SetTrigger("Hit");
        isStunAble = true;
        BeStunable(true);
        timeBeingStun = m_enemyController.timeBeingStuned;
    }

    public virtual void BeStunable(bool b)
    {
        m_enemyController.StopMoving(b);
    }

}
