using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundHitSign : MonoBehaviour
{
    
    [Header("Anim Movement")]
    public Vector3 m_startLocalPos;
    public Vector3 m_endLocalPos;
    public float m_timeToDoMoveAnim = 5;
    public AnimationCurve m_moveCurve;

    [Header("Anim Color")]
    [SerializeField] AnimColor m_animColor;
    [System.Serializable] public class AnimColor {
        public float m_delayBeforStart = 0;
        public Color m_startColor;
        public Color m_endColor;
        public float m_timeToDoColorAnim = 5;
        public AnimationCurve m_colorCurve;
    }

    Image m_image;

    void Start()
    {
        m_image = GetComponent<Image>();
        m_image.enabled = false;
    }

    IEnumerator HitMoveSign()
    {
        Vector3 fromPos = m_startLocalPos;
        Vector3 toPos = m_endLocalPos;

        float fracJourney = 0;
        float distance = Vector3.Distance(m_startLocalPos, m_endLocalPos);
        float vitesse = distance / m_timeToDoMoveAnim;

        while (transform.localPosition != toPos)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            transform.localPosition = Vector3.Lerp(fromPos, toPos, m_moveCurve.Evaluate(fracJourney));
            yield return null;
        }
        m_image.enabled = false;
    }
    IEnumerator HitColorSign()
    {
        Color fromColor = m_animColor.m_startColor;
        Color toColor = m_animColor.m_endColor;

        m_image.color = fromColor;

        yield return new WaitForSeconds(m_animColor.m_delayBeforStart);

        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float vitesse = distance / m_animColor.m_timeToDoColorAnim;
        Color actualColor = fromColor;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualColor = Color.Lerp(fromColor, toColor, m_animColor.m_colorCurve.Evaluate(fracJourney));
            m_image.color = actualColor;
            yield return null;
        }
    }

    public void StartToMove()
    {
        m_image.enabled = true;
        transform.localPosition = m_startLocalPos;
        StartCoroutine(HitMoveSign());
    }
    public void StartToChangeColor()
    {
        m_image.enabled = true;
        StartCoroutine(HitColorSign());
    }
    public void StopAllGroundHitCoroutine()
    {
        StopAllCoroutines();
    }

}
