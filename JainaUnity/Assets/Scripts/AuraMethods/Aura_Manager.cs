using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura_Manager : MonoBehaviour
{
    [Header("Aura Bonus")]
    public int _bonusDamage;
    [Range(1,30)]
    public float _radius;
    [Space]
    Collider col;
    List<Collider> myList = new List<Collider>();
    //public int _bonusAttackSpeed;

    public void Start()
    {
        col = GetComponentInParent<Collider>();
        GetComponentInParent<EnemyStats>().m_auraSign.SetActive(true);
        StartCoroutine(ControledOverLap());
    }

    #region Aura Methods

    IEnumerator ControledOverLap()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Collider[] hitCollider = Physics.OverlapSphere(transform.position, _radius);
            for (int i = 0; i < hitCollider.Length; i++)
            {
                if (hitCollider[i].CompareTag("Enemy"))
                {
                    if (hitCollider[i] != col)
                    {
                        myList.Add(hitCollider[i]);
                        AuraMethod(hitCollider[i], 1, true);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0,l = myList.Count; i < l; ++i)
        {
            float dist = Vector3.Distance(transform.position, myList[i].transform.position);
            if(dist > _radius)
            {
                AuraMethod(myList[i], -1, false);
            }
            else
            {
                AuraMethod(myList[i], 1, true);
            }
        }
    }
    void AuraMethod(Collider other, int i, bool AuraOn)
    {
        EnemyStats stats = other.GetComponentInParent<EnemyStats>();
        EnemyController control = other.GetComponentInParent<EnemyController>();
        if(!stats.m_auraSign.activeSelf && AuraOn || stats.m_auraSign.activeSelf && !AuraOn)
        {
            stats.damage.baseValue = stats.damage.baseValue + _bonusDamage * i;
        }
        stats.m_auraSign.SetActive(AuraOn);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

}
