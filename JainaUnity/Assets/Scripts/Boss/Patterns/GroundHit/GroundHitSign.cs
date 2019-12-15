using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHitSign : MonoBehaviour
{
    
    [Header("Anim Movement")]
    public Vector3 m_startLocalPos;
    public Vector3 m_endLocalPos;
    public float m_timeToDoMoveAnim = 5;
    public AnimationCurve m_moveCurve;

    MeshRenderer m_mesh;

    void Start()
    {
        m_mesh = GetComponent<MeshRenderer>();
        m_mesh.enabled = false;
    }

    public void StartToMove()
    {
        m_mesh.enabled = true;
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
        m_mesh.enabled = false;
    }

}
