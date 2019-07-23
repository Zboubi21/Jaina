using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum_Butcher;
using PoolTypes;

public class ButcherController : EnemyController
{
    [Header("Warboar Impatience Variables")]
    public int numberOfJump;
    int nbrJump;
    public float CoolDownGettingImpatient = 2f;
    [Space]
    public GameObject m_signImpatienceFx;
    public GameObject m_impactJumpFx;
    [Space]
    [Space]
    public int impactDamage;
    public float rangeImpact;
    public Color impactRangeColor;
    [Space]
    [Space]
    public float rangeMinForJump;
    public Color minRangeColor;
    [Space]
    public float rangeMaxForJump;
    public Color maxRangeColor;
    [Space]
    public float m_tempsJumpAnim = 2f;

    [Header("Butcher jump")]
    public ButcherJump m_butcherJump = new ButcherJump();
	[System.Serializable] public class ButcherJump {
        [Header("Rotate before jump")]
		public float m_timeToDoRotation = 0.25f;
        public AnimationCurve m_rotationCurve;

        [Header("Jump")]
		public float m_timeToDoJump = 1;
        public float m_miniJumpDistance = 1;
        public AnimationCurve m_jumpCurve;
        [Range(0,1)] public float m_timeToStartEndJumpAnim = 0.75f;
        [Range(0,1)] public float m_timeToDoDamage = 0.8f;

        [Header("Additional forward")]
        public float m_additionalForwardJainaNotMoving = 0.5f;
        public float m_additionalForwardJainaMoving = 2;

        [Space]
        public float m_timeToExitStateAfterJump = 1;

        [Header("Collider")]
        [Range(0,1)] public float m_timeToDisableCollider = 0.1f;
        [Range(0,1)] public float m_timeToEnabelCollider = 0.9f;

        [Header("FX")]
        [Range(0,1)] public float m_timeToDestroyJumpSign = 0.75f;

        [Header("Check area")]
        public float m_maxRangeToJumpInNewArea = 0.5f;
        public Color m_maxRangeColor = Color.black;
        public ObjectType m_butcherCheckArea = ObjectType.ButcherCheckArea;
        [HideInInspector] public NavMeshAgent m_butcherCheckAreaAgent;
        [HideInInspector] public bool m_checkArea = false;
        [HideInInspector] public Vector3 m_targetJumpPos;

        [Header("Camera shake")]
        public Camerashake m_cameraShake = new Camerashake();
        [System.Serializable] public class Camerashake {
            public bool m_useShakeCam = true;
            [Range(0,1)] public float m_timeToShake = 0.9f;
            public float m_magnitudeShake = 2f;
            public float m_roughnessShake = 2f;
            public float m_fadeInTimeShake = 0.1f;
            public float m_fadeOutTimeShake = 0.15f;
        }
	}    

    float m_animTime;
    float m_cdImpatient;
    bool isImpatience;

#region Get Set
    public float CdImpatient
    {
        get
        {
            return m_cdImpatient;
        }

        set
        {
            m_cdImpatient = value;
        }
    }

    public int NbrJump
    {
        get
        {
            return nbrJump;
        }

        set
        {
            nbrJump = value;
        }
    }

    public float TempsJumpAnim
    {
        get
        {
            return m_tempsJumpAnim;
        }

        set
        {
            m_tempsJumpAnim = value;
        }
    }

    public bool IsImpatience
    {
        get
        {
            return isImpatience;
        }

        set
        {
            isImpatience = value;
        }
    }

    public float AnimTime
    {
        get
        {
            return m_animTime;
        }

        set
        {
            m_animTime = value;
        }
    }

    IEnumerator m_checkJumpAreaCorout;
    public IEnumerator CheckJumpAreaCorout{
        get{
            return m_checkJumpAreaCorout;
        }
        set{
            m_checkJumpAreaCorout = value;
        }
    }
    IEnumerator m_rotateCorout;
    public IEnumerator RotateCorout{
        get{
            return m_rotateCorout;
        }
        set{
            m_rotateCorout = value;
        }
    }
    IEnumerator m_jumpCorout;
    public IEnumerator JumpCorout{
        get{
            return m_jumpCorout;
        }
        set{
            m_jumpCorout = value;
        }
    }
    IEnumerator m_exitStateCorout;
    public IEnumerator ExitStateCorout{
        get{
            return m_exitStateCorout;
        }
        set{
            m_exitStateCorout = value;
        }
    }
    GameObject m_jumpSign;
    public GameObject JumpSign{
        get{
            return m_jumpSign;
        }
        set{
            m_jumpSign = value;
        }
    }
    bool m_targetIsClosed;
    public bool TargetIsClosed{
        get{
            return m_targetIsClosed;
        }
        set{
            m_targetIsClosed = value;
        }
    }
    
#endregion
    public override void LogicAtStart()
    {
        base.LogicAtStart();

        m_cdImpatient = CoolDownGettingImpatient;
        m_animTime = m_tempsJumpAnim;
    }

