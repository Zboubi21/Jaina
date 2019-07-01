using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyStats : CharacterStats {

    Image[] lifeBar;
    Image slider;
    Canvas canvas;

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
    float TimerArcane;
    float TimerFire;
    float TimerGivre;

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

    //bool checkiIfItIsDead;


    public override void Start()
    {
        base.Start();
        canvas = GetComponentInChildren<Canvas>();
        lifeBar = GetComponentsInChildren<Image>();

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

        m_canvas.SetActive(false);
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
    public override void ArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        base.ArcanMark(damage, timerDebuf, nbrMarks);
        if (!arcaneHasBeenInstanciated)
        {
            MarqueDeArcane = Instantiate(MarqueArcane, DebufRoot.transform);
            arcaneHasBeenInstanciated = true;
        }
        StartArcaneCooldown = true;
        TimerArcane = saveTimerArcane = timerDebuf;
    }
    public override void AutoAttackFireMark(float timerDebuf)
    {
        base.AutoAttackFireMark(timerDebuf);
        if (!fireHasBeenInstanciated)
        {
            MarqueDeFeu = Instantiate(MarqueFeu, DebufRoot.transform);

            fireHasBeenInstanciated = true;
        }
        StartFireCooldown = true;
        TimerFire = saveTimerFire = timerDebuf;
    }
    public override void FireMark(float timerDebuf)
    {
        base.FireMark(timerDebuf);
        if (!fireHasBeenInstanciated)
        {
            MarqueDeFeu = Instantiate(MarqueFeu, DebufRoot.transform);

            fireHasBeenInstanciated = true;
        }
        StartFireCooldown = true;
        TimerFire = saveTimerFire = timerDebuf;
        if (FireMarkCount == 2)
        {
            Destroy(MarqueDeFeu);
            fireHasBeenInstanciated = false;
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
            MarqueDeGivre = Instantiate(MarqueGivre, DebufRoot.transform);
            iceHasBeenInstanciated = true;
        }
        StartGivreCooldown = true;
        agent.speed = ((saveSpeed) * ((100f - iceSlow) / 100f));
        TimerGivre = saveTimerGivre = timerDebuf;
    }
    public override void ArcaneExplosion(int damage)
    {
        base.ArcaneExplosion(damage);
        Destroy(MarqueDeFeu);
        Destroy(MarqueDeArcane);
        Destroy(MarqueDeGivre);
        arcaneHasBeenInstanciated = fireHasBeenInstanciated = iceHasBeenInstanciated = StartArcaneCooldown = StartFireCooldown = StartGivreCooldown = false;
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

        canvas.transform.LookAt(mainCamera.transform);

        MarksCoolDownMethods();
        if (!IsDead)
        {
            CanvasSetActiveMethod();
        }
        else
        {
            m_canvas.SetActive(false);
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
            TimerArcane -= Time.deltaTime;
            if (TimerArcane <= 0)
            {
                Destroy(MarqueDeArcane);
                arcaneHasBeenInstanciated = false;
                ArcanMarkCount = 0;
                StartArcaneCooldown = false;
            }
            else
            {
                MarqueDeArcane.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, saveTimerArcane, TimerArcane);
            }
        }
        if (StartFireCooldown)
        {
            TimerFire -= Time.deltaTime;
            MarqueDeFeu.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, saveTimerFire, TimerFire);
            TimerTickDamage -= Time.deltaTime;
            if (TimerFire <= 0)
            {
                Destroy(MarqueDeFeu);
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
            TimerGivre -= Time.deltaTime;
            MarqueDeGivre.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, saveTimerGivre, TimerGivre);
            if (TimerGivre <= 0)
            {
                agent.speed = saveSpeed;
                Destroy(MarqueDeGivre);
                iceHasBeenInstanciated = false;
                GivreMarkCount = 0;
                StartGivreCooldown = false;
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        slider.fillAmount = Mathf.InverseLerp(0, maxHealth, CurrentHealth);
    }
    public override void Die()
    {
        base.Die();
        //checkiIfItIsDead = true;
        enemyController.OnEnemyDie();
    }
}
