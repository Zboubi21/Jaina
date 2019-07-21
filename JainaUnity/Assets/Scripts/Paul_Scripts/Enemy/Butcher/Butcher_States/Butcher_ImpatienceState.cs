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

        if(butcherController == null)
            butcherController = m_enemyController.GetComponent<ButcherController>();

        butcherController.IsImpatience = true;

        StateAnimation(m_enemyController.Anim);

        // Destination();
        // FaceTarget();

        // sign = m_enemyController.InstantiateObjects(butcherController.signImpatience, m_enemyController.TargetStats1.GetComponent<CharacterStats>().transform.position, Quaternion.identity);
        sign = m_enemyController.InstantiateObjects(butcherController.m_signImpatienceFx, butcherController.m_butcherJump.m_targetJumpPos, butcherController.m_signImpatienceFx.transform.rotation);

        target = sign.transform;

        // butcherController.StartCoroutine(CheckJumpArea(target.position));
        butcherController.StartCoroutine(RotateBeforeJump(butcherController.m_butcherJump.m_targetJumpPos));

        // m_enemyController.Agent.speed += 10;

        butcherController.NbrJump++;
        //Debug.Log(butcherController.NbrJump);

        butcherController.ImpatienceSign.gameObject.SetActive(true);
        butcherController.ImpatienceSign.StartParticle();

    }

    public override void FixedUpdate()
    {
        //butcherController.TranslateMove(target);
        // butcherController.FaceTarget(target);
    }

    public override void Update()
    {
        // GetOutOfState();
    }

    public override void Exit()
    {
        DestroySign();
        m_enemyController.Agent.enabled = true;
        // m_enemyController.Agent.speed -= 10;
        //m_enemyController.Agent.enabled = true;
        butcherController.TempsJumpAnim = butcherController.AnimTime;
        butcherController.ImpatienceSign.gameObject.SetActive(false);
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

    IEnumerator RotateBeforeJump(Vector3 targetPos){

        // Debug.Log("Start RotateCorout");
        Quaternion fromRot = butcherController.transform.rotation;

        Vector3 direction = (targetPos - butcherController.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        float distance = Quaternion.Dot(fromRot, lookRotation);
        distance = Mathf.Abs(distance);

		float moveFracJourney = new float();
		float vitesse = distance / butcherController.m_butcherJump.m_timeToDoRotation;

        // Debug.Log("fromRot = " + fromRot + " | lookRotation = " + lookRotation);

        bool isArrive = false;

		// while(butcherController.transform.rotation != lookRotation){
		while(!isArrive){
            // Debug.Log("Calcul RotateCorout");
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			Quaternion qua = Quaternion.Lerp(fromRot, lookRotation, butcherController.m_butcherJump.m_rotationCurve.Evaluate(moveFracJourney));
            // Debug.Log("qua = " + qua);
			butcherController.transform.rotation = qua;

            Quaternion actualRotAbs = new Quaternion(Mathf.Abs(butcherController.transform.rotation.x), Mathf.Abs(butcherController.transform.rotation.y), Mathf.Abs(butcherController.transform.rotation.z), Mathf.Abs(butcherController.transform.rotation.w));
            Quaternion targetRotAbs = new Quaternion(Mathf.Abs(lookRotation.x), Mathf.Abs(lookRotation.y), Mathf.Abs(lookRotation.z), Mathf.Abs(lookRotation.w));
            if(actualRotAbs == targetRotAbs){
                isArrive = true;
            }

			yield return null;
		}
        // Debug.Log("End RotateCorout");
        butcherController.StartCoroutine(JumpCoroutine(targetPos));
    }

    IEnumerator JumpCoroutine(Vector3 targetPos){

        // Debug.Log("Start JumpCorout");
        Vector3 fromPos = butcherController.transform.position;
        float distance = Vector3.Distance(fromPos, targetPos);
		float moveFracJourney = new float();
		float vitesse = distance / butcherController.m_butcherJump.m_timeToDoJump;

        if(distance < 1){
            butcherController.Anim.SetTrigger("ImpatienceEnd");
        }else{
            butcherController.Anim.SetTrigger("ImpatienceMiddle");
        }

        bool endAnim = false;
        bool doDamage = true;

        bool disableCollider = false;
        bool enableCollider = false;

        bool doShake = false;

		while(butcherController.transform.position != targetPos){
            // Debug.Log("Calcul JumpCorout");
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			butcherController.transform.position = Vector3.Lerp(fromPos, targetPos, butcherController.m_butcherJump.m_jumpCurve.Evaluate(moveFracJourney));

            if(moveFracJourney >= butcherController.m_butcherJump.m_timeToStartEndJumpAnim && !endAnim){
                endAnim = true;
                butcherController.Anim.SetTrigger("ImpatienceEnd");
            }

            if(moveFracJourney > butcherController.m_butcherJump.m_timeToDoDamage && doDamage){
                doDamage = false;
                butcherController.OnImpactDamage();
                m_enemyController.InstantiateObjects(butcherController.m_impactJumpFx, butcherController.m_butcherJump.m_targetJumpPos, butcherController.m_impactJumpFx.transform.rotation);
            }

            if(moveFracJourney > butcherController.m_butcherJump.m_timeToDisableCollider && !disableCollider){
                disableCollider = true;
                butcherController.Mycollider.enabled = false;
            }
            if(moveFracJourney > butcherController.m_butcherJump.m_timeToEnabelCollider && !enableCollider){
                enableCollider = true;
                butcherController.Mycollider.enabled = true;
            }

            if(moveFracJourney > butcherController.m_butcherJump.m_cameraShake.m_timeToShake && !doShake){
                doShake = true;
                if(butcherController.m_butcherJump.m_cameraShake.m_useShakeCam){
                    butcherController.ShakeCamera(butcherController.m_butcherJump.m_cameraShake.m_magnitudeShake, butcherController.m_butcherJump.m_cameraShake.m_roughnessShake, butcherController.m_butcherJump.m_cameraShake.m_fadeInTimeShake, butcherController.m_butcherJump.m_cameraShake.m_fadeOutTimeShake);
                }
            }

			yield return null;
		}
        // Debug.Log("End JumpCorout");
        butcherController.StartCoroutine(EndStateCorout());
    }

    IEnumerator EndStateCorout(){
        yield return new WaitForSeconds(butcherController.m_butcherJump.m_timeToExitStateAfterJump);
        if(butcherController.NbrJump == butcherController.numberOfJump)
        {
            m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ChaseState);       //Chase
            butcherController.NbrJump = 0;
        }
        else
        {
            butcherController.StartCheckJumpArea();
            // m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ImpatienceState);  //Impatience
        }
    }

}
