using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaBeamController : BossAttack
{
    public GameObject[] laserSpanwerPhase3;
    public GameObject[] laserSpanwerPhase2;
    [Space]
    [Header("Lava Beam Var")]
    public Transform target;
    public float timeBeforeShooting;
    public float timeBetweenEachShoot;
    float _currentTimeBeforeShooting;
    public int nbrOfShot=1;
    public int damage;
    [Space]
    [Header("VFX")]
    public GameObject signActifSpawner;
    public GameObject lavaBeamVFX;
    [Space]
    [Header("DEBUG")]
    [Range(1,3)]
    public int phaseNbr;
    public float offset = 89f;

    bool lavaBeamPaternActif;
    bool allShootingHaveBeenDone = true;
    List<GameObject> actifSpawner = new List<GameObject>();
    List<Beam_Spawner_Controller> actifSpawnerScript = new List<Beam_Spawner_Controller>();
    List<GameObject> actifSpawnerParent = new List<GameObject>();


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
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
                RaycastHit hit;
                if(Physics.Raycast(actifSpawner[i].transform.position, actifSpawner[i].transform.forward, out hit, Mathf.Infinity, mask))
                {
                    /*float fill = Mathf.InverseLerp(actifSpawner[i].transform.position.z, target.position.z, hit.point.z);
                    actifSpawnerScript[i].sign.fillAmount = fill;*/
                    Debug.DrawLine(actifSpawner[i].transform.position, target.position, Color.green);
                }
                /*if (Physics.Linecast(actifSpawner[i].transform.position, target.position, mask, QueryTriggerInteraction.Collide))
                {
                }*/
                else
                {
                    actifSpawnerScript[i].sign.fillAmount = 1;
                    Debug.DrawLine(actifSpawner[i].transform.position, target.position, Color.red);
                }

                if (actifSpawnerScript[i].HasToLookAt)
                {
                    actifSpawner[i].transform.LookAt(target);
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

            scrpitRef.pivotPoint.gameObject.SetActive(true);
            laserSpanwerPhase[i].SetActive(true);

            actifSpawnerParent.Add(laserSpanwerPhase[i]);
            actifSpawner.Add(go);
            actifSpawnerScript.Add(scrpitRef);

            if(nbrOfPhase == 1)
            {
                break;
            }
        }
    }


    IEnumerator WaitUntilShooting()
    {
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
            Level.AddFX(lavaBeamVFX, actifSpawner[i].transform.position, actifSpawner[i].transform.rotation);
            actifSpawnerScript[i].NbrOfShoot++;
            yield return new WaitForSeconds(timeBetweenEachShoot/2);
            actifSpawnerScript[i].HasToLookAt = true;
            if(actifSpawnerScript[i].NbrOfShoot == nbrOfShot)
            {
                actifSpawnerScript[i].NbrOfShoot = 0;
                actifSpawnerParent[i].gameObject.SetActive(false);
                actifSpawnerScript[i].pivotPoint.gameObject.SetActive(false);
                actifSpawnerScript[i].HasToLookAt = true;
            }
        }
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
