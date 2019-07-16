using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Butcher;


public class Butcher_ChaseState : ChaseState
{

    // CONSTRUCTOR
    ButcherController m_enemyController;
    public Butcher_ChaseState(ButcherController enemyController) : base(enemyController)
    {
        m_enemyController = enemyController;
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
        if (m_enemyController.PlayerInAttackBox()) //GreenBox
        {
            m_enemyController.ChangeState((int)EnemyButcherState.Butcher_AttackState); //Attack
        }
        else if (m_enemyController.TargetInImpatienceDonuts())
        {
            if (m_enemyController.CoolDownImpatience())
            {
                m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ImpatienceState); //Impatience
                m_enemyController.NbrJump++;
            }
        }
        else
        {
            m_enemyController.CdImpatient = m_enemyController.CoolDownGettingImpatient;
        }
    }
    #endregion

}
