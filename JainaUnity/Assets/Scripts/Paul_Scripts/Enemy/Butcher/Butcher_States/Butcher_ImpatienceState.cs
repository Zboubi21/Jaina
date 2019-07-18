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

        m_enemyController.Agent.enabled = false;
        butcherController = m_enemyController.GetComponent<ButcherController>();
        butcherController.IsImpatience = true;

        StateAnimation(m_enemyController.Anim);

        // Destination();
        // FaceTarget();

        sign = m_enemyController.InstantiateObjects(butcherController.signImpatience, m_enemyController.TargetStats1.GetComponent<CharacterStats>().transform.position, Quaternion.identity);

        target = sign.transform;

        // m_enemyController.Agent.speed += 10;

        butcherController.NbrJump++;
        //Debug.Log(butcherController.NbrJump);

    }

    public override void FixedUpdate()
    {
        //butcherController.TranslateMove(target);
        //butcherController.FaceTarget(target);
    }

    public override void Update()
    {
        GetOutOfState();
    }

    public override void Exit()
    {
        DestroySign();
        m_enemyController.Agent.enabled = true;
        // m_enemyController.Agent.speed -= 10;
        //m_enemyController.Agent.enabled = true;
        butcherController.TempsJumpAnim = butcherController.AnimTime;
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
        if(m_enemyController.GetTargetDistance(target) <= 2)
        {
            if (m_enemyController.CheckIfAnimEnded())
            {
                if(butcherController.NbrJump == butcherController.numberOfJump)
                {
                    m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ChaseState);       //Chase
                    butcherController.NbrJump = 0;
                }
                else
                {
                    m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ImpatienceState);  //Impatience
                }
                //m_enemyController.OnImpactDamage();
            }
        }
    }

    #endregion
}
