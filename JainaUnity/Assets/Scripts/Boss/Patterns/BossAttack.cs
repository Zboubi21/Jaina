using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    GolemController m_golemController;
    public GolemController GolemController { get { return m_golemController; } set { m_golemController = value; } }

    public virtual void On_AttackBegin(int phaseNbr)
    {
        // Debug.Log("On_AttackBegin: " + this);
    }
    public virtual void On_AttackEnd()
    {
        // Debug.Log("On_AttackEnd: " + this);
        if(m_golemController != null)
        {
            m_golemController.On_AttackIsFinished();
        }
    }
    public virtual void On_GolemAreGoingToDie()
    {
        if(m_golemController.transform.eulerAngles.y != m_golemController.YStartRotation)
        {
            StopAllCoroutines();
            StartCoroutine(RotateGolemBeforeGolemDie());
        }
        else
        {
            m_golemController.On_GolemDie();
        }
    }
    IEnumerator RotateGolemBeforeGolemDie()
    {
        yield return StartCoroutine(RotateGolemToLookAtPointWithTime(m_golemController.YStartRotation, m_golemController.m_die.m_changeYRotSpeed, m_golemController.m_die.m_changeYRotCurve, true));
        m_golemController.On_GolemDie();
    }


    protected IEnumerator RotateGolemToLookAtPointWithTime(float toRot, float timeToRotate, AnimationCurve rotateCurve, bool useSpeedInsteadOfTime = false)
    {
        float fromRot = m_golemController.transform.rotation.eulerAngles.y;
        float fracJourney = 0;
        float distance = Mathf.Abs(fromRot - toRot);
        // float vitesse = distance / timeToRotate;
        float vitesse = !useSpeedInsteadOfTime ? distance / timeToRotate : timeToRotate;
        float actualValue = fromRot;

        while (actualValue != toRot)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualValue = Mathf.Lerp(fromRot, toRot, rotateCurve.Evaluate(fracJourney));
            m_golemController.transform.eulerAngles = new Vector3(m_golemController.transform.rotation.eulerAngles.x, actualValue, m_golemController.transform.rotation.eulerAngles.z);
            yield return null;
        }
    }
    protected void RotateGolemToLookAtPoint(Transform lookAtPoint)
    {
        m_golemController.transform.LookAt(lookAtPoint);
        m_golemController.transform.localEulerAngles = new Vector3(0, m_golemController.transform.localEulerAngles.y, m_golemController.transform.localEulerAngles.z);
    }

}
