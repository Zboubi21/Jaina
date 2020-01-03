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

    Image m_image;

    void Start()
    {
        m_image = GetComponent<Image>();
        m_image.enabled = false;
    }

    public void StartToMove()
    {
        m_image.enabled = true;
        transform.localPosition = m_startLocalPos;
        StartCoroutine(HitMoveSign());
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

}
