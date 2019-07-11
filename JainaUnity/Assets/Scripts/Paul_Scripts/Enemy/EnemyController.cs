using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum;
using PoolTypes;

public class EnemyController : MonoBehaviour {

    [SerializeField] bool m_isInstatiate = true;
    [SerializeField] EnemyType m_enemyType;

    #region State Machine

    public StateMachine m_sM = new StateMachine();

    public virtual void OnEnable()
    {

        if(Anim == null)
            Anim = GetComponentInChildren<Animator>();

        if(!m_isInstatiate){
            ChangeState(EnemyState.IdleState);
            m_hasBeenOnAlert = false;
        }
        else
        {
            ChangeState(EnemyState.ChaseState);
            LogicAtStart();
            LogicWhenEnable();
            m_hasBeenOnAlert = true;
        }
    }

    public void ChangeState(EnemyState newEnemyState)
    {
        m_sM.ChangeState((int)newEnemyState);
    }

    public int GetLastStateIndex()
    {
        return m_sM.LastStateIndex;
    }
    [Space]

    #endregion

    [Header("Radius Variable")]
    public float lookRadius = 10f;
    public float maxLookRadiusOnAlert = 50f;
    public float yellRadius = 5f;
    [Range(0.1f, 5f)]
    public float RadiusExpansionSpeed = 1;
    float smoothRot = 5f;

    #region Alert Var
    bool m_hasBeenOnAlert;
    float StartlookRadius;
    float alertExpensionSpeed;
    #endregion

    #region Yell Var
    List<Collider> enemy = new List<Collider>();
    bool beenYelled = false;
    #endregion

    [Space]
    [Header("RedBoxCollison")]
    public Vector3 RedBoxPosition;
    public Vector3 RedBoxScale;
    [Header("GreenBoxCollison")]
    public Vector3 GreenBoxPosition;
    public Vector3 GreenBoxScale;

    

    [Space]
    [Header("Attack Variable")]
    public float attackSpeed = 1f;
    float attackCooldown = 0f;
    bool canAttack = true;
    bool CheckAnimEnd = false;
    float attackImpatience = 3f;
    float currentImpatience;
    [Header("Stuned when attack")]
    public float timerBeforeGettingStunable;
    public float timeBeingStuned;
    //float saveTimeBeforeGettingStunable;
    //float saveTimerAttackedFreeze;
    [Header("Impatience Sign")]
    public StartParticles ImpatienceSign;
    bool m_isImpatient;
    float agentSpeed;
    [Header("Impatience Sprint")]
    float currentTimeBeforeGettingImpatient;
    float currentTimeBeforeGettingImpatientWhenInAttackRange;
    public float speedSprint = 15f;
    public float TimeBeforeGettingImpatient = 3f;
    bool beingAttacked;
    bool beingStunable = true;
    bool hasBeenAttacked;
    bool isStun;

    [Header("Detected FX")]
    public float m_timeToShowDetectedFx = 2;
    public StartParticles m_detectedFx;

    [Space]
    [Header("Debug AI Comportement")]
    public bool TestIAcomportement;

    [Header("FX")]
    public FXs m_fxs = new FXs();
    [System.Serializable] public class FXs {
        public GameObject m_freezed;
        public GameObject m_markExplosion;
        public Transform m_markExplosionRoot;
    }

    Transform target;
    NavMeshAgent agent;
    Animator anim;
    CharacterStats TargetStats;
    CharacterStats myStats;
    Collider Mycollider;
    Collider[] enemyController;
    PlayerManager m_playerManager;
    bool m_enemyIsInVictory = false;

#region Get Set
    RuntimeAnimatorController rac;
    public RuntimeAnimatorController Rac
    {
        get
        {
            return rac;
        }

        set
        {
            rac = value;
        }
    }

    CharacterStats myStas;
    public CharacterStats MyStas
    {
        get
        {
            return myStas;
        }

        set
        {
            myStas = value;
        }
    }

    bool AnimEnd;
    public bool AnimEnd1
    {
        get
        {
            return AnimEnd;
        }

        set
        {
            AnimEnd = value;
        }
    }

