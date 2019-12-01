using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaArea : Spell
{
    [Header("Damages")]
    [SerializeField] float m_lavaTick = 0.25f;
    
    // 100 dégâts en 5s toute les 0.25s = 5 damage/s
    [SerializeField] int m_lavaTickDamage = 5;

    [Header("Animation")]
    [SerializeField] Vector3 m_fromScale = Vector3.zero;
    [SerializeField] Vector3 m_toScale = Vector3.one;
    [SerializeField] float m_timeToDoAnim = 0.25f;
    [SerializeField] AnimationCurve m_animCurve;

    void OnEnable()
    {
        StartCoroutine(SpawnLavaAnimation());
    }

    void OnTriggerEnter(Collider col)
    {
		if(col.CompareTag("Player") || col.CompareTag("Enemy")){
            // Debug.Log("OnTriggerEnter");
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();

            if(CharacterStats.LavaTick != m_lavaTick)
            {
                CharacterStats.LavaTick = m_lavaTick;
            }

            if(CharacterStats.LavaTickDamage != m_lavaTickDamage)
            {
                CharacterStats.LavaTickDamage = m_lavaTickDamage;
            }

            CharacterStats.OnCharacterEnterInLavaArea();

            // CharacterStats.TakeDamage(m_damage);
        }

	}
    void OnTriggerExit(Collider col)
    {
		if(col.CompareTag("Player") || col.CompareTag("Enemy"))
        {
            // Debug.Log("OnTriggerExit");
            CharacterStats = col.gameObject.GetComponent<CharacterStats>();
            CharacterStats.OnCharacterExitInLavaArea();
        }
    }

    IEnumerator SpawnLavaAnimation()
    {
        SetAreaScale(m_fromScale);

        float fracJourney = 0;
        float distance = Vector3.Distance(m_fromScale, m_toScale);
        float vitesse = distance / m_timeToDoAnim;
        Vector3 actualScale = m_fromScale;

        while (transform.localScale != m_toScale)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualScale = Vector3.Lerp(m_fromScale, m_toScale, m_animCurve.Evaluate(fracJourney));
            SetAreaScale(actualScale);
            yield return null;
        }    
    }

    void SetAreaScale(Vector3 newScale)
    {
        transform.localScale = newScale;
    }

}
