using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using EnemyStateEnum;

public class EnemyStats : CharacterStats {

    Image[] lifeBar;
    Image slider;
    [Header("Enemy Info")]
    [TextArea(1, 20)]
    public string _name;
    [Range(0,2)]
    public int m_enemyPowerLevel;
    public EnemyType m_enemyType = EnemyType.Young;
    public enum EnemyType
    {
        Young,
        Fighter,
        Conqueror
    }
    public bool _hasBackPack;
    bool[] arcanOn;
    bool[] fireOn;
    bool[] iceOn;
    [Header("Debuf Var")]
    public GameObject DebufRoot;
    public GameObject MarqueArcane;
    public GameObject MarqueFeu;
    public GameObject MarqueGivre;
    bool arcaneHasBeenInstanciated;
    bool fireHasBeenInstanciated;
    bool iceHasBeenInstanciated;
    GameObject MarqueDeArcane;
    GameObject MarqueDeFeu;
    GameObject MarqueDeGivre;
    [Space]
    [Header("Canvas")]
    public GameObject m_canvas;
    public float timeBeforeLifeBarOff = 5f;
    float saveTimeBeforeLifeBarOff;
    [Space]
    [Header("Selection Circle")]
    public GameObject m_cirlceCanvas;
    [Space]
    [Header("Aura Sign")]
    public GameObject m_auraSign;
    [Space]
    [Header("BackPack")]
    public GameObject m_backPack;

    float m_timerArcane;
    float m_timerFire;
    float m_timerGivre;

    float saveTimerArcane;
    float saveTimerFire;
    float saveTimerGivre;

    bool StartArcaneCooldown;
    bool StartFireCooldown;
    bool StartGivreCooldown;

    bool StartFreezSnareCooldown;
    float TimeFreezed;
    float saveTimerFreezeSnare;

    float TimerTickDamage;
    int FireTickDamage;
    int FireExplosionDamage;
    float saveDamageTick;

    NavMeshAgent agent;
    EnemyController enemyController;
    int iceSlow;
    int SpeedPercent;
    float saveSpeed;

    float TimeBetweenFireTrailTicks;
    int DamageFireTrailTicks;
    #region get set
    public bool ArcaneHasBeenInstanciated
    {
        get
        {
            return arcaneHasBeenInstanciated;
        }

        set
        {
            arcaneHasBeenInstanciated = value;
        }
    }

    public bool FireHasBeenInstanciated
    {
        get
        {
            return fireHasBeenInstanciated;
        }

        set
        {
            fireHasBeenInstanciated = value;
        }
    }

    public bool IceHasBeenInstanciated
    {
        get
        {
            return iceHasBeenInstanciated;
        }

        set
        {
            iceHasBeenInstanciated = value;
        }
    }

    public float TimerArcane
    {
        get
        {
            return m_timerArcane;
        }

        set
        {
            m_timerArcane = value;
        }
    }

    public float TimerFire
    {
        get
        {
            return m_timerFire;
        }

        set
        {
            m_timerFire = value;
        }
    }

    public float TimerGivre
    {
        get
        {
            return m_timerGivre;
        }

        set
        {
            m_timerGivre = value;
        }
    }

    public float SaveTimerArcane
    {
        get
        {
            return saveTimerArcane;
        }

        set
        {
            saveTimerArcane = value;
        }
    }

    public float SaveTimerFire
    {
        get
        {
            return saveTimerFire;
        }

        set
        {
            saveTimerFire = value;
        }
    }

    public float SaveTimerGivre
    {
        get
        {
            return saveTimerGivre;
        }

        set
        {
            saveTimerGivre = value;
        }
    }

    public int ArcanMarkPos
    {
        get
        {
            return m_arcanMarkPos;
        }

        set
        {
            m_arcanMarkPos = value;
        }
    }

    public int FireMarkPos
    {
        get
        {
            return m_fireMarkPos;
        }

        set
        {
            m_fireMarkPos = value;
        }
    }

    public int IceMarkPos
    {
        get
        {
            return m_iceMarkPos;
        }

        set
        {
            m_iceMarkPos = value;
        }
    }

    public bool[] ArcanOn
    {
        get
        {
            return arcanOn;
        }

        set
        {
            arcanOn = value;
        }
    }

