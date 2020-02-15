using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class LavaBeamController : BossAttack
{
    public GameObject[] laserSpanwerPhase3;
    public GameObject[] laserSpanwerPhase2;
    [Space]
    [Header("Lava Beam Var")]
    // public Transform target;
    public float timeBeforeShooting;
    public float timeBetweenEachShoot;
    float _currentTimeBeforeShooting;
    public int nbrOfShot=1;
    public int damage;
    [Space]
    [Header("MovingSign")]
    public AnimationCurve scaleSignCurve;
    public Color signColor;
    public Color shootingSignColor;
    float minSignSize;
    public float maxSignSize;
    [Space]
    [Header("VFX")]
    public AnimationCurve[] startAndEndCurve;
    public float timeOfEntranceExitAnimation;
    public GameObject signActifSpawner;
    public AnimationCurve[] shootingCurve;
    public float timeOfShootingAnimation;
    public float minAnimationSize;
    public float maxAnimationSize;
    public GameObject lavaBeamVFX;
    [Space]
    [Header("SFX")]
    public GameObject lavaBeamShootSFX;
    [Space]
    [Header("DEBUG")]
    [Range(1,3)]
    public int phaseNbr;
    public float offset = 89f;
    public float lookAtOffset;
    [SerializeField] bool m_debugInput = false;

    bool lavaBeamPaternActif;
    bool allShootingHaveBeenDone = true;
    List<GameObject> actifSpawner = new List<GameObject>();
    List<Beam_Spawner_Controller> actifSpawnerScript = new List<Beam_Spawner_Controller>();
    List<GameObject> actifSpawnerParent = new List<GameObject>();

    Transform m_target;

    void Start()
    {
        m_target = PlayerManager.Instance.transform;
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartLaser(phaseNbr);
            lavaBeamPaternActif = true;
        }
#endif
        //Debug.Log(Time.deltaTime);
        SpawnerLookAt();
    }
    public LayerMask mask;
    void SpawnerLookAt()
    {
        if (lavaBeamPaternActif)
        {
            for (int i = 0, l = actifSpawner.Count; i < l; ++i)
            {
                if (actifSpawnerScript[i].HasToLookAt)
                {
                    float PosY = m_target.localPosition.y + lookAtOffset;
                    Vector3 pos = new Vector3(m_target.localPosition.x, PosY, m_target.localPosition.z);


                    actifSpawner[i].transform.LookAt(m_target);

                    Vector3 angle = actifSpawner[i].transform.localEulerAngles;
                    Vector3 pivotAngle = actifSpawnerScript[i].pivotPoint.transform.localEulerAngles;
                    pivotAngle.z = -angle.y + offset;
                    actifSpawnerScript[i].pivotPoint.transform.localEulerAngles = pivotAngle;
                }
            }
        }
    }

    void StartLaser(int phaseNbr)
    {
        actifSpawner.Clear();
        actifSpawnerScript.Clear();
        actifSpawnerParent.Clear();
        switch (phaseNbr)
        {
            case 1:

                InitiateControle(laserSpanwerPhase3, phaseNbr);

                break;

            case 2:

                InitiateControle(laserSpanwerPhase2, phaseNbr);

                break;

            case 3:

                InitiateControle(laserSpanwerPhase3, phaseNbr);

                break;

            default:
                break;

        }
        StartCoroutine(EntranceDeclencher());
    }

    void InitiateControle(GameObject[] laserSpanwerPhase, int nbrOfPhase)
    {
        for (int i = 0, l = laserSpanwerPhase.Length; i < l; ++i)
        {
            Beam_Spawner_Controller scrpitRef = laserSpanwerPhase[i].GetComponent<Beam_Spawner_Controller>();
            GameObject go = scrpitRef.actifVFX;

            actifSpawnerParent.Add(laserSpanwerPhase[i]);
            actifSpawner.Add(go);
            actifSpawnerScript.Add(scrpitRef);

            if(nbrOfPhase == 1)
            {
                break;
            }
        }
    }

    IEnumerator EntranceDeclencher()
    {
        for (int i = 0, l = actifSpawner.Count; i < l; ++i)
        {
            yield return new WaitForSeconds(timeBeforeShooting / 2);
            yield return StartCoroutine(EntranceAndExit(startAndEndCurve[0], i, actifSpawner[i]));
            maxSignSize = actifSpawnerScript[i].sign.rectTransform.sizeDelta.y;
            actifSpawnerScript[i].movingSign.color = signColor;
            StartCoroutine(WaitUntilShooting(i));
        }
    }

    IEnumerator WaitUntilShooting(int i)
    {
        for (int a = 0; a < nbrOfShot; ++a)
        {
            yield return StartCoroutine(MovingOrbsSign(scaleSignCurve, i));
            yield return StartCoroutine(ShootLava(nbrOfShot, i));
        }

        if (i == (actifSpawner.Count-1))
        {
            On_AttackEnd();
        }
    }

    IEnumerator ShootLava(int nbrOfShot, int ActifSpawner)
    {
        actifSpawnerScript[ActifSpawner].HasToLookAt = false;

        yield return new WaitForSeconds(timeBetweenEachShoot/2);

        yield return StartCoroutine(SignAboutToShootForSpawner(shootingCurve[0], actifSpawner[ActifSpawner].GetComponent<ParticleSystem>()));

        actifSpawnerScript[ActifSpawner].movingSign.color = shootingSignColor;
        GameObject go = ObjectPooler.Instance.SpawnSpellFromPool(SpellType.LavaBeam, actifSpawner[ActifSpawner].transform.position, actifSpawner[ActifSpawner].transform.rotation);
        go.GetComponent<ProjectileReference>().childScript.GetComponent<Projectile>().Damage = damage;
        Level.AddFX(lavaBeamShootSFX, transform.position, transform.rotation);

        yield return StartCoroutine(SignAboutToShootForSpawner(shootingCurve[1], actifSpawner[ActifSpawner].GetComponent<ParticleSystem>()));

        actifSpawnerScript[ActifSpawner].movingSign.color = signColor;
        actifSpawnerScript[ActifSpawner].NbrOfShoot++;

        yield return new WaitForSeconds(timeBetweenEachShoot/2);

        actifSpawnerScript[ActifSpawner].HasToLookAt = true;

        if (actifSpawnerScript[ActifSpawner].NbrOfShoot == nbrOfShot)
        {
            StartCoroutine(StopUseShootLava(ActifSpawner));
        }
    }

    IEnumerator StopUseShootLava(int i)
    {
        actifSpawnerScript[i].HasToLookAt = false;
        actifSpawnerScript[i].NbrOfShoot = 0;

        yield return StartCoroutine(EntranceAndExit(startAndEndCurve[1], i, actifSpawner[i])); 

        actifSpawnerParent[i].gameObject.SetActive(false);
        actifSpawnerScript[i].pivotPoint.gameObject.SetActive(false);
        actifSpawnerScript[i].HasToLookAt = true;
    }

    IEnumerator EntranceAndExit(AnimationCurve curve, int i, GameObject actifSpawner)
    {
        float _currentTimeOfAnimation = 0;
        actifSpawnerParent[i].SetActive(true);
        while (_currentTimeOfAnimation / timeOfEntranceExitAnimation <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float height = curve.Evaluate(_currentTimeOfAnimation / timeOfEntranceExitAnimation);
            Vector3 pos = actifSpawner.transform.localPosition;

            pos.y = Mathf.Lerp(actifSpawnerScript[i].startPivot.localPosition.y, 0, height);

            actifSpawner.transform.localPosition = pos;
            yield return null;

        }
        actifSpawnerScript[i].pivotPoint.gameObject.SetActive(true);
        _currentTimeOfAnimation = 0;
    }

    IEnumerator MovingOrbsSign(AnimationCurve curve, int actifSpawner)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation < timeBeforeShooting)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            _currentTimeOfAnimation += Time.deltaTime;
            float size = curve.Evaluate(_currentTimeOfAnimation / timeBeforeShooting);

            Vector2 tempTrans = actifSpawnerScript[actifSpawner].sign.rectTransform.sizeDelta;
            tempTrans.y = Mathf.Lerp(0, maxSignSize, size);
            actifSpawnerScript[actifSpawner].movingSign.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempTrans.y);
            yield return null;
        }
        _currentTimeOfAnimation = 0;
    }

    IEnumerator SignAboutToShootForSpawner(AnimationCurve curve, ParticleSystem actifSpawner)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / timeOfShootingAnimation <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float size = curve.Evaluate(_currentTimeOfAnimation / timeOfShootingAnimation);

            ParticleSystem.SizeOverLifetimeModule _sizeOver = actifSpawner.sizeOverLifetime;

            _sizeOver.size = Mathf.Lerp(minAnimationSize, maxAnimationSize, size);
            yield return null;

        }
        _currentTimeOfAnimation = 0;
    }

    public override void On_AttackBegin(int phaseNbr)
    {
        base.On_AttackBegin(phaseNbr);
        StartLaser(phaseNbr);
        lavaBeamPaternActif = true;
    }

    public override void On_AttackEnd()
    {
        base.On_AttackEnd();
        lavaBeamPaternActif = false;
    }

    public override void On_GolemAreGoingToDie()
    {
        StopAllCoroutines();
        base.On_GolemAreGoingToDie();
        lavaBeamPaternActif = false;

        for (int i = 0, l = actifSpawner.Count; i < l; ++i)
        {
            StartCoroutine(StopUseShootLava(i));
        }
        
    }
    
}
