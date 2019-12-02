using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using EnemyStateEnum;

public class EnemyStats : CharacterStats {

    Image[] lifeBar;
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
    [Tooltip("No little life bar - No BackPack - Pooling")]
    public bool _bigBossFight;
    public bool _isNotMoving;
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
    public Image slider;
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
    GolemController bossController;
    WarLord_MiseEnScene miseEnScene;
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

    public float Slow
    {
        get
        {
            return slow;
        }

        set
        {
            slow = value;
        }
    }
    #endregion
    //bool checkiIfItIsDead;

    public override void OnEnable()
    {
        if (!_bigBossFight)
        {
            base.OnEnable();
            if(m_backPack != null)
            {
                m_backPack.SetActive(_hasBackPack);
                MeshRenderer[] go = m_backPack.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < go.Length; i++)
                {
                    if (go[i] != m_backPack.GetComponent<MeshRenderer>())
                    {
                        if (!go[i].gameObject.activeSelf)
                        {
                            go[i].gameObject.SetActive(true);
                        }
                    }
                }
            }
            lifeBar = m_canvas.GetComponentsInChildren<Image>();
            enemyController = GetComponent<EnemyController>();
            m_canvas.SetActive(false);
            m_cirlceCanvas.SetActive(false);
            slider.fillAmount = 1;
            DestroyAllMarks();
        }
        else
        {
            bossController = GetComponent<GolemController>();
        }
    }
    private void OnDisable()
    {
        if (!_bigBossFight)
        {
            m_canvas.SetActive(false);
            if(enemyController.m_fxs.m_freezed != null)
            {
                enemyController.m_fxs.m_freezed.SetActive(false);
            }
        }
    }
    public override void Start()
    {
        base.Start();
        if (!_bigBossFight)
        {
            lifeBar = m_canvas.GetComponentsInChildren<Image>();
            if(m_backPack != null)
            {
                m_backPack.SetActive(_hasBackPack);
                MeshRenderer[] go = m_backPack.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < go.Length; i++)
                {
                    if (go[i] != m_backPack.GetComponent<MeshRenderer>())
                    {
                        if (!go[i].gameObject.activeSelf)
                        {
                            go[i].gameObject.SetActive(true);
                        }
                    }
                }
            }
            slider.fillAmount = 1;
            miseEnScene = GetComponent<WarLord_MiseEnScene>();

            #region Canvas Var

            saveTimeBeforeLifeBarOff = timeBeforeLifeBarOff;

            #endregion
        }

        enemyController = GetComponent<EnemyController>();
        bossController = GetComponent<GolemController>();
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
        if (!_isNotMoving)
        {
            agent = GetComponent<NavMeshAgent>();
            saveSpeed = agent.speed;
            iceSlow = PlayerManager.Instance.m_debuffs.m_IceSlow.m_iceSlow;
        }
        #endregion

        #region Ice Nova Var
        saveTimerFreezeSnare = PlayerManager.Instance.m_powers.m_iceNova.m_timeFreezed;
        #endregion
        Debug.Log(CurrentHealth);
    }


    #region Mark Methods
    int m_arcanMarkPos;
    int m_fireMarkPos;
    int m_iceMarkPos;

    float speed;
    float slow = 1;

    public override void ArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        base.ArcanMark(damage, timerDebuf, nbrMarks);
        if (!arcaneHasBeenInstanciated && ArcanMarkCount <= MaxArcanMarkCount && CurrentHealth - damage > 0)
        {
            if (!_bigBossFight)
            {
                MarqueDeArcane = InstantiateMarks(MarqueArcane, DebufRoot);
                m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
            }
            arcaneHasBeenInstanciated = true;
        }
        StartArcaneCooldown = true;
        m_timerArcane = saveTimerArcane = timerDebuf;
    }
    public override void AutoAttackArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        base.AutoAttackArcanMark(damage, timerDebuf, nbrMarks);
        if (!arcaneHasBeenInstanciated && ArcanMarkCount <= MaxArcanMarkCount && CurrentHealth - damage > 0)
        {
            if (!_bigBossFight)
            {
                MarqueDeArcane = InstantiateMarks(MarqueArcane, DebufRoot);
                m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
            }
            arcaneHasBeenInstanciated = true;
        }
        if(ArcanMarkCount == 1)
        {
            m_timerArcane = saveTimerArcane = timerDebuf;
        }
        StartArcaneCooldown = true;
    }

    public override void AutoAttackFireMark(float timerDebuf)
    {
        base.AutoAttackFireMark(timerDebuf);
        if (!fireHasBeenInstanciated)
        {
            if (!_bigBossFight)
            {
                MarqueDeFeu = InstantiateMarks(MarqueFeu, DebufRoot);
                m_fireMarkPos = CheckPosition(arcaneHasBeenInstanciated, iceHasBeenInstanciated);
            }

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
            if (!_bigBossFight)
            {
                MarqueDeFeu = InstantiateMarks(MarqueFeu, DebufRoot);
                m_fireMarkPos = CheckPosition(arcaneHasBeenInstanciated, iceHasBeenInstanciated);
            }

            fireHasBeenInstanciated = true;
        }
        StartFireCooldown = true;
        m_timerFire = saveTimerFire = timerDebuf;

        if (FireMarkCount == 2)
        {
            if (!_bigBossFight)
            {
                Destroy(MarqueDeFeu);
                m_iceMarkPos = CheckPosition(arcaneHasBeenInstanciated, fireHasBeenInstanciated);
                m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
                Level.AddFX(enemyController.m_fxs.m_markExplosion, enemyController.m_fxs.m_markExplosionRoot.position, enemyController.m_fxs.m_markExplosionRoot.rotation);
            }
            fireHasBeenInstanciated = false;
            FireMarkCount = 0;
            TimerTickDamage = saveDamageTick;
            StartFireCooldown = false;
            // Debug.Log("tien prend : " + FireExplosionDamage + " degats dasn ta face");

            // Fx pour faire exploser la marque de feu, faut le faire meme si ça arrive pas souvent
            Level.AddFX(bossController.m_fxs.m_markExplosion, bossController.m_fxs.m_markExplosionRoot.position, bossController.m_fxs.m_markExplosionRoot.rotation);

            TakeDamage(FireExplosionDamage);
        }
    }
    
    public override void IceMark(float timerDebuf)
    {
        base.IceMark(timerDebuf);
        if (!iceHasBeenInstanciated)
        {
            if (!_bigBossFight)
            {
                MarqueDeGivre = InstantiateMarks(MarqueGivre, DebufRoot);
                m_iceMarkPos = CheckPosition(arcaneHasBeenInstanciated, fireHasBeenInstanciated);
            }

            iceHasBeenInstanciated = true;
        }
        StartGivreCooldown = true;
        if (!_isNotMoving)
        {
            if(GivreMarkCount == 1)
            {
                speed = agent.speed;
            }
            slow = 1-((float)(iceSlow * GivreMarkCount)/100);
            agent.speed = speed * slow;
        }
        m_timerGivre = saveTimerGivre = timerDebuf;
    }
    public override void ArcaneExplosion(int damage)
    {
        base.ArcaneExplosion(damage);
        //Destroy(MarqueDeFeu);
        //Destroy(MarqueDeArcane);
        //Destroy(MarqueDeGivre);
        DestroyAllMarks();
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
            TimeBetweenFireTrailTicks = PlayerManager.Instance.m_powers.m_fireTrail.m_fireTrailTick;

            //Debug.Log("Boom " + DamageFireTrailTicks + " damage dans les dents");
        }
    }
    #endregion




    public override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(4999);
        }
        else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(1000);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(100);
        }
