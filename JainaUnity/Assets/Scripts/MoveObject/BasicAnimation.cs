using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimation : UseValueChanger
{

    [Header("Paramaters")]
    [SerializeField] bool m_launchAnimationAtStart = true;

    [Header("Animation")]
    [SerializeField] float m_timeToDoAnimation = 1;
    [SerializeField] Transform m_targetPosition;    
    [SerializeField] AnimationCurve m_animationCurve;

    void Start(){
        if(m_launchAnimationAtStart){
            StartAnimation();
        }
    }

    public void StartAnimation(){
        m_valueChangerBase.MovePositionWithTime(transform, m_targetPosition.position, m_timeToDoAnimation, m_animationCurve);
    }

}
