using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeBarArray : MonoBehaviour
{
    public TextMeshProUGUI m_unitName;
    [Space]
    public Image m_lifeBar;
    public Image m_whiteLifeBar;
    [Space]
    public Transform m_markRoot;
    [Space]
    public GameObject[] m_levelOfLifeBar;
    [Space]
    public GameObject[] phaseIcons;
}
