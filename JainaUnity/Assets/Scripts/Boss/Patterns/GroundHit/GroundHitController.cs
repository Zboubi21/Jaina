using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class GroundHitController : MonoBehaviour
{
    [Header("Debug")]
    [Range(1, 3), SerializeField] int m_phaseNbr = 1;
    [SerializeField] bool m_useDebugInput = false;
    [SerializeField] KeyCode m_testInput = KeyCode.K;
    [SerializeField] AreaType m_actualArea = AreaType.Left;
    
    [Header("Area")]
    [SerializeField] GroundHitArea m_leftArea;
    [SerializeField] GroundHitArea m_middleArea;
    [SerializeField] GroundHitArea m_rightArea;

    [Header("Sign")]
    [SerializeField] Image m_leftSignMesh;
    [SerializeField] Image m_middleSignMesh;
    [SerializeField] Image m_rightSignMesh;

    [Header("Anim Color")]
    public AnimColor m_animColor;
    [Serializable] public class AnimColor {
        public float m_delayBeforStart = 0;
        public Color m_startColor;
        public Color m_endColor;
        public float m_timeToDoColorAnim = 5;
        public AnimationCurve m_colorCurve;
    }

    [Header("Damage")]
    public Damage m_damage;
    [Serializable] public class Damage {
        public float m_timeToDoDamage = 5;
        public int m_damage = 25;

        [Header("Feedback")]
        public Color m_hitSignColor = Color.white;
        public float m_waitTimeToShowHitSign = 5;
        public float m_timeToShowHitSign = 0.125f;

        public CameraShake m_cameraShake;
        [Serializable] public class CameraShake {
        public float m_magnitude = 4f;
        public float m_roughness = 4f;
        public float m_fadeInTime = 0.1f;
        public float m_fadeOutTime = 0.1f;
    }
    }

    [Header("Stalactite Spawn")]
    public StalactiteSpawn m_stalactiteSpawn;
    [Serializable] public class StalactiteSpawn {
        public float m_timeToFallStalactite = 1;
        public SpawnParameters m_spawnPhaseTwo; 
        public SpawnParameters m_spawnPhaseThree; 

        [Serializable] public class SpawnParameters {
            public int m_spawnMinStalactite = 1;
            public int m_spawnMaxStalactite = 3;
        }
    }

    [Header("Test")]
    [SerializeField] StalactiteSpawnManager m_stalactiteSpawner;

    GroundHitSign m_leftGroundHitSign;
    GroundHitSign m_middleGroundHitSign;
    GroundHitSign m_rightGroundHitSign;

    IEnumerator m_hitSignCourout;

    int m_actualPhaseNbr = 1;
    bool m_rightRotateDirection;
    bool m_stopRotation = false;

	public enum AreaType{
		Left,
		Middle,
		Right
	}

    void Awake()
    {
        m_leftArea.GroundHitController = this;
        m_middleArea.GroundHitController = this;
        m_rightArea.GroundHitController = this;

        m_leftGroundHitSign = m_leftSignMesh.GetComponent<GroundHitSign>();
        m_middleGroundHitSign = m_middleSignMesh.GetComponent<GroundHitSign>();
        m_rightGroundHitSign = m_rightSignMesh.GetComponent<GroundHitSign>();
    }

    void Start()
    {
        if(m_actualArea == AreaType.Left)
        {
            m_rightRotateDirection = true;
        }
        else if(m_actualArea == AreaType.Right)
        {
            m_rightRotateDirection = false;
        }
    }

    void Update()
    {
        if(m_useDebugInput && Input.GetKeyDown(m_testInput))
        {
            StartGroundHit(m_phaseNbr);
        }
    }

    public void StartGroundHit(int phaseNbr)
    {
        m_actualPhaseNbr = phaseNbr;
        m_stopRotation = false;
        ChooseArea();
    }

    void ChooseArea()
    {
        StartArea(m_actualArea);
        switch (m_actualArea)
        {
            case AreaType.Left:
                if(m_rightRotateDirection)
                {
                    m_actualArea = AreaType.Middle;
                }
                else
                {
                    // Stop the pattern
                    m_stopRotation = true;
                }
            break;
            case AreaType.Middle:
                if(m_rightRotateDirection)
                {
                    m_actualArea = AreaType.Right;
                }
                else
                {
                    m_actualArea = AreaType.Left;
                }
            break;
            case AreaType.Right:
                if(m_rightRotateDirection)
                {
                    // Stop the pattern
                    m_stopRotation = true;
                }
                else
                {
                    m_actualArea = AreaType.Middle;
                }
            break;
        }
    }

    void StartArea(AreaType areaType)
    {
        ShowHitSign(areaType);
        StartCoroutine(WaitTimeToDoDamage(areaType, m_actualPhaseNbr));
    }

    void ShowHitSign(AreaType areaType)
    {
        switch (areaType)
        {
            case AreaType.Left:
                m_hitSignCourout = HitColorSign(m_leftSignMesh);
                StartCoroutine(HitDamageSign(m_leftSignMesh));
                m_leftGroundHitSign.StartToMove();
            break;

            case AreaType.Middle:
                m_hitSignCourout = HitColorSign(m_middleSignMesh);
                StartCoroutine(HitDamageSign(m_middleSignMesh));
                m_middleGroundHitSign.StartToMove();
            break;

            case AreaType.Right:
                m_hitSignCourout = HitColorSign(m_rightSignMesh);
                StartCoroutine(HitDamageSign(m_rightSignMesh));
                m_rightGroundHitSign.StartToMove();
            break;
        }
        StartCoroutine(m_hitSignCourout);
    }
    
    IEnumerator HitColorSign(Image img)
    {
        Color fromColor = m_animColor.m_startColor;
        Color toColor = m_animColor.m_endColor;

        img.color = fromColor;

        yield return new WaitForSeconds(m_animColor.m_delayBeforStart);

        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float vitesse = distance / m_animColor.m_timeToDoColorAnim;
        Color actualColor = fromColor;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualColor = Color.Lerp(fromColor, toColor, m_animColor.m_colorCurve.Evaluate(fracJourney));
            img.color = actualColor;
            yield return null;
        }
    }

    IEnumerator HitDamageSign(Image img)
    {
        yield return new WaitForSeconds(m_damage.m_waitTimeToShowHitSign);
        if(m_hitSignCourout != null)
        {
            StopCoroutine(m_hitSignCourout);
        }
        img.color = m_damage.m_hitSignColor;
        yield return new WaitForSeconds(m_damage.m_timeToShowHitSign);
        img.enabled = false;
    }
    
    IEnumerator WaitTimeToDoDamage(AreaType areaType, int phaseNbr)
    {
        yield return new WaitForSeconds(m_damage.m_timeToDoDamage);
        CheckDamageArea(areaType);
        ShakeCamera(m_damage.m_cameraShake.m_magnitude, m_damage.m_cameraShake.m_roughness, m_damage.m_cameraShake.m_fadeInTime, m_damage.m_cameraShake.m_fadeOutTime);
        yield return new WaitForSeconds(m_stalactiteSpawn.m_timeToFallStalactite);
        if(phaseNbr == 2)
        {
            // Debug.Log("On fait tomber les stalactite basic");
            m_stalactiteSpawner.OnGenerateStalactite(CalculateStalactiteNbr(phaseNbr), false);
        }
        if(phaseNbr == 3)
        {
            // Debug.Log("On fait tomber les stalactite en fusion");
            m_stalactiteSpawner.OnGenerateStalactite(CalculateStalactiteNbr(phaseNbr), true);
        }
    }
    int CalculateStalactiteNbr(int phaseNbr)
    {
        int stalactiteToSpawn = 0;
        if(phaseNbr == 2)
        {
            stalactiteToSpawn = UnityEngine.Random.Range(m_stalactiteSpawn.m_spawnPhaseTwo.m_spawnMinStalactite, m_stalactiteSpawn.m_spawnPhaseTwo.m_spawnMaxStalactite);
        }
        if(phaseNbr == 3)
        {
            stalactiteToSpawn = UnityEngine.Random.Range(m_stalactiteSpawn.m_spawnPhaseThree.m_spawnMinStalactite, m_stalactiteSpawn.m_spawnPhaseThree.m_spawnMaxStalactite);
        }
        return stalactiteToSpawn;
    }
    void CheckDamageArea(AreaType areaType)
    {
        switch (areaType)
        {
            case AreaType.Left:
                m_leftArea.CheckArea();
            break;

            case AreaType.Middle:
                m_middleArea.CheckArea();
            break;

            case AreaType.Right:
                m_rightArea.CheckArea();
            break;
        }

        if(!m_stopRotation)
        {
            ChooseArea();
        }
        else
        {
            m_rightRotateDirection =! m_rightRotateDirection;
        }
    }

    void ShakeCamera(float magnitude, float rougness, float fadeInTime, float fadeOutTime){
		CameraShaker.Instance.ShakeOnce(magnitude, rougness, fadeInTime, fadeOutTime);
	}
    
}
