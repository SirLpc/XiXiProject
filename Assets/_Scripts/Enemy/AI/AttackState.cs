using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class AttackState : IEnemyState
{
    private readonly StatePatternEnemy enemy;

    private float lastAttackTime = 0;

    public float LastAttackTime { get { return lastAttackTime; } }

    public AttackState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
        lastAttackTime = enemy.AttackInterval;
    }

    public void ToAlertState()
    {
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        enemy.anim.SetBool(Consts.AniIsChase, true);
        enemy.anim.SetBool(Consts.AniIsInAttack, false);
    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        enemy.anim.SetBool(Consts.AniIsChase, false);
        enemy.anim.SetBool(Consts.AniIsInAttack, false);
    }

    public void ToAttackState()
    {
        Debug.Log("Can't transition to attack state");
    }

    public void ToSAHurtState()
    {
        enemy.anim.SetTrigger(Consts.AniTriggerSAHurt);
        enemy.currentState = enemy.saHurtState;
    }

    public void UpdateState()
    {
        if (enemy.IsPlayingAttack() || enemy.IsInDefenseHurt)
            return;
        Look();
        Attack();
    }



    private void Attack()
    {
        if (Time.time - lastAttackTime < enemy.AttackInterval)
            return;

        lastAttackTime = Time.time;
        //这只是个临时解决方案，不知道为什么，会在脱攻距离后，多攻击两下
        if (CheckDistanceToChase())
        {
            ToChaseState();
            return;
        }

        if (enemy.IsPlayingHurt())
        {
            return;
        }

        var ran = Random.Range(0, 3);
        enemy.anim.SetTrigger(ran == 0 ? Consts.AniTriggerAttack2 : Consts.AniTriggerAttack);
    }

    private void Look()
    {
        enemy.meshRendererFlag.material.color = Color.magenta;

        if (CheckDistanceToChase())
        {
            ToChaseState();
            return;
        }
        Vector3 dir = new Vector3(
            enemy.chaseTarget.position.x,
            enemy._myTransform.position.y,
            enemy.chaseTarget.position.z);
        enemy.transform.LookAt(dir);
    }

    public void OnAttackComplete()
    {
        if (CheckDistanceToChase())
        {
            ToChaseState();
            return;
        }

        if (!PlayerController.Instance.IsInDefense)
        {
            //todo set damage num
            PlayerController.Instance.DamangeHandler.Damage(enemy.AttackNum);

            //在这里变成非追捕状态，是为了防止攻击之后在间隔中变成idel状态，又自动从idel变成了追捕
            //enemy.anim.SetBool(Consts.AniIsChase, false);
        }
        else
        {
            enemy.DamageHandler.DefenseDamage();
            enemy.anim.SetBool(Consts.AniIsChase, true);
        }
    }

    private bool CheckDistanceToChase()
    {
        float distance = Vector3.Distance(enemy.transform.position, enemy.chaseTarget.transform.position);
        if (distance > enemy.navMeshAgent.stoppingDistance + 1f)
        {
            return true;
        }
        return false;
    }


}
