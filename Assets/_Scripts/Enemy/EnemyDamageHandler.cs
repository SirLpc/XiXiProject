using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyDamageHandler : vp_DamageHandler
{
    private StatePatternEnemy enemy;

    private void Awake()
    {
        enemy = GetComponent<StatePatternEnemy>();
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
        enemy.anim.SetTrigger(Consts.AniTriggerHurt);

        if (m_CurrentHealth <= 0.0f)
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

        RemoveBulletHoles();

        vp_Utility.Activate(gameObject, false);

        foreach (GameObject o in DeathSpawnObjects)
        {
            if (o != null)
                vp_Utility.Instantiate(o, transform.position, transform.rotation);
        }

    }

    private IEnumerator CoDownOut()
    {
        yield return new WaitForSeconds(0.28f);
    }
         
    //测试代码
    private void OnGUI()
    {
        GUILayout.Label("enemy hp:"+m_CurrentHealth);
    }


}
