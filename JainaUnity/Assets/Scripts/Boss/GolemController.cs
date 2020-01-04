using System;
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
            public BossAttack m_bossAttacks;
            [Range(0, 100)] public int m_probabilities;
        }

        [Header("Attack Trigger per phase")]
        public int m_stalactiteNbrToTriggerFall = 0;
        public int m_stalactiteNbrToTriggerArmedialsWrath = 20;

        [SerializeField] StalactiteNbrTrigger m_stalactiteNbrTrigger;
        [Serializable] public class StalactiteNbrTrigger {

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

#endregion

#region Encapsulate Variables
    public int PhaseNbr { get { return m_phaseNbr; } }

    bool m_needToDoArmedialsWrath = false;
    public bool NeedToDoArmedialsWrath { get { return m_needToDoArmedialsWrath; } set { m_needToDoArmedialsWrath = value; } }

    bool m_needToFallStalactite = false;
    public bool NeedToFallStalactite { get { return m_needToFallStalactite; } set { m_needToFallStalactite = value; } }
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
        if(Input.GetKeyDown(KeyCode.L))
        {
            StartAttack();
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
                if(m_bossAttacks.m_attacks[i].m_bossAttacks != null)
                {
                    m_bossAttacks.m_attacks[i].m_bossAttacks.GolemController = this;
                }
            }
        }
    }

    void StartAttack()
    {
        AttackType attackToDo = ChoseAttack();
        Debug.Log("attackToDo = " + attackToDo);
        if(m_bossAttacks.m_attacks[(int)attackToDo].m_bossAttacks != null)
        {
            m_bossAttacks.m_attacks[(int)attackToDo].m_bossAttacks.On_AttackBegin(m_phaseNbr);
        }
        m_lastAttack = attackToDo;
    }

    AttackType ChoseAttack()
    {
        CheckStalactiteNbr();
        
        // Est-ce qu'il faut faire un "ArmedialsWrath" car il y a trop de stalactite ?
        if(m_needToDoArmedialsWrath)
        {
            m_needToDoArmedialsWrath = false;
            return AttackType.ArmedialsWrath;
        }

        // Est-ce qu'il faut faire un "StalactiteFall" car il n'y a pas assez de stalactite ?
        if(m_needToFallStalactite)
        {
            m_needToFallStalactite = false;
            return AttackType.StalactiteFall;
        }

        int[] probs = new int[4];
        for (int i = 0, l = probs.Length; i < l; ++i)
        {
            probs[i] = m_bossAttacks.m_attacks[i].m_probabilities;
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
        if(m_livingStalactite <= m_bossAttacks.m_stalactiteNbrToTriggerFall)
        {
            m_needToFallStalactite = true;
        }
        if(m_livingStalactite >= m_bossAttacks.m_stalactiteNbrToTriggerArmedialsWrath)
        {
            m_needToDoArmedialsWrath = true;
        }
    }

#endregion

#region Public Functions
    public void ChangeState(GolemState newState){
		m_sM.ChangeState((int)newState);
	}

    public void OnEnemyDie()
    {
        ChangeState(GolemState.Idle); // Die //passer en die state
    }

    public void On_AttackIsFinished()
    {
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

#endregion

}
