using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;
using PlayerStateEnum;
using PoolTypes;
using DuloGames.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance;

	public PlayerDebug m_playerDebug = new PlayerDebug();
	[Serializable] public class PlayerDebug {
		public bool m_playerCanDie = true;
		public bool m_useSymetricalHudSpellAnim = true;
		public PlayerState m_playerStartState;
		// [Space]
		// public Transform m_fromPos;
		// public Transform m_toPos;
		// public Transform m_newToPos;
	}

	public StateMachine m_sM = new StateMachine();

#region Enum
	public ElementType m_currentElement = ElementType.Fire;
	public enum ElementType{
		Arcane,
		Ice,
		Fire
	}
	[HideInInspector] public ElementType m_arcaneElement = ElementType.Arcane;
	[HideInInspector] public ElementType m_iceElement = ElementType.Ice;
	[HideInInspector] public ElementType m_fireElement = ElementType.Fire;

#endregion Enum

#region Public [Serializable] Variables

	[Header("AutoAttacks")]
	public AutoAttacks m_autoAttacks = new AutoAttacks();
	[Serializable] public class AutoAttacks {
		public float m_cooldown = 0.25f;
		public float m_buffCooldown = 0.125f;
		[HideInInspector] public bool m_isBuff = false;
		[HideInInspector] public float m_actualCooldown = 0;

		[Header("Prefabs")]
		public Transform m_positionRoot;
		public Transform m_rotationRoot;
		
		[Header("Sounds")]
		public GameObject[] m_autoAttacksSounds;
	}

	[Header("Powers")]
	public Powers m_powers = new Powers();
	[Serializable] public class Powers {

		public Blink m_blink = new Blink();
		[Serializable] public class Blink {
			[HideInInspector] public bool m_canSwitch = true;
			[HideInInspector] public bool m_startCooldown = false;
			public float m_cooldown = 5;
			[HideInInspector] public float m_actualCooldown = 0;
			public float m_maxDistance = 5;
			public bool m_useMouseBlink = true;
			public Transform m_rayCastToCanBlink;
			public LayerMask m_colliderToCanNotBlink;

			[Header("UI")]
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;

			[Header("FX")]
			public ParticleSystem m_blinkFx;
			public ParticleSystem m_rightCircleFx;
			public ParticleSystem m_leftCircleFx;
			[Space]
			public float m_timeToTrailRendererIsActive = 1;
			public TrailRenderer m_trailRenderer;

			[Header("Audio")]
			public GameObject m_spellSound;
		}

		public Block m_Block = new Block();
		[Serializable] public class Block {
            [HideInInspector] public bool m_inIceBlock = false;

			public ParticleSystem m_block;
			[HideInInspector] public bool m_canSwitch = true;
			[HideInInspector] public bool m_startCooldown = false;
			public float m_cooldown = 5;
			[HideInInspector] public float m_actualCooldown = 0;
			public float m_timeToBeInIceBlock = 2;
			[HideInInspector] public float m_actualIceBlockTimer = 0;

			[Header("UI")]
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;

			[Header("Audio")]
			public AudioSource m_spellSound;
		}

		public IceNova m_iceNova = new IceNova();
		[Serializable] public class IceNova {
			[Header("Spell prefab")]
			// public GameObject m_nova;
			public Transform m_root;

			[Header("Cooldown")]
			public float m_cooldown = 5;
			[HideInInspector] public bool m_canSwitch = true;
			[HideInInspector] public bool m_startCooldown = false;
			[HideInInspector] public float m_actualCooldown = 0;

			[Header("Timers")]
			public float m_waitTimeToThrowSpell = 0;
			public float m_waitTimeToExitState = 0;

            [Header("Ice Nova Freeze")]
            public float m_timeFreezed = 3f;

            [Header("UI")]
			public SpellUI m_uI = new SpellUI();

			[Header("Audio")]
			public GameObject m_spellSound;
		}
		public IceBuff m_iceBuff = new IceBuff();
		[Serializable] public class IceBuff {
			[Header("Spell prefab")]
			// public GameObject m_buff;
			public Transform m_root;

			[Header("Cooldown")]
			public float m_cooldown = 5;
			[HideInInspector] public bool m_canSwitch = true;
			[HideInInspector] public bool m_startCooldown = false;
			[HideInInspector] public float m_actualCooldown = 0;

			[Header("Timers")]
			public float m_waitTimeToThrowSpell = 0;
			public float m_waitTimeToExitState = 0;
			public bool m_stopMovingAfterSpell = true;

			[Header("UI")]
			public SpellUI m_uI = new SpellUI();

			[Header("Audio")]
			public GameObject m_spellSound;

			[HideInInspector] public GameObject m_actualBuff;
		}
		public FireBalls m_fireBalls = new FireBalls();
		[Serializable] public class FireBalls {
			[Header("Spell prefab")]
			public GameObject m_balls;
			public Transform m_root;

			[Header("Cooldown")]
			public float m_cooldown = 5;
			[HideInInspector] public bool m_canSwitch = true;
			[HideInInspector] public bool m_startCooldown = false;
			[HideInInspector] public float m_actualCooldown = 0;

			[Header("Timers")]
			public float m_waitTimeToThrowSpell = 0;
			public float m_waitTimeToExitState = 0;

			[Header("UI")]
			public SpellUI m_uI = new SpellUI();

			[Header("Audio")]
			public GameObject m_spellSound;
		}
		public FireTrail m_fireTrail = new FireTrail();
		[Serializable] public class FireTrail {
			[Header("Spell prefab")]
			// public GameObject m_trail;
			public Transform m_root;

			[Header("Cooldown")]
			public float m_cooldown = 5;
			[HideInInspector] public bool m_canSwitch = true;
			[HideInInspector] public bool m_startCooldown = false;
			[HideInInspector] public float m_actualCooldown = 0;

			[Header("Timers")]
			public float m_waitTimeToThrowSpell = 0;
			public float m_waitTimeToExitState = 0;

			[Header("Fire Trail Burned")]
            public float m_fireTrailTick = 0.5f;
            public int m_fireTrailTickDamage = 5;

			[Header("UI")]
			public SpellUI m_uI = new SpellUI();

			[Header("Audio")]
			public GameObject m_spellSound;
		}

		public ArcaneProjectiles m_arcaneProjectiles = new ArcaneProjectiles();
		[Serializable] public class ArcaneProjectiles {
			[Header("Spell prefab")]
			// public GameObject m_firstProjectile;
			// public GameObject m_secondProjectile;
			// public GameObject m_thirdProjectile;
			public Transform m_root;

			[Header("Cooldown")]
			public float m_cooldown = 5;
			[HideInInspector] public bool m_canSwitch = true;
			[HideInInspector] public bool m_startCooldown = false;
			[HideInInspector] public float m_actualCooldown = 0;

			[Header("Timers")]
			public float m_waitTimeToThrowFirstSpell = 0;
			public float m_waitTimeToThrowSecondSpell = 0;
			public float m_waitTimeToThrowThirdSpell = 0;
			public float m_waitTimeToExitState = 0;

			[Header("UI")]
			public SpellUI m_uI = new SpellUI();

			[Header("Audio")]
			public GameObject m_firstSpellSound;
			public GameObject m_secondSpellSound;
			public GameObject m_thirdSpellSound;

			[Header("Shake camera")]
			public ShakeCamera m_ShakeCamera = new ShakeCamera();
			[Serializable] public class ShakeCamera {
				[Header("First Shake")]
				public FirstShake m_firstShake = new FirstShake();
				[Serializable] public class FirstShake {
					public bool m_useShakeCam = true;
					public float m_magnitudeShake = 4f;
					public float m_roughnessShake = 4f;
					public float m_fadeInTimeShake = 0.1f;
					public float m_fadeOutTimeShake = 0.1f;
				}
				[Header("Second Shake")]
				public SecoundShake m_secoundShake = new SecoundShake();
				[Serializable] public class SecoundShake {
				public bool m_useShakeCam = true;
				public float m_magnitudeShake = 4f;
				public float m_roughnessShake = 4f;
				public float m_fadeInTimeShake = 0.1f;
				public float m_fadeOutTimeShake = 0.1f;
				}
				[Header("Third Shake")]
				public ThirdShake m_thirdShake = new ThirdShake();
				[Serializable] public class ThirdShake {
				public bool m_useShakeCam = true;
				public float m_magnitudeShake = 4f;
				public float m_roughnessShake = 4f;
				public float m_fadeInTimeShake = 0.1f;
				public float m_fadeOutTimeShake = 0.1f;
				}
			}
		}
		public ArcaneExplosion m_arcaneExplosion = new ArcaneExplosion();
		[Serializable] public class ArcaneExplosion {
			[Header("Spell prefab")]
			// public GameObject m_projectile;
			public Transform m_root;
            [Header("Spell multiplicateur")]
            [Range(0,100)]
            public float m_blastMultiplicateur = 50f;

            [Header("Cooldown")]
			public float m_cooldown = 5;
			[HideInInspector] public bool m_canSwitch = true;
			[HideInInspector] public bool m_startCooldown = false;
			[HideInInspector] public float m_actualCooldown = 0;

			[Header("Timers")]
			public float m_waitTimeToThrowSpell = 0;
			public float m_waitTimeToExitState = 0;

			[Header("UI")]
			public SpellUI m_uI = new SpellUI();

			[Header("Audio")]
			public GameObject m_spellSound;

			[Header("Shake camera")]
			public bool m_useShakeCam = true;
			public float m_magnitudeShake = 4f;
			public float m_roughnessShake = 4f;
			public float m_fadeInTimeShake = 0.1f;
			public float m_fadeOutTimeShake = 0.1f;
		}

		[Serializable] public class SpellUI {
			[Header("First UI")]
			public RectTransform m_firstUiParent;
			public TextMeshProUGUI m_firsText;
			public Image m_firstSpellImage;
			public Image m_firstCooldownImage;

			public bool m_firstUiIsInAnimation = false;

			[Header("Second UI")]
			public RectTransform m_secondUiParent;
			public TextMeshProUGUI m_secondText;
			public Image m_secondSpellImage;
			public Image m_secondCooldownImage;

			public bool m_secondUiIsInAnimation = false;
		}

		[Header("UI")]
		public UI m_uI = new UI();
		[Serializable] public class UI {
			[Header("Width/Height")]
			public Vector2 m_minScale = new Vector2(44, 44);
			public Vector2 m_maxScale = new Vector2(58, 58);

			[Header("Text size")]
			public float m_minSize = 20;
			public float m_maxSize = 30;

			[Header("UI Animations")]
			public UIAnimations m_uIAnimations = new UIAnimations();
			[Serializable] public class UIAnimations {
				public float m_timeToFinish = 0.25f;
				public AnimationCurve m_curveAnim;

				public RectTransform m_leftLeftPosition;
				public RectTransform m_leftRightPosition;

				public RectTransform m_rightLeftPosition;
				public RectTransform m_rightRightPosition;
			}

			[Header("CastBar")]
			public CastBar m_castBar = new CastBar();
			[Serializable] public class CastBar {
				public GameObject m_castBarCanvas;
				public Image m_spellImg;
				public TextMeshProUGUI m_spellName;
				public TextMeshProUGUI m_stopChanelled;
				public TextMeshProUGUI m_currentTimerTxt;
				public TextMeshProUGUI m_totalTimerTxt;
				public UIProgressBar m_progressBar;
				[HideInInspector] public float m_currentSpellTimer = 0;
				[HideInInspector] public float m_totalSpellTimer;

				[Header("Spell")]
				public Spell m_arcaneProjectiles;
				public Spell m_chronoBlock;
				[Serializable] public class Spell {
					public Sprite m_sprite;
					public String m_name;
					public String m_inputToStopChanelled;
					[HideInInspector] public bool m_startDecreaseTimer = false;
					[HideInInspector] public bool m_decreaseTimerStarted = false;
					[HideInInspector] public bool m_decreaseTimer = false;
					[HideInInspector] public bool m_decreaseTimerEnded = false;
				}
			}
		}
	}

	[Header("Elements")]
    [Range(0,100)]
    public float m_percentMultiplicateur = 25f;
    public int m_maxArcanMarkCount = 10;
    public Debuffs m_debuffs = new Debuffs();
    [Serializable]
    public class Debuffs
    {
        [Header("Fire Ticks")]
		public FireTicks m_fireTicks = new FireTicks();
		[Serializable]
		public class FireTicks
		{
			public float m_timerTickDamage = 0.25f;
			public int m_fireTickDamage = 5;
			public int m_fireExplosionDamage = 100;
        }
		[Header("Ice Slow")]
		public IceSlow m_IceSlow = new IceSlow();
		[Serializable]
		public class IceSlow
		{
			[Range(0,100)]
			public int m_iceSlow = 25;
            public float m_frozenTime;
        }
    }

	[Header("Move speed")]
	public MoveSpeed m_moveSpeed = new MoveSpeed();
	[Serializable] public class MoveSpeed {
		public float m_normalspeed = 10;
		public float m_fastSpeed = 15;
		[Space]
		public float m_timeToKeepFastSpeed = 2;
		public TrailRenderer[] m_trailRenderers = new TrailRenderer[3];

		[HideInInspector] public bool m_playerInBuff = false;
		[HideInInspector] public float m_actualTimer = 0;
		[HideInInspector] public bool m_iceBuffIsCast = false;
	}

	[Header("Cinematic animation")]
	public Cinematic m_cinematic = new Cinematic();
	[Serializable] public class Cinematic {
		[Header("Strip")]
		public RectTransform m_topStrip;
		public RectTransform m_botStrip;

		[Header("Height")]
		public float m_startHeight = 0;
		public float m_endHieght = 155;

		[Header("Animation")]
		public float m_timeToEndAnim = 3;
		public AnimationCurve m_curve;

		[HideInInspector] public bool m_isInCinematicState = false;
		[HideInInspector] public float m_timeToBeInCinematic = 0;

	}

    #endregion Public [Serializable] Variables

    [Space]
	public GameObject m_playerMesh;
	public GameObject m_jainaMesh;
	public GameObject m_clickOnGroundFx;
	public LayerMask m_groundLayer;
	public LayerMask m_rotatePlayerLayer;

	[HideInInspector] public bool m_canThrowSpell = true;
	NavMeshAgent m_agent;
	bool m_canAutoAttack = true;
	PlayerUiAnimationCorout m_playerUiCorout;
	GameObject m_lastAutoAttackSound;
	ClickOnGround m_actualClickOnGroundFx;
	ClickOnGround m_actualClickOnGroundStartDestroyedFx;
	
