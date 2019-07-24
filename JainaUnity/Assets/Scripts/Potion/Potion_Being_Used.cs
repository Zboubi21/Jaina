﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class Potion_Being_Used : MonoBehaviour
{
    public int heal_Amount;

    [SerializeField] GameObject m_takingPotionFx;

    ParticleSystem[] particle;
    Animator anim;
    ObjectPooler m_objectPooler;

    void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
        particle = GetComponentsInChildren<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
    }
    private void OnEnable()
    {
        gameObject.GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterStats stats = other.GetComponent<CharacterStats>();
            if(stats.CurrentHealth != stats.maxHealth)
            {
                anim.SetTrigger("Used");
                particle[0].Stop();
                particle[1].Play();
                stats.HealDamage(heal_Amount);
                StartCoroutine(waitendofanim());
                Level.AddFX(m_takingPotionFx, Vector3.zero, Quaternion.identity);
            }
        }
    }

    IEnumerator waitendofanim()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(0.75f);
        // gameObject.SetActive(false);
        m_objectPooler.ReturnObjectToPool(ObjectType.LifePotion, gameObject);
    }

}
