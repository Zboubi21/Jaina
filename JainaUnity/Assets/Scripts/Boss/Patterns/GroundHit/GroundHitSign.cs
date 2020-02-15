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
    public bool m_hideImageAfterMove = true;

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
    LavaWaveController m_lavaWaveController;

    void Start()
    {
        m_image = GetComponent<Image>();
        EnableSignImg(false);
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
        if (m_hideImageAfterMove)
            EnableSignImg(false);
    }
    IEnumerator HitMoveSignWithSpeed(float xPos, float speed)
    {
        Vector3 fromPos = m_startLocalPos;
        fromPos.x = xPos;
        Vector3 toPos = m_endLocalPos;
        toPos.x = xPos;

        transform.localPosition = fromPos;

        float fracJourney = 0;
        float distance = Vector3.Distance(fromPos, toPos);

        while (transform.localPosition != toPos)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            transform.localPosition = Vector3.Lerp(fromPos, toPos, m_moveCurve.Evaluate(fracJourney));
            yield return null;
        }

        if (m_hideImageAfterMove)
            EnableSignImg(false);

        if (m_lavaWaveController != null)
            m_lavaWaveController.On_LavaWaveAreaStopped();
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

    void EnableSignImg(bool enable)
    {
        if (m_image != null)
            m_image.enabled = enable;
    }

    public void StartToMove()
    {
        EnableSignImg(true);
        transform.localPosition = m_startLocalPos;
        StartCoroutine(HitMoveSign());
    }
    public void StartToMoveWithSpeed(float xPos, float newSpeed, LavaWaveController lavaWaveController)
    {
        m_lavaWaveController = lavaWaveController;
        StartCoroutine(HitMoveSignWithSpeed(xPos, newSpeed));
    }
    public void StartToChangeColor()
    {
        EnableSignImg(true);
        StartCoroutine(HitColorSign());
    }
    public void StopShowSign()
    {
        EnableSignImg(false);
    }
    public void StopAllGroundHitCoroutine()
    {
        StopAllCoroutines();
    }

}
