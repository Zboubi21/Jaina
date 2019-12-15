using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHitController : MonoBehaviour
{
    [Header("Debug")]
    [Range(1, 3), SerializeField] int m_phaseNbr = 1;
    [SerializeField] KeyCode m_testInput = KeyCode.K;
    [SerializeField] AreaType m_actualArea = AreaType.Left;
    
    [Header("Area")]
    [SerializeField] GroundHitArea m_leftArea;
    [SerializeField] GroundHitArea m_middleArea;
    [SerializeField] GroundHitArea m_rightArea;

    [Header("Sign")]
    [SerializeField] MeshRenderer m_leftSignMesh;
    [SerializeField] MeshRenderer m_middleSignMesh;
    [SerializeField] MeshRenderer m_rightSignMesh;

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
    public float m_timeToDoDamage = 5;
    public int m_damage = 25;

    [Header("Delay")]
    [SerializeField] float m_delayBetweenHit = 5;

    GroundHitSign m_leftGroundHitSign;
    GroundHitSign m_middleGroundHitSign;
    GroundHitSign m_rightGroundHitSign;

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
        if(Input.GetKeyDown(m_testInput))
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
        StartCoroutine(WaitTimeToDoDamage(areaType));
    }

    void ShowHitSign(AreaType areaType)
    {
        switch (areaType)
        {
            case AreaType.Left:
                StartCoroutine(HitColorSign(m_leftSignMesh));
                m_leftGroundHitSign.StartToMove();
            break;

            case AreaType.Middle:
                StartCoroutine(HitColorSign(m_middleSignMesh));
                m_middleGroundHitSign.StartToMove();
            break;

            case AreaType.Right:
                StartCoroutine(HitColorSign(m_rightSignMesh));
                m_rightGroundHitSign.StartToMove();
            break;
        }
    }
    
    IEnumerator HitColorSign(MeshRenderer mesh)
    {
        Color fromColor = m_animColor.m_startColor;
        Color toColor = m_animColor.m_endColor;

        mesh.material.color = fromColor;

        yield return new WaitForSeconds(m_animColor.m_delayBeforStart);

        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float vitesse = distance / m_animColor.m_timeToDoColorAnim;
        Color actualColor = fromColor;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualColor = Color.Lerp(fromColor, toColor, m_animColor.m_colorCurve.Evaluate(fracJourney));
            mesh.material.color = actualColor;
            yield return null;
        }
    }
    
    IEnumerator WaitTimeToDoDamage(AreaType areaType)
    {
        yield return new WaitForSeconds(m_timeToDoDamage);
        CheckDamageArea(areaType);
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
    
}
