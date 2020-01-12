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
        if (Input.GetKeyDown(KeyCode.M) && m_debugInput)
        {
            StartLaser(phaseNbr);
            lavaBeamPaternActif = true;
        }

        SpawnerLookAt();
    }
    public LayerMask mask;
    void SpawnerLookAt()
    {
        if (lavaBeamPaternActif)
        {
            for (int i = 0, l = actifSpawner.Count; i < l; ++i)
            {
                /*RaycastHit hit;
                if(Physics.Raycast(actifSpawner[i].transform.position, actifSpawner[i].transform.forward, out hit, Mathf.Infinity, mask))
                {
                    float fill = Mathf.InverseLerp(actifSpawner[i].transform.position.z, target.position.z, hit.point.z);
                    actifSpawnerScript[i].sign.fillAmount = fill;
                    Debug.DrawLine(actifSpawner[i].transform.position, target.position, Color.green);
                }
                if (Physics.Linecast(actifSpawner[i].transform.position, target.position, mask, QueryTriggerInteraction.Collide))
                {
                }
                else
                {
                    actifSpawnerScript[i].sign.fillAmount = 1;
                    Debug.DrawLine(actifSpawner[i].transform.position, target.position, Color.red);
                }*/

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
        StartCoroutine(WaitUntilShooting());
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

        }
        actifSpawnerScript[i].pivotPoint.gameObject.SetActive(true);
        _currentTimeOfAnimation = 0;
    }


    IEnumerator WaitUntilShooting()
    {

        for (int i = 0, l = actifSpawner.Count; i < l; ++i)
        {
            StartCoroutine(EntranceAndExit(startAndEndCurve[0], i, actifSpawner[i]));
        }

        for (int i = 0; i < nbrOfShot; ++i)
        {
            yield return new WaitForSeconds(timeBeforeShooting);
            yield return StartCoroutine(ShootLava(nbrOfShot));
        }
        
        On_AttackEnd();
    }
    IEnumerator ShootLava(int nbrOfShot)
    {
        for (int i = 0, l = actifSpawner.Count; i < l; ++i)
        {
            actifSpawnerScript[i].HasToLookAt = false;
            yield return new WaitForSeconds(timeBetweenEachShoot/2);
            yield return StartCoroutine(SignAboutToShootForSpawner(shootingCurve[0], actifSpawner[i].GetComponent<ParticleSystem>()));
            GameObject go = ObjectPooler.Instance.SpawnSpellFromPool(SpellType.LavaBeam, actifSpawner[i].transform.position, actifSpawner[i].transform.rotation);
            //Level.AddFX(lavaBeamVFX, actifSpawner[i].transform.position, actifSpawner[i].transform.rotation);
            go.GetComponent<ProjectileReference>().childScript.GetComponent<Projectile>().Damage = damage;
            Level.AddFX(lavaBeamShootSFX, transform.position, transform.rotation);
            yield return StartCoroutine(SignAboutToShootForSpawner(shootingCurve[1], actifSpawner[i].GetComponent<ParticleSystem>()));
            actifSpawnerScript[i].NbrOfShoot++;
            yield return new WaitForSeconds(timeBetweenEachShoot/2);
            actifSpawnerScript[i].HasToLookAt = true;
            if(actifSpawnerScript[i].NbrOfShoot == nbrOfShot)
            {
                actifSpawnerScript[i].HasToLookAt = false;
                actifSpawnerScript[i].NbrOfShoot = 0;
                yield return StartCoroutine(EntranceAndExit(startAndEndCurve[1], i, actifSpawner[i]));
                actifSpawnerParent[i].gameObject.SetActive(false);
                actifSpawnerScript[i].pivotPoint.gameObject.SetActive(false);
                actifSpawnerScript[i].HasToLookAt = true;
            }
        }
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
}
