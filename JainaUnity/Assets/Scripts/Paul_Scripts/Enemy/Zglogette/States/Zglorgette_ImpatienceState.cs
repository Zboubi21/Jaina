﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Zglorgette;

public class Zglorgette_ImpatienceState : ImpatienceState
{

    // CONSTRUCTOR
    ZglorgetteController m_enemyController;
    public Zglorgette_ImpatienceState(ZglorgetteController enemyController) : base(enemyController)
    {
        m_enemyController = enemyController;
    }
    GameObject sign;

    int rayCastReturn;
    int rayCastFowardReturn;

    int nbrOfProjectilThrown;
    IEnumerator m_castImpatienceCorout;

    public override void Enter()
    {
        StateAnimation(m_enemyController.Anim);
        // m_enemyController.StartCoroutine(CastImpatience());
        m_castImpatienceCorout = CastImpatience();
        m_enemyController.StartCoroutine(m_castImpatienceCorout);
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        FaceTarget();
        rayCastReturn = m_enemyController.OnRayCastSide();
        rayCastFowardReturn = m_enemyController.OnRayCast();
    }

    public override void Exit()
    {
        DestroySign();
        if(m_castImpatienceCorout != null){
            m_enemyController.StopCoroutine(m_castImpatienceCorout);
        }
    }

    #region Animation

    public override void StateAnimation(Animator anim)
    {
        anim.SetTrigger("Impatience");
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

    #region Impatience Effect

    public override void DestroySign()
    {
        m_enemyController.DestroyGameObject(sign);

    }

    #endregion

    IEnumerator CastImpatience()
    {
        yield return new WaitForSeconds(0.5f);
        while (nbrOfProjectilThrown < m_enemyController.nombreDeGrandeAttack)
        {
            m_enemyController.OnCastImpatienceProjectil();
            nbrOfProjectilThrown++;
            yield return new WaitForSeconds(m_enemyController.timeBetweenImpatiencePorjectil);
        }
        nbrOfProjectilThrown = 0;
        yield return new WaitForSeconds(0.5f);
        if (rayCastReturn == 2 && rayCastFowardReturn == 2/*&& m_enemyController.OnRayRightCast() == 2 && m_enemyController.OnRayLeftCast() == 2*/)
        {
            m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_AttackState); //Attack
        }
        else
        {
            m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_ChaseState); //Chase
        }
    }


}
