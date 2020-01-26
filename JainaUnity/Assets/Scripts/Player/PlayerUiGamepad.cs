using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiGamepad : MonoBehaviour
{
    
    Toggle m_gamepadToggle;
    PlayerManager m_playerManager;

    void Start()
    {
        m_gamepadToggle = GetComponent<Toggle>();
        m_playerManager = PlayerManager.Instance;
        m_gamepadToggle.isOn = m_playerManager.m_playerDebug.m_useGamepad;
    }
    
}
