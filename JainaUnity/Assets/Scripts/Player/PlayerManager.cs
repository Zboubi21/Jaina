using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;
using PlayerStateEnum;
using PoolTypes;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance;

	public PlayerDebug m_playerDebug = new PlayerDebug();
	[System.Serializable] public class PlayerDebug {
		public bool m_playerCanDie = true;
		public bool m_useSymetricalHudSpellAnim = true;
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

#region Public [System.Serializable] Variables

	[Header("AutoAttacks")]
	public AutoAttacks m_autoAttacks = new AutoAttacks();
	[System.Serializable] public class AutoAttacks {
		public float m_cooldown = 0.25f;
		public float m_buffCooldown = 0.125f;
		[HideInInspector] public bool m_isBuff = false;
		[HideInInspector] public float m_actualCooldown = 0;

		[Header("Prefabs")]
		public Transform m_positionRoot;
		public Transform m_rotationRoot;
		// [Space]
		// public GameObject m_arcaneAttack;
		// public GameObject m_iceAttack;
		// public GameObject m_fireAttack;
	}

	[Header("Powers")]
	public Powers m_powers = new Powers();
	[System.Serializable] public class Powers {

		public Blink m_blink = new Blink();
		[System.Serializable] public class Blink {
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
		[System.Serializable] public class Block {
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
		[System.Serializable] public class IceNova {
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
		[System.Serializable] public class IceBuff {
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
		[System.Serializable] public class FireBalls {
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
		[System.Serializable] public class FireTrail {
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
		[System.Serializable] public class ArcaneProjectiles {
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
			[System.Serializable] public class ShakeCamera {
				[Header("First Shake")]
				public FirstShake m_firstShake = new FirstShake();
				[System.Serializable] public class FirstShake {
					public bool m_useShakeCam = true;
				public float m_magnitudeShake = 4f;
				public float m_roughnessShake = 4f;
				public float m_fadeInTimeShake = 0.1f;
				public float m_fadeOutTimeShake = 0.1f;
				}
				[Header("Second Shake")]
				public SecoundShake m_secoundShake = new SecoundShake();
				[System.Serializable] public class SecoundShake {
				public bool m_useShakeCam = true;
				public float m_magnitudeShake = 4f;
				public float m_roughnessShake = 4f;
				public float m_fadeInTimeShake = 0.1f;
				public float m_fadeOutTimeShake = 0.1f;
				}
				[Header("Third Shake")]
				public ThirdShake m_thirdShake = new ThirdShake();
				[System.Serializable] public class ThirdShake {
				public bool m_useShakeCam = true;
				public float m_magnitudeShake = 4f;
				public float m_roughnessShake = 4f;
				public float m_fadeInTimeShake = 0.1f;
				public float m_fadeOutTimeShake = 0.1f;
				}
			}
		}
		public ArcaneExplosion m_arcaneExplosion = new ArcaneExplosion();
		[System.Serializable] public class ArcaneExplosion {
			[Header("Spell prefab")]
			// public GameObject m_projectile;
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

			[Header("Shake camera")]
			public bool m_useShakeCam = true;
			public float m_magnitudeShake = 4f;
			public float m_roughnessShake = 4f;
			public float m_fadeInTimeShake = 0.1f;
			public float m_fadeOutTimeShake = 0.1f;
		}

		[System.Serializable] public class SpellUI {
			[Header("First UI")]
			public RectTransform m_firstUiParent;
			public TextMeshProUGUI m_firsText;
			public Image m_firstSpellImage;
			public Image m_firstCooldownImage;

			[Header("Second UI")]
			public RectTransform m_secondUiParent;
			public TextMeshProUGUI m_secondText;
			public Image m_secondSpellImage;
			public Image m_secondCooldownImage;
		}

		[Header("UI")]
		public UI m_uI = new UI();
		[System.Serializable] public class UI {
			[Header("Width/Height")]
			public Vector2 m_minScale = new Vector2(44, 44);
			public Vector2 m_maxScale = new Vector2(58, 58);

			[Header("Text size")]
			public float m_minSize = 20;
			public float m_maxSize = 30;

			[Header("UI Animations")]
			public UIAnimations m_uIAnimations = new UIAnimations();
			[System.Serializable] public class UIAnimations {
				public float m_timeToFinish = 0.25f;
				public AnimationCurve m_curveAnim;

				public RectTransform m_leftLeftPosition;
				public RectTransform m_leftRightPosition;

				public RectTransform m_rightLeftPosition;
				public RectTransform m_rightRightPosition;
			}
		}
	}

	[Header("Elements")]
    [Range(0,100)]
    public float m_percentMultiplicateur = 25f;
    public int m_maxArcanMarkCount = 10;
    public Debuffs m_debuffs = new Debuffs();
    [System.Serializable]
    public class Debuffs
    {
        [Header("Fire Ticks")]
		public FireTicks m_fireTicks = new FireTicks();
		[System.Serializable]
		public class FireTicks
		{
			public float m_timerTickDamage = 0.25f;
			public int m_fireTickDamage = 5;
			public int m_fireExplosionDamage = 100;
        }
		[Header("Ice Slow")]
		public IceSlow m_IceSlow = new IceSlow();
		[System.Serializable]
		public class IceSlow
		{
			[Range(0,100)]
			public int m_iceSlow = 25;
            public float m_frozenTime;
        }
    }

	[Header("Move speed")]
	public MoveSpeed m_moveSpeed = new MoveSpeed();
	[System.Serializable] public class MoveSpeed {
		public float m_normalspeed = 10;
		public float m_fastSpeed = 15;
		[Space]
		public float m_timeToKeepFastSpeed = 2;
		public TrailRenderer[] m_trailRenderers = new TrailRenderer[3];

		[HideInInspector] public bool m_playerInBuff = false;
		[HideInInspector] public float m_actualTimer = 0;
		[HideInInspector] public bool m_iceBuffIsCast = false;
	}

    #endregion Public [System.Serializable] Variables

    [Space]
	public GameObject m_playerMesh;
	public GameObject m_jainaMesh;
	public LayerMask m_groundLayer;
	public LayerMask m_rotatePlayerLayer;

	[HideInInspector] public bool m_canThrowSpell = true;
	bool m_canAutoAttack = true;
	
#region Input Buttons

	[HideInInspector] public bool m_leftMouseDownClick;
	[HideInInspector] public bool m_leftMouseClick;
	[HideInInspector] public bool m_rightMouseClick;
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

	NavMeshAgent m_agent;
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

    void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of PlayerManager");
		}
		m_sM.AddStates(new List<IState> {
			new NoThrowSpellState(this),		// 0 = No Throw Spella
			new BlinkState(this),		 		// 1 - Blink	
			new IceBlockState(this), 			// 2 - Ice Block
			new NovaState(this),				// 3 - Nova
			new FireBallsState(this),			// 4 - Fire Balls
			new ArcaneProjectilesState(this), 	// 5 - Arcane Projectiles
			new IceBuffState(this),				// 6 - Ice Buff
			new FireTrailState(this),			// 7 - Fire Trail
			new ArcaneExplosionState(this),		// 8 - Arcane Explosion
			new PlayerDieState(this),			// 8 - Die
		});
		string[] playerStateNames = System.Enum.GetNames (typeof(PlayerState));
		if(m_sM.States.Count != playerStateNames.Length){
			Debug.LogError("You need to have the same number of State in PlayerManager and PlayerStateEnum");
		}

		m_agent = GetComponent<NavMeshAgent>();
		SetUIElements();
	}

	void Start(){
		m_jainaAnimator = m_jainaMesh.GetComponent<Animator>();
		m_saveManager = SaveManager.Instance;
		m_objectPooler = ObjectPooler.Instance;
		InitializeStartAutoAttackCooldown();
		SetPlayerSpeed(m_moveSpeed.m_normalspeed);
	}
	
	void OnEnable(){
		ChangeState(0);
	}

	void Update(){
		m_sM.Update();
		UpdateInputButtons();
		RaycastToMovePlayer();
		DecreaseCooldown();
		UpdatePlayerSpeed();
	}

	void FixedUpdate(){
		m_sM.FixedUpdate();

		if(m_agent.velocity != Vector3.zero ){
			m_jainaAnimator.SetBool("isMoving", true);
		}else{
			m_jainaAnimator.SetBool("isMoving", false);
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

		m_rightMouseClick = Input.GetButton("RightClick");
		m_blinkButton = Input.GetButtonDown("Blink");
		m_iceBlockButton = Input.GetButtonDown("IceBlock");
		m_leftSpellButton = Input.GetButtonDown("LeftSpell");
		m_rightSpellButton = Input.GetButtonDown("RightSpell");
	}

	void RaycastToMovePlayer(){
		if(m_rightMouseClick){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, m_groundLayer)){
				PlayerTargetPosition = hit.point;
			}
		}
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

		ChangeSpellAlpha(m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0);
		ChangeSpellAlpha(m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0);
		ChangeSpellAlpha(m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0);
		ChangeSpellAlpha(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0);
		ChangeSpellAlpha(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0);
		ChangeSpellAlpha(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0);

		ChangeUIElements();
	}

	void ChangeUIElements(bool rightSpell = true){
		switch(m_currentElement){
			case ElementType.Arcane:
				// --------------------------
				// ---------- LEFT ----------
				// --------------------------

				// -------------------------
				// ---------- ICE ----------
				if(!rightSpell){
					// First
					StartCoroutine(MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition));
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceNova.m_uI.m_firstSpellImage, m_powers.m_iceNova.m_uI.m_firstCooldownImage, m_powers.m_iceNova.m_uI.m_firsText, 1, 0));
					
					StartCoroutine(ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_iceNova.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition;
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0, 1));

					StartCoroutine(MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_secondUiParent, m_powers.m_iceNova.m_uI.m_secondUiParent.localPosition, m_leftLeftPos, 
					m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstSpellImage, m_powers.m_iceNova.m_uI.m_firstCooldownImage, m_powers.m_iceNova.m_uI.m_firsText, 1, 
					m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0));
					// -------------------------
				}else{
					StartCoroutine(MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_leftLeftPos));

					StartCoroutine(ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}

				StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_leftMiddlePos));
				StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				StartCoroutine(ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// --------------------------
				// ---------- FIRE ----------
				if(rightSpell){
					// First
					StartCoroutine(MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition));
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireBalls.m_uI.m_firstSpellImage, m_powers.m_fireBalls.m_uI.m_firstCooldownImage, m_powers.m_fireBalls.m_uI.m_firsText, 1, 0));
					
					StartCoroutine(ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_fireBalls.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition;
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0, 1));

					StartCoroutine(MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_secondUiParent, m_powers.m_fireBalls.m_uI.m_secondUiParent.localPosition, m_leftRightPos, 
					m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstSpellImage, m_powers.m_fireBalls.m_uI.m_firstCooldownImage, m_powers.m_fireBalls.m_uI.m_firsText, 1, 
					m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0));
					// --------------------------
				}else{
					StartCoroutine(MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_leftRightPos));
				
					StartCoroutine(ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}


				// ---------------------------
				// ---------- RIGHT ----------
				// ---------------------------

				// --------------------------
				// ---------- FIRE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 0));

						StartCoroutine(ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0, 1));
						
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_secondUiParent, m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
						m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 
						m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0));
						// --------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}			
				}else{
					if(rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 0));

						StartCoroutine(ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0, 1));
						
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_secondUiParent, m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
						m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 
						m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0));
						// --------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}
				}
				
				StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightMiddlePos));
				StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				StartCoroutine(ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));
				
				// -------------------------
				// ---------- ICE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(!rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 0));
						
						StartCoroutine(ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0, 1));

						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_secondUiParent, m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
						m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 
						m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0));
						// -------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}else{
					if(!rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 0));
						
						StartCoroutine(ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0, 1));

						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_secondUiParent, m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
						m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 
						m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0));
						// -------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
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
					StartCoroutine(MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition));
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireBalls.m_uI.m_firstSpellImage, m_powers.m_fireBalls.m_uI.m_firstCooldownImage, m_powers.m_fireBalls.m_uI.m_firsText, 1, 0));
					
					StartCoroutine(ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_fireBalls.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition;
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0, 1));

					StartCoroutine(MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_secondUiParent, m_powers.m_fireBalls.m_uI.m_secondUiParent.localPosition, m_leftLeftPos, 
					m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstSpellImage, m_powers.m_fireBalls.m_uI.m_firstCooldownImage, m_powers.m_fireBalls.m_uI.m_firsText, 1, 
					m_powers.m_fireBalls.m_uI.m_secondSpellImage, m_powers.m_fireBalls.m_uI.m_secondCooldownImage, m_powers.m_fireBalls.m_uI.m_secondText, 0));
					// -------------------------
				}else{
					StartCoroutine(MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_leftLeftPos));

					StartCoroutine(ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}
				
				StartCoroutine(MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_leftMiddlePos));
				StartCoroutine(ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				StartCoroutine(ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// ----------------------------
				// ---------- ARCANE ----------
				if(rightSpell){
					// First
					StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition));
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneProjectiles.m_uI.m_firstSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_firsText, 1, 0));
					
					StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition;
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0, 1));

					StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent, m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent.localPosition, m_leftRightPos, 
					m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_firsText, 1, 
					m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0));
					// ----------------------------
				}else{
					StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_leftRightPos));
				
					StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}


				// ---------------------------
				// ---------- RIGHT ----------
				// ---------------------------

				// ----------------------------
				// ---------- ARCANE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 0));

						StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0, 1));

						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_secondUiParent, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
						m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 
						m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0));
						// ----------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}else{
					if(rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 0));

						StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0, 1));

						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_secondUiParent, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
						m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 
						m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0));
						// ----------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}

				StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightMiddlePos));
				StartCoroutine(ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				StartCoroutine(ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// --------------------------
				// ---------- FIRE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(!rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 0));

						StartCoroutine(ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0, 1));

						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_secondUiParent, m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
						m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 
						m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0));
						// --------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}else{
					if(!rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 0));

						StartCoroutine(ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0, 1));
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_secondUiParent, m_powers.m_fireTrail.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
						m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstSpellImage, m_powers.m_fireTrail.m_uI.m_firstCooldownImage, m_powers.m_fireTrail.m_uI.m_firsText, 1, 
						m_powers.m_fireTrail.m_uI.m_secondSpellImage, m_powers.m_fireTrail.m_uI.m_secondCooldownImage, m_powers.m_fireTrail.m_uI.m_secondText, 0));
						// --------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
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
					StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition));
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneProjectiles.m_uI.m_firstSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_firsText, 1, 0));
					
					StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition;
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0, 1));

					StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent, m_powers.m_arcaneProjectiles.m_uI.m_secondUiParent.localPosition, m_leftLeftPos, 
					m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_firstCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_firsText, 1, 
					m_powers.m_arcaneProjectiles.m_uI.m_secondSpellImage, m_powers.m_arcaneProjectiles.m_uI.m_secondCooldownImage, m_powers.m_arcaneProjectiles.m_uI.m_secondText, 0));
					// -------------------------
				}else{
					StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.localPosition, m_leftLeftPos));
				
					StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent, m_powers.m_arcaneProjectiles.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_arcaneProjectiles.m_uI.m_firsText, m_powers.m_arcaneProjectiles.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}

				StartCoroutine(MoveToYourNextPosition(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.localPosition, m_leftMiddlePos));
				StartCoroutine(ChangeSpriteSize(m_powers.m_fireBalls.m_uI.m_firstUiParent, m_powers.m_fireBalls.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				StartCoroutine(ChangeFontSize(m_powers.m_fireBalls.m_uI.m_firsText, m_powers.m_fireBalls.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// -------------------------
				// ---------- ICE ----------
				if(rightSpell){
					// First
					StartCoroutine(MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_leftLeftPosition.localPosition));
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceNova.m_uI.m_firstSpellImage, m_powers.m_iceNova.m_uI.m_firstCooldownImage, m_powers.m_iceNova.m_uI.m_firsText, 1, 0));
					
					StartCoroutine(ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					// Second
					m_powers.m_iceNova.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_leftRightPosition.localPosition;
					StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0, 1));

					StartCoroutine(MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_secondUiParent, m_powers.m_iceNova.m_uI.m_secondUiParent.localPosition, m_leftRightPos, 
					m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstSpellImage, m_powers.m_iceNova.m_uI.m_firstCooldownImage, m_powers.m_iceNova.m_uI.m_firsText, 1, 
					m_powers.m_iceNova.m_uI.m_secondSpellImage, m_powers.m_iceNova.m_uI.m_secondCooldownImage, m_powers.m_iceNova.m_uI.m_secondText, 0));
					// -------------------------
				}else{
					StartCoroutine(MoveToYourNextPosition(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.localPosition, m_leftRightPos));
					
					StartCoroutine(ChangeSpriteSize(m_powers.m_iceNova.m_uI.m_firstUiParent, m_powers.m_iceNova.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
					StartCoroutine(ChangeFontSize(m_powers.m_iceNova.m_uI.m_firsText, m_powers.m_iceNova.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
				}


				// ---------------------------
				// ---------- RIGHT ----------
				// ---------------------------

				// -------------------------
				// ---------- ICE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 0));

						StartCoroutine(ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0, 1));
						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_secondUiParent, m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
						m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 
						m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0));
						// -------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}else{
					if(rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 0));

						StartCoroutine(ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0, 1));

						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_secondUiParent, m_powers.m_iceBuff.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
						m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstSpellImage, m_powers.m_iceBuff.m_uI.m_firstCooldownImage, m_powers.m_iceBuff.m_uI.m_firsText, 1, 
						m_powers.m_iceBuff.m_uI.m_secondSpellImage, m_powers.m_iceBuff.m_uI.m_secondCooldownImage, m_powers.m_iceBuff.m_uI.m_secondText, 0));
						// -------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_iceBuff.m_uI.m_firstUiParent, m_powers.m_iceBuff.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_iceBuff.m_uI.m_firsText, m_powers.m_iceBuff.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}

				StartCoroutine(MoveToYourNextPosition(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.localPosition, m_rightMiddlePos));
				StartCoroutine(ChangeSpriteSize(m_powers.m_fireTrail.m_uI.m_firstUiParent, m_powers.m_fireTrail.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_maxScale));
				StartCoroutine(ChangeFontSize(m_powers.m_fireTrail.m_uI.m_firsText, m_powers.m_fireTrail.m_uI.m_firsText.fontSize, m_powers.m_uI.m_maxSize));

				// ----------------------------
				// ---------- ARCANE ----------
				if(m_playerDebug.m_useSymetricalHudSpellAnim){
					if(!rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 0));
						
						StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition;
						
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0, 1));
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_secondUiParent, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition, m_rightRightPos, 
						m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 
						m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0));
						// ----------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightRightPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}				
				}else{
					if(!rightSpell){
						// First
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_powers.m_uI.m_uIAnimations.m_rightRightPosition.localPosition));
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 0));
						
						StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
						// Second
						m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition = m_powers.m_uI.m_uIAnimations.m_rightLeftPosition.localPosition;
						
						StartCoroutine(ChangeSpellAlphaCorout(m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0, 1));
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_secondUiParent, m_powers.m_arcaneExplosion.m_uI.m_secondUiParent.localPosition, m_rightLeftPos, 
						m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstSpellImage, m_powers.m_arcaneExplosion.m_uI.m_firstCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_firsText, 1, 
						m_powers.m_arcaneExplosion.m_uI.m_secondSpellImage, m_powers.m_arcaneExplosion.m_uI.m_secondCooldownImage, m_powers.m_arcaneExplosion.m_uI.m_secondText, 0));
						// ----------------------------
					}else{
						StartCoroutine(MoveToYourNextPosition(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.localPosition, m_rightLeftPos));

						StartCoroutine(ChangeSpriteSize(m_powers.m_arcaneExplosion.m_uI.m_firstUiParent, m_powers.m_arcaneExplosion.m_uI.m_firstUiParent.sizeDelta, m_powers.m_uI.m_minScale));
						StartCoroutine(ChangeFontSize(m_powers.m_arcaneExplosion.m_uI.m_firsText, m_powers.m_arcaneExplosion.m_uI.m_firsText.fontSize, m_powers.m_uI.m_minSize));
					}
				}
			break;
		}
	}

	void ChangeSpellAlpha(Image spellImg, Image cdImg, TextMeshProUGUI text, float newAlpha){
		spellImg.color = new Color(spellImg.color.r, spellImg.color.g, spellImg.color.b, newAlpha);
		cdImg.color = new Color(cdImg.color.r, cdImg.color.g, cdImg.color.b, newAlpha);
		text.color = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
	}

	IEnumerator ChangeSpellAlphaCorout(Image spellImg, Image cdImg, TextMeshProUGUI text, float fromAlpha, float toAlpha){
		
		float distance = Mathf.Abs(fromAlpha - toAlpha);
		float moveFracJourney = new float();
		float vitesse = distance / m_powers.m_uI.m_uIAnimations.m_timeToFinish;
		Color desiredColor = new Color(spellImg.color.r, spellImg.color.g, spellImg.color.b, toAlpha);

		while(spellImg.color != desiredColor){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			float alphaValue = Mathf.Lerp(fromAlpha, toAlpha, m_powers.m_uI.m_uIAnimations.m_curveAnim.Evaluate(moveFracJourney));
			spellImg.color = new Color(spellImg.color.r, spellImg.color.g, spellImg.color.b, alphaValue);
			cdImg.color = new Color(cdImg.color.r, cdImg.color.g, cdImg.color.b, alphaValue);
			text.color = new Color(text.color.r, text.color.g, text.color.b, alphaValue);
			yield return null;
		}
	}

	IEnumerator MoveToYourNextPosition(RectTransform transformObject, Vector3 fromPosition, Vector3 toPosition, RectTransform firstSpellToTp = null, Image firstSpellImg = null, Image firstCdImg = null, TextMeshProUGUI firstText = null, float firstNewAlpha = 0, Image secondSpellImg = null, Image secondCdImg = null, TextMeshProUGUI secondText = null, float secondNewAlpha = 0){
		
		float distance = Vector3.Distance(fromPosition, toPosition);
		float moveFracJourney = new float();
		float vitesse = distance / m_powers.m_uI.m_uIAnimations.m_timeToFinish;

		while(transformObject.localPosition != toPosition){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			transformObject.localPosition = Vector3.Lerp(fromPosition, toPosition, m_powers.m_uI.m_uIAnimations.m_curveAnim.Evaluate(moveFracJourney));
			yield return null;
		}
		if(firstSpellToTp != null){
			firstSpellToTp.localPosition = transformObject.localPosition;				// On TP le spell principal
			ChangeSpellAlpha(firstSpellImg, firstCdImg, firstText, firstNewAlpha);		// On enlève sa transparence
			ChangeSpellAlpha(secondSpellImg, secondCdImg, secondText, secondNewAlpha);	// On met transparent le spell secondaire
		}
	}
	IEnumerator ChangeSpriteSize(RectTransform transformObject, Vector2 fromScale, Vector2 toScale){
		
		float distance = Vector3.Distance(fromScale, toScale);
		float moveFracJourney = new float();
		float vitesse = distance / m_powers.m_uI.m_uIAnimations.m_timeToFinish;

		while(transformObject.sizeDelta != toScale){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			transformObject.sizeDelta = Vector3.Lerp(fromScale, toScale, m_powers.m_uI.m_uIAnimations.m_curveAnim.Evaluate(moveFracJourney));
			yield return null;
		}
	}
	IEnumerator ChangeFontSize(TextMeshProUGUI textObject, float fromSize, float toSize){
		
		float distance = Mathf.Abs(fromSize - toSize);
		float moveFracJourney = new float();
		float vitesse = distance / m_powers.m_uI.m_uIAnimations.m_timeToFinish;

		while(textObject.fontSize != toSize){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			textObject.fontSize = Mathf.Lerp(fromSize, toSize, m_powers.m_uI.m_uIAnimations.m_curveAnim.Evaluate(moveFracJourney));
			yield return null;
		}
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
				m_powers.m_blink.m_cooldownImage.fillAmount -= 1 / m_powers.m_blink.m_cooldown * Time.deltaTime;
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

}
