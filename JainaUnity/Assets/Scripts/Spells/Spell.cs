﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {

	public ElementType m_currentElement = ElementType.None;
	public enum ElementType{
		None,
		Arcane,
		Ice,
		Fire
	}

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

    

    [System.Serializable] public class MarksTime {
		float m_arcane = 10;
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



}
