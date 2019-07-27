using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnShowLogo : MonoBehaviour
{
    [Header("Image Référence")]
    public Image FirstPanel;
    public Image Logo;
    Image lastPanel;
    [Space]
    [Header("Image Référence")]
    public float firstPanelFadeSpeed = 1;
    public float timeToShowLogo = 1;
    public float logoFadeSpeed = 1;
    public float lastPanelFadeSpeed = 1;
    [Space]
    [Header("Activate At End Of anim")]
    public GameObject[] obj;


    public static bool showLogo;

    void Start()
    {
        if (!showLogo)
        {
            lastPanel = GetComponent<Image>();
            for (int i = 0, l = obj.Length; i < l; ++i)
            {
                obj[i].SetActive(false);
            }
            showLogo = true;
            StartCoroutine(Fades());
        }
        else
        {
            for (int i = 0, l = obj.Length; i < l; ++i)
            {
                obj[i].SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }

    IEnumerator Fades()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f/Mathf.Abs(firstPanelFadeSpeed));
            Color alp = FirstPanel.color;
            alp.a -= 0.01f;
            FirstPanel.color = alp;
            if(alp.a <= 0)
            {
                yield return new WaitForSeconds(timeToShowLogo);
                while (true)
                {
                    yield return new WaitForSeconds(0.02f / Mathf.Abs(logoFadeSpeed));
                    Color Lalp = Logo.color;
                    Lalp.a -= 0.01f;
                    Logo.color = Lalp;
                    if(Lalp.a <= 0)
                    {
                        for (int i = 0, l = obj.Length; i < l; ++i)
                        {
                            obj[i].SetActive(true);
                        }
                        while (true)
                        {
                            yield return new WaitForSeconds(0.02f / Mathf.Abs(lastPanelFadeSpeed));
                            Color LAalp = lastPanel.color;
                            LAalp.a -= 0.01f;
                            lastPanel.color = LAalp;
                            if(LAalp.a <= 0)
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }
    }
}
