using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemTornado : MonoBehaviour
{

    [SerializeField] Vector3 m_desiredScale = Vector3.zero;
    [SerializeField] float m_timeToScaleTornado = 5;
    [SerializeField] AnimationCurve m_scaleTornadoCurve;

    public void On_ScaleTornado()
    {
        StartCoroutine(ScaleTornado());
    }
    IEnumerator ScaleTornado()
    {
        Vector3 fromScale = transform.localScale;
        Vector3 actualScale = fromScale;
        Vector3 toScale = m_desiredScale;
        
        float fracJourney = 0;
        float distance = Vector3.Distance(fromScale, toScale);
        float vitesse = distance / m_timeToScaleTornado;

        while (actualScale != toScale)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualScale = Vector3.Lerp(fromScale, toScale, m_scaleTornadoCurve.Evaluate(fracJourney));
            transform.localScale = actualScale;
            yield return null;
        }
    }

}
