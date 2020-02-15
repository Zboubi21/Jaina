﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StalactiteStateEnum;

public class Projectile : Spell {

#region Projectile Type
	public ProjectileType m_projectileType = ProjectileType.Player;
	public enum ProjectileType{
		Player,
		Enemy
	}
#endregion Projectile Type

	[SerializeField] int m_damage = 10;
	public float m_speed = 25;
	[Space]
	[Header("FX")]
	[SerializeField] GameObject m_dieFX;
    [SerializeField] bool hasToRespectColliderNormal = true;
    [SerializeField] float timeToGoBackToPool = 3;
    [SerializeField] GameObject parentToReturnToPool;
    [Tooltip("Only if hasToRespectColliderNormal == false")]
    [SerializeField] Quaternion rotation;
    [Space]
	[HideInInspector] public bool m_haveMaxLifeTime = true;
	[HideInInspector] public bool m_dieWhenHit = true;
	
	Rigidbody m_rBody;

#region Get Set
    public int Damage
    {
        get
        {
            return m_damage;
        }

        set
        {
            m_damage = value;
        }
    }

    public Rigidbody RBody
    {
        get
        {
            return m_rBody;
        }

        set
        {
            m_rBody = value;
        }
    }
#endregion

    void OnEnable()
    {
        StartCoroutine(GoBackToPoolAnyWay());
    }

    public override void Start(){
		base.Start();
		RBody = GetComponent<Rigidbody>();
	}

	public virtual void FixedUpdate(){
		RBody.velocity = transform.forward * m_speed;

		// if(m_scripToBecameInvisible.TimeToDestroy){
		// 	ProjectileReturnToPool();
		// }
	}
    IEnumerator GoBackToPoolAnyWay()
    {
        yield return new WaitForSeconds(timeToGoBackToPool);
        ProjectileReturnToPool();
    }

	void OnTriggerEnter(Collider col){

        // Debug.Log(col);

		// Le tir d'un enemy touche le player
		if(col.CompareTag("Player")){
			if(m_projectileType == ProjectileType.Enemy){
                col.gameObject.GetComponent<CharacterStats>().TakeDamage(Damage);

                if(m_dieWhenHit){
					DestroyProjectile();
				}
			}
		}

		// Le tir du player touche un enemy
		if(col.CompareTag("Enemy")){

			CharacterStats = col.gameObject.GetComponent<CharacterStats>();

			if(m_projectileType == ProjectileType.Player){
				switch(m_currentElement){
					case ElementType.None:

					break;
					
					case ElementType.Arcane:
                        OnArcanEnter(col);
                        break;
					case ElementType.Ice:
                        CharacterStats.IceMark(MarksTime1.Ice);
                        CharacterStats.TakeDamage(Damage);
                        break;
					case ElementType.Fire:
                        OnFireEnter(col);
                        CharacterStats.TakeDamage(Damage);
                        break;
				}
            	CharacterStats.StartHitFxCorout();
                if(col.gameObject.GetComponent<GolemController>() == null && col.gameObject.GetComponent<EnemyController>() != null)
                {
                    col.gameObject.GetComponent<EnemyController>().CheckIfStunable();
                }
                if (m_dieWhenHit){
					DestroyProjectile();
				}
			}
		}

		// Le projectile touche un élément du décor
		if(col.CompareTag("Untagged")){
			if(m_dieWhenHit){
				DestroyProjectile();
			}
		}

		// Le projectile touche un élément du décor
		if(col.CompareTag("Stalactite")){
            StalactiteStats CharacterStats = col.gameObject.GetComponent<StalactiteStats>();
            StalactiteController controller = col.gameObject.GetComponent<StalactiteController>();
            if (m_projectileType == ProjectileType.Player)
            {
                switch (m_currentElement)
                {
                    case ElementType.None:

                        break;

                    case ElementType.Arcane:
                        OnArcanEnter(col);
                        break;
                    case ElementType.Ice:
                        CharacterStats.IceMark(MarksTime1.Ice);
                        CharacterStats.TakeDamage(Damage);
                        break;
                    case ElementType.Fire:
                        OnFireEnter(col);
                        if (controller.StalactiteState == StalactiteState.Fusion)
                        {
                            CharacterStats.TakeDamage(Damage * CharacterStats.fireDamageMutliplicater);
                        }
                        else
                        {
                            CharacterStats.TakeDamage(Damage);
                        }
                        break;
                }
            	CharacterStats.StartHitFxCorout();
                if (m_dieWhenHit)
                {
                    DestroyProjectile();
                }
            }
            else if(m_projectileType == ProjectileType.Enemy)
            {
                switch (m_currentElement)
                {
                    case ElementType.None:
                        controller.AddStalactiteState();
                        break;
                    case ElementType.Arcane:
                        break;
                    case ElementType.Ice:
                        break;
                    case ElementType.Fire:
                        break;
                }
                if (m_dieWhenHit)
                {
                    DestroyProjectile();
                }
            }
        }
	}

	public virtual void OnArcanEnter(Collider col){

        col.gameObject.GetComponent<CharacterStats>().AutoAttackArcanMark(Damage, MarksTime1.Arcane, 1);
    }

    public virtual void OnFireEnter(Collider col)
    {
        col.gameObject.GetComponent<CharacterStats>().AutoAttackFireMark(MarksTime1.Fire);
    }

	void DestroyProjectile(){
		if(m_dieFX != null){
            if (hasToRespectColliderNormal)
            {
			    Level.AddFX(m_dieFX, transform.position, transform.rotation);
            }
            else
            {
                Level.AddFX(m_dieFX, transform.position, rotation);
            }
        }
		
		PoolTracker poolTracker = GetComponent<PoolTracker>();
        if(poolTracker != null){
            Destroy(poolTracker);
        }

		ProjectileReturnToPool();
	}

	public void ProjectileReturnToPool()
    {
        if (hasToRespectColliderNormal)
        {
		    ObjectPoolerInstance.ReturnSpellToPool(m_spellType, gameObject);
        }
        else if(parentToReturnToPool != null)
        {

            ObjectPoolerInstance.ReturnSpellToPool(m_spellType, parentToReturnToPool);
            gameObject.transform.position = parentToReturnToPool.transform.position;
        }
    }

	public void SetTargetPos(Vector3 targetPos){
		Vector3 projectileToMouse = targetPos - transform.position;
		projectileToMouse.y = 0f;
		Quaternion newRotation = Quaternion.LookRotation(projectileToMouse);
		transform.rotation = newRotation;
	}

	public void SetTargetPosWithGamepad(Vector3 targetPos){
		Vector3 projectileToMouse = targetPos - transform.position;
		projectileToMouse.y = 0f;
		Quaternion newRotation = Quaternion.LookRotation(projectileToMouse);
		newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z);
		transform.rotation = newRotation;
	}

}