    public bool[] FireOn
    {
        get
        {
            return fireOn;
        }

        set
        {
            fireOn = value;
        }
    }

    public bool[] IceOn
    {
        get
        {
            return iceOn;
        }

        set
        {
            iceOn = value;
        }
    }

    public bool HasTakenDamage
    {
        get
        {
            return hasTakenDamage;
        }

        set
        {
            hasTakenDamage = value;
        }
    }
    #endregion
    //bool checkiIfItIsDead;

    public override void OnEnable()
    {
        base.OnEnable();
        if(m_backPack != null)
        {
        m_backPack.SetActive(_hasBackPack);
        }
        lifeBar = m_canvas.GetComponentsInChildren<Image>();
        m_canvas.SetActive(false);
    }

    public override void Start()
    {
        base.Start();
        lifeBar = m_canvas.GetComponentsInChildren<Image>();
        if(m_backPack != null)
        {
        m_backPack.SetActive(_hasBackPack);
        }
        for (int i = 0, l = lifeBar.Length; i < l; ++i)
        {
            if (lifeBar[i].fillOrigin == 1)                     //Il faut bien que le "fill origin" du slider soit le SEUL a 1
            {
                slider = lifeBar[i];
                break;
            }
        }
        enemyController = GetComponent<EnemyController>();
        #region Canvas Var

        // m_canvas.SetActive(false);
        saveTimeBeforeLifeBarOff = timeBeforeLifeBarOff;

        #endregion

        #region Fire Mark Tick Damage Var
        TimerTickDamage = PlayerManager.Instance.m_debuffs.m_fireTicks.m_timerTickDamage;
        FireTickDamage = PlayerManager.Instance.m_debuffs.m_fireTicks.m_fireTickDamage;
        FireExplosionDamage = PlayerManager.Instance.m_debuffs.m_fireTicks.m_fireExplosionDamage;
        saveDamageTick = TimerTickDamage;
        #endregion

        #region Fire Trail Var 

        TimeBetweenFireTrailTicks = PlayerManager.Instance.m_powers.m_fireTrail.m_fireTrailTick;
        DamageFireTrailTicks = PlayerManager.Instance.m_powers.m_fireTrail.m_fireTrailTickDamage;

        #endregion

        #region Ice Mark Slow Var
        agent = GetComponent<NavMeshAgent>();
        saveSpeed = agent.speed;
        iceSlow = PlayerManager.Instance.m_debuffs.m_IceSlow.m_iceSlow;
        #endregion

        #region Ice Nova Var
        saveTimerFreezeSnare = PlayerManager.Instance.m_powers.m_iceNova.m_timeFreezed;
        #endregion

    }


    #region Mark Methods
    int m_arcanMarkPos;
    int m_fireMarkPos;
    int m_iceMarkPos;


