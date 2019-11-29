using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StalactiteStateEnum;
using PoolTypes;

public class Stalactite : MonoBehaviour
{

#region [SerializeField] Variables

#endregion    

#region Private Variables

    StalactiteState m_stalactiteState = StalactiteState.Basic;
    ObjectPooler m_objectPooler;

#endregion

#region Encapsulate Variables

#endregion

#region Event Functions

    void OnEnable()
    {

    }

    void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
    }

    void FixedUpdate()
    {

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)){
            AddStalactiteState();
        }
        if(Input.GetKeyDown(KeyCode.M)){
            RemoveStalactiteState();
        }
    }

    void OnDisable()
    {
        ResetStalactiteForPool();
    }

#endregion

#region Private Functions

    void OnBeKilled()
    {
        DisableStalactite();
    }

    void OnExploded()
    {
        Debug.Log("OnExploded");
        m_objectPooler.SpawnSpellFromPool(SpellType.LavaArea, transform.position, Quaternion.identity);
        DisableStalactite();
    }

    void DisableStalactite()
    {
        PoolTracker poolTracker = GetComponent<PoolTracker>();
        if(poolTracker != null)
        {
            Destroy(poolTracker);
        }
        m_objectPooler.ReturnEnemyToPool(EnemyType.Stalactite, gameObject);
    }

    void ResetStalactiteForPool()
    {
        m_stalactiteState = StalactiteState.Basic;
    }

#endregion

#region Public Functions

    public void AddStalactiteState()
    {
        switch (m_stalactiteState)
        {
            case StalactiteState.Basic:
                m_stalactiteState = StalactiteState.Fusion;
            break;

            case StalactiteState.Fusion:
                m_stalactiteState = StalactiteState.Exploded;
                OnExploded();
            break;
        }
    }

    public void RemoveStalactiteState()
    {
        switch (m_stalactiteState)
        {
            case StalactiteState.Basic:
                // Y'a rien à faire normalement
            break;

            case StalactiteState.Fusion:
                m_stalactiteState = StalactiteState.Basic;
            break;
        }
    }

#endregion

}
