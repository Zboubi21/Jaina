using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	[Space]
	[HideInInspector] public bool m_haveMaxLifeTime = true;
	[HideInInspector] public bool m_dieWhenHit = true;
	
	public float m_maxLifeTime = 5;
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

    public override void Start(){
		base.Start();
		RBody = GetComponent<Rigidbody>();
	}

	public virtual void FixedUpdate(){
		RBody.velocity = transform.forward * m_speed;

		if(m_scripToBecameInvisible.TimeToDestroy){
			ProjectileReturnToPool();
		}
	}

	void OnTriggerEnter(Collider col){

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
                col.gameObject.GetComponent<EnemyController>().CheckIfStunable();
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
			if(m_dieWhenHit){
				DestroyProjectile();
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
			Level.AddFX(m_dieFX, transform.position, transform.rotation);
		}
		
		PoolTracker poolTracker = GetComponent<PoolTracker>();
        if(poolTracker != null){
            Destroy(poolTracker);
        }

		ProjectileReturnToPool();
	}

	public void ProjectileReturnToPool(){
		ObjectPoolerInstance.ReturnSpellToPool(m_spellType, gameObject);
	}

	public void SetTargetPos(Vector3 targetPos){
		// Debug.Log("SetTargetPos");
		Vector3 projectileToMouse = targetPos - transform.position;
		projectileToMouse.y = 0f;
		Quaternion newRotation = Quaternion.LookRotation(projectileToMouse);
		transform.rotation = newRotation;

		// transform.LookAt(targetPos, Vector3.up * transform.position.y);
	}

}
