using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion_Being_Used : MonoBehaviour
{
    public int heal_Amount;


    ParticleSystem[] particle;
    Animator anim;
    void Start()
    {
        particle = GetComponentsInChildren<ParticleSystem>();
        anim = GetComponent<Animator>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterStats stats = other.GetComponent<CharacterStats>();
            anim.SetTrigger("Used");
            particle[0].Stop();
            particle[1].Play();
            if (stats.CurrentHealth + heal_Amount >= stats.maxHealth)
            {
                stats.CurrentHealth = stats.maxHealth;
            }
            else
            {
                stats.CurrentHealth += heal_Amount;
            }
            StartCoroutine(waitendofanim());
        }
    }

    IEnumerator waitendofanim()
    {
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }

}
