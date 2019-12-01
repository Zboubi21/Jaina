using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StalactiteStateEnum;
using PoolTypes;
using UnityEngine.AI;

public class Stalactite : MonoBehaviour
{

#region [SerializeField] Variables
    [SerializeField] float m_explosionRange = 6;
    [SerializeField] LayerMask m_explosionLayer;
    [SerializeField] GameObject m_explosionFX;
    [SerializeField] GameObject m_meshToHideOnExploded;
    [SerializeField] float m_waitTimeToSpawnLava = 0.5f;
    [SerializeField] float m_waitToCheckOtherStalactiteArea = 0.25f;

    [Header("Gizmos")]
    [SerializeField] bool m_showGizmos = true;
    [SerializeField] Color m_gizmosColor = Color.white;

    [Header("DEBUG")]
    [SerializeField] bool m_canExploded = false;
#endregion    

#region Private Variables

    StalactiteState m_stalactiteState = StalactiteState.Basic;
    ObjectPooler m_objectPooler;

    NavMeshObstacle m_navMeshObstacle;
    CapsuleCollider m_collider;

#endregion

#region Encapsulate Variables

#endregion

#region Event Functions

    void OnEnable()
    {
        if(!m_meshToHideOnExploded.activeSelf){
            m_meshToHideOnExploded.SetActive(true);
        }
        if(m_navMeshObstacle != null && !m_navMeshObstacle.enabled){
            m_navMeshObstacle.enabled = true;
        }
        if(m_collider != null && !m_collider.enabled){
            m_collider.enabled = true;
        }
    }

    void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
        m_navMeshObstacle = GetComponent<NavMeshObstacle>();
        m_collider = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate()
    {

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && m_canExploded){
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

    void OnExploded()
    {
        m_meshToHideOnExploded.SetActive(false);
        m_navMeshObstacle.enabled = false;
        m_collider.enabled = false;

        Level.AddFX(m_explosionFX, transform.position, Quaternion.identity);
        StartCoroutine(CheckOtherStalactiteArea());
        StartCoroutine(SpawnLava());
    }

    IEnumerator CheckOtherStalactiteArea(){
        yield return new WaitForSeconds(m_waitToCheckOtherStalactiteArea);
         Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosionRange, m_explosionLayer);
		foreach(Collider col in colliders)
        {
			if(col.gameObject.CompareTag("Stalactite"))
            {
                Stalactite stalactite = col.GetComponent<Stalactite>();
                if(stalactite != null)
                {
                    stalactite.AddStalactiteState();
                }
            }
		}
    }

    IEnumerator SpawnLava(){
        yield return new WaitForSeconds(m_waitTimeToSpawnLava);
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

    public void OnBeKilled()
    {
        DisableStalactite();
    }

#endregion

    void OnDrawGizmos()
    {
        if(!m_showGizmos)
        {
            return;
        }
        Gizmos.color = m_gizmosColor;
        Gizmos.DrawWireSphere(transform.position, m_explosionRange);
    }

}
