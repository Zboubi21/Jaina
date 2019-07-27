using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum;
using EnemyStateEnum_Butcher;
using PoolTypes;
using EZCameraShake;

public class EnemyController : MonoBehaviour {

    [SerializeField] bool m_isInstatiate = true;
    [SerializeField] EnemyType m_enemyType;

    [Header("Sounds")]
    public Sounds m_sounds = new Sounds();
	[System.Serializable] public class Sounds {
        public bool m_useZglorgSoundManager = false;
        public GameObject[] m_impatienceFx;
        public GameObject[] m_detectionFx;
        public GameObject[] m_dieFx;
	}

    #region State Machine

    public StateMachine m_sM = new StateMachine();

    public virtual void OnEnable()
    {

        if(Anim == null)
            Anim = GetComponentInChildren<Animator>();

        if(m_sounds.m_useZglorgSoundManager){
            m_zglorgSoundManager = ZglorgSoundManager.Instance;
        }

        if(!m_isInstatiate){
            ChangeState((int)EnemyState.IdleState);
            m_hasBeenOnAlert = false;
        }
        else
        {
            HasBeenOnAlert = true;
            ChangeState((int)EnemyState.ChaseState);
            LogicAtStart();
            LogicWhenEnable();
            m_hasBeenOnAlert = true;
        }
    }

    public void ChangeState(int i)
    {
        m_sM.ChangeState(i);
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
    [Header("Speed Variable")]
    public float speed;

    [Space]
    [Header("Attack Variable")]
    public float attackSpeed = 1f;
    float attackCooldown = 0f;
    bool canAttack = true;
    bool CheckAnimEnd = false;
    /*float attackImpatience = 3f;
    float currentImpatience;*/
    [Header("Stuned when attack")]
    public float timerBeforeGettingStunable;
    public float timeBeingStuned;
    bool beingStunable = true;
    [Header("Impatience Sign")]
    public StartParticles ImpatienceSign;
    bool m_isImpatient;
    float agentSpeed;
    /*[Header("Impatience Sprint")]
    float currentTimeBeforeGettingImpatient;
    float currentTimeBeforeGettingImpatientWhenInAttackRange;
    public float speedSprint = 15f;
    public float TimeBeforeGettingImpatient = 3f;
    bool beingAttacked;
    bool beingStunable = true;
    bool hasBeenAttacked;
    bool isStun;*/

    [Header("Detected FX")]
    public float m_timeToShowDetectedFx = 2;
    public StartParticles m_detectedFx;

    [Header("Stun FX")]
    public GameObject m_stunFx;

    [Header("FX")]
    public FXs m_fxs = new FXs();
    [System.Serializable] public class FXs {
        public GameObject m_freezed;
        public GameObject m_markExplosion;
        public Transform m_markExplosionRoot;
    }

    [Header("Die")]
    [SerializeField] float m_timeToWaitBeforeDespawnEnemy = 3;

    NavMeshAgent agent;
    Animator anim;
    CharacterStats TargetStats;
    CharacterStats myStats;
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

    Transform target;
    public Transform Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
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

    /*public float AttackImpatience
    {
        get
        {
            return attackImpatience;
        }

        set
        {
            attackImpatience = value;
        }
    }*/

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

    /*public float SaveCurrentTimeBeforeGettingImpatient
    {
        get
        {
            return TimeBeforeGettingImpatient;
        }

        set
        {
            TimeBeforeGettingImpatient = value;
        }
    }*/

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

    public PlayerManager PlayerManager
    {
        get
        {
            return m_playerManager;
        }

        set
        {
            m_playerManager = value;
        }
    }

    public NavMeshAgent Agent
    {
        get
        {
            return agent;
        }

        set
        {
            agent = value;
        }
    }

    /*public float CurrentTimeBeforeGettingImpatientWhenInAttackRange
    {
        get
        {
            return currentTimeBeforeGettingImpatientWhenInAttackRange;
        }

        set
        {
            currentTimeBeforeGettingImpatientWhenInAttackRange = value;
        }
    }*/

    /*public float CurrentImpatience
    {
        get
        {
            return currentImpatience;
        }

        set
        {
            currentImpatience = value;
        }
    }*/

    /*public float CurrentTimeBeforeGettingImpatient
    {
        get
        {
            return currentTimeBeforeGettingImpatient;
        }

        set
        {
            currentTimeBeforeGettingImpatient = value;
        }
    }*/
    Collider m_mycollider;
    public Collider Mycollider
    {
        get
        {
            return m_mycollider;
        }

        set
        {
            m_mycollider = value;
        }
    }

    public ObjectPooler ObjectPooler
    {
        get
        {
            return m_objectPooler;
        }

        set
        {
            m_objectPooler = value;
        }
    }

    public EnemyStats Enemystats
    {
        get
        {
            return enemystats;
        }

        set
        {
            enemystats = value;
        }
    }

    ZglorgSoundManager m_zglorgSoundManager;
    public ZglorgSoundManager ZglorgSoundManager
    {
        get
        {
            return m_zglorgSoundManager;
        }

        set
        {
            m_zglorgSoundManager = value;
        }
    }
    #endregion

    public virtual void LogicWhenEnable()
    {
        myStats = GetComponent<CharacterStats>();
        myStats.CurrentHealth = myStas.maxHealth;
        m_mycollider.enabled = true;
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
        m_mycollider = gameObject.GetComponent<Collider>();

        //Get The Time Of The Animation Being Played
        Rac = Anim.runtimeAnimatorController;

        //Get Agent's Speed
        agent.speed = speed;
        AgentSpeed = agent.speed;

        //Get Agent's LookRadius
        StartlookRadius = lookRadius;

        //currentImpatience = AttackImpatience;
        /*currentTimeBeforeGettingImpatient = SaveCurrentTimeBeforeGettingImpatient;
        currentTimeBeforeGettingImpatientWhenInAttackRange = SaveCurrentTimeBeforeGettingImpatient;*/

    }
    EnemyStats enemystats;
    ObjectPooler m_objectPooler;


    public virtual void Start () {

        if (!m_isInstatiate)
        {
            LogicAtStart();
        }
        enemystats = GetComponent<EnemyStats>();
        ObjectPooler = ObjectPooler.Instance;

    }

    public virtual void Update () {

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
            ChangeState((int)EnemyState.VictoryState);
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

    public void SetDestination(Vector3 target)
    {
        agent.SetDestination(target);
    }

    public void StopMoving(bool b)
    {
        if(agent != null)
        {
            agent.isStopped = b;
        }
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

    public void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smoothRot);
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


    #region Only Zglorg Methods
    public virtual bool CanAttackWhenImpatience() {

        return false;
    }

    public virtual void Attack()
    {

    }

    public virtual bool IsChasing()
    {
        return false;
    }

    public virtual bool IsInAttackRangeForToLong()
    {
        return false;
    }

    public virtual void Sprint(float speed)
    {

    }

    public virtual void Timed()
    {
        
    }

    public virtual void IceSlow()
    {
       
    }
    #endregion


    #region Only Butcher Methods

    public virtual bool TargetInImpatienceDonuts()
    {
        return false;
    }

    public virtual bool CoolDownImpatience()
    {
        return false;
    }

    public virtual bool CheckIfAnimEnded()
    {
        return false;
    }

    public virtual void TranslateMove(Transform target)
    {

    }

    public virtual void FaceTarget(Transform target)
    {

    }

    public virtual void OnImpactDamage()
    {

    }

    #endregion

    #region Zglorgette Methods

    public virtual void OnCastProjectil()
    {

    }
    public virtual void OnCastImpatienceProjectil()
    {

    }

    public virtual bool CoolDownWitchImpatience()
    {
        return false;
    }

    #endregion


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
            ChangeState((int)EnemyState.StunState);
        }
    }

