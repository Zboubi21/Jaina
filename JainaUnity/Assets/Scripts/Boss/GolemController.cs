using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GolemStateEnum;
using EZCameraShake;

public class GolemController : MonoBehaviour
{
	public static GolemController Instance;

#region Serializable Variables
    [Header("Debug")]
    [SerializeField] StateMachine m_sM = new StateMachine();
    [SerializeField, Range(1, 3)] int m_phaseNbr = 1;

    [Header("Boss Attacks")]
    [SerializeField] BossAttacks m_bossAttacks;
    [Serializable] public class BossAttacks {
        public Attacks[] m_attacks = new Attacks[4];
        [Serializable] public class Attacks {
            public BossAttack m_attack;
            [Range(0, 100)] public int m_probability;
        }
        public LavaWaveController m_lavaWaveAttack;

        [Header("Attack Trigger per phase")]
        public StalactiteNbrTrigger[] m_stalactiteNbrTrigger = new StalactiteNbrTrigger[3];
        [Serializable] public class StalactiteNbrTrigger {
            public int m_triggerFallStalactite = 0;
            public int m_triggerArmedialsWrath = 20;
        }

        [Header("Delay")]
        public float[] m_delayBetweenAttacks = new float[3];

        [Header("Change phase")]
        public float[] m_delayToChangeBossPhase = new float[2];
        public CameraShake m_changePhaseShake;
        public GameObject m_changePhaseScream_SFX;
        [Space]
        public float m_waitTimeToStartLavaWave = 5;
    }

    [Header("FX")]
    public FXs m_fxs;
    [Serializable] public class FXs {
        public GameObject m_freezed;
        public GameObject m_markExplosion;
        public Transform m_markExplosionRoot;
    }

    [Header("Staging")]
    [SerializeField] Staging m_staging;
    [Serializable] public class Staging {
        public float m_waitTimeToLaunchFirstAttack = 3;
    }

    [Header("Stalactite Die")]
    [SerializeField] StalactiteDie m_stalactiteDie;
    [Serializable] public class StalactiteDie {
        public float m_waitTimeBetweenStalactiteDie = 0.25f;
        public float m_decreaseTimeAfterStalactiteDie = 0.05f;
        public float m_minimumSpeed = 0.1f;
    }

    [Header("Die")]
    public Die m_die;
    [Serializable] public class Die {
        [Header("Before Die")]
        public float m_changeYRotSpeed = 1;
        public AnimationCurve m_changeYRotCurve; 

        [Header("Die")]
        public float m_waitTimeToDieCrystal = 4.5f;
        public GolemCrystal m_golemCrystal;

        public GolemCrystal[] m_phase2Crystal = new GolemCrystal[2];
        public MeshRenderer[] m_phase2CrystalHit = new MeshRenderer[2];
        public GolemCrystal m_phase3Crystal;
        public MeshRenderer m_phase3CrystalHit;
        public ParticleSystem[] m_phase3Particles = new ParticleSystem[2];
        [Space]
        public float m_waitTimeToAssEffect = 3f;
        public RFX4_EffectSettings m_assFX;
        [Space]
        public GolemTornado m_golemTornado;
        [Space]
        public GolemSmoke m_golemSmoke;
        [Space]
        public GameObject[] m_dieSFX;

        [Space]
        [Header("Armedial's Artefact")]
        public ArmedialLightReference m_armedialLight;
        public float m_waitTimeToActivateArtefact = 5;

        [Header("Light")]
        public float m_toLightValue = 2;
        public float m_timeToActivateLight = 3;
        public AnimationCurve m_activateLightCurve;

        [Header("Material")]
        public float m_timeToChangeColor = 3;
        public AnimationCurve m_materialCurve;
    }

    [Header("Ambience Sounds")]
    [SerializeField] BossAmbienceSoundManager m_bossSoundManager;

    [Header("Alea Debug")]
    public AleaDebug m_aleaDebug;
    [Serializable] public class AleaDebug {
        public bool m_useDebug = false;
        public KeyCode m_debugInput = KeyCode.G;
        public int m_testNbr = 100;
        public int[] m_testProb = new int[4];
        public int[] m_value = new int[4];
    }

