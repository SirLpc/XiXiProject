using UnityEngine;
using System.Collections;

public class PatrolState : IEnemyState

{
    private readonly StatePatternEnemy enemy;
    private int nextWayPoint;

    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
        Patrol();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("on trigger enter fired");
            ToAlertState();
        }
    }

    public void ToPatrolState()
    {
        Debug.Log("Can't transition to same state");
    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
        enemy.anim.SetBool(Consts.AniIsChase, false);
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        enemy.anim.SetBool(Consts.AniIsChase, true);
    }

    public void ToAttackState()
    {

    }

    public void ToSAHurtState()
    {
        enemy.anim.SetTrigger(Consts.AniTriggerSAHurt);
        enemy.currentState = enemy.saHurtState;
    }

    private void Look()
    {
        RaycastHit hit;
        //Debug.DrawRay(enemy.eyes.transform.position, enemy.eyes.transform.forward * enemy.sightRange, Color.blue,0.5f );
        if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            ToChaseState();
        }
    }

    void Patrol()
    {
        enemy.meshRendererFlag.material.color = Color.green;
        enemy.navMeshAgent.destination = enemy.wayPoints[nextWayPoint].position;
        enemy.navMeshAgent.Resume();

        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending)
        {
            nextWayPoint = (nextWayPoint + 1) % enemy.wayPoints.Length;

        }
     }
}