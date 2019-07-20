using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour {

    [SerializeField] float m_minScale = 0.75f;
    [SerializeField] float m_maxScale = 1.25f;
    [Space]
    [SerializeField] float m_speed = 3;
    [Space]
    [SerializeField] ButtonAnimation m_friendButton;

    float m_time = 0;
    public float Time{
        get{
            return m_time;
        }
        set{
            m_time = value;
        }
    }

    RectTransform m_rectTrans;

    void Start(){
        m_rectTrans = GetComponent<RectTransform>();
    }

    void OnEnable(){
        if(m_friendButton != null){
            m_time = m_friendButton.Time;
        }else{
            m_time = 0;
        }
    }

    void Update(){
        m_time += UnityEngine.Time.deltaTime * m_speed;
        float actualScale = Mathf.Lerp (m_minScale, m_maxScale, Mathf.PingPong(m_time, 1));
        m_rectTrans.localScale = new Vector2(actualScale, actualScale);
    }  
    
}
