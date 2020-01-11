using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLifeBar : MonoBehaviour
{
    public BigEnemyLifeBarManager lifeBar;
    public EnemyStats bossStats;
    bool FightOn;
    public ParticleSystem[] flammingDoor;
    public GameObject cantBlinkAgent;

    GolemController m_golemController;
    CameraManager m_cameraManager;

    void Start()
    {
        m_golemController = GolemController.Instance;
        m_cameraManager = CameraManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !FightOn)
        {
            FightOn = true;
            PlayerManager.Instance.GetComponent<PlayerStats>().IsInCombat = FightOn;
            for (int i = 0, l = flammingDoor.Length; i < l; ++i)
            {
                if (flammingDoor[i] != null)
                { 
                    flammingDoor[i].Play(true); 
                }
            }
            if (!cantBlinkAgent.activeSelf)
            {
                cantBlinkAgent.SetActive(true);
            }

            m_golemController.On_StartFight();
            m_cameraManager.StartCoroutine(m_cameraManager.SwitchCamOffset());
        }
    }

    private void Update()
    {
        if (FightOn)
        {
            lifeBar.OnLoadBossGameObject(bossStats);
            lifeBar.OnFightBoss(true);
        }
    }
}
