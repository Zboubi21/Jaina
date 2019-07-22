using System.Collections;
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

    int nbrOfProjectilThrown;

    public override void Enter()
    {
        StateAnimation(m_enemyController.Anim);
        m_enemyController.StartCoroutine(CastImpatience());
        Debug.Log("ImpatienceEnter");
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        FaceTarget();
    }

    public override void Exit()
    {
        DestroySign();
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
        if (m_enemyController.OnRayCast() == 2)
        {
            m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_AttackState); //Attack
        }
        else
        {
            m_enemyController.ChangeState((int)EnemyZglorgetteState.Zglorgette_ChaseState); //Chase
        }
    }


}
