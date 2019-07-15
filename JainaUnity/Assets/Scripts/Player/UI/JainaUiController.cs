using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JainaUiController : MonoBehaviour {
    
    [SerializeField] JainaUI[] m_uiImage;
    [SerializeField] UI[] m_ui;

    void Awake(){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            m_uiImage[i].UiController = this;
        }
        for (int i = 0, l = m_ui.Length; i < l; ++i) {
            m_ui[i].UiController = this;
        }
    }

    public void On_UiPointerOver(JainaUI clickedUI){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            if(clickedUI != m_uiImage[i]){
                m_uiImage[i].CloseUI();
            }
        }
    }

    public void On_UiPointerExit(){
        StartCoroutine(WaitToExit());
        // bool isIn = false;
        // Debug.Log("bool isIn = " + isIn);
        // for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
        //     if(m_uiImage[i].MouseInUI){
        //         isIn = true;
        //         Debug.Log("m_uiImage isIn = " + isIn);
        //     }
        // }
        // for (int i = 0, l = m_ui.Length; i < l; ++i) {
        //     if(m_ui[i].MouseInUI){
        //         isIn = true;
        //         Debug.Log("m_ui isIn = " + isIn);
        //     }
        // }
        // Debug.Log("FINAL isIn = " + isIn);
        
        // if(!isIn){
        //     Debug.Log("Tata");
        // }
    }

    IEnumerator WaitToExit(){
        yield return new WaitForEndOfFrame();
        bool isIn = false;
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            if(m_uiImage[i].MouseInUI){
                isIn = true;
            }
        }
        for (int i = 0, l = m_ui.Length; i < l; ++i) {
            if(m_ui[i].MouseInUI){
                isIn = true;
            }
        }
        if(!isIn){
            for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
                m_uiImage[i].CloseUI();
            }
        }
    }
    
}

// Before just use PointerOver
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JainaUiController : MonoBehaviour {
    
    [SerializeField] JainaUI[] m_uiImage;

    JainaUI m_selectedUI;
    public JainaUI SelectedUI{
        get{
            return m_selectedUI;
        }
        set{
            m_selectedUI = value;
        }
    }

    void Awake(){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            m_uiImage[i].UiController = this;
        }
    }

    public void On_UiPointerOver(JainaUI clickedUI){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            if(clickedUI != m_uiImage[i]){
                m_uiImage[i].CloseUI();
            }
        }
    }

    public void On_UiPointerClick(JainaUI clickedUI, bool isSelected){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            if(clickedUI != m_uiImage[i]){
                m_uiImage[i].CloseUI();
            }
            m_selectedUI = isSelected ? clickedUI : null;
        }
    }
    
}*/
