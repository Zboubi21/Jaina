using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Zglorgette;


public class Zglorgette_AttackState : AttackState
{

    // CONSTRUCTOR
    ZglorgetteController m_enemyController;
    public Zglorgette_AttackState(ZglorgetteController enemyController) : base (enemyController)
    {
        m_enemyController = enemyController;
    }
    int rayCastReturn;
    int rayCastFowardReturn;


    public override void Enter()
    {
        base.Enter();
        m_enemyController.StopMoving(true);
    }
    public override void Update()
    {
        base.Update();
        m_enemyController.FaceTarget();
    }
    /*
    public void Enter()
    {
        m_enemyController.CheckAnimEnd1 = false;
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        OnOutState();
        Attack();
    }

    public void Exit()
    {
    }
    */
    #region AttackMethods

    public override void IfInAttackRange()
    {

    }

    public override void Attack()
    {
        base.Attack();
        /*if (m_enemyController.CanAttack && !m_enemyController.CheckAnimEnd1) //attack CoolDown
        {
            m_enemyController.StartAttackCoolDown();
            m_enemyController.Attack();
            m_enemyController.AnimFinished();
        }*/
    }

    #endregion



    #region Leaving State

    public override void OnOutState()
    {
        rayCastReturn = m_enemyController.OnRayCastSide();
        rayCastFowardReturn = m_enemyController.OnRayCast();
        if (!m_enemyController.CoolDownWitchImpatience())
        {
            if (m_enemyController.CheckAnimEnd1)
            {
                if (rayCastReturn == 2 && rayCastFowardReturn == 2/* && m_enemyController.OnRayRightCast() == 2 && m_enemyController.OnRayLeftCast() == 2*/)
                {
                    m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_AttackState); //Attack
                }
                else
                {
                    m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_ChaseState); //Chase
                }
            }
        }
        else
        {
            m_enemyController.TimeBeforeZglorgetteGettingImpatient = m_enemyController.CurrentTimeBeforeZglorgetteGettingImpatient;
            m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_ImpatienceState); //Impatience

        }
    }

    #endregion
}
