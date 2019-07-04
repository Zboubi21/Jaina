using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigEnemyLifeBarManager : MonoBehaviour {

	public static BigEnemyLifeBarManager Instance;

	[SerializeField] GameObject m_UnitFrame;
	[SerializeField] Image m_lifeBar;
    [SerializeField] Transform m_markRoot;
    [SerializeField] Transform m_markBackGroundRoot;
    [SerializeField] GameObject m_iceMark;
    [SerializeField] GameObject m_fireMark;
    [SerializeField] GameObject m_arcanMark;

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

    void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of BigEnemyLifeBarManager");
		}
        m_UnitFrame.SetActive(false);

    }

    /*public void ShowLifeBar(EnemyStats enemyStat){
		m_enemyStats = enemyStat;
	}*/

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        enemyStats = GetByRay<EnemyStats>(ray);

        if ((enemyStats != null && Input.GetKeyDown(KeyCode.Mouse0)) || unitFrameOn)
        {
            if (!unitFrameOn)
            {
                enemyStatsLocked = enemyStats;
            }
            unitFrameOn = true;
            ActivateUnitFrame(enemyStatsLocked);
            m_UnitFrame.SetActive(true);
        }
        else if(enemyStats != null)
        {
            ActivateUnitFrame(enemyStats);
            m_UnitFrame.SetActive(true);
        }


        if (enemyStats == null && unitFrameOn && Input.GetKeyDown(KeyCode.Tab))
        {
            m_UnitFrame.SetActive(false);
            unitFrameOn = false;
        }
        else if(enemyStats == null && !unitFrameOn)
        {
            m_UnitFrame.SetActive(false);
        }


        if(enemyStatsLocked != null)
        {
            if(enemyStatsLocked.CurrentHealth <= 0)
            {
                m_UnitFrame.SetActive(false);
                unitFrameOn = false;
            }
        }


    }

    void ActivateUnitFrame(EnemyStats m_enemyStats)
    {
        m_lifeBar.fillAmount = Mathf.InverseLerp(0, m_enemyStats.maxHealth, m_enemyStats.CurrentHealth);

        if (m_enemyStats.ArcaneHasBeenInstanciated)
        {
            if (!arcaneOn)
            {
                MarqueDeArcane = Instantiate(m_arcanMark, m_markRoot.transform);
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
                MarqueDeFire = Instantiate(m_fireMark, m_markRoot.transform);
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
                MarqueDeGivre = Instantiate(m_iceMark, m_markRoot.transform);
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