#region Input Buttons

	[HideInInspector] public bool m_leftMouseDownClick;
	[HideInInspector] public bool m_leftMouseClick;
	[HideInInspector] public bool m_rightMouseClickDown;
	[HideInInspector] public bool m_rightMouseClick;
	[HideInInspector] public bool m_rightMouseClickUp;
	[HideInInspector] public bool m_blinkButton;
	[HideInInspector] public bool m_iceBlockButton;
	[HideInInspector] public bool m_leftSpellButton;
	[HideInInspector] public bool m_rightSpellButton;

#endregion Input Buttons

#region UI Positions
	Vector3 m_leftLeftPos;
	Vector3 m_leftMiddlePos;
	Vector3 m_leftRightPos;
	Vector3 m_rightLeftPos;
	Vector3 m_rightMiddlePos;
	Vector3 m_rightRightPos;
#endregion UI Positions

#region Encapsuled
	Vector3 m_playerTargetPosition;
    public Vector3 PlayerTargetPosition
    {
        get
        {
            return m_playerTargetPosition;
        }

        set
        {
            m_playerTargetPosition = value;
        }
    }

	Animator m_jainaAnimator;
	public Animator JainaAnimator{
        get{
            return m_jainaAnimator;
        }
    }

	bool m_playerIsDead = false;
	public bool PlayerIsDead {
        get{
            return m_playerIsDead;
        }
		set{
            m_playerIsDead = value;
        }
    }

    SaveManager m_saveManager;
    public SaveManager SaveManager {
        get{
            return m_saveManager;
        }
        set{
            m_saveManager = value;
        }
    }

	ObjectPooler m_objectPooler;
	public ObjectPooler ObjectPooler {
        get{
            return m_objectPooler;
        }
        set{
            m_objectPooler = value;
        }
    }

	CameraManager m_cameraManager;
	public CameraManager CameraManager {
        get{
            return m_cameraManager;
        }
        set{
            m_cameraManager = value;
        }
    }

	bool m_isMoving;
	public bool IsMoving {
        get{
            return m_isMoving;
        }
        set{
            m_isMoving = value;
        }
    }
