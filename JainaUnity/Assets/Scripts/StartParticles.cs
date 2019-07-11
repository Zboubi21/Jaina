using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartParticles : MonoBehaviour {

    [SerializeField] ParticleSystem[] m_particleSystem;
    
    public void StartParticle(){
        for (int i = 0, l = m_particleSystem.Length; i < l; ++i) {
            m_particleSystem[i].Stop(true);
            m_particleSystem[i].Play(true);
        }
    }

}
