using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaWaveController : BossAttack
{

    [SerializeField] Transform m_lavaWave;

    [Header("Y Position")]
    [SerializeField] Mover m_moveYPos;

    [Header("Z Position")]
    [SerializeField] Mover m_moveZPos;

    [System.Serializable] class Mover
    {
        public float m_delayToStart;
        public float m_fromValue;
        public float m_toValue;
        public float m_timeToMove;
        public AnimationCurve m_moveCurve;
    }

    [Header("Debug")]
    [SerializeField] bool m_useDebugInput = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && m_useDebugInput)
        {
            On_AttackBegin(0);
        }
    }

    IEnumerator MoveYPosition()
    {
        yield return new WaitForSeconds(m_moveYPos.m_delayToStart);

        float fracJourney = 0;
        float distance = Mathf.Abs(m_moveYPos.m_fromValue - m_moveYPos.m_toValue);
        float vitesse = distance / m_moveYPos.m_timeToMove;
        float actualValue = m_moveYPos.m_fromValue;

        while (actualValue != m_moveYPos.m_toValue)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualValue = Mathf.Lerp(m_moveYPos.m_fromValue, m_moveYPos.m_toValue, m_moveYPos.m_moveCurve.Evaluate(fracJourney));
            m_lavaWave.localPosition = new Vector3(m_lavaWave.localPosition.x, actualValue, m_lavaWave.localPosition.z);
            yield return null;
        }
    }
    IEnumerator MoveZPosition()
    {
        yield return new WaitForSeconds(m_moveZPos.m_delayToStart);

        float fracJourney = 0;
        float distance = Mathf.Abs(m_moveZPos.m_fromValue - m_moveZPos.m_toValue);
        float vitesse = distance / m_moveZPos.m_timeToMove;
        float actualValue = m_moveZPos.m_fromValue;

        while (actualValue != m_moveZPos.m_toValue)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualValue = Mathf.Lerp(m_moveZPos.m_fromValue, m_moveZPos.m_toValue, m_moveZPos.m_moveCurve.Evaluate(fracJourney));
            m_lavaWave.localPosition = new Vector3(m_lavaWave.localPosition.x, m_lavaWave.localPosition.y, actualValue);
            yield return null;
        }
        m_lavaWave.gameObject.SetActive(false);
    }

    public override void On_AttackBegin(int phaseNbr)
    {
        base.On_AttackBegin(phaseNbr);
        m_lavaWave.gameObject.SetActive(true);
        StartCoroutine(MoveYPosition());
        StartCoroutine(MoveZPosition());
    }

    public override void On_AttackEnd()
    {
        base.On_AttackEnd();
    }

    public override void On_GolemAreGoingToDie()
    {
        // StopAllCoroutines();
        base.On_GolemAreGoingToDie();
    }

}
