using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZglorgSoundManager : MonoBehaviour {

#region Singleton

	public static ZglorgSoundManager Instance;

	void Awake(){
		if(Instance == null){
			Instance = this;
            // DontDestroyOnLoad(gameObject);
		}else{
			Debug.LogError("Two instance of ZglorgSoundManager");
			// gameObject.SetActive(false);
            // Destroy(gameObject);
		}
	}

#endregion Singleton
    
    [Header("Detected sound")]
    public ZglorgSound m_detectedSound = new ZglorgSound();

    [Header("Impatience sound")]
    public ZglorgSound m_impatienceSound = new ZglorgSound();

    [Header("Death sound")]
    public ZglorgSound m_deathSound = new ZglorgSound();

	[System.Serializable] public class ZglorgSound {
        public float m_minTimeBetweenSound = 0.5f;
        public float m_maxTimeBetweenSound = 1f;
        // [Space]
        // public float m_maxNumberOfSameSound = 2;
        // public float m_maxTimeToHaveSameSound = 0.5f;
        [HideInInspector] public bool m_canDoSound = true;
    }

    public bool CanDoDetectedSound(){
        if(m_detectedSound.m_canDoSound){
            StartCoroutine(WaitToCanDoDetectedSound());
            return true;
        }else{
            return false;
        }
    }
    IEnumerator WaitToCanDoDetectedSound(){
        m_detectedSound.m_canDoSound = false;
        float alea = Random.Range(m_detectedSound.m_minTimeBetweenSound, m_detectedSound.m_maxTimeBetweenSound);
        yield return new WaitForSeconds(alea);
        m_detectedSound.m_canDoSound = true;
    }

    public bool CanDoImpatienceSound(){
        if(m_impatienceSound.m_canDoSound){
            StartCoroutine(WaitToCanDoImpatienceSound());
            return true;
        }else{
            return false;
        }
    }
    IEnumerator WaitToCanDoImpatienceSound(){
        m_impatienceSound.m_canDoSound = false;
        float alea = Random.Range(m_impatienceSound.m_minTimeBetweenSound, m_impatienceSound.m_maxTimeBetweenSound);
        yield return new WaitForSeconds(alea);
        m_impatienceSound.m_canDoSound = true;
    }

    public bool CanDoDeathSound(){
        if(m_deathSound.m_canDoSound){
            StartCoroutine(WaitToCanDoDeathSound());
            return true;
        }else{
            return false;
        }
    }
    IEnumerator WaitToCanDoDeathSound(){
        m_deathSound.m_canDoSound = false;
        float alea = Random.Range(m_deathSound.m_minTimeBetweenSound, m_deathSound.m_maxTimeBetweenSound);
        yield return new WaitForSeconds(alea);
        m_deathSound.m_canDoSound = true;
    }

}