    public override void ArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        base.ArcanMark(damage, timerDebuf, nbrMarks);
        if (!arcaneHasBeenInstanciated)
        {
            MarqueDeArcane = InstantiateMarks(MarqueArcane, DebufRoot);
            m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
            arcaneHasBeenInstanciated = true;
        }
        StartArcaneCooldown = true;
        m_timerArcane = saveTimerArcane = timerDebuf;
    }

    public override void AutoAttackFireMark(float timerDebuf)
    {
        base.AutoAttackFireMark(timerDebuf);
        if (!fireHasBeenInstanciated)
        {
            MarqueDeFeu = InstantiateMarks(MarqueFeu, DebufRoot);
            m_fireMarkPos = CheckPosition(arcaneHasBeenInstanciated, iceHasBeenInstanciated);

            fireHasBeenInstanciated = true;
        }
        StartFireCooldown = true;
        m_timerFire = saveTimerFire = timerDebuf;
    }
    public override void FireMark(float timerDebuf)
    {
        base.FireMark(timerDebuf);
        if (!fireHasBeenInstanciated)
        {
            MarqueDeFeu = InstantiateMarks(MarqueFeu, DebufRoot);
            m_fireMarkPos = CheckPosition(arcaneHasBeenInstanciated, iceHasBeenInstanciated);

            fireHasBeenInstanciated = true;
        }
        StartFireCooldown = true;
        m_timerFire = saveTimerFire = timerDebuf;
        if (FireMarkCount == 2)
        {
            Destroy(MarqueDeFeu);
            fireHasBeenInstanciated = false;
            m_iceMarkPos = CheckPosition(arcaneHasBeenInstanciated, fireHasBeenInstanciated);
            m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);

            FireMarkCount = 0;
            TimerTickDamage = saveDamageTick;
            StartFireCooldown = false;
            // Debug.Log("tien prend : " + FireExplosionDamage + " degats dasn ta face");
            TakeDamage(FireExplosionDamage);
            Level.AddFX(enemyController.m_fxs.m_markExplosion, enemyController.m_fxs.m_markExplosionRoot.position, enemyController.m_fxs.m_markExplosionRoot.rotation);
        }
    }

    public override void IceMark(float timerDebuf)
    {
        base.IceMark(timerDebuf);
        if (!iceHasBeenInstanciated)
        {
            MarqueDeGivre = InstantiateMarks(MarqueGivre, DebufRoot);
            m_iceMarkPos = CheckPosition(arcaneHasBeenInstanciated, fireHasBeenInstanciated);

            iceHasBeenInstanciated = true;
        }
        StartGivreCooldown = true;
        if(GivreMarkCount < 5)
        {
            agent.speed = ((agent.speed) * ((100f - (iceSlow/* GivreMarkCount*/)) / 100f));
        }
        m_timerGivre = saveTimerGivre = timerDebuf;
        #region givreMarkCount
        if (GivreMarkCount == 5)
        {
            /*Destroy(MarqueDeGivre);
            iceHasBeenInstanciated = false;
            GivreMarkCount = 0;
            StartGivreCooldown = false;*/


            // Debug.Log("tien prend : " + FireExplosionDamage + " degats dasn ta face");

            //enemyController.ChangeState(EnemyState.FrozenState); // Block In Ice

           

            //Level.AddFX(enemyController.m_fxs.m_markExplosion, enemyController.m_fxs.m_markExplosionRoot.position, enemyController.m_fxs.m_markExplosionRoot.rotation);
        }
        #endregion
    }
    public override void ArcaneExplosion(int damage)
    {
        base.ArcaneExplosion(damage);
        Destroy(MarqueDeFeu);
        Destroy(MarqueDeArcane);
        Destroy(MarqueDeGivre);
        arcaneHasBeenInstanciated = fireHasBeenInstanciated = iceHasBeenInstanciated = StartArcaneCooldown = StartFireCooldown = StartGivreCooldown = false;
    }

    GameObject InstantiateMarks(GameObject mark, GameObject root)
    {
        GameObject marksave = Instantiate(mark, root.transform);

        return marksave;
    }

    int CheckPosition(bool otherMark_1, bool otherMark_2)
    {
        if(!otherMark_1 && !otherMark_2)
        {
            return 0;
        }
        else if((otherMark_1 || otherMark_2) && (!otherMark_1 || !otherMark_2))
        {
            return 1;
        }
        else if (otherMark_1 && otherMark_2)
        {
            return 2;
        }
        return 4;
    }

    #endregion

    #region Spells Methods

    public override void FireTrail()
    {
        base.FireTrail();
        TimeBetweenFireTrailTicks -= Time.deltaTime;
        if(TimeBetweenFireTrailTicks <= 0)
        {
            // Debug.Log("FireTrailTicks");
            TakeDamage(DamageFireTrailTicks);
            //Debug.Log("Boom " + DamageFireTrailTicks + " damage dans les dents");
        }
    }
    #endregion




    public override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(100);
        }
