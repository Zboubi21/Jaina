﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Zglorgette;


public class Zglorgette_ChaseState : ChaseState
{
    Ray ray;

    RaycastHit hit;
    // CONSTRUCTOR
    ZglorgetteController m_enemyController;
    public Zglorgette_ChaseState(ZglorgetteController enemyController) : base(enemyController)
    {
        m_enemyController = enemyController;
    }

    ButcherController butcherController;

    public override void Enter()
    {
        base.Enter();
        //m_enemyController.IsImpatience = false;
        //m_enemyController.CdImpatient = m_enemyController.CoolDownGettingImpatient;

        //if(butcherController == null)
            //butcherController = m_enemyController.GetComponent<ButcherController>();
    }

    #region Animation

    public override void StateAnimation(Animator anim)
    {
        base.StateAnimation(anim);
    }

    #endregion

    #region AgentMotion

    public override void Destination()
    {
        base.Destination();
    }

    public override void FaceTarget()
    {
        base.FaceTarget();
    }

    #endregion

    #region Yell Methods

    public override void MakeThenBeingYelledable(bool b)
    {
        base.MakeThenBeingYelledable(b);
    }

    public override void OnYelling()
    {
        base.OnYelling();
        /*if (!yell)
        {
            m_enemyController.Yell(m_enemyController.m_sM.CurrentStateIndex);
            yell = true;
        }*/
    }

    #endregion

    #region Leaving State
    public override void GetOutOfState()
    {
        Vector3 rayTarget = m_enemyController.TargetStats1.transform.position - m_enemyController.transform.position;
        if (Physics.Linecast(m_enemyController.transform.position, m_enemyController.TargetStats1.transform.position, out hit, m_enemyController.layers))
        {
            float targetDistance = Vector3.Distance(m_enemyController.transform.position, m_enemyController.TargetStats1.transform.position);
            if(targetDistance > m_enemyController.range)
            {
                Debug.DrawRay(m_enemyController.transform.position, rayTarget, Color.red);
            }
            else if (hit.collider != m_enemyController.TargetStats1.GetComponent<Collider>())
            {
                Debug.DrawRay(m_enemyController.transform.position, rayTarget, Color.yellow);
            }
            else
            {
                Debug.DrawRay(m_enemyController.transform.position, rayTarget, Color.green);
                m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_AttackState); //Attacks
            }
        }
        //if (m_enemyController.PlayerInAttackBox()) //GreenBox
        //{
        //    m_enemyController.ChangeState((int)EnemyButcherState.Butcher_AttackState); //Attack
        //}
        //else if (m_enemyController.TargetInImpatienceDonuts() && !m_enemyController.IsRootByIceNova)
        //{
        //    if (m_enemyController.CoolDownImpatience())
        //    {
        //        if(!m_enemyController.m_butcherJump.m_checkArea){
        //            butcherController.m_butcherJump.m_checkArea = true;
        //            butcherController.StartCheckJumpArea();
        //        }
        //    }
        //}
        //else
        //{
        //    m_enemyController.CdImpatient = m_enemyController.CoolDownGettingImpatient;
        //}

    }
    #endregion

}