    [Serializable] public class CameraShake {
        public float m_magnitudeShake = 4f;
        public float m_roughnessShake = 4f;
        public float m_fadeInTimeShake = 0.1f;
        public float m_fadeOutTimeShake = 0.1f;
    }
#endregion

#region Enum
    enum AttackType
    {
		StalactiteFall, // 0
		TripleStrike,   // 1
		LavaBeam,       // 2
        ArmedialsWrath  // 3
	}
#endregion

#region Private Variables
    Animator m_animator;
    AttackType m_lastAttack = AttackType.ArmedialsWrath;
    bool m_inAttackPattern = false;
    int m_livingStalactite = 0;

    bool m_needToDoArmedialsWrath = false;
    bool m_needToFallStalactite = false;
    bool m_needToChangePhase = false;
    bool m_fightIsStarted = false;
    EnemyStats m_enemyStats;

    PlayerManager m_playerManager;
    CameraManager m_cameraManager;
    CameraShaker m_cameraShaker;

    List<StalactiteController> m_stalactites = new List<StalactiteController>();
    float m_waitTimeAfterNextStalactiteDie = 0;
    
#endregion

#region Encapsulate Variables
    public int PhaseNbr { get { return m_phaseNbr; } }

    float m_yStartRotation;
    public float YStartRotation { get { return m_yStartRotation; } }

    bool m_isDead = false;
    public bool IsDead { get { return m_isDead; } }
    
#endregion

#region Event Functions
    void Awake()
    {
        if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of GolemController");
		}
		SetupStateMachine();
        SetupAttacks();
    }

    void OnEnable()
    {

    }

    void Start()
    {
        m_playerManager = PlayerManager.Instance;
        m_cameraManager = CameraManager.Instance;
        m_cameraShaker = CameraShaker.Instance;
        m_animator = GetComponentInChildren<Animator>();
		ChangeState(GolemState.Idle);
        m_yStartRotation = transform.eulerAngles.y;
        m_enemyStats = GetComponent<EnemyStats>();
    }

    void FixedUpdate()
    {
        m_sM.FixedUpdate();
    }

    void Update()
    {
        m_sM.Update();
        if(m_aleaDebug.m_useDebug && Input.GetKeyDown(m_aleaDebug.m_debugInput))
        {
            for (int i = 0; i < m_aleaDebug.m_testNbr; ++i)
            {
                int alea = Choose(m_aleaDebug.m_testProb);
                m_aleaDebug.m_value[alea] ++;
            }
        }
        // if(Input.GetKeyDown(KeyCode.L) && m_fightIsStarted == false)
        // {
        //     StartAttack();
        // }
        // if(Input.GetKeyDown(KeyCode.N))
        // {
        //     On_GolemAreGoingToDie();
        // }
    }

#endregion

