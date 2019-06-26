using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour {

	public static CameraManager Instance;

    public float m_smoothing = 5f;
    public float m_clampedValue = 1f;
	[Range(0, 1)] public float m_posBetweenTargetAndMouse = 0.5f;
	public LayerMask m_groundLayer;

	[HideInInspector] public Vector3 m_cursorPosition;

	Transform m_target;
    Vector3 m_offset;
	Vector3 mousePoint;

	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of CameraManager");
		}
	}

    void Start (){
		m_target = PlayerManager.Instance.transform;
        m_offset = transform.position - m_target.position;
    }

	void Update(){
		Ray();
	}

    void LateUpdate (){
		
        Vector3 targetCamPos = m_target.position + m_offset;

		Vector3 m_targetCamPos = targetCamPos - m_offset;

		Vector3 middlePos = Vector3.Lerp(targetCamPos, mousePoint, m_posBetweenTargetAndMouse);

        transform.position = Vector3.Lerp(transform.position, middlePos, m_smoothing * Time.deltaTime);
		transform.position = new Vector3(transform.position.x, m_offset.y, transform.position.z);
    }

	void OnDrawGizmos(){
		// Gizmos.color = Color.black;
		// Gizmos.DrawSphere(m_cursorPosition, .5f);
	}

	void Ray(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit floorHit;

        if(Physics.Raycast (ray, out floorHit, Mathf.Infinity, m_groundLayer)){
			Vector3 hitPoint = floorHit.point;
			m_cursorPosition = hitPoint;
			hitPoint = new Vector3(Mathf.Clamp(hitPoint.x, m_target.position.x - m_clampedValue, m_target.position.x + m_clampedValue), m_offset.y, Mathf.Clamp(hitPoint.z, m_target.position.z - m_clampedValue, m_target.position.z + m_clampedValue));
			//Debug.Log("hitPoint = " + hitPoint);
            mousePoint = hitPoint + m_offset;
			//Debug.Log("mousePoint = " + mousePoint);
		}
	}

}
