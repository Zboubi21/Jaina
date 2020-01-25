using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamController : BossAttack
{
    [Header("Debug")]
    [Range(1, 3), SerializeField] int m_phaseNbr = 1;
    [SerializeField] Transform[] m_rayPos;
    [SerializeField] bool m_useDebug = false;
    [SerializeField] KeyCode m_debugInput = KeyCode.H;
    [Space]
    [SerializeField] ProgressControlV3D m_laserControlFx;
    [SerializeField] Transform m_golemLookAtPoint;
    [Space]
    [SerializeField] float m_waitTimeToRotateGolem = 2;
    [SerializeField] float m_timeToGolemLookAtPoint = 2;
    [SerializeField] AnimationCurve m_golemLookAtPointCurve;
    [SerializeField] float m_waitAdditionnalTimeToEndAttack = 1f;

    // [Space]
    // [SerializeField] Transform m_centerPos;
    // [Range(1, 50)] [SerializeField] int m_rayCastNbr = 10;
    // [SerializeField] float m_rightPos = 2f;
    // [SerializeField] float m_leftPos = -2f;
    // float m_distanceBeetweenEachRay;

#region [SerializeField] Variables
    public Rotate m_rotate;
    [Serializable] public class Rotate {
        public float m_leftWorldRotation = 300;
        public float m_rightWorldRotation = 50;
        public float m_timeToRotate = 10;
        public AnimationCurve m_rotateCurve;
        [Space]
        public float m_waitTimeBetweenRotations = 1;
    }

    public Laser m_laser;
    [Serializable] public class Laser {
        public float m_castDistance = 100;
        // public LayerMask m_firstLaserLayer;
        // public LayerMask m_secondLaserLayer;
        public LayerMask m_damageLayer;

        [Header("Damages")]
        public int m_damage = 5;
        public float m_tick = 0.2f;
    }

    public LaserGizmos m_gizmos;
    [Serializable] public class LaserGizmos {
        public bool m_show = true;
        public Color m_laserColor = Color.white;
        // public Color m_secondLaserColor = Color.magenta;
    }

#endregion

#region Private Variables

    bool m_lastRotateDirectionWasRight = false;
    int m_nbrOfRotationToDo = 1;
    int m_actualNbrOfRotation = 0;

    LaserBeamArea m_laserArea;
    CharacterStats m_characterStats;

    GameObject m_lastStalactite;
    public GameObject LastStalactite{
        get{
            return m_lastStalactite;
        }
    }

    bool m_playerInLaserArea = false;
    bool m_playerTouchByLaser = false;
    GameObject m_player;

    bool m_laserIsUsed = false;

    #endregion

#region Event Functions
    void Start()
    {
        m_laserArea = GetComponentInChildren<LaserBeamArea>();
    }

    void Update()
    {
        if(m_useDebug && Input.GetKeyDown(m_debugInput))
        {
            On_AttackBegin(m_phaseNbr);
        }
    }

    void FixedUpdate()
    {
        // CheckLaserCollision();
        if(m_laserIsUsed)
        {
            CheckIfPlayerIsInLaser();
            RotateGolemToLookAtPoint(m_golemLookAtPoint);
        }
    }

    // public float xValue;
    void OnDrawGizmos()
    {
        if(!m_gizmos.m_show)
        {
            return;
        }
        Gizmos.color = m_gizmos.m_laserColor;
        for (int i = 0, l = m_rayPos.Length; i < l; ++i)
        {
            Gizmos.DrawRay(m_rayPos[i].position, m_rayPos[i].forward * m_laser.m_castDistance);
        }

        // float dist = Mathf.Abs(m_rightPos) + Mathf.Abs(m_leftPos);
        // // Debug.Log("dist = " + dist);
        // m_distanceBeetweenEachRay = dist / m_rayCastNbr;
        // // Debug.Log("m_distanceBeetweenEachRay = " + m_distanceBeetweenEachRay);

        // // float xValue = 0;
        // for (int i = 0; i < m_rayCastNbr; ++i)
        // {
        //     // xValue += m_distanceBeetweenEachRay;
        //     // Debug.Log("xValue = " + xValue);

        //     Vector3 rayPos = new Vector3(m_centerPos.position.x + xValue, m_centerPos.position.y, m_centerPos.position.z);
        //     // Gizmos.DrawRay(rayPos, m_centerPos.forward + rayPos * m_laser.m_castDistance);

        //     m_centerPos.localPosition = new Vector3(m_centerPos.localPosition.x + xValue, m_centerPos.localPosition.y, m_centerPos.localPosition.z);

        //     Vector3 forwardMultiply = m_centerPos.forward * m_laser.m_castDistance;
        //     Gizmos.DrawRay(m_centerPos.position + new Vector3(xValue, 0, 0), forwardMultiply);
        //     // Gizmos.DrawRay(m_centerPos.position + new Vector3(xValue, 0, 0), forwardMultiply + new Vector3(xValue, 0, 0))
        //     ;
        //     // Debug.Log("sans multiply = " + (m_centerPos.forward + new Vector3(0, xValue, 0)));
        //     // Debug.Log("avec multiply = " + (m_centerPos.forward + new Vector3(0, xValue, 0)) * m_laser.m_castDistance);
        // }
    }
#endregion

#region Private Functions
    void StartLaserBeam(int phaseNbr)
    {
        m_actualNbrOfRotation = 0;
        UseLaser(true);
        RotateLaser(phaseNbr);
    }
    IEnumerator WaitTimeToStartLaser(int phaseNbr)
    {
        yield return new WaitForSeconds(m_waitTimeToRotateGolem);
        StartLaserBeam(phaseNbr);
    }

    void RotateLaser(int phaseNbr)
    {        
        m_nbrOfRotationToDo = phaseNbr;
        if(m_lastRotateDirectionWasRight)
        {
            StartCoroutine(RotateLaserCorout(m_rotate.m_rightWorldRotation, m_rotate.m_leftWorldRotation));
        }
        else
        {
            StartCoroutine(RotateLaserCorout(m_rotate.m_leftWorldRotation, m_rotate.m_rightWorldRotation));
        }
        m_lastRotateDirectionWasRight =! m_lastRotateDirectionWasRight;
    }

    IEnumerator RotateLaserCorout(float fromRot, float toRot)
    {
        float fracJourney = 0;
        float distance = Mathf.Abs(fromRot - toRot);
        float vitesse = distance / m_rotate.m_timeToRotate;
        float actualValue = fromRot;

        while (actualValue != toRot)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualValue = Mathf.Lerp(fromRot, toRot, m_rotate.m_rotateCurve.Evaluate(fracJourney));
            transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, actualValue, transform.rotation.eulerAngles.z);
            yield return null;
        }

        m_laserArea.ResetStalactiteList();
        m_actualNbrOfRotation ++;
        if(m_actualNbrOfRotation == m_nbrOfRotationToDo)
        {
            StartCoroutine(WaitAdditionnalTimeToEndAttack());
        }else{
            StartCoroutine(WaitTimeToRotateAgain());
        }
    }
    IEnumerator WaitTimeToRotateAgain()
    {
        yield return new WaitForSeconds(m_rotate.m_waitTimeBetweenRotations);
        RotateLaser(m_phaseNbr);
    }

    // void CheckLaserCollision()
    // {
    //     RaycastHit hit;
	// 	Vector3 from = transform.position;
    //     Vector3 to = transform.forward;

    //     if(!Physics.Raycast(transform.position, transform.forward, out hit, m_laser.m_castDistance, m_laser.m_secondLaserLayer))
    //     {
    //         if(Physics.Raycast(transform.position, transform.forward, out hit, m_laser.m_castDistance, m_laser.m_firstLaserLayer))
    //         {
    //             // Debug.DrawLine(transform.position, hit.point, m_gizmos.m_firstLaserColor);
    //             // Debug.DrawLine(hit.point, transform.position + transform.forward * m_laser.m_castDistance, m_gizmos.m_secondLaserColor);
    //         }else{
    //             // Debug.DrawLine(transform.position,transform.position + transform.forward * m_laser.m_castDistance, m_gizmos.m_firstLaserColor);
    //         }
    //     }
    //     else
    //     {
    //         // Debug.DrawLine(transform.position, hit.point, m_gizmos.m_secondLaserColor);
    //     }
    // }

    bool TargetIsInLaser(GameObject target)
    {
        bool isInLaser = false;
        for (int i = 0, l = m_rayPos.Length; i < l; ++i)
        {
            RaycastHit hit;
            if(Physics.Raycast(m_rayPos[i].position, m_rayPos[i].forward, out hit, m_laser.m_castDistance, m_laser.m_damageLayer))
            {
                if(hit.collider.gameObject == target)
                {
                   isInLaser = true; 
                }
            }
        }
        return isInLaser;
    }

    void AddStalactiteState(GameObject stalac)
    {
        m_lastStalactite = stalac;
        StalactiteController stalactite = stalac.GetComponent<StalactiteController>();
        if(stalactite != null)
        {
            stalactite.AddStalactiteState();
        }
    }

    void CheckIfPlayerIsInLaser()
    {
        if(m_playerInLaserArea)
        {
            if(TargetIsInLaser(m_player))
            {
                if(!m_playerTouchByLaser)
                {
                    m_playerTouchByLaser = true;
                    PlayerIsInLaserArea();
                }
            }
            else
            {
                if(m_playerTouchByLaser)
                {
                    m_playerTouchByLaser = false;
                    PlayerIsNotInLaserArea();
                }
            }
        }
    }

    void PlayerIsInLaserArea()
    {
        if(m_characterStats == null)
        {
            m_characterStats = m_player.GetComponent<CharacterStats>();
        }

        if(m_characterStats.LaserTick != m_laser.m_tick)
        {
            m_characterStats.LaserTick = m_laser.m_tick;
        }
        if(m_characterStats.LaserTickDamage != m_laser.m_damage)
        {
            m_characterStats.LaserTickDamage = m_laser.m_damage;
        }
        m_characterStats.OnCharacterEnterInLaserArea();
    }
    void PlayerIsNotInLaserArea()
    {
        m_characterStats.OnCharacterExitInLaserArea();
    }

    void UseLaser(bool use)
    {
        if(use)
        {
            m_laserControlFx.StartLaserFx();
            m_laserIsUsed = true;
        }
        else
        {
            m_laserControlFx.StopLaserFx();
            m_laserIsUsed = false;
        }
    }

