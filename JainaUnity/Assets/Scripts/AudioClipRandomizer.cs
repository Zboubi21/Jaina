using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipRandomizer : MonoBehaviour
{
    public AudioClip[] clips;
    AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
        int random = Random.Range(0, clips.Length);
        source.clip = clips[random];
        source.enabled = true;
    }
}
