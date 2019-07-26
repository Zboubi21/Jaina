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

    // GameObject m_jumpSign;
    // Transform target;
    ButcherController butcherController;

    // IEnumerator m_rotateCorout;
    // IEnumerator m_jumpCorout;
    // IEnumerator m_exitStateCorout;

    public override void Enter()
    {
        m_enemyController.StopMoving(true);

        m_enemyController.Agent.enabled = false;

        if(butcherController == null)
            butcherController = m_enemyController.GetComponent<ButcherController>();

        butcherController.IsImpatience = true;

        // StateAnimation(m_enemyController.Anim);
        if(GetDistanceFromTarget() > butcherController.m_butcherJump.m_miniJumpDistance){
            butcherController.TargetIsClosed = false;
            butcherController.Anim.SetTrigger("Impatience");
        }else{
            butcherController.TargetIsClosed = true;
            butcherController.Anim.SetTrigger("ImpatienceSlow");
        }

        butcherController.JumpSign = m_enemyController.InstantiateObjects(butcherController.m_signImpatienceFx, butcherController.m_butcherJump.m_targetJumpPos, butcherController.m_signImpatienceFx.transform.rotation);

        m_enemyController.SpawnRandomGameObject(m_enemyController.m_sounds.m_impatienceFx);

        butcherController.RotateCorout = butcherController.RotateBeforeJump(butcherController.m_butcherJump.m_targetJumpPos, butcherController.TargetIsClosed);
        butcherController.StartCoroutine(butcherController.RotateCorout);

        butcherController.NbrJump++;
        //Debug.Log(butcherController.NbrJump);

        butcherController.ImpatienceSign.gameObject.SetActive(true);
        butcherController.ImpatienceSign.StartParticle();
    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        m_enemyController.Agent.enabled = true;
        butcherController.TempsJumpAnim = butcherController.AnimTime;
        butcherController.ImpatienceSign.gameObject.SetActive(false);
        butcherController.StopAllButcherCoroutines();
        butcherController.DestroyButcherJumpSign();
    }

    #region Animation

    // public override void StateAnimation(Animator anim)
    // {
    //     anim.SetTrigger("Impatience");
    // }

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
        // if(m_jumpSign != null){
        //     m_enemyController.DestroyGameObject(m_jumpSign);
        // }
    }

    #endregion

    float GetDistanceFromTarget(){
        Vector3 fromPos = butcherController.transform.position;
        float distance = Vector3.Distance(fromPos, butcherController.m_butcherJump.m_targetJumpPos);
        return distance;
    }

}