#endif
        MarksCoolDownMethods();
        if (hasTakenDamage)
        {
            m_timeToDecreaseWhiteLifeBar -= Time.deltaTime;

            if(m_timeToDecreaseWhiteLifeBar <= 0)
            {
                hasTakenDamage = false;
            }
        }
        if (!_bigBossFight)
        {
            m_canvas.transform.LookAt(mainCamera.transform);

            if (!IsDead)
            {
                CanvasSetActiveMethod();
            }
            else
            {
                m_canvas.SetActive(false);
            }

        }
    }
    void CanvasSetActiveMethod()
    {
        if((StartArcaneCooldown || StartFireCooldown || StartGivreCooldown) && PlayerManager.Instance.GetComponent<PlayerStats>().IsInCombat && isActiveAndEnabled && !_bigBossFight)
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
            if(MarqueDeArcane != null)
            {
                MarqueDeArcane.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, saveTimerArcane, m_timerArcane);
            }
            if (m_timerArcane <= 0)
            {
                if (!_bigBossFight)
                {
                    Destroy(MarqueDeArcane);
                    m_iceMarkPos = CheckPosition(arcaneHasBeenInstanciated, fireHasBeenInstanciated);
                    m_fireMarkPos = CheckPosition(arcaneHasBeenInstanciated, iceHasBeenInstanciated);
                }
                arcaneHasBeenInstanciated = false;
                ArcanMarkCount = 0;
                StartArcaneCooldown = false;
            }
        }
        if (StartFireCooldown)
        {
            m_timerFire -= Time.deltaTime;
            if(MarqueDeFeu != null)
            {
                MarqueDeFeu.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, saveTimerFire, m_timerFire);
            }
            TimerTickDamage -= Time.deltaTime;
            if (m_timerFire <= 0)
            {
                if (!_bigBossFight)
                {
                    Destroy(MarqueDeFeu);
                    m_iceMarkPos = CheckPosition(arcaneHasBeenInstanciated, fireHasBeenInstanciated);
                    m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
                }

                fireHasBeenInstanciated = false;
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
            if(MarqueDeGivre != null)
            {
                MarqueDeGivre.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, saveTimerGivre, m_timerGivre);
            }
            if (m_timerGivre <= 0)
            {
                if (!_bigBossFight)
                {
                    //enemyController.IceSlow();
                    Destroy(MarqueDeGivre);
                    m_fireMarkPos = CheckPosition(arcaneHasBeenInstanciated, iceHasBeenInstanciated);
                    m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
                }
                if (!_isNotMoving)
                {
                    agent.speed = agent.speed / slow;
                    slow = 1;
                }

                iceHasBeenInstanciated = false;
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
        if(PlayerManager.Instance.GetComponent<PlayerStats>().IsInCombat && isActiveAndEnabled && !_bigBossFight)
        {
            m_canvas.SetActive(true);
        }
        if (!_bigBossFight)
        {
            if (m_canvas.activeSelf)
            {
                timeBeforeLifeBarOff -= Time.deltaTime;
                if (timeBeforeLifeBarOff <= 0)
                {
                    m_canvas.SetActive(false);
                    timeBeforeLifeBarOff = saveTimeBeforeLifeBarOff;
                }
            }
            slider.fillAmount = Mathf.InverseLerp(0, maxHealth, CurrentHealth);
        }

        m_timeToDecreaseWhiteLifeBar = BigEnemyLifeBarManager.Instance.m_timeForWhiteLifeBarToDecrease;
        hasTakenDamage = true;
        //BigEnemyLifeBarManager.Instance.TimeForWhiteLifeBar = BigEnemyLifeBarManager.Instance.m_timeForWhiteLifeBarToDecrease;

    }

    public override void Die()
    {
        base.Die();
        //checkiIfItIsDead = true;
        DestroyAllMarks();
        if (!_bigBossFight)
        {
            enemyController.OnEnemyDie();
        }
        else
        {
            bossController.OnEnemyDie();
        }
    }

    public void DestroyAllMarks(){
        if(MarqueDeArcane != null)
        {
            if (!_bigBossFight)
            {
                m_iceMarkPos = CheckPosition(fireHasBeenInstanciated, arcaneHasBeenInstanciated);
                m_fireMarkPos = CheckPosition(iceHasBeenInstanciated, arcaneHasBeenInstanciated);
                Destroy(MarqueDeArcane);
            }

            m_timerArcane = 0;
            arcaneHasBeenInstanciated = false;
            ArcanMarkCount = 0;
            StartArcaneCooldown = false;
        }
        if(MarqueDeFeu != null)
        {
            if (!_bigBossFight)
            {
                m_iceMarkPos = CheckPosition(arcaneHasBeenInstanciated, fireHasBeenInstanciated);
                m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
                Destroy(MarqueDeFeu);
            }

            m_timerFire = 0;
            fireHasBeenInstanciated = false;
            FireMarkCount = 0;
            TimerTickDamage = saveDamageTick;
            StartFireCooldown = false;
        }
        if(MarqueDeGivre != null)
        {
            if (!_bigBossFight)
            {
                m_fireMarkPos = CheckPosition(arcaneHasBeenInstanciated, iceHasBeenInstanciated);
                m_arcanMarkPos = CheckPosition(fireHasBeenInstanciated, iceHasBeenInstanciated);
                Destroy(MarqueDeGivre);
            }
            if (!_isNotMoving)
            {
                agent.speed = agent.speed / slow;
                slow = 1;
            }
            m_timerGivre = 0;
            iceHasBeenInstanciated = false;
            GivreMarkCount = 0;
            StartGivreCooldown = false;
        }
    }

    void DestroyAllMarkOnDead(GameObject mark, bool beenInstanciated, int count, bool coolDown)
    {
        Destroy(mark);
        beenInstanciated = false;
        count = 0;
        coolDown = false;
    }
}
