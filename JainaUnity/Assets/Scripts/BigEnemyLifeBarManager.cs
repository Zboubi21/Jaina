using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemyLifeBarManager : MonoBehaviour {

	public static BigEnemyLifeBarManager Instance;

	[SerializeField] GameObject m_bigLifeBar;

	EnemyStats m_enemyStats;
    Ray ray;

    void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of BigEnemyLifeBarManager");
		}
	}

    /*public void ShowLifeBar(EnemyStats enemyStat){
		m_enemyStats = enemyStat;
	}*/

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        m_enemyStats = GetByRay<EnemyStats>(ray);

        if(m_enemyStats != null)
        {
            m_bigLifeBar.SetActive(true);
        }
        else
        {
            m_bigLifeBar.SetActive(false);
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
