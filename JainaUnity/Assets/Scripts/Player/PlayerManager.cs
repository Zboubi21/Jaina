using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;
using PlayerStateEnum;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance;

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
		/*[HideInInspector]*/ public float m_actualCooldown = 0;

		[Header("Prefabs")]
		public Transform m_spawnRoot;
		[Space]
		public GameObject m_arcaneAttack;
		public GameObject m_iceAttack;
		public GameObject m_fireAttack;
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
			[Space]
			public bool m_useMouseBlink = true;

			[Header("UI")]
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;
		}

		public Block m_Block = new Block();
		[System.Serializable] public class Block {
            [HideInInspector] public bool m_inIceBlock = false;

			public GameObject m_block;
			[HideInInspector] public bool m_canSwitch = true;
			[HideInInspector] public bool m_startCooldown = false;
			public float m_cooldown = 5;
			[HideInInspector] public float m_actualCooldown = 0;
			public float m_timeToBeInIceBlock = 2;
			[HideInInspector] public float m_actualIceBlockTimer = 0;

			[Header("UI")]
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;
		}

		public IceNova m_iceNova = new IceNova();
		[System.Serializable] public class IceNova {
			[Header("Spell prefab")]
			public GameObject m_nova;
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
			public GameObject m_UiParent;
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;
		}
		public IceBuff m_iceBuff = new IceBuff();
		[System.Serializable] public class IceBuff {
			[Header("Spell prefab")]
			public GameObject m_buff;
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
			public GameObject m_UiParent;
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;

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
			public GameObject m_UiParent;
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;
		}
		public FireTrail m_fireTrail = new FireTrail();
		[System.Serializable] public class FireTrail {
			[Header("Spell prefab")]
			public GameObject m_trail;
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
			public GameObject m_UiParent;
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;
		}

		public ArcaneProjectiles m_arcaneProjectiles = new ArcaneProjectiles();
		[System.Serializable] public class ArcaneProjectiles {
			[Header("Spell prefab")]
			public GameObject m_firstProjectile;
			public GameObject m_secondProjectile;
			public GameObject m_thirdProjectile;
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
			public GameObject m_UiParent;
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;

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
			public GameObject m_projectile;
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
			public GameObject m_UiParent;
			public TextMeshProUGUI m_text;
			public Image m_cooldownImage;

			[Header("Shake camera")]
			public bool m_useShakeCam = true;
			public float m_magnitudeShake = 4f;
			public float m_roughnessShake = 4f;
			public float m_fadeInTimeShake = 0.1f;
			public float m_fadeOutTimeShake = 0.1f;
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
		}
    }

    #endregion Public [System.Serializable] Variables
    [Space]
	public GameObject m_playerMesh;
	public LayerMask m_groundLayer;
	public LayerMask m_enemyLayer;

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
	Vector3 m_topLeftPos;
	Vector3 m_middleLeftPos;
	Vector3 m_botLeftPos;
	Vector3 m_topRightPos;
	Vector3 m_middleRightPos;
	Vector3 m_botRightPos;
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
		});
		string[] playerStateNames = System.Enum.GetNames (typeof(PlayerState));
		if(m_sM.States.Count != playerStateNames.Length){
			Debug.LogError("You need to have the same number of State in PlayerManager and PlayerStateEnum");
		}

		m_agent = GetComponent<NavMeshAgent>();
		SetUIElements();
	}

	void Start(){
		InitializeStartAutoAttackCooldown();
	}
	
	void OnEnable(){
		ChangeState(0);
	}

	void Update(){
		m_sM.Update();
		UpdateInputButtons();
		RaycastToMovePlayer();
		DecreaseCooldown();
		LookEnemyLifeBar();
	}

	void FixedUpdate(){
		m_sM.FixedUpdate();
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
        if(Physics.Raycast (ray, out floorHit, Mathf.Infinity, m_groundLayer)){
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            m_playerMesh.transform.rotation = newRotation;
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
		ChangeUIElements();
	}

	void SetUIElements(){
		m_topLeftPos = m_powers.m_fireBalls.m_UiParent.transform.position;
		m_middleLeftPos = m_powers.m_iceNova.m_UiParent.transform.position;
		m_botLeftPos = m_powers.m_arcaneProjectiles.m_UiParent.transform.position;

		m_topRightPos = m_powers.m_fireTrail.m_UiParent.transform.position;
		m_middleRightPos = m_powers.m_iceBuff.m_UiParent.transform.position;
		m_botRightPos = m_powers.m_arcaneExplosion.m_UiParent.transform.position;

		ChangeUIElements();
	}

	void ChangeUIElements(){
		switch(m_currentElement){
			case ElementType.Arcane:
				m_powers.m_fireBalls.m_UiParent.transform.position = m_topLeftPos;
				m_powers.m_iceNova.m_UiParent.transform.position = m_botLeftPos;
				m_powers.m_arcaneProjectiles.m_UiParent.transform.position = m_middleLeftPos;

				m_powers.m_arcaneExplosion.m_UiParent.transform.position = m_middleRightPos;
				m_powers.m_iceBuff.m_UiParent.transform.position = m_botRightPos;
				m_powers.m_fireTrail.m_UiParent.transform.position = m_topRightPos;
			break;
			case ElementType.Ice:
				m_powers.m_fireBalls.m_UiParent.transform.position = m_botLeftPos;
				m_powers.m_iceNova.m_UiParent.transform.position = m_middleLeftPos;
				m_powers.m_arcaneProjectiles.m_UiParent.transform.position = m_topLeftPos;

				m_powers.m_arcaneExplosion.m_UiParent.transform.position = m_topRightPos;
				m_powers.m_iceBuff.m_UiParent.transform.position = m_middleRightPos;
				m_powers.m_fireTrail.m_UiParent.transform.position = m_botRightPos;
			break;
			case ElementType.Fire:
				m_powers.m_fireBalls.m_UiParent.transform.position = m_middleLeftPos;
				m_powers.m_iceNova.m_UiParent.transform.position = m_topLeftPos;
				m_powers.m_arcaneProjectiles.m_UiParent.transform.position = m_botLeftPos;

				m_powers.m_arcaneExplosion.m_UiParent.transform.position = m_botRightPos;
				m_powers.m_iceBuff.m_UiParent.transform.position = m_topRightPos;
				m_powers.m_fireTrail.m_UiParent.transform.position = m_middleRightPos;
			break;
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
			m_powers.m_iceNova.m_cooldownImage.gameObject.SetActive(true);
			m_powers.m_iceNova.m_text.gameObject.SetActive(true);
			if(m_powers.m_iceNova.m_actualCooldown > 0){
				m_powers.m_iceNova.m_actualCooldown -= Time.deltaTime;
				m_powers.m_iceNova.m_canSwitch = false;

				m_powers.m_iceNova.m_text.text = m_powers.m_iceNova.m_actualCooldown.ToString("F1");
				m_powers.m_iceNova.m_cooldownImage.fillAmount -= 1 / m_powers.m_iceNova.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_iceNova.m_actualCooldown = m_powers.m_iceNova.m_cooldown;
				m_powers.m_iceNova.m_startCooldown = false;
				m_powers.m_iceNova.m_canSwitch = true;

				m_powers.m_iceNova.m_cooldownImage.gameObject.SetActive(false);
				m_powers.m_iceNova.m_text.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_iceNova.m_actualCooldown = m_powers.m_iceNova.m_cooldown;

			m_powers.m_iceNova.m_cooldownImage.fillAmount = 1;
		}
		// Ice Buff
		if(m_powers.m_iceBuff.m_startCooldown){
			m_powers.m_iceBuff.m_cooldownImage.gameObject.SetActive(true);
			m_powers.m_iceBuff.m_text.gameObject.SetActive(true);
			if(m_powers.m_iceBuff.m_actualCooldown > 0){
				m_powers.m_iceBuff.m_actualCooldown -= Time.deltaTime;
				m_powers.m_iceBuff.m_canSwitch = false;

				m_powers.m_iceBuff.m_text.text = m_powers.m_iceBuff.m_actualCooldown.ToString("F1");
				m_powers.m_iceBuff.m_cooldownImage.fillAmount -= 1 / m_powers.m_iceBuff.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_iceBuff.m_actualCooldown = m_powers.m_iceBuff.m_cooldown;
				m_powers.m_iceBuff.m_startCooldown = false;
				m_powers.m_iceBuff.m_canSwitch = true;

				m_powers.m_iceBuff.m_cooldownImage.gameObject.SetActive(false);
				m_powers.m_iceBuff.m_text.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_iceBuff.m_actualCooldown = m_powers.m_iceBuff.m_cooldown;

			m_powers.m_iceBuff.m_cooldownImage.fillAmount = 1;
		}
		// Fire Balls
		if(m_powers.m_fireBalls.m_startCooldown){
			m_powers.m_fireBalls.m_cooldownImage.gameObject.SetActive(true);
			m_powers.m_fireBalls.m_text.gameObject.SetActive(true);
			if(m_powers.m_fireBalls.m_actualCooldown > 0){
				m_powers.m_fireBalls.m_actualCooldown -= Time.deltaTime;
				m_powers.m_fireBalls.m_canSwitch = false;

				m_powers.m_fireBalls.m_text.text = m_powers.m_fireBalls.m_actualCooldown.ToString("F1");
				m_powers.m_fireBalls.m_cooldownImage.fillAmount -= 1 / m_powers.m_fireBalls.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_fireBalls.m_actualCooldown = m_powers.m_fireBalls.m_cooldown;
				m_powers.m_fireBalls.m_startCooldown = false;
				m_powers.m_fireBalls.m_canSwitch = true;

				m_powers.m_fireBalls.m_cooldownImage.gameObject.SetActive(false);
				m_powers.m_fireBalls.m_text.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_fireBalls.m_actualCooldown = m_powers.m_fireBalls.m_cooldown;

			m_powers.m_fireBalls.m_cooldownImage.fillAmount = 1;
		}
		// Fire Trail
		if(m_powers.m_fireTrail.m_startCooldown){
			m_powers.m_fireTrail.m_cooldownImage.gameObject.SetActive(true);
			m_powers.m_fireTrail.m_text.gameObject.SetActive(true);
			if(m_powers.m_fireTrail.m_actualCooldown > 0){
				m_powers.m_fireTrail.m_actualCooldown -= Time.deltaTime;
				m_powers.m_fireTrail.m_canSwitch = false;

				m_powers.m_fireTrail.m_text.text = m_powers.m_fireTrail.m_actualCooldown.ToString("F1");
				m_powers.m_fireTrail.m_cooldownImage.fillAmount -= 1 / m_powers.m_fireTrail.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_fireTrail.m_actualCooldown = m_powers.m_fireTrail.m_cooldown;
				m_powers.m_fireTrail.m_startCooldown = false;
				m_powers.m_fireTrail.m_canSwitch = true;

				m_powers.m_fireTrail.m_cooldownImage.gameObject.SetActive(false);
				m_powers.m_fireTrail.m_text.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_fireTrail.m_actualCooldown = m_powers.m_fireTrail.m_cooldown;

			m_powers.m_fireTrail.m_cooldownImage.fillAmount = 1;
		}
		// Arcane Projectiles
		if(m_powers.m_arcaneProjectiles.m_startCooldown){
			m_powers.m_arcaneProjectiles.m_cooldownImage.gameObject.SetActive(true);
			m_powers.m_arcaneProjectiles.m_text.gameObject.SetActive(true);
			if(m_powers.m_arcaneProjectiles.m_actualCooldown > 0){
				m_powers.m_arcaneProjectiles.m_actualCooldown -= Time.deltaTime;
				m_powers.m_arcaneProjectiles.m_canSwitch = false;

				m_powers.m_arcaneProjectiles.m_text.text = m_powers.m_arcaneProjectiles.m_actualCooldown.ToString("F1");
				m_powers.m_arcaneProjectiles.m_cooldownImage.fillAmount -= 1 / m_powers.m_arcaneProjectiles.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_arcaneProjectiles.m_actualCooldown = m_powers.m_arcaneProjectiles.m_cooldown;
				m_powers.m_arcaneProjectiles.m_startCooldown = false;
				m_powers.m_arcaneProjectiles.m_canSwitch = true;

				m_powers.m_arcaneProjectiles.m_cooldownImage.gameObject.SetActive(false);
				m_powers.m_arcaneProjectiles.m_text.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_arcaneProjectiles.m_actualCooldown = m_powers.m_arcaneProjectiles.m_cooldown;

			m_powers.m_arcaneProjectiles.m_cooldownImage.fillAmount = 1;
		}
		// Arcane Exploqion
		if(m_powers.m_arcaneExplosion.m_startCooldown){
			m_powers.m_arcaneExplosion.m_cooldownImage.gameObject.SetActive(true);
			m_powers.m_arcaneExplosion.m_text.gameObject.SetActive(true);
			if(m_powers.m_arcaneExplosion.m_actualCooldown > 0){
				m_powers.m_arcaneExplosion.m_actualCooldown -= Time.deltaTime;
				m_powers.m_arcaneExplosion.m_canSwitch = false;

				m_powers.m_arcaneExplosion.m_text.text = m_powers.m_arcaneExplosion.m_actualCooldown.ToString("F1");
				m_powers.m_arcaneExplosion.m_cooldownImage.fillAmount -= 1 / m_powers.m_arcaneExplosion.m_cooldown * Time.deltaTime;
			}else{
				m_powers.m_arcaneExplosion.m_actualCooldown = m_powers.m_arcaneExplosion.m_cooldown;
				m_powers.m_arcaneExplosion.m_startCooldown = false;
				m_powers.m_arcaneExplosion.m_canSwitch = true;

				m_powers.m_arcaneExplosion.m_cooldownImage.gameObject.SetActive(false);
				m_powers.m_arcaneExplosion.m_text.gameObject.SetActive(false);
			}
		}else{
			m_powers.m_arcaneExplosion.m_actualCooldown = m_powers.m_arcaneExplosion.m_cooldown;

			m_powers.m_arcaneExplosion.m_cooldownImage.fillAmount = 1;
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
			switch(m_currentElement){
				case ElementType.Arcane:
					Instantiate(m_autoAttacks.m_arcaneAttack, m_autoAttacks.m_spawnRoot.position, m_autoAttacks.m_spawnRoot.rotation);
				break;
				case ElementType.Ice:
					Instantiate(m_autoAttacks.m_iceAttack, m_autoAttacks.m_spawnRoot.position, m_autoAttacks.m_spawnRoot.rotation);
				break;
				case ElementType.Fire:
					Instantiate(m_autoAttacks.m_fireAttack, m_autoAttacks.m_spawnRoot.position, m_autoAttacks.m_spawnRoot.rotation);
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

	public GameObject InstantiateSpells(GameObject obj, Vector3 pos, Quaternion rot, Transform parent = null){
		return Instantiate(obj, pos, rot, parent);
	}

	public void ShakeCamera(float magnitude, float rougness, float fadeInTime, float fadeOutTime){
		CameraShaker.Instance.ShakeOnce(magnitude, rougness, fadeInTime, fadeOutTime);
	}


	EnemyStats m_currentEnemyStats;
	void LookEnemyLifeBar(){
		/*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit enemyHit;
        if(Physics.Raycast (ray, out enemyHit, Mathf.Infinity, m_enemyLayer)){
            EnemyStats enemyStatsTemp = enemyHit.collider.gameObject.GetComponent<EnemyStats>();
			if(m_currentEnemyStats != enemyStatsTemp){
				m_currentEnemyStats = enemyStatsTemp;
				BigEnemyLifeBarManager.Instance.ShowLifeBar(m_currentEnemyStats);
			}
        }*/
	}

	public void SetTpPoint(Vector3 position){
		m_agent.Warp(position);
	}

}
