using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRotation : UseValueChanger
{

    [Header("Paramaters")]
    [SerializeField] bool m_launchAnimationAtStart = true;

    [Header("Animation")]
    [SerializeField] float m_timeToDoAnimation = 1;
    [SerializeField] Vector3 m_targetRotation;    
    [SerializeField] AnimationCurve m_animationCurve;

    void Start(){
        if(m_launchAnimationAtStart){
            StartAnimation();
        }
    }

    public void StartAnimation(){
        m_valueChangerBase.MoveRotationWithTime(transform, m_targetRotation, m_timeToDoAnimation, m_animationCurve);
    }

}
