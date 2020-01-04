using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StalactiteStateEnum;
using PoolTypes;
using UnityEngine.AI;
using UnityEngine.UI;
using EZCameraShake;
public class StalactiteController : MonoBehaviour
{

#region [SerializeField] Variables

    [Header("Signs")]
    public Sign m_sign;
	[Serializable] public class Sign {
        public GameObject m_fallSign;
        public float m_timetoFallStalactite = 5;

        [Header("Anim")]
        public Anim m_anim;
        [Serializable] public class Anim {
            [Header("Color")]
            public Color m_startColor;
            public Color m_endColor;
            public float m_timeToDoColorAnim = 5;
            public AnimationCurve m_colorCurve;
            [Header("Size")]
            public Vector2 m_startSize;
            public Vector2 m_endSize;
            public float m_timeToDoSizeAnim = 5;
            public AnimationCurve m_sizeCurve;
        }
    }

    [Header("Move")]
    public MoveAnimation m_moveAnimation;
	[Serializable] public class MoveAnimation {
        public float m_worldYTargetedPosition = -1;
        public float m_timeToReachPosition = 3;
        public AnimationCurve m_moveCurve;
    }

    [Header("Fall Damage")]
    public FallDamage m_fallDamage;
	[Serializable] public class FallDamage {
        public int m_damageValue = 15;
        public float m_damageRange = 3f;
        public LayerMask m_damageLayer;

        public CameraShake m_shakeCamera;
        public GameObject audioFall;
    }

    [Header("Explosion")]
    public Explosion m_explosion;
	[Serializable] public class Explosion {
        public float m_explosionRange = 6;
        public LayerMask m_explosionLayer;
        public GameObject m_explosionFX;
        public GameObject m_meshToHideOnExploded;
        public float m_waitTimeToSpawnLava = 0.5f;
        public float m_waitToCheckOtherStalactiteArea = 0.25f;

        public CameraShake m_shakeCamera;
        public GameObject audioExplosion;
    }
    [Header("Explosion When Destroyed")]
    public DestroyExplosion m_destroyExplosion;
    [Serializable]
    public class DestroyExplosion
    {
        public GameObject regularExpolion;
        public GameObject cristalExpolion;
    }
    [Header("FX")]
    public FXs m_fxs = new FXs();
    [System.Serializable]
    public class FXs
    {
        public GameObject m_markExplosion;
        public Transform m_markExplosionRoot;
    }
    [Header("Cristals")]
    public Cristals m_cristals;
    [Serializable]public class Cristals
    {
        public GameObject cristalsParent;
    }

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
    [SerializeField] Color m_fallDamageRangeColor = Color.grey;
    [SerializeField] Color m_explosionRangeColor = Color.white;

    [Header("DEBUG")]
    [SerializeField] bool m_canExploded = false;

    [Serializable] public class CameraShake {
        public float m_magnitudeShake = 4f;
        public float m_roughnessShake = 4f;
        public float m_fadeInTimeShake = 0.1f;
        public float m_fadeOutTimeShake = 0.1f;
    }

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

    GameObject m_fallSignGo;
    Image m_fallSignImg;

    CameraShaker m_cameraShaker;

    int intSlotPosition;
    StalactiteSpawnManager spawnManager;
    bool isCristilize;
    bool isInLava;
    bool hasSpawnInRedSlots;


    #endregion

    #region Get Set
    public int IntSlotPosition
    {
        get
        {
            return intSlotPosition;
        }

        set
        {
            intSlotPosition = value;
        }
    }

    public StalactiteSpawnManager SpawnManager
    {
        get
        {
            return spawnManager;
        }

        set
        {
            spawnManager = value;
        }
    }

    public bool IsCristilize
    {
        get
        {
            return isCristilize;
        }

        set
        {
            isCristilize = value;
        }
    }

    public bool IsInLava
    {
        get
        {
            return isInLava;
        }

        set
        {
            isInLava = value;
        }
    }

    public StalactiteState StalactiteState
    {
        get
        {
            return m_stalactiteState;
        }

        set
        {
            m_stalactiteState = value;
        }
    }

    public bool HasSpawnInRedSlots
    {
        get
        {
            return hasSpawnInRedSlots;
        }

        set
        {
            hasSpawnInRedSlots = value;
        }
    }
    #endregion

    #region Encapsulate Variables
    #endregion

    #region Event Functions

    void OnEnable()
    {
        if(!m_explosion.m_meshToHideOnExploded.activeSelf)
        {
            m_explosion.m_meshToHideOnExploded.SetActive(true);
        }


        EnableStalactiteColliderAndNavMesh(false);

        if(m_firstInitialization)
        {
            for (int i = 0, l = m_meshes.Length; i < l; ++i)
            {
                m_meshes[i].material.SetColor("_Color", m_startBaseColor);
                m_meshes[i].material.SetColor("_EmissionColor", m_startEmissiveColor);
            }
        }
        //Invoke("StartFallingStalactite", 2); // À enlever
    }

    void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
        m_navMeshObstacle = GetComponent<NavMeshObstacle>();
        m_collider = GetComponent<CapsuleCollider>();
        m_cameraShaker = CameraShaker.Instance;

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
        m_explosion.m_meshToHideOnExploded.SetActive(false);

        EnableStalactiteColliderAndNavMesh(false);

        Level.AddFX(m_explosion.m_explosionFX, transform.position, Quaternion.identity);
        Level.AddFX(m_explosion.audioExplosion, transform.position, Quaternion.identity);
        StartCoroutine(CheckOtherStalactiteArea());

        if (!isInLava)
        {
            StartCoroutine(SpawnLava());
        }
        else
        {
            DisableStalactite();
        }

        #region Might need to reset bool when disable





        #endregion

