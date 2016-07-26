using UnityEngine;
using System.Collections;

public class SAHurtState : IEnemyState
{

    private readonly StatePatternEnemy enemy;

    public SAHurtState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Hold();
    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        enemy.anim.SetBool(Consts.AniIsChase, false);
        enemy.anim.SetBool(Consts.AniIsInAttack, false);
    }

    public void ToAlertState()
    {

    }

    public void ToChaseState()
    {
        enemy.anim.SetBool(Consts.AniIsChase, true);
        enemy.anim.SetBool(Consts.AniIsInAttack, false);
        
        enemy.currentState = enemy.chaseState;
    }


    public void ToAttackState()
    {

    }

    public void ToSAHurtState()
    {
        Debug.Log("can not from sahurtstate to sahurtstate");
    }

    private void Hold()
    {
        enemy.meshRendererFlag.material.color = Color.black;

        var info = enemy.anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("BaseLayer.hurt2"))
            return;
        if(info.normalizedTime >= 0.99)
        {
            ToChaseState();
        }
    }
  
}