    #endregion

    #region IceNovaMethods
    public virtual void Freeze()
    {
        if (!MyStas.IsDead)
        {
            Anim.SetTrigger("Freeze");
            FreezTime = m_playerManager.m_powers.m_iceNova.m_timeFreezed;
            m_fxs.m_freezed.SetActive(true);
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
        AttackCooldown = attackSpeed;
    }
    #endregion

    #region CoolDownEndAnimAttack
    public void AnimFinished()
    {
        StartCoroutine(AnimTime());
    }
    IEnumerator AnimTime()
    {
        //Debug.Log(Rac.animationClips[0].length);
        yield return new WaitForSeconds(attackSpeed);
        CheckAnimEnd1 = true;
    }
    #endregion

    #region Die Methods

    public void OnEnemyDie()
    {
        ChangeState((int)EnemyState.DieState); // Die
    }

    public virtual void Die()
    {
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
            ObjectPooler.SpawnObjectFromPool(enemystats.m_backPack.GetComponent<BackPack_Inventory>()._inventory[0], transform.position, transform.rotation);
        }
        StartCoroutine(OnWaitForAnimToEnd());
    }

    IEnumerator OnWaitForAnimToEnd()
    {
        if(!m_sounds.m_useZglorgSoundManager){
            SpawnRandomGameObject(m_sounds.m_dieFx);
        }else{
            if(m_zglorgSoundManager.CanDoDeathSound()){
                SpawnRandomGameObject(m_sounds.m_dieFx);
            }
        }
        m_mycollider.enabled = false;
        agent.enabled = false;
        yield return new WaitForSeconds(m_timeToWaitBeforeDespawnEnemy);    //Animation time
        TargetStats1.OnEnemyKillCount();
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
            if (enemystats._hasBackPack)
            {
                MeshRenderer[] go = enemystats.m_backPack.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < go.Length; i++)
                {
                    if (go[i] != enemystats.m_backPack.GetComponent<MeshRenderer>())
                    {
                        go[i].gameObject.SetActive(true);
                    }
                }
                enemystats._hasBackPack = false;
            }

            ObjectPooler.Instance.ReturnEnemyToPool(m_enemyType, gameObject);
        }
        else
        {
            // gameObject.SetActive(false);
            Destroy(gameObject);
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
                if(hitCollider[i] != m_mycollider)
                {
                    Enemy.Add(hitCollider[i]);
                    if (!hitCollider[i].gameObject.GetComponent<EnemyController>().BeenYelled)
                    {
                        hitCollider[i].gameObject.GetComponent<EnemyController>().ChangeState((int)(EnemyState)CurrentState);
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

    public virtual void OnDrawGizmosSelected()
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

    public void DestroyGameObject(GameObject obj)
    {
        Destroy(obj);
    }
    public void ShakeCamera(float magnitude, float rougness, float fadeInTime, float fadeOutTime){
		CameraShaker.Instance.ShakeOnce(magnitude, rougness, fadeInTime, fadeOutTime);
	}

    public void SpawnRandomGameObject(GameObject[] objects){
        if(objects.Length > 0){
            int alea = Random.Range(0, objects.Length - 1);
            if(objects[alea] != null){
                Instantiate(objects[alea], Vector3.zero, Quaternion.identity);
            }
        }
    }

    public void ChangeEnemyToCinematicState(){
        ChangeState((int)EnemyState.CinematicState);
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