#region Private Functions
    void SetupStateMachine()
	{
		m_sM.AddStates(new List<IState> {
			new GolemIdleState(this),        // 0 = Idle
		});

		string[] golemStateNames = System.Enum.GetNames (typeof(GolemState));
		if(m_sM.States.Count != golemStateNames.Length){
            Debug.LogError("You need to have the same number of State in GolemController and GolemStateEnum");
		}
	}

    void SetupAttacks()
    {
        if(m_bossAttacks.m_attacks != null)
        {
            for (int i = 0, l = m_bossAttacks.m_attacks.Length; i < l; ++i)
            {
                if(m_bossAttacks.m_attacks[i].m_attack != null)
                {
                    m_bossAttacks.m_attacks[i].m_attack.GolemController = this;
                }
            }
        }
    }

    void StartAttack()
    {
        if(m_isDead)
        {
            return;
        }

        if(m_fightIsStarted == false)
        {
            m_fightIsStarted = true;
        }
        AttackType attackToDo = ChoseAttack();
        // Debug.Log("attackToDo = " + attackToDo);
        if(m_bossAttacks.m_attacks[(int)attackToDo].m_attack != null)
        {
            m_bossAttacks.m_attacks[(int)attackToDo].m_attack.On_AttackBegin(m_phaseNbr);
        }
        m_lastAttack = attackToDo;
        m_inAttackPattern = true;

        SetBoolAnimation("FightIdle", false);

        switch (attackToDo)
        {
            case AttackType.StalactiteFall:
                SetTriggerAnimation("Stalactite Fall");
            break;

            case AttackType.TripleStrike:
                // m_animator.SetTrigger("Triple Strike");
            break;

            case AttackType.LavaBeam:
            break;

            case AttackType.ArmedialsWrath:
                SetTriggerAnimation("Armedial's Wrath");
            break;
        }
    }

    AttackType ChoseAttack()
    {
        CheckStalactiteNbr();
        
        // Est-ce qu'il faut faire un "ArmedialsWrath" car il y a trop de stalactite ?
        if(m_needToDoArmedialsWrath && m_lastAttack != AttackType.ArmedialsWrath)
        {
            m_needToDoArmedialsWrath = false;
            return AttackType.ArmedialsWrath;
        }

        // Est-ce qu'il faut faire un "StalactiteFall" car il n'y a pas assez de stalactite ?
        if(m_needToFallStalactite && m_lastAttack != AttackType.StalactiteFall)
        {
            m_needToFallStalactite = false;
            return AttackType.StalactiteFall;
        }

        int[] probs = new int[4];
        for (int i = 0, l = probs.Length; i < l; ++i)
        {
            probs[i] = m_bossAttacks.m_attacks[i].m_probability;
        }

        switch (m_lastAttack)
        {
            case AttackType.StalactiteFall:
                probs[0] = 0;
            break;

            case AttackType.TripleStrike:
                probs[1] = 0;
            break;

            case AttackType.LavaBeam:
                probs[2] = 0;
            break;

            case AttackType.ArmedialsWrath:
                probs[3] = 0;
            break;
        }
        return (AttackType)Choose(probs);
    }
    int Choose (int[] probs) {

        int total = 0;

        foreach (int elem in probs) {
            total += elem;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i= 0; i < probs.Length; i++) {
            if (randomPoint < probs[i]) {
                return i;
            }
            else {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }
    
    IEnumerator DelayToDoNextAttack()
    {
        if(m_needToChangePhase)
        {
            StartCoroutine(ChangePhase(true));
        }
        else
        {
            float delay = m_bossAttacks.m_delayBetweenAttacks[m_phaseNbr - 1];
            yield return new WaitForSeconds(delay);
            if(m_needToChangePhase)
            {
                StartCoroutine(ChangePhase(false));
            }
            else
            {
                StartAttack();
            }
        }
    }
    IEnumerator ChangePhase(bool needToWait)
    {
        if(needToWait)
        {
            yield return new WaitForSeconds(1);
        }
        m_needToChangePhase = false;
        m_phaseNbr ++;
        SetTriggerAnimation("ChangePhase");
        Level.AddFX(m_bossAttacks.m_changePhaseScream_SFX, Vector3.zero, Quaternion.identity);
        ShakeCamera(m_bossAttacks.m_changePhaseShake.m_magnitudeShake, m_bossAttacks.m_changePhaseShake.m_roughnessShake, m_bossAttacks.m_changePhaseShake.m_fadeInTimeShake, m_bossAttacks.m_changePhaseShake.m_fadeOutTimeShake);
        
        if(m_phaseNbr == 2)
        {
            for (int i = 0, l = m_die.m_phase2Crystal.Length; i < l; ++i)
            {
                m_die.m_phase2Crystal[i].On_CrystalLive(true);
            }
            for (int i = 0, l = m_die.m_phase2CrystalHit.Length; i < l; ++i)
            {
                m_die.m_phase2CrystalHit[i].enabled = true;
            }
            m_bossSoundManager.On_GolemSwitchToP2();
        }
        if(m_phaseNbr == 3)
        {
            m_die.m_phase3Crystal.On_CrystalLive(true);
            m_die.m_phase3CrystalHit.enabled = true;
            for (int i = 0, l = m_die.m_phase3Particles.Length; i < l; ++i)
            {
                m_die.m_phase3Particles[i].Play();
            }
            m_bossSoundManager.On_GolemSwitchToP3();
        }

        yield return new WaitForSeconds(m_bossAttacks.m_waitTimeToStartLavaWave);

        SetTriggerAnimation("LavaWave");
        m_bossAttacks.m_lavaWaveAttack.On_AttackBegin(0);

        float delayToStartAttack = m_bossAttacks.m_delayToChangeBossPhase[m_phaseNbr - 2] - 1;
        yield return new WaitForSeconds(delayToStartAttack - m_bossAttacks.m_waitTimeToStartLavaWave);
        StartAttack();
    }

    void CheckStalactiteNbr()
    {
        if (m_livingStalactite <= m_bossAttacks.m_stalactiteNbrTrigger[m_phaseNbr - 1].m_triggerFallStalactite)
        {
            m_needToFallStalactite = true;
        }
        if (m_livingStalactite >= m_bossAttacks.m_stalactiteNbrTrigger[m_phaseNbr - 1].m_triggerArmedialsWrath)
        {
            m_needToDoArmedialsWrath = true;
        }
    }

    IEnumerator WaitCrystalGolemDieFX()
    {
        yield return new WaitForSeconds(m_die.m_waitTimeToDieCrystal);
        m_die.m_golemCrystal.On_CrystalLive(false);
        for (int i = 0, l = m_die.m_phase2Crystal.Length; i < l; ++i)
        {
            m_die.m_phase2Crystal[i].On_CrystalLive(false);
        }
        m_die.m_phase3Crystal.On_CrystalLive(false);
        for (int i = 0, l = m_die.m_phase3Particles.Length; i < l; ++i)
        {
            m_die.m_phase3Particles[i].Play();
            m_die.m_phase3Particles[i].loop = false;
        }
    }

    IEnumerator WaitTimeToLunchFirstAttack()
    {
        yield return new WaitForSeconds(m_staging.m_waitTimeToLaunchFirstAttack);
        StartAttack();
    }

    void ShakeCamera(float magnitude, float rougness, float fadeInTime, float fadeOutTime){
		CameraShaker.Instance.ShakeOnce(magnitude, rougness, fadeInTime, fadeOutTime);
	}

    void ActivateArmedialLight()
    {
        for (int i = 0, l = m_die.m_armedialLight.lights.Length; i < l; ++i)
        {
            m_die.m_armedialLight.ResetLightPos();
            StartCoroutine(ChangeArmedialLightValue(m_die.m_armedialLight.lights[i], m_die.m_toLightValue));
        }
        StartCoroutine(ChangeArmedialMaterialValue());
    }
    IEnumerator ChangeArmedialLightValue(Light light, float toValue)
    {
        yield return new WaitForSeconds(m_die.m_waitTimeToActivateArtefact);

        float fromValue = light.intensity;
        float fracJourney = 0;
        float distance = Mathf.Abs(fromValue - toValue);
        float vitesse = distance / m_die.m_timeToActivateLight;
        float actualValue = fromValue;

        while (actualValue != toValue)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualValue = Mathf.Lerp(fromValue, toValue, m_die.m_activateLightCurve.Evaluate(fracJourney));
            light.intensity = actualValue;
            yield return null;
        }
    }
    IEnumerator ChangeArmedialMaterialValue()
    {
        yield return new WaitForSeconds(m_die.m_waitTimeToActivateArtefact);
        
        Color fromValue = m_die.m_armedialLight.mats[0].color;
        Color actualValue = fromValue;
        Color toValue = m_die.m_armedialLight.mats[1].color;

        float fracJourney = 0;
        float distance = Mathf.Abs(fromValue.r - toValue.r) + Mathf.Abs(fromValue.g - toValue.g) + Mathf.Abs(fromValue.b - toValue.b) + Mathf.Abs(fromValue.a - toValue.a);
        float vitesse = distance / m_die.m_timeToChangeColor;

        MeshRenderer mesh = m_die.m_armedialLight.GetComponent<MeshRenderer>();

        while (actualValue != toValue)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualValue = Color.Lerp(fromValue, toValue, m_die.m_materialCurve.Evaluate(fracJourney));
            mesh.material.color = actualValue;
            yield return null;
        }
    }

    bool m_isFirstPass = false;
    IEnumerator ExplodeAllStalactite()
    {
        if(m_stalactites.Count > 0)
        {
            m_stalactites[0].OnBeKilled();
            yield return new WaitForSeconds(m_waitTimeAfterNextStalactiteDie);
            if (!m_isFirstPass)
            {
                m_isFirstPass = true;
                m_waitTimeAfterNextStalactiteDie = m_stalactiteDie.m_waitTimeBetweenStalactiteDie;
            }
            m_waitTimeAfterNextStalactiteDie = m_waitTimeAfterNextStalactiteDie - m_stalactiteDie.m_decreaseTimeAfterStalactiteDie;
            if(m_waitTimeAfterNextStalactiteDie < m_stalactiteDie.m_minimumSpeed)
                m_waitTimeAfterNextStalactiteDie = m_stalactiteDie.m_minimumSpeed;
            StartCoroutine(ExplodeAllStalactite());
        }
    }

