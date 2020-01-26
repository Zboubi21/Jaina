using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAmbienceSoundManager : MonoBehaviour
{
    
    [Header("P1")]
    [SerializeField] Sounds m_p1Sounds;

    [Header("P2")]
    [SerializeField] Sounds m_p2Sounds;

    [Header("P3")]
    [SerializeField] Sounds m_p3Sounds;
    [Space]
    [SerializeField] float m_additionnalWaitTimeToEndFightSound = 2;
    [SerializeField] AudioSource m_endFight;

    [System.Serializable] class Sounds
    {
        public AudioSource m_startSound;
        public AudioSource m_loopSound;
    }

    IEnumerator StartSoundWithDelay(AudioSource audio, float delay)
    {
        yield return new WaitForSeconds(delay);
        audio.Play();
    }

    public void On_GolemStartFight()
    {
        m_p1Sounds.m_startSound.Play();
        StartCoroutine(StartSoundWithDelay(m_p1Sounds.m_loopSound, m_p1Sounds.m_startSound.clip.length));
    }
    public void On_GolemSwitchToP2()
    {
        m_p1Sounds.m_loopSound.Stop();
        m_p2Sounds.m_startSound.Play();
        StartCoroutine(StartSoundWithDelay(m_p2Sounds.m_loopSound, m_p2Sounds.m_startSound.clip.length));
    }
    public void On_GolemSwitchToP3()
    {
        m_p2Sounds.m_loopSound.Stop();
        m_p3Sounds.m_startSound.Play();
        StartCoroutine(StartSoundWithDelay(m_p3Sounds.m_loopSound, m_p3Sounds.m_startSound.clip.length));
    }
    public void On_GolemDie()
    {
        StartCoroutine(StartDeathSound());
    }
    IEnumerator StartDeathSound()
    {
        yield return new WaitForSeconds(m_additionnalWaitTimeToEndFightSound);
        m_p3Sounds.m_loopSound.Stop();
        m_endFight.Play();
    }

}
