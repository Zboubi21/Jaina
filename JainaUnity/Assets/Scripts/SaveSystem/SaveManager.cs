using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour {

#region Singleton
	public static SaveManager Instance;
    void Awake(){
		if(Instance == null){
			Instance = this;
            DontDestroyOnLoad(gameObject);
		}else{
			// Debug.LogError("Two instance of SaveManager");
            Destroy(gameObject);
		}
	}
#endregion Singleton

    [Header("Checkoint animation")]
    [SerializeField] GameObject m_checkPointCanvas;
    [SerializeField] float m_timeToSeeCheckPointCanvas = 2;

    [Header("Die animation")]
    public Animator m_dieAnimator;
    [SerializeField] float m_timeToStartFadeIn = 0.5f;
    [SerializeField] float m_timeToRespawn = 2.25f;
    [SerializeField] float m_timeToStartFadeOut = 0.1f;
    
    bool m_checkpointIsTake = false;
    public bool CheckpointIsTake{
        get{
            return m_checkpointIsTake;
        }
    }

    Vector3 m_savePosition;
    int m_actualCheckpointNumber = 0;
    public int ActualCheckpointNumber{
        get{
            return m_actualCheckpointNumber;
        }
    }

    Animator m_checkpointCanvasAnimator;
    PlayerManager m_playerManager;
    ObjectPooler m_objectPooler;

    void Start(){
        m_checkpointCanvasAnimator = m_checkPointCanvas.GetComponent<Animator>();
        m_playerManager = PlayerManager.Instance;
        m_objectPooler = ObjectPooler.Instance;
    }

    public void On_CheckpointIsTake(Transform newSavePosition, int newCheckpointNumber){
        m_savePosition = newSavePosition.position;
        StartCoroutine(SeeCheckpointCanvas());

        if(!m_checkpointIsTake){
            m_checkpointIsTake = true;
        }
        m_actualCheckpointNumber = newCheckpointNumber;
    }

    IEnumerator SeeCheckpointCanvas(){
        m_checkpointCanvasAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(m_timeToSeeCheckPointCanvas);
        m_checkpointCanvasAnimator.SetTrigger("FadeOut");
    }

    public void On_PlayerDie(){
        StartCoroutine(DieCoroutine());
    }
    
    IEnumerator DieCoroutine(){
        yield return new WaitForSeconds(m_timeToStartFadeIn);
        m_dieAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(m_timeToRespawn);
        m_objectPooler.On_ReturnAllPool();
        SceneManager.LoadScene(0);
        yield return new WaitForSeconds(m_timeToStartFadeOut);
        PlayerManager.Instance.SetTpPoint(m_savePosition);
        m_dieAnimator.SetTrigger("FadeOut");
    }


}