#endregion

#region Public Functions

    public void On_StalactiteEnterInLaserTrigger(GameObject obj)
    {
        if (!m_laserIsUsed)
            return;
            
        if(TargetIsInLaser(obj))
        {
            // Debug.Log("! InLaser !");
            AddStalactiteState(obj);
        }
        else
        {
            // Debug.Log("NOT_InLaser");
        }
    }
    public void On_StalactiteExitFromLaserTrigger(GameObject obj)
    {
        if(obj == LastStalactite)
        {
            if(TargetIsInLaser(obj))
            {
                // Debug.Log("! InLaser !");
                AddStalactiteState(obj);
            }
            else
            {
                // Debug.Log("NOT_InLaser");
            }
        }
    }

    public void On_PlayerEnterInLaserTrigger(GameObject player)
    {
        m_player = player;
        m_playerInLaserArea = true;
    }
    public void On_PlayerExitFromLaserTrigger()
    {
        m_playerInLaserArea = false;
    }

    public override void On_AttackBegin(int phaseNbr)
    {
        base.On_AttackBegin(phaseNbr);
        m_phaseNbr = phaseNbr;
        StartCoroutine(WaitTimeToStartLaser(phaseNbr));

        if(m_lastRotateDirectionWasRight)
        {
            StartCoroutine(RotateGolemToLookAtPointWithTime(m_rotate.m_rightWorldRotation, m_timeToGolemLookAtPoint, m_golemLookAtPointCurve));
        }
        else
        {
            StartCoroutine(RotateGolemToLookAtPointWithTime(m_rotate.m_leftWorldRotation, m_timeToGolemLookAtPoint, m_golemLookAtPointCurve));
        }
    }

    IEnumerator WaitAdditionnalTimeToEndAttack()
    {
        UseLaser(false);
        yield return new WaitForSeconds(m_waitAdditionnalTimeToEndAttack);
        On_AttackEnd();
    }
    public override void On_AttackEnd()
    {
        base.On_AttackEnd();
        StartCoroutine(RotateGolemToLookAtPointWithTime(GolemController.YStartRotation, m_timeToGolemLookAtPoint, m_golemLookAtPointCurve));
    }

    public override void On_GolemAreGoingToDie()
    {
        StopAllCoroutines();
        base.On_GolemAreGoingToDie();
        UseLaser(false);
    }

#endregion

}
