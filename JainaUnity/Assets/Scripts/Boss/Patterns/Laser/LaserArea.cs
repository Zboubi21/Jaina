using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserArea : MonoBehaviour
{
    LaserBeamController m_laserBeamController;
    List<GameObject> m_stalactiteInTrigger = new List<GameObject>();

    void Start()
    {
        m_laserBeamController = GetComponentInParent<LaserBeamController>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Stalactite"))
        {
            GameObject go = col.gameObject;
            m_stalactiteInTrigger. Add(go);
            m_laserBeamController.On_StalactiteEnterInLaserTrigger(go);
        }
        if(col.CompareTag("Player"))
        {
            m_laserBeamController.On_PlayerEnterInLaserTrigger(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("Stalactite"))
        {
            GameObject go = col.gameObject;
            m_stalactiteInTrigger.Remove(go);
            if(go == m_laserBeamController.LastStalactite)
            {
                for (int i = 0, l = m_stalactiteInTrigger.Count; i < l; i++)
                {
                    m_laserBeamController.On_StalactiteExitFromLaserTrigger(m_stalactiteInTrigger[i]);
                }
            }
        }
        if(col.CompareTag("Player"))
        {
            m_laserBeamController.On_PlayerExitFromLaserTrigger();
        }
    }

    public void ResetStalactiteList()
    {
        if(m_stalactiteInTrigger.Count != 0)
        {
            m_stalactiteInTrigger.Clear();
        }
    }

}
