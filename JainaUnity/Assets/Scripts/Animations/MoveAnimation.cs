using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveAnimation : MonoBehaviour {

    [Header("Local positions")]
    [SerializeField] Vector3 m_startPosition;
    [SerializeField] Vector3 m_desiredPosition;

    [Header("Animations")]
    [SerializeField] float m_timeToDoAnimation = 3;
    [SerializeField] AnimationCurve m_animationCurve;

    [Header("Event")]
    public bool m_useEvents = true;
	public Event[] m_events;
	[System.Serializable] public class Event {
        [Range(0,1)] public float m_timeToDoEvent = 0.5f;
        public UnityEvent m_event;

        [HideInInspector] public bool m_eventIsInvoke = false;
    }

    void Start(){
        transform.localPosition = m_startPosition;
    }

    public void DoMoveAnimation(){
        StartCoroutine(MovePositionCorout());
    }

    IEnumerator MovePositionCorout(){

		float moveFracJourney = new float();
        float moveJourneyLength = Vector3.Distance(m_startPosition, m_desiredPosition);
        float animationSpeed = moveJourneyLength / m_timeToDoAnimation;

        if(m_useEvents){
            InitializeEvents();
        }
		while(transform.localPosition != m_desiredPosition){
			moveFracJourney += (Time.deltaTime) * animationSpeed / moveJourneyLength;
			transform.localPosition = Vector3.Lerp(m_startPosition, m_desiredPosition, m_animationCurve.Evaluate(moveFracJourney));

            if(m_useEvents){
                CheckTimeToDoEvent(moveFracJourney);
            }
			yield return null;
		}
	}

    void InitializeEvents(){
        for (int i = 0, l = m_events.Length; i < l; ++i) {
            m_events[i].m_eventIsInvoke = false;
        }
    }
    void CheckTimeToDoEvent(float moveFracJourney){
        for (int i = 0, l = m_events.Length; i < l; ++i) {
            if(moveFracJourney > m_events[i].m_timeToDoEvent && !m_events[i].m_eventIsInvoke){
                m_events[i].m_eventIsInvoke = true;
                m_events[i].m_event.Invoke();
            }
        }
    }

}