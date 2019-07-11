using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class PoolTracker : MonoBehaviour {
    
    ObjectPooler m_objectPooler;

    PoolType m_poolType;
    public PoolType PoolType{
        get{
            return m_poolType;
        }
        set{
            m_poolType = value;
        }
    }

    EnemyType m_enemyType;
    public EnemyType EnemyType{
        get{
            return m_enemyType;
        }
        set{
            m_enemyType = value;
        }
    }

    SpellType m_spellType;
    public SpellType SpellType{
        get{
            return m_spellType;
        }
        set{
            m_spellType = value;
        }
    }

    ObjectType m_objectType;
    public ObjectType ObjectType{
        get{
            return m_objectType;
        }
        set{
            m_objectType = value;
        }
    }

    void Start(){
        m_objectPooler = ObjectPooler.Instance;
    }

    public void ResetTrackedObject(){
        switch (m_poolType){
            case PoolType.EnemyType:
		        m_objectPooler.ReturnEnemyToPool(m_enemyType, gameObject);
                GetComponent<EnemyStats>().DestroyAllMarks();
            break;
            case PoolType.SpellType:
		        m_objectPooler.ReturnSpellToPool(m_spellType, gameObject);
            break;
            case PoolType.ObjectType:
		        m_objectPooler.ReturnObjectToPool(m_objectType, gameObject);
            break;
        }
        Destroy(this);
    }
    
}
