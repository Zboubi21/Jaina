using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartParticles : MonoBehaviour {

    [SerializeField] Animation m_anim;
    [SerializeField] ParticleSystem[] m_particleSystem;
    
    public void StartParticle(){
        Anim();
        Particle();
    }

    void Anim(){
        if(m_anim != null){
            m_anim.Play();
        }
    }

    void Particle(){
        for (int i = 0, l = m_particleSystem.Length; i < l; ++i) {
            m_particleSystem[i].Stop(true);
            m_particleSystem[i].Play(true);
        }
    }

}