#endif
        m_canvas.transform.LookAt(mainCamera.transform);

        MarksCoolDownMethods();
        if (!IsDead)
        {
            CanvasSetActiveMethod();
        }
        else
        {
            m_canvas.SetActive(false);
        }

        if (hasTakenDamage)
        {
            m_timeToDecreaseWhiteLifeBar -= Time.deltaTime;

            if(m_timeToDecreaseWhiteLifeBar <= 0)
            {
                hasTakenDamage = false;
            }
        }

    }
    void CanvasSetActiveMethod()
    {
        if((StartArcaneCooldown || StartFireCooldown || StartGivreCooldown))
        {
            m_canvas.SetActive(true);
        }
        else if(m_canvas.activeSelf)
        {
            timeBeforeLifeBarOff -= Time.deltaTime;
            // Debug.Log(timeBeforeLifeBarOff);
            if (timeBeforeLifeBarOff <= 0)
            {
                // Debug.Log("LifeBarOff");
                m_canvas.SetActive(false);
                timeBeforeLifeBarOff = saveTimeBeforeLifeBarOff;
            }
        }
    }

    void MarksCoolDownMethods()
    {
        if (StartArcaneCooldown)
        {
            m_timerArcane -= Time.deltaTime;
            if (m_timerArcane <= 0)
            {
                Destroy(MarqueDeArcane);
                arcaneHasBeenInstanciated = false;
                m_iceMarkPos = CheckPosition(arcaneHasBeenInstanciated, fireHasBeenInstanciated);
                m_fireMarkPos = CheckPosition(arcaneHasBeenInstanciated, iceHasBeenInstanciated);
                ArcanMarkCount = 0;
                StartArcaneCooldown = false;
            }
            else
            {
                MarqueDeArcane.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, saveTimerArcane, m_timerArcane);
            }
        }
        if (StartFireCooldown)
        {
            m_timerFire -= Time.deltaTime;
            MarqueDeFeu.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, saveTimerFire, m_timerFire);
            TimerTickDamage -= Time.deltaTime;
            if (m_timerFire <= 0)
            {
                Destroy(MarqueDeFeu);
                fireHasBeenInstanciated = false;
                m_iceMarkPos = CheckPosition(arcaneHasBeenInstanciated, fireHasBeenInstanciated);
                m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
                FireMarkCount = 0;
                TimerTickDamage = saveDamageTick;
                StartFireCooldown = false;

            }else if (TimerTickDamage <= 0)
            {
                TimerTickDamage = saveDamageTick;
                TakeDamage(FireTickDamage);
            }
        }
        if (StartGivreCooldown)
        {
            m_timerGivre -= Time.deltaTime;
            MarqueDeGivre.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, saveTimerGivre, m_timerGivre);
            if (m_timerGivre <= 0)
            {
                if (enemyController.IsImpatient)
                {
                    agent.speed = enemyController.speedSprint;
                }
                else
                {
                    agent.speed = saveSpeed;
                }
                Destroy(MarqueDeGivre);
                iceHasBeenInstanciated = false;
                m_fireMarkPos = CheckPosition(arcaneHasBeenInstanciated, iceHasBeenInstanciated);
                m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
                GivreMarkCount = 0;
                StartGivreCooldown = false;
            }
        }
    }
    bool hasTakenDamage;
    float m_timeToDecreaseWhiteLifeBar;
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        m_timeToDecreaseWhiteLifeBar = BigEnemyLifeBarManager.Instance.m_timeForWhiteLifeBarToDecrease;
        hasTakenDamage = true;
        //BigEnemyLifeBarManager.Instance.TimeForWhiteLifeBar = BigEnemyLifeBarManager.Instance.m_timeForWhiteLifeBarToDecrease;

        slider.fillAmount = Mathf.InverseLerp(0, maxHealth, CurrentHealth);
    }

    public override void Die()
    {
        base.Die();
        //checkiIfItIsDead = true;
        if(MarqueDeArcane != null)
        {
            Destroy(MarqueDeArcane);
            arcaneHasBeenInstanciated = false;
            ArcanMarkCount = 0;
            StartArcaneCooldown = false;
        }
        if(MarqueDeFeu != null)
        {
            Destroy(MarqueDeFeu);
            fireHasBeenInstanciated = false;
            FireMarkCount = 0;
            TimerTickDamage = saveDamageTick;
            StartFireCooldown = false;
        }
        if(MarqueDeGivre != null)
        {
            Destroy(MarqueDeGivre);
            iceHasBeenInstanciated = false;
            GivreMarkCount = 0;
            StartGivreCooldown = false;
        }
        enemyController.OnEnemyDie();
    }

    void DestroyAllMarkOnDead(GameObject mark, bool beenInstanciated, int count, bool coolDown)
    {
        Destroy(mark);
        beenInstanciated = false;
        count = 0;
        coolDown = false;
    }
}
