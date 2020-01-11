using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayFX : FX
{
    [SerializeField, Range(0, 10)] float m_soundDelay = 0.5f;

    void Awake()
    {
        Invoke("StartWithDelay", m_soundDelay);
    }

    void StartWithDelay()
    {
		GetComponent<AudioSource>().Play();
        base.Start();
    }

    protected override void Start()
    {

    }

}
