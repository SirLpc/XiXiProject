using UnityEngine;
using System.Collections;
using System;

public class BossDamageHandler : vp_DamageHandler
{
    [SerializeField] private float _hitDamage;

    private BossCtr _bossCtr;
    private const float BossMaxHealth = 8000;
    private static float BossCurrentHealth = BossMaxHealth;

    private void Awake()
    {
        _bossCtr = GetComponentInParent<BossCtr>();
    }

    /// <summary>
    /// reduces current health by 'damage' points and kills the
    /// object if health runs out
    /// </summary>
    public override void Damage(float damage)
    {

        if (_bossCtr.CurState == BossState.INACTIVE)
            return;

        if (!vp_Utility.IsActive(gameObject))
            return;

        if (BossCurrentHealth <= 0.0f)
            return;

        BossCurrentHealth = Mathf.Min(BossCurrentHealth - _hitDamage, BossMaxHealth);

        _bossCtr.UICtr.SetHP(BossCurrentHealth, BossMaxHealth);

        if (BossCurrentHealth <= 0.0f)
        {
            vp_Timer.In(UnityEngine.Random.Range(MinDeathDelay, MaxDeathDelay), delegate()
            {
                SendMessage("Die"); // picked up by vp_DamageHandlers and vp_Respawners
            });
        }

        // TIP: if you want to do things like play a special impact
        // sound upon every hit (but only if the object survives)
        // this is the place

    }

    /// <summary>
    /// removes the object, plays the death effect and schedules
    /// a respawn if enabled, otherwise destroys the object
    /// </summary>
    public override void Die()
    {
        if (_bossCtr.CurState == BossState.INACTIVE)
            return;

        if (!enabled || !vp_Utility.IsActive(gameObject))
            return;

        if (m_Audio != null)
        {
            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot(DeathSound);
        }

        _bossCtr.Anim.SetTrigger(Consts.AniBossTriggerDie);
        _bossCtr.DisActiveBoss();
        StartCoroutine(CoFadeOut());
    }

    private IEnumerator CoFadeOut()
    {
        yield return new WaitForSeconds(5f);
        UIViewCtr.Instance.FadeOut(OnFadeOutComplete);
    }

    private void OnFadeOutComplete()
    {
        UIViewCtr.Instance.ShowIcon();
        var enemies = GameObject.FindGameObjectsWithTag(Consts.EnemyTag);
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }
}
