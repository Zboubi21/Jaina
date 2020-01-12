using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemTornado : MonoBehaviour
{

    [SerializeField] ChangeTornadoScale m_turnOn;
    [SerializeField] ChangeTornadoScale m_turnOff;
    [Space]
    [SerializeField] bool m_startOff = true;

    [Serializable] class ChangeTornadoScale
    {
        public Vector3 m_desiredScale = Vector3.zero;
        public float m_timeToScaleTornado = 5;
        public AnimationCurve m_scaleTornadoCurve;
    }

    void Start()
    {
        if(m_startOff)
        {
            transform.localScale = m_turnOff.m_desiredScale;
        }
        else
        {
            transform.localScale = m_turnOn.m_desiredScale;
        }
    }

    public void On_ScaleTornado(bool toSeeIt)
    {
        StopAllCoroutines();
        if(toSeeIt)
        {
            StartCoroutine(ScaleTornado(m_turnOn.m_desiredScale, m_turnOn.m_timeToScaleTornado, m_turnOn.m_scaleTornadoCurve));
        }
        else
        {
            StartCoroutine(ScaleTornado(m_turnOff.m_desiredScale, m_turnOff.m_timeToScaleTornado, m_turnOff.m_scaleTornadoCurve));
        }
    }
    IEnumerator ScaleTornado(Vector3 toScale, float timeToScale, AnimationCurve scaleCurve)
    {
        Vector3 fromScale = transform.localScale;
        Vector3 actualScale = fromScale;
        
        float fracJourney = 0;
        float distance = Vector3.Distance(fromScale, toScale);
        float vitesse = distance / timeToScale;

        while (actualScale != toScale)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualScale = Vector3.Lerp(fromScale, toScale, scaleCurve.Evaluate(fracJourney));
            transform.localScale = actualScale;
            yield return null;
        }
    }

}