    public CharacterStats TargetStats1
    {
        get
        {
            return TargetStats;
        }

        set
        {
            TargetStats = value;
        }
    }

    public float AttackImpatience
    {
        get
        {
            return attackImpatience;
        }

        set
        {
            attackImpatience = value;
        }
    }

    public float AttackCooldown
    {
        get
        {
            return attackCooldown;
        }

        set
        {
            attackCooldown = value;
        }
    }

    public bool CheckAnimEnd1
    {
        get
        {
            return CheckAnimEnd;
        }

        set
        {
            CheckAnimEnd = value;
        }
    }

    public bool CanAttack
    {
        get
        {
            return canAttack;
        }

        set
        {
            canAttack = value;
        }
    }

    public Animator Anim
    {
        get
        {
            return anim;
        }

        set
        {
            anim = value;
        }
    }

    public bool BeenYelled
    {
        get
        {
            return beenYelled;
        }

        set
        {
            beenYelled = value;
        }
    }

    public List<Collider> Enemy
    {
        get
        {
            return enemy;
        }

        set
        {
            enemy = value;
        }
    }

    public float SaveCurrentTimeBeforeGettingImpatient
    {
        get
        {
            return TimeBeforeGettingImpatient;
        }

        set
        {
            TimeBeforeGettingImpatient = value;
        }
    }

    public float AgentSpeed
    {
        get
        {
            return agentSpeed;
        }

        set
        {
            agentSpeed = value;
        }
    }

    public bool IsRootByIceNova
    {
        get
        {
            return m_isRootByIceNova;
        }

        set
        {
            m_isRootByIceNova = value;
        }
    }

    public bool IsImpatient
    {
        get
        {
            return m_isImpatient;
        }

        set
        {
            m_isImpatient = value;
        }
    }

    public GameObject FreezedObject
    {
        get
        {
            return freezedObject;
        }

        set
        {
            freezedObject = value;
        }
    }

    public bool HasBeenOnAlert
    {
        get
        {
            return m_hasBeenOnAlert;
        }

        set
        {
            m_hasBeenOnAlert = value;
        }
    }
    #endregion

    public virtual void LogicWhenEnable()
    {
        myStats = GetComponent<CharacterStats>();
        myStats.CurrentHealth = myStas.maxHealth;
        Mycollider.enabled = true;
        agent.enabled = true;
        myStats.IsDead = false;
    }

    public virtual void LogicAtStart()
    {
        if (m_playerManager == null)
        {
            m_playerManager = PlayerManager.Instance;
        }
        m_enemyIsInVictory = false;
        // Get Instance Of The Player and his CharacterStats
        target = m_playerManager.gameObject.transform;
        TargetStats1 = target.GetComponent<CharacterStats>();

        // Get The EnnmyController's NavMeshAgent, CharacterStats and Collider
        agent = GetComponent<NavMeshAgent>();
        MyStas = GetComponent<CharacterStats>();
        Mycollider = gameObject.GetComponent<Collider>();

        //Get The Time Of The Animation Being Played
        Rac = Anim.runtimeAnimatorController;

        //Get Agent's Speed
        AgentSpeed = agent.speed;

        //Get Agent's LookRadius
        StartlookRadius = lookRadius;

        currentImpatience = AttackImpatience;
        currentTimeBeforeGettingImpatient = SaveCurrentTimeBeforeGettingImpatient;
        currentTimeBeforeGettingImpatientWhenInAttackRange = SaveCurrentTimeBeforeGettingImpatient;

        //saveTimeBeforeGettingStunable = timeBeingStuned;
        //saveTimerAttackedFreeze = timerAttackedFreeze;
        //Debug.Log(Rac.animationClips[0].length);
        //enemyController = GetComponents<EnemyController>();
    }
    EnemyStats enemystats;
    ObjectPooler m_objectPooler;
    
    public virtual void Start () {

        if (!m_isInstatiate)
        {
            LogicAtStart();
        }
        enemystats = GetComponent<EnemyStats>();
        m_objectPooler = ObjectPooler.Instance;

    }