        if(spawnManager != null)
        {
            spawnManager.StalactiteHasBeenDestroyed(intSlotPosition, !isInLava, hasSpawnInRedSlots);               //Add a lava slot in the list
        }

        ShakeCamera(m_explosion.m_shakeCamera.m_magnitudeShake, m_explosion.m_shakeCamera.m_roughnessShake, m_explosion.m_shakeCamera.m_fadeInTimeShake, m_explosion.m_shakeCamera.m_fadeOutTimeShake);
    }

    IEnumerator CheckOtherStalactiteArea(){
        yield return new WaitForSeconds(m_explosion.m_waitToCheckOtherStalactiteArea);
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosion.m_explosionRange, m_explosion.m_explosionLayer);
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
        float random = UnityEngine.Random.Range(0f, 360f);
        yield return new WaitForSeconds(m_explosion.m_waitTimeToSpawnLava);
        GameObject go =  m_objectPooler.SpawnSpellFromPool(SpellType.LavaArea, transform.position, Quaternion.identity);
        Vector3 trans = go.transform.rotation.eulerAngles;
        trans.y = random;
        go.transform.eulerAngles = trans;
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

    IEnumerator FallSizeSign()
    {
        Vector2 fromSize = m_sign.m_anim.m_startSize;
        Vector2 toSize = m_sign.m_anim.m_endSize;

        RectTransform m_fallSignRectTrans = m_fallSignGo.GetComponent<RectTransform>();

        float fracJourney = 0;
        float distance = Vector2.Distance(fromSize, toSize);
        float vitesse = distance / m_sign.m_anim.m_timeToDoSizeAnim;
        Vector2 actualSize = fromSize;

        while (actualSize != toSize)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualSize = Vector2.Lerp(fromSize, toSize, m_sign.m_anim.m_sizeCurve.Evaluate(fracJourney));
            m_fallSignRectTrans.sizeDelta = actualSize;
            yield return null;
        }
    }
    IEnumerator FallColorSign()
    {
        Color fromColor = m_sign.m_anim.m_startColor;
        Color toColor = m_sign.m_anim.m_endColor;

        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float vitesse = distance / m_sign.m_anim.m_timeToDoColorAnim;
        Color actualColor = fromColor;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualColor = Color.Lerp(fromColor, toColor, m_sign.m_anim.m_colorCurve.Evaluate(fracJourney));
            m_fallSignImg.color = actualColor;
            yield return null;
        }
    }

    IEnumerator MoveStalactiteAnimation()
    {
        yield return new WaitForSeconds(m_sign.m_timetoFallStalactite);

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
        CheckFallDamageArea();
        m_objectPooler.ReturnObjectToPool(ObjectType.StalactiteSign, m_fallSignGo);
        EnableStalactiteColliderAndNavMesh(true);
        ShakeCamera(m_fallDamage.m_shakeCamera.m_magnitudeShake, m_fallDamage.m_shakeCamera.m_roughnessShake, m_fallDamage.m_shakeCamera.m_fadeInTimeShake, m_fallDamage.m_shakeCamera.m_fadeOutTimeShake);
        Level.AddFX(m_fallDamage.audioFall, transform.position, Quaternion.identity);
    }

    void CheckFallDamageArea()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_fallDamage.m_damageRange, m_fallDamage.m_damageLayer);
		foreach(Collider col in colliders)
        {
			if(col.gameObject.CompareTag("Player"))
            {
                PlayerStats playerStats = col.GetComponent<PlayerStats>();
                playerStats.TakeDamage(m_fallDamage.m_damageValue);
            }
		}
    }

    void EnableStalactiteColliderAndNavMesh(bool enabled){
        if(m_navMeshObstacle != null && m_navMeshObstacle.enabled != enabled)
        {
            m_navMeshObstacle.enabled = enabled;
        }
        if(m_collider != null && m_collider.enabled != enabled)
        {
            m_collider.enabled = enabled;
        }
    }

    void ShakeCamera(float magnitude, float rougness, float fadeInTime, float fadeOutTime){
		CameraShaker.Instance.ShakeOnce(magnitude, rougness, fadeInTime, fadeOutTime);
	}

#endregion

#region Public Functions

    public void StartFallingStalactite()
    {
        Vector3 toPos = new Vector3(transform.position.x, m_moveAnimation.m_worldYTargetedPosition, transform.position.z);
        
        //m_fallSignGo = m_objectPooler.SpawnObjectFromPool(ObjectType.StalactiteSign, toPos, m_sign.m_fallSign.transform.rotation);
        m_fallSignGo = ObjectPooler.Instance.SpawnObjectFromPool(ObjectType.StalactiteSign, toPos, m_sign.m_fallSign.transform.rotation);
        m_fallSignImg = m_fallSignGo.GetComponentInChildren<Image>();

        StartCoroutine(FallSizeSign());
        StartCoroutine(FallColorSign());
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
        spawnManager.StalactiteHasBeenDestroyed(intSlotPosition, false, hasSpawnInRedSlots);

        if (IsCristilize)
        {
            PlayerManager.Instance.GetComponent<CristalsChargeCounter>().AddCristCount();
            Level.AddFX(m_destroyExplosion.cristalExpolion, transform.position, Quaternion.identity);

        }
        else
        {
            Level.AddFX(m_destroyExplosion.regularExpolion, transform.position, Quaternion.identity);
        }

    }

    #endregion

    void OnDrawGizmos()
    {
        if(!m_showGizmos)
        {
            return;
        }
        Gizmos.color = m_explosionRangeColor;
        Gizmos.DrawWireSphere(transform.position, m_explosion.m_explosionRange);

        Gizmos.color = m_fallDamageRangeColor;
        Gizmos.DrawWireSphere(transform.position, m_fallDamage.m_damageRange);
    }

}
