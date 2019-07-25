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
    public Waves_Methods NextWaveMethods;
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
            return minute;
        }

        set
        {
            minute = value;
        }
    }

    public float Second
    {
        get
        {
            return seconds;
        }

        set
        {
            seconds = value;
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

    public float MinutesWave
    {
        get
        {
            return minutesWave;
        }

        set
        {
            minutesWave = value;
        }
    }

    public float SecondWave
    {
        get
        {
            return secondWave;
        }

        set
        {
            secondWave = value;
        }
    }

    public int TotalOfWave
    {
        get
        {
            return totalOfWave;
        }

        set
        {
            totalOfWave = value;
        }
    }

    #endregion

    public void OnLaunchWave()
    {
        if (!isLaunchByTrigger)
        {
            _playerOnTrigger = true;
            if (PreviousWaveMethods != null)
            {
                maximumDeVague = PreviousWaveMethods.maximumDeVague;
            }
            float timeToWave = m_timeBetweenEachWave[0];
            StartCoroutine(WaitForFirstWave(timeToWave));
        }
    }

    IEnumerator WaitForFirstWave(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            time -= Time.deltaTime;
            Debug.Log(this.name +" this is the time : " + time);
            Debug.Log(this.name +" previous dead : " + PreviousWaveMethods.nbrEnemyDead);
            Debug.Log(this.name +" previous enemy : " + PreviousWaveMethods.nbrOfEnemy);
            if (time <= 0 || PreviousWaveMethods.nbrEnemyDead == PreviousWaveMethods.nbrOfEnemy || PreviousWaveMethods.nbrEnemyDead == 0 || PreviousWaveMethods.nbrOfEnemy == 0)
            {
                playerStats = PlayerManager.Instance.GetComponent<PlayerStats>();
                playerStats.OnCheckArenaStillGoing(true);

                OnFirstWaveStart.Invoke();
                if (useArenaUI)
                {
                    OnUsingAreanUI(useArenaUI);
                }
                Debug.Log("this is the nbrOfWave :" + nbrOfWave);
                Spawner(nbrOfWave);
                StopCoroutine(WaitForFirstWave(time));
                break;
            }
        }
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
        else
        {
            seconds = PreviousWaveMethods.Second;
            /*if (seconds > 60f)
            {
                seconds = 0;
            }*/
            minute = PreviousWaveMethods.Minutes;
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
    float minute;
    float seconds;
    bool b = false;
    bool thisIsDone;
    private void Update()
    {
        if (_playerOnTrigger && nbrOfWave != nombreDeVague && !thisIsDone)
        {
            minutesWave = timeNextWave / 60f;
            secondWave = timeNextWave % 60f;
            timeNextWave -= Time.deltaTime;
            if(timeNextWave <= 0f && nbrOfEnemy != nbrEnemyDead)
            {

                Spawner(nbrOfWave);
            }
            else if(nbrOfEnemy == nbrEnemyDead && nbrEnemyDead != 0 && timeNextWave > 0f)
            {
                nbrEnemyDead = 0;
                nbrOfEnemy = 0;
                Spawner(nbrOfWave);
            }
        }
        else
        {
            if(NextWaveMethods == null)
            {
                if(nbrOfWave == nombreDeVague && ((nbrOfEnemy == nbrEnemyDead && nbrOfEnemy != 0) /*|| (timeNextWave <= 0f)*/) && !thisIsDone)
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
                    thisIsDone = true;
                }
            }
            else
            {
                minutesWave = timeNextWave / 60f;
                secondWave = timeNextWave % 60f;
                timeNextWave -= Time.deltaTime;
                if (nbrOfWave == nombreDeVague && ((nbrOfEnemy == nbrEnemyDead && nbrOfEnemy != 0) || (timeNextWave <= 0f)) && !thisIsDone)
                {
                    OnLastWaveOver.Invoke();
                    timeNextWave = NextWaveMethods.m_timeBetweenEachWave[0];
                    playerStats.OnCheckArenaStillGoing(false);
                    nbrOfEnemy = 0;
                    if (useArenaUI)
                    {
                        nbrOfWave = 0;
                        victoryScreen.SetActive(true);
                        waveUI.SetActive(false);
                        wave_Identifier.timerWave.fontSize = 45;
                    }
                    thisIsDone = true;
                }
            }
        }

        if (useArenaUI && !thisIsDone)
        {
            if(totalOfWave != 0)
            {
                OnChronoMethods();

                /*if (PreviousWaveMethods == null)
                {
                    OnChronoMethods(seconds);
                }
                else
                {
                    OnChronoMethods(PreviousWaveMethods.Second);
                }*/
                TimeToNextWaveMethods(minutesWave, secondWave);
            }
            if(NextWaveMethods != null && nbrOfWave == nombreDeVague && nbrOfWave != 0)
            {
                if (!saveNextWaveTimer)
                {
                    timeForNextWave = m_timeBetweenEachWave[nbrOfWave-1];
                    saveNextWaveTimer = true;
                }
                timeForNextWave -= Time.deltaTime;
                minutesNextWave = timeForNextWave / 60f;
                secondsNextWave = timeForNextWave % 60f;
                TimeToNextWaveMethods(minutesNextWave, secondsNextWave);
            }
            else
            {
                saveNextWaveTimer = false;
            }
        }
    }
    bool saveNextWaveTimer;
    float timeForNextWave;
    float minutesNextWave;
    float secondsNextWave;


    float timed;
    float timedMinutes;
    float timedSeconds;

    void TimeToNextWaveMethods(float minutesWave, float secondWave)
    {
        if (nbrOfWave != nombreDeVague && nbrOfWave != 0)
        {
            DisplayTime(wave_Identifier.timerWave, minutesWave, secondWave);

            //if (secondWave < 10)
            //{
            //    if (minutesWave < 10)
            //    {
            //        wave_Identifier.timerWave.text = string.Format("0{0}:0{1}", (int)minutesWave, (int)secondWave);
            //    }
            //    else
            //    {
            //        wave_Identifier.timerWave.text = string.Format("{0}:0{1}", (int)minutesWave, (int)secondWave);
            //    }

            //}
            //else
            //{
            //    if (minutesWave < 10)
            //    {
            //        wave_Identifier.timerWave.text = string.Format("0{0}:{1}", (int)minutesWave, (int)secondWave);
            //    }
            //    else
            //    {
            //        wave_Identifier.timerWave.text = string.Format("{0}:{1}", (int)minutesWave, (int)secondWave);
            //    }
            //}

            OnRedCloudChangeColor();
        }
        else if(totalOfWave == maximumDeVague && PreviousWaveMethods != null && maximumDeVague !=0 && totalOfWave!=0)
        {
            wave_Identifier.timerWave.fontSize = 35;
            wave_Identifier.timerWave.text = "Last Wave";
        }
        else
        {
            DisplayTime(wave_Identifier.timerWave, minutesWave, secondWave);

            //if (secondWave < 10)
            //{
            //    if (minutesWave < 10)
            //    {
            //        wave_Identifier.timerWave.text = string.Format("0{0}:0{1}", (int)minutesWave, (int)secondWave);
            //    }
            //    else
            //    {
            //        wave_Identifier.timerWave.text = string.Format("{0}:0{1}", (int)minutesWave, (int)secondWave);
            //    }

            //}
            //else
            //{
            //    if (minutesWave < 10)
            //    {
            //        wave_Identifier.timerWave.text = string.Format("0{0}:{1}", (int)minutesWave, (int)secondWave);
            //    }
            //    else
            //    {
            //        wave_Identifier.timerWave.text = string.Format("{0}:{1}", (int)minutesWave, (int)secondWave);
            //    }
            //}
        }
    }

    void OnShowTimeOnThisWave()
    {
        if(nbrOfWave - 1 >= 0)
        {
            timed = m_timeBetweenEachWave[nbrOfWave - 1] - timeNextWave;
            timedMinutes = timed / 60;
            timedSeconds = timed % 60;
            DisplayTime(wave_Identifier.TimeToEndWave, timedMinutes, timedSeconds);

            //if (timedSeconds < 10)
            //{
            //    if (timedMinutes < 10)
            //    {
            //        wave_Identifier.TimeToEndWave.text = string.Format("0{0}:0{1}", (int)timedMinutes, (int)timedSeconds);
            //    }
            //    else
            //    {
            //        wave_Identifier.TimeToEndWave.text = string.Format("{0}:0{1}", (int)timedMinutes, (int)timedSeconds);
            //    }

            //}
            //else
            //{
            //    if (timedMinutes < 10)
            //    {
            //        wave_Identifier.TimeToEndWave.text = string.Format("0{0}:{1}", (int)timedMinutes, (int)timedSeconds);
            //    }
            //    else
            //    {
            //        wave_Identifier.TimeToEndWave.text = string.Format("{0}:{1}", (int)timedMinutes, (int)timedSeconds);
            //    }
            //}
        }
        else if(PreviousWaveMethods != null)
        {
            DisplayTime(wave_Identifier.TimeToEndWave, PreviousWaveMethods.timedMinutes, PreviousWaveMethods.timedSeconds);

            //if (PreviousWaveMethods.timedSeconds < 10)
            //{
            //    if (PreviousWaveMethods.timedMinutes < 10)
            //    {
            //        wave_Identifier.TimeToEndWave.text = string.Format("0{0}:0{1}", (int)PreviousWaveMethods.timedMinutes, (int)PreviousWaveMethods.timedSeconds);
            //    }
            //    else
            //    {
            //        wave_Identifier.TimeToEndWave.text = string.Format("{0}:0{1}", (int)PreviousWaveMethods.timedMinutes, (int)PreviousWaveMethods.timedSeconds);
            //    }

            //}
            //else
            //{
            //    if (PreviousWaveMethods.timedMinutes < 10)
            //    {
            //        wave_Identifier.TimeToEndWave.text = string.Format("0{0}:{1}", (int)PreviousWaveMethods.timedMinutes, (int)PreviousWaveMethods.timedSeconds);
            //    }
            //    else
            //    {
            //        wave_Identifier.TimeToEndWave.text = string.Format("{0}:{1}", (int)PreviousWaveMethods.timedMinutes, (int)PreviousWaveMethods.timedSeconds);
            //    }
            //}
        }
        else
        {
            wave_Identifier.TimeToEndWave.text = string.Format("0{0}:0{1}", 0, 0);
        }
    }

    void OnChronoMethods()
    {
        seconds += Time.deltaTime;
        if ((int)seconds >= 60)
        {
            seconds = 0;
            minute++;
        }
        DisplayTime(wave_Identifier.Chrono, minute, seconds);
    }

    void DisplayTime(TMP_Text text,float minute,float seconds)
    {
        if (seconds < 10)
        {
            if (minute < 10)
            {
                text.text = string.Format("0{0}:0{1}", (int)minute, (int)seconds);
            }
            else
            {
                text.text = string.Format("{0}:0{1}", (int)minute, (int)seconds);
            }

        }
        else
        {
            if (minute < 10)
            {
                text.text = string.Format("0{0}:{1}", (int)minute, (int)seconds);
            }
            else
            {
                text.text = string.Format("{0}:{1}", (int)minute, (int)seconds);
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
    int totalOfWave;
    void Spawner(int wave)
    {
        wave_Identifier.timerWave.fontSize = 45;
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
        
        /*if (nbrOfWave == nombreDeVague && NextWaveMethods != null && !thisIsDone)
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
            thisIsDone = true;
        }*/
        if (useArenaUI)
        {
            if(PreviousWaveMethods == null)
            {
                //Debug.Log(this.name + " called ?? wtf");
                totalOfWave = NombreDeVague;
                wave_Identifier.waveCounter.text = string.Format("{0}", nbrOfWave);
            }
            else
            {
                //Debug.Log(this.name + " I'm wave 6");
                totalOfWave = nbrOfWave + PreviousWaveMethods.TotalOfWave;
                wave_Identifier.waveCounter.text = string.Format("{0}", totalOfWave);
            }
        }
    }
}