#endregion Encapsuled

    void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of PlayerManager");
		}
		m_sM.AddStates(new List<IState> {
			new NoThrowSpellState(this),		//  0 = No Throw Spella
			new BlinkState(this),		 		//  1 - Blink	
			new IceBlockState(this), 			//  2 - Ice Block
			new NovaState(this),				//  3 - Nova
			new FireBallsState(this),			//  4 - Fire Balls
			new ArcaneProjectilesState(this), 	//  5 - Arcane Projectiles
			new IceBuffState(this),				//  6 - Ice Buff
			new FireTrailState(this),			//  7 - Fire Trail
			new ArcaneExplosionState(this),		//  8 - Arcane Explosion
			new PlayerDieState(this),			//  9 - Die
			new PlayerCinematicState(this),		// 10 - Cinematic
		});
		string[] playerStateNames = System.Enum.GetNames (typeof(PlayerState));
		if(m_sM.States.Count != playerStateNames.Length){
			Debug.LogError("You need to have the same number of State in PlayerManager and PlayerStateEnum");
		}

		m_agent = GetComponent<NavMeshAgent>();
	}

	void Start(){
		m_playerUiCorout = GetComponent<PlayerUiAnimationCorout>();


		SetUIElements();

		m_jainaAnimator = m_jainaMesh.GetComponent<Animator>();
		m_saveManager = SaveManager.Instance;
		m_objectPooler = ObjectPooler.Instance;
		m_cameraManager = CameraManager.Instance;
		InitializeStartAutoAttackCooldown();
		SetPlayerSpeed(m_moveSpeed.m_normalspeed);
	}
	
	void OnEnable(){
		ChangeState(m_playerDebug.m_playerStartState);
	}

	void Update(){
		m_sM.Update();
		UpdateInputButtons();
		RaycastToMovePlayer();
		DecreaseCooldown();
		DecreaseChanneledSpell();
		UpdatePlayerSpeed();

		if(Input.GetKeyDown(KeyCode.G)){
			// SwitchPlayerToCinematicState(5);
			StartCoroutine(CinematicStringCorout(0, 100));
		}
	}

	void FixedUpdate(){
		m_sM.FixedUpdate();

        if(m_sM.CurrentStateIndex != ((int)PlayerState.PlayerCinematicState)){
			if(m_agent.velocity != Vector3.zero ){
				m_isMoving = true;
				m_jainaAnimator.SetBool("isMoving", m_isMoving);
			}else{
				m_isMoving = false;
				m_jainaAnimator.SetBool("isMoving", m_isMoving);
			}
		}
	}

	void LateUpdate(){
		MoveAnimation();
	}

	void MoveAnimation(){
		// ------------------------------
		// Try to have direction Vector
		// ------------------------------
		// ----- First part -----
		Vector3 direction;
		direction = transform.InverseTransformDirection(Vector3.forward);
		Vector2 inputDirection;
		inputDirection = new Vector2(-direction.x, direction.z);

        if(inputDirection.x > 0.3f){
			inputDirection.x = 1;
		}else if(inputDirection.x < -0.3f){
			inputDirection.x = -1;
		}else if(inputDirection.x < 0.3f && inputDirection.x > -0.3f){
			inputDirection.x = 0;
		}
        
		if(inputDirection.y > 0.3f){
			inputDirection.y = 1;
		}else if(inputDirection.y < -0.3f){
			inputDirection.y = -1;
		}else if(inputDirection.y < 0.3f && inputDirection.y > -0.3f){
			inputDirection.y = 0;
		}

		// Debug.Log("direction = " + inputDirection);

		// ----- Second part -----
		Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
 
        if (moveDirection.magnitude > 1.0f)
        {
            moveDirection = moveDirection.normalized;
        }
 
        moveDirection = m_playerMesh.transform.InverseTransformDirection(moveDirection);
 
        // Debug.Log("moveDirection = " + moveDirection);

        // m_jainaAnimator.SetFloat("moveX", moveDirection.x, 0.05f, Time.deltaTime);
        // m_jainaAnimator.SetFloat("moveY", moveDirection.z, 0.05f, Time.deltaTime);
        m_jainaAnimator.SetFloat("moveX", moveDirection.x, Time.fixedDeltaTime, Time.deltaTime);
        m_jainaAnimator.SetFloat("moveY", moveDirection.z, Time.fixedDeltaTime, Time.deltaTime);
	}

	public void ChangeState(PlayerState newPlayerState){
		m_sM.ChangeState((int)newPlayerState);
	}

	void UpdateInputButtons(){
		m_leftMouseClick = Input.GetButton("LeftClick");
		m_leftMouseDownClick = Input.GetButtonDown("LeftClick");

		m_rightMouseClickDown = Input.GetButtonDown("RightClick");
		m_rightMouseClick = Input.GetButton("RightClick");
		m_rightMouseClickUp = Input.GetButtonUp("RightClick");

		m_blinkButton = Input.GetButtonDown("Blink");
		m_iceBlockButton = Input.GetButtonDown("IceBlock");
		m_leftSpellButton = Input.GetButtonDown("LeftSpell");
		m_rightSpellButton = Input.GetButtonDown("RightSpell");
	}

	Vector3 m_lastDestination;
	void RaycastToMovePlayer(){
		if(m_rightMouseClickDown){
			RaycastHit hit;
			hit = GroundRaycast();
			PlayerTargetPosition = hit.point;
			if(m_actualClickOnGroundStartDestroyedFx != null){
				m_actualClickOnGroundStartDestroyedFx.DestroyFx();
			}
			m_actualClickOnGroundFx = Instantiate(m_clickOnGroundFx, hit.point, m_clickOnGroundFx.transform.rotation).GetComponent<ClickOnGround>();

			if(m_agent.destination != m_lastDestination){
				m_actualClickOnGroundFx.transform.position = m_agent.destination;
				// m_actualClickOnGroundFx.transform.position = hit.point;
			}else{
				m_actualClickOnGroundFx.gameObject.SetActive(false);
			}

			// if(m_agent.destination != transform.position){
			// 	m_actualClickOnGroundFx.transform.position = m_agent.destination;
			// }
		}
		if(m_rightMouseClick){
			RaycastHit hit;
			hit = GroundRaycast();
			PlayerTargetPosition = hit.point;
			if(!m_actualClickOnGroundFx.gameObject.activeSelf){
				m_actualClickOnGroundFx.gameObject.SetActive(true);
			}
			if(m_agent.destination != transform.position){
				m_actualClickOnGroundFx.transform.position = m_agent.destination;
				// m_actualClickOnGroundFx.transform.position = hit.point;
			}
			m_lastDestination = m_agent.destination;
		}
		if(m_rightMouseClickUp){
			if(m_actualClickOnGroundFx != null){
				m_actualClickOnGroundFx.StartBeDestroyed();
				m_actualClickOnGroundStartDestroyedFx = m_actualClickOnGroundFx;
				m_actualClickOnGroundFx = null;
			}
		}
	}
	RaycastHit GroundRaycast(){
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, m_groundLayer);
		return hit;
	}
	public void MovePlayer(){
		if(PlayerTargetPosition != Vector3.zero){
			m_agent.SetDestination(PlayerTargetPosition);
		}
	}
	public void StopPlayerMovement(){
		m_agent.ResetPath();
	}
	public void RotatePlayer(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit floorHit;
        if(Physics.Raycast (ray, out floorHit, Mathf.Infinity, m_rotatePlayerLayer)){
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            m_playerMesh.transform.rotation = newRotation;
        }
	}
	public void SetPlayerSpeed(float newSpeed){
		// Debug.Log("new speed = " + newSpeed);
		m_agent.speed = newSpeed;
	}
	public void SetPlayerSpeedWithAccelerationDeceleration(float newSpeed, float timeToChangeSpeed){
		
	}
	void UpdatePlayerSpeed(){
		if(m_moveSpeed.m_playerInBuff){
			if(m_moveSpeed.m_actualTimer != m_moveSpeed.m_timeToKeepFastSpeed){
				m_moveSpeed.m_actualTimer = m_moveSpeed.m_timeToKeepFastSpeed;
				EnableIceBuffTrailRenderers(true);
			}
		}else{
			if(m_moveSpeed.m_iceBuffIsCast){
				m_moveSpeed.m_actualTimer -= Time.deltaTime;
				if(m_moveSpeed.m_actualTimer <= 0){
					SetPlayerSpeed(m_moveSpeed.m_normalspeed);
					m_moveSpeed.m_iceBuffIsCast = false;
					EnableIceBuffTrailRenderers(false);
				}
			}
		}
	}
	void EnableIceBuffTrailRenderers(bool activate){
		for (int i = 0, l = m_moveSpeed.m_trailRenderers.Length; i < l; ++i){
			m_moveSpeed.m_trailRenderers[i].enabled = activate;
		}
	}

	public void ChangePower(bool rightSpell){
		switch(m_currentElement){
			case ElementType.Arcane:
				m_currentElement = rightSpell == true ? m_currentElement = ElementType.Fire : m_currentElement = ElementType.Ice;
			break;
			case ElementType.Ice:
				m_currentElement = rightSpell == true ? m_currentElement = ElementType.Arcane : m_currentElement = ElementType.Fire;
			break;
			case ElementType.Fire:
				m_currentElement = rightSpell == true ? m_currentElement = ElementType.Ice : m_currentElement = ElementType.Arcane;
			break;
		}
		ChangeUIElements(rightSpell);
	}

	void SetUIElements(){
		m_leftLeftPos = m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition;
		m_leftMiddlePos = m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition;
		m_leftRightPos = m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition;

		m_rightLeftPos = m_playerDebug.m_useSymetricalHudSpellAnim ? m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition : m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition;
		m_rightMiddlePos = m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition;
		m_rightRightPos = m_playerDebug.m_useSymetricalHudSpellAnim ? m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition : m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition;

		m_playerUiCorout.ChangeSpellAlpha(m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0);
		m_playerUiCorout.ChangeSpellAlpha(m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0);
		m_playerUiCorout.ChangeSpellAlpha(m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0);
		m_playerUiCorout.ChangeSpellAlpha(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0);
		m_playerUiCorout.ChangeSpellAlpha(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0);
		m_playerUiCorout.ChangeSpellAlpha(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0);

		// Si le joueur ne commence pas avec le même élément que dans l'HUD
		if(m_currentElement != ElementType.Ice){
			ChangeUIElements();
		}
	}

	void ChangeUIElements(bool rightSpell = true){
		m_playerUiCorout.On_StopAllCoroutines();
		// Debug.LogError("StopAllCoroutines");
		switch(m_currentElement){
			case ElementType.Arcane:
				// --------------------------
				// ---------- LEFT ----------
				// --------------------------

				// -------------------------
				// ---------- ICE ----------
				if(!rightSpell){
					// First
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiIsInAnimation, m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceNova.m_uI.m_firstSpellImage, m_powers.m_iceNova.m_uI.m_firstCooldownImage, m_powers.m_iceNova.m_uI.m_firsText, 1, 0));
					
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_iceNova.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition;
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0, 1));

					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_secondUiIsInAnimation, m_powers.m_iceNova.m_uI.m_secondUiParent, m_powers.m_iceNova.m_uI.m_secondUiParent.localPosition, m_leftLeftPos, 
									m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstSpellImage, m_powers.m_iceNova.m_uI.m_firstCooldownImage, m_powers.m_iceNova.m_uI.m_firsText, 1, 
									m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0));
					// -------------------------
				}else{
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiIsInAnimation, m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_leftLeftPos));

					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}

				m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_leftMiddlePos));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// --------------------------
				// ---------- FIRE ----------
				if(rightSpell){
					// First
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiIsInAnimation, m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireBalls.m_uI.m_firstSpellImage, m_powers.m_fireBalls.m_uI.m_firstCooldownImage, m_powers.m_fireBalls.m_uI.m_firsText, 1, 0));
					
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_fireBalls.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition;
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0, 1));

					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_secondUiIsInAnimation, m_powers.m_fireBalls.m_uI.m_secondUiParent, m_powers.m_fireBalls.m_uI.m_secondUiParent.localPosition, m_leftRightPos, 
									m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstSpellImage, m_powers.m_fireBalls.m_uI.m_firstCooldownImage, m_powers.m_fireBalls.m_uI.m_firsText, 1, 
									m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0));
					// --------------------------
				}else{
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiIsInAnimation, m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_leftRightPos));
				
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}


				// ---------------------------
				// ---------- RIGHT ----------
				// ---------------------------

				// --------------------------
				// ---------- FIRE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 0));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0, 1));
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_secondUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_secondUiParent, m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
										m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 
										m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0));
						// --------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}			
				}else{
					if(rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 0));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0, 1));
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_secondUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_secondUiParent, m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
										m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 
										m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0));
						// --------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}
				}
				
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightMiddlePos));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));
				
				// -------------------------
				// ---------- ICE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(!rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 0));
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0, 1));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_secondUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_secondUiParent, m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
										m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 
										m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0));
						// -------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}else{
					if(!rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 0));
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0, 1));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_secondUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_secondUiParent, m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
										m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 
										m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0));
						// -------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}				
			break;
			case ElementType.Ice:
				// --------------------------
				// ---------- LEFT ----------
				// --------------------------

				// --------------------------
				// ---------- FIRE ----------
				if(!rightSpell){
					// First
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiIsInAnimation, m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireBalls.m_uI.m_firstSpellImage, m_powers.m_fireBalls.m_uI.m_firstCooldownImage, m_powers.m_fireBalls.m_uI.m_firsText, 1, 0));
					
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_fireBalls.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition;
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0, 1));

					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_secondUiIsInAnimation, m_powers.m_fireBalls.m_uI.m_secondUiParent, m_powers.m_fireBalls.m_uI.m_secondUiParent.localPosition, m_leftLeftPos, 
									m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstSpellImage, m_powers.m_fireBalls.m_uI.m_firstCooldownImage, m_powers.m_fireBalls.m_uI.m_firsText, 1, 
									m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0));
					// -------------------------
				}else{
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiIsInAnimation, m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_leftLeftPos));

					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}
				
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiIsInAnimation, m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_leftMiddlePos));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// ----------------------------
				// ---------- ARCANE ----------
				if(rightSpell){
					// First
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneProjectiles.m_uI.m_firstSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_firsText, 1, 0));
					
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition;
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0, 1));

					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_secondUiIsInAnimation, m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent, m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent.localPosition, m_leftRightPos, 
									m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_firsText, 1, 
									m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0));
					// ----------------------------
				}else{
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_leftRightPos));
				
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}


				// ---------------------------
				// ---------- RIGHT ----------
				// ---------------------------

				// ----------------------------
				// ---------- ARCANE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 0));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0, 1));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_secondUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
										m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 
										m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0));
						// ----------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}else{
					if(rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 0));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0, 1));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_secondUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
										m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 
										m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0));
						// ----------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}

				m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightMiddlePos));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// --------------------------
				// ---------- FIRE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(!rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 0));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0, 1));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_secondUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_secondUiParent, m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
										m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 
										m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0));
						// --------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}else{
					if(!rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 0));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0, 1));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_secondUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_secondUiParent, m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
										m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 
										m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0));
						// --------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}				
				
			break;
			case ElementType.Fire:
				// --------------------------
				// ---------- LEFT ----------
				// --------------------------

				// ----------------------------
				// ---------- ARCANE ----------
				if(!rightSpell){
					// First
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneProjectiles.m_uI.m_firstSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_firsText, 1, 0));
					
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition;
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0, 1));

					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_secondUiIsInAnimation, m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent, m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent.localPosition, m_leftLeftPos, 
									m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_firsText, 1, 
									m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0));
					// -------------------------
				}else{
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_leftLeftPos));
				
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}

				m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiIsInAnimation, m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_leftMiddlePos));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// -------------------------
				// ---------- ICE ----------
				if(rightSpell){
					// First
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiIsInAnimation, m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceNova.m_uI.m_firstSpellImage, m_powers.m_iceNova.m_uI.m_firstCooldownImage, m_powers.m_iceNova.m_uI.m_firsText, 1, 0));
					
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_iceNova.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition;
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0, 1));

					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_secondUiIsInAnimation, m_powers.m_iceNova.m_uI.m_secondUiParent, m_powers.m_iceNova.m_uI.m_secondUiParent.localPosition, m_leftRightPos, 
									m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstSpellImage, m_powers.m_iceNova.m_uI.m_firstCooldownImage, m_powers.m_iceNova.m_uI.m_firsText, 1, 
									m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0));
					// -------------------------
				}else{
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiIsInAnimation, m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_leftRightPos));
					
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}


				// ---------------------------
				// ---------- RIGHT ----------
				// ---------------------------

				// -------------------------
				// ---------- ICE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 0));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0, 1));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_secondUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_secondUiParent, m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
										m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 
										m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0));
						// -------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}else{
					if(rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 0));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0, 1));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_secondUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_secondUiParent, m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
										m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 
										m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0));
						// -------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiIsInAnimation, m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}

				m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiIsInAnimation, m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightMiddlePos));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// ----------------------------
				// ---------- ARCANE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(!rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 0));
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0, 1));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_secondUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
										m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 
										m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0));
						// ----------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}				
				}else{
					if(!rightSpell){
						// First
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 0));
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0, 1));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_secondUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
										m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 
										m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0));
						// ----------------------------
					}else{
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiIsInAnimation, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						m_playerUiCorout.StartCoroutine(m_playerUiCorout.ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}
			break;
		}
	}

	public void DecreaseChanneledSpell(){
		// Chrono block
		if(m_powers.m_uI.m_castBar.m_chronoBlock.m_startDecreaseTimer){
			if(m_powers.m_uI.m_castBar.m_chronoBlock.m_decreaseTimer){
				if(!m_powers.m_uI.m_castBar.m_chronoBlock.m_decreaseTimerStarted){
					m_powers.m_uI.m_castBar.m_chronoBlock.m_decreaseTimerStarted = true;
					m_powers.m_uI.m_castBar.m_spellImg.sprite = m_powers.m_uI.m_castBar.m_chronoBlock.m_sprite;
					m_powers.m_uI.m_castBar.m_spellName.text = m_powers.m_uI.m_castBar.m_chronoBlock.m_name;
					m_powers.m_uI.m_castBar.m_stopChanelled.text = m_powers.m_uI.m_castBar.m_chronoBlock.m_inputToStopChanelled;
					m_powers.m_uI.m_castBar.m_totalSpellTimer = m_powers.m_Block.m_timeToBeInIceBlock;
					m_powers.m_uI.m_castBar.m_currentSpellTimer = m_powers.m_uI.m_castBar.m_totalSpellTimer;
					m_powers.m_uI.m_castBar.m_currentTimerTxt.text = m_powers.m_uI.m_castBar.m_currentSpellTimer.ToString();
					m_powers.m_uI.m_castBar.m_totalTimerTxt.text = m_powers.m_uI.m_castBar.m_totalSpellTimer.ToString();
					m_powers.m_uI.m_castBar.m_castBarCanvas.SetActive(true);
				}
				if(m_powers.m_uI.m_castBar.m_currentSpellTimer >= 0){
					m_powers.m_uI.m_castBar.m_currentSpellTimer -= Time.deltaTime;
					m_powers.m_uI.m_castBar.m_currentTimerTxt.text = m_powers.m_uI.m_castBar.m_currentSpellTimer.ToString("F1");
					float newValue = Mathf.InverseLerp(0, m_powers.m_uI.m_castBar.m_totalSpellTimer, m_powers.m_uI.m_castBar.m_currentSpellTimer);
					m_powers.m_uI.m_castBar.m_progressBar.fillAmount = newValue;
				}else{
					StopDecreaseBlockTimer();
				}
			}else{
				if(!m_powers.m_uI.m_castBar.m_chronoBlock.m_decreaseTimerEnded){
					m_powers.m_uI.m_castBar.m_chronoBlock.m_decreaseTimerStarted = true;
					m_powers.m_uI.m_castBar.m_castBarCanvas.SetActive(false);
					m_powers.m_uI.m_castBar.m_chronoBlock.m_startDecreaseTimer = false;
				}
			}
		}
		// Arcane projectiles
		if(m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_startDecreaseTimer){
			if(m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_decreaseTimer){
				if(!m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_decreaseTimerStarted){
					m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_decreaseTimerStarted = true;
					m_powers.m_uI.m_castBar.m_spellImg.sprite = m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_sprite;
					m_powers.m_uI.m_castBar.m_spellName.text = m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_name;
					m_powers.m_uI.m_castBar.m_stopChanelled.text = m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_inputToStopChanelled;
					m_powers.m_uI.m_castBar.m_totalSpellTimer = m_powers.m_arcaneProjectiles.m_waitTimeToThrowFirstSpell + m_powers.m_arcaneProjectiles.m_waitTimeToThrowSecondSpell + m_powers.m_arcaneProjectiles.m_waitTimeToThrowThirdSpell + m_powers.m_arcaneProjectiles.m_waitTimeToExitState;
					m_powers.m_uI.m_castBar.m_currentSpellTimer = m_powers.m_uI.m_castBar.m_totalSpellTimer;
					m_powers.m_uI.m_castBar.m_currentTimerTxt.text = m_powers.m_uI.m_castBar.m_currentSpellTimer.ToString();
					m_powers.m_uI.m_castBar.m_totalTimerTxt.text = m_powers.m_uI.m_castBar.m_totalSpellTimer.ToString();
					m_powers.m_uI.m_castBar.m_castBarCanvas.SetActive(true);
				}
				if(m_powers.m_uI.m_castBar.m_currentSpellTimer > 0){
					m_powers.m_uI.m_castBar.m_currentSpellTimer -= Time.deltaTime;
					m_powers.m_uI.m_castBar.m_currentTimerTxt.text = m_powers.m_uI.m_castBar.m_currentSpellTimer.ToString("F1");
					float newValue = Mathf.InverseLerp(0, m_powers.m_uI.m_castBar.m_totalSpellTimer, m_powers.m_uI.m_castBar.m_currentSpellTimer);
					m_powers.m_uI.m_castBar.m_progressBar.fillAmount = newValue;
				}else{
					StopDecreaseArcaneProjectilesTimer();
				}
			}else{
				if(!m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_decreaseTimerEnded){
					m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_decreaseTimerStarted = true;
					m_powers.m_uI.m_castBar.m_castBarCanvas.SetActive(false);
					m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_startDecreaseTimer = false;
				}
			}
		}
	}
	public void StartDecreaseBlockTimer(){
		m_powers.m_uI.m_castBar.m_chronoBlock.m_startDecreaseTimer = true;
		m_powers.m_uI.m_castBar.m_chronoBlock.m_decreaseTimer = true;
		m_powers.m_uI.m_castBar.m_chronoBlock.m_decreaseTimerStarted = false;
		m_powers.m_uI.m_castBar.m_chronoBlock.m_decreaseTimerEnded = false;
	}
	public void StopDecreaseBlockTimer(){
		m_powers.m_uI.m_castBar.m_chronoBlock.m_decreaseTimer = false;
	}
	public void StartDecreaseArcaneProjectilesTimer(){
		m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_startDecreaseTimer = true;
		m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_decreaseTimer = true;
		m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_decreaseTimerStarted = false;
		m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_decreaseTimerEnded = false;
	}
	public void StopDecreaseArcaneProjectilesTimer(){
		m_powers.m_uI.m_castBar.m_arcaneProjectiles.m_decreaseTimer = false;
	}

	public void DecreaseCooldown(){
		// Blink
		if(m_powers.m_blink.m_startCooldown){
			m_powers.m_blink.m_cooldownImage.gameObject.SetActive(true);
			m_powers.m_blink.m_text.gameObject.SetActive(true);
			if(m_powers.m_blink.m_actualCooldown > 0){
				m_powers.m_blink.m_actualCooldown -= Time.deltaTime;
				m_powers.m_blink.m_canSwitch = false;

				m_powers.m_blink.m_text.text = m_powers.m_blink.m_actualCooldown.ToString("F1");
				// m_powers.m_blink.m_cooldownImage.fillAmount -= 1 / m_powers.m_blink.m_cooldown * Time.deltaTime; // Avant que Paul trouve le bug d'affichage du cooldown
				m_powers.m_blink.m_cooldownImage.fillAmount = Mathf.InverseLerp(0, m_powers.m_blink.m_cooldown, m_powers.m_blink.m_actualCooldown); // Tentative de régler le bug
			}else{
				m_powers.m_blink.m_actualCooldown = m_powers.m_blink.m_cooldown;
				m_powers.m_blink.m_startCooldown = false;
				m_powers.m_blink.m_canSwitch = true;

				m_powers.m_blink.m_cooldownImage.gameObject.SetActive(false);
				m_powers.m_blink.m_text.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_blink.m_actualCooldown = m_powers.m_blink.m_cooldown;

			m_powers.m_blink.m_cooldownImage.fillAmount = 1;
		}
		// Block
		if(m_powers.m_Block.m_startCooldown){
			m_powers.m_Block.m_cooldownImage.gameObject.SetActive(true);
			m_powers.m_Block.m_text.gameObject.SetActive(true);
			if(m_powers.m_Block.m_actualCooldown > 0){
				m_powers.m_Block.m_actualCooldown -= Time.deltaTime;
				m_powers.m_Block.m_canSwitch = false;

				m_powers.m_Block.m_text.text = m_powers.m_Block.m_actualCooldown.ToString("F1");
				m_powers.m_Block.m_cooldownImage.fillAmount -= 1 / m_powers.m_Block.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_Block.m_actualCooldown = m_powers.m_Block.m_cooldown;
				m_powers.m_Block.m_startCooldown = false;
				m_powers.m_Block.m_canSwitch = true;

				m_powers.m_Block.m_cooldownImage.gameObject.SetActive(false);
				m_powers.m_Block.m_text.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_Block.m_actualCooldown = m_powers.m_Block.m_cooldown;

			m_powers.m_Block.m_cooldownImage.fillAmount = 1;
		}
		// Ice Nova
		if(m_powers.m_iceNova.m_startCooldown){
			m_powers.m_iceNova.m_uI.m_firstCooldownImage.gameObject.SetActive(true);
			m_powers.m_iceNova.m_uI.m_secondCooldownImage.gameObject.SetActive(true);
			m_powers.m_iceNova.m_uI.m_firsText.gameObject.SetActive(true);
			m_powers.m_iceNova.m_uI.m_secondText.gameObject.SetActive(true);
			if(m_powers.m_iceNova.m_actualCooldown > 0){
				m_powers.m_iceNova.m_actualCooldown -= Time.deltaTime;
				m_powers.m_iceNova.m_canSwitch = false;

				m_powers.m_iceNova.m_uI.m_firsText.text = m_powers.m_iceNova.m_actualCooldown.ToString("F1");
				m_powers.m_iceNova.m_uI.m_secondText.text = m_powers.m_iceNova.m_actualCooldown.ToString("F1");
				m_powers.m_iceNova.m_uI.m_firstCooldownImage.fillAmount -= 1 / m_powers.m_iceNova.m_cooldown * Time.deltaTime;
				m_powers.m_iceNova.m_uI.m_secondCooldownImage.fillAmount -= 1 / m_powers.m_iceNova.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_iceNova.m_actualCooldown = m_powers.m_iceNova.m_cooldown;
				m_powers.m_iceNova.m_startCooldown = false;
				m_powers.m_iceNova.m_canSwitch = true;

				m_powers.m_iceNova.m_uI.m_firstCooldownImage.gameObject.SetActive(false);
				m_powers.m_iceNova.m_uI.m_secondCooldownImage.gameObject.SetActive(false);
				m_powers.m_iceNova.m_uI.m_firsText.gameObject.SetActive(false);
				m_powers.m_iceNova.m_uI.m_secondText.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_iceNova.m_actualCooldown = m_powers.m_iceNova.m_cooldown;

			m_powers.m_iceNova.m_uI.m_firstCooldownImage.fillAmount = 1;
			m_powers.m_iceNova.m_uI.m_secondCooldownImage.fillAmount = 1;
		}
		// Ice Buff
		if(m_powers.m_iceBuff.m_startCooldown){
			m_powers.m_iceBuff.m_uI.m_firstCooldownImage.gameObject.SetActive(true);
			m_powers.m_iceBuff.m_uI.m_secondCooldownImage.gameObject.SetActive(true);
			m_powers.m_iceBuff.m_uI.m_firsText.gameObject.SetActive(true);
			m_powers.m_iceBuff.m_uI.m_secondText.gameObject.SetActive(true);
			if(m_powers.m_iceBuff.m_actualCooldown > 0){
				m_powers.m_iceBuff.m_actualCooldown -= Time.deltaTime;
				m_powers.m_iceBuff.m_canSwitch = false;

				m_powers.m_iceBuff.m_uI.m_firsText.text = m_powers.m_iceBuff.m_actualCooldown.ToString("F1");
				m_powers.m_iceBuff.m_uI.m_secondText.text = m_powers.m_iceBuff.m_actualCooldown.ToString("F1");
				m_powers.m_iceBuff.m_uI.m_firstCooldownImage.fillAmount -= 1 / m_powers.m_iceBuff.m_cooldown * Time.deltaTime;
				m_powers.m_iceBuff.m_uI.m_secondCooldownImage.fillAmount -= 1 / m_powers.m_iceBuff.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_iceBuff.m_actualCooldown = m_powers.m_iceBuff.m_cooldown;
				m_powers.m_iceBuff.m_startCooldown = false;
				m_powers.m_iceBuff.m_canSwitch = true;

				m_powers.m_iceBuff.m_uI.m_firstCooldownImage.gameObject.SetActive(false);
				m_powers.m_iceBuff.m_uI.m_secondCooldownImage.gameObject.SetActive(false);
				m_powers.m_iceBuff.m_uI.m_firsText.gameObject.SetActive(false);
				m_powers.m_iceBuff.m_uI.m_secondText.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_iceBuff.m_actualCooldown = m_powers.m_iceBuff.m_cooldown;

			m_powers.m_iceBuff.m_uI.m_firstCooldownImage.fillAmount = 1;
			m_powers.m_iceBuff.m_uI.m_secondCooldownImage.fillAmount = 1;
		}
		// Fire Balls
		if(m_powers.m_fireBalls.m_startCooldown){
			m_powers.m_fireBalls.m_uI.m_firstCooldownImage.gameObject.SetActive(true);
			m_powers.m_fireBalls.m_uI.m_secondCooldownImage.gameObject.SetActive(true);
			m_powers.m_fireBalls.m_uI.m_firsText.gameObject.SetActive(true);
			m_powers.m_fireBalls.m_uI.m_secondText.gameObject.SetActive(true);
			if(m_powers.m_fireBalls.m_actualCooldown > 0){
				m_powers.m_fireBalls.m_actualCooldown -= Time.deltaTime;
				m_powers.m_fireBalls.m_canSwitch = false;

				m_powers.m_fireBalls.m_uI.m_firsText.text = m_powers.m_fireBalls.m_actualCooldown.ToString("F1");
				m_powers.m_fireBalls.m_uI.m_secondText.text = m_powers.m_fireBalls.m_actualCooldown.ToString("F1");
				m_powers.m_fireBalls.m_uI.m_firstCooldownImage.fillAmount -= 1 / m_powers.m_fireBalls.m_cooldown * Time.deltaTime;
				m_powers.m_fireBalls.m_uI.m_secondCooldownImage.fillAmount -= 1 / m_powers.m_fireBalls.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_fireBalls.m_actualCooldown = m_powers.m_fireBalls.m_cooldown;
				m_powers.m_fireBalls.m_startCooldown = false;
				m_powers.m_fireBalls.m_canSwitch = true;

				m_powers.m_fireBalls.m_uI.m_firstCooldownImage.gameObject.SetActive(false);
				m_powers.m_fireBalls.m_uI.m_secondCooldownImage.gameObject.SetActive(false);
				m_powers.m_fireBalls.m_uI.m_firsText.gameObject.SetActive(false);
				m_powers.m_fireBalls.m_uI.m_secondText.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_fireBalls.m_actualCooldown = m_powers.m_fireBalls.m_cooldown;

			m_powers.m_fireBalls.m_uI.m_firstCooldownImage.fillAmount = 1;
			m_powers.m_fireBalls.m_uI.m_secondCooldownImage.fillAmount = 1;
		}
		// Fire Trail
		if(m_powers.m_fireTrail.m_startCooldown){
			m_powers.m_fireTrail.m_uI.m_firstCooldownImage.gameObject.SetActive(true);
			m_powers.m_fireTrail.m_uI.m_secondCooldownImage.gameObject.SetActive(true);
			m_powers.m_fireTrail.m_uI.m_firsText.gameObject.SetActive(true);
			m_powers.m_fireTrail.m_uI.m_secondText.gameObject.SetActive(true);
			if(m_powers.m_fireTrail.m_actualCooldown > 0){
				m_powers.m_fireTrail.m_actualCooldown -= Time.deltaTime;
				m_powers.m_fireTrail.m_canSwitch = false;

				m_powers.m_fireTrail.m_uI.m_firsText.text = m_powers.m_fireTrail.m_actualCooldown.ToString("F1");
				m_powers.m_fireTrail.m_uI.m_secondText.text = m_powers.m_fireTrail.m_actualCooldown.ToString("F1");
				m_powers.m_fireTrail.m_uI.m_firstCooldownImage.fillAmount -= 1 / m_powers.m_fireTrail.m_cooldown * Time.deltaTime;
				m_powers.m_fireTrail.m_uI.m_secondCooldownImage.fillAmount -= 1 / m_powers.m_fireTrail.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_fireTrail.m_actualCooldown = m_powers.m_fireTrail.m_cooldown;
				m_powers.m_fireTrail.m_startCooldown = false;
				m_powers.m_fireTrail.m_canSwitch = true;

				m_powers.m_fireTrail.m_uI.m_firstCooldownImage.gameObject.SetActive(false);
				m_powers.m_fireTrail.m_uI.m_secondCooldownImage.gameObject.SetActive(false);
				m_powers.m_fireTrail.m_uI.m_firsText.gameObject.SetActive(false);
				m_powers.m_fireTrail.m_uI.m_secondText.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_fireTrail.m_actualCooldown = m_powers.m_fireTrail.m_cooldown;

			m_powers.m_fireTrail.m_uI.m_firstCooldownImage.fillAmount = 1;
			m_powers.m_fireTrail.m_uI.m_secondCooldownImage.fillAmount = 1;
		}
		// Arcane Projectiles
		if(m_powers.m_arcaneProjectiles.m_startCooldown){
			m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage.gameObject.SetActive(true);
			m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage.gameObject.SetActive(true);
			m_powers.m_arcaneProjectiles.m_uI.m_firsText.gameObject.SetActive(true);
			m_powers.m_arcaneProjectiles.m_uI.m_secondText.gameObject.SetActive(true);
			if(m_powers.m_arcaneProjectiles.m_actualCooldown > 0){
				m_powers.m_arcaneProjectiles.m_actualCooldown -= Time.deltaTime;
				m_powers.m_arcaneProjectiles.m_canSwitch = false;

				m_powers.m_arcaneProjectiles.m_uI.m_firsText.text = m_powers.m_arcaneProjectiles.m_actualCooldown.ToString("F1");
				m_powers.m_arcaneProjectiles.m_uI.m_secondText.text = m_powers.m_arcaneProjectiles.m_actualCooldown.ToString("F1");
				m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage.fillAmount -= 1 / m_powers.m_arcaneProjectiles.m_cooldown * Time.deltaTime;
				m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage.fillAmount -= 1 / m_powers.m_arcaneProjectiles.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_arcaneProjectiles.m_actualCooldown = m_powers.m_arcaneProjectiles.m_cooldown;
				m_powers.m_arcaneProjectiles.m_startCooldown = false;
				m_powers.m_arcaneProjectiles.m_canSwitch = true;

				m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage.gameObject.SetActive(false);
				m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage.gameObject.SetActive(false);
				m_powers.m_arcaneProjectiles.m_uI.m_firsText.gameObject.SetActive(false);
				m_powers.m_arcaneProjectiles.m_uI.m_secondText.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_arcaneProjectiles.m_actualCooldown = m_powers.m_arcaneProjectiles.m_cooldown;

			m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage.fillAmount = 1;
			m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage.fillAmount = 1;
		}
		// Arcane Explosion
		if(m_powers.m_arcaneExplosion.m_startCooldown){
			m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage.gameObject.SetActive(true);
			m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage.gameObject.SetActive(true);
			m_powers.m_arcaneExplosion.m_uI.m_firsText.gameObject.SetActive(true);
			m_powers.m_arcaneExplosion.m_uI.m_secondText.gameObject.SetActive(true);
			if(m_powers.m_arcaneExplosion.m_actualCooldown > 0){
				m_powers.m_arcaneExplosion.m_actualCooldown -= Time.deltaTime;
				m_powers.m_arcaneExplosion.m_canSwitch = false;

				m_powers.m_arcaneExplosion.m_uI.m_firsText.text = m_powers.m_arcaneExplosion.m_actualCooldown.ToString("F1");
				m_powers.m_arcaneExplosion.m_uI.m_secondText.text = m_powers.m_arcaneExplosion.m_actualCooldown.ToString("F1");
				m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage.fillAmount -= 1 / m_powers.m_arcaneExplosion.m_cooldown * Time.deltaTime;
				m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage.fillAmount -= 1 / m_powers.m_arcaneExplosion.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_arcaneExplosion.m_actualCooldown = m_powers.m_arcaneExplosion.m_cooldown;
				m_powers.m_arcaneExplosion.m_startCooldown = false;
				m_powers.m_arcaneExplosion.m_canSwitch = true;

				m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage.gameObject.SetActive(false);
				m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage.gameObject.SetActive(false);
				m_powers.m_arcaneExplosion.m_uI.m_firsText.gameObject.SetActive(false);
				m_powers.m_arcaneExplosion.m_uI.m_secondText.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_arcaneExplosion.m_actualCooldown = m_powers.m_arcaneExplosion.m_cooldown;

			m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage.fillAmount = 1;
			m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage.fillAmount = 1;
		}
		AutoAttackCooldown();
	}

	void InitializeStartAutoAttackCooldown(){
		m_autoAttacks.m_actualCooldown = m_autoAttacks.m_cooldown;
	}
	void AutoAttackCooldown(){
		if(!m_canAutoAttack){
			m_autoAttacks.m_actualCooldown -= Time.deltaTime;
			if(m_autoAttacks.m_actualCooldown <= 0){
				m_canAutoAttack = true;
				if(m_autoAttacks.m_isBuff){
					m_autoAttacks.m_actualCooldown = m_autoAttacks.m_buffCooldown;
				}else{
					m_autoAttacks.m_actualCooldown = m_autoAttacks.m_cooldown;
				}
			}
		}
	}
	public void AutoAttack(){
		if(m_canAutoAttack){
			m_canAutoAttack = false;
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 hitPoint = new Vector3();
			RaycastHit floorHit;
			if(Physics.Raycast (ray, out floorHit, Mathf.Infinity, m_rotatePlayerLayer)){
				hitPoint = floorHit.point;
			}
			switch(m_currentElement){
				case ElementType.Arcane:
					GameObject arcaneGo = m_objectPooler.SpawnSpellFromPool(SpellType.AutoAttack_Arcane, m_autoAttacks.m_positionRoot.position, m_autoAttacks.m_rotationRoot.rotation);
					if(arcaneGo != null){
						arcaneGo.GetComponent<Projectile>().SetTargetPos(hitPoint);
					}
				break;
				case ElementType.Ice:
					GameObject iceGo = m_objectPooler.SpawnSpellFromPool(SpellType.AutoAttack_Ice, m_autoAttacks.m_positionRoot.position, m_autoAttacks.m_rotationRoot.rotation);
					if(iceGo != null){
						iceGo.GetComponent<Projectile>().SetTargetPos(hitPoint);
					}
				break;
				case ElementType.Fire:
					GameObject fireGo = m_objectPooler.SpawnSpellFromPool(SpellType.AutoAttack_Fire, m_autoAttacks.m_positionRoot.position, m_autoAttacks.m_rotationRoot.rotation);
					if(fireGo != null){
						fireGo.GetComponent<Projectile>().SetTargetPos(hitPoint);
					}
				break;
			}
			if(m_lastAutoAttackSound != null){
				Destroy(m_lastAutoAttackSound);
			}
			m_lastAutoAttackSound = SpawnRandomGameObject(m_autoAttacks.m_autoAttacksSounds);
		}
	}
	public void On_AutoAttackBuffChange(bool isBuff){
		if(isBuff){
			if(m_autoAttacks.m_actualCooldown > m_autoAttacks.m_buffCooldown){
				m_autoAttacks.m_actualCooldown = m_autoAttacks.m_buffCooldown;
			}
		}
	}

	public GameObject InstantiateGameObject(GameObject obj, Vector3 pos, Quaternion rot, Transform parent = null){
		if(obj != null){
			return Instantiate(obj, pos, rot, parent);
		}else{
			return null;
		}
	}

	public void ShakeCamera(float magnitude, float rougness, float fadeInTime, float fadeOutTime){
		CameraShaker.Instance.ShakeOnce(magnitude, rougness, fadeInTime, fadeOutTime);
	}

	public void SetTpPoint(Vector3 position){
		m_agent.Warp(position);
	}

	public void SetBlinkTrailRenderer(){
		StartCoroutine(BlinkRendererCorout());
	}
	IEnumerator BlinkRendererCorout(){
		m_powers.m_blink.m_trailRenderer.enabled = true;
		yield return new WaitForSeconds(m_powers.m_blink.m_timeToTrailRendererIsActive);
		m_powers.m_blink.m_trailRenderer.enabled = false;
	}

	public void On_PlayerDie(){
		if(m_playerDebug.m_playerCanDie){
			ChangeState(PlayerState.PlayerDieState);
		}
	}

	public void ManageSound(AudioSource audioSource, bool startSound){
		if(startSound){
			audioSource.Play();
		}else{
			audioSource.Stop();
		}
	}

	public void ChangeHudSpellAnim(){
		m_playerDebug.m_useSymetricalHudSpellAnim =! m_playerDebug.m_useSymetricalHudSpellAnim;
	}
	public void ChangeJainaCanDie(){
		m_playerDebug.m_playerCanDie =! m_playerDebug.m_playerCanDie;
	}

	public void SwitchPlayerToCinematicState(float timeToBeInCinematic){
		m_cinematic.m_timeToBeInCinematic = timeToBeInCinematic;
		ChangeState(PlayerState.PlayerCinematicState);
	}
	public void StartCinematicStringCorout(bool isShow){
		if(isShow){
			StartCoroutine(CinematicStringCorout(m_cinematic.m_startHeight, m_cinematic.m_endHieght));
		}else{
			StartCoroutine(CinematicStringCorout(m_cinematic.m_endHieght, m_cinematic.m_startHeight));
		}
	}
	IEnumerator CinematicStringCorout(float fromHeight, float toHeight){
		
		float distance = Mathf.Abs(fromHeight - toHeight);
		float vitesse = distance / m_cinematic.m_timeToEndAnim;
		float moveFracJourney = new float();
		float actualHeight = 1000000000;

		while(actualHeight != toHeight){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			actualHeight = Mathf.Lerp(fromHeight, toHeight, m_cinematic.m_curve.Evaluate(moveFracJourney));
			m_cinematic.m_topStrip.sizeDelta = new Vector2(m_cinematic.m_topStrip.sizeDelta.x, actualHeight);
			m_cinematic.m_botStrip.sizeDelta = new Vector2(m_cinematic.m_topStrip.sizeDelta.x, actualHeight);
			yield return null;
		}
	}
	public void GetOutOfCinematicState(){
		ChangeState(PlayerState.NoThrowSpellState);
	}

	public GameObject SpawnRandomGameObject(GameObject[] objects){
        if(objects.Length > 0){
            int alea = UnityEngine.Random.Range(0, objects.Length - 1);
            if(objects[alea] != null){
                // return Instantiate(objects[alea], Vector3.zero, Quaternion.identity);
                return Level.AddFX(objects[alea], Vector3.zero, Quaternion.identity).gameObject;
            }
        }
		return null;
    }

}