    private void Awake()
    {
        m_sM.AddStates(new List<IState> {
            new Butcher_IdleState(this),                    // Numéro 0
            new Butcher_StunState(this),                    // Numéro 1
            new Butcher_AlerteState(this),                  // Numéro 2
            new Butcher_ChaseState(this),                   // Numéro 3
            new Butcher_ImpatienceState(this),              // Numéro 4
            new Butcher_AttackState(this),                  // Numéro 5
            new FrozenState(this),                          // Numéro 6
            new DieState(this),                             // Numéro 7
            new VictoryState(this),                         // Numéro 8
            new CinematicState(this),                       // Numéro 9
		});
        string[] enemyStateNames = System.Enum.GetNames(typeof(EnemyButcherState));
        if (m_sM.States.Count != enemyStateNames.Length)
        {
            Debug.LogError("You need to have the same number of State in ButcherController and EnemyStateEnum");
        }
    }

    public override bool TargetInImpatienceDonuts()
    {
        if (GetTargetDistance(TargetStats1.gameObject.transform) <= rangeMaxForJump && GetTargetDistance(TargetStats1.gameObject.transform) >= rangeMinForJump)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool CheckIfAnimEnded()
    {
        m_tempsJumpAnim -= Time.deltaTime;
        if (m_tempsJumpAnim <= 0)
        {
            return true;
        }
        return false;
    }

    

    public override bool CoolDownImpatience()
    {
        m_cdImpatient -= Time.deltaTime;
        if(m_cdImpatient <= 0)
        {
            return true;
        }
        return false;
    }

    public override void Timed()
    {
        if (CheckCollision(RedBoxScale, RedBoxPosition) != null)
        {
            TargetStats1.TakeDamage(MyStas.damage.GetValue());
        }
    }

    public override void Attack()
    {
        Anim.SetTrigger("Attack");
    }

    public override void OnImpactDamage()
    {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, rangeImpact);
        for (int i = 0, l = hitCollider.Length; i < l; ++i)
        {
            if (hitCollider[i].CompareTag("Player"))
            {
                hitCollider[i].GetComponent<CharacterStats>().TakeDamage(impactDamage);
                break;
            }
        }
    }

    public override void OnChangeToStunState()
    {
        if (!MyStas.IsDead && !isImpatience)
        {
            ChangeState((int)EnemyButcherState.Butcher_StunState);
        }
    }

    public override void Freeze()
    {
        if (!isImpatience)
        {
            base.Freeze();
        }
    }

