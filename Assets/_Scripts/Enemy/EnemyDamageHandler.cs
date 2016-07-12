using UnityEngine;
using System.Collections.Generic;

public class EnemyDamageHandler : vp_DamageHandler
{

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

        if (m_CurrentHealth <= 0.0f)
            vp_Timer.In(UnityEngine.Random.Range(MinDeathDelay, MaxDeathDelay), delegate ()
            {
                SendMessage("Die");     // picked up by vp_DamageHandlers and vp_Respawners
            });


        // TIP: if you want to do things like play a special impact
        // sound upon every hit (but only if the object survives)
        // this is the place

    }








}
