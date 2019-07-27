using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ChaseState : IState
{
    bool yell;
    bool hasCalledFunction;

    bool hasBeenCounted;
    // CONSTRUCTOR
    EnemyController m_enemyController;
    public ChaseState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public virtual void Enter()
    {
        MakeThenBeingYelledable(hasCalledFunction);
        StateAnimation(m_enemyController.Anim);

        // if(m_enemyController._signImpatience != null)
        // {
        //     m_enemyController.DestroyGameObject(m_enemyController._signImpatience);
        // }
        if (!hasBeenCounted && m_enemyController.TargetStats1 != null)
        {
            m_enemyController.TargetStats1.OnEnemyInCombatCount();
            hasBeenCounted = true;
        }

        m_enemyController.ImpatienceSign.gameObject.SetActive(false);

        if(m_enemyController.m_sM.IsLastStateIndex((int)EnemyState.AlerteState) || !m_enemyController.HasBeenOnAlert)
        {
            m_enemyController.StartCoroutine(StartDetectedFx());
            m_enemyController.HasBeenOnAlert = true;
        }
    }

    public virtual void FixedUpdate()
    {
        Destination();
        // FaceTarget();
    }

    public void Update()
    {
        GetOutOfState();
        OnYelling();
        FaceTarget();
    }

    public void Exit()
    {
        m_enemyController.IsChasing();
    }


    #region Animation

    public virtual void StateAnimation(Animator anim)
    {
        if (!m_enemyController.IsRootByIceNova)
        {
            anim.SetTrigger("Chase");
        }
        else
        {
            // anim.SetTrigger("Idle");
            anim.SetTrigger("Freeze");
        }
    }

    #endregion

    #region AgentMotion

    public virtual void Destination()
    {
        m_enemyController.SetDestination(m_enemyController.Target.transform.position);
    }

    public virtual void FaceTarget()
    {
        m_enemyController.FaceTarget(m_enemyController.Target.transform.position);
    }

    #endregion

    #region Yell Methods

    public virtual void MakeThenBeingYelledable(bool b)
    {
        if (!b)
        {
            for (int i = 0, l = m_enemyController.Enemy.Count; i < l; i++)
            {
                if(m_enemyController.Enemy[i] != null){  // J'ai rajouté ça sinon il y avait des errurs ! <3
                    m_enemyController.Enemy[i].gameObject.GetComponent<EnemyController>().BeenYelled = false;
                }
                yell = false;
            }
            hasCalledFunction = true;
        }
    }

    public virtual void OnYelling()
    {
        if (!yell)
        {
            m_enemyController.Yell(m_enemyController.m_sM.CurrentStateIndex);
            yell = true;
        }
    }

    #endregion

    #region Leaving State
    public virtual void GetOutOfState()
    {
        if (/*m_enemyController.InAttackRange()*/ /*Stopping Distance*/ /*&&*/ m_enemyController.PlayerInAttackBox()) //GreenBox
        {
            m_enemyController.ChangeState((int)EnemyState.AttackState); //Attack
        }
        //else if (m_enemyController.InAttackRange() /*Stopping Distance*/ && !m_enemyController.PlayerInAttackBox() && m_enemyController.IsInAttackRangeForToLong())
        //{
        //    Debug.Log("Nop");
        //    m_enemyController.ChangeState(EnemyState.AttackState); //Attack
        //}
        else if (!m_enemyController.IsRootByIceNova)
        {
            if (m_enemyController.IsChasing())
            {
                m_enemyController.ChangeState((int)EnemyState.ImpatienceState); // Impatience
            }
        }
    }
    #endregion

    IEnumerator StartDetectedFx(){
        // if(!m_enemyController.m_sounds.m_useZglorgSoundManager){
            m_enemyController.SpawnRandomGameObject(m_enemyController.m_sounds.m_detectionFx);
        // }else{
        //     if(m_enemyController.ZglorgSoundManager.CanDoDeathSound()){
        //         m_enemyController.SpawnRandomGameObject(m_enemyController.m_sounds.m_detectionFx);
        //     }
        // }
        m_enemyController.m_detectedFx.gameObject.SetActive(true);
        m_enemyController.m_detectedFx.StartParticle();
        yield return new WaitForSeconds(m_enemyController.m_timeToShowDetectedFx);
        m_enemyController.m_detectedFx.gameObject.SetActive(false);
    }

}
