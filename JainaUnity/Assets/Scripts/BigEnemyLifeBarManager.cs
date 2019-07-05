using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BigEnemyLifeBarManager : MonoBehaviour {

	public static BigEnemyLifeBarManager Instance;

	[SerializeField] GameObject[] m_UnitFrame;
    [Space]
    [SerializeField] TextMeshProUGUI m_unitName;
	[SerializeField] Image m_lifeBar;
	[SerializeField] Image m_whiteLifeBar;
    [Space]

    [SerializeField] Transform m_markRoot;
    [SerializeField] Transform m_markBackGroundRoot;
    [Space]

    [SerializeField] GameObject m_iceMark;
    [SerializeField] GameObject m_fireMark;
    [SerializeField] GameObject m_arcanMark;

    [Space]
    [SerializeField] float timeToShowLifeBar;
    //[SerializeField] float timeToHideLifeBar;
    [Space]

    public float m_timeForWhiteLifeBarToDecrease;
    [Range(0.1f,10f)]
    public float m_decreaseSpeed;
    float timeForWhiteLifeBar;
    float m_showLifeBar;
    //float m_hidLifeBar;

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
            if (m_UnitFrame[i].activeSelf)
            {
                m_UnitFrame[i].SetActive(false);
            }
        }
        m_showLifeBar = timeToShowLifeBar;
        timeForWhiteLifeBar = m_timeForWhiteLifeBarToDecrease;

        //m_hidLifeBar = timeToHideLifeBar;
    }

    /*public void ShowLifeBar(EnemyStats enemyStat){
		m_enemyStats = enemyStat;
	}*/

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        enemyStats = GetByRay<EnemyStats>(ray);
        //m_whiteLifeBar.fillAmount -= Mathf.Lerp(0, 1f, Time.deltaTime/20);

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
        }
        else if(enemyStats != null)
        {
            if (DescreaseTime())
            {
                ActivateUnitFrame(enemyStats);
                ActivateLifeBar(enemyStats.m_enemyPowerLevel, true);

            }
        }


        if (enemyStats == null && unitFrameOn && Input.GetKeyDown(KeyCode.Tab))
        {
            ActivateLifeBar(enemyStatsLocked.m_enemyPowerLevel, false);
            enemyStatsLocked.m_cirlceCanvas.SetActive(false);
            unitFrameOn = false;
        }
        else if(enemyStats == null && !unitFrameOn)
        {
            if (DescreaseTime())
            {
                ActivateLifeBar(enemyStats.m_enemyPowerLevel, false);
            }
        }


        if(enemyStatsLocked != null)
        {
            if(enemyStatsLocked.CurrentHealth <= 0)
            {
                if (m_whiteLifeBar.fillAmount <= m_lifeBar.fillAmount)
                {
                    if (DescreaseTime())
                    {
                        ActivateLifeBar(enemyStatsLocked.m_enemyPowerLevel, false);
                        enemyStatsLocked.m_cirlceCanvas.SetActive(false);
                        unitFrameOn = false;
                    }
                }
            }
        }
    }

    void ActivateLifeBar(int i, bool b)
    {
        m_UnitFrame[i].SetActive(b);
    }

    bool DescreaseTime()
    {
        timeToShowLifeBar -= Time.deltaTime;
        if(timeToShowLifeBar <= 0)
        {
            timeToShowLifeBar = m_showLifeBar;
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
        m_lifeBar.fillAmount = Mathf.InverseLerp(0, m_enemyStats.maxHealth, m_enemyStats.CurrentHealth);
        if (DecreaseTimer() && m_lifeBar.fillAmount != m_whiteLifeBar.fillAmount)
        {
            //Mathf.InverseLerp(0, m_enemyStats.maxHealth, m_enemyStats.CurrentHealth);
            if (go)
            {
                whitefill = m_whiteLifeBar.fillAmount;
                damageRange = (m_whiteLifeBar.fillAmount - m_lifeBar.fillAmount);
                go = false;
            }
            time += Time.deltaTime;
            //m_whiteLifeBar.fillAmount = Mathf.InverseLerp(m_lifeBar.fillAmount, whitefill, Mathf.Lerp(0, 1, Time.deltaTime / 20));

            m_whiteLifeBar.fillAmount -= Mathf.Lerp(0 , damageRange, time / m_decreaseSpeed);

            if(m_whiteLifeBar.fillAmount <= m_lifeBar.fillAmount)
            {
                Debug.Log("It's over");
                timeForWhiteLifeBar = m_timeForWhiteLifeBarToDecrease;
                time = 0;
                go = true;
            }
        }
        
        m_unitName.text = m_enemyStats._name;
        if (m_enemyStats.ArcaneHasBeenInstanciated)
        {
            if (!arcaneOn)
            {
                MarqueDeArcane = InstantiateMarks(m_arcanMark, m_markRoot.transform);
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
                MarqueDeFire = InstantiateMarks(m_fireMark, m_markRoot.transform);
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
                MarqueDeGivre = InstantiateMarks(m_iceMark, m_markRoot);
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
