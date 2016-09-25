using UnityEngine;
using System.Collections;

public class InsectDamageHandler : vp_DamageHandler
{
    private InsectCtr _insectCtr;
    private float _lieOnGroundTime = 2;
    private float _fadeDownOutSpeed = 3;
    private Collider[] _colliders;

    void Awake ()
    {
        _insectCtr = GetComponent<InsectCtr>();
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
            if (PlayerController.Instance.SpecialAttackEffectived)
                TakeSpecialHurt();
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

    private void TakeSpecialHurt()
    {
        _insectCtr.ToSAHurt();
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

        _insectCtr.SetDie();
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
