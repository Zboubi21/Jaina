﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GolemStateEnum;

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

        [Header("Attack Trigger per phase")]
        public StalactiteNbrTrigger[] m_stalactiteNbrTrigger = new StalactiteNbrTrigger[3];
        [Serializable] public class StalactiteNbrTrigger {
            public int m_triggerFallStalactite = 0;
            public int m_triggerArmedialsWrath = 20;
        }

        [Header("Delay")]
        public float[] m_delayBetweenAttacks = new float[3];
    }

    [Header("FX")]
    public FXs m_fxs;
    [Serializable] public class FXs {
        public GameObject m_freezed;
        public GameObject m_markExplosion;
        public Transform m_markExplosionRoot;
    }

    [Header("Die")]
    [SerializeField] Die m_die;
    [Serializable] public class Die {
        public float m_waitTimeToDieCrystal = 4.5f;
        public GolemCrystal m_golemCrystal;
        [Space]
        public float m_waitTimeToAssEffect = 3f;
        public RFX4_EffectSettings m_assFX;
        [Space]
        public GolemTornado m_golemTornado;
        [Space]
        public GolemSmoke m_golemSmoke;
        [Space]
        public GameObject[] m_dieSFX;
    }

    [Header("Alea Debug")]
    public AleaDebug m_aleaDebug;
    [Serializable] public class AleaDebug {
        public bool m_useDebug = false;
        public KeyCode m_debugInput = KeyCode.G;
        public int m_testNbr = 100;
        public int[] m_testProb = new int[4];
        public int[] m_value = new int[4];
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
    int m_livingStalactite = 0;

    bool m_needToDoArmedialsWrath = false;
    bool m_needToFallStalactite = false;
    bool m_fightIsStarted = false;

#endregion

#region Encapsulate Variables
    public int PhaseNbr { get { return m_phaseNbr; } }

    float m_yStartRotation;
    public float YStartRotation { get { return m_yStartRotation; } }

    bool m_isDead = false;
    public bool IsDead { get { return m_isDead; } set { m_isDead = value; } }
    
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
        m_animator = GetComponentInChildren<Animator>();
		ChangeState(GolemState.Idle);
        m_yStartRotation = transform.eulerAngles.y;
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
        if(Input.GetKeyDown(KeyCode.L) && m_fightIsStarted == false)
        {
            StartAttack();
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            m_isDead = true;
            On_GolemDie();
        }
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
        float delay = 0;
        delay = m_bossAttacks.m_delayBetweenAttacks[m_phaseNbr - 1];
        yield return new WaitForSeconds(delay);
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
        m_die.m_golemCrystal.On_CrystalDie();
    }

#endregion

#region Public Functions
    public void ChangeState(GolemState newState){
		m_sM.ChangeState((int)newState);
	}

    public void On_StartFight()
    {
        StartAttack();
    }

    public void OnEnemyDie()
    {
        ChangeState(GolemState.Idle); // Die //passer en die state
    }

    public void On_AttackIsFinished()
    {
        SetBoolAnimation("FightIdle", true);
        StartCoroutine(DelayToDoNextAttack());
    }

    public void On_StalactiteLive()
    {
        m_livingStalactite ++;
    }
    public void On_StalactiteDie()
    {
        if(m_livingStalactite > 0)
        {
            m_livingStalactite --;
        }
    }

    public void SetTriggerAnimation(string name)
    {
        m_animator.SetTrigger(name);
    }
    public void SetBoolAnimation(string name, bool value)
    {
        m_animator.SetBool(name, value);
    }

    public void On_GolemDie()
    {
        SetTriggerAnimation("Die");
        StartCoroutine(WaitCrystalGolemDieFX());
        m_die.m_assFX.FadeoutTime = m_die.m_waitTimeToAssEffect;
        m_die.m_assFX.IsVisible = false;
        m_die.m_golemTornado.On_ScaleTornado();
        m_die.m_golemSmoke.On_ChangeSmoke();
        for (int i = 0, l = m_die.m_dieSFX.Length; i < l; ++i)
        {
            Level.AddFX(m_die.m_dieSFX[i], Vector3.zero, Quaternion.identity);
        }
    }  

#endregion

}