#endregion

#region Public Functions
    public void ChangeState(GolemState newState){
		m_sM.ChangeState((int)newState);
	}

    public void On_StartFight()
    {
        m_die.m_golemCrystal.On_CrystalLive(true);
        m_die.m_golemTornado.On_ScaleTornado(true);
        m_die.m_assFX.IsVisible = true;
        StartCoroutine(WaitTimeToLunchFirstAttack());
        m_enemyStats.m_canTakeDamage = true;
        m_bossSoundManager.On_GolemStartFight();
    }

    public void On_GolemChangePhase()
    {
        m_needToChangePhase = true;
    }

    public void On_GolemAreGoingToDie()
    {
        if(m_inAttackPattern)
        {
            if(m_bossAttacks.m_attacks[(int)m_lastAttack].m_attack != null)
            {
                m_bossAttacks.m_attacks[(int)m_lastAttack].m_attack.On_GolemAreGoingToDie();
            }
        }
        else
        {
            On_GolemDie();
        }
        StartCoroutine(ExplodeAllStalactite());
    }
    public void On_GolemDie()
    {
        m_isDead = true;
        // ChangeState(GolemState.Idle); // Die //passer en die state
        SetTriggerAnimation("Die");
        StartCoroutine(WaitCrystalGolemDieFX());
        m_die.m_assFX.FadeoutTime = m_die.m_waitTimeToAssEffect;
        m_die.m_assFX.IsVisible = false;
        m_die.m_golemTornado.On_ScaleTornado(false);
        m_die.m_golemSmoke.On_ChangeSmoke();
        for (int i = 0, l = m_die.m_dieSFX.Length; i < l; ++i)
        {
            Level.AddFX(m_die.m_dieSFX[i], Vector3.zero, Quaternion.identity);
        }
        m_playerManager.SwitchPlayerToCinematicState(900);
        m_playerManager.StartCoroutine(m_playerManager.StartBossFightBlackScreen());
        m_cameraManager.StartCoroutine(m_cameraManager.LookEndBossFightPos());
        ActivateArmedialLight();

        m_bossSoundManager.On_GolemDie();
    }    

    public void On_AttackIsFinished()
    {
        m_inAttackPattern = false;
        SetBoolAnimation("FightIdle", true);
        StartCoroutine(DelayToDoNextAttack());
    }

    public void On_StalactiteStartToLive(StalactiteController stalactite)
    {
        m_stalactites.Add(stalactite);
    }
    public void On_StalactiteLive()
    {
        m_livingStalactite ++;
    }
    public void On_StalactiteDie(StalactiteController stalactite)
    {
        if(m_livingStalactite > 0)
        {
            m_livingStalactite --;
        }
        m_stalactites.Remove(stalactite);
    }

    public void SetTriggerAnimation(string name)
    {
        m_animator.SetTrigger(name);
    }
    public void SetBoolAnimation(string name, bool value)
    {
        m_animator.SetBool(name, value);
    }

#endregion

}
