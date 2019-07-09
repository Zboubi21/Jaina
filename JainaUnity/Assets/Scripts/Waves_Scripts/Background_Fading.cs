using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Background_Fading : MonoBehaviour
{


    public Material mat;
    public TextMeshProUGUI text;
    public float fadeSpeed = 0.04f;

    float alpha = 1;
    //public bool WavesOn;


    /*bool lockon;
    bool lockoff = true;*/

    private void Update()
    {
        /*if (WavesOn && lockon)
        {
            lockoff = true;
            StartCoroutine(AlphaReduction(0.1f));
            lockon = false;
        }
        else if(!WavesOn && lockoff)
        {
            lockoff = false;
            StartCoroutine(AlphaReduction(0.1f));
            lockon = true;
        }*/
    }
    /*private void OnEnable()
    {
        StartCoroutine(AlphaReduction(-0.1f));
    }*/


    public IEnumerator AlphaReduction(float reduc)
    {

        while (alpha >= 0 && alpha <= 1)
        {
            yield return new WaitForSeconds(fadeSpeed);
            alpha += reduc;
            text.alpha += -reduc/1.5f;

            /*if (!WavesOn)
            {
                text.alpha -= reduc;
            }
            else
            {
                alpha -= reduc;
                text.alpha += reduc;
            }*/
            mat.SetFloat("_Alpha", alpha);
        }
        if(alpha < 0)
        {
            alpha = 0;
        }
        else if(alpha > 1)
        {
            alpha = 1;
            /*enabled = false;
            gameObject.SetActive(false);*/
        }
        StopCoroutine(AlphaReduction(reduc));
    }
}