    void Update () {

        m_sM.Update();
        if (IsRootByIceNova)
        {
            if (FreezTiming())
            {
                IsRootByIceNova = false;
                if(enemystats.CurrentHealth > 0)
                {
                    anim.SetTrigger("Chase");
                }
            }
        }
    }

    private void FixedUpdate()
    {
        m_sM.FixedUpdate();

        if(m_playerManager.PlayerIsDead && !m_enemyIsInVictory && !m_sM.CompareState((int)EnemyState.DieState)){
            m_enemyIsInVictory = true;
            ChangeState(EnemyState.VictoryState);
        }
    }
    

    public bool PlayerInLookRange()
    {
        if ((GetTargetDistance(target) <= lookRadius))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetTargetDistance(Transform target)
    {
        return Vector3.Distance(target.position, transform.position);
    }

    public void SetDestination()
    {
        agent.SetDestination(target.position);
    }

    public void StopMoving(bool b)
    {
        agent.isStopped = b;
    }

    public bool InAttackRange()
    {
        if (GetTargetDistance(target) <= agent.stoppingDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void IsAlert()
    {
        alertExpensionSpeed += (Time.deltaTime / (1 / RadiusExpansionSpeed));
        lookRadius = Mathf.Lerp(StartlookRadius, maxLookRadiusOnAlert, /*Mathf.PingPong(*/alertExpensionSpeed/*, 1)*/);
    }

    public void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smoothRot);
    }

    public void DestroyGameObject(GameObject obj)
    {
        Destroy(obj);
    }

    public bool CanAttackWhenImpatience() {

        currentImpatience -= Time.deltaTime;
        if (currentImpatience <= 0)
        {
            // _signImpatience = Instantiate(ImpatienceSign, ImpatienceSignRoot);
            // _signImpatience = InstantiateObjects(ImpatienceSign, ImpatienceSignRoot.position, ImpatienceSignRoot.rotation, ImpatienceSignRoot);
            ImpatienceSign.gameObject.SetActive(true);
            ImpatienceSign.StartParticle();
            //Debug.LogError("pause");
            currentImpatience = AttackImpatience;
            return true;
        }
        else if (!PlayerInAttackBox())
        {
            return false;
        }
        return false;
    }



    public bool PlayerInAttackBox()
    {
        if (CheckCollision(GreenBoxScale, GreenBoxPosition) != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void Attack()
    {
        Anim.SetTrigger("Attack");
    }

    public bool IsChasing()
    {
        currentTimeBeforeGettingImpatient -= Time.deltaTime;
        if(currentTimeBeforeGettingImpatient <= 0)
        {
            // _signImpatience = InstantiateObjects(ImpatienceSign, ImpatienceSignRoot.position, ImpatienceSignRoot.rotation, ImpatienceSignRoot);
            ImpatienceSign.gameObject.SetActive(true);
            ImpatienceSign.StartParticle();
            currentTimeBeforeGettingImpatient = SaveCurrentTimeBeforeGettingImpatient;
            return true;
        }
        return false;
    }
    public bool IsInAttackRangeForToLong()
    {
        currentTimeBeforeGettingImpatientWhenInAttackRange -= Time.deltaTime;
        if (currentTimeBeforeGettingImpatientWhenInAttackRange <= 0)
        {
            //_signImpatience = InstantiateObjects(ImpatienceSign, ImpatienceSignRoot.position, ImpatienceSignRoot.rotation, ImpatienceSignRoot);
            currentTimeBeforeGettingImpatientWhenInAttackRange = SaveCurrentTimeBeforeGettingImpatient;
            return true;
        }
        return false;
    }

    public virtual void Sprint(float speed)
    {
        speed = speed * ((100f - (m_playerManager.m_debuffs.m_IceSlow.m_iceSlow * enemystats.GivreMarkCount)) / 100f);
        agent.speed = speed;
    }

    public virtual void Timed()
    {
        
    }

    #region StunMethods

    public void CheckIfStunable()
    {
        AttackedStunCoolDownMethods();
    }

    void AttackedStunCoolDownMethods()
    {
        if (beingStunable)
        {
            OnChangeToStunState();
            StartCoroutine(CoolDownTimerBeforeGettingStunable());
        }else
        {
            // Debug.Log("J'ai pas a être stun moi pd de merde ");
        }
    }

    IEnumerator CoolDownTimerBeforeGettingStunable()
    {
        beingStunable = false;
        yield return new WaitForSeconds(timerBeforeGettingStunable);
        beingStunable = true;
    }

    public virtual void OnChangeToStunState()
    {
        if (!MyStas.IsDead)
        {
            ChangeState(EnemyState.StunState);
        }
    }

    #endregion

    #region IceNovaMethods
    public void Freeze()
    {
        if (!MyStas.IsDead)
        {
            //ChangeState(EnemyState.FrozenState);
            Anim.SetTrigger("Idle");
            FreezTime = m_playerManager.m_powers.m_iceNova.m_timeFreezed;
            // if (FreezedObject == null)
            // {
                // FreezedObject = InstantiateObjects(m_fxs.m_freezed, transform.position, transform.rotation, transform);
                m_fxs.m_freezed.SetActive(true);
            // }
            IsRootByIceNova = true;
            BeFreezed(IsRootByIceNova);
        }
    }
    bool m_isRootByIceNova;
    float FreezTime;
    GameObject freezedObject;

    public bool FreezTiming()
    {
        FreezTime -= Time.deltaTime;
        if (FreezTime <= 0)
        {
            IsRootByIceNova = false;
            if(enemystats.CurrentHealth > 0)
            {
                BeFreezed(IsRootByIceNova);
            }
            return true;
        }
        return false;
    }

    public virtual void BeFreezed(bool b)
    {
        StopMoving(b);
        if (!b)
        {
            // DestroyGameObject(FreezedObject);
            m_fxs.m_freezed.SetActive(false);
        }
    }

    #endregion

    #region CoolDownAttackSpeed
    public void StartAttackCoolDown()
    {
        StartCoroutine(CoolDownBeforeAttack());
    }

    IEnumerator CoolDownBeforeAttack()
    {
        CanAttack = false;
        yield return new WaitForSeconds(AttackCooldown);
        CanAttack = true;
        AttackCooldown = 1f / attackSpeed;
    }
    #endregion

    #region CoolDownEndAnimAttack
    public void AnimFinished()
    {
        StartCoroutine(AnimTime());
    }
    IEnumerator AnimTime()
    {
        //CheckAnimEnd1 = false;
        yield return new WaitForSeconds(Rac.animationClips[0].length);
        CheckAnimEnd1 = true;
    }
    #endregion

    #region Die Methods

    public void OnEnemyDie()
    {
        ChangeState(EnemyState.DieState); // Die
    }

    public virtual void Die()
    {
        // if(FreezedObject != null){
        //     Destroy(FreezedObject);
        // }
        m_fxs.m_freezed.SetActive(false);
        if (enemystats._hasBackPack)
        {
            MeshRenderer[] go = enemystats.m_backPack.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < go.Length; i++)
            {
                if(go[i] != enemystats.m_backPack.GetComponent<MeshRenderer>())
                {
                    go[i].gameObject.SetActive(false);
                }
            }
            m_objectPooler.SpawnObjectFromPool(enemystats.m_backPack.GetComponent<BackPack_Inventory>()._inventory[0], transform.position, transform.rotation);
        }
        StartCoroutine(OnWaitForAnimToEnd());
        //Destroy(GetComponent<CapsuleCollider>());
        // Destroy(gameObject, 3f);
        // ObjectPooler.Instance.ReturnToPool("Zglorg", gameObject);
    }

    IEnumerator OnWaitForAnimToEnd()
    {
        Mycollider.enabled = false;
        agent.enabled = false;
        yield return new WaitForSeconds(3f);                            //Animation time
        Spawned_Tracker spawnTracker = GetComponent<Spawned_Tracker>();
        if(spawnTracker != null)
        {
            spawnTracker.CallDead();
            Destroy(spawnTracker);
        }
        PoolTracker poolTracker = GetComponent<PoolTracker>();
        if(poolTracker != null)
        {
            Destroy(poolTracker);
        }
        if(m_isInstatiate){
            ObjectPooler.Instance.ReturnEnemyToPool(m_enemyType, gameObject);
        }
    }

    #endregion

    public void Yell(int CurrentState)
    {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, yellRadius);
        for (int i = 0, l = hitCollider.Length; i < l; ++i)
        {

            if (hitCollider[i].CompareTag("Enemy"))
            {
                if(hitCollider[i] != Mycollider)
                {
                    // Debug.Log("Le collider " + i + " est : " + hitCollider[i].name + " et sont BeenYelled est à "+ hitCollider[i].gameObject.GetComponent<EnemyController>().BeenYelled);
                    Enemy.Add(hitCollider[i]);
                    if (!hitCollider[i].gameObject.GetComponent<EnemyController>().BeenYelled)
                    {
                        hitCollider[i].gameObject.GetComponent<EnemyController>().ChangeState((EnemyState)CurrentState);
                    }
                }
            }
        }
        BeenYelled = true;
    }

    public Collider CheckCollision(Vector3 boxScale, Vector3 BoxPosition)
    {
        Vector3 boxPosition = transform.TransformPoint(BoxPosition);
        Collider[] hitCollider = Physics.OverlapBox(boxPosition, boxScale, transform.localRotation);
        for (int i = 0, l = hitCollider.Length; i < l; ++i)
        {
            if (hitCollider[i].CompareTag("Player"))
            {
                return hitCollider[i];
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        if (agent != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, yellRadius);
    }

    public GameObject InstantiateObjects(GameObject obj, Vector3 pos, Quaternion rot, Transform parent = null){
        return Instantiate(obj, pos, rot, parent);
    }

}

//////////////////////////////////////////////////////////////////////////////////////////////////////
/// SAUVEGARDE DU 18/04/2019 (chez Paul) AVANT DE METTRE EN PLACE LA STATE MACHINE POUR LES ENEMIS ///
//////////////////////////////////////////////////////////////////////////////////////////////////////
/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public float lookRadius = 10f;
    public float maxLookRadiusOnAlert = 20f;
    [Range(0.1f,5f)]
    public float RadiusExpansionSpeed = 1;
    private float smoothRot = 5f;
    [Space]
    [Header("RedBoxCollison")]
    public Vector3 RedBoxPosition;
    public Vector3 RedBoxScale;
    [Header("GreenBoxCollison")]
    public Vector3 GreenBoxPosition;
    public Vector3 GreenBoxScale;
    Transform target;
    NavMeshAgent agent;
    Animator anim;

    [Space]
    [Header("Attack Variable")]
    public float attackSpeed = 1f;
    public float attackDelay = 0.35f;
    float attackCooldown = 0f;
    float attackImpatience = 3f;
    [Header("Stuned when attack")]
    public float timerAttackedFreeze;
    public float timeBeforeGettingStunable;
    float saveTimeBeforeGettingStunable;
    float saveTimerAttackedFreeze;
    float saveSpeed;
    bool beingAttacked;
    bool beingStunable = true;
    bool hasBeenAttacked;
    bool isStun;
    [Space]
    [Header("Debug AI Comportement")]
    public bool TestIAcomportement;
    bool asFoundTarget = false;
    #region Get Set
    RuntimeAnimatorController rac;
    public RuntimeAnimatorController Rac
    {
        get
        {
            return rac;
        }

        set
        {
            rac = value;
        }
    }

    CharacterStats myStas;
    public CharacterStats MyStas
    {
        get
        {
            return myStas;
        }

        set
        {
            myStas = value;
        }
    }

    bool AnimEnd;
    public bool AnimEnd1
    {
        get
        {
            return AnimEnd;
        }

        set
        {
            AnimEnd = value;
        }
    }
    #endregion

    bool modeAlert;
    float StartlookRadius;
    float speed;

    #region State Machine

    public StateMachine m_sM = new StateMachine();

    public virtual void OnEnable()
    {
        ChangeState(0);
    }

    public void ChangeState(int i)
    {
        m_sM.ChangeState(i);
    }

    #endregion

    public virtual void Start () {
         
        target = PlayerManager.Instance.gameObject.transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        Rac = anim.runtimeAnimatorController;
        MyStas = GetComponent<CharacterStats>();
        StartlookRadius = lookRadius;
        saveSpeed = agent.speed;
        saveTimeBeforeGettingStunable = timeBeforeGettingStunable;
        saveTimerAttackedFreeze = timerAttackedFreeze;
    }
	
	void Update () {

        m_sM.Update();

        AttackedFreezeCoolDownMethods();
        if (!isStun)
        {
            LookingForTargetMethod();
        }
        else
        {
            agent.SetDestination(transform.position);
        }
    }

    private void FixedUpdate()
    {
        m_sM.FixedUpdate();
    }

    public void LookingForTargetMethod()
    {
        if (modeAlert)
        {
            speed += (Time.deltaTime / (1 / RadiusExpansionSpeed));
            lookRadius = Mathf.Lerp(StartlookRadius, maxLookRadiusOnAlert, Mathf.PingPong(speed, 1));
        }
        else
        {
            speed = 0;
        }

        attackCooldown -= Time.deltaTime;

        float distance = Vector3.Distance(target.position, transform.position);

        if ((distance <= lookRadius || asFoundTarget) && !AnimEnd1)
        {
            modeAlert = false;
            agent.SetDestination(target.position);
            if (!TestIAcomportement)
            {
                asFoundTarget = true;
            }
            if (distance <= agent.stoppingDistance)
            {
                CharacterStats mytarget = target.GetComponent<CharacterStats>();
                if (CheckCollision(mytarget, GreenBoxScale, GreenBoxPosition) != null)
                {
                    Attack(mytarget);
                }
                else
                {
                    attackImpatience -= Time.deltaTime;
                    if (attackImpatience <= 0)
                    {
                        Attack(mytarget);
                    }
                }
            }
            FaceTarget();
        }
    }

    void AttackedFreezeCoolDownMethods()
    {
        if (!beingStunable)
        {
            timeBeforeGettingStunable -= Time.deltaTime;
            if (timeBeforeGettingStunable <= 0)
            {
                beingStunable = true;
                timeBeforeGettingStunable = saveTimeBeforeGettingStunable;
            }
        }
        if (beingAttacked)
        {
            timerAttackedFreeze -= Time.deltaTime;
            if (timerAttackedFreeze <= 0)
            {
                timerAttackedFreeze = saveTimerAttackedFreeze;
                //agent.speed = saveSpeed;
                beingAttacked = false;
                isStun = false;
            }
            else
            {
                if (beingStunable)
                {
                    //agent.speed = 0;
                    Debug.Log("Je suis stun");
                    beingStunable = false;
                    isStun = true;
                    anim.SetTrigger("Chocked");
                }
            }
        }
    }

    Quaternion FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smoothRot);

        return transform.rotation;
        
    }

