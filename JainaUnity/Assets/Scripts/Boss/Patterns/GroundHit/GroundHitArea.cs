using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHitArea : MonoBehaviour
{
    GroundHitController m_groundHitController;
    public GroundHitController GroundHitController{
        set{
            m_groundHitController = value;
        }
    }

    List<GameObject> m_damageObjectInTrigger = new List<GameObject>();
    BoxCollider m_col;

    void Awake()
    {
        m_col = GetComponent<BoxCollider>();
        m_col.enabled = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Stalactite") || col.CompareTag("Player"))
        {
            GameObject go = col.gameObject;
            if(go.activeSelf)
            {
                m_damageObjectInTrigger. Add(go);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("Stalactite") || col.CompareTag("Player"))
        {
            GameObject go = col.gameObject;
            m_damageObjectInTrigger.Remove(go);
        }
    }

    void ResetList()
    {
        if(m_damageObjectInTrigger.Count != 0)
        {
            m_damageObjectInTrigger.Clear();
        }
    }

    public void CheckArea()
    {
        StartCoroutine(CheckAreaCorout());
    }
    IEnumerator CheckAreaCorout()
    {
        m_col.enabled = true;
        yield return new WaitForSeconds(0.25f);
        // yield return new WaitForFixedUpdate();
        DoDamage();
        yield return new WaitForSeconds(0.25f);
        // yield return new WaitForFixedUpdate();
        m_col.enabled = false;
        yield return new WaitForSeconds(0.25f);
        ResetList();
    }
    void DoDamage()
    {
        for (int i = 0, l = m_damageObjectInTrigger.Count; i < l; ++i)
        {
            GameObject go = m_damageObjectInTrigger[i].gameObject;
            if(go.activeSelf)
            {            
                if(go.CompareTag("Player"))
                {
                    go.GetComponent<CharacterStats>().TakeDamage(m_groundHitController.m_damage);
                }
                if(go.CompareTag("Stalactite"))
                {
                    go.GetComponent<StalactiteController>().AddStalactiteState();
                }
            }
        }
    }
}
