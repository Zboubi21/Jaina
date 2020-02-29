using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionDelay : MonoBehaviour
{
    
    [SerializeField] bool m_activeAtStart = false;

    [SerializeField] Delayer[] m_delayer;
    [System.Serializable] class Delayer
    {
        public float m_timeToDoAction = 1;
        public UnityEvent m_actionsToDo;
        [HideInInspector] public bool m_actionIsDone = false;
    }
    
    bool m_isActive;
    float m_timer = 0;

    void Start()
    {
        m_isActive = m_activeAtStart;
    }
    void Update()
    {
        if (!m_isActive)
            return;

        m_timer += Time.deltaTime;

        for (int i = 0, l = m_delayer.Length; i < l; ++i)
        {
            if (m_timer > m_delayer[i].m_timeToDoAction && !m_delayer[i].m_actionIsDone)
            {
                m_delayer[i].m_actionIsDone = true;
                m_delayer[i].m_actionsToDo.Invoke();
            }
        }
    }

    public void ActiveActionDelay()
    {
        m_isActive = true;
    }

}