    public virtual void Attack(CharacterStats targetStats)
    {
        if (attackCooldown <= 0)
        {
            attackImpatience = 3f;
            anim.SetTrigger("Attack");
            AnimEnd1 = true;
            attackCooldown = 1f / attackSpeed;
        }
    }

    public virtual void AlertLookingForPlayer()
    {
        anim.SetTrigger("Alert");
        modeAlert = true;
        beingAttacked = true;
    }

    public Collider CheckCollision(CharacterStats stats, Vector3 boxScale, Vector3 BoxPosition)
    {
        Vector3 boxPosition = transform.TransformPoint(BoxPosition);
        Collider[] hitCollider = Physics.OverlapBox(boxPosition, boxScale);
        for (int i = 0, l = hitCollider.Length; i < l; ++i)
        {
            if (hitCollider[i].CompareTag("Player"))
            {
                return hitCollider[i];
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        if (agent != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
        }
    }

    public virtual void Timed()
    {
        
    }
}
*/
//////////////////////////////////////////////////////////////////////////////////////////////////////
/// SAUVEGARDE DU 18/04/2019 (chez Paul) AVANT DE METTRE EN PLACE LA STATE MACHINE POUR LES ENEMIS ///
//////////////////////////////////////////////////////////////////////////////////////////////////////