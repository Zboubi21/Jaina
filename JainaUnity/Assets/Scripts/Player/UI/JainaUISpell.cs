using UnityEngine;

public class JainaUISpell : MonoBehaviour {
    
    [SerializeField] GameObject m_plusBtn;
    [SerializeField] GameObject m_minusBtn;

    void OnEnable(){

    }

    void OnDisable(){
        m_plusBtn.SetActive(true);
        m_minusBtn.SetActive(false);
    }

}
