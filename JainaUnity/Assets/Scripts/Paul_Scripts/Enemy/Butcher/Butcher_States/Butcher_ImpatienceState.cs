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
    GameObject sign;
    Transform target;
    ButcherController butcherController;

    public override void Enter()
    {
        butcherController = m_enemyController.GetComponent<ButcherController>();
        StateAnimation(m_enemyController.Anim);
        Destination();
        FaceTarget();
        sign = m_enemyController.InstantiateObjects(butcherController.signImpatience, m_enemyController.TargetStats1.GetComponent<CharacterStats>().transform.position, Quaternion.identity);
        target = sign.transform;
        m_enemyController.Agent.speed += 10;

        butcherController.NbrJump++;

    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        GetOutOfState();
    }

    public override void Exit()
    {
        DestroySign();
        m_enemyController.Agent.speed -= 10;
        butcherController.TempsJumpAnim = 3f;
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

    #region Leaving State

    public override void GetOutOfState()
    {
        if (m_enemyController.PlayerInAttackBox())
        {
            m_enemyController.ChangeState((int)EnemyButcherState.Butcher_AttackState); //Attack
        }
        if(m_enemyController.GetTargetDistance(target) <= 2 && m_enemyController.CheckIfAnimEnded())
        {
            m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ChaseState); //Chase
        }
    }

    #endregion
}
