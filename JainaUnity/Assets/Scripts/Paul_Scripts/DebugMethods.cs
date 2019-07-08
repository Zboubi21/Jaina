using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMethods : MonoBehaviour
{

    Transform parent;

    public GameObject[] combosArcan;
    public GameObject[] combosFire;
    public GameObject[] combosIce;


    public Text[] arcanText;
    public Text[] fireText;
    public Text[] iceText;

    int[] count;

    Text[] allCombo;

    private void Start()
    {
        count = new int[6];
        parent = GetComponent<Transform>();
        for (int a = 0; a < fireText.Length; a++)
        {
            arcanText[a].text = "";
            iceText[a].text = "";
            fireText[a].text = "";
        }
    }
    bool go;
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
        {
            go = !go;
            allCombo = parent.GetComponentsInChildren<Text>();
            for (int i = 0, l = allCombo.Length; i < l; ++i)
            {
                if(allCombo[i].gameObject != null)
                {
                    Destroy(allCombo[i].gameObject);
                }

            }
            for (int i = 0; i < count.Length; i++)
            {
                count[i] = 0;
            }

            for (int a = 0; a < fireText.Length; a++)
            {
                arcanText[a].text = "";
                iceText[a].text = "";
                fireText[a].text = "";
            }
        }


        if (go)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                switch (PlayerManager.Instance.m_currentElement)
                {
                    case PlayerManager.ElementType.Arcane:
                        Instantiate(combosArcan[0], parent);
                        count[0]++;
                        arcanText[0].text = string.Format("A : {0}", count[0]);
                        break;
                    case PlayerManager.ElementType.Ice:
                        Instantiate(combosIce[0], parent);
                        count[1]++;
                        iceText[0].text = string.Format("A : {0}", count[1]);
                        break;
                    case PlayerManager.ElementType.Fire:
                        Instantiate(combosFire[0], parent);
                        count[2]++;
                        fireText[0].text = string.Format("A : {0}", count[2]);
                        break;
                    default:
                        break;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Z))
            {
                switch (PlayerManager.Instance.m_currentElement)
                {
                    case PlayerManager.ElementType.Arcane:
                        Instantiate(combosArcan[1], parent);
                        count[3]++;
                        arcanText[1].text = string.Format("Z : {0}", count[3]);
                        break;
                    case PlayerManager.ElementType.Ice:
                        Instantiate(combosIce[1], parent);
                        count[4]++;
                        iceText[1].text = string.Format("Z : {0}", count[4]);
                        break;
                    case PlayerManager.ElementType.Fire:
                        Instantiate(combosFire[1], parent);
                        count[5]++;
                        fireText[1].text = string.Format("Z : {0}", count[5]);
                        break;
                    default:
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                allCombo = parent.GetComponentsInChildren<Text>();
                for (int i = 0, l = allCombo.Length; i < l; ++i)
                {
                    if (allCombo[i].gameObject != null)
                    {
                        Destroy(allCombo[i].gameObject);
                    }
                }

                for (int i = 0; i < count.Length; i++)
                {
                    count[i] = 0;
                }

                for (int a = 0; a < fireText.Length; a++)
                {
                    arcanText[a].text = "";
                    iceText[a].text = "";
                    fireText[a].text = "";
                }
            }
        }
    }
}
