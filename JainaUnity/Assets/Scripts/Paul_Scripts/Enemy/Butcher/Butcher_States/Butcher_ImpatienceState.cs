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
        butcherController.StopAllCoroutines();
        DestroySign();
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

    // IEnumerator RotateBeforeJump(Vector3 targetPos){

    //     if(m_targetIsClosed){
    //         yield return new WaitForSeconds(butcherController.m_butcherJump.m_timeToDoRotation);
    //         m_jumpCorout = JumpCoroutine(targetPos);
    //         butcherController.StartCoroutine(m_jumpCorout);
    //     }else{
    //         // Debug.Log("Start RotateCorout");
    //         Quaternion fromRot = butcherController.transform.rotation;

    //         Vector3 direction = (targetPos - butcherController.transform.position).normalized;
    //         Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

    //         float distance = Quaternion.Dot(fromRot, lookRotation);
    //         distance = Mathf.Abs(distance);

    //         float moveFracJourney = new float();
    //         float vitesse = distance / butcherController.m_butcherJump.m_timeToDoRotation;

    //         // Debug.Log("fromRot = " + fromRot + " | lookRotation = " + lookRotation);

    //         bool isArrive = false;

    //         // while(butcherController.transform.rotation != lookRotation){
    //         while(!isArrive){
    //             // Debug.Log("Calcul RotateCorout");
    //             moveFracJourney += (Time.deltaTime) * vitesse / distance;
    //             // Debug.Log("moveFracJourney = " + moveFracJourney);
    //             Quaternion qua = Quaternion.Lerp(fromRot, lookRotation, butcherController.m_butcherJump.m_rotationCurve.Evaluate(moveFracJourney));
    //             // Debug.Log("qua = " + qua);
    //             butcherController.transform.rotation = qua;

    //             Quaternion actualRotAbs = new Quaternion(Mathf.Abs(butcherController.transform.rotation.x), Mathf.Abs(butcherController.transform.rotation.y), Mathf.Abs(butcherController.transform.rotation.z), Mathf.Abs(butcherController.transform.rotation.w));
    //             Quaternion targetRotAbs = new Quaternion(Mathf.Abs(lookRotation.x), Mathf.Abs(lookRotation.y), Mathf.Abs(lookRotation.z), Mathf.Abs(lookRotation.w));
    //             if(actualRotAbs == targetRotAbs){
    //                 isArrive = true;
    //             }

    //             yield return null;
    //         }
    //         // Debug.Log("End RotateCorout");
    //         m_jumpCorout = JumpCoroutine(targetPos);
    //         butcherController.StartCoroutine(m_jumpCorout);
    //     }

    // }
    // IEnumerator JumpCoroutine(Vector3 targetPos){

    //     // Debug.Log("Start JumpCorout");
    //     Vector3 fromPos = butcherController.transform.position;
    //     float distance = Vector3.Distance(fromPos, targetPos);
	// 	float moveFracJourney = new float();
	// 	float vitesse = distance / butcherController.m_butcherJump.m_timeToDoJump;

    //     if(m_targetIsClosed){
    //         yield return new WaitForSeconds(0.25f);
    //         // butcherController.Anim.SetTrigger("ImpatienceEnd");
    //         butcherController.Anim.SetTrigger("ImpatienceEndSlow");
    //         yield return new WaitForSeconds(0.75f);
    //         DestroySign();
    //     }else{
    //         butcherController.Anim.SetTrigger("ImpatienceMiddle");
    //     }

    //     bool endAnim = false;
    //     bool damageDone = false;

    //     bool disableCollider = false;
    //     bool enableCollider = false;

    //     bool destroyJumpSign = false;

    //     bool doShake = false;

	// 	while(butcherController.transform.position != targetPos){
	// 		moveFracJourney += (Time.deltaTime) * vitesse / distance;
	// 		butcherController.transform.position = Vector3.Lerp(fromPos, targetPos, butcherController.m_butcherJump.m_jumpCurve.Evaluate(moveFracJourney));

    //         if(moveFracJourney >= butcherController.m_butcherJump.m_timeToStartEndJumpAnim && !endAnim){
    //             endAnim = true;
    //             butcherController.Anim.SetTrigger("ImpatienceEnd");
    //         }

    //         // if(moveFracJourney > butcherController.m_butcherJump.m_timeToDoDamage && !damageDone){
    //         //     damageDone = true;
    //         //     butcherController.OnImpactDamage();
    //         //     m_enemyController.InstantiateObjects(butcherController.m_impactJumpFx, butcherController.m_butcherJump.m_targetJumpPos, butcherController.m_impactJumpFx.transform.rotation);
    //         // }

    //         if(moveFracJourney > butcherController.m_butcherJump.m_timeToDisableCollider && !disableCollider){
    //             disableCollider = true;
    //             butcherController.Mycollider.enabled = false;
    //         }
    //         if(moveFracJourney > butcherController.m_butcherJump.m_timeToEnabelCollider && !enableCollider){
    //             enableCollider = true;
    //             butcherController.Mycollider.enabled = true;
    //         }

    //         if(moveFracJourney > butcherController.m_butcherJump.m_timeToDestroyJumpSign && !destroyJumpSign){
    //             destroyJumpSign = true;
    //             DestroySign();
    //         }

    //         if(moveFracJourney > butcherController.m_butcherJump.m_cameraShake.m_timeToShake && !doShake){
    //             doShake = true;
    //             if(butcherController.m_butcherJump.m_cameraShake.m_useShakeCam){
    //                 butcherController.ShakeCamera(butcherController.m_butcherJump.m_cameraShake.m_magnitudeShake, butcherController.m_butcherJump.m_cameraShake.m_roughnessShake, butcherController.m_butcherJump.m_cameraShake.m_fadeInTimeShake, butcherController.m_butcherJump.m_cameraShake.m_fadeOutTimeShake);
    //             }
    //         }
	// 		yield return null;
	// 	}
    //     if(!destroyJumpSign){
    //         destroyJumpSign = true;
    //         DestroySign();
    //     }
    //     if(!damageDone){
    //         damageDone = true;
    //         butcherController.OnImpactDamage();
    //         m_enemyController.InstantiateObjects(butcherController.m_impactJumpFx, butcherController.m_butcherJump.m_targetJumpPos, butcherController.m_impactJumpFx.transform.rotation);
    //     }
    //     // Debug.Log("End JumpCorout");
    //     m_exitStateCorout = EndStateCorout();
    //     butcherController.StartCoroutine(m_exitStateCorout);
    // }
    // IEnumerator EndStateCorout(){
    //     yield return new WaitForSeconds(butcherController.m_butcherJump.m_timeToExitStateAfterJump);
    //     if(butcherController.NbrJump == butcherController.numberOfJump)
    //     {
    //         m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ChaseState);       //Chase
    //         butcherController.NbrJump = 0;
    //     }
    //     else
    //     {
    //         butcherController.StartCheckJumpArea();
    //         // m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ImpatienceState);  //Impatience
    //     }
    // }

    float GetDistanceFromTarget(){
        Vector3 fromPos = butcherController.transform.position;
        float distance = Vector3.Distance(fromPos, butcherController.m_butcherJump.m_targetJumpPos);
        return distance;
    }

}
