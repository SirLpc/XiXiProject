using UnityEngine;
using System.Collections;

public class ChaseState : IEnemyState

{

    private readonly StatePatternEnemy enemy;
    //private float lastAttackTime;

    public ChaseState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        //Now always chase once discovered player until out of boundary.
        //Look();
        Chase();
    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        enemy.anim.SetBool(Consts.AniIsChase, false);
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
        //    return;
        //lastAttackTime = Time.time;

        enemy.anim.SetBool(Consts.AniIsChase, false);
        enemy.anim.SetBool(Consts.AniIsInAttack, true);
        enemy.currentState = enemy.attackState;
    }

    public void ToSAHurtState()
    {
        enemy.anim.SetBool(Consts.AniIsChase, false);
        enemy.anim.SetTrigger(Consts.AniTriggerSAHurt);
        enemy.currentState = enemy.saHurtState;
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