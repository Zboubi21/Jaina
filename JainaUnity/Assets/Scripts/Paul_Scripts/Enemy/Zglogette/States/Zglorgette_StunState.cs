using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Zglorgette;

public class Zglorgette_StunState : StunState
{
    /*float timeBeingStun;
    bool isStunAble = false;*/

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public Zglorgette_StunState(EnemyController enemyController) : base(enemyController)
    {
        m_enemyController = enemyController;
    }
    /*
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
    }
    */
    public override void GetOutOfState()
    {
        if(!m_enemyController.HasBeenOnAlert)
        {
            m_enemyController.ChangeState((int)EnemyZglorgetteState.AlerteState);
            m_enemyController.HasBeenOnAlert = true;
        }
        else if (m_enemyController.GetComponent<ZglorgetteController>().OnRayCast() == 2)
        {
            m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_AttackState);
        }
        else
        {
            m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_ChaseState);
        }
    }

    public override void IsStunable()
    {
        base.IsStunable();
        /*m_enemyController.Anim.SetTrigger("Hit");
        isStunAble = true;
        BeStunable(true);
        timeBeingStun = m_enemyController.timeBeingStuned;*/
    }

    public override void BeStunable(bool b)
    {
        base.BeStunable(b);
        //m_enemyController.StopMoving(b);
    }

}
