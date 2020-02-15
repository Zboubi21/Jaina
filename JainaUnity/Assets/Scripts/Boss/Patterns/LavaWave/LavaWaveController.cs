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

    [Header("Lava Wave Area")]
    [SerializeField] LavaWaveArea m_lavaWaveArea;
    [System.Serializable] class LavaWaveArea
    {
        public GroundHitSign m_waveAreaMask;
        public float m_waitTimeToStartShowLavaWaveArea = 10;
        public float m_rightXPos = -9;
        public float m_leftXPos = 11.5f;

        [Header("Anim")]
        public float m_timeToShowLavaAreaInP2 = 15;
        public float m_timeToShowLavaAreaInP3 = 15;
        [Space]
        public float m_yLavaPos;
        public float m_timeToMoveYLavaPos = 1;
    }

    [Header("Debug")]
    [SerializeField] bool m_useDebugInput = false;
#endregion

#region Private Variables
    PlayerManager m_playerManager;

    float m_moveZPosSpeed;
    public float MoveZPosSpeed
    { get { return m_moveZPosSpeed; } }

    float m_startYLavaWaveAreaPos;

    float m_startZLavaWaveScale;

    IEnumerator m_showingLavaWaveAreaCorout;
    #endregion

    #region Event Functions
    void Start()
    {
        m_playerManager = PlayerManager.Instance;

        float distance = Mathf.Abs(m_moveZPos.m_fromValue - m_moveZPos.m_toValue);
        m_moveZPosSpeed = distance / m_moveZPos.m_timeToMove;

        m_startYLavaWaveAreaPos = m_lavaWaveArea.m_waveAreaMask.transform.localPosition.y;

        m_startZLavaWaveScale = m_lavaWave.localScale.z;
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

    IEnumerator WaitTimeToStartShowLavaWaveArea(float xPos)
    {
        yield return new WaitForSeconds(m_lavaWaveArea.m_waitTimeToStartShowLavaWaveArea);
        m_lavaWaveArea.m_waveAreaMask.transform.localPosition = new Vector3(m_lavaWaveArea.m_waveAreaMask.transform.localPosition.x, m_startYLavaWaveAreaPos, m_lavaWaveArea.m_waveAreaMask.transform.localPosition.z);
        
        if (m_showingLavaWaveAreaCorout != null)
            StopCoroutine(m_showingLavaWaveAreaCorout);
        m_lavaWaveArea.m_waveAreaMask.StartToMoveWithSpeed(xPos, m_moveZPosSpeed, this);
    }
    IEnumerator ShowingLavaWaveArea()
    {
        m_right.m_hitSign.StopShowSign();
        m_left.m_hitSign.StopShowSign();

        float timeToWait = 0;
        if (GolemController.PhaseNbr == 2)
            timeToWait = m_lavaWaveArea.m_timeToShowLavaAreaInP2;

        if (GolemController.PhaseNbr == 3)
            timeToWait = m_lavaWaveArea.m_timeToShowLavaAreaInP3;

        yield return new WaitForSeconds(timeToWait);
        StartCoroutine(MovaLavaYPos());
    }
    IEnumerator MovaLavaYPos()
    {
        Vector3 fromPos = m_lavaWaveArea.m_waveAreaMask.transform.localPosition;
        Vector3 toPos = new Vector3(m_lavaWaveArea.m_waveAreaMask.transform.localPosition.x, m_lavaWaveArea.m_yLavaPos, m_lavaWaveArea.m_waveAreaMask.transform.localPosition.z);

        float fracJourney = 0;
        float distance = Vector3.Distance(fromPos, toPos);
        float speed = distance / m_lavaWaveArea.m_timeToMoveYLavaPos;

        while (m_lavaWaveArea.m_waveAreaMask.transform.localPosition != toPos)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            m_lavaWaveArea.m_waveAreaMask.transform.localPosition = Vector3.Lerp(fromPos, toPos, fracJourney);
            yield return null;
        }
    }

    float LavaXPos()
    {
        if (GolemController.PlayerPosIsClosestToRightPos())
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

        // Set x LavaWave pos
        m_lavaWave.localPosition = new Vector3(LavaXPos(), m_lavaWave.localPosition.y, m_lavaWave.localPosition.z);

        // Set LavaWave scale
        float zScale = GolemController.PlayerPosIsClosestToRightPos() ? -m_startZLavaWaveScale : m_startZLavaWaveScale;
        m_lavaWave.localScale = new Vector3(m_lavaWave.localScale.x, m_lavaWave.localScale.y, zScale);

        float lavaWaveAreaXPos;
        if(GolemController.PlayerPosIsClosestToRightPos())
        {
            m_right.m_hitSign.StartToMove();
            m_right.m_hitSign.StartToChangeColor();
            lavaWaveAreaXPos = m_lavaWaveArea.m_rightXPos;
        }
        else
        {
            m_left.m_hitSign.StartToMove();
            m_left.m_hitSign.StartToChangeColor();
            lavaWaveAreaXPos = m_lavaWaveArea.m_leftXPos;
        }
        StartCoroutine(MoveYPosition());
        StartCoroutine(MoveZPosition());

        StartCoroutine(WaitTimeToStartShowLavaWaveArea(lavaWaveAreaXPos));
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

    public void On_LavaWaveAreaStopped()
    {
        m_showingLavaWaveAreaCorout = ShowingLavaWaveArea();
        StartCoroutine(m_showingLavaWaveAreaCorout);
    }
#endregion

}