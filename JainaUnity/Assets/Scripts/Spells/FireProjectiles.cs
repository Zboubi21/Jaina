using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectiles : Projectile {

	[Space]
	[SerializeField] float m_distance = 5;
	[Space]
	[SerializeField] AnimationCurve m_speedCurve;

	float m_duration;

	void Awake(){
		m_dieWhenHit = false;
		m_haveMaxLifeTime = false;

		m_duration = (m_distance + (m_distance / 2)) / m_speed;

		Invoke("DestroyProjectile", m_duration);

		FireFlameInstantiator ffi = GetComponentInParent<FireFlameInstantiator>();
		if(ffi != null){
			ffi.DestroyInstantiator(m_duration + 1);
		}
	}

	float timeFromStart;
	public override void FixedUpdate(){

		float f = Mathf.Lerp(m_duration / m_duration, 0, timeFromStart += Time.deltaTime / m_duration);
		//print("f = " + f);
		RBody.velocity = transform.forward * m_speed * m_speedCurve.Evaluate(f);
	}
    public override void OnFireEnter(Collider col)
    {
        col.gameObject.GetComponent<CharacterStats>().FireMark(MarksTime1.Fire);
    }

    void OnDrawGizmosSelected(){
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, m_distance);
	}

}
