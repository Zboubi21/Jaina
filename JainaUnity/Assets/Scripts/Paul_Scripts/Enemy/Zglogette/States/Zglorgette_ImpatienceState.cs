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

    


    public override void Enter()
    {
        StateAnimation(m_enemyController.Anim);
        // m_enemyController.StartCoroutine(CastImpatience());
        m_enemyController.CastImpatienceCorout = m_enemyController.CastImpatience();
        m_enemyController.StartCoroutine(m_enemyController.CastImpatienceCorout);
        m_enemyController.SpawnRandomGameObject(m_enemyController.m_sounds.m_impatienceFx);
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
        if(m_enemyController.CastImpatienceCorout != null){
            m_enemyController.StopCoroutine(m_enemyController.CastImpatienceCorout);
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

    


}
