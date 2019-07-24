using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Waves_Methods : MonoBehaviour
{
    [Header("Arena UI")]
    [Space]
    [Tooltip("Is This Arena Using The Arena UI ?")]
    public bool useArenaUI;

    public GameObject waveUI;
    public GameObject victoryScreen;
    public Waves_Methods PreviousWaveMethods;
    public int maximumDeVague;
    UI_Wave_Identifier wave_Identifier;

    [Header("Spawner Var")]
    [Space]
    public bool isLaunchByTrigger = true;
    [Space]
    public GameObject[] m_spawners;
    [Space]
    public float[] m_timeBetweenEachWave;
    [Space]

    public float timeToSpawn = 0.5f;
    [Space]
    [Header("Spawner Event")]
    [Space]

    public UnityEvent OnFirstWaveStart;
    public UnityEvent OnAnyWaveStart;
    public UnityEvent OnLastWaveOver;

    Spawner_Methods[] _spawnerMethod;

    PlayerStats playerStats;

    int nbrOfWave;
    int nombreDeVague;
    float timeNextWave;

    bool _playerOnTrigger;


    int nbrOfEnemy;
    int nbrEnemyDead;

    #region Get Set
    public int NbrOfEnemy
    {
        get
        {
            return nbrOfEnemy;
        }

        set
        {
            nbrOfEnemy = value;
        }
    }

    public float Minutes
    {
        get
        {
            return minutes;
        }

        set
        {
            minutes = value;
        }
    }

    public float Second
    {
        get
        {
            return second;
        }

        set
        {
            second = value;
        }
    }

    public int NombreDeVague
    {
        get
        {
            return nombreDeVague;
        }

        set
        {
            nombreDeVague = value;
        }
    }

    #endregion

    public void OnLaunchWave(float TimeBeforeNextWave)
    {
        if (!isLaunchByTrigger)
        {
            _playerOnTrigger = true;
            StartCoroutine(WaitForFirstWave(TimeBeforeNextWave));
        }
    }

    IEnumerator WaitForFirstWave(float time)
    {
        yield return new WaitForSeconds(time);
        playerStats = PlayerManager.Instance.GetComponent<PlayerStats>();
        playerStats.OnCheckArenaStillGoing(true);

        OnFirstWaveStart.Invoke();
        if (useArenaUI)
        {
            OnUsingAreanUI(useArenaUI);
        }
        Spawner(nbrOfWave);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_playerOnTrigger && isLaunchByTrigger)
            {
                playerStats = other.GetComponent<PlayerStats>();
                if (useArenaUI)
                {
                    OnUsingAreanUI(useArenaUI);
                }
                Spawner(nbrOfWave);
                _playerOnTrigger = true;
                playerStats.OnCheckArenaStillGoing(true);
                OnFirstWaveStart.Invoke();
            }
        }
    }
    void OnUsingAreanUI(bool b)
    {
        wave_Identifier = waveUI.GetComponent<UI_Wave_Identifier>();
        Color color = wave_Identifier.redCloud.color;
        color.b = 1f;
        color.g = 1f;
        color.r = 1f;
        wave_Identifier.redCloud.color = color;
        if(PreviousWaveMethods == null)
        {
            wave_Identifier.maxWave.text = string.Format("{0}", maximumDeVague);
        }
        wave_Identifier.timerWave.fontSize = 45;
        waveUI.SetActive(b);
    }
    private void Start()
    {
        timeNextWave = m_timeBetweenEachWave[0];
        _spawnerMethod = new Spawner_Methods[m_spawners.Length];
        
        for (int a = 0, f = m_spawners.Length; a < f; a++)
        {
            if(m_spawners != null)
            {
                _spawnerMethod[a] = m_spawners[a].GetComponent<Spawner_Methods>();
            }
        }
        for (int i = 0, l = m_spawners.Length; i < l; ++i)
        {
            if (m_spawners != null)
            {
                if (m_spawners.Length-1 != i)
                {
                    if (_spawnerMethod[i]._nbrOfWaves.Length != _spawnerMethod[i+1]._nbrOfWaves.Length)
                    {
                        Debug.LogError("Le spawner " + _spawnerMethod[i] + " a un nombre de vagues différentes du spawner " + _spawnerMethod[i - 1] + ". Tous les spawners d'une arène doivent avoir le même nombre de vague");
                    }
                    else
                    {
                        nombreDeVague = _spawnerMethod[i]._nbrOfWaves.Length;
                    }
                }
                else
                {
                    nombreDeVague = _spawnerMethod[i]._nbrOfWaves.Length;
                }
            }
        }
        if (useArenaUI)
        {
            OnUsingAreanUI(false);
        }
    }
    float minutesWave;
    float secondWave;
    float minutes;
    float second;
    bool b = false;

    private void Update()
    {
        if (_playerOnTrigger && nbrOfWave != nombreDeVague)
        {
            minutesWave = timeNextWave / 60f;
            secondWave = timeNextWave % 60f;
            timeNextWave -= Time.deltaTime;
            if(timeNextWave <= 0)
            {

                Spawner(nbrOfWave);
            }
            else if(nbrOfEnemy == nbrEnemyDead && nbrEnemyDead != 0)
            {
                nbrEnemyDead = 0;
                nbrOfEnemy = 0;
                Spawner(nbrOfWave);
            }
        }
        else if (nbrOfWave == nombreDeVague && nbrOfEnemy == nbrEnemyDead && nbrOfEnemy !=0)
        {
            OnLastWaveOver.Invoke();
            playerStats.OnCheckArenaStillGoing(false);
            nbrOfEnemy = 0;
            if (useArenaUI)
            {
                nbrOfWave = 0;
                victoryScreen.SetActive(true);
                waveUI.SetActive(false);
                wave_Identifier.timerWave.fontSize = 45;
            }
        }
        if (useArenaUI)
        {
            if(nbrOfWave != 0)
            {
                if(PreviousWaveMethods == null)
                {
                    OnChronoMethods(second,minutes);
                }
                else
                {
                    OnChronoMethods(PreviousWaveMethods.Second, PreviousWaveMethods.Minutes);
                }
            }
            TimeToNextWaveMethods();
        }
    }

    void TimeToNextWaveMethods()
    {
        if (nbrOfWave != nombreDeVague && nbrOfWave != 0)
        {

            if (secondWave < 10)
            {
                if (minutesWave < 10)
                {
                    wave_Identifier.timerWave.text = string.Format("0{0}:0{1}", (int)minutesWave, (int)secondWave);
                }
                else
                {
                    wave_Identifier.timerWave.text = string.Format("{0}:0{1}", (int)minutesWave, (int)secondWave);
                }

            }
            else
            {
                if (minutesWave < 10)
                {
                    wave_Identifier.timerWave.text = string.Format("0{0}:{1}", (int)minutesWave, (int)secondWave);
                }
                else
                {
                    wave_Identifier.timerWave.text = string.Format("{0}:{1}", (int)minutesWave, (int)secondWave);
                }
            }

            OnRedCloudChangeColor();
        }
        else
        {
            wave_Identifier.timerWave.fontSize = 35;
            wave_Identifier.timerWave.text = "Last Wave";
        }
    }

    void OnShowTimeOnThisWave()
    {
        if(nbrOfWave - 1 >= 0)
        {
            float timed = m_timeBetweenEachWave[nbrOfWave - 1] - timeNextWave;
            float timedMinutes = timed / 60;
            float timedSeconds = timed % 60;
            if (timedSeconds < 10)
            {
                if (timedMinutes < 10)
                {
                    wave_Identifier.TimeToEndWave.text = string.Format("0{0}:0{1}", (int)timedMinutes, (int)timedSeconds);
                }
                else
                {
                    wave_Identifier.TimeToEndWave.text = string.Format("{0}:0{1}", (int)timedMinutes, (int)timedSeconds);
                }

            }
            else
            {
                if (timedMinutes < 10)
                {
                    wave_Identifier.TimeToEndWave.text = string.Format("0{0}:{1}", (int)timedMinutes, (int)timedSeconds);
                }
                else
                {
                    wave_Identifier.TimeToEndWave.text = string.Format("{0}:{1}", (int)timedMinutes, (int)timedSeconds);
                }
            }
        }
        else
        {
            wave_Identifier.TimeToEndWave.text = string.Format("0{0}:0{1}", 0,0);
        }
    }

    void OnChronoMethods(float second,float minutes)
    {
        second += Time.deltaTime;
        if ((int)second >= 60)
        {
            second = 0;
            minutes++;
        }
        if (second < 10)
        {
            if (minutes < 10)
            {
                wave_Identifier.Chrono.text = string.Format("0{0}:0{1}", (int)minutes, (int)second);
            }
            else
            {
                wave_Identifier.Chrono.text = string.Format("{0}:0{1}", (int)minutes, (int)second);
            }

        }
        else
        {
            if (minutes < 10)
            {
                wave_Identifier.Chrono.text = string.Format("0{0}:{1}", (int)minutes, (int)second);
            }
            else
            {
                wave_Identifier.Chrono.text = string.Format("{0}:{1}", (int)minutes, (int)second);
            }
        }
    }

    void OnRedCloudChangeColor()
    {
        if (secondWave > 10 || minutesWave > 1)
        {
            Color color = wave_Identifier.redCloud.color;
            color.b = Mathf.InverseLerp(10, m_timeBetweenEachWave[nbrOfWave - 1], timeNextWave);
            color.g = Mathf.InverseLerp(10, m_timeBetweenEachWave[nbrOfWave - 1], timeNextWave);
            color.r = 1f;
            wave_Identifier.redCloud.color = color;
        }
        else
        {
            Color color = wave_Identifier.redCloud.color;
            if (color.r > 0.8f && !b)
            {
                color.b = 0f;
                color.g = 0f;
                color.r = color.r - 0.004f;
                wave_Identifier.timerWave.fontSize = wave_Identifier.timerWave.fontSize + 0.125f;
                wave_Identifier.redCloud.color = color;

            }
            else
            {
                b = true;
                color.b = 0f;
                color.g = 0f;
                color.r = color.r + 0.004f;
                wave_Identifier.timerWave.fontSize = wave_Identifier.timerWave.fontSize - 0.125f;
                wave_Identifier.redCloud.color = color;
                if (color.r >= 1f)
                {
                    color.r = 1f;
                    b = false;
                }
            }
        }
    }

    public void CountDeath()
    {
        nbrEnemyDead++;
    }

    void Spawner(int wave)
    {

        //nbrEnemyDead = 0;
        //nbrOfEnemy = 0;
        if (useArenaUI)
        {
            OnShowTimeOnThisWave();
        }
        

        OnAnyWaveStart.Invoke();
        if (wave > m_timeBetweenEachWave.Length-1)
        {
            timeNextWave = m_timeBetweenEachWave[wave-1];
        }
        else
        {
            timeNextWave = m_timeBetweenEachWave[wave];
        }
        for (int i = 0, l = m_spawners.Length; i < l; ++i)
        {
            if (m_spawners[i] != null)
            {
                _spawnerMethod[i].StartCoroutine(_spawnerMethod[i].WaveSpawner(i, wave, this));
            }
        }
        nbrOfWave++;
        if (useArenaUI)
        {
            if(PreviousWaveMethods == null)
            {
                wave_Identifier.waveCounter.text = string.Format("{0}", nbrOfWave);
            }
            else
            {
                wave_Identifier.waveCounter.text = string.Format("{0}", nbrOfWave + PreviousWaveMethods.NombreDeVague);
            }
        }
    }
}
