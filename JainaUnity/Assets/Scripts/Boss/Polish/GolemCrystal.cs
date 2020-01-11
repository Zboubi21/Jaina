using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemCrystal : MonoBehaviour
{
    
    [Header("Light anim")]
    [SerializeField] float m_timeToLightOff = 5;
    [SerializeField] AnimationCurve m_lightOffCurve;

    [Header("Crystal emissive anim")]
    [SerializeField] Color m_crystalEmissiveColor;
    [SerializeField] float m_timeToOffCrystalEmissive = 5;
    [SerializeField] AnimationCurve m_crystalEmissiveCurve;

    Light m_crystalLight;
    MeshRenderer m_crystalMesh;

    void Start()
    {
        m_crystalLight = GetComponentInChildren<Light>();
        m_crystalMesh = GetComponent<MeshRenderer>();
        m_crystalMesh.material.EnableKeyword("_EMISSION");
    }

    public void On_CrystalDie()
    {
        StartCoroutine(LightOff());
        StartCoroutine(CrystalEmissiveOff());
    }
    IEnumerator LightOff()
    {
        float fromLight = m_crystalLight.intensity;
        float actualLight = fromLight;
        float toLight = 0;
        
        float fracJourney = 0;
        float distance = Mathf.Abs(fromLight - toLight);
        float vitesse = distance / m_timeToLightOff;

        while (actualLight != toLight)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualLight = Mathf.Lerp(fromLight, toLight, m_lightOffCurve.Evaluate(fracJourney));
            m_crystalLight.intensity = actualLight;
            yield return null;
        }
    }
    IEnumerator CrystalEmissiveOff()
    {
        Color fromColor = m_crystalMesh.material.GetColor("_EmissionColor");
        Color actualColor = fromColor;
        Color toColor = m_crystalEmissiveColor;
        
        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float vitesse = distance / m_timeToOffCrystalEmissive;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualColor = Color.Lerp(fromColor, toColor, m_crystalEmissiveCurve.Evaluate(fracJourney));
            m_crystalMesh.material.SetColor("_EmissionColor", actualColor);
            yield return null;
        }
    }

}
