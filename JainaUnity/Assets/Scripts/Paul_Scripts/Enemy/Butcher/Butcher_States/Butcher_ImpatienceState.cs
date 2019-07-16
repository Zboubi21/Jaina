using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Butcher;


public class Butcher_ImpatienceState : ImpatienceState
{

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public Butcher_ImpatienceState(EnemyController enemyController) : base(enemyController)
    {
        m_enemyController = enemyController;
    }

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

    #region Impatience Effect

    public override void ImpatienceEffect(float speed)
    {

    }

    public override void DestroySign()
    {

    }

    #endregion

    #region Leaving State

    public override void GetOutOfState()
    {
        if (m_enemyController.PlayerInAttackBox())
        {
            m_enemyController.ChangeState((int)EnemyButcherState.Butcher_AttackState); //Attack
        }
    }

    #endregion
}
