using UnityEngine;
using System.Collections;
using System;

public class ZhiZhuDamageHandler : vp_DamageHandler
{
    private ZhiZhuCtr _zzCtr;
    //private BossCtr _bossCtr;
    private float _takeDamage = 10;
    private float _lieOnGroundTime = 2;
    private float _fadeDownOutSpeed = 3;
    private Collider[] _colliders;

    private void Awake()
    {
        _zzCtr = GetComponent<ZhiZhuCtr>();
        _colliders = GetComponentsInChildren<Collider>();
        //var boss = GameObject.FindWithTag(Consts.BossTag);
        //if (boss != null)
        //    _bossCtr = boss.GetComponentInParent<BossCtr>();
        //else
        //    Destroy(this.gameObject);
    }

    /// <summary>
    /// reduces current health by 'damage' points and kills the
    /// object if health runs out
    /// </summary>
    public override void Damage(float damage)
    {
        //if (!vp_Utility.IsActive(gameObject))
        //    return;

        if (m_CurrentHealth <= 0.0f)
            return;

        m_CurrentHealth = Mathf.Min(m_CurrentHealth - _takeDamage, MaxHealth);
        Debug.Log("damaged" + _takeDamage);

        if (m_CurrentHealth <= 0.0f)
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
        if (!enabled || !vp_Utility.IsActive(gameObject))
            return;

        if (m_Audio != null)
        {
            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot(DeathSound);
        }

        EnableAllColliders(false);
        _zzCtr.Anim.SetTrigger(Consts.AniTriggerDie);
        BossCtr.CurZhiZhuNum--;

        StartCoroutine(CoFadeDownOutAndRecycle());
    }

    private IEnumerator CoFadeDownOutAndRecycle()
    {
        _zzCtr.enabled = false;
        yield return new WaitForSeconds(Consts.AniDieDuration);
        _zzCtr.Anim.enabled = false;

        yield return new WaitForSeconds(_lieOnGroundTime);

        var timer = 0.0f;
        while (timer < 5f)
        {
            yield return null;
            timer += Time.deltaTime;
            transform.localPosition += Vector3.down * Time.deltaTime * _fadeDownOutSpeed;
        }

        RemoveBulletHoles();

        vp_Utility.Activate(gameObject, false);

        //EnableAllColliders(true);

        foreach (GameObject o in DeathSpawnObjects)
        {
            if (o != null)
                vp_Utility.Instantiate(o, transform.position, transform.rotation);
        }

        Destroy(this.gameObject);
    }

    private void EnableAllColliders(bool enabled)
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = enabled;
        }
    }


    //todo kill测试代码
    private void OnGUI()
    {
        GUILayout.Label("enemy hp:" + m_CurrentHealth);
    }
}
