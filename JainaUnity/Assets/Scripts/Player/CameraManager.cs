﻿using System.Collections;
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
	[Space]
	[SerializeField] float m_distanceToMoveWithGamepad = 3;

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

	PlayerManager m_playerManager;
	Transform m_target;
	Vector3 m_mousePoint;

    void Start (){
		m_playerManager = PlayerManager.Instance;
		m_target = m_playerManager.transform;
        // m_offset = transform.position - m_target.position;
		// transform.position = m_target.position + m_offset;
		m_mousePoint = m_target.position + m_offset;
    }

	void Update(){
		if(m_canMoveCamera){
			Ray();
			MoveCameraWithGamepad();
		}else{
			m_mousePoint = m_target.position + m_offset;
		}
	}

    void LateUpdate (){
		Vector3 targetCamPos = m_target.position + m_offset;
		Vector3 middlePos = Vector3.Lerp(targetCamPos, m_mousePoint, m_posBetweenTargetAndMouse);
		transform.position = Vector3.Lerp(transform.position, middlePos, m_smoothing * Time.deltaTime);
		transform.position = new Vector3(transform.position.x, m_offset.y, transform.position.z);
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
			hitPoint = new Vector3(Mathf.Clamp(hitPoint.x, m_target.position.x - m_clampedValue, m_target.position.x + m_clampedValue), m_offset.y, Mathf.Clamp(hitPoint.z, m_target.position.z - m_clampedValue, m_target.position.z + m_clampedValue));
            m_mousePoint = hitPoint + m_offset;
		}
	}

	void MoveCameraWithGamepad(){
		if(!m_playerManager.m_playerDebug.m_useGamepad){
			return;
		}
		Vector3 targetPos = m_playerManager.transform.position + m_playerManager.RotationInput * m_distanceToMoveWithGamepad;
		targetPos.y = m_offset.y;
		m_mousePoint = targetPos + m_offset;
	}

	public void ResetPosition(){
		transform.position = m_target.position + m_offset;
		m_mousePoint = m_target.position + m_offset;
	}

	void OnDrawGizmos(){
		// Gizmos.color = Color.black;
		// Gizmos.DrawSphere(m_cursorPosition, .5f);
	}

}
