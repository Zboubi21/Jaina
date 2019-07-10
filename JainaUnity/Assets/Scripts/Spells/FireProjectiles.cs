using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectiles : Projectile {

	[Space]
	[SerializeField] float m_distance = 5;
	[Space]
	[SerializeField] AnimationCurve m_speedCurve;

	[Header("Raycasts")]
	public RaycastCollision m_raycastCollision = new RaycastCollision();
	[System.Serializable] public class RaycastCollision {
		[Header("Forward")]
		public float m_maxForwardDistance = 0.25f;
		public LayerMask m_forwardCollision;
		public Transform m_forwardRightRaycast;
		public Transform m_forwardMiddleRaycast;
		public Transform m_forwardLeftRaycast;

		[Header("Down")]
		public float m_maxDownDistance = 0.25f;
		public LayerMask m_botCollision;
		public Transform m_botRightRaycast;
		public Transform m_botMiddleRaycast;
		public Transform m_botLeftRaycast;
	}

	float m_duration;
	float m_timeFromStart = 0;
	float m_anotherTimer = 0;
	// bool m_canMove = true;

	FireFlameInstantiator ffi;
	public FireFlameInstantiator Ffi{
        get{
            return ffi;
        }
        set{
            ffi = value;
        }
    }

	void OnEnable(){
		m_dieWhenHit = false;
		m_haveMaxLifeTime = false;

		m_duration = (m_distance + (m_distance / 2)) / m_speed;
		m_timeFromStart = 0;
		m_anotherTimer = 0;

		// Invoke("DestroyProjectile", m_duration);

		FireFlameInstantiator ffi = GetComponentInParent<FireFlameInstantiator>();
		if(ffi != null){
			ffi.DestroyInstantiator(m_duration + 1);
		}
	}


    public override void FixedUpdate(){
		// if(m_canMove){
			float f = Mathf.Lerp(m_duration / m_duration, 0, m_timeFromStart += Time.deltaTime / m_duration);
			RBody.velocity = transform.forward * m_speed * m_speedCurve.Evaluate(f);

			// if(m_timeFromStart >= m_duration){
			// 	ProjectileReturnToPool();
			// }
		// }else{
		// 	RBody.velocity = Vector3.zero;
		// }

		// CheckRaycasts();
	}
	void Update(){
		m_anotherTimer += Time.deltaTime;
		if(m_anotherTimer >= m_duration){
			ProjectileReturnToPool();
		}
	}
    public override void OnFireEnter(Collider col)
    {
        col.gameObject.GetComponent<CharacterStats>().FireMark(MarksTime1.Fire);
    }

	// void CheckRaycasts(){
	// 	if(!Physics.Raycast(m_raycastCollision.m_botRightRaycast.transform.position, - m_raycastCollision.m_botRightRaycast.transform.up, m_raycastCollision.m_maxDownDistance, m_raycastCollision.m_botCollision)
	// 	|| !Physics.Raycast(m_raycastCollision.m_botMiddleRaycast.transform.position, - m_raycastCollision.m_botMiddleRaycast.transform.up, m_raycastCollision.m_maxDownDistance, m_raycastCollision.m_botCollision)
	// 	|| !Physics.Raycast(m_raycastCollision.m_botLeftRaycast.transform.position, - m_raycastCollision.m_botLeftRaycast.transform.up, m_raycastCollision.m_maxDownDistance, m_raycastCollision.m_botCollision)){
	// 		m_canMove = false;
	// 	}

	// 	if(Physics.Raycast(m_raycastCollision.m_forwardRightRaycast.transform.position, m_raycastCollision.m_forwardRightRaycast.transform.forward, m_raycastCollision.m_maxForwardDistance, m_raycastCollision.m_forwardCollision)
	// 	|| Physics.Raycast(m_raycastCollision.m_forwardMiddleRaycast.transform.position, m_raycastCollision.m_forwardMiddleRaycast.transform.forward, m_raycastCollision.m_maxForwardDistance, m_raycastCollision.m_forwardCollision)
	// 	|| Physics.Raycast(m_raycastCollision.m_forwardLeftRaycast.transform.position, m_raycastCollision.m_forwardLeftRaycast.transform.forward, m_raycastCollision.m_maxForwardDistance, m_raycastCollision.m_forwardCollision)){
	// 		m_canMove = false;
	// 	}
	// }

    void OnDrawGizmosSelected(){
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, m_distance);

		// Gizmos.DrawLine(m_raycastCollision.m_forwardRightRaycast.transform.position, m_raycastCollision.m_forwardRightRaycast.transform.position + m_raycastCollision.m_forwardRightRaycast.transform.forward * m_raycastCollision.m_maxForwardDistance);
		// Gizmos.DrawLine(m_raycastCollision.m_forwardMiddleRaycast.transform.position, m_raycastCollision.m_forwardMiddleRaycast.transform.position + m_raycastCollision.m_forwardMiddleRaycast.transform.forward * m_raycastCollision.m_maxForwardDistance);
		// Gizmos.DrawLine(m_raycastCollision.m_forwardLeftRaycast.transform.position, m_raycastCollision.m_forwardLeftRaycast.transform.position + m_raycastCollision.m_forwardLeftRaycast.transform.forward * m_raycastCollision.m_maxForwardDistance);

		// Gizmos.DrawLine(m_raycastCollision.m_botRightRaycast.transform.position, m_raycastCollision.m_botRightRaycast.transform.position - m_raycastCollision.m_botRightRaycast.transform.up * m_raycastCollision.m_maxDownDistance);
		// Gizmos.DrawLine(m_raycastCollision.m_botMiddleRaycast.transform.position, m_raycastCollision.m_botMiddleRaycast.transform.position - m_raycastCollision.m_botMiddleRaycast.transform.up * m_raycastCollision.m_maxDownDistance);
		// Gizmos.DrawLine(m_raycastCollision.m_botLeftRaycast.transform.position, m_raycastCollision.m_botLeftRaycast.transform.position - m_raycastCollision.m_botLeftRaycast.transform.up * m_raycastCollision.m_maxDownDistance);
	}

}
