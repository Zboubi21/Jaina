using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Arena_Victory : MonoBehaviour
{
    public Waves_Methods arena;
    public GameObject clock;
    public Image img;
    public GameObject victoryScreen;

    private void OnEnable()
    {
        StartCoroutine(FadeIn());



        float second = arena.Second;
        float minutes = arena.Minutes;
        TextMeshProUGUI textPro = clock.GetComponent<TextMeshProUGUI>();
        if (second < 10)
        {
            if (minutes < 10)
            {
                textPro.text = string.Format("0{0}:0{1}", (int)minutes, (int)second);
            }
            else
            {
                textPro.text = string.Format("{0}:0{1}", (int)minutes, (int)second);
            }

        }
        else
        {
            if (minutes < 10)
            {
                textPro.text = string.Format("0{0}:{1}", (int)minutes, (int)second);
            }
            else
            {
                textPro.text = string.Format("{0}:{1}", (int)minutes, (int)second);
            }
        }
    }

    IEnumerator FadeIn()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            Color alp = img.color;
            alp.a += 0.05f;
            img.color = alp;
            if(alp.a >= 1)
            {
                StopCoroutine(FadeIn());
                break;
            }else if (alp.a > 0.8f)
            {
                victoryScreen.SetActive(true);
            }
        }
    }
}
