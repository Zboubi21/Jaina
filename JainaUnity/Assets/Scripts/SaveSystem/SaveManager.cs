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
            gameObject.SetActive(false);
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

    [Header("FX")]
    [SerializeField] CheckPointFx[] m_checkPointFxs;

    [System.Serializable] public class CheckPointFx {
		public GameObject m_fx;
        public float m_timeToAddFx = 0;
	}

    Vector3 m_startGamePosition;
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
    List<Checkpoint> m_checkpoints = new List<Checkpoint>();

    void Start(){
        m_checkpointCanvasAnimator = m_checkPointCanvas.GetComponent<Animator>();
        m_playerManager = PlayerManager.Instance;
        m_objectPooler = ObjectPooler.Instance;
        m_startGamePosition = m_playerManager.transform.position;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.P)){
            m_checkpoints.Clear();
        }
    }

    public void AddCheckpoint(Checkpoint newCheckpoint){
        m_checkpoints.Add(newCheckpoint);
    }

    public void On_CheckpointIsTake(Transform newSavePosition, int newCheckpointNumber){
        m_savePosition = newSavePosition.position;
        StartCoroutine(SeeCheckpointCanvas());

        PlayerManager.Instance.GetComponent<PlayerStats>().FullHeal();
        m_objectPooler.On_ReturnLifePotionInPool();

        m_actualCheckpointNumber = newCheckpointNumber;

        for (int i = 0, l = m_checkPointFxs.Length; i < l; ++i) {
            StartCoroutine(FxCoroutines(m_checkPointFxs[i].m_fx, m_checkPointFxs[i].m_timeToAddFx));
        }
    }

    IEnumerator FxCoroutines(GameObject fx, float timeToWait){
        yield return new WaitForSeconds(timeToWait);
        Level.AddFX(fx, Vector3.zero, Quaternion.identity);
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
        ReloadScene();
        yield return new WaitForSeconds(m_timeToStartFadeOut);
        ResetPlayerPos(m_savePosition);
        m_playerManager.SetPlayerMenuMode(false);
        SetCameraPos();
        m_dieAnimator.SetTrigger("FadeOut");
    }

    public void ReloadScene(){
        m_objectPooler.On_ReturnAllInPool();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void ResetPlayerPos(Vector3 savePos){
        m_playerManager = PlayerManager.Instance;
        m_playerManager.SetTpPoint(savePos);
    }

    public IEnumerator On_RestartFromLastCheckPoint(){
        ReloadScene();
        yield return new WaitForFixedUpdate();
        ResetPlayerPos(m_savePosition);
        m_playerManager.SetPlayerMenuMode(false);
        SetCameraPos();
    }
    public IEnumerator On_RestartGame(){
        ReloadScene();
        for (int i = 0, l = m_checkpoints.Count; i < l; ++i) {
            if(m_checkpoints[i] != null){
                m_checkpoints[i].ResetCheckpoint();
            }
        }
        m_actualCheckpointNumber = 0;
        m_savePosition = m_startGamePosition;
        yield return new WaitForFixedUpdate();
        ResetPlayerPos(m_startGamePosition);
        m_playerManager.SetPlayerMenuMode(false);
    }

    void SetCameraPos(){
        CameraManager.Instance.ResetPosition();
    }

}
