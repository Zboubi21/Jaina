using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatCheckpoint : MonoBehaviour {

    [Header("Animations")]
    [SerializeField] float m_startYPosition = - 42;
    [SerializeField] float m_endYPosition = - 5;
    [Space]
    [SerializeField] float m_timeToDoAnimation = 3;
    [SerializeField] AnimationCurve m_animationCurve;
    
    Vector3 m_desiredPosition;

    void Start(){
        transform.position = new Vector3(transform.position.x, m_startYPosition, transform.position.z);
        m_desiredPosition = new Vector3(transform.position.x, m_endYPosition, transform.position.z);
    }

    public void On_CheckpointIsTake(){
        StartCoroutine(MovePositionCorout());
    }

    IEnumerator MovePositionCorout(){

		float moveFracJourney = new float();
        float moveJourneyLength = Vector3.Distance(transform.position, m_desiredPosition);
        float animationSpeed = moveJourneyLength / m_timeToDoAnimation;

		while(transform.position != m_desiredPosition){
			moveFracJourney += (Time.deltaTime) * animationSpeed / moveJourneyLength;
			transform.position = Vector3.Lerp(transform.position, m_desiredPosition, m_animationCurve.Evaluate(moveFracJourney));
			yield return null;
		}
	}

}
