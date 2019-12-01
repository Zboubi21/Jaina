using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StalactiteStateEnum;
using PoolTypes;
using UnityEngine.AI;

public class StalactiteController : MonoBehaviour
{

#region [SerializeField] Variables

    [Header("Move")]
    public MoveAnimation m_moveAnimation;
	[Serializable] public class MoveAnimation {
        public float m_worldYTargetedPosition = -1;
        public float m_timeToReachPosition = 3;
        public AnimationCurve m_moveCurve;
    }

    [Header("Explosion")]
    [SerializeField] float m_explosionRange = 6;
    [SerializeField] LayerMask m_explosionLayer;
    [SerializeField] GameObject m_explosionFX;
    [SerializeField] GameObject m_meshToHideOnExploded;
    [SerializeField] float m_waitTimeToSpawnLava = 0.5f;
    [SerializeField] float m_waitToCheckOtherStalactiteArea = 0.25f;

    [Space]

    [SerializeField] Renderer[] m_meshes;

    [Header("Change Colors")]
    public ChangeColors m_changeColors;
	[Serializable] public class ChangeColors {
        public BaseColor m_baseColor = new BaseColor();
        [Serializable] public class BaseColor {
            public Color m_desiredColor = Color.red;
            public float m_timeToChangeColor = 0.5f;
            public AnimationCurve m_changeColorCurve;
        }

		public EmissiveColor m_emissiveColor = new EmissiveColor();
        [Serializable] public class EmissiveColor {
            public Color m_desiredColor = Color.red;
            public float m_timeToChangeColor = 0.5f;
            public AnimationCurve m_changeColorCurve;
        }
	}

    [SerializeField] Material m_fireMaterial;

    [Header("Gizmos")]
    [SerializeField] bool m_showGizmos = true;
    [SerializeField] Color m_gizmosColor = Color.white;

    [Header("DEBUG")]
    [SerializeField] bool m_canExploded = false;
#endregion    

#region Private Variables

    bool m_firstInitialization = false;

    StalactiteState m_stalactiteState = StalactiteState.Basic;
    ObjectPooler m_objectPooler;

    NavMeshObstacle m_navMeshObstacle;
    CapsuleCollider m_collider;

    Color m_startBaseColor;
    Color m_startEmissiveColor;
    IEnumerator m_changeColorCorout;
    IEnumerator m_changeEmissiveColorCorout;

#endregion

#region Encapsulate Variables
#endregion

#region Event Functions

    void OnEnable()
    {
        if(!m_meshToHideOnExploded.activeSelf)
        {
            m_meshToHideOnExploded.SetActive(true);
        }
        if(m_navMeshObstacle != null && !m_navMeshObstacle.enabled)
        {
            m_navMeshObstacle.enabled = true;
        }
        if(m_collider != null && !m_collider.enabled)
        {
            m_collider.enabled = true;
        }
        if(m_firstInitialization)
        {
            for (int i = 0, l = m_meshes.Length; i < l; ++i)
            {
                m_meshes[i].material.SetColor("_Color", m_startBaseColor);
                m_meshes[i].material.SetColor("_EmissionColor", m_startEmissiveColor);
            }
        }

        StartToMoveStalactite();
    }

    void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
        m_navMeshObstacle = GetComponent<NavMeshObstacle>();
        m_collider = GetComponent<CapsuleCollider>();

        m_startBaseColor = m_meshes[0].material.GetColor("_Color");
        m_startEmissiveColor = m_meshes[0].material.GetColor("_EmissionColor");

        for (int i = 0, l = m_meshes.Length; i < l; ++i)
        {
            m_meshes[i].material.EnableKeyword("_EMISSION");
        }

        m_firstInitialization = true;
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
                StalactiteController stalactite = col.GetComponent<StalactiteController>();
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

    IEnumerator ChangeMaterialColor(Color fromColor, Color toColor)
    {
        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float vitesse = distance / m_changeColors.m_baseColor.m_timeToChangeColor;
        Color actualColor = fromColor;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualColor = Color.Lerp(fromColor, toColor, m_changeColors.m_baseColor.m_changeColorCurve.Evaluate(fracJourney));

            for (int i = 0, l = m_meshes.Length; i < l; ++i)
            {
                m_meshes[i].material.SetColor("_Color", actualColor);
            }
            yield return null;
        }    
    }

    IEnumerator ChangeMaterialEmissiveColor(Color fromColor, Color toColor)
    {
        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float vitesse = distance / m_changeColors.m_emissiveColor.m_timeToChangeColor;
        Color actualColor = fromColor;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualColor = Color.Lerp(fromColor, toColor, m_changeColors.m_emissiveColor.m_changeColorCurve.Evaluate(fracJourney));

            for (int i = 0, l = m_meshes.Length; i < l; ++i)
            {
                m_meshes[i].material.SetColor("_EmissionColor", actualColor);
            }
            yield return null;
        }    
    }

    IEnumerator MoveStalactiteAnimation()
    {
        Vector3 fromPos = transform.position;
        Vector3 toPos = new Vector3(fromPos.x, m_moveAnimation.m_worldYTargetedPosition, fromPos.z);
        float fracJourney = 0;
        float distance = Vector3.Distance(fromPos, toPos);
        float vitesse = distance / m_moveAnimation.m_timeToReachPosition;

        while (transform.position != toPos)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            transform.position = Vector3.Lerp(fromPos, toPos, m_moveAnimation.m_moveCurve.Evaluate(fracJourney));
            yield return null;
        }    
    }

#endregion

#region Public Functions

    public void StartToMoveStalactite()
    {
        StartCoroutine(MoveStalactiteAnimation());
    }

    public void AddStalactiteState()
    {
        switch (m_stalactiteState)
        {
            case StalactiteState.Basic:
                m_stalactiteState = StalactiteState.Fusion;

                if(m_changeColorCorout != null){
                    StopCoroutine(m_changeColorCorout);
                }
                m_changeColorCorout = ChangeMaterialColor(m_meshes[0].material.GetColor("_Color"), m_changeColors.m_baseColor.m_desiredColor);
                StartCoroutine(m_changeColorCorout);

                if(m_changeEmissiveColorCorout != null){
                    StopCoroutine(m_changeEmissiveColorCorout);
                }
                m_changeEmissiveColorCorout = ChangeMaterialEmissiveColor(m_meshes[0].material.GetColor("_EmissionColor"), m_changeColors.m_emissiveColor.m_desiredColor);
                StartCoroutine(m_changeEmissiveColorCorout);

                for (int i = 0, l = m_meshes.Length; i < l; ++i)
                {
                    m_meshes[i].materials[1] = m_fireMaterial;
                }
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

                if(m_changeColorCorout != null){
                    StopCoroutine(m_changeColorCorout);
                }
                m_changeColorCorout = ChangeMaterialColor(m_meshes[0].material.GetColor("_Color"), m_startBaseColor);
                StartCoroutine(m_changeColorCorout);

                if(m_changeEmissiveColorCorout != null){
                    StopCoroutine(m_changeEmissiveColorCorout);
                }
                m_changeEmissiveColorCorout = ChangeMaterialEmissiveColor(m_meshes[0].material.GetColor("_EmissionColor"), m_startEmissiveColor);
                StartCoroutine(m_changeEmissiveColorCorout);
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
