using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour {

#region Singleton
	public static CameraManager Instance;
    void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of CameraManager");
		}
	}
#endregion Singleton
	
	[SerializeField] bool m_startWithCloseOffset = true;
	[SerializeField] Vector3 m_closeOffset = new Vector3(0, 20, -20);
	[SerializeField] Vector3 m_farOffset = new Vector3(0, 30, -20);
    [SerializeField] float m_smoothing = 5f;
    [SerializeField] float m_clampedValue = 1f;
	[SerializeField, Range(0, 1)] float m_posBetweenTargetAndMouse = 0.5f;
	[SerializeField] LayerMask m_groundLayer;
	[Space]
	[SerializeField] float m_distanceToMoveWithGamepad = 3;

	[Header("Switch Camera")]
	[SerializeField] float m_timeToSwitchCamera = 3;
	[SerializeField] AnimationCurve m_switchCameraCurve;

	[Header("End Boss Fight")]
	[SerializeField] Vector3 m_camPos;
	[SerializeField] float m_timeToChangePos = 3;
	[SerializeField] AnimationCurve m_changePosCurve;

	[HideInInspector] public Vector3 m_cursorPosition;
	
	bool m_canMoveCamera = true;
	public bool CanMoveCamera {
        get{
            return m_canMoveCamera;
        }
        set{
            m_canMoveCamera = value;
        }
    }

	bool m_inEndBossCinematic = false;

	PlayerManager m_playerManager;
	Transform m_target;
	Vector3 m_mousePoint;
	Vector3 m_actualOffset;
	bool m_isCloseOffset = true;

    void Start (){
		m_playerManager = PlayerManager.Instance;
		m_target = m_playerManager.transform;
        // m_offset = transform.position - m_target.position;
		// transform.position = m_target.position + m_offset;
		m_isCloseOffset = m_startWithCloseOffset;
		m_actualOffset = m_isCloseOffset ? m_closeOffset : m_farOffset;
		m_mousePoint = m_target.position + m_actualOffset;
    }

	void Update(){
		if(m_canMoveCamera){
			Ray();
			MoveCameraWithGamepad();
		}else{
			m_mousePoint = m_target.position + m_actualOffset;
		}
	}

    void LateUpdate (){
		if(m_inEndBossCinematic)
		{
			return;
		}
		Vector3 targetCamPos = m_target.position + m_actualOffset;
		Vector3 middlePos = Vector3.Lerp(targetCamPos, m_mousePoint, m_posBetweenTargetAndMouse);
		transform.position = Vector3.Lerp(transform.position, middlePos, m_smoothing * Time.deltaTime);
		transform.position = new Vector3(transform.position.x, m_actualOffset.y, transform.position.z);
    }

	void Ray(){
		if(m_playerManager.m_playerDebug.m_useGamepad){
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit floorHit;

        if(Physics.Raycast (ray, out floorHit, Mathf.Infinity, m_groundLayer)){
			Vector3 hitPoint = floorHit.point;
			m_cursorPosition = hitPoint;
			hitPoint = new Vector3(Mathf.Clamp(hitPoint.x, m_target.position.x - m_clampedValue, m_target.position.x + m_clampedValue), m_actualOffset.y, Mathf.Clamp(hitPoint.z, m_target.position.z - m_clampedValue, m_target.position.z + m_clampedValue));
            m_mousePoint = hitPoint + m_actualOffset;
		}
	}

	void MoveCameraWithGamepad(){
		if(!m_playerManager.m_playerDebug.m_useGamepad){
			return;
		}
		Vector3 targetPos = m_playerManager.transform.position + m_playerManager.RotationInput * m_distanceToMoveWithGamepad;
		targetPos.y = m_actualOffset.y;
		m_mousePoint = targetPos + m_actualOffset;
	}

	public void ResetPosition(){
		transform.position = m_target.position + m_actualOffset;
		m_mousePoint = m_target.position + m_actualOffset;
	}

	public IEnumerator SwitchCamOffset()
	{
		Vector3 fromOffset = m_actualOffset;
		Vector3 toOffset = m_isCloseOffset ? m_farOffset : m_closeOffset;

		m_isCloseOffset =! m_isCloseOffset;

        float fracJourney = 0;
        float distance = Vector3.Distance(fromOffset, toOffset);
        float vitesse = distance / m_timeToSwitchCamera;

        while (m_actualOffset != toOffset)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            m_actualOffset = Vector3.Lerp(fromOffset, toOffset, m_switchCameraCurve.Evaluate(fracJourney));
            yield return null;
        }
	}

	public IEnumerator LookEndBossFightPos()
	{
		m_inEndBossCinematic = true;
		Vector3 fromPos = transform.position;
		Vector3 toPos = m_camPos;

        float fracJourney = 0;
        float distance = Vector3.Distance(fromPos, toPos);
        float vitesse = distance / m_timeToChangePos;

        while (transform.position != toPos)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            transform.position = Vector3.Lerp(fromPos, toPos, m_changePosCurve.Evaluate(fracJourney));
            yield return null;
        }
	}

	void OnDrawGizmos(){
		// Gizmos.color = Color.black;
		// Gizmos.DrawSphere(m_cursorPosition, .5f);
	}

}
