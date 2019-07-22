using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class Spell : MonoBehaviour {

	public ElementType m_currentElement = ElementType.None;
	public enum ElementType{
		None,
		Arcane,
		Ice,
		Fire
	}

    public SpellType m_spellType;

    public BecameInvisible m_scripToBecameInvisible;

	MarksTime m_marksTime = new MarksTime();
    public MarksTime MarksTime1
    {
        get
        {
            return m_marksTime;
        }

        set
        {
            m_marksTime = value;
        }
    }

    ObjectPooler m_objectPooler;
    public ObjectPooler ObjectPoolerInstance{
        get{
            return m_objectPooler;
        }
        set{
            m_objectPooler = value;
        }
    }

    CharacterStats m_characterStats;
    public CharacterStats CharacterStats{
        get{
            return m_characterStats;
        }
        set{
            m_characterStats = value;
        }
    }

    [System.Serializable] public class MarksTime {
		float m_arcane = 6;
        public float Arcane
        {
            get
            {
                return m_arcane;
            }

            set
            {
                m_arcane = value;
            }
        }
		
		float m_ice= 10;
        public float Ice
        {
            get
            {
                return m_ice;
            }

            set
            {
                m_ice = value;
            }
        }

		float m_fire = 10;
        public float Fire
        {
            get
            {
                return m_fire;
            }

            set
            {
                m_fire = value;
            }
        }
    }

    public virtual void Start(){
        m_objectPooler = ObjectPooler.Instance;
    }

}
