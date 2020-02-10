using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using EnemyStateEnum;
using StalactiteStateEnum;

public class StalactiteStats : EnemyStats {

    public int fireDamageMutliplicater;

    StalactiteController controller;


    public override void Start()
    {
        base.Start();
        controller = GetComponent<StalactiteController>();
    }

    public override void Die()
    {
        //checkiIfItIsDead = true;
        DestroyAllMarks();
        if (!_bigBossFight)
        {
            
            controller.OnBeKilled();
        }
    }
    /*public override void AutoAttackFireMark(float timerDebuf)
    {
        if (FireMarkCount == 0)
        {
            FireMarkCount++;
        }
        if (!FireHasBeenInstanciated)
        {
            if (!_bigBossFight)
            {
                MarqueDeFeu = InstantiateMarks(MarqueFeu, DebufRoot);
                FireMarkPos = CheckPosition(ArcaneHasBeenInstanciated, IceHasBeenInstanciated);
            }

            FireHasBeenInstanciated = true;
        }
        StartFireCooldown = true;
        TimerFire = SaveTimerFire = timerDebuf;
    }*/
    public override void FireTrail()
    {
        TimeBetweenFireTrailTicks -= Time.deltaTime;
        if (TimeBetweenFireTrailTicks <= 0)
        {
            if (controller.StalactiteState == StalactiteState.Fusion)
            {
                TakeDamage(DamageFireTrailTicks * fireDamageMutliplicater);
            }
            else
            {
                TakeDamage(DamageFireTrailTicks);
            }
            TimeBetweenFireTrailTicks = PlayerManager.Instance.m_powers.m_fireTrail.m_fireTrailTick;
        }
    }
    public override void FireMark(float timerDebuf)
    {
        if (FireMarkCount <= 2)
        {
            FireMarkCount++;
        }
        if (!FireHasBeenInstanciated)
        {
            if (!_bigBossFight)
            {
                MarqueDeFeu = InstantiateMarks(MarqueFeu, DebufRoot);
                FireMarkPos = CheckPosition(ArcaneHasBeenInstanciated, IceHasBeenInstanciated);
            }

            FireHasBeenInstanciated = true;
        }
        StartFireCooldown = true;
        TimerFire = SaveTimerFire = timerDebuf;

        if (FireMarkCount == 2)
        {
            if (!_bigBossFight)
            {
                Destroy(MarqueDeFeu);
                IceMarkPos = CheckPosition(ArcaneHasBeenInstanciated, FireHasBeenInstanciated);
                ArcanMarkPos = CheckPosition(FireHasBeenInstanciated, IceHasBeenInstanciated);
                Level.AddFX(controller.m_fxs.m_markExplosion, controller.m_fxs.m_markExplosionRoot.position, controller.m_fxs.m_markExplosionRoot.rotation);
            }
            FireHasBeenInstanciated = false;
            FireMarkCount = 0;
            TimerTickDamage = saveDamageTick;
            StartFireCooldown = false;

            if(controller.StalactiteState == StalactiteState.Fusion)
            {
                TakeDamage(FireExplosionDamage * fireDamageMutliplicater);
            }
            else
            {
                TakeDamage(FireExplosionDamage);
            }
        }
    }

    public override void OnStartFireCooldown()
    {
        if (StartFireCooldown)
        {
            TimerFire -= Time.deltaTime;
            if (MarqueDeFeu != null)
            {
                MarqueDeFeu.GetComponent<ReferenceScript>().marksArray[1].fillAmount = Mathf.InverseLerp(0, SaveTimerFire, TimerFire);
            }
            TimerTickDamage -= Time.deltaTime;
            if (TimerFire <= 0)
            {
                if (!_bigBossFight)
                {
                    Destroy(MarqueDeFeu);
                    IceMarkPos = CheckPosition(ArcaneHasBeenInstanciated, FireHasBeenInstanciated);
                    ArcanMarkPos = CheckPosition(FireHasBeenInstanciated, IceHasBeenInstanciated);
                }

                FireHasBeenInstanciated = false;
                FireMarkCount = 0;
                TimerTickDamage = saveDamageTick;
                StartFireCooldown = false;

            }
            else if (TimerTickDamage <= 0)
            {
                TimerTickDamage = saveDamageTick;
                if (controller.StalactiteState == StalactiteState.Fusion)
                {
                    TakeDamage(FireTickDamage * fireDamageMutliplicater);
                }
                else
                {
                    TakeDamage(FireTickDamage);
                }
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        StartHitFxCorout();
        
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        if (CurrentHealth > 0)
        {
            CurrentHealth -= damage;
        }

        CheckIfHasToDie(CurrentHealth);

        if (PlayerManager.Instance.GetComponent<PlayerStats>().IsInCombat && isActiveAndEnabled && !_bigBossFight)
        {
            m_canvas.SetActive(true);
        }
        if (!_bigBossFight)
        {
            if (m_canvas.activeSelf)
            {
                timeBeforeLifeBarOff -= Time.deltaTime;
                if (timeBeforeLifeBarOff <= 0)
                {
                    m_canvas.SetActive(false);
                    timeBeforeLifeBarOff = saveTimeBeforeLifeBarOff;
                }
            }
            slider.fillAmount = Mathf.InverseLerp(0, maxHealth, CurrentHealth);
        }

        m_timeToDecreaseWhiteLifeBar = BigEnemyLifeBarManager.Instance.m_timeForWhiteLifeBarToDecrease;
        HasTakenDamage = true;
    }
}
