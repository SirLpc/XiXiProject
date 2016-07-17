using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyDamageHandler : vp_DamageHandler
{
    [SerializeField] private float _fadeDownOutSpeed = 3f;
    [SerializeField] private float _lieOnGroundTime = 2f;
    [SerializeField] private float _defenseBackSpeed = 3f;

    private StatePatternEnemy _enemy;
    private Collider[] _colliders;
    public bool IsInDefenseHurt { get; private set; }


    private void Awake()
    {
        _enemy = GetComponent<StatePatternEnemy>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    /// <summary>
    /// reduces current health by 'damage' points and kills the
    /// object if health runs out
    /// </summary>
    public override void Damage(float damage)
    {

        if (!enabled)
            return;

        if (!vp_Utility.IsActive(gameObject))
            return;

        if (m_CurrentHealth <= 0.0f)
            return;

        m_CurrentHealth = Mathf.Min(m_CurrentHealth - damage, MaxHealth);

        if (m_CurrentHealth > 0.0f)
        {
            if(_enemy.CanPlayHurtAnim())
            {
                if (_enemy.currentState == _enemy.attackState)
                    _enemy.anim.SetTrigger(Consts.AniTriggerDefenseHurt);
                else
                {
                    _enemy.anim.SetTrigger(Consts.AniTriggerHurt);
                }
            }
        }
        else
            vp_Timer.In(UnityEngine.Random.Range(MinDeathDelay, MaxDeathDelay), delegate ()
            {
                SendMessage("Die");     // picked up by vp_DamageHandlers and vp_Respawners
            });


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

        _enemy.IsAlive = false;
        _enemy.navMeshAgent.Stop();
        _enemy.navMeshAgent.enabled = false;
        _enemy.anim.SetTrigger(Consts.AniTriggerDie);

        StartCoroutine(CoFadeDownOutAndRecycle());
    }

    public void DefenseDamage()
    {
        IsInDefenseHurt = true;
        _enemy.anim.SetTrigger(Consts.AniTriggerDefenseHurt);
        StartCoroutine(CoBeenDefenseBack());
    }

    private IEnumerator CoBeenDefenseBack()
    {
        var t = 0f;
        while (t < Consts.AniDefenseHurtDuration)
        {
            t += Time.deltaTime;
            yield return null;
            transform.Translate(Vector3.back * Time.deltaTime * _defenseBackSpeed);
        }

        IsInDefenseHurt = false;
    }

    private IEnumerator CoFadeDownOutAndRecycle()
    {
        yield return new WaitForSeconds(Consts.AniDieDuration);
        _enemy.anim.enabled = false;
        _enemy.enabled = false;

        yield return  new WaitForSeconds(_lieOnGroundTime);

        var timer = 0.0f;
        while (timer < 5f)
        {
            yield return null;
            timer += Time.deltaTime;
            transform.localPosition += Vector3.down * Time.deltaTime * _fadeDownOutSpeed;
        }

        RemoveBulletHoles();

        vp_Utility.Activate(gameObject, false);

        EnableAllColliders(true);

        foreach (GameObject o in DeathSpawnObjects)
        {
            if (o != null)
                vp_Utility.Instantiate(o, transform.position, transform.rotation);
        }
    }

    private void EnableAllColliders(bool enabled)
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = enabled;
        }
    }

    //测试代码
    private void OnGUI()
    {
        GUILayout.Label("enemy hp:"+m_CurrentHealth);
    }


}
