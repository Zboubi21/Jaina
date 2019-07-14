using System.Collections;
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
    
}
