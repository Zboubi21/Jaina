using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Zglorgette;

public class Zglorgette_ImpatienceState : ImpatienceState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public Zglorgette_ImpatienceState(EnemyController enemyController) : base(enemyController)
    {
        m_enemyController = enemyController;
    }
    GameObject sign;
    Transform target;

    public override void Enter()
    {

    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
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

    public override void ImpatienceEffect(float speed)
    {

    }

    public override void DestroySign()
    {
        m_enemyController.DestroyGameObject(sign);

    }

    #endregion


}
