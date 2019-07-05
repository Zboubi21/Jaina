using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BigEnemyLifeBarManager : MonoBehaviour {

    public static BigEnemyLifeBarManager Instance;

    [SerializeField] GameObject[] m_UnitFrame;
    [Space]
    [Header("Arcan Marks")]
    [SerializeField] GameObject[] m_arcanMark;
    [Space]
    [Header("Fire Marks")]
    [SerializeField] GameObject[] m_fireMark;
    [Space]
    [Header("Ice Marks")]
    [SerializeField] GameObject[] m_iceMark;

    [Space]
    [SerializeField] float timeToShowLifeBar;
    [SerializeField] float timeToHideLifeBar;
    [Space]

    public float m_timeForWhiteLifeBarToDecrease;
    [Range(0.1f,10f)]
    public float m_decreaseSpeed;
    float timeForWhiteLifeBar;
    float m_showLifeBar;
    float m_hidLifeBar;

    float whitefill;
    float damageRange;
    bool go = true;

    GameObject MarqueDeArcane;
    GameObject MarqueDeFire;
    GameObject MarqueDeGivre;

    bool arcaneOn;
    bool fireOn;
    bool iceOn;

    bool unitFrameOn;

    EnemyStats enemyStats;
    EnemyStats enemyStatsLocked;
    Ray ray;

    float time;

    #region Get Set
    public float TimeForWhiteLifeBar
    {
        get
        {
            return timeForWhiteLifeBar;
        }

        set
        {
            timeForWhiteLifeBar = value;
        }
    }
    #endregion

    void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of BigEnemyLifeBarManager");
		}
        for (int i = 0, l = m_UnitFrame.Length; i < l; ++i)
        {
            if(m_UnitFrame[i] != null)
            {
                if (m_UnitFrame[i].activeSelf)
                {
                    m_UnitFrame[i].SetActive(false);
                }
            }
        }
        m_showLifeBar = timeToShowLifeBar;
        timeForWhiteLifeBar = m_timeForWhiteLifeBarToDecrease;

        m_hidLifeBar = timeToHideLifeBar;
    }

    
    EnemyStats enemyStatsSave;
    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        enemyStats = GetByRay<EnemyStats>(ray);

        if ((enemyStats != null && Input.GetKeyDown(KeyCode.Mouse0)) || unitFrameOn)
        {
            if (!unitFrameOn)
            {
                enemyStatsLocked = enemyStats;
                timeToShowLifeBar = m_showLifeBar;
                unitFrameOn = true;
            }
            ActivateUnitFrame(enemyStatsLocked);
            ActivateLifeBar(enemyStatsLocked.m_enemyPowerLevel, true);
            enemyStatsLocked.m_cirlceCanvas.SetActive(true);
            timeToShowLifeBar = m_showLifeBar;

        }
        else if(enemyStats != null && !unitFrameOn)
        {
            if ((DescreaseTimeToShowLifeBar() && enemyStatsSave == null) || enemyStatsSave == enemyStats)
            {
                enemyStatsSave = enemyStats;
                ActivateUnitFrame(enemyStats);
                ActivateLifeBar(enemyStats.m_enemyPowerLevel, true);
            }
        }


        if (enemyStats == null && unitFrameOn && Input.GetKeyDown(KeyCode.Tab))
        {
            ActivateLifeBar(enemyStatsLocked.m_enemyPowerLevel, false);
            enemyStatsLocked.m_cirlceCanvas.SetActive(false);
            DestroyMarques();
            DeactivateBool(false);
            unitFrameOn = false;
        }
        else if(enemyStats == null && !unitFrameOn && enemyStatsSave != enemyStats)
        {
            if (DescreaseTimeToHideLifeBar())
            {
                ActivateLifeBar(enemyStatsSave.m_enemyPowerLevel, false);
                DestroyMarques();
                DeactivateBool(false);
                enemyStatsSave = null;
                timeToHideLifeBar = m_hidLifeBar;
                timeToShowLifeBar = m_showLifeBar;
            }
        }


        if(enemyStatsLocked != null)
        {
            if(enemyStatsLocked.CurrentHealth <= 0)
            {
                if (m_UnitFrame[enemyStatsLocked.m_enemyPowerLevel].GetComponent<LifeBarArray>().m_whiteLifeBar.fillAmount <= m_UnitFrame[enemyStatsLocked.m_enemyPowerLevel].GetComponent<LifeBarArray>().m_lifeBar.fillAmount)
                {
                    if (DescreaseTimeToHideLifeBar())
                    {
                        ActivateLifeBar(enemyStatsLocked.m_enemyPowerLevel, false);
                        DestroyMarques();
                        DeactivateBool(false);
                        enemyStatsLocked.m_cirlceCanvas.SetActive(false);
                        unitFrameOn = false;
                        timeToHideLifeBar = m_hidLifeBar;
                        timeToShowLifeBar = m_showLifeBar;

                    }
                }
            }
        }
    }


    void DestroyMarques()
    {

        if(MarqueDeArcane != null)
        {
            Destroy(MarqueDeArcane);
        }

        if (MarqueDeFire != null)
        {
            Destroy(MarqueDeFire);
        }

        if (MarqueDeGivre != null)
        {
            Destroy(MarqueDeGivre);
        }
    }

    void DeactivateBool(bool b)
    {
        arcaneOn = b;
        fireOn = b;
        iceOn = b;
    }

    void ActivateLifeBar(int i, bool b)
    {
        m_UnitFrame[i].SetActive(b);
    }

    bool DescreaseTimeToShowLifeBar()
    {
        timeToShowLifeBar -= Time.deltaTime;
        if(timeToShowLifeBar <= 0)
        {
            return true;
        }
        return false;
    }

    bool DescreaseTimeToHideLifeBar()
    {
        timeToHideLifeBar -= Time.deltaTime;
        if (timeToHideLifeBar <= 0)
        {
            return true;
        }
        return false;
    }

    bool DecreaseTimer()
    {
        timeForWhiteLifeBar -= Time.deltaTime;
        if (timeForWhiteLifeBar <= 0)
        {
            return true;
        }
        return false;
    }

    /*IEnumerator ChangeFontSize(TextMeshProUGUI textObject, float fromSize, float toSize)
    {

        float distance = Mathf.Abs(fromSize - toSize);
        float moveFracJourney = new float();
        float vitesse = distance / m_powers.m_uI.m_uIAnimations.m_timeToFinish;

        while (textObject.fontSize != toSize)
        {
            moveFracJourney += (Time.deltaTime) * vitesse / distance;
            textObject.fontSize = Mathf.Lerp(fromSize, toSize, m_powers.m_uI.m_uIAnimations.m_curveAnim.Evaluate(moveFracJourney));
            yield return null;
        }
    }*/


    void ActivateUnitFrame(EnemyStats m_enemyStats)
    {
        LifeBarArray lifeArray = m_UnitFrame[m_enemyStats.m_enemyPowerLevel].GetComponent<LifeBarArray>();
        lifeArray.m_lifeBar.fillAmount = Mathf.InverseLerp(0, m_enemyStats.maxHealth, m_enemyStats.CurrentHealth);
        if (DecreaseTimer() && lifeArray.m_lifeBar.fillAmount != lifeArray.m_whiteLifeBar.fillAmount)
        {
            //Mathf.InverseLerp(0, m_enemyStats.maxHealth, m_enemyStats.CurrentHealth);
            if (go)
            {
                whitefill = lifeArray.m_whiteLifeBar.fillAmount;
                damageRange = (lifeArray.m_whiteLifeBar.fillAmount - lifeArray.m_lifeBar.fillAmount);
                go = false;
            }
            time += Time.deltaTime;
            //m_whiteLifeBar.fillAmount = Mathf.InverseLerp(m_lifeBar.fillAmount, whitefill, Mathf.Lerp(0, 1, Time.deltaTime / 20));

            lifeArray.m_whiteLifeBar.fillAmount -= Mathf.Lerp(0 , damageRange, time / m_decreaseSpeed);

            if(lifeArray.m_whiteLifeBar.fillAmount <= lifeArray.m_lifeBar.fillAmount)
            {
                timeForWhiteLifeBar = m_timeForWhiteLifeBarToDecrease;
                time = 0;
                go = true;
            }
        }

        lifeArray.m_unitName.text = m_enemyStats._name;
        if (m_enemyStats.ArcaneHasBeenInstanciated)
        {
            if (!arcaneOn)
            {
                MarqueDeArcane = InstantiateMarks(m_arcanMark[m_enemyStats.m_enemyPowerLevel], lifeArray.m_markRoot.transform);
                arcaneOn = true;
            }
            MarqueDeArcane.GetComponent<ReferenceScript>().marksArray[2].fillAmount = Mathf.InverseLerp(0, m_enemyStats.SaveTimerArcane, m_enemyStats.TimerArcane);
            MarqueDeArcane.GetComponent<ReferenceScript>().count.text = string.Format("{0}", m_enemyStats.ArcanMarkCount);
        }
        else if (MarqueDeArcane != null)
        {
            Destroy(MarqueDeArcane);
            arcaneOn = false;

        }

        if (m_enemyStats.FireHasBeenInstanciated)
        {
            if (!fireOn)
            {
                MarqueDeFire = InstantiateMarks(m_fireMark[m_enemyStats.m_enemyPowerLevel], lifeArray.m_markRoot.transform);
                fireOn = true;
            }
            MarqueDeFire.GetComponent<ReferenceScript>().marksArray[2].fillAmount = Mathf.InverseLerp(0, m_enemyStats.SaveTimerFire, m_enemyStats.TimerFire);
            //MarqueDeFire.GetComponent<ReferenceScript>().count.text = string.Format("{0}", m_enemyStats.FireMarkCount);

        }
        else if (MarqueDeFire != null)
        {
            Destroy(MarqueDeFire);
            fireOn = false;
        }

        if (m_enemyStats.IceHasBeenInstanciated)
        {
            if (!iceOn)
            {
                MarqueDeGivre = InstantiateMarks(m_iceMark[m_enemyStats.m_enemyPowerLevel], lifeArray.m_markRoot);
                iceOn = true;
            }
            MarqueDeGivre.GetComponent<ReferenceScript>().marksArray[2].fillAmount = Mathf.InverseLerp(0, m_enemyStats.SaveTimerGivre, m_enemyStats.TimerGivre);
            MarqueDeGivre.GetComponent<ReferenceScript>().count.text = string.Format("{0}", m_enemyStats.GivreMarkCount);

        }
        else if (MarqueDeGivre != null)
        {
            Destroy(MarqueDeGivre);
            iceOn = false;
        }

    }

    GameObject InstantiateMarks(GameObject mark, Transform root)
    {
        GameObject marksave = Instantiate(mark, root.transform);
        return marksave;
    }

    public T GetByRay<T>(Ray ray) where T : class
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
        {
            return hit.transform.GetComponent<T>();
        }

        return null;
    }
}
