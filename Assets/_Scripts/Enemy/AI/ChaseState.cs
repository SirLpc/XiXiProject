using UnityEngine;
using System.Collections;

public class ChaseState : IEnemyState

{

    private readonly StatePatternEnemy enemy;

    private float lastAttackTime;


    public ChaseState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
        Chase();
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void ToPatrolState()
    {

    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
        enemy.anim.SetBool(Consts.AniIsChase, false);
    }

    public void ToChaseState()
    {

    }

    public void ToAttackState()
    {
        //if (Time.time - lastAttackTime < enemy.AttackInterval)
            //return;

        lastAttackTime = Time.time;
        enemy.currentState = enemy.attackState;
        enemy.anim.SetBool(Consts.AniIsInAttack, true);
        enemy.anim.SetBool(Consts.AniIsChase, false);
    }

    private void Look()
    {
        RaycastHit hit;
        Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;
        Debug.DrawRay(enemy.eyes.transform.position, enemyToTarget, Color.red,0.5f);
        if (Physics.Raycast(enemy.eyes.transform.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
        }
        else
        {
            ToAlertState();
        }

    }

    private void Chase()
    {
        enemy.meshRendererFlag.material.color = Color.red;
        enemy.navMeshAgent.destination = enemy.chaseTarget.position;

        enemy.navMeshAgent.Resume();

        if (enemy.navMeshAgent.remainingDistance < enemy.navMeshAgent.stoppingDistance)
        {
            ToAttackState();
            return;
        }

    }

}