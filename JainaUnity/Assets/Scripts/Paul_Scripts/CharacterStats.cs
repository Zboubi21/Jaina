using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterStats : MonoBehaviour {

    public float maxHealth = 100;
    [Range(0,100)]
    public float m_currentHealth = 100;

    public bool m_canTakeDamage = true;

    public Stats damage;
    public Stats armor;

    [HideInInspector]
    public Camera mainCamera;

	public Hit m_hit = new Hit();
	[System.Serializable] public class Hit {
        public float m_timeToSeeHit = 0.1f;
        public float m_timeToRecovery = 0.1f;
		public GameObject[] m_whiteSkin;
	}

    #region Get Set
    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }

        set
        {
            currentHealth = value;
        }
    }

    public int ArcanMarkCount
    {
        get
        {
            return arcanMarkCount;
        }

        set
        {
            arcanMarkCount = value;
        }
    }

    public int FireMarkCount
    {
        get
        {
            return fireMarkCount;
        }

        set
        {
            fireMarkCount = value;
        }
    }

    public int GivreMarkCount
    {
        get
        {
            return givreMarkCount;
        }

        set
        {
            givreMarkCount = value;
        }
    }

    public bool IsDead
    {
        get
        {
            return isDead;
        }

        set
        {
            isDead = value;
        }
    }

    public int MaxArcanMarkCount
    {
        get
        {
            return maxArcanMarkCount;
        }

        set
        {
            maxArcanMarkCount = value;
        }
    }

    #endregion
    private float currentHealth;// { get; private set; }
    int arcanMarkCount;
    int fireMarkCount;
    int givreMarkCount;

    float bonusDamage = 1f;

    float multiplicateur;
    float arcanBlastMultiplicateur;

    float previousDamage = 1;

    int maxArcanMarkCount;

    bool isDead = false;
    bool m_canShowHitFx = true;

    PlayerStats m_playerState;

    public virtual void OnEnable()
    {
        if(mainCamera == null){
            mainCamera = Camera.main;//.GetComponent<Camera>();
        }
    }

    public virtual void Start()
    {

        mainCamera = Camera.main;//.GetComponent<Camera>();
        CurrentHealth = maxHealth * (m_currentHealth /100f);

        multiplicateur = PlayerManager.Instance.m_percentMultiplicateur / 100f;
        arcanBlastMultiplicateur = PlayerManager.Instance.m_powers.m_arcaneExplosion.m_blastMultiplicateur / 100f;
        maxArcanMarkCount = PlayerManager.Instance.m_maxArcanMarkCount;
        //Debug.Log(multiplicateur);

        m_playerState = GetComponent<PlayerStats>();
    }

    public virtual void Update()
    {
        
    }

    #region Mark Methods
    public virtual void ArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        if(ArcanMarkCount + nbrMarks < maxArcanMarkCount)
        {
            ArcanMarkCount += nbrMarks;
        }
        else if (ArcanMarkCount + nbrMarks >= maxArcanMarkCount)
        {
            ArcanMarkCount = maxArcanMarkCount;
        }
        //TakeDamage((int)(damage*(1 + multiplicateur * ArcanMarkCount)));
        float dmg = (previousDamage * multiplicateur * ArcanMarkCount) + damage;
        previousDamage = dmg;
        TakeDamage((int)dmg);
    }
    public virtual void AutoAttackArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        if (ArcanMarkCount < maxArcanMarkCount)
        {
            ArcanMarkCount++;
        }
        float dmg = (previousDamage * multiplicateur * ArcanMarkCount) + damage;
        previousDamage = dmg;
        TakeDamage((int)dmg);
    }
    public virtual void AutoAttackFireMark(float timerDebuf)
    {
        if(!m_canTakeDamage) return;
        if (FireMarkCount == 0)
        {
            FireMarkCount++;
        }
    }
    public virtual void FireMark(float timerDebuf)
    {
        if (FireMarkCount <= 2)
        {
            FireMarkCount++;
        }
    }
    public virtual void IceMark(float timerDebuf)
    {
        if (GivreMarkCount < 5)
        {
            GivreMarkCount++;
        }
    }
    public virtual void ArcaneExplosion(int damage)
    {
        bonusDamage = (GivreMarkCount + FireMarkCount + ArcanMarkCount) * arcanBlastMultiplicateur;
        //Debug.Log("YAYAYAAYYAAY" + bonusDamage);
        TakeDamage((int)(damage + (damage * bonusDamage)));
        ArcanMarkCount = FireMarkCount = GivreMarkCount = 0;
        bonusDamage = 1;
    }

    #endregion

    public virtual void OnEnemyInCombatCount()
    {

    }

    public virtual void OnEnemyKillCount()
    {

    }


    // public virtual void ArcaneProjectil(int damage,float timerDebuf, int nbrMarks)
    // {
    //     if (ArcanMarkCount < MaxArcanMarkCount)
    //     {
    //         ArcanMarkCount += nbrMarks;
    //     }
    //     TakeDamage((int)(damage + (damage * ((ArcanMarkCount) * multiplicateur))));
    // }


    public virtual void IceNova()
    {

    }

    public virtual void FireTrail()
    {

    }

    public virtual void TakeDamage (int damage)
    {   
        if(!m_canTakeDamage) return;

        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        // Debug.Log(transform.name + " takes " + damage + " damages.");
        //Debug.Log(transform.name + "n'a plus que :" + CurrentHealth + " point de vie.");

        if (CurrentHealth > 0)
        {
            CurrentHealth -= damage;
        }

        CheckIfHasToDie(CurrentHealth);
    }
    public virtual bool HealDamage(int heal)
    {
        return false;
    }
    public virtual void FullHeal()
    {

    }

    protected void CheckIfHasToDie(float health)
    {
        if (health <= 0)
        {
            if (!IsDead)
            {
                Die();
                IsDead = true;
            }
        }
    }

    public virtual void Die()
    {
        // Debug.Log(transform.name + " died.");
    }

    public void StartHitFxCorout(){
        if(!m_canTakeDamage) return;
        StartCoroutine(HitFxCorout());
    }
    IEnumerator HitFxCorout(){
        if(m_hit.m_whiteSkin == null || !m_canShowHitFx){
            yield break;
        }
        m_canShowHitFx = false;
        SetActiveGameObject(m_hit.m_whiteSkin, true);
        yield return new WaitForSeconds(m_hit.m_timeToSeeHit);
        SetActiveGameObject(m_hit.m_whiteSkin, false);
        StartCoroutine(RecoveryHit());
    }
    void SetActiveGameObject(GameObject[] objects ,bool enable){
        for (int i = 0, l = objects.Length; i < l; ++i){
            objects[i].SetActive(enable);
        }
    }
    IEnumerator RecoveryHit(){
        yield return new WaitForSeconds(m_hit.m_timeToRecovery);
        m_canShowHitFx = true;
    }


    bool m_characterInLava = false;
    float m_lavaTick;
    public float LavaTick
    {
        get
        {
            return m_lavaTick;
        }

        set
        {
            m_lavaTick = value;
        }
    }
    float m_actualLavaTick = 0;

    int m_lavaTickDamage;
    public int LavaTickDamage
    {
        get
        {
            return m_lavaTickDamage;
        }

        set
        {
            m_lavaTickDamage = value;
        }
    }

    int m_lavaAreaNb = 0;
    public void OnCharacterEnterInLavaArea()
    {
        m_lavaAreaNb ++;
        m_characterInLava = true;
        if(m_actualLavaTick == 0){
            m_actualLavaTick = m_lavaTick;
        }
    }
    public void OnCharacterExitInLavaArea()
    {
        m_lavaAreaNb --;
        if(m_lavaAreaNb == 0){
            m_characterInLava = false;
            m_actualLavaTick = m_lavaTick;
        }
    }
    void LavaAreaDamage()
    {
        m_actualLavaTick -= Time.deltaTime;
        if(m_actualLavaTick <= 0)
        {
            if(m_playerState != null){
                m_playerState.TakeDamage(m_lavaTickDamage);
            }else{
                TakeDamage(m_lavaTickDamage);
                StartHitFxCorout();
            }
            m_actualLavaTick = m_lavaTick;
        }
    }

    bool m_characterInLaser = false;
    float m_laserTick;
    public float LaserTick
    {
        get
        {
            return m_laserTick;
        }

        set
        {
            m_laserTick = value;
        }
    }
    float m_actualLaserTick = 0;

    int m_laserTickDamage;
    public int LaserTickDamage
    {
        get
        {
            return m_laserTickDamage;
        }

        set
        {
            m_laserTickDamage = value;
        }
    }
    public void OnCharacterEnterInLaserArea()
    {
        m_characterInLaser = true;
        if(m_actualLaserTick == 0){
            m_actualLaserTick = m_laserTick;
        }
        TakeLaserDamage();
    }
    public void OnCharacterExitInLaserArea()
    {
        m_characterInLaser = false;
        m_actualLaserTick = m_laserTick;
    }
    void LaserAreaDamage()
    {
        m_actualLaserTick -= Time.deltaTime;
        if(m_actualLaserTick <= 0)
        {
            TakeLaserDamage();
            m_actualLaserTick = m_laserTick;
        }
    }
    void TakeLaserDamage()
    {
        if(m_playerState != null){
            m_playerState.TakeDamage(m_laserTickDamage);
        }else{
            TakeDamage(m_laserTickDamage);
            StartHitFxCorout();
        }
    }

    void FixedUpdate(){
        if(m_characterInLava && !isDead){
            LavaAreaDamage();
        }
        if(m_characterInLaser && !isDead){
            LaserAreaDamage();
        }
    }

}
