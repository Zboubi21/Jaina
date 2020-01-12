using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealHandeler : MonoBehaviour
{
    public bool useHealUI;
    [Space]
    public int healAmount;
    [Space]
    public int maxHealCount;
    int _currentHealCount;
    [Space]
    public float healCooldown;
    float _currenthealCooldown;
    [Space]
    public ArmedialLightReference artefactRef;
    public AnimationCurve artefactLightPathCurve;
    public float timeToLightFeedBack;
    public GameObject m_healSFX;
    public GameObject m_healVFX;
    public GameObject m_healVFX_ForArmedial;

    ReferenceScript _getImg;
    CharacterStats stats;

    private void Start()
    {
        _getImg = GetComponent<ReferenceScript>();
        _currentHealCount = maxHealCount;
        if(artefactRef != null)
        {
            artefactRef.gameObject.GetComponent<MeshRenderer>().material = artefactRef.mats[0];
        }

        if (!useHealUI)
        {
            gameObject.SetActive(false);
        }

        stats = PlayerManager.Instance.GetComponent<CharacterStats>();
    }


    private void Update()
    {
        HealCoolDownHandeler();
    }

    public void HealEffect()
    {
        if(!useHealUI)
        {
            return;
        }
        if (_currentHealCount - 1 >= 0 && stats.HealDamage(healAmount))
        {
            _currentHealCount--;
            if(artefactRef != null)
            {
                StartCoroutine(ArmedialsHealFeedBack());
                Level.AddFX(m_healVFX_ForArmedial, artefactRef.VFX_Spawn.position, artefactRef.VFX_Spawn.rotation);
            }
            Level.AddFX(m_healSFX, Vector3.zero, Quaternion.identity);
            Level.AddFX(m_healVFX, stats.gameObject.transform.position, stats.gameObject.transform.rotation);
        }
    }


    IEnumerator ArmedialsHealFeedBack()
    {
        artefactRef.gameObject.GetComponent<MeshRenderer>().material = artefactRef.mats[1];

        float _currentTimeOfAnimation = 0;

        for (int i = 0, l = artefactRef.lights.Length; i < l; ++i)
        {
            artefactRef.lights[i].transform.position = artefactRef.lights[i].gameObject.GetComponent<ArmedialLightPathReference>().startPoint.position;
        }

        while (_currentTimeOfAnimation / timeToLightFeedBack <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            for (int i = 0, l = artefactRef.lights.Length; i < l; ++i)
            {
                float startPointY = artefactRef.lights[i].gameObject.GetComponent<ArmedialLightPathReference>().startPoint.position.y;
                float endPointY = artefactRef.lights[i].gameObject.GetComponent<ArmedialLightPathReference>().endPoint.position.y;
                Vector3 posY = artefactRef.lights[i].transform.position;
                float evaluate = artefactLightPathCurve.Evaluate(_currentTimeOfAnimation / timeToLightFeedBack);
                posY.y = Mathf.Lerp(startPointY, endPointY, evaluate);
                artefactRef.lights[i].transform.position = posY;
                artefactRef.lights[i].intensity = Mathf.Lerp(0, 2, _currentTimeOfAnimation / timeToLightFeedBack);
            }

        }
        _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / timeToLightFeedBack <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            for (int i = 0, l = artefactRef.lights.Length; i < l; ++i)
            {
                artefactRef.lights[i].intensity = Mathf.Lerp(2, 0, _currentTimeOfAnimation / timeToLightFeedBack);
            }

        }
        _currentTimeOfAnimation = 0;
        artefactRef.gameObject.GetComponent<MeshRenderer>().material = artefactRef.mats[0];

    }

    public void HealCoolDownHandeler()
    {
        if (_currentHealCount < maxHealCount)
        {
            _currenthealCooldown += Time.deltaTime;

            if (_currentHealCount > 0)
            {
                _getImg.marksArray[1].fillAmount = Mathf.InverseLerp(0, healCooldown, _currenthealCooldown);
            }
            else
            {
                _getImg.marksArray[1].fillAmount = 0;
                _getImg.marksArray[0].fillAmount = Mathf.InverseLerp(0, healCooldown, _currenthealCooldown);
            }

            if (_currenthealCooldown >= healCooldown)
            {
                _currenthealCooldown = 0;
                _currentHealCount++;
            }
        }
        else
        {
            _currentHealCount = maxHealCount;
        }

        _getImg.count.text = string.Format("x {0}", _currentHealCount);

    }
}
