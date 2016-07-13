using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class AttackState : IEnemyState
{
    private readonly StatePatternEnemy enemy;

    private float lastAttackTime = 0;

    public AttackState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
        lastAttackTime = enemy.AttackInterval;
    }

    public void OnTriggerEnter(Collider other)
    {
    }

    public void ToAlertState()
    {
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        enemy.anim.SetBool(Consts.AniIsChase, true);
        lastAttackTime = enemy.AttackInterval;
    }

    public void ToPatrolState()
    {
    }

    public void ToAttackState()
    {
        Debug.Log("Can't transition to attack state");
    }

    public void UpdateState()
    {
        Look();
        Attack();
    }



    private void Attack()
    {
        if (Time.time - lastAttackTime < enemy.AttackInterval)
            return;

        lastAttackTime = Time.time;
        //这只是个临时解决方案，不知道为什么，会在脱攻距离后，多攻击两下
        if (Vector3.Distance(enemy.transform.position, enemy.chaseTarget.transform.position) > enemy.navMeshAgent.stoppingDistance)
        {
            ToChaseState();
            return;
        }
        var ran = Random.Range(0, 3);
        enemy.anim.SetTrigger(ran == 0 ? Consts.AniTriggerAttack2 : Consts.AniTriggerAttack);

    }

    private void Look()
    {
        if (Vector3.Distance(enemy.transform.position, enemy.chaseTarget.transform.position) > enemy.navMeshAgent.stoppingDistance)
        {
            ToChaseState();
            return;
        }
        enemy.transform.LookAt(enemy.chaseTarget);
    }

    public void OnAttackComplete()
    {
        if (Vector3.Distance(enemy.transform.position, enemy.chaseTarget.transform.position) > enemy.navMeshAgent.stoppingDistance)
        {
            ToChaseState();
            return;
        }

        //todo set damage num
        PlayerController.Instance.DamangeHandler.Damage(enemy.AttackNum);

        //在这里变成非追捕状态，是为了防止攻击之后在间隔中变成idel状态，又自动从idel变成了追捕
        enemy.anim.SetBool(Consts.AniIsChase, false);
    }
}
