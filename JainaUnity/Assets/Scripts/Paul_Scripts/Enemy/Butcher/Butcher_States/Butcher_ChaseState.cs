using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Butcher;


public class Butcher_ChaseState : ChaseState
{
    bool yell;
    bool hasCalledFunction;
    // CONSTRUCTOR
    EnemyController m_enemyController;
    public Butcher_ChaseState(EnemyController enemyController) : base(enemyController)
    {
        m_enemyController = enemyController;
    }
    /*
    public void Enter()
    {
        MakeThenBeingYelledable(hasCalledFunction);
        StateAnimation(m_enemyController.Anim);
        m_enemyController.ImpatienceSign.gameObject.SetActive(false);
        if(m_enemyController.m_sM.IsLastStateIndex((int)EnemyState.AlerteState)){
            m_enemyController.StartCoroutine(StartDetectedFx());
        }
    }

    public void FixedUpdate()
    {
        Destination();
        FaceTarget();
    }

    public void Update()
    {
        GetOutOfState();
        OnYelling();
    }

    public void Exit()
    {
        m_enemyController.IsChasing();
    }
    */

    #region Animation

    public override void StateAnimation(Animator anim)
    {
        base.StateAnimation(anim);
        /*if (!m_enemyController.IsRootByIceNova)
        {
            anim.SetTrigger("Chase");
        }
        else
        {
            anim.SetTrigger("Idle");
        }*/
    }

    #endregion

    #region AgentMotion

    public override void Destination()
    {
        base.Destination();
        //m_enemyController.SetDestination();
    }

    public override void FaceTarget()
    {
        base.FaceTarget();
        //m_enemyController.FaceTarget();
    }

    #endregion

    #region Yell Methods

    public override void MakeThenBeingYelledable(bool b)
    {
        base.MakeThenBeingYelledable(b);
        /*if (!b)
        {
            for (int i = 0, l = m_enemyController.Enemy.Count; i < l; i++)
            {
                if(m_enemyController.Enemy[i] != null){  // J'ai rajouté ça sinon il y avait des errurs ! <3
                    m_enemyController.Enemy[i].gameObject.GetComponent<EnemyController>().BeenYelled = false;
                }
                yell = false;
            }
            hasCalledFunction = true;
        }*/
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
        if (m_enemyController.PlayerInAttackBox()) //GreenBox
        {
            m_enemyController.ChangeState((int)EnemyButcherState.Butcher_AttackState); //Attack
        }
    }
    #endregion

    /*IEnumerator StartDetectedFx(){
        m_enemyController.m_detectedFx.gameObject.SetActive(true);
        m_enemyController.m_detectedFx.StartParticle();
        yield return new WaitForSeconds(m_enemyController.m_timeToShowDetectedFx);
        m_enemyController.m_detectedFx.gameObject.SetActive(false);
    }*/

}
