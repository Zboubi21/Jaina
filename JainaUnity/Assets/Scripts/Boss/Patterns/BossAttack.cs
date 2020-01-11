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
        if(GolemController != null)
        {
            GolemController.On_AttackIsFinished();
        }
    }


    public IEnumerator RotateGolemToLookAtPointWithTime(float toRot, float timeToRotate, AnimationCurve rotateCurve)
    {
        float fromRot = GolemController.transform.rotation.eulerAngles.y;

        float fracJourney = 0;
        float distance = Mathf.Abs(fromRot - toRot);
        float vitesse = distance / timeToRotate;
        float actualValue = fromRot;

        while (actualValue != toRot)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualValue = Mathf.Lerp(fromRot, toRot, rotateCurve.Evaluate(fracJourney));
            GolemController.transform.eulerAngles = new Vector3(GolemController.transform.rotation.eulerAngles.x, actualValue, GolemController.transform.rotation.eulerAngles.z);
            yield return null;
        }
    }
    public void RotateGolemToLookAtPoint(Transform lookAtPoint)
    {
        GolemController.transform.LookAt(lookAtPoint);
        GolemController.transform.localEulerAngles = new Vector3(0, GolemController.transform.localEulerAngles.y, GolemController.transform.localEulerAngles.z);
    }

}
