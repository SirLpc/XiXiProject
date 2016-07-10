using UnityEngine;
using System.Collections;
using System;

public class AttackState : IEnemyState
{
    private readonly StatePatternEnemy enemy;

    public AttackState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void OnTriggerEnter(Collider other)
    {
    }

    public void ToAlertState()
    {
    }

    public void ToChaseState()
    {
    }

    public void ToPatrolState()
    {
    }

    public void ToAttackState()
    {
        Debug.Log("Can't transition to attack state");
    }

    private float attackTimer = 0;
    public void UpdateState()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer > 0.28f)
        {
            attackTimer = 0;
            Attack();
        }
    }

    private void Attack()
    {
        enemy.anim.SetTrigger("triggerAttack");
    }
}