    public override void TranslateMove(Transform target)
    {
        transform.Translate(target.localPosition);
    }
    public override void FaceTarget(Transform target)
    {
        Vector3 direction = (target.localPosition - transform.localPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public override void OnDrawGizmosSelected()
    {
        Gizmos.color = minRangeColor;
        Gizmos.DrawWireSphere(transform.position, rangeMinForJump);

        Gizmos.color = maxRangeColor;
        Gizmos.DrawWireSphere(transform.position, rangeMaxForJump);

        Gizmos.color = impactRangeColor;
        Gizmos.DrawWireSphere(transform.position, rangeImpact);

        if(m_butcherJump.m_butcherCheckAreaAgent != null){
            Gizmos.color = m_butcherJump.m_maxRangeColor;
            Gizmos.DrawWireSphere(m_butcherJump.m_butcherCheckAreaAgent.transform.position, m_butcherJump.m_maxRangeToJumpInNewArea);
        }
    }

    public void StartCheckJumpArea(){
        m_checkJumpAreaCorout = CheckJumpArea(TargetStats1.GetComponent<CharacterStats>().transform);
        StartCoroutine(m_checkJumpAreaCorout);
    }
    IEnumerator CheckJumpArea(Transform targetPos) {
        // Debug.LogError("CheckJumpArea");
        
        GameObject butcherCheckArea = ObjectPooler.SpawnObjectFromPool(m_butcherJump.m_butcherCheckArea, Vector3.zero, Quaternion.identity);
        m_butcherJump.m_butcherCheckAreaAgent = butcherCheckArea.GetComponent<NavMeshAgent>();

        Vector3 targetPosition;
        if(PlayerManager.IsMoving){
            targetPosition = targetPos.position + targetPos.forward * m_butcherJump.m_additionalForwardJainaMoving;
        }else{
            targetPosition = targetPos.position + targetPos.forward * m_butcherJump.m_additionalForwardJainaNotMoving;
        }

        m_butcherJump.m_butcherCheckAreaAgent.Warp(targetPosition);
        yield return new WaitForSeconds(0.05f); 
        
        // Debug.Log("targetPos = " + targetPos);
        // Debug.Log("m_butcherCheckAreaPos = " + m_butcherJump.m_butcherCheckArea.transform.position);

        float distance = Vector3.Distance(targetPosition, m_butcherJump.m_butcherCheckAreaAgent.transform.position);

        if(distance < m_butcherJump.m_maxRangeToJumpInNewArea){
            m_butcherJump.m_targetJumpPos = m_butcherJump.m_butcherCheckAreaAgent.transform.position;
            ChangeState((int)EnemyButcherState.Butcher_ImpatienceState);
        }else{
            if(m_sM.CurrentStateIndex == (int)EnemyButcherState.Butcher_ImpatienceState){
                ChangeState((int)EnemyButcherState.Butcher_ChaseState);
                NbrJump = 0;
            }
        }
        m_butcherJump.m_checkArea = false;
        ObjectPooler.ReturnObjectToPool(m_butcherJump.m_butcherCheckArea, butcherCheckArea);
    }

    public IEnumerator RotateBeforeJump(Vector3 targetPos, bool targetIsClosed = false,  bool isCinematic = false){

        if(targetIsClosed){
            yield return new WaitForSeconds(m_butcherJump.m_timeToDoRotation);
            m_jumpCorout = JumpCoroutine(targetPos, m_targetIsClosed);
            StartCoroutine(m_jumpCorout);
        }else{
            // Debug.Log("Start RotateCorout");
            Quaternion fromRot = transform.rotation;

            Vector3 direction = (targetPos - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            float distance = Quaternion.Dot(fromRot, lookRotation);
            distance = Mathf.Abs(distance);

            float moveFracJourney = new float();
            float vitesse = distance / m_butcherJump.m_timeToDoRotation;

            // Debug.Log("fromRot = " + fromRot + " | lookRotation = " + lookRotation);

            bool isArrive = false;

            // while(butcherController.transform.rotation != lookRotation){
            while(!isArrive){
                // Debug.Log("Calcul RotateCorout");
                moveFracJourney += (Time.deltaTime) * vitesse / distance;
                // Debug.Log("moveFracJourney = " + moveFracJourney);
                Quaternion qua = Quaternion.Lerp(fromRot, lookRotation, m_butcherJump.m_rotationCurve.Evaluate(moveFracJourney));
                // Debug.Log("qua = " + qua);
                transform.rotation = qua;

                Quaternion actualRotAbs = new Quaternion(Mathf.Abs(transform.rotation.x), Mathf.Abs(transform.rotation.y), Mathf.Abs(transform.rotation.z), Mathf.Abs(transform.rotation.w));
                Quaternion targetRotAbs = new Quaternion(Mathf.Abs(lookRotation.x), Mathf.Abs(lookRotation.y), Mathf.Abs(lookRotation.z), Mathf.Abs(lookRotation.w));
                if(actualRotAbs == targetRotAbs){
                    isArrive = true;
                }

                yield return null;
            }
            // Debug.Log("End RotateCorout");
            m_jumpCorout = JumpCoroutine(targetPos, m_targetIsClosed);
            StartCoroutine(m_jumpCorout);
        }

    }
    public IEnumerator JumpCoroutine(Vector3 targetPos, bool targetIsClosed = false){

        // Debug.Log("Start JumpCorout");
        Vector3 fromPos = transform.position;
        float distance = Vector3.Distance(fromPos, targetPos);
		float moveFracJourney = new float();
		float vitesse = distance / m_butcherJump.m_timeToDoJump;

        if(targetIsClosed){
            yield return new WaitForSeconds(0.25f);
            // butcherController.Anim.SetTrigger("ImpatienceEnd");
            Anim.SetTrigger("ImpatienceEndSlow");
            yield return new WaitForSeconds(0.75f);
            DestroyButcherJumpSign();
        }else{
            Anim.SetTrigger("ImpatienceMiddle");
        }

        bool endAnim = false;
        bool damageDone = false;

        bool disableCollider = false;
        bool enableCollider = false;

        bool destroyJumpSign = false;

        bool doShake = false;

		while(transform.position != targetPos){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			transform.position = Vector3.Lerp(fromPos, targetPos, m_butcherJump.m_jumpCurve.Evaluate(moveFracJourney));

            if(moveFracJourney >= m_butcherJump.m_timeToStartEndJumpAnim && !endAnim){
                endAnim = true;
                Anim.SetTrigger("ImpatienceEnd");
            }

            // if(moveFracJourney > butcherController.m_butcherJump.m_timeToDoDamage && !damageDone){
            //     damageDone = true;
            //     butcherController.OnImpactDamage();
            //     m_enemyController.InstantiateObjects(butcherController.m_impactJumpFx, butcherController.m_butcherJump.m_targetJumpPos, butcherController.m_impactJumpFx.transform.rotation);
            // }

            if(moveFracJourney > m_butcherJump.m_timeToDisableCollider && !disableCollider){
                disableCollider = true;
                Mycollider.enabled = false;
            }
            if(moveFracJourney > m_butcherJump.m_timeToEnabelCollider && !enableCollider){
                enableCollider = true;
                Mycollider.enabled = true;
            }

            if(moveFracJourney > m_butcherJump.m_timeToDestroyJumpSign && !destroyJumpSign){
                destroyJumpSign = true;
                DestroyButcherJumpSign();
            }

            if(moveFracJourney > m_butcherJump.m_cameraShake.m_timeToShake && !doShake){
                doShake = true;
                if(m_butcherJump.m_cameraShake.m_useShakeCam){
                    ShakeCamera(m_butcherJump.m_cameraShake.m_magnitudeShake, m_butcherJump.m_cameraShake.m_roughnessShake, m_butcherJump.m_cameraShake.m_fadeInTimeShake, m_butcherJump.m_cameraShake.m_fadeOutTimeShake);
                }
            }
			yield return null;
		}
        if(!destroyJumpSign){
            destroyJumpSign = true;
            DestroyButcherJumpSign();
        }
        if(!damageDone){
            damageDone = true;
            OnImpactDamage();
            InstantiateObjects(m_impactJumpFx, m_butcherJump.m_targetJumpPos, m_impactJumpFx.transform.rotation);
        }
        // Debug.Log("End JumpCorout");
        m_exitStateCorout = EndStateCorout();
        StartCoroutine(m_exitStateCorout);
    }
    public IEnumerator EndStateCorout(){
        yield return new WaitForSeconds(m_butcherJump.m_timeToExitStateAfterJump);
        if(NbrJump == numberOfJump)
        {
            ChangeState((int)EnemyButcherState.Butcher_ChaseState);       //Chase
            NbrJump = 0;
        }
        else
        {
            StartCheckJumpArea();
            // m_enemyController.ChangeState((int)EnemyButcherState.Butcher_ImpatienceState);  //Impatience
        }
    }

    public void DestroyButcherJumpSign(){
        if(m_jumpSign != null){
            DestroyGameObject(m_jumpSign);
        }
    }

    public void StopAllButcherCoroutines(){
        if(CheckJumpAreaCorout != null){
            StopCoroutine(CheckJumpAreaCorout);
        }
        if(RotateCorout != null){
            StopCoroutine(RotateCorout);
        }
        if(JumpCorout != null){
            StopCoroutine(JumpCorout);
        }
        if(ExitStateCorout != null){
            StopCoroutine(ExitStateCorout);
        }
    }

    public void CinematicJumpOnTarget(Transform targetPos){
        NbrJump = numberOfJump - 1;
        m_butcherJump.m_targetJumpPos = targetPos.position;
        ChangeState((int)EnemyButcherState.Butcher_ImpatienceState);
    }

}
