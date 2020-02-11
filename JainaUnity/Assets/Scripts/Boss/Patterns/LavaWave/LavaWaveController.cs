using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaWaveController : BossAttack
{

#region SerializeField Variables
    [SerializeField] Transform m_lavaWave;

    [Header("Positions")]
    [SerializeField] Positions m_right;
    [SerializeField] Positions m_left;

    [System.Serializable] class Positions
    {
        public float m_posValue;
        public Transform m_targetPos;
        public GroundHitSign m_hitSign;
    }

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

    [Space]
    [SerializeField] float m_waitTimeToShowHitSings = 10;

    [Header("Debug")]
    [SerializeField] bool m_useDebugInput = false;
#endregion

#region Private Variables
    PlayerManager m_playerManager;
#endregion

#region Event Functions
    void Start()
    {
        m_playerManager = PlayerManager.Instance;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && m_useDebugInput)
        {
            On_AttackBegin(0);
        }
    }
#endregion

#region Private Functions
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

    IEnumerator WaitTimeToStopShowHitSigns()
    {
        yield return new WaitForSeconds(m_waitTimeToShowHitSings);
        m_right.m_hitSign.StopShowSign();
        m_left.m_hitSign.StopShowSign();
    }

    bool PlayerPosIsClosestToRightPos()
    {
        float rightDistance = Vector3.Distance(m_playerManager.transform.position, m_right.m_targetPos.position);
        float leftDistance = Vector3.Distance(m_playerManager.transform.position, m_left.m_targetPos.position);
        if (rightDistance < leftDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    float LavaXPos()
    {
        if (PlayerPosIsClosestToRightPos())
        {
            return m_right.m_posValue;
        }
        else
        {
            return m_left.m_posValue;
        }
    }
    
#endregion

#region Public Functions
    public override void On_AttackBegin(int phaseNbr)
    {
        base.On_AttackBegin(phaseNbr);
        m_lavaWave.gameObject.SetActive(true);

        float yPos = PlayerPosIsClosestToRightPos() ? m_lavaWave.localPosition.y : - m_lavaWave.localPosition.y;
        m_lavaWave.localPosition = new Vector3(LavaXPos(), yPos, m_lavaWave.localPosition.z);
        if(PlayerPosIsClosestToRightPos())
        {
            m_right.m_hitSign.StartToMove();
            m_right.m_hitSign.StartToChangeColor();
        }
        else
        {
            m_left.m_hitSign.StartToMove();
            m_left.m_hitSign.StartToChangeColor();
        }
        StartCoroutine(MoveYPosition());
        StartCoroutine(MoveZPosition());
        StartCoroutine(WaitTimeToStopShowHitSigns());
    }

    public override void On_AttackEnd()
    {
        base.On_AttackEnd();
    }

    public override void On_GolemAreGoingToDie()
    {
        // StopAllCoroutines();
        base.On_GolemAreGoingToDie();
        m_right.m_hitSign.StopAllGroundHitCoroutine();
        m_left.m_hitSign.StopAllGroundHitCoroutine();
    }
#endregion

}