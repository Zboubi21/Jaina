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

	[SerializeField] Vector3 m_offset = new Vector3(0, 20, -20);
    [SerializeField] float m_smoothing = 5f;
    [SerializeField] float m_clampedValue = 1f;
	[SerializeField, Range(0, 1)] float m_posBetweenTargetAndMouse = 0.5f;
	[SerializeField] LayerMask m_groundLayer;

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

	Transform m_target;
    // Vector3 m_offset;
	Vector3 m_mousePoint;

    void Start (){
		m_target = PlayerManager.Instance.transform;
        // m_offset = transform.position - m_target.position;
		// transform.position = m_target.position + m_offset;
    }

	void Update(){
		if(m_canMoveCamera){
			Ray();
		}else{
			m_mousePoint = m_target.position + m_offset;
		}
	}

    void LateUpdate (){
		Vector3 targetCamPos = m_target.position + m_offset;
		// Vector3 m_targetCamPos = targetCamPos - m_offset;
		Vector3 middlePos = Vector3.Lerp(targetCamPos, m_mousePoint, m_posBetweenTargetAndMouse);
		transform.position = Vector3.Lerp(transform.position, middlePos, m_smoothing * Time.deltaTime);
		transform.position = new Vector3(transform.position.x, m_offset.y, transform.position.z);
    }

	void Ray(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit floorHit;

        if(Physics.Raycast (ray, out floorHit, Mathf.Infinity, m_groundLayer)){
			Vector3 hitPoint = floorHit.point;
			m_cursorPosition = hitPoint;
			hitPoint = new Vector3(Mathf.Clamp(hitPoint.x, m_target.position.x - m_clampedValue, m_target.position.x + m_clampedValue), m_offset.y, Mathf.Clamp(hitPoint.z, m_target.position.z - m_clampedValue, m_target.position.z + m_clampedValue));
			//Debug.Log("hitPoint = " + hitPoint);
            m_mousePoint = hitPoint + m_offset;
			//Debug.Log("mousePoint = " + mousePoint);
		}
	}

	public void ResetPosition(){
		transform.position = m_target.position + m_offset;
		m_mousePoint = Vector3.zero;
	}

	void OnDrawGizmos(){
		// Gizmos.color = Color.black;
		// Gizmos.DrawSphere(m_cursorPosition, .5f);
	}

}
