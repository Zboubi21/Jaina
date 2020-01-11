using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressControlV3D : MonoBehaviour {

    [Header("Anim")]
    public float m_timeToChangeMaxDistance = 0.5f;
    public AnimationCurve m_changeDistCurve;

    [Header("ProgressControlV3D")]
    public bool changeAllMaxLength = true;
    public float maxLength = 32f;
    public float globalProgressSpeed = 1f;
    public float globalImpactProgressSpeed = 1f;
    public bool always = true;
    public bool colorizeAll = true;
    public Color finalColor;
    [Range(0.2f, 1.0f)]
    public float gammaLinear = 1f;
    public Renderer meshRend;
    public float meshRendPower = 3f;
    public Light pointLight;
    public StartPointEffectControllerV3D startPointEffect;
    public EndPointEffectControllerV3D endPointEffect;
    public SmartWaveParticlesControllerV3D smartWaveParticles;
    public SFXControllerV3D sfxcontroller;

    private float globalProgress;
    private float globalImpactProgress;
    private LaserLineV3D[] lls;
    private LightLineV3D[] lils;
    private Renderer[] renderers;

    bool m_laserIsActive = false;
    float m_actualLength = 0;
    IEnumerator m_changeLaserDistance;

    private void Start()
    {
        globalProgress = 1f;
        globalImpactProgress = 1f;
        lls = GetComponentsInChildren<LaserLineV3D>(true);
        lils = GetComponentsInChildren<LightLineV3D>(true);
        renderers = GetComponentsInChildren<Renderer>(true);
        // StartLaserFx();
        // Invoke("StopLaserFx", 5);
    }

    public void StartLaserFx()
    {
        m_laserIsActive = true;
        globalProgress = 0f;
        endPointEffect.emit = true;
        globalImpactProgress = 0f;
        sfxcontroller.StartSound();
        StartChangeMaxLaserDistanceCorout(ChangeMaxLaserDistance(m_actualLength, maxLength));
    }
    public void StopLaserFx()
    {
        StartChangeMaxLaserDistanceCorout(ChangeMaxLaserDistance(m_actualLength, 0));
    }
    void StartChangeMaxLaserDistanceCorout(IEnumerator corout)
    {
        if(m_changeLaserDistance != null)
        {
            StopCoroutine(m_changeLaserDistance);
        }
        m_changeLaserDistance = corout;
        StartCoroutine(m_changeLaserDistance);
    }
    IEnumerator ChangeMaxLaserDistance(float fromDist, float toDist)
    {
        float fracJourney = 0;
        float distance = Mathf.Abs(fromDist - toDist);
        float vitesse = distance / m_timeToChangeMaxDistance;
        float actualValue = fromDist;

        while (actualValue != toDist)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualValue = Mathf.Lerp(fromDist, toDist, m_changeDistCurve.Evaluate(fracJourney));
            m_actualLength = actualValue;
            yield return null;
        }

        if(toDist == 0)
        {
            m_laserIsActive = false;
            endPointEffect.emit = false;
        }
    }

    public void ChangeColor(Color color)
    {
        finalColor = color;
    }

    void Update()
    {
        // Control Gamma and Linear modes
        foreach (Renderer rend in renderers)
        {
            rend.material.SetFloat("_GammaLinear", gammaLinear);
        }

        // Sending global progress value to other scripts
        startPointEffect.SetGlobalProgress(globalProgress);
        startPointEffect.SetGlobalImpactProgress(globalImpactProgress);
        endPointEffect.SetGlobalProgress(globalProgress);
        endPointEffect.SetGlobalImpactProgress(globalImpactProgress);
        smartWaveParticles.SetGlobalProgress(globalProgress);

        // Color control of all child prefabs
        if (colorizeAll == true)
        {
            foreach (LightLineV3D lil in lils)
            {
                lil.SetFinalColor(finalColor);
            }            
            startPointEffect.SetFinalColor(finalColor);
            endPointEffect.SetFinalColor(finalColor);
            foreach (Renderer rend in renderers)
            {
                rend.material.SetColor("_FinalColor", finalColor);
            }
        }        

        // Overall progress control
        if (meshRend != null)
        {
            meshRend.material.SetColor("_EmissionColor", finalColor * meshRendPower);
        }

        if (globalProgress < 1f)
        {
            globalProgress += Time.deltaTime * globalProgressSpeed;
        }

        if (globalImpactProgress < 1f)
        {
            globalImpactProgress += Time.deltaTime * globalImpactProgressSpeed;
        }

        // if (Input.GetMouseButton(0) || always == true)
        // {
        //     globalProgress = 0f;
        //     endPointEffect.emit = true;
        // }
        // else
        // {
        //     endPointEffect.emit = false;
        // }

        // if (Input.GetMouseButtonDown(0))
        // {
        //     globalImpactProgress = 0f;
        // }
        
        if (m_laserIsActive)
        {
            globalProgress = 0f;
        }

        // if (always == true)
        // {
        //     globalProgress = 0f;
        // }

        foreach (LaserLineV3D ll in lls)
        {
            ll.SetGlobalProgress(globalProgress);
            ll.SetGlobalImpactProgress(globalImpactProgress);
            if (changeAllMaxLength == true)
            {
                ll.maxLength = m_actualLength;
            }            
        }

        foreach (LightLineV3D lil in lils)
        {
            lil.SetGlobalProgress(globalProgress);
            lil.SetGlobalImpactProgress(globalImpactProgress);
            if (changeAllMaxLength == true)
            {
                lil.maxLength = m_actualLength;
            }
        }

        sfxcontroller.SetGlobalProgress(1f - globalProgress);
    }
}
