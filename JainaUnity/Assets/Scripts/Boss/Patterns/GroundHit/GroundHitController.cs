using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class GroundHitController : BossAttack
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
    [SerializeField] AnimColor m_animColor;
    [Serializable] public class AnimColor {
        public float m_delayBeforStart = 0;
        public Color m_startColor;
        public Color m_endColor;
        public float m_timeToDoColorAnim = 5;
        public AnimationCurve m_colorCurve;
    }

    [Header("Rotate")]
    [SerializeField] Rotate m_rotate;
    [Serializable] public class Rotate {
        public float m_yRightAttackGolemRotation;
        public float m_yMiddleAttackGolemRotation;
        public float m_yLeftAttackGolemRotation;
        [Space]
        public float m_timeToRotate = 1;
        public AnimationCurve m_rotateCurve;
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

    [Header("Impact FX")]
    public ImpactFX m_impactFX;
    [Serializable] public class ImpactFX {
        [Header("SFX")]
        public GameObject[] m_sounds;

        [Header("FX")]
        public GameObject m_hitGroundFX;
        public Transform[] m_hitPos = new Transform[3];
    }

    [Header("Stalactite Spawn")]
    [SerializeField] StalactiteSpawn m_stalactiteSpawn;
    [Serializable] public class StalactiteSpawn {
        public float m_timeToFallStalactite = 1;
        public SpawnParameters m_spawnPhaseTwo; 
        public SpawnParameters m_spawnPhaseThree; 

        [Serializable] public class SpawnParameters {
            public int m_spawnMinStalactite = 1;
            public int m_spawnMaxStalactite = 3;
        }
    }

    [Header("Attack Delay")]
    [SerializeField] float m_waitTimeBetweenAttack = 3;

    [Header("Test")]
    [SerializeField] StalactiteSpawnManager m_stalactiteSpawner;

    GroundHitSign m_leftGroundHitSign;
    GroundHitSign m_middleGroundHitSign;
    GroundHitSign m_rightGroundHitSign;

    IEnumerator m_hitSignCourout;

    int m_actualPhaseNbr = 1;
    bool m_rightRotateDirection;
    bool m_stopRotation = false;

    int m_actualNbrOfAttack = 0;

    List <FX> m_impactSounds = new List<FX>();

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
            On_AttackBegin(m_phaseNbr);
        }
    }

    void StartGroundHit(int phaseNbr)
    {
        m_actualPhaseNbr = phaseNbr;
        m_stopRotation = false;
        StartArea(m_actualArea);
    }

    void SelectArea()
    {
        switch (m_actualArea)
        {
            case AreaType.Left:
                if(m_rightRotateDirection)
                {
                    m_actualArea = AreaType.Middle;
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
                if(!m_rightRotateDirection)
                {
                    m_actualArea = AreaType.Middle;
                }
            break;
        }
        
    }

    void StartArea(AreaType areaType)
    {
        GolemController.SetTriggerAnimation("Triple Strike");
        ShowHitSign(areaType);
        StartCoroutine(WaitTimeToDoDamage(areaType, m_actualPhaseNbr));
        switch (areaType)
        {
            case AreaType.Left:
                StartCoroutine(RotateGolemToLookAtPointWithTime(m_rotate.m_yLeftAttackGolemRotation, m_rotate.m_timeToRotate, m_rotate.m_rotateCurve));
            break;

            case AreaType.Middle:
                StartCoroutine(RotateGolemToLookAtPointWithTime(m_rotate.m_yMiddleAttackGolemRotation, m_rotate.m_timeToRotate, m_rotate.m_rotateCurve));
            break;

            case AreaType.Right:
                StartCoroutine(RotateGolemToLookAtPointWithTime(m_rotate.m_yRightAttackGolemRotation, m_rotate.m_timeToRotate, m_rotate.m_rotateCurve));
            break;
        }

        m_impactSounds.Clear();
        for (int i = 0, l = m_impactFX.m_sounds.Length; i < l; ++i)
        {
            FX newFx = Level.AddFX(m_impactFX.m_sounds[i], Vector3.zero, Quaternion.identity);
            m_impactSounds.Add(newFx);
        }
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
            m_stalactiteSpawner.OnGenerateStalactite(CalculateStalactiteNbr(phaseNbr), false, false);
        }
        if(phaseNbr == 3)
        {
            // Debug.Log("On fait tomber les stalactite en fusion");
            m_stalactiteSpawner.OnGenerateStalactite(CalculateStalactiteNbr(phaseNbr), true, false);
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
        Vector3 newRot;
        switch (areaType)
        {
            case AreaType.Left:
                m_leftArea.CheckArea();
                newRot = new Vector3(90, m_impactFX.m_hitPos[0].rotation.eulerAngles.y, m_impactFX.m_hitPos[0].rotation.eulerAngles.z);
                Level.AddFX(m_impactFX.m_hitGroundFX, m_impactFX.m_hitPos[0].position, Quaternion.Euler(newRot));
            break;

            case AreaType.Middle:
                m_middleArea.CheckArea();
                newRot = new Vector3(90, m_impactFX.m_hitPos[1].rotation.eulerAngles.y, m_impactFX.m_hitPos[1].rotation.eulerAngles.z);
                Level.AddFX(m_impactFX.m_hitGroundFX, m_impactFX.m_hitPos[1].position,  Quaternion.Euler(newRot));
            break;

            case AreaType.Right:
                m_rightArea.CheckArea();
                newRot = new Vector3(90, m_impactFX.m_hitPos[2].rotation.eulerAngles.y, m_impactFX.m_hitPos[2].rotation.eulerAngles.z);
                Level.AddFX(m_impactFX.m_hitGroundFX, m_impactFX.m_hitPos[2].position,  Quaternion.Euler(newRot));
            break;
        }
        m_actualNbrOfAttack ++;
        GolemController.SetTriggerAnimation("Triple Strike Idle");
        StartCoroutine(WaitTimeBetweenAttack());
    }

    IEnumerator WaitTimeBetweenAttack()
    {
        if(m_actualNbrOfAttack == 3)
        {
            On_AttackEnd();
        }
        else
        {
            yield return new WaitForSeconds(m_waitTimeBetweenAttack);
            SelectArea();
            StartArea(m_actualArea);
        }
    }

    void ShakeCamera(float magnitude, float rougness, float fadeInTime, float fadeOutTime)
    {
		CameraShaker.Instance.ShakeOnce(magnitude, rougness, fadeInTime, fadeOutTime);
	}

    public override void On_AttackBegin(int phaseNbr)
    {
        base.On_AttackBegin(phaseNbr);
        m_actualNbrOfAttack = 0;
        StartGroundHit(phaseNbr);
    }

    public override void On_AttackEnd()
    {
        base.On_AttackEnd();
        m_rightRotateDirection =! m_rightRotateDirection;
        StartCoroutine(RotateGolemToLookAtPointWithTime(GolemController.YStartRotation, m_rotate.m_timeToRotate, m_rotate.m_rotateCurve));
    }

    public override void On_GolemAreGoingToDie()
    {
        StopAllCoroutines();
        base.On_GolemAreGoingToDie();
        ResetAll();
    }

    void ResetAll()
    {
        m_leftSignMesh.color = new Color(0, 0, 0, 0);
        m_middleSignMesh.color = new Color(0, 0, 0, 0);
        m_rightSignMesh.color = new Color(0, 0, 0, 0);
        
        if (m_impactSounds.Count > 0)
        {
            for (int i = 0, l = m_impactSounds.Count; i < l; ++i)
            {
                Destroy(m_impactSounds[i].gameObject);
            }
        }
    }

}
