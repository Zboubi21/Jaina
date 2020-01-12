using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemCrystal : MonoBehaviour
{
    [Header("Change Crystal")]
    [SerializeField] ChangeCrystalValues m_turnOn;
    [SerializeField] ChangeCrystalValues m_turnOff;
    [Space]
    [SerializeField] bool m_startOff = true;

    [Serializable] class ChangeCrystalValues
    {
        [Header("Light anim")]
        public float m_lightIntensity = 3;
        public float m_timeToChangeLight = 5;
        public AnimationCurve m_changeLightCurve;

        [Header("Crystal emissive anim")]
        public Color m_crystalEmissiveColor;
        public float m_timeToChangeCrystalEmissive = 5;
        public AnimationCurve m_crystalEmissiveCurve;
    }
    
    Light m_crystalLight;
    MeshRenderer m_crystalMesh;

    void Start()
    {
        m_crystalLight = GetComponentInChildren<Light>();
        m_crystalMesh = GetComponent<MeshRenderer>();
        m_crystalMesh.material.EnableKeyword("_EMISSION");

        if(m_startOff)
        {
            m_crystalLight.intensity = m_turnOff.m_lightIntensity;
            m_crystalMesh.material.SetColor("_EmissionColor", m_turnOff.m_crystalEmissiveColor);
        }
        else
        {
            m_crystalLight.intensity = m_turnOn.m_lightIntensity;
            m_crystalMesh.material.SetColor("_EmissionColor", m_turnOn.m_crystalEmissiveColor);
        }
    }

    public void On_CrystalLive(bool live)
    {
        StopAllCoroutines();
        if(live)
        {
            StartCoroutine(ChangeLight(m_turnOn.m_lightIntensity, m_turnOn.m_timeToChangeLight, m_turnOn.m_changeLightCurve));
            StartCoroutine(ChangeCrystalEmissive(m_turnOn.m_crystalEmissiveColor, m_turnOn.m_timeToChangeCrystalEmissive, m_turnOn.m_crystalEmissiveCurve));
        }
        else
        {
            StartCoroutine(ChangeLight(m_turnOff.m_lightIntensity, m_turnOff.m_timeToChangeLight, m_turnOff.m_changeLightCurve));
            StartCoroutine(ChangeCrystalEmissive(m_turnOff.m_crystalEmissiveColor, m_turnOff.m_timeToChangeCrystalEmissive, m_turnOff.m_crystalEmissiveCurve));
        }
    }
    IEnumerator ChangeLight(float toLight, float timeToChangeValue, AnimationCurve changeCurve)
    {
        float fromLight = m_crystalLight.intensity;
        float actualLight = fromLight;
        
        float fracJourney = 0;
        float distance = Mathf.Abs(fromLight - toLight);
        float vitesse = distance / timeToChangeValue;

        while (actualLight != toLight)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualLight = Mathf.Lerp(fromLight, toLight, changeCurve.Evaluate(fracJourney));
            m_crystalLight.intensity = actualLight;
            yield return null;
        }
    }
    IEnumerator ChangeCrystalEmissive(Color toColor, float timeToChangeValue, AnimationCurve changeCurve)
    {
        Color fromColor = m_crystalMesh.material.GetColor("_EmissionColor");
        Color actualColor = fromColor;
        
        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float vitesse = distance / timeToChangeValue;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualColor = Color.Lerp(fromColor, toColor, changeCurve.Evaluate(fracJourney));
            m_crystalMesh.material.SetColor("_EmissionColor", actualColor);
            yield return null;
        }
    }

}
