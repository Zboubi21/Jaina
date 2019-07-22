using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class StunState : IState
{
    float timeBeingStun;
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
        m_enemyController.m_stunFx.SetActive(true);
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
        if (!m_enemyController.IsRootByIceNova)
        {
            BeStunable(false);
        }
        m_enemyController.m_stunFx.SetActive(false);
    }

    public virtual void GetOutOfState()
    {
        
        if(!m_enemyController.HasBeenOnAlert)
        {
            m_enemyController.ChangeState((int)EnemyState.AlerteState);
            m_enemyController.HasBeenOnAlert = true;
            Debug.Log(m_enemyController.name);
        }
        //else if (m_enemyController.GetLastStateIndex() != 6)
        //{
        //    m_enemyController.ChangeState(m_enemyController.GetLastStateIndex());
        //}
        else
        {
            m_enemyController.ChangeState((int)EnemyState.ChaseState);
